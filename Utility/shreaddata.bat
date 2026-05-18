@echo off
chcp 65001 > nul
setlocal

:: =================================================================
:: [설정 구역]
:: =================================================================
set "SRC_BASE=C:\git\Empen\SharedData"
set "DST_BASE=C:\git\Twinkle\Twinkle\Assets\0_Scripts_Twinkle\Script\Server"

echo.
echo [시스템 확인] 반복문 없이 직접 복사를 수행합니다.
echo 원본: %SRC_BASE%
echo 대상: %DST_BASE%
echo ---------------------------------------------------

:: =================================================================
:: [실행 구역] 각 폴더를 하나씩 명확하게 처리합니다.
:: =================================================================

call :CopyProcess Dto
call :CopyProcess Models
call :CopyProcess Query
call :CopyProcess Request

echo.
echo ---------------------------------------------------
echo 모든 작업이 완료되었습니다.
pause
exit /b

:: =================================================================
:: [함수 구역] 실제 복사를 수행하는 내부 로직
:: =================================================================
:CopyProcess
set "F_NAME=%~1"
echo.
echo [%F_NAME% 폴더 처리 중...]

:: 1. 원본 폴더 존재 확인
if not exist "%SRC_BASE%\%F_NAME%" (
    echo [ERROR] 원본 폴더를 찾을 수 없습니다! 건너뜁니다.
    echo 경로: "%SRC_BASE%\%F_NAME%"
    goto :eof
)

:: 2. 대상 폴더 생성
if not exist "%DST_BASE%\%F_NAME%" (
    mkdir "%DST_BASE%\%F_NAME%"
)

:: 3. Robocopy 실행
:: /E: 하위폴더 포함, /XO: 최신 파일만, /NFL /NDL: 로그 간소화
robocopy "%SRC_BASE%\%F_NAME%" "%DST_BASE%\%F_NAME%" /E /NFL /NDL /R:1 /W:1

if errorlevel 8 (
    echo [FAIL] 복사 실패
) else (
    echo [OK] 복사 성공
)

goto :eof