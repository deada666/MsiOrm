using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Evil.MsiOrm.Core
{
    internal class MsiRepository<T> : IMsiRepository<T>, IDisposableRepository
    {
        private readonly MsiRowToObjectConverter msiRowToObjectConverter;

        private readonly MsiQueryTranslator msiQueryTranslator;

        private readonly string selectAllQuery;

        private readonly List<T> rowsToDelete = new List<T>();

        /// <summary>
        /// All entites that were created by repository.
        /// </summary>
        private readonly Dictionary<T, Record> requestedRecords = new Dictionary<T, Record>();

        private readonly Database database;

        public MsiRepository(MsiRowToObjectConverter converter, Database db)
        {
            msiRowToObjectConverter = converter;
            database = db;
            selectAllQuery = string.Format("SELECT * FROM `{0}`", msiRowToObjectConverter.TableName);
            msiQueryTranslator = new MsiQueryTranslator(msiRowToObjectConverter);
        }

        private T ProcessRecord(Record record)
        {
            var entity = (T)msiRowToObjectConverter.Convert(record);
            requestedRecords.Add(entity, record);
            return entity;
        }

        private IEnumerable<T> ExecuteSql(string sql)
        {
            using (var view = database.OpenView(sql))
            {
                view.Execute();
                Record record;
                do
                {
                    record = view.Fetch();
                    if (record != null)
                    {
                        yield return ProcessRecord(record);
                    }
                }
                while (record != null);
            }
        }

        public IEnumerable<T> Query(Expression<Predicate<T>> expression = null)
        {
            var query = expression == null ? selectAllQuery : msiQueryTranslator.Translate(expression);
            return ExecuteSql(query);
        }

        public void Delete(T entity)
        {
            rowsToDelete.Add(entity);
        }

        public void SaveChanges()
        {
            using (var view = database.OpenView(selectAllQuery))
            {
                foreach (var record in rowsToDelete.Select(x => requestedRecords[x]).ToArray())
                {
                    view.Seek(record);
                    view.Delete(record);
                }
            }
        }

        public void Dispose()
        {
            foreach (var record in requestedRecords.Values)
            {
                record.Dispose();
            }
        }
    }
}
