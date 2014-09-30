namespace Evil.MsiOrm.Core
{
    public interface IMsiDbContext
    {
        IMsiRepository<T> GetRepository<T>();
    }
}
