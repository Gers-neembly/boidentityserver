using System;
using System.Collections.Generic;
using System.Text;

namespace Neembly.BOIDServer.SharedClasses.Pagination
{
    [Serializable]
    public class PagedResult<T>
    {
        #region Properties 
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<T> Result { get; set; }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }
        #endregion

        #region Constructor

        public PagedResult(IEnumerable<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalItems = count;
            this.Result = items;
        }

        #endregion
    }
}
