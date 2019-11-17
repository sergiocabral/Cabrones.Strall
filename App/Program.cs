using Strall;
using Strall.Persistence.SQLite;

namespace App
{
    internal static class Program
    {
        private static void Main()
        {
            Information.DataAccessDefault = 
                new PersistenceProviderSqLite()
                    .Open(new ConnectionInfo { Filename = "teste.db" })
                    .CreateStructure();

//            var information1 = new Information() as IInformation;
//            information1.Id = Guid.Parse("df36f419-4534-4709-bfca-863f5c402a12");
//            information1.Get();
//            information1.Update();
//            
//            var information2 = new Information() as IInformation;
//            information2.Id = Guid.Parse("136d803a-dfe1-49c8-bd2d-161dc0c3d401");
//            information2.Get();
//
//            information2.CloneFrom = information1;
//            information2.UpdateOrCreate();
//            information2.Get();
//
//            Console.WriteLine($"{information2.Description}: {information2.Content}");
        }
    }
}