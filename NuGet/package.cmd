echo off
rmdir /S /Q lib
mkdir lib
cd lib
mkdir netstandard1.3
cd ..
copy /Y ..\Source\Nuits.Interception.Fody\bin\Release\netstandard1.3\Nuits.Interception.Fody.dll .
copy /Y ..\Source\Nuits.Interception.Fody\bin\Release\netstandard1.3\Nuits.Interception.Fody.pdb .
copy /Y ..\Source\Nuits.Interception\bin\Release\netstandard1.3\Nuits.Interception.dll .
copy /Y ..\Source\Nuits.Interception\bin\Release\netstandard1.3\Nuits.Interception.pdb .
copy /Y ..\Source\Nuits.Interception\bin\Release\netstandard1.3\Nuits.Interception.dll "lib/netstandard1.3"
copy /Y ..\Source\Nuits.Interception\bin\Release\netstandard1.3\Nuits.Interception.pdb "lib/netstandard1.3"
nuget.exe pack Nuits.Interception.Fody.nuspec -Exclude nuget.exe -Exclude package.cmd