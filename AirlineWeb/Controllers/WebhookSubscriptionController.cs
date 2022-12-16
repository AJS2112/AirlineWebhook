using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookSubscriptionController : ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;

        public WebhookSubscriptionController(AirlineDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;   
        }

        [HttpGet("{secret}", Name = "GetSubscriptionBySecret")]
        public ActionResult<WebhookSubscriptionReadDto> GetSubscriptionBySecret(string secret)
        {
            var subscription = _context.WebhookSubscriptions.FirstOrDefault(s => s.Secret == secret);
            if (subscription == null)
            {
                return NotFound();
            } 

            var response = _mapper.Map<WebhookSubscriptionReadDto>(subscription);
            return Ok(response);    
        }

        [HttpPost]
        public ActionResult<WebhookSubscriptionReadDto> CreateSubscription(WebhookSubscriptionCreateDto request)
        {
            var subscription = _context.WebhookSubscriptions.FirstOrDefault(s => s.WebhookURI == request.WebhookURI);

            if(subscription == null)
            {
                subscription = _mapper.Map<WebhookSubscription>(request);
                subscription.Secret = Guid.NewGuid().ToString();
                subscription.WebhookPublisher = "LatamAirlines";
                try
                {
                    _context.WebhookSubscriptions.Add(subscription);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                var response = _mapper.Map<WebhookSubscriptionReadDto>(subscription);
                return CreatedAtRoute(nameof(GetSubscriptionBySecret), new { secret = response.Secret }, response);
            }
            else
            {
                return NoContent();
            }
        }

        
    }
}
