@echo off
chcp 65001 >nul
title 한성 H58 드라이버 설치기 빌더
cd /d "%~dp0"

echo ====================================================================
echo   한성컴퓨터 H58 원클릭 드라이버 설치기 빌드 스크립트
echo ====================================================================
echo.

set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist %CSC% (
    echo [오류] C# 컴파일러(csc.exe)를 찾을 수 없습니다: %CSC%
    pause
    exit /b 1
)

echo [1/2] csc.exe를 사용하여 실행 파일(.exe)을 컴파일 중...
%CSC% /nologo /target:exe /win32manifest:"src\app.manifest" /out:"H58_원클릭_드라이버설치.exe" "src\Program.cs"

if %errorlevel% neq 0 (
    echo [실패] 컴파일 중 에러가 발생했습니다.
    pause
    exit /b %errorlevel%
)

echo [2/2] 영문 호환 실행 파일(Install_All_Drivers.exe) 복사 중...
copy /y "H58_원클릭_드라이버설치.exe" "Install_All_Drivers.exe" >nul

echo.
echo [성공] 빌드가 완료되었습니다!
echo 출력 파일: H58_원클릭_드라이버설치.exe / Install_All_Drivers.exe
echo.
pause
