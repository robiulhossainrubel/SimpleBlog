@echo off
setlocal

set Report_Folder_Name=UnitTestResult
set Report_File_Name=coverage
set DotNet_Path=dotnet
set Test_Project=ServiceTest

echo.
echo === Building the solution ===
%DotNet_Path% build SimpleBlog.sln
if %errorlevel% neq 0 (
    echo Build failed!
    exit /b 1
)

echo.
echo === Creating report folder if it does not exist ===
if exist "%Report_Folder_Name%" (
    echo Folder "%Report_Folder_Name%" already exists.
) else (
    mkdir "%Report_Folder_Name%"
    echo Folder "%Report_Folder_Name%" created.
)

echo.
echo === Running unit tests with code coverage ===
%DotNet_Path% test %Test_Project% --no-build --collect:"XPlat Code Coverage" --results-directory "%Report_Folder_Name%"
if %errorlevel% neq 0 (
    echo Unit tests failed!
    exit /b 1
)

echo.
echo === Checking for coverage report ===
REM Find cobertura file recursively
for /r "%Report_Folder_Name%" %%f in (coverage.cobertura.xml) do (
    set "CoverageFile=%%f"
)

if not defined CoverageFile (
    echo Coverage file not found. Test coverage data may be missing.
    exit /b 1
)

echo Found coverage file: "%CoverageFile%"

echo.
echo === Installing ReportGenerator if not already installed ===
%DotNet_Path% tool list -g | findstr /i "reportgenerator" >nul
if %errorlevel% neq 0 (
    echo Installing ReportGenerator...
    %DotNet_Path% tool install -g dotnet-reportgenerator-globaltool
)

set PATH=%USERPROFILE%\.dotnet\tools;%PATH%

echo.
echo === Generating HTML coverage report ===
reportgenerator -reports:"%CoverageFile%" -targetdir:"%Report_Folder_Name%" -reporttypes:Html;HtmlSummary
if %errorlevel% neq 0 (
    echo Failed to generate report.
    exit /b 1
)

echo.
echo === Attempting to open report in browser ===
if exist "%Report_Folder_Name%\index.html" (
    start "" "%cd%\%Report_Folder_Name%\index.html"
) else (
    echo Report file not found: "%Report_Folder_Name%\index.html"
    dir "%Report_Folder_Name%"
)

endlocal
