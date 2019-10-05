# Using API Versioning with ASP.NET Core and Swagger

This repo demonstrates how to get Swagger to work in ASP.NET Core with API versioning enabled.

There are two examples - one for Swashbuckle, and another for NSwag. Should be pretty self explanatory which is which.

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
