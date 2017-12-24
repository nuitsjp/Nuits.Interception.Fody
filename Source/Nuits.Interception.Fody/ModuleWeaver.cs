﻿using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Nuits.Interception;
using Nuits.Interception.Fody;
using MethodAttributes = Mono.Cecil.MethodAttributes;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }

    public void Execute()
    {
        var methods =
            ModuleDefinition.Types.SelectMany(
                x => x.Methods.Where(
                    method => method.CustomAttributes.Any(
                        attribute => attribute.AttributeType.FullName == typeof(InterceptAttribute).FullName))).Distinct().ToList();
        foreach (var methodDefinition in methods)
        {
            var originalName = methodDefinition.Name;
            var type = methodDefinition.DeclaringType;

            methodDefinition.Name = methodDefinition.Name + "Inner";
            methodDefinition.Attributes &= ~MethodAttributes.Public;
            methodDefinition.Attributes |= MethodAttributes.Private;


            var innerInvoker = InnerInvoker.Create(ModuleDefinition, type, methodDefinition);
            var targetMethod = CreateAddMethod(type, methodDefinition, innerInvoker, originalName);

            type.Methods.Add(targetMethod);
            type.NestedTypes.Add(innerInvoker.TypeDefinition);

            //CreateGetInterceptAttribute(type, methodDefinition, innerInvoker);
        }
    }

    private MethodDefinition CreateAddMethod(TypeDefinition type, MethodDefinition originalMethod, InnerInvoker innerInvoker, string originalName)
    {
        var targetMethod =
            new MethodDefinition(originalName, MethodAttributes.Public | MethodAttributes.HideBySig, type)
            {
                Body =
                {
                        MaxStackSize = originalMethod.Parameters.Count + 1,
                        InitLocals = true
                },
                ReturnType = originalMethod.ReturnType
            };
        // Add Parameter
        foreach (var parameter in originalMethod.Parameters)
        {
            var newParameter = new ParameterDefinition(parameter.Name, parameter.Attributes, parameter.ParameterType);
            targetMethod.Parameters.Add(newParameter);
        }

        targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldtoken, originalMethod));
        var getMethodFromHandle =
            typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });
        targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Call, ModuleDefinition.ImportReference(getMethodFromHandle)));

        // InterceptAttribute interceptorAttribute = ((MemberInfo)methodInfo).GetCustomAttribute<InterceptAttribute>();
        var getCustomAttribute = typeof(CustomAttributeExtensions).GetMethods()
            .Where(x => x.Name == "GetCustomAttribute" && x.GetGenericArguments().Length == 1)
            .Single(x =>
            {
                var parameters = x.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType == typeof(MemberInfo);
            }).MakeGenericMethod(typeof(InterceptAttribute));
        targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Call, ModuleDefinition.ImportReference(getCustomAttribute)));

        // interceptorAttribute.InterceptorTypes
        var get_InterceptorTypes = typeof(InterceptAttribute).GetTypeInfo().GetMethod("get_InterceptorTypes");
        targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Call, ModuleDefinition.ImportReference(get_InterceptorTypes)));

        // new AddInvocation
        var innerInvokerConstructor = innerInvoker.TypeDefinition.GetConstructors().Single();
        targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Newobj, innerInvokerConstructor));
        // AddInvocation.Class = this
        targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Dup));
        targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Stfld, innerInvoker.ParentTypeFieldDefinition));
        //  AddInvocation.ValueN = ParamN
        for (var i = 0; i < innerInvoker.ParameterFieldDefinisions.Count; i++)
        {
            targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Dup));
            targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, targetMethod.Parameters[i]));
            targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Stfld, innerInvoker.ParameterFieldDefinisions[i]));
        }
        // invocation.Invoke();
        var invoke = typeof(IInvocation).GetTypeInfo().DeclaredMethods.Single(x => x.Name == "Invoke");
        targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, ModuleDefinition.ImportReference(invoke)));
        if (targetMethod.ReturnType.IsPrimitive)
            targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Unbox_Any, targetMethod.ReturnType));

        targetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        return targetMethod;
    }
}
