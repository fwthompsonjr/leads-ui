# HOWTO : Install Lead Lead Applications

## Prerequisites
1. Get Latest Version of DOTNET Runtime for Microsoft
 - Documentation
 You can find more details on this install from the [reference guide](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script)
 - Install latest dotnet package using powershell
	```powershell
	# Run a separate PowerShell process because the script calls exit, so it will end the current PowerShell session.
	&powershell -NoProfile -ExecutionPolicy unrestricted -Command "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; &([scriptblock]::Create((Invoke-WebRequest -UseBasicParsing 'https://dot.net/v1/dotnet-install.ps1'))) -Channel LTS"
	```   
2. Download Legal Lead Installer
	- Documentation
	More information on the latest version of legallead.installer in [package home](https://www.myget.org/feed/fwthompsonjr/package/nuget/legallead.installer)
	- Install using the script below
	```powershell
	$name = 'legallead.installer';
	$feed = 'https://www.myget.org/F/fwthompsonjr/api/v3/index.json';
	dotnet tool install -g $name --add-source $feed
	```
3. Execute install command from leadcli installed in step 2
- Documentation