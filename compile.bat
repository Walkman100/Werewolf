@echo off
set VBC="%WinDir%\Microsoft.NET\Framework64\v4.0.30319\vbc.exe"
set VBCFLAGS=-nologo -quiet -utf8output -rootnamespace:werewolf -target:exe /win32icon:characters-square.ico
set VBFILES=werewolf.vb AssemblyInfo.vb

    %VBC% %VBCFLAGS% -out:werewolf.exe %VBFILES%

if Not ERRORLEVEL==1 goto EOF
    echo VBC command failed, trying again in 32-bit folder...
    set VBC="%WinDir%\Microsoft.NET\Framework\v4.0.30319\vbc.exe"

    %VBC% %VBCFLAGS% -out:werewolf.exe %VBFILES%

:EOF