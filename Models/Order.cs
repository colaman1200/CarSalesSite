using System.ComponentModel.DataAnnotations;

namespace CarSalesSite.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CarId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        public User User { get; set; } = null!;
        public Car Car { get; set; } = null!;
    }
}