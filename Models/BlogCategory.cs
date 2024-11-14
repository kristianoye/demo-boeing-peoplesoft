using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo_boeing_peoplesoft.Models
{
    public class BlogCategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogCategoryId { get; set; }

        [Required, MinLength(1)]
        public string Name { get; set; } = string.Empty;

        public BlogCategory? ParentCategory { get; set; } = null;

        public int? ParentCategoryId { get; set; } = null;

        public ICollection<BlogEntryCategory> BlogEntryCategories { get; set; }
    }
}
