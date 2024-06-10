# legallead.installer
- application used to install legal lead components

## Releases
| Id | Date | Description |  
| --- | --- | --- |
| x.y.z | {{ date }} | {{ description }} |

## Commands:   
| Command | Description |
| --- | --- |
| help | Display help. |
| install |  Install legallead application |
| list | Display release details for legallead applications |
| locals | Display all local installed applications |
| run | Run application as defined by parameters |  
| uninstall | Uninstalls application as defined by parameters |
| upgrade | Upgrade application as defined by parameters |
| version | Display version information for legallead.installer |

## install command

```shell
Usage: install [options...]

Install legallead application

Options:
  -v, --version <String>    version number (Default: )
  -n, --app <String>        application name (Default: )
  -i, --id <String>         application id (Default: )
```   

## list command

```shell
Usage: list

Display release details for legallead applications
```   

## locals command

```shell
Usage: locals

Display all local installed applications
```   

## run command

```shell
Usage: run [options...]

Run application as defined by parameters

Options:
  -n, --name <String>       application name (Required)
  -v, --version <String>    version number (Default: )
```    

## uninstall command

```shell
Usage: uninstall [options...]

Uninstalls application as defined by parameters

Options:
  -n, --name <String>       application name (Required)
  -v, --version <String>    version number (Default: )
```  

## upgrade command

```shell
Usage: upgrade [options...]

Upgrade installed application

Options:
  -n, --name <String>    application name (Default: legallead.reader.service)
```  

## version command

```shell
Usage: version

Display version details for legallead installer
```   

Please contact administrator for additional details.
Confirm unit tests passing prior to release.