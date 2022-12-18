using AirlineSendAgent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSendAgent.Client
{
    public interface IWebhookClient
    {
        Task SendWebNotificationAsync(FlightDetailChangePayloadDto flightDetailChangePayloadDto);
    }
}
