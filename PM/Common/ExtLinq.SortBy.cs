using System;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PM.Common
{
    public static partial class ExtLinq
    {
        public static IOrderedQueryable<TEntity> SortBy<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, dynamic>> sortPredicate)
            where TEntity : class, new()
        {
            return InvokeSortBy(query, sortPredicate, SortOrder.Ascending);
        }

        public static IOrderedQueryable<TEntity> SortByDescending<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, dynamic>> sortPredicate)
            where TEntity : class, new()
        {
            return InvokeSortBy(query, sortPredicate, SortOrder.Descending);
        }

        private static IOrderedQueryable<TEntity> InvokeSortBy<TEntity>(IQueryable<TEntity> query,
            Expression<Func<TEntity, dynamic>> sortPredicate, SortOrder sortOrder)
            where TEntity : class, new()
        {
            var param = sortPredicate.Parameters[0];
            string propertyName = null;
            Type propertyType = null;
            Expression bodyExpression = null;
            if (sortPredicate.Body is UnaryExpression)
            {
                var unaryExpression = sortPredicate.Body as UnaryExpression;
                bodyExpression = unaryExpression.Operand;
            }
            else if (sortPredicate.Body is MemberExpression)
            {
                bodyExpression = sortPredicate.Body;
            }
            else
                throw new ArgumentException(@"The body of the sort predicate expression should be 
                either UnaryExpression or MemberExpression.", "sortPredicate");
            var memberExpression = (MemberExpression)bodyExpression;
            propertyName = memberExpression.Member.Name;
            if (memberExpression.Member.MemberType == MemberTypes.Property)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null) propertyType = propertyInfo.PropertyType;
            }
            else
                throw new InvalidOperationException(@"Cannot evaluate the type of property since the member expression 
                represented by the sort predicate expression does not contain a PropertyInfo object.");

            var funcType = typeof(Func<,>).MakeGenericType(typeof(TEntity), propertyType);
            var convertedExpression = Expression.Lambda(funcType,
                Expression.Convert(Expression.Property(param, propertyName), propertyType), param);

            var sortingMethods = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static);
            var sortingMethodName = GetSortingMethodName(sortOrder);
            var sortingMethod = sortingMethods.First(sm => sm.Name == sortingMethodName &&
                                                           sm.GetParameters().Length == 2);
            return (IOrderedQueryable<TEntity>)sortingMethod
                .MakeGenericMethod(typeof(TEntity), propertyType)
                .Invoke(null, new object[] { query, convertedExpression });
        }

        private static string GetSortingMethodName(SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    return "OrderBy";
                case SortOrder.Descending:
                    return "OrderByDescending";
                default:
                    throw new ArgumentException("Sort Order must be specified as either Ascending or Descending.",
            "sortOrder");
            }
        }
    }


    public static class QueryableExtension
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            return _OrderBy<T>(query, propertyName, false);
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            return _OrderBy<T>(query, propertyName, true);
        }

        static IOrderedQueryable<T> _OrderBy<T>(IQueryable<T> query, string propertyName, bool isDesc)
        {
            string methodname = (isDesc) ? "OrderByDescendingInternal" : "OrderByInternal";

            var memberProp = typeof(T).GetProperty(propertyName);

            var method = typeof(QueryableExtension).GetMethod(methodname)
                                       .MakeGenericMethod(typeof(T), memberProp.PropertyType);

            return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, memberProp });
        }
        public static IOrderedQueryable<T> OrderByInternal<T, TProp>(IQueryable<T> query, System.Reflection.PropertyInfo memberProperty)
        {//public
            return query.OrderBy(_GetLamba<T, TProp>(memberProperty));
        }
        public static IOrderedQueryable<T> OrderByDescendingInternal<T, TProp>(IQueryable<T> query, System.Reflection.PropertyInfo memberProperty)
        {//public
            return query.OrderByDescending(_GetLamba<T, TProp>(memberProperty));
        }
        static Expression<Func<T, TProp>> _GetLamba<T, TProp>(System.Reflection.PropertyInfo memberProperty)
        {
            if (memberProperty.PropertyType != typeof(TProp)) throw new Exception();

            var thisArg = Expression.Parameter(typeof(T));
            var lamba = Expression.Lambda<Func<T, TProp>>(Expression.Property(thisArg, memberProperty), thisArg);

            return lamba;
        }
    }
}