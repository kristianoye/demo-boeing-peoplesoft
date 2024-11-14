using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo_boeing_peoplesoft.Models
{
    [PrimaryKey("BlogCategoryId", "BlogEntryId")]
    public class BlogEntryCategory
    {
        [Key, Column(Order = 0)]
        public int BlogCategoryId { get; set; }

        public BlogCategory BlogCategory { get; set; }

        [Key, Column(Order = 1)]
        public int BlogEntryId { get; set; }

        public BlogEntry BlogEntry { get; set; }
    }
}
