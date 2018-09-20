Login-AzureRmAccount
$azureAdApplication = New-AzureRmADApplication -DisplayName "AzureAutomationApp" -HomePage "https://www.myapp.org/" -IdentifierUris "https://www.myApp.org/azureAutomationApp" -Password $(ConvertTo-SecureString -String "mseW!nd0w5P@ss2015" -AsPlainText -Force)
New-AzureRmADServicePrincipal -ApplicationId $azureAdApplication.ApplicationId
New-AzureRmRoleAssignment -ServicePrincipalName $azureAdApplication.ApplicationId -RoleDefinitionName Contributor

$domain = pedromullermisegundoempleo.onmicrosoft.com
$tenant  = c1be0918-f9b1-4af1-b274-c3de43833466
$userId = "7ec4dbf5-00ec-4f2a-bd60-706b3af02ae3"
$password = ConvertTo-SecureString -String "mseW!nd0w5P@ss2015" -AsPlainText -Force
$cred = New-Object -TypeName System.Management.Automation.PSCredential($userId ,$password)

Connect-AzureRmAccount -Credential $cred -ServicePrincipal -TenantId c1be0918-f9b1-4af1-b274-c3de43833466

$importRequest = New-AzureRmSqlDatabaseImport -ResourceGroupName "mse" -ServerName "sqlremax" -DatabaseName "MyImportSample" -DatabaseMaxSizeBytes "262144000" -StorageKeyType "StorageAccessKey" -StorageKey "7dp4aMlvIadwBD4GW+llr5uaRyAmyXkZMiDzO1/IJQjGyRW2KiutkYrH05tD41rsLyWG8pMKCnpNsbA76udZ2A==" -StorageUri "https://remaxccademo.blob.core.windows.net/dbbkps/REMAXCCA_DB-2018-9-12-9-14.bacpac" -Edition "Basic"  -ServiceObjectiveName "Basic" -AdministratorLogin "db-admin" -AdministratorLoginPassword $(ConvertTo-SecureString -String "mseW!nd0w5P@ss2015" -AsPlainText -Force)

$importStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
[Console]::Write("Importing")
while ($importStatus.Status -eq "InProgress")
{
    $importStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
    [Console]::Write(".")
    Start-Sleep -s 10
}
[Console]::WriteLine("")
$importStatus