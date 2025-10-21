using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Services.ServiceBus
{
    public interface IServiceBusService
    {
        Task SendMessageAsync<T>(string queueName, T message);
    }
}
