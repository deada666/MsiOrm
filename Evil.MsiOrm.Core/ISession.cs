using System;

namespace Evil.MsiOrm.Core
{
    public interface ISession : IDisposable
    {
        IMsiRepository<T> GetRepository<T>();
    }
}
