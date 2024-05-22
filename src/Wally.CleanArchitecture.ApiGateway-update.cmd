SET "NAME=Wally.CleanArchitecture"
SET "SERVICE_NAME=ApiGateway"

SET "AUTHOR_NAME=Wally"
SET "TEMPLATE_NAME=%AUTHOR_NAME%.CleanArchitecture"

dotnet new install %TEMPLATE_NAME%.Template
git diff --exit-code && dotnet new %TEMPLATE_NAME% --output . --name %NAME% --serviceName %SERVICE_NAME% -proxy=true --force
pause