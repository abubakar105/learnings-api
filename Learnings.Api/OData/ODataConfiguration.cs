using Learnings.Application.Dtos.ProductsDto;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Learnings.Api.OData
{
    public static class ODataConfiguration
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            var productType = builder.EntityType<AddProductDto>();
            productType.HasKey(d => d.ProductId);

            builder.EntitySet<AddProductDto>("Products");

            return builder.GetEdmModel();
        }
    }
}
