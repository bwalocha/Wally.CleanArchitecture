REM @echo off
REM dotnet tool install --global dotnet-ef

set /p MIGRATION_NAME="Enter migration name (i.e. UpdateUser): "

dotnet ef migrations add %MIGRATION_NAME% --context ApplicationDbContext --startup-project ./../Wally.CleanArchitecture.WebApi/Wally.CleanArchitecture.WebApi.csproj --project ./Wally.CleanArchitecture.Persistence.SqlServer.csproj