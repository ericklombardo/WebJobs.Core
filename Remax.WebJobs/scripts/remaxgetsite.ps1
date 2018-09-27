param (
    [string]$modulepath = $(throw "-modulepath is required."),
	[string]$appServiceName = $(throw "-appServiceName is required."), 
    [string]$password = $(throw "-password is required."),    
    [string]$username = $(throw "-username is required.")
)

Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted
Import-Module $modulepath

$secpasswd = ConvertTo-SecureString $password -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential ($username, $secpasswd)
$syncParams = @{
	 SourcePath = 'wwwroot'
	 TargetPath = "C:\inetpub\wwwroot\$appServiceName"
	 ComputerName = "https://$appServiceName.scm.azurewebsites.net:443/msdeploy.axd?site=$appServiceName"
	 Credential = $credential
}

Sync-Website @syncParams