using Microsoft.Extensions.Configuration;
using SmartVault.DataGeneration.Services;
using System.IO;

namespace SmartVault.DataGeneration
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            string databaseFileName = configuration["DatabaseFileName"];
            string connectionString = string.Format(configuration?["ConnectionStrings:DefaultConnection"] ?? "", databaseFileName);

            var _dataGenerationService = new DataGenerationService(connectionString, databaseFileName);

            _dataGenerationService.GenerateData();
            _dataGenerationService.ReadAndPrintData();
            
        }
   
    }
}