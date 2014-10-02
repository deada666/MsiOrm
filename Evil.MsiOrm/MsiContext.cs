using Evil.MsiOrm.Core;
using Microsoft.Deployment.WindowsInstaller;

namespace Evil.MsiOrm
{
    public class MsiContext : MsiDbContext
    {
        public MsiContext(string dbPath)
            : base(dbPath)
        {
        }

        public MsiContext(string dbPath, DatabaseOpenMode openMode)
            : base(dbPath, openMode)
        {
        }

        protected override void OnInitializing()
        {
            RegisterTable<FileTable>();
        }
    }
}
