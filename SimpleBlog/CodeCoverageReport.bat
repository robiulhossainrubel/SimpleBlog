@echo off
setlocal

set Report_Folder_Name=UnitTestResult
set DotNet_Path=dotnet
set Solution_Name=SimpleBlog
set Test_Project_Name=SimpleBlog.Test

echo "Building the solution..."
%DotNet_Path% build %Solution_Name%.sln
if %errorlevel% neq 0 (
    echo Build failed!
    exit /b 1
)

echo "Creating a report folder if it does not exist..."
if exist "%Report_Folder_Name%" (
    echo %Report_Folder_Name% exists
) else (
    mkdir "%Report_Folder_Name%" && echo %Report_Folder_Name% created
)

echo "Running unit tests with coverage..."
%DotNet_Path% test %Test_Project_Name% --no-build --collect:"XPlat Code Coverage" --results-directory:%Report_Folder_Name%
if %errorlevel% neq 0 (
    echo Unit test run failed!
    exit /b 1
)

echo "Installing ReportGenerator if not already installed..."
%DotNet_Path% tool list -g | findstr /i "reportgenerator" >nul
if %errorlevel% neq 0 (
    %DotNet_Path% tool install -g dotnet-reportgenerator-globaltool
)

echo "Generating Code Coverage Report from XML"
set PATH=%USERPROFILE%\.dotnet\tools;%PATH%

reportgenerator -reports:"%Report_Folder_Name%\**\coverage.cobertura.xml" -targetdir:"%Report_Folder_Name%" -reporttypes:Html;HtmlSummary
if %errorlevel% neq 0 (
    echo Report generation failed!
    exit /b 1
)

echo "Opening results in default browser"
start "" "%cd%\%Report_Folder_Name%\index.html"

endlocal
