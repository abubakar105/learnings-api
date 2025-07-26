using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos.ProductsDto
{
    public class UpdateReviewDto
    {
        [Required] public Guid ReviewId { get; set; }
        [Range(1, 5)] public int Rating { get; set; }
        [MaxLength(200)] public string Title { get; set; }
        [Required, MaxLength(2000)] public string Body { get; set; }
    }
}
