# Using API Versioning with ASP.NET Core and Swagger

This repo demonstrates how to get Swagger to work in ASP.NET Core with API versioning enabled. There are two sets of examples,
one for .NET Core 2.x and another for .NET Core 3.0. Both demonstrate how this works for both NSwag and Swashbuckle. 

The .NET Core 3.0 demo also includes an example of how to generate an API client using NSwag. See [here](https://github.com/RicoSuter/NSwag)
for a link to the code gen tooling in question, specifically [this](https://www.npmjs.com/package/nswag) npm package.

I recommend using NSwag over Swashbuckle. Integrates better with the code gen, and seems a bit more flexible when defining documents.
Specifically, NSwag allows you to define authentication schemes at the spec level, but Swashbuckle seems to do this across all
specs.
