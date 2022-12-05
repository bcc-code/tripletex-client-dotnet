# Tripletex Client for .Net

A .Net Client/SDK based on the Tripletex swagger specification: https://tripletex.no/v2/swagger.json

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
* Transforms the name and positioning of "verbs" used as part of the method names. Examples:
	- DeliveryAddressGetAsync() -> GetDeliveryAddressAsync()
	- DeliveryAddressPostAsync() -> AddDeliveryAddressAsync()
	- DeliveryAddressPutAsync() -> UpdateDeliveryAddressAsync()
	- DeliveryAddressDeleteAsync() -> DeleteDeliveryAddressAsync()
* Removes "required" JSON validation which doesn't work since Tripletex doesn't always return the entire model.

Note that some of the generated method names are still not intuitive (e.g. ActivityListAsync(..) is used to _add_ multiple activities and ContactListAsync(..) is used to _delete_ multiple contacts).  
This is due to the way things are named in the Tripletex Swagger document.



