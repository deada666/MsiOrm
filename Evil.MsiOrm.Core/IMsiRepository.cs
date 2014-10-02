using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Evil.MsiOrm.Core
{
    public interface IMsiRepository<T>
    {
        IEnumerable<T> Query(Expression<Predicate<T>> expression = null);

        void Delete(T entity);
    }
}
