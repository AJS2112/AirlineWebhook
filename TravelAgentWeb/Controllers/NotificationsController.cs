using Microsoft.AspNetCore.Mvc;
using TravelAgentWeb.Data;
using TravelAgentWeb.Models;

namespace TravelAgentWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly TravelAgentDbContext _context;
        public NotificationsController(TravelAgentDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult FlightChanged(FlightUpdateDetailDto request)
        {
            Console.WriteLine($"Webhook received from: {request.Publisher}");

            var secretModel = _context.SubscriptionSecrets.FirstOrDefault(s => 
                s.Publisher == request.Publisher && 
                s.Secret == request.Secret);

            if (secretModel == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Secret - Ignored Webhook");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Valid Webhook!");
                Console.WriteLine($"Old Price: {request.OldPrice}, New Price: {request.NewPrice}");

                Console.ResetColor();
            }

            return Ok();
        }
    }
}
