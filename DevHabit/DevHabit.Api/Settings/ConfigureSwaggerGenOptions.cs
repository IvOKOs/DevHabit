using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace DevHabit.Api.Settings;

public sealed class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider)// it knows about all the API versions defined in the app, so it can create a swagger document for each version
    : IConfigureNamedOptions<SwaggerGenOptions>// this interface is used to configure the options for swagger generation by defering the configuration until after everything is built and the version info is available
{
    public void Configure(SwaggerGenOptions options)
    {
        // for each version it finds, it creates a separate swagger document
        foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = $"DevHabit.API v{description.ApiVersion}",
                Version = description.ApiVersion.ToString()
            };

            options.SwaggerDoc(description.GroupName, openApiInfo);
        }

        options.ResolveConflictingActions(options => options.First());

        string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);

        options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
        options.DescribeAllParametersInCamelCase();
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }
}

public sealed class ConfigureSwaggerUIOptions(IApiVersionDescriptionProvider provider) 
    : IConfigureNamedOptions<SwaggerUIOptions>
{
    public void Configure(SwaggerUIOptions options)
    {
        // for each version it finds, it adds a swagger endpoint to the UI
        foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
        }
    }

    public void Configure(string? name, SwaggerUIOptions options)
    {
        Configure(options);
    }
}
