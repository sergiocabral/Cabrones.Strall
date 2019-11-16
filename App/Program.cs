using System;
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

            var information1 = new Information() as IInformation;
            information1.Id = Guid.Parse("df36f419-4534-4709-bfca-863f5c402a12");
            information1.Get();
            
            var information2 = new Information() as IInformation;
            information2.Id = Guid.Parse("136d803a-dfe1-49c8-bd2d-161dc0c3d401");
            information2.Get();

            information2.CloneFrom = information1;
            information2.UpdateOrCreate();
            information2.Get();

            
            Console.WriteLine($"{information2.Description}: {information2.Content}");


//            var information = new Information();
//            information.Id = persistence.Create(information);
//            information.Content = DateTime.Now.ToString(CultureInfo.InvariantCulture);
//            persistence.Update(information);
//            var exists = persistence.Exists(information.Id);
//            var hasChildren = persistence.HasChildren(information.Id);
//            persistence.Delete(information.Id);
//
//            information = persistence.Get(Guid.Parse("F1BF7183-1852-4D26-AA3F-828C7F503324"));
//            exists = persistence.Exists(information.Id);
//            hasChildren = persistence.HasChildren(information.Id);
//            var children = persistence.Children(information.Id);
//            var children2 = children.ToList();
        }
    }
}