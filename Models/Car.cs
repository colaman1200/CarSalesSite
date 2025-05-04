using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSalesSite.Models
{
    public class Car
    {
        [Key]
        [Column("car_id")]
        public int CarId { get; set; }

        // Валидация связи с брендом
        [ForeignKey("Brand")]
        [Column("brand_id")]
        [Required(ErrorMessage = "Выберите марку автомобиля")]
        [Display(Name = "Марка")]
        public int BrandId { get; set; }

        // Валидация модели
        [Required(ErrorMessage = "Введите модель автомобиля")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длина модели от 2 до 50 символов")]
        [Column("model")]
        [Display(Name = "Модель")]
        public string Model { get; set; }

        // Валидация года выпуска
        [Required(ErrorMessage = "Укажите год выпуска")]
        [Range(1900, 2100, ErrorMessage = "Некорректный год выпуска")]
        [Column("year")]
        [Display(Name = "Год выпуска")]
        public int Year { get; set; }

        // Валидация пробега
        [Required(ErrorMessage = "Укажите пробег")]
        [Range(0, 1_000_000, ErrorMessage = "Пробег должен быть от 0 до 1 000 000 км")]
        [Column("mileage")]
        [Display(Name = "Пробег")]
        public int Mileage { get; set; }

        // Валидация цены
        [Required(ErrorMessage = "Укажите цену")]
        [Range(1, 1_000_000_000, ErrorMessage = "Цена должна быть от 1 до 1 000 000 000 $")]
        [Column("price")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        // Валидация валюты
        [Required(ErrorMessage = "Выберите валюту")]
        [Column("currency")]
        [Display(Name = "Валюта")]
        public string Currency { get; set; }

        // Валидация типа кузова
        [Required(ErrorMessage = "Выберите тип кузова")]
        [Column("body_type")]
        [Display(Name = "Тип кузова")]
        public string BodyType { get; set; }

        // Валидация типа топлива
        [Required(ErrorMessage = "Выберите тип топлива")]
        [Column("fuel_type")]
        [Display(Name = "Тип топлива")]
        public string FuelType { get; set; }

        // Валидация коробки передач
        [Required(ErrorMessage = "Выберите тип коробки передач")]
        [Column("transmission")]
        [Display(Name = "Коробка передач")]
        public string Transmission { get; set; }

        // Валидация привода
        [Required(ErrorMessage = "Выберите тип привода")]
        [Column("drive_type")]
        [Display(Name = "Привод")]
        public string DriveType { get; set; }

        // Валидация объема двигателя
        [Required(ErrorMessage = "Укажите объем двигателя")]
        [Range(0.5, 10.0, ErrorMessage = "Объем двигателя от 0.5 до 10.0 л")]
        [Column("engine_volume")]
        [Display(Name = "Объем двигателя")]
        public decimal EngineVolume { get; set; }

        // Валидация мощности
        [Required(ErrorMessage = "Укажите мощность")]
        [Range(50, 2000, ErrorMessage = "Мощность от 50 до 2000 л.с.")]
        [Column("power_hp")]
        [Display(Name = "Мощность")]
        public int PowerHp { get; set; }

        // Валидация цвета
        [Required(ErrorMessage = "Укажите цвет автомобиля")]
        [Column("color")]
        [Display(Name = "Цвет")]
        public string Color { get; set; }

        // Валидация страны происхождения
        [Required(ErrorMessage = "Выберите страну происхождения")]
        [Column("country_origin")]
        [Display(Name = "Страна")]
        public string CountryOrigin { get; set; }

        // Валидация описания
        [StringLength(2000, ErrorMessage = "Описание не должно превышать 2000 символов")]
        [Column("description")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Column("photos")]
        public string Photos { get; set; } = string.Empty;

        [NotMapped]
        public List<string> PhotoUrls =>
            !string.IsNullOrEmpty(Photos)
                ? Photos.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                : new List<string>();

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual Brand Brand { get; set; }
    }
}