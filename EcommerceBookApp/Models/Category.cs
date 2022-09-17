using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceBookApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1,100, ErrorMessage="This value must be between 1 and 100. ")]
        public int DisplayOrder { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
