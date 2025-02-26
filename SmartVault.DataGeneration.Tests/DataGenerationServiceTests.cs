﻿using Microsoft.Extensions.Configuration;
using SmartVault.DataGeneration.Services;
using System.Data.SQLite;

namespace SmartVault.DataGeneration.Tests;

public class DataGenerationServiceTests
{
    private DataGenerationService _dataGenerationService;
    private string _connectionString;

    public DataGenerationServiceTests()
    {
        var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json").Build();

        string databaseFileName = configuration["DatabaseFileName"];
        _connectionString = string.Format(configuration?["ConnectionStrings:DefaultConnection"] ?? "", databaseFileName);

        _dataGenerationService = new DataGenerationService(_connectionString, databaseFileName);
    }

    //Best practice would be to use in memory database, I'm simplifying since the goal here is to show how unit testing will work for this appplication
    [Fact]
    public void GenerateData_ShouldCrateTheTablesAndPopulate_WhenDataGenerationIsSuccessful()
    {
        // Arrange
        bool userExistAndIsPopulated = false;
        bool accountExistAndIsPopulated = false;
        bool documentsExistAndIsPopulated = false;
        string tablesExistQuery = "SELECT EXISTS (SELECT COUNT(*) FROM User) AS UserExists, EXISTS (SELECT COUNT(*) FROM Account) AS AccountExists, EXISTS (SELECT COUNT(*) FROM Document) AS DocumentExists;";

        // Act  
        _dataGenerationService.GenerateData();

        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = tablesExistQuery;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userExistAndIsPopulated = reader.GetBoolean(0);
                        accountExistAndIsPopulated = reader.GetBoolean(1);
                        documentsExistAndIsPopulated = reader.GetBoolean(2);

                    }
                }
            }
        }

        // Assert
        Assert.True(userExistAndIsPopulated && accountExistAndIsPopulated && documentsExistAndIsPopulated, "No data was generated by  _dataGenerationService.GenerateData()");
    }
}
