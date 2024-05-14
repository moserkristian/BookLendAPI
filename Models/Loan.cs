using System.ComponentModel.DataAnnotations;

namespace BookLendAPI.Models
{
    public class Loan
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        public DateTime DateBorrowed { get; set; } = DateTime.Now;
        public DateTime? DateReturned { get; set; }
    }
}
