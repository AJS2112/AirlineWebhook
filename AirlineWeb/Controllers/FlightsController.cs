using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.MessageBus;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public FlightsController(AirlineDbContext context, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _context = context;
            _mapper = mapper;
            _messageBusClient = messageBusClient;   
        }

        [HttpGet("{flightCode}", Name = "GetFlightDetailByCode")]
        public ActionResult<FlightDetailReadDto> GetFlightDetailByCode(string flightCode)
        {
            var flightDetail = _context.FlightDetails.FirstOrDefault(s => s.FlightCode == flightCode);
            if (flightDetail == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<FlightDetailReadDto>(flightDetail);
            return Ok(response);
        }

        [HttpPost]
        public ActionResult<FlightDetailReadDto> CreateFlightDetail(FlightDetailCreateDto request)
        {
            var flightDetail = _context.FlightDetails.FirstOrDefault(s => s.FlightCode == request.FlightCode);

            if (flightDetail == null)
            {
                flightDetail = _mapper.Map<FlightDetail>(request);
                try
                {
                    _context.FlightDetails.Add(flightDetail);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                var response = _mapper.Map<FlightDetailReadDto>(flightDetail);
                return CreatedAtRoute(nameof(GetFlightDetailByCode), new { flightCode = response.FlightCode }, response);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPut("{id}")]
        public ActionResult<FlightDetailReadDto> UpdateFlightDetail(int id, FlightDetailUpdateDto request)
        {
            var flightDetail = _context.FlightDetails.FirstOrDefault(s => s.Id == id);

            if (flightDetail == null)
            {
                return NotFound();   
            }
            else
            {
                decimal oldPrice = flightDetail.Price;

                flightDetail = _mapper.Map(request, flightDetail);
                try
                {
                    _context.SaveChanges();
                    if (oldPrice != flightDetail.Price)
                    {
                        Console.WriteLine("Price changed - place message on bus");
                        var message = new NotificationMessageDto { 
                            WebhookType = "pricechange",
                            NewPrice = flightDetail.Price,
                            OldPrice = oldPrice,
                            FlightCode = flightDetail.FlightCode
                        };

                        _messageBusClient.SendMessage(message);

                    } else
                    {
                        Console.WriteLine("No price change");
                    }

                    return NoContent();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
