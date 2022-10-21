Migrating to Azure
==================

This code supports the talk at https://robrich.org/slides/migrating-to-azure/. In this talk we upgrade an app to remove legacy features like AppData and per-server cache. Then we host it in both Azure Web Apps and Azure Kubernetes Service. Grab this code to follow along or recreate the demos at your leisure.

There are two folders: `start` that has legacy technologies and `done` that removes these. We'll deploy the `done` app to Azure Web Apps and Azure Kubernetes Service (AKS).

There are many more resources in Azure than the ones we'll use today, but this will definitely get you started. I encourage you to continue experimenting in Azure, provisioning other resources and playing and reading how they work.


`start`
-------

This app contains legacy technology we must first eliminate before we can move to the cloud. Compare with `done` to see how we eliminate per-server cache and AppData.

### Usage:

1. From a terminal, run `docker-compose up`. This gets a SQL Server instance ready to go. Alternatively, you could use another SQL Server.

2. In Azure Data Studio, SSMS, or your favorite query tool, create the Profiles table by running `create-db.sql`.

3. Load `MovingToAzure.sln` in Visual Studio.

4. Set connection string: In Visual Studio, in the Solution Explorer, right-click on the project, and choose Manage User Secrets.

5. In the secrets.json file, add the connection details to your SQL Server.

   If you're using the `mssql` container from docker-compose, it looks like this:

   ```json
   {
     "ConnectionStrings": {
       "MSSQL": "Server=localhost,1435;Database=ToTheCLoud;User ID=sa;Password=passw@rd!"
     }
   }
   ```

   **Note:** These secrets are /not/ in appsettings.json to keep them out of source control.

6. Run the project inside Visual Studio.

7. Click the Profile page, and click the + button. Add details and click save.

8. Notice how the list doesn't update. It's cached for 1 minute. Wait a minute, push refresh, and your item will show.

9. Notice how the image you uploaded is in `wwwroot/AppData`.


`done`
------

We started with the `start` app and made a few changes to get this ready for cloud hosting. In this app, we replace image storage from the AppData folder to Azure Blob Storage, and move caching from an in-memory dictionary to Redis Cache. With this sample, we're now ready to move to the cloud.

### Usage:

1. From a terminal, run `docker-compose up`. This starts a SQL Server instance and a Redis instance. Alternatively, you could use any SQL Server and Redis servers.

2. In Azure Data Studio, SSMS, or your favorite query tool, create the Profiles table by running `create-db.sql`.

3. Load `MovingToAzure.sln` in Visual Studio.

4. Set connection strings: In Visual Studio, in the Solution Explorer, right-click on the project, and choose Manage User Secrets.

5. In the secrets.json file, add the connection details to SQL Server, Blob storage, and Redis.

   If you're using the `mssql` and `redis` containers from docker-compose, it looks like this:

   ```json
   {
     "ConnectionStrings": {
       "MSSQL": "Server=localhost,1435;Database=ToTheCLoud;User ID=sa;Password=passw@rd!",
       "Redis": "localhost:6379",
       "Blob": "DefaultEndpointsProtocol=https;AccountName=SET_ME;AccountKey=SET_ME;EndpointSuffix=core.windows.net"
     },
     "AppSettings": {
       "BlobContainerName": "SET_ME",
       "CacheSeconds": 60
     }
   }
   ```

   **Note:** These secrets are /not/ in appsettings.json to keep them out of source control.

6. In the AppSettings section, also add the blob container name (the folder inside Azure Blobs), and a cache timeout for Redis.

7. Run the project inside Visual Studio.

8. Click the Profile page, and click the + button. Add details and click save.

9. Notice how the list updates with the new info. View page source, and you can see the image came from Azure.


Azure Web Apps
--------------

Now that we have a cloud-ready app, we're ready to move to Azure. Azure Web App (also known as Azure Web Service) is a really simple mechanism for hosting monoliths or shifting up from IIS.

