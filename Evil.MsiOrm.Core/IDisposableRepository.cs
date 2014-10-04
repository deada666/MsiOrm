using System;

namespace Evil.MsiOrm.Core
{
    internal interface IDisposableRepository : IDisposable
    {
        void SaveChanges();
    }
}
