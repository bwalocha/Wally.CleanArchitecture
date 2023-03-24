REM @echo off
REM dotnet tool install --global dotnet-ef

IF "%1"=="" (
    SET /p MIGRATION_NAME="Enter migration name (i.e. Initial): "
)
ELSE (
    SET "MIGRATION_NAME=%1"
)

ECHO %MIGRATION_NAME%

SET "STARTUP_PROJECT=./../Wally.CleanArchitecture.MicroService.WebApi/Wally.CleanArchitecture.MicroService.WebApi.csproj"
SET "PROJECT=./Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.MySql.csproj"
SET "Database__ProviderType=MySql"

dotnet ef migrations add %MIGRATION_NAME% --context ApplicationDbContext --startup-project %STARTUP_PROJECT% --project %PROJECT% --verbose
