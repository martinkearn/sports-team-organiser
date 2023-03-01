@description('The unique name for the deployment.')
param uniqueName string = uniqueString(resourceGroup().id)

@description('Location for all resources.')
param location string = resourceGroup().location

//STORAGE ACCOUNT
var storageAccountName = 'storage${uniqueName}'
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
}

//STORAGE TABLEs
resource storageAccountTableService 'Microsoft.Storage/storageAccounts/tableServices@2022-05-01' = {
  name: 'default'
  parent: storageAccount
}

resource storageAccountTableServicePlayersTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: 'players'
  parent: storageAccountTableService
}

resource storageAccountTableServiceGamesTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: 'games'
  parent: storageAccountTableService
}

//APP INSIGHTS
var appInsightsName = 'appinisghts-${uniqueName}'
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: {
    'hidden-link:${resourceId('Microsoft.Web/sites', appInsightsName)}': 'Resource'
  }
  properties: { Application_Type: 'web' }
  kind: 'web'
}

//APP SERVICE PLAN for WEB APP
resource webAppServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: 'webapp-service-${uniqueName}'
  location: location
  sku: {
    name: 'B1'
  }
  kind: 'linux'
  properties: { reserved: true }
}

//WEB APP
resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: 'webapp-${uniqueName}'
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: webAppServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|7.0'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference(applicationInsights.id, '2020-02-02').InstrumentationKey
        }
        {
          name: 'StorageConfiguration__PlayersTable'
          value: storageAccountTableServicePlayersTable.name
        }
        {
          name: 'StorageConfiguration__GamesTable'
          value: storageAccountTableServiceGamesTable.name
        } 
        {
          name: 'StorageConfiguration__ConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, '2019-06-01').keys[0].value}'
        } 
      ]
    }
    httpsOnly: true
  }
}
output webAppName string  = webApp.name
