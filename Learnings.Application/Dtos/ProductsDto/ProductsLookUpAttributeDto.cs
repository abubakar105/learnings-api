using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos.ProductsDto
{
    public class ProductsLookUpAttributeDto
    {
        public Guid ProductsAttributeId { get; set; }
        public string Name { get; set; }
    }
    public class ProductsLookUpAttributesValueDto
    {
        public Guid ProductsAttributeId { get; set; }
        public string Name { get; set; }
        public List<LookupsValues>? Values { get; set; }
    }
    public class LookupsValues
    {
        public Guid ProductsAttributeId { get; set; }
        public string value { get; set; }
    }
    public class LookupRequestDto
    {
        public Guid ProductId { get; set; }
        public List<Guid> ProductsAttributesIds { get; set; }
    }

}
