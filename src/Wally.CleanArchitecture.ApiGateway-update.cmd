SET "NAME=Wally.CleanArchitecture"
SET "SERVICE_NAME=ApiGateway"

SET "AUTHOR_NAME=wally"
SET "TEMPLATE_NAME=%AUTHOR_NAME%.cleanarchitecture"

git diff --exit-code && dotnet new %TEMPLATE_NAME% --output . --name %NAME% --serviceName %SERVICE_NAME% -proxy=true --force
pause
