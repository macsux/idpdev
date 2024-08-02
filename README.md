## Development OIDC server

This is a simple config driven OIDC server for use in development based on IdentityServer4 project.

*** WARNING: DO NOT USE IN PRODUCTION. THIS IS NOT A SECURE IMPLEMENTATION AND IS OPTIMIZED FOR SIMPLICITY ***

## How to use 

1. Modify `config-repo/identityserver.yml` to match your security configuration. The configuration yaml file maps to [this model class](https://github.com/macsux/idpdev/blob/main/Store/Config/SecuritySettings.cs) . To help understand possible config values, see related model classes [here](https://github.com/IdentityServer/IdentityServer4/tree/main/src/Storage/src/Models) and [here](https://github.com/IdentityServer/IdentityServer4/blob/main/src/IdentityServer4/src/Test/TestUser.cs).
2. With .NET 8 SDK installed, run `dotnet run` from project root folder

Server runs on port https://localhost:5011 by default. 
