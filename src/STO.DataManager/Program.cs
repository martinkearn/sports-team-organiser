using Microsoft.Extensions.Configuration;
using Azure;
using Azure.Data.Tables;

namespace STO.DataManager;

class Program
{
    public static async Task Main(string[] args)
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
        
        // Setup Table
        // -----------
        var tableClient = new TableClient(connectionString, dataTable);
        await tableClient.CreateIfNotExistsAsync();
        
        // Replace values
        // --------------
        // Query all entities from the table
        var entities = tableClient.Query<TableEntity>();

        // Iterate over each entity
        var updateCount = 0;
        foreach (var entity in entities)
        {
            // Check if the property exists and update it
            if (!entity.ContainsKey("Forecast")) continue;
            entity["Forecast"] = 0;
                
            // Alternatively, add a new property if it doesn't exist
            // entity["NewProperty"] = "NewValue";

            // Update the entity in the table
            await tableClient.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Replace);
            
            Console.WriteLine($"Updated {entity.RowKey}");
            updateCount += 1;
        }
        
        
        Console.WriteLine($"Updated {updateCount} entities.");
    }
}