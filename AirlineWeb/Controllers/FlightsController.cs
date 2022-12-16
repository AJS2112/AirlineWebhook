using AirlineWeb.Data;
using AirlineWeb.Dtos;
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

        public FlightsController(AirlineDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                flightDetail = _mapper.Map(request, flightDetail);
                try
                {
                    _context.SaveChanges();
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
