using System;
using System.Globalization;
using System.Linq;
using Strall;
using Strall.Persistence.SQLite;

namespace App
{
    internal static class Program
    {
        private static void Main()
        {
            using var persistence = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            persistence.Open(new ConnectionInfo { Filename = "teste.db" }).CreateStructure();

            var information = new Information();
            information.Id = persistence.Create(information);
            information.Content = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            persistence.Update(information);
            var exists = persistence.Exists(information.Id);
            var hasChildren = persistence.HasChildren(information.Id);
            persistence.Delete(information.Id);

            information = persistence.Get(Guid.Parse("F1BF7183-1852-4D26-AA3F-828C7F503324"));
            exists = persistence.Exists(information.Id);
            hasChildren = persistence.HasChildren(information.Id);
            var children = persistence.Children(information.Id);
            var children2 = children.ToList();
        }
    }
}