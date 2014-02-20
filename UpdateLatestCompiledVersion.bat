@echo off
if not exist "%1" goto notok
cd "%1"
goto ok
:notok
cd "Latest Compiled Version"
:ok
echo Cleaning directory...
rm *.md5> NUL 2>&1
rm *.vshost.*> NUL 2>&1
echo Updating default.cfg...
copy ..\src\PS3DumpChecker\config.xml default.cfg >NUL
echo Updating default.hashlist...
copy ..\src\PS3DumpChecker\hashlist.xml default.hashlist >NUL
..\MD5Gen.exe default.cfg default.hashlist PS3DumpChecker.exe
pause