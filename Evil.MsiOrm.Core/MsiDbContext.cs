using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;

namespace Evil.MsiOrm.Core
{
    public abstract class MsiDbContext : IMsiDbContext
    {
        private readonly Dictionary<Type, MsiRowToObjectConverter> tableConverters = new Dictionary<Type, MsiRowToObjectConverter>();

        private readonly Database database;

        protected MsiDbContext(Database db)
        {
            database = db;
            OnInitializing();
        }

        protected abstract void OnInitializing();

        protected void RegisterTable<T>()
        {
            var tableType = typeof(T);
            var converter = new MsiRowToObjectConverter(tableType);
            tableConverters.Add(tableType, converter);
        }

        public ISession CreateSession()
        {
            return new MsiSession(database, tableConverters);
        }
    }
}
