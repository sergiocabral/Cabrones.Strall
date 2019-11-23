using System;
using Strall;
using Strall.Persistence.SqlServer;

namespace App
{
    internal static class Program
    {
        private static void Main()
        {
            ((Information?)null).SetDataAccess(
                new PersistenceProviderSqlServer()
                    .Open(new SqlServerConnectionInfo { Database = "strall" })
                    .CreateStructure());

            var information1 = new Information {Id = Guid.Parse("2E6CF730-BF70-487F-A9B9-4B2897576C7A")}.Get().UpdateOrCreate();
            
            Console.WriteLine($"information1: {information1.Description}: {information1.Content}");
            
            var information2 = new Information {Id = Guid.Parse("4F081AA5-718F-4CE4-90B9-B8A244627DFF"), ContentFromId = information1.Id}.Get().UpdateOrCreate();

            information2.Content = $"Tempo {DateTime.Now.Second}";

            Console.WriteLine($"information2: {information2.Description}: {information2.Content}");

            information2.Update();
        }
    }
}