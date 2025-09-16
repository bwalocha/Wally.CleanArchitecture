@ECHO OFF
SET "NAME=wally.cleanarchitecture"

SET "AUTHOR_NAME=Wally"
SET "AUTHOR_SHORT_NAME=wally"
SET "TEMPLATE_NAME=%AUTHOR_NAME%.CleanArchitecture"
SET "TEMPLATE_SHORT_NAME=%AUTHOR_SHORT_NAME%.cleanarchitecture"

dotnet new install %TEMPLATE_NAME%.Template
git diff --exit-code && dotnet new %TEMPLATE_SHORT_NAME% --output . --name %NAME% -frontend --force
pause