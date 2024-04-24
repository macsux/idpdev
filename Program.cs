// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using IdentityServerDemo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Extensions.Configuration.ConfigServer;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddSerilog();
builder.Configuration
    .AddYamlFile("appsettings.yml")
    .AddYamlFile("config-repo/identityserver.yml");

if (builder.Configuration.GetValue<string>("spring:cloud:config:uri") != null)
{
    builder.Configuration.AddConfigServer();
}

builder.Services.AddSerilog();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ICorsPolicyService, ConfigCorsPolicyService>();
builder.Services.AddIdentityServer(options =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
        
        // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
        options.EmitStaticAudienceClaim = true;
    })
    .AddCorsPolicyService<ConfigCorsPolicyService>()
    .AddResourceStore<ConfigResourceStore>()
    .AddClientStore<ConfigClientStore>()
    .AddExtensionGrantValidator<DelegationGrantValidator>()
    .AddExtensionGrantValidator<TokenExchangeGrantValidator>()
    .AddSecretValidator<PlainTextSharedSecretValidator>()
    .AddProfileService<ConfigUserProfileService>()
    .AddResourceOwnerValidator<ConfigUserResourceOwnerPasswordValidator>()
    .AddDeveloperSigningCredential();

builder.Services.AddSingleton<ConfigUserStore>();
builder.Services.AddOptions();
builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("security"));

// builder.Services.AddAuthentication()
//  .AddGoogle(options =>
//  {
//   options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
//
//   // register your IdentityServer with Google at https://console.developers.google.com
//   // enable the Google+ API
//   // set the redirect URI to https://localhost:5001/signin-google
//   options.ClientId = "copy client ID from Google here";
//   options.ClientSecret = "copy client secret from Google here";
//  });


try
{
    Log.Information("Starting host...");
    var app = builder.Build();
    
    app.UseDeveloperExceptionPage();
    app.UseIdentityServer();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}