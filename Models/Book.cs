using System.ComponentModel.DataAnnotations;

namespace BookLendAPI.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
    }
}
