using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo_boeing_peoplesoft.Models
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MinLength(3), MaxLength(20)]
        public string? Username { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Email Address")]
        [MaxLength(100)]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(255)]
        public string? Password { get; set; }

        [ForeignKey(nameof(MediaFile))]
        public int? ProfileImageID { get; set; }

        public MediaFile? ProfileImage { get; set; }

        [Display(Name = "Signup Date")]
        [Column(TypeName = "date")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "Last Login")]
        [Column(TypeName = "date")]
        public DateTime? LastLogin { get; set; }

        [Display(Name = "Disabled")]
        public bool IsDisabled { get; set; }

        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }

        [NotMapped]
        public string? Error { get; set; }
    }
}
