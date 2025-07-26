using Learnings.Application.Services.Interface.Contracts.HUBS;
using Microsoft.AspNetCore.SignalR;

namespace Learnings.Api.Hubs
{
    public class ReviewHub : Hub<IReviewClient>
    {}
}
