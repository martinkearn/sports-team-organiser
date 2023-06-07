# Workflows
There are three workflows in this repo. `deployment.yaml` is the main deployment workflow template which is used to create a deployment and deploy the application code to it. This uses Bicep for infrastrcuture deployment and various other actions to build and deploy the application code. This accepts inputs with specific environmental settings in.

## Secrets
The `cd-prod.yml` and `cd-dev.yml` workflows are for the dev/test and production environments. Both workflows rely on secrets being stored in Github (Settings > Secrets & Variables > Actions):
- `AZURE_SUBSCRIPTION` is used by both workflows and represents the subscription to deploy to. This contains only the Azure subscription ID.
- `AZURE_CREDENTIALS` is used by `cd-prod.yml` and is used for the production deployment. This contains the output JSON from the `az ad sp create-for-rbac` command in the context of the production resource group.
- `AZURE_CREDENTIALS_DEV` follows the same schema as `cd-prod.yml` but is for the dev/test deployment and is used by `cd-dev.yml`. This contains the output JSON from the `az ad sp create-for-rbac` command in the context of the dev resource group.

The configuration is based on the guidance in this article: https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/deploy-github-actions?tabs=userlevel%2CCLI
