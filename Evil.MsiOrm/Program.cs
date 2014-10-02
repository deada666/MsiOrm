using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Linq;

namespace Evil.MsiOrm
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = Cool();
            var context = new MsiContext(@"c:\TestMSi.msi", DatabaseOpenMode.Transact);
            using (var session = context.CreateSession())
            {
                var repository = session.GetRepository<FileTable>();
                var table = repository.Query(x => x.Sequence > a && x.Language == null).ToArray();
                foreach (var row in table)
                {
                    Console.WriteLine(row.File);
                    repository.Delete(row);
                }

                //session.SaveChanges();
            }

            using (var session = context.CreateSession())
            {
                var repository = session.GetRepository<FileTable>();
                var table = repository.Query(x => x.Sequence > a && x.Language == null).ToArray();
                foreach (var row in table)
                {
                    Console.WriteLine(row.File);
                }
            }
            
            Console.ReadKey(true);
        }

        private static int Cool()
        {
            return 1;
        }
    }
}
