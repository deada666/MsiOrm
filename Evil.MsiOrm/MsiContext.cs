using Evil.MsiOrm.Core;
using Microsoft.Deployment.WindowsInstaller;

namespace Evil.MsiOrm
{
    public class MsiContext : MsiDbContext
    {
        public MsiContext(Database db)
            : base(db)
        {
        }

        public override void OnInitializing()
        {
            RegisterTable<FileTable>();
        }
    }
}
