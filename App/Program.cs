using Strall.Persistence.SQLite;

namespace App
{
    internal static class Program
    {
        private static void Main()
        {
            using var persistence = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            persistence.Open(new ConnectionInfo { Filename = "teste.db" }).CreateStructure();
        }
    }
}