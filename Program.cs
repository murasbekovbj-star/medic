var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        if (document.Paths != null)
        {
            foreach (var path in document.Paths.Values)
            {
                if (path.Operations == null) continue;
                foreach (var op in path.Operations.Values)
                {
                    if (op.Parameters == null) continue;
                    foreach (var param in op.Parameters)
                    {
                        if (param.Schema is Microsoft.OpenApi.OpenApiSchema schema)
                        {
                            if (param.Name == "id" || schema.Format == "int32")
                            {
                                schema.Type = Microsoft.OpenApi.JsonSchemaType.Integer;
                                schema.Pattern = null;
                            }
                        }
                    }
                }
            }
        }
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
