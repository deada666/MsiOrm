using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;

namespace Evil.MsiOrm.Core
{
    internal class MsiSession : ISession
    {
        private readonly Database database;

        private readonly Dictionary<Type, IDisposable> requestedRepositories = new Dictionary<Type, IDisposable>();

        private readonly IDictionary<Type, MsiRowToObjectConverter> tableConverters;

        public MsiSession(Database db, IDictionary<Type, MsiRowToObjectConverter> converters)
        {
            database = db;
            tableConverters = converters;
        }

        public IMsiRepository<T> GetRepository<T>()
        {
            var tableType = typeof(T);
            if (requestedRepositories.ContainsKey(tableType))
            {
                return (IMsiRepository<T>)requestedRepositories[tableType];
            }

            var converter = tableConverters[tableType];
            var repository = (MsiRepository<T>)Activator.CreateInstance(typeof(MsiRepository<T>), converter, database);
            requestedRepositories.Add(tableType, repository);
            return repository;
        }

        public void Dispose()
        {
            foreach (var repository in requestedRepositories.Values)
            {
                repository.Dispose();
            }

            database.Dispose();
        }

        public void SaveChanges()
        {
            database.Commit();
        }
    }
}
