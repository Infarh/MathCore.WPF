@echo off

for %%d in (bin obj) do for /f %%f in ('dir /s /b /d %%d') do rd /s /q "%%f"