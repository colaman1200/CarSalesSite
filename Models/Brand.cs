using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CarSalesSite.Models
{
    public class Brand
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("brand_id")]
        public int BrandId { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; }

    }
}