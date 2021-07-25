using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SAM.Web.Common
{
    public class CustonQuery<T> where T : class 
    {
        private CustonQuery() { }

        public static IQueryable<T> CreateCustonQueryIQueryables(DbContext context, String sqlQuery)
        {
            return (IQueryable<T>)context.Set<T>().SqlQuery(sqlQuery).AsNoTracking().AsQueryable();
        }

        public static IQueryable CreateCustonQueryIQueryable(DbContext context, String sqlQuery)
        {
            return (IQueryable)context.Set<T>().SqlQuery(sqlQuery).AsNoTracking().AsQueryable();
        }
    }
}