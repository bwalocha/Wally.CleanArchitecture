SET "NAME=Wally.CleanArchitecture"
SET "SERVICE_NAME=ApiGatevay"
git diff --exit-code && dotnet new wally.cleanarchitecture --output . --name %NAME% --serviceName %SERVICE_NAME% -proxy=true --force
pause