**NOTE:** This sample shows the easy way to deploy because the focus is on Azure primitives not deployment strategies. Production setups should use GitHub Actions, Azure DevOps, or another tool to build an automated DevOps deployment pipeline. As [Damian Brady](https://twitter.com/Damovisa) says, "Friends don't let friends right-click publish."

0. Create a new Resource Group, naming it specific to this tutorial. Choose a convenient Azure region such as "West US 3".

1. Create an Azure SQL Database using the Azure Portal. Choose a billing plan that fits your goals and budget. Ensure this is in the resource group created above.

2. Using Azure Data Studio, SSMS, or the Azure Portal, run the portion of `create-db.sql` that creates the Profiles table in the current database. Ensure this is in the resource group created above.

3. Create an Azure Redis Cache using the Azure Portal. Choose a SKU and scaling setup that fits your goals and budget. Ensure this is in the resource group created above.

4. Create an Azure Web App using the Azure Portal. Choose a Linux hosting plan and scaling details compatible to your goals and budget. Ensure this is in the resource group created above.

5. In the Azure Web App, create Connection Strings for `MSSQL`, `Redis`, and `Blob`. Create App Settings for `AppSettings__BlobContainerName` and `AppSettings__CacheSeconds`. (Note the `AppSettings__` are required before each to tell .NET these are settings nested inside the AppSettings json object.)

6. Open MovingToTheCloud.sln in Visual Studio.

7. In the Solution Explorer, right-click on the project, and choose publish.

8. If you're not already, login to your Microsoft account.

9. In the dialogs, choose the correct Azure subscription, resource group, and the Azure Web App created above. Click publish to deploy to Azure.

10. In the Azure portal, navigate to the Azure Web App, and copy the site URL on the Overview page.

11. Load the Azure Web App in a browser and create a new profile.

Congratulations! You've deployed a website to Azure.

NOTE: Did it error? You can view the logs using the Advanced Tools towards the bottom of the left menu.


Azure Kubernetes Service
------------------------

Now that we have a cloud-ready app, we're ready to move to Azure. Azure Kubernetes Service (AKS) is a great way to host a herd of microservices. Azure Kubernetes Service is a simple, 1-button setup of upstream Kubernetes.

**NOTE:** This sample shows the easy way to deploy because the focus is on Azure primitives not deployment strategies. Production setups should use GitHub Actions, Azure DevOps, or another tool to build an automated DevOps deployment pipeline. As [Damian Brady](https://twitter.com/Damovisa) says, "Friends don't let friends right-click publish."

0. Create a new Resource Group, naming it specific to this tutorial. Choose a convenient Azure region such as "West US 3".

1. Create an Azure SQL Database using the Azure Portal. Choose a billing plan that fits your goals and budget. Ensure this is in the resource group created above.

2. Using Azure Data Studio, SSMS, or the Azure Portal, run the portion of `create-db.sql` that creates the Profiles table in the current database.

3. Create an Azure Redis Cache using the Azure Portal. Choose a SKU and scaling setup that fits your goals and budget. Ensure this is in the resource group created above.

4. Create an Azure Container Registry (ACR). This is like Docker Hub, but private to your account. Ensure this is in the resource group created above.

5. From a terminal, run `docker login YOUR_REGISTRY_HERE.azurecr.io`, substituting YOUR_REGISTRY_HERE with the URL of your container registry. Use the username and password from the Keys page of your Azure Container Registry.

6. Inside the `done` folder, run `docker build -t YOUR_REGISTRY_HERE.azurecr.io/movingtoazure:v0.1` to build the container.

7. In the same terminal, run `docker push YOUR_REGISTRY_HERE.azurecr.io/movingtoazure:v0.1` to push this to your container registry.

   Now with the container pushed to the registry, we could deploy this to Azure Web Apps, Azure Container Instance, Azure Kubernetes Apps, or any container hosting platform.

8. In the Azure Portal, create an Azure Kubernetes Service (AKS). Choose the size and quantity of nodes to fit your budget and goals. Enable HTTP Application Routing. Ensure this is in the resource group created above.

   Note: HTTP Application Routing is not a highly available service as it runs from a single pod. For production scenarios, use a different mechanism to support external DNS into AKS.

9. Update `k8s.yaml` adding in all the connection strings and environment variables from the connection details in each Azure service. E.g. grab the Redis connection from the Azure Redis Service's Access keys page.

10. In a terminal from within the root of the project, run `kubectl apply -f k8s.yaml` to start the site in AKS.

11. Give ample time for HTTP Application Routing's DNS to propagate.

12. Run `kubectl get all` to get the URL of your application, then visit this URL in the browser.

Congratulations! You've deployed a container to Azure.

Note: Did it error? You can view the pod's logs by running `kubectl logs deployment/movingtoazure`.


Cleanup
-------

To ensure you're not continuously billed for the content created today, ensure you delete the Azure resources. The easiest way to do this is to delete the Resource Group(s) which will delete all Azure resources within it.

1. Login to the Azure Portal.

2. In the search bar at the top, type `Resource Group`.

3. Choose Resource Groups from the menu.

4. Choose the Resource Group created today.

5. On the right, choose "Delete resource group", enter the resource group name, and confirm you'll delete all resources in the group.

6. Repeat this procedure for each resource group created as part of this tutorial.

7. Search for Resource Group again, open the Resource Groups list, push refresh, and ensure all resources are deleted.


License
-------

Ryan Alba
