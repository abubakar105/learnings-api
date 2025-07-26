using Learnings.Api.Hubs;
using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.Services.Interface.Contracts.HUBS;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Infrastructure.Services.CONTRACTS.HUBS
{
    public class ReviewNotificationService : IReviewNotificationService
    {
        private readonly IHubContext<Api.Hubs.ReviewHub, IReviewClient> _hub;

        public ReviewNotificationService(IHubContext<Api.Hubs.ReviewHub, IReviewClient> hub)
        {
            _hub = hub;
        }

        public Task NotifyNewReviewAsync(ReviewDto review)
        {
            return _hub.Clients.All.NewReview(review);
        }
    }
}
