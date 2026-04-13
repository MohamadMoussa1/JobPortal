using System;
using System.Collections.Generic;
using System.Text;

namespace JobPortal.Application.DTOs.common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Jobs { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
