SET "NAME=Wally.CleanArchitecture"
SET "SERVICE_NAME=ApiGateway"

dotnet list %NAME%.%SERVICE_NAME%.sln package --vulnerable --include-transitive
pause