using SmartVault.Program.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace SmartVault.Program.Services
{
    public class BusinessObjectService
    {
        const string ACCOUNTS_GET_ALL_QUERY = "SELECT Id FROM Account";
        const string DOCUMENTS_GET_ALL_QUERY = "SELECT Id, AccountId, FilePath FROM Document";

        private string _connectionString;
        public BusinessObjectService(string connectionString, string dataBaseFileName)
        {
            SQLiteConnection.CreateFile(dataBaseFileName);

            _connectionString = connectionString;
        }

        public void WriteEveryThirdFileToFile()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var accounts = GetAllAccounts(connection);

                foreach (var account in accounts)
                {
                    List<Document> documents = GetAllDocuments().Where(doc=> doc.AccountId == account.Id).ToList();

                    ProcessDocumentsFiles(documents);
                }
            }
        }

        public void GetAllFileSizes()
        {
            List<Document> documents = GetAllDocuments();

            if (documents != null && documents.Count > 0)
            {
                foreach (var document in documents)
                {
                    string filePath = document.FilePath;

                    if (File.Exists(filePath))
                    {
                        FileInfo fileInfo = new FileInfo(filePath);
                        Console.WriteLine($"The size of the document ID {document.Id} is {fileInfo.Length} bytes. \n");
                    }
                }
            }
        }

        private List<Document> GetAllDocuments()
        {
            List<Document> documents = new List<Document>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(DOCUMENTS_GET_ALL_QUERY, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var document = new Document
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                AccountId = reader.GetInt32(reader.GetOrdinal("AccountId")),
                                FilePath = reader.GetString(reader.GetOrdinal("FilePath"))
                            };

                            documents.Add(document);

                        }
                    }
                }
            }

            return documents;
        }

        private List<Account> GetAllAccounts(SQLiteConnection connection)
        {
            List<Account> accounts = new List<Account>();

            using (var command = new SQLiteCommand(ACCOUNTS_GET_ALL_QUERY, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var account = new Account
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id"))
                        };

                        accounts.Add(account);
                    }
                }
            }

            return accounts;
        }

        private void ProcessDocumentsFiles(List<Document> documents)
        {
            string searchString = "SMITH PROPERTY";

            List<Document> everyThirdDocument = documents
           .Where((doc, index) => (index + 1) % 3 == 0) 
           .ToList();

            foreach (var document in everyThirdDocument)
            {
                string filePath = document.FilePath;
                string content = File.ReadAllText(filePath);

                if (content.Contains(searchString))
                {
                    CopyFile(filePath, $@"{Path.GetDirectoryName(filePath)}\NewDocForDocument{document.Id}.txt");
                }
            }
        }

        private static void CopyFile(string sourcePath, string destinationPath)
        {
            if (File.Exists(sourcePath))
            {
                try
                {
                    File.Copy(sourcePath, destinationPath, overwrite: true);

                    Console.WriteLine("File copied successfully.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"An IO error occurred: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Access denied: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Source file does not exist");
            }
        }
    }
}
