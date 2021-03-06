param (
    [string]$modulepath = $(throw "-modulepath is required."),
	[string]$root = $(throw "-root is required."),
	[string]$wwwroot = $(throw "-wwwroot is required."),
	[string]$appServiceName = $(throw "-appServiceName is required."), 
    [string]$password = $(throw "-password is required."),    
    [string]$username = $(throw "-username is required."),
	[int]$timeout = $(throw "-timeout is required.")
)

Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted
Import-Module $modulepath

$secpasswd = ConvertTo-SecureString $password -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential ($username, $secpasswd)
$syncParams = @{
	 SourcePath = $root
	 TargetPath = "$wwwroot\$appServiceName"
	 ComputerName = "https://$appServiceName.scm.azurewebsites.net/msdeploy.axd?site=$appServiceName"
	 Credential = $credential
	 Timeout = $timeout
}

Sync-Website @syncParams