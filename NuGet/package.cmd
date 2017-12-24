echo off
rmdir /S /Q lib
mkdir lib
cd lib
mkdir netstandard2.0
cd ..
copy /Y ..\Source\Nuits.Interception.Fody\bin\Release\netstandard2.0\Nuits.Interception.Fody.dll .
copy /Y ..\Source\Nuits.Interception.Fody\bin\Release\netstandard2.0\Nuits.Interception.Fody.pdb .
copy /Y ..\Source\Nuits.Interception\bin\Release\netstandard2.0\Nuits.Interception.dll lib/netstandard2.0
copy /Y ..\Source\Nuits.Interception\bin\Release\netstandard2.0\Nuits.Interception.pdb lib/netstandard2.0
nuget.exe pack Nuits.Interception.Fody.nuspec -Exclude nuget.exe -Exclude package.cmd