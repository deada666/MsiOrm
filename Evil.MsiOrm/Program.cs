using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evil.MsiOrm
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new Database(@"C:\InstEd-1.5.15.26.msi");
            var a = Cool();
            var context = new MsiContext(db);
            var repository = context.GetRepository<FileTable>();
            var table = repository.Query(x => x.Sequence > a && x.Language == null);
            foreach (var row in table)
            {
                Console.WriteLine(row.File);
            }

            Console.ReadKey(true);
        }

        private static int Cool()
        {
            return 1;
        }
    }
}
