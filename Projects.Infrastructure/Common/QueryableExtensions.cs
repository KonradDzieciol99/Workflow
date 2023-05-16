using System.Linq.Expressions;
using System.Reflection;

namespace Projects.Infrastructure.Common
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyOrFieldName, bool ascending)
        {
            var type = typeof(T);

            var orderByMethodName = ascending ? "OrderBy" : "OrderByDescending";

            var property = type.GetProperty(propertyOrFieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                throw new ArgumentException($"Property {propertyOrFieldName} not found on type {type.FullName}");
            }

            var parameterExpression = Expression.Parameter(type);

            Expression propertyOrFieldExpression = Expression.PropertyOrField(parameterExpression, propertyOrFieldName);

            var selector = Expression.Lambda(propertyOrFieldExpression, parameterExpression);

            var orderByExpression = Expression.Call(typeof(Queryable), orderByMethodName,
                new[] { type, propertyOrFieldExpression.Type }, queryable.Expression, selector);

            return queryable.Provider.CreateQuery<T>(orderByExpression);
        }
    }
}
