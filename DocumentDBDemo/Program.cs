using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Using the db requires these imports
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace DocumentDBDemo {
    class MainClass {

        // Should get your own endpoint and auth key, but this is an example
		private const string EndpointUrl = "https://azuredocdbdemo.documents.azure.com:443/";
		private const string AuthorizationKey =
		   "BBhjI0gxdVPdDbS4diTjdloJq7Fp4L5RO/StTt6UtEufDM78qM2CtBZWbyVwFPSJIm8AcfDu2O+AfV T+TYUnBQ==";

		static void Main(string[] args) {
			try {
				CreateDocumentClient().Wait();
			}
			catch (Exception e) {
				Exception baseException = e.GetBaseException();
				Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
			}
			Console.ReadKey();
		}

		private static async Task CreateDocumentClient() {
			// Create a new instance of the DocumentClient
			using (var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey)) {
				// await CreateDatabase(client);
                GetDatabases(client);
				await DeleteDatabase(client);
				GetDatabases(client);
			}
		}

        /******************************************
         * 
         * 
         * Creating the database using a .NET SDK 
         *
         *
         ******************************************/

		// Create the new database by creating a new database object. 
        // To create a new database, we only need to assign the Id property, 
        // which we are setting to “mynewdb” in a CreateDatabase task
		private async static Task CreateDatabase(DocumentClient client) {
			Console.WriteLine();
			Console.WriteLine("******** Create Database *******");

			var databaseDefinition = new Database { Id = "mynewdb" };
			var result = await client.CreateDatabaseAsync(databaseDefinition);
			var database = result.Resource;

			Console.WriteLine(" Database Id: {0}; Rid: {1}", database.Id, database.ResourceId);
			Console.WriteLine("******** Database Created *******");
		}

		/******************************************
         * 
         * 
         * Listing the databases
         *
         *
         ******************************************/

		private static void GetDatabases(DocumentClient client) {
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("******** Get Databases List ********");

			var databases = client.CreateDatabaseQuery().ToList();

			foreach (var database in databases) {
				Console.WriteLine(" Database Id: {0}; Rid: {1}", database.Id, database.ResourceId);
			}

			Console.WriteLine();
			Console.WriteLine("Total databases: {0}", databases.Count);
		}

		/******************************************
         * 
         * 
         * Deleting a database
         *
         *
         ******************************************/

		/*
         * 
         * This time, we can call AsEnumerable instead of ToList() because we don't actually need a 
         * list object. Expecting only result, calling AsEnumerable is sufficient so that we can get 
         * the first database object returned by the query with First(). This is the database object for 
         * tempdb1 and it has a SelfLink that we can use to call DeleteDatabaseAsync which deletes the database.
         * 
         */

		private async static Task DeleteDatabase(DocumentClient client) {
			Console.WriteLine("******** Delete Database ********");
			Database database = client
			   .CreateDatabaseQuery("SELECT * FROM c WHERE c.id = 'tempdb1'")
			   .AsEnumerable()
			   .First();
			await client.DeleteDatabaseAsync(database.SelfLink);
		}


	}
}
