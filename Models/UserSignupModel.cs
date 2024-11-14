using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace demo_boeing_peoplesoft.Models
{
    public class UserSignupModel: UserModel
    {
        public UserSignupModel()
        {
            Password = Password2 = Username = string.Empty;
        }

        public UserSignupModel(UserModel? user)
        {
            if (user != null)
            {
                UserId = user.UserId;
                Username = user.Username;
                CreateDate = user.CreateDate;
                LastLogin = user.LastLogin;
                Email = user.Email;
                Name = user.Name;
                Password = Password2 = string.Empty;
                ProfileImage = user.ProfileImage;
                ProfileImageID = user.ProfileImageID;
                IsAdmin = user.IsAdmin;
                IsDisabled = user.IsDisabled;
            }
        }

        [MinLength(8), MaxLength(255)]
        [Display(Name = "Password (again)")]
        [NotMapped]
        public string? Password2 { get; set; }
    }
}
