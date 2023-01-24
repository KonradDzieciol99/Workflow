using System.Linq.Expressions;

namespace WorkflowApi.Extensions
{
    public static class QueryableExtensions
    {
        //public static IQueryable<Message> MarkUnreadAsRead(this IQueryable<Message> query, string currentUsername)
        //{
        //    var unreadMessages = query.Where(m => m.DateRead == null
        //        && m.RecipientUsername == currentUsername);

        //    if (unreadMessages.Any())
        //    {
        //        foreach (var message in unreadMessages)
        //        {
        //            message.DateRead = DateTime.UtcNow;
        //        }
        //    }

        //    return query;
        //}
        //public static IOrderedQueryable<TSource> OrderByWithDirection<TSource, TKey>(this IQueryable<TSource> source,
        //    Func<TSource, TKey> keySelector,bool descending)
        //{
        //    return descending ? source.OrderByDescending(keySelector)
        //                      : source.OrderBy(keySelector);
        //}

        //public static IOrderedQueryable<TSource> OrderByWithDirection<TSource, TKey>(this IQueryable<TSource> source,
        //     Expression<Func<TSource, TKey>> keySelector,
        //     bool descending)
        //{
        //    return descending ? source.OrderByDescending(keySelector)
        //                      : source.OrderBy(keySelector);
        //}
        public static IQueryable<T> OrderByPropertyOrField<T>(this IQueryable<T> queryable, string propertyOrFieldName, bool ascending = true)
        {
            var elementType = typeof(T);
            var orderByMethodName = ascending ? "OrderBy" : "OrderByDescending";

            var parameterExpression = Expression.Parameter(elementType);
            var propertyOrFieldExpression = Expression.PropertyOrField(parameterExpression, propertyOrFieldName);
            var selector = Expression.Lambda(propertyOrFieldExpression, parameterExpression);

            var orderByExpression = Expression.Call(typeof(Queryable), orderByMethodName,
                new[] { elementType, propertyOrFieldExpression.Type }, queryable.Expression, selector); ///<-- tutaj wywołuje  OrderBy albo OrderByDescending

            return queryable.Provider.CreateQuery<T>(orderByExpression);
        }
        public static IQueryable<IGrouping<object, T>> GroupByPropertyOrField<T>(this IQueryable<T> queryable, string propertyOrFieldName)
        {
            var elementType = typeof(T);

            var parameterExpression = Expression.Parameter(elementType);
            var propertyOrFieldExpression = Expression.PropertyOrField(parameterExpression, propertyOrFieldName);

            var convertedpropertyOrFieldExpression = Expression.Convert(propertyOrFieldExpression, typeof(object));

            var selector = Expression.Lambda(convertedpropertyOrFieldExpression, parameterExpression);

            var orderByExpression = Expression.Call(typeof(Queryable), "GroupBy",
                new[] { elementType, convertedpropertyOrFieldExpression.Type }, queryable.Expression, selector);

            return queryable.Provider.CreateQuery<IGrouping<object, T>>(orderByExpression);
        }
    }
}
