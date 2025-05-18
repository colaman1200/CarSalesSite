using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSalesSite.Models
{
    public class CarHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public int CarId { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public EventType EventType { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [ForeignKey("CarId")]
        public Car? Car { get; set; }
    }

    public enum EventType
    {
        Sale,
        Maintenance,
        Accident
    }
}