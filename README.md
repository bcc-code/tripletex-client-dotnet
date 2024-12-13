# Tripletex Client for .Net

A .Net Client/SDK based on the Tripletex swagger specification: https://tripletex.no/v2/swagger.json

Available as NuGet package `https://www.nuget.org/packages/BccCode.Tripletex.Client`

## Project Structure

* **BccCode.Tripletex.Client** - SDK
    - TripletexClientGenerated.cs -- client generated using NSwag.net
	- TripletexClient.cs -- custom extentions to generated client, to better facilitate authentication, token caching etc.

* **BccCode.Tripletex.Tests** - SDK tests


## Updating client 
In order to re-generate the client code (based on a new swagger specification):
1. open the project in Visual Studio 2022
2. Select "Generate" as the build configuration (instead of Release or Debug)
3. Build the project

The project file includes a number of "PreBuildEvents" which are run when the project is built in with "Generate" configuration selected.   
These prebuild tasks perform the following operations:
* Generate code using Nswag.net using the configuration in `nswag.json`
* Removes invalid characters such as '>' and ':' which are included in the path names of some of the Tripltex API endpoints
* Transforms the name and positioning of "verbs" used as part of the method names.
* Removes "required" JSON validation which doesn't work since Tripletex doesn't always return the entire model.

## .Net SDK

To use the SDK in a .Net application, add the `BccCode.Tripletex.Client` nuget package.

The `ITripletexClient` service can be added to the applications services during startup (startup.cs or program.cs) using the following code (.net 6):

```csharp
\\ ...

builder.Services.AddTripletexClient(new TripletexClientOptions
{
    EmployeeToken = "<your employee token>",
    ConsumerToken = "<your consumer token>"

});

```

Alternatively, the configuration can be read automatically from a "Tripletex" section in your configuration:

```csharp
\\ ...

builder.Services.AddTripletexClient(); //Read from "Tripletex" configuration section:

```

**appsettings.json**
```json

{
  "Tripletex": {
    "EmployeeToken": "<USE USER SECRETS>",
    "ConsumerToken": "<USE USER SECRETS>"
  }
}
```

**Environment Variables**
```bash

TRIPLETEX__EMPLOYEETOKEN=<your employee token>
TRIPLETEX__CONSUMERTOKEN=<your employee token>


```






