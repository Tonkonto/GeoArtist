
rem base clean
dotnet clean

rem clean out proj files
rd /s /q GeoComponent\bin
rd /s /q GeoComponent\obj

rd /s /q WebView\bin
rd /s /q WebView\obj

rem restore, rebuild
dotnet restore
dotnet build