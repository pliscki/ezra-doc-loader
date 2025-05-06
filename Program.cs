using Azure.Identity;
using Ezra.Scribe.Core.CloudStorage;
using Ezra.Scribe.Infrastructure.GoogleDrive;
using Ezra.Scribe.Extensions;
using Microsoft.Extensions.Options;
using Ezra.Scribe.Core.CloudStorage.GoogleDrive;
using Google.Apis.Auth.OAuth2.Flows;

var builder = WebApplication.CreateBuilder(args);

// Add configuration for user secrets in development and Azure Key Vault in non-development environments
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();    
}
else
{
    var keyVaultEndpoint = builder.Configuration["KeyVault:Endpoint"];
    if (!string.IsNullOrEmpty(keyVaultEndpoint))
    {
        builder.Configuration.AddAzureKeyVault(new Uri(keyVaultEndpoint), new DefaultAzureCredential());
    }
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEzraScribe(builder.Configuration, builder.Environment);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGoogleDriveEndpoints();
app.MapTestEndpoints();

app.Run();

