using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace STO.DataManager;

class Program
{
    static void Main(string[] args)
    {
        // Setup configuration to read from appsettings.Development.json
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  // Set the base path
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load default appsettings.json
            .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true) // Load development-specific settings
            .AddEnvironmentVariables(); // Optionally add environment variables

        IConfiguration configuration = builder.Build();

        // Access values from appsettings.Development.json
        var connectionString = configuration["StorageConfiguration:ConnectionString"];
        var dataTable = configuration["StorageConfiguration:DataTable"];

        // Output the values
        Console.WriteLine($"{connectionString}");
        Console.WriteLine($"{dataTable}");
        
        
        
        
    }
}