# Using API Versioning with ASP.NET Core and Swagger

This repo demonstrates how to get Swagger to work in ASP.NET Core with API versioning enabled.

There are two examples - one for Swashbuckle, and another for NSwag. Should be pretty self explanatory which is which.

See [this branch](https://github.com/kane-armstrong/dotnet-core-api-versioning-and-open-api/tree/net-core-2.2) for a
.NET Core 2.2 version.

## Client Generation

There is an example of getting client generation working with NSwag in the **codegen** folder. 

Links:

* [GitHub source code for the code gen tooling](https://github.com/RicoSuter/NSwag)
* [NPM package used in this example](https://www.npmjs.com/package/nswag)

Some gotchas:

### Generating a client for each version of your API in the same output

When using API versioning, you might want to generate a client for each version of your API. This won't happen if you are running 
independent specs for each version; you will need a spec that displays all versions of your API. Example of how to do this in `ConfigureServices`:

````
    services.AddOpenApiDocument(document =>
    {
        document.DocumentName = "all";
        document.Version = "all";
        document.Title = "Weather Forecast API";
        document.Description = "Weather Forecast API";
    });
````

### Ensuring the generated code is usable in a DevOps pipeline or as a project depedency

All the nswag tooling does is spit out code. If you want to be able to use it, you need to ensure the directory to which this code is exported
contains a csproj with Newtonsoft.Json installed. At the time of writing this is the only dependency.

### Avoiding bad client configuration like methods called Get2Async

NSwag picks up client class/interface name from the name of your controller. Likewise, method names come from the name of the action on the controller.
If you are following a convention like `Controllers/Vx/UsersController`, and you have two versions of `UsersController`, then NSwag will only generate
a single interface. If there is an action on each called `Get`, you will end up with `GetAsync` and `Get2Async` in your generated client. You probably
don't want that... To ensure you have `IUsersClientV1` and `IUsersClientV2`, with a single `GetAsync` on each, you will need to name your controllers
`UsersV1Controller` and `UsersV2Controller`. There might be a way around this, i.e. by configuring nswag.json, but I have yet to look.
