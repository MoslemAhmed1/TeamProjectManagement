using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TeamProjectManagement.Application.Common;

namespace TeamProjectManagement.Infrastructure.Repositories.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortBy, bool descending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            var property = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance); // get T property

            if (property == null)
                return query;

            var parameter = Expression.Parameter(typeof(T), "x"); // x
            var memberAccess = Expression.MakeMemberAccess(parameter, property); // x.Property
            var lambda = Expression.Lambda(memberAccess, parameter); // (x) => x.Property -> expression tree

            var methodName = descending ? "OrderByDescending" : "OrderBy";
            var methodCall = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.PropertyType },
                query.Expression,
                lambda);

            return query.Provider.CreateQuery<T>(methodCall);
        }

        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
