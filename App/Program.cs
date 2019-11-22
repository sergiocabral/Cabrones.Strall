using System;
using Strall;
using Strall.Persistence.SQLite;

namespace App
{
    internal static class Program
    {
        private static void Main()
        {
            ((Information?)null).SetDataAccess(
                new PersistenceProviderSqLite()
                    .Open(new ConnectionInfo { Filename = "teste.db" })
                    .CreateStructure());

            var information1 = new Information() as IInformation;
            information1.Id = Guid.Parse("66e35dca-3822-4686-a6a5-885aeb9489e4");
            information1.Get();
            information1.Update();
            
            var information2 = new Information() as IInformation;
            information2.Id = Guid.Parse("b935e71d-57d2-4193-be20-f2122cc0f95f");
            information2.Get();

            information2.ContentFromId = information1.Id;
            information2.UpdateOrCreate();
            information2.Get();

            Console.WriteLine($"{information2.Description}: {information2.Content} ({information2.ContentLoad()})");
        }
    }
}