@ECHO OFF
dotnet new --list wally.cleanarchitecture > NUL ^
	&& ECHO Uninstalling previous version... ^
	&& dotnet new --uninstall ./src

dotnet new --install ./src

ECHO Usage:
ECHO dotnet new wally.cleanarchitecture --output . --author wally --name MyCompany.MyService