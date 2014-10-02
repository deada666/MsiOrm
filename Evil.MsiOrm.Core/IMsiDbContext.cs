namespace Evil.MsiOrm.Core
{
    public interface IMsiDbContext
    {
        ISession CreateSession();
    }
}
