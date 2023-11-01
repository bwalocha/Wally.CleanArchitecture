SET "NAME=Wally.CleanArchitecture"
SET "SERVICE_NAME=MicroService"
git diff --exit-code && dotnet new wally.cleanarchitecture --output . --name %NAME% --serviceName %SERVICE_NAME% -service=true --force
pause
