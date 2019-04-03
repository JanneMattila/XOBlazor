# Instructions

[![Build Status](https://jannemattila.visualstudio.com/jannemattila/_apis/build/status/JanneMattila.XOBlazor)](https://jannemattila.visualstudio.com/jannemattila/_build/latest?definitionId=31)

## Working with 'XO Blazor'

### How to create image locally

```batch
# Build container image
docker build . -t xoblazor:latest

# Run container using command
docker run -p "1085:80" xoblazor:latest
``` 

### How to deploy to Azure Container Instances (ACI)

Deploy published image to [Azure Container Instances (ACI)](https://docs.microsoft.com/en-us/azure/container-instances/) the Azure CLI way:

```batch
# Variables
aciName="xoblazor"
resourceGroup="xoblazor-dev-rg"
location="westeurope"
image="jannemattila/xoblazor:latest"

# Login to Azure
az login

# *Explicitly* select your working context
az account set --subscription <YourSubscriptionName>

# Create new resource group
az group create --name $resourceGroup --location $location

# Create ACI
az container create --name $aciName --image $image --resource-group $resourceGroup --ip-address public

# Show the properties
az container show --name $aciName --resource-group $resourceGroup

# Show the logs
az container logs --name $aciName --resource-group $resourceGroup

# Wipe out the resources
az group delete --name $resourceGroup -y
``` 

Deploy published image to [Azure Container Instances (ACI)](https://docs.microsoft.com/en-us/azure/container-instances/) the Azure PowerShell way:

```powershell
# Variables
$aciName="xoblazor"
$resourceGroup="xoblazor-dev-rg"
$location="westeurope"
$image="jannemattila/xoblazor:latest"

# Login to Azure
Login-AzureRmAccount

# *Explicitly* select your working context
Select-AzureRmSubscription -SubscriptionName <YourSubscriptionName>

# Create new resource group
New-AzureRmResourceGroup -Name $resourceGroup -Location $location

# Create ACI
New-AzureRmContainerGroup -Name $aciName -Image $image -ResourceGroupName $resourceGroup -IpAddressType Public

# Show the properties
Get-AzureRmContainerGroup -Name $aciName -ResourceGroupName $resourceGroup

# Show the logs
Get-AzureRmContainerInstanceLog -ContainerGroupName $aciName -ResourceGroupName $resourceGroup

# Wipe out the resources
Remove-AzureRmResourceGroup -Name $resourceGroup -Force
```

### How to deploy to Azure Container Services (AKS)

Deploy published image to [Azure Container Services (AKS)](https://docs.microsoft.com/en-us/azure/aks/):

Create `xoblazor.yaml`:

```yaml
apiVersion: extensions/v1beta1
kind: Deployment
metadata:
  name: xoblazor
  namespace: xo
spec:
  replicas: 1
  template:
    metadata:
      labels:
        app: xoblazor
    spec:
      containers:
      - image: jannemattila/xoblazor:latest
        name: xoblazor
        livenessProbe:
          httpGet:
            path: /
            port: 80
          initialDelaySeconds: 5
          timeoutSeconds: 1
          periodSeconds: 10
          failureThreshold: 2
        readinessProbe:
          httpGet:
            path: /
            port: 80
          initialDelaySeconds: 30
          timeoutSeconds: 1
          periodSeconds: 10
          failureThreshold: 3
        ports:
        - containerPort: 80
          name: http
          protocol: TCP
        env:
          - name: APPLICATION_INSIGHTS_IKEY
            value: ""
```

```batch
kubectl apply -f xoblazor.yaml
```
