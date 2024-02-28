// Use https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/deploy-github-actions?tabs=userlevel%2CCLI to setup GitHub Actions for Bicep deployment

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

resource storageAccountTableServiceDataTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: 'data'
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
    name: 'B2'
  }
  kind: 'linux'
  properties: { reserved: true }
}

//API WEB APP
resource apiWebApp 'Microsoft.Web/sites@2022-03-01' = {
  name: 'apiwebapp-${uniqueName}'
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: webAppServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: true
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'StorageConfiguration__DataTable'
          value: storageAccountTableServiceDataTable.name
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
output apiWebAppName string  = apiWebApp.name

//WEB APP
resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: 'webapp-${uniqueName}'
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: webAppServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: true
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'StorageConfiguration__DataTable'
          value: storageAccountTableServiceDataTable.name
        }
        {
          name: 'StorageConfiguration__ConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, '2019-06-01').keys[0].value}'
        }
        {
          name: 'StorageConfiguration__ApiHost'
          value: 'https://${apiWebApp.properties.defaultHostName}'
        }
        {
          name: 'AzureAdB2C__Instance'
          value: 'https://tuesdayfootball.b2clogin.com/'
        }
        {
          name: 'AzureAdB2C__ClientId'
          value: 'e0c23c36-7084-4597-86e7-494611963e50'
        }
        {
          name: 'AzureAdB2C__CallbackPath'
          value: '/signin-oidc'
        }
        {
          name: 'AzureAdB2C__Domain'
          value: 'tuesdayfootball.onmicrosoft.com'
        }
        {
          name: 'AzureAdB2C__SignUpSignInPolicyId'
          value: 'b2c_1_susi'
        }
        {
          name: 'AzureAdB2C__ResetPasswordPolicyId'
          value: ''
        }
        {
          name: 'AzureAdB2C__EditProfilePolicyId'
          value: ''
        }
      ]
    }
    httpsOnly: true
  }
}
output webAppName string  = webApp.name


