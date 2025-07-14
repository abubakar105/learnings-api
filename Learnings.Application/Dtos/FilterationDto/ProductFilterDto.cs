using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos.FilterationDto
{
    public class ProductFilterDto
    {
        public string? Search { get; set; }        
        public List<Guid>? CategoryIds { get; set; } 
        public string? Gender { get; set; }        
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }      
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

}
