using Neembly.BOIDServer.SharedClasses.Pagination;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Neembly.BOIDServer.SharedClasses.QueryExtension
{
    public static class IQueryableExtension
    {
        public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> query, int pageIndex, int pageSize = 20)
        {
            var count = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<T>(items, count, pageIndex, pageSize);
        }
    }
}
