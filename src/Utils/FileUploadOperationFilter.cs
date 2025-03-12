using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace src.Utils;


public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if any parameter is of type IFormFile
        var fileParams = context.ApiDescription.ParameterDescriptions
            .Where(x => x.Type == typeof(IFormFile)).ToList();

        if (!fileParams.Any())
            return;

        // Remove any parameter that Swagger might have generated for file uploads
        operation.Parameters.Clear();

        // Define the request body for file upload
        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["file"] = new OpenApiSchema
                            {
                                Description = "Upload file",
                                Type = "string",
                                Format = "binary"
                            }
                        },
                        Required = new HashSet<string> { "file" }
                    }
                }
            }
        };
    }
}