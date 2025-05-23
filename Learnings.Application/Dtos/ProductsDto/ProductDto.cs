using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos.ProductsDto
{
    public class AddProductDto
    {
        public Guid? ProductId { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public List<CategoriesDto> CategoryIds { get; set; } = new();
        public List<AttributeValueDto> Attributes { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new();
    }

    public class CategoriesDto
    {
        public Guid parentCategoryId { get; set; }
        public Guid childCategoryId { get; set; }
    }
    public class AttributeValueDto
    {
        public Guid AttributeTypeId { get; set; }
        public string Value { get; set; }
    }
}
