using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSalesSite.Models
{
    [Table("Favourites")] 
    public class Favourites
    {
        [Key]
        [Column("fav_id")] 
        public int Id { get; set; }

        [Required]
        [Column("user_id")] 
        public int UserId { get; set; }

        [Required]
        [Column("car_id")] 
        public int CarId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [ForeignKey("CarId")]
        public Car Car { get; set; } = null!;
    }
}