using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo_boeing_peoplesoft.Models
{
    [Index("UrlSlug")]
    public class BlogEntry
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogEntryId { get; set; }


        [Required, MaxLength(255), MinLength(3)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string UrlSlug { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        public UserModel User { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public DateTime? DatePublished { get; set; } = null;

        public DateTime? DateUpdated { get; set; } = null;

        public ICollection<BlogEntryCategory> BlogEntryCategories { get; set; }
    }
}
