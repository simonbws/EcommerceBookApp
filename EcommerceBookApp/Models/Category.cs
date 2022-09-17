﻿using System.ComponentModel.DataAnnotations;

namespace EcommerceBookApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}