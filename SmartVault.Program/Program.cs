
using Microsoft.Extensions.Configuration;
using SmartVault.DataGeneration.Services;
using SmartVault.Program.Services;
using System.IO;

namespace SmartVault.Program
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

            //Best practice here would be to use Dependency Injection, I'm simplifying since the goal here is to show the usage of the services
            var _dataGenerationService = new DataGenerationService(connectionString, databaseFileName);
            var _businessObjectService = new BusinessObjectService(connectionString, databaseFileName);

            _dataGenerationService.GenerateData();
            _businessObjectService.WriteEveryThirdFileToFile();
            _businessObjectService.GetAllFileSizes();

        }
    }
}