SET "NAME=Wally.CleanArchitecture"
SET "SERVICE_NAME=MicroService"

dotnet list %NAME%.%SERVICE_NAME%.sln package --deprecated
dotnet list %NAME%.%SERVICE_NAME%.sln package --outdated
dotnet list %NAME%.%SERVICE_NAME%.sln package --vulnerable --include-transitive
pause