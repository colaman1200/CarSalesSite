using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSalesSite.Models
{


    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required, EmailAddress]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("role")]
        public string? Role { get; set; } = "user";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public virtual ICollection<Favourites> Favourites { get; set; } = new List<Favourites>();

        public void SetPassword(string password)
        {
            Password = BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, Password);
        }



    }

}