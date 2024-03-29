# NetworkRestart
Console-based application that allows for the scripted restart, logoff, or shutdown of selected computers within a Windows Active Directory Domain

## Getting Started

Simply clone the project to get your development enviornment started up. Once your solution is open, right click on the project, go to the Debug tab, and add a path to your json configuration file.

See deployment for notes on how to deploy the project on a live system.

### Prerequisites

Json configuration file used for providing correct data to application. 
Here is a sample config file. 

```
{
  "Action": {
    "action": "restart",
    "installUpdates": "true",
    "forceAction": "true"
  },
  "ServerConfiguration": {
    "domain": "xxxx.local",
    "domainAdminUserName": "xxxx@xxxx.com",
    "domainAdminPassword": "xxxx",
    "rootOUdistinguishedName": "OU=xxxx,OU=Computers,OU=Production,DC=xxxx,DC=local",
    "includeSubfolders": "true"
  },
  "computersToIgnore": [
    "xxxx-SPAR0001",
    "xxxx-SPAR0002"
  ],
  "EmailConfiguration": {
    "Server": "xxx",
    "Port": 2525,
    "userName": "xxxx@xxxx.net",
    "password": "xxxx",
    "fromAddress": "noreply@xxxx.net",
    "toAddresses": ["xxxx@xxxx.net","xxxx@xxxx;net"],
    "sendErrorsOnly": false
  }
}
```

### Installing

Clone the application then install/restore all required packages. 

## Deployment

Please refer to the documents to deploy your application:

ClickOnce - Quick steps to Deploy, Install and Update Windows Based Client Applications. - https://www.codeproject.com/Articles/17003/ClickOnce-Quick-steps-to-Deploy-Install-and-Update

Deploying Windows applications using ClickOnce (Visual Studio 2008) - https://www.codeproject.com/Articles/196537/Deploying-Windows-applications-using-ClickOnce-Vis

How-To: Use ClickOnce to deploy your Applications - https://weblogs.asp.net/shahar/how-to-use-clickonce-to-deploy-your-applications

## Built With

* .NET Framework 4.6.1
* Console Application

## Authors

* **Chet  Cromer** - President, CEO, Lead Developer at C2IT Consulting, Inc
* **Carr O'Connor** - Jr. Developer at C2IT Consulting, Inc

## Acknowledgments

* We thought this could be a useful tool for others trying to accomplish similar goals with automation of tasks through an active directory. We hope this helps future devs. Thank You. 
