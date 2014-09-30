using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Evil.MsiOrm.Core
{
    internal class MsiRepository<T> : IMsiRepository<T>
    {
        private readonly MsiRowToObjectConverter msiRowToObjectConverter;

        private readonly MsiQueryTranslator msiQueryTranslator;

        private readonly Database database;

        public MsiRepository(MsiRowToObjectConverter converter, Database db)
        {
            msiRowToObjectConverter = converter;
            database = db;
            msiQueryTranslator = new MsiQueryTranslator(msiRowToObjectConverter);
        }

        public IEnumerable<T> GetRowCollection()
        {
            return ExecuteSql("SELECT * FROM `" + msiRowToObjectConverter.TableName + "`");
        }

        private T ProcessRecord<T>(Record record, MsiRowToObjectConverter converter)
        {
            using (record)
            {
                return (T)converter.Convert(record);
            }
        }

        private IEnumerable<T> ExecuteSql(string sql)
        {
            using (var view = database.OpenView(sql))
            {
                view.Execute();
                return view.Select(x => ProcessRecord<T>(x, msiRowToObjectConverter)).ToArray();
            }
        }

        public IEnumerable<T> Query(Expression<Predicate<T>> expression)
        {
            var query = msiQueryTranslator.Translate(expression);
            return ExecuteSql(query);
        }
    }
}
