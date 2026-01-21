@echo off
:: Запуск процесса публикации пакета NuGet

:: Изменим кодировку терминала на UTF-8 для корректного отображения символов
chcp 65001 > nul

set "script_dir=%~dp0"
if "%script_dir:~-1%"=="\" set "script_dir=%script_dir:~0,-1%"
for %%i in ("%script_dir%") do set "PackageName=%%~nxi"
echo Публикация пакета %PackageName%

:: Получим версию проекта, подставляя %PackageName% в путь к csproj
set "local_version="
set "temp_ver=%TEMP%\%PackageName%_ver.txt"
:: Запускаем dotnet и перенаправляем вывод во временный файл чтобы избежать проблем с синтаксисом командной строки
dotnet run .scripts\xml-xpath.cs "%~dp0\%PackageName%\%PackageName%.csproj" "/Project/PropertyGroup/Version/text()" > "%temp_ver%" 2>nul
if exist "%temp_ver%" (
    set /p local_version=<"%temp_ver%"
    del "%temp_ver%" 2>nul
)
if defined local_version (
    echo Локальная версия: %local_version%
) else (
    echo Не удалось получить версию проекта
    exit /b 1
)

:: Получим версию пакета на сервере через скрипт .scripts\nuget-ver-remote.cs
set "remote_version="
set "temp_remote=%TEMP%\%PackageName%_remote_ver.txt"
:: Вызов скрипта, вывод версии в файл
dotnet run .scripts\nuget-ver-remote.cs "%PackageName%" > "%temp_remote%" 2>nul
if exist "%temp_remote%" (
    set /p remote_version=<"%temp_remote%"
    del "%temp_remote%" 2>nul
)
if defined remote_version (
    echo Серверная версия: %remote_version%
    if "%local_version%"=="%remote_version%" (
        echo Версия на сервере совпадает с локальной, публикация не требуется.
        exit /b 0
    ) else (
        echo Локальная версия отличается от серверной, продолжаем публикацию.
    )
) else (
    echo Не удалось получить версию с сервера, продолжаем публикацию.
)

:: Проверим что в локальном репозитории нет незакоммиченных изменений
rem git status --porcelain > nul
rem if not errorlevel 1 (
rem     echo Есть незакоммиченные изменения. Пожалуйста, закоммитьте их перед публикацией.
rem     pause
rem     exit /b 1
rem )

git pull

:: Выполнение мержа из ветки dev в ветку master
::git checkout master
::git merge dev
::git push origin master
::git checkout dev

:: Сборка и публикация проекта произойдёт автоматически на сервере GitHub Actions

:: Ожидание обновления новой версии на сервер NuGet.org 15 итераций по 20 секунд
dotnet run .scripts\nuget-ver-wait.cs "%PackageName%" "%local_version%" -n 15 -t 20000
:: если errorlevel не 0, дождаться не удалось. Требуется внимание пользователя
if errorlevel 1 (
    echo Не удалось подтвердить публикацию пакета на сервере NuGet.org.
    echo Пожалуйста, проверьте вручную.
    pause
    exit /b 1
)

:: Теперь пройдём по всем зависимостям от данного пакета и вызовем их публикацию
:: для этого перечислим все строки в файле .\.scripts\dependencies.txt

echo.
if not exist ".\.scripts\dependencies.txt" (
    echo Файл зависимостей не найден: .\.scripts\dependencies.txt
    echo Публикация завершена.
    exit /b 0
)

rem тут будет выполнение скрипта ожидания завершения проверки пакета на сервере NuGet.org


for /f "usebackq delims=" %%i in (".\.scripts\dependencies.txt") do (
    rem Проверим что каталог существует
    if not exist "%%i" (
        echo Каталог не найден: %%i
        rem пропустить эту итерацию
    ) else (
        rem Проверим, что в каталоге есть файл publish-nuget.bat
        if not exist "%%i\publish-nuget.bat" (
            echo Файл publish-nuget.bat не найден в каталоге: %%i
            rem пропустить эту итерацию
        ) else (
            echo.
            echo Публикация зависимого пакета: %%i
            pushd "%%i"
            rem Вызов публикации зависимого пакета (раскомментировать при необходимости)
            call publish-nuget.bat
            if errorlevel 1 (
                echo Ошибка при публикации зависимого пакета: %%i
                popd
                exit /b 1
            )
            popd
        )
    )
)

pause
exit /b 0