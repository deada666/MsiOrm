using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Evil.MsiOrm.Core
{
    internal class MsiRepository<T> : IMsiRepository<T>, IDisposable
    {
        private readonly MsiRowToObjectConverter msiRowToObjectConverter;

        private readonly MsiQueryTranslator msiQueryTranslator;

        /// <summary>
        /// All entites that were created by repository.
        /// </summary>
        private readonly Dictionary<T, Record> requestedRecords = new Dictionary<T, Record>();

        /// <summary>
        /// All opened views that were opened during lifetime of repository.
        /// </summary>
        private readonly List<View> openedViews = new List<View>();

        /// <summary>
        /// Dictionary of all records that were requested from repository.
        /// </summary>
        private readonly Dictionary<Record, View> recordView = new Dictionary<Record, View>();

        private readonly Database database;

        public MsiRepository(MsiRowToObjectConverter converter, Database db)
        {
            msiRowToObjectConverter = converter;
            database = db;
            msiQueryTranslator = new MsiQueryTranslator(msiRowToObjectConverter);
        }

        private T ProcessRecord(View view, Record record, MsiRowToObjectConverter converter)
        {
            recordView.Add(record, view);
            var entity = (T)converter.Convert(record);
            requestedRecords.Add(entity, record);
            return entity;
        }

        private IEnumerable<T> ExecuteSql(string sql)
        {
            var view = database.OpenView(sql);
            openedViews.Add(view);
            view.Execute();
            return view.Select(x => ProcessRecord(view, x, msiRowToObjectConverter));
        }

        public IEnumerable<T> Query(Expression<Predicate<T>> expression = null)
        {
            var query = expression == null ? 
                "SELECT * FROM `" + msiRowToObjectConverter.TableName + "`" 
                : msiQueryTranslator.Translate(expression);
            return ExecuteSql(query);
        }

        public void Dispose()
        {
            foreach (var record in recordView.Keys)
            {
                record.Dispose();
            }

            foreach (var view in openedViews)
            {
                view.Dispose();
            }
        }
    }
}
