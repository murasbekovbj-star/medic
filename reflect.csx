
using System;
using Microsoft.OpenApi;

Console.WriteLine("Properties of OpenApiParameter:");
foreach (var p in typeof(OpenApiParameter).GetProperties()) {
    Console.WriteLine($"  {p.Name} : {p.PropertyType.Name} (CanWrite={p.CanWrite})");
}

Console.WriteLine("Properties of OpenApiSchema:");
foreach (var p in typeof(OpenApiSchema).GetProperties()) {
    Console.WriteLine($"  {p.Name} : {p.PropertyType.Name} (CanWrite={p.CanWrite})");
}
