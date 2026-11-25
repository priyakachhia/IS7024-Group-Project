# Deploy Metro Biz Advisor to Azure

Use **Azure App Service** for the .NET 10 Razor Pages app and **Azure SQL Database** on an Azure SQL logical server for saved analyses and community posts. The app reads the Cincinnati crime and permit datasets from the city's public API.

## Set up the Azure resources

1. In Azure Portal, create or select a resource group. Create a **Web App** with **Publish** set to **Code**, **Runtime stack** set to **.NET 10 (LTS)**, and **Operating system** set to **Linux**. Review the App Service plan price. Leave continuous deployment off during creation so GitHub uses the workflow in this repository.
2. Create an **Azure SQL Database** and its logical **SQL server** in the same region. Configure a Microsoft Entra administrator on the SQL server. For an eligible class subscription, look for **Apply offer** and choose the option to pause when the free database allowance is exhausted. Check the web app plan separately.
3. Under the web app's **Identity**, enable **System assigned**. For a SQL server with a public endpoint, use **Selected networks** and add individual firewall rules for the web app's listed **Outbound IP Addresses** (under its **Properties**) and your database administrator's client IP. A change of hosting plan can change the outbound addresses.

## Initialize the database

Open the new **SQL database → Query editor** and sign in as the Microsoft Entra administrator. Run [azure-sql-schema.sql](azure-sql-schema.sql) in the application database. Then run the following, replacing `APP_SERVICE_NAME` with the exact web app resource name.

```sql
IF DATABASE_PRINCIPAL_ID(N'APP_SERVICE_NAME') IS NULL
    CREATE USER [APP_SERVICE_NAME] FROM EXTERNAL PROVIDER;

GRANT SELECT, INSERT ON dbo.HistoryEntries TO [APP_SERVICE_NAME];
GRANT SELECT, INSERT ON dbo.CommunityPosts TO [APP_SERVICE_NAME];
```

In the web app's **Settings → Environment variables → Connection strings**, add a connection string named **MetroBizAdvisor** with type **SQLAzure**. Replace both placeholders and apply the change.

```text
Server=tcp:SQL_SERVER_NAME.database.windows.net,1433;Database=DATABASE_NAME;Authentication=Active Directory Managed Identity;Encrypt=True;TrustServerCertificate=False;Connect Timeout=60;
```

The SQL script creates the two application tables. An empty Azure database starts with empty History and Community pages. Any existing local SQLite rows require a separate data import.

## Connect GitHub Actions

1. Follow Microsoft's [OpenID Connect setup](https://learn.microsoft.com/azure/app-service/deploy-github-actions) to create a deployment identity with access to this App Service. Set the federated credential's GitHub entity to **Branch**, branch **main**, for **priyakachhia/IS7024-Group-Project**. Scope the deployment role to the web app.
2. Under **GitHub → Settings → Secrets and variables → Actions**, add repository secrets `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, and `AZURE_SUBSCRIPTION_ID` for that deployment identity. Under **Variables**, add `AZURE_WEBAPP_NAME` with the exact App Service resource name.
3. Commit or merge the workflow into `main` and open **Actions → Build, test, and deploy to Azure**. Run it on `main` if the variables were set after the commit. A successful run publishes the app. Each later push to `main` triggers the same build, tests, and deployment.

When ready for live verification, save an analysis and publish a sample Community post from the deployed site. Reload History and Community to verify both records persist. The current Community page accepts posts without signing in.

The command-line test jobs call both test projects directly. The separate Playwright browser acceptance tests require a running test host and a browser installation.
