SET "NAME=Wally.CleanArchitecture"
SET "SERVICE_NAME=ApiGateway"

dotnet format %NAME%.%SERVICE_NAME%.sln --verify-no-changes --severity warn
pause