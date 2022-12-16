using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AirlineWeb.Dtos
{
    public class FlightDetailCreateDto
    {
        [Required]
        public string FlightCode { get; set; }
        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }
    }
}
