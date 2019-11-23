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

            var information1 = new Information() as IInformation;
            information1.Id = Guid.Parse("95579031-B1BA-480E-9189-DE8EBD09C8D7");
            information1.Get();
            information1.Update();
            
            var information2 = new Information() as IInformation;
            information2.Id = Guid.Parse("5005D67F-4D7C-44D7-9EC0-441ADC6F6E2D");
            information2.Get();

            information2.ContentFromId = information1.Id;
            information2.UpdateOrCreate();
            information2.Get();

            Console.WriteLine($"{information2.Description}: {information2.Content} ({information2.ContentLoad()})");
        }
    }
}