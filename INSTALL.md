# HOWTO : Install Legal Lead Applications

## Prerequisites
1. Get Latest Version of DOTNET Runtime for Microsoft
 - Documentation   
 You can find more details on this install 
 from the [reference guide](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script)   
 - Install latest dotnet package using powershell
	```powershell
	# Run a separate PowerShell process 
	# because the script calls exit, 
	# so it will end the current PowerShell session.
	
	$url = 'https://dot.net/v1/dotnet-install.ps1'
	$cmd = "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; "
	$cmd += "&([scriptblock]::Create((Invoke-WebRequest -UseBasicParsing '$url'))) -Channel LTS"
	&powershell -NoProfile -ExecutionPolicy unrestricted -Command $cmd
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

## Installation
1. Execute install command from leadcli installed in step 2
- Confirm that cli has been installed   

	```powershell
	leadcli version
	```   

	Expected output:
	```
	3.2.0 - legallead.installer
    ```
- Install legallead-desktop application
	```powershell
	leadcli install legallead-desktop
	```   
