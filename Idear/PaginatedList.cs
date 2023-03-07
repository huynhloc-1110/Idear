using System;
using System.Linq;
using Idear.Areas.Staff.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace Idear
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex= pageIndex;
            TotalPages = (int)Math.Ceiling(count/(double)pageSize);
            this.AddRange(items);
        }

        public static PaginatedList<T> Create (List<T> source, int pageIndex, int pageSize) 
        {
            var count = source.Count;
            var items = source.Skip((pageIndex-1)*pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        public static PaginatedList<T> Create(List<T> source, int pageIndex)
            => Create(source, pageIndex, 5);

    }
}
