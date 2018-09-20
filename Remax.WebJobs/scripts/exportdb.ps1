$Wcl = new-object System.Net.WebClient
$Wcl.Headers.Add("user-agent", "PowerShell Script")
$Wcl.Proxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials

$strDate = Get-Date
$strday = "$($strDate.Year)$($strDate.Month)$($strDate.Day)"
$tenant  = "5aa42d39-bea8-4ac6-9032-3986d181b4ce"
$userId = "7453b75e-6126-4c73-b4d3-98af2abf2a72"
$password = ConvertTo-SecureString -String "mseW!nd0w5P@ss2015" -AsPlainText -Force
$cred = New-Object -TypeName System.Management.Automation.PSCredential($userId ,$password)

Connect-AzureRmAccount -Credential $cred -ServicePrincipal -TenantId $tenant

$exportRequest = New-AzureRmSqlDatabaseExport -ResourceGroupName "Default-SQL-WestUS" -ServerName "h0zz7swbt8" -DatabaseName "REMAXCCA_DB" -StorageKeyType "StorageAccessKey" -StorageKey "7VVLct9WFNQpA82gCiftypbOYbMREGraD0Uej2Zc2Wf8bwO3tUg4oNed0HmACKad4j+h34Doa6HKteCOwwAt9w==" -StorageUri "https://resourcesitediag383.blob.core.windows.net/dbbacpacs/REMAXCCA_DB_$($strday).bacpac" -AdministratorLogin "adminremaxcca" -AdministratorLoginPassword $(ConvertTo-SecureString -String "Ccaremax1" -AsPlainText -Force)

$exportStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $exportRequest.OperationStatusLink
[Console]::Write("Exporting")
while ($exportStatus.Status -eq "InProgress")
{
    Start-Sleep -s 10
    $exportStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $exportRequest.OperationStatusLink
    [Console]::Write(".")   
}
[Console]::WriteLine("")
$exportStatus