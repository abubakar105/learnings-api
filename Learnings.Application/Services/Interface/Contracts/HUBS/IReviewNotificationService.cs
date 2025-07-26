using Learnings.Application.Dtos.ProductsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Services.Interface.Contracts.HUBS
{
    public interface IReviewNotificationService
    {
        Task NotifyNewReviewAsync(ReviewDto review);
    }
}
