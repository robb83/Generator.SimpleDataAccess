using Generator.SimpleDataAccess.Generators;
using Generator.SimpleDataAccess.Model;
using System;
using System.Data.SqlClient;

namespace Generator.SimpleDataAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length < 4)
            {
                Console.WriteLine("Usage: connectionstring outputfile namespace classname");
                return;
            }

            String connectionString = args[0];
            String outputFileName = args[1];
            String ns = args[2];
            String dataAccessClassName = args[3];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                DatabaseSchemaInfo schema = DatabaseSchemaExtractor.GetDatabaseSchemaInfo(connection);
                schema.Namespace = ns;
                schema.ClassName = dataAccessClassName;

                CSharpTextGenerator.GenerateDatabaseAccessCode(schema, outputFileName);
            }
        }
    }
}
