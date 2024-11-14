using Amazon.S3;
using Amazon.S3.Model;
using demo_boeing_peoplesoft.Data;
using demo_boeing_peoplesoft.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace demo_boeing_peoplesoft.Controllers
{
    public class AccountController : Controller
    {
        const string BlogBucketName = "klfblogfs";

        public AccountController(IAmazonS3 amazonS3, BoeingDbContext blogDbContext) : base() {
            S3Client = amazonS3;
            BlogDbContext = blogDbContext;
        }

        private readonly IAmazonS3 S3Client;

        private readonly BoeingDbContext BlogDbContext;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserSignupModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserSignupModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (user != null)
                    {
                        if (user.Password == user.Password2)
                        {
                            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                            user.Password = hashedPassword;
                            user.Email = user.Email.ToLowerInvariant();
                            user.CreateDate = DateTime.Now;
                            user.LastLogin = DateTime.Now;

                            var userExists = BlogDbContext.Users.Any(u => u.Email == user.Email || u.Username == user.Username);

                            if (!userExists)
                            {
                                BlogDbContext.Users.Add(user);
                                BlogDbContext.SaveChanges();

                                await DoLogin(user);

                                return RedirectToAction("Index", "Home");
                            }
                            else
                                throw new ArgumentException("Username or email in use");
                        }
                        else
                            throw new ArgumentException("Passwords do not match");
                    }
                }
            }
            catch(Exception ex)
            {
                user.Error = ex.Message;
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new UserModel());
        }

        /// <summary>
        /// Create an authentication token and log the user in
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task DoLogin(UserModel user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, user.Username!));
            if (!string.IsNullOrEmpty(user.Name))
                claims.Add(new Claim(ClaimTypes.GivenName, user.Name));
            claims.Add(new Claim("UserID", user.UserId.ToString()));

            var identity = new BoeingClaimsIdentity(user.UserId, claims, CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, "password"));
            identity.AddClaim(new Claim(ClaimTypes.Authentication, "true"));

            if (user.IsAdmin)
            {
                identity.AddClaim(new Claim(identity.RoleClaimType, "admin"));
                identity.AddClaim(new Claim(identity.RoleClaimType, "user"));
            }
            else
            {
                identity.AddClaim(new Claim(identity.RoleClaimType, "user"));
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserModel model)
        {
            try
            {
                var user = BlogDbContext.Users.SingleOrDefault(u => u.Username == model.Username);
                if (user != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                    {
                        await DoLogin(user);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        throw new AuthenticationFailureException("Incorrect password");
                }
                else
                    throw new AuthenticationFailureException("Unknown User");
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
                model.Password = string.Empty;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public IActionResult Profile()
        {
            var user = BlogDbContext.Users.SingleOrDefault(u => u.Username == User.Identity!.Name!);
            var userModel = new UserSignupModel(user);
            return View(userModel);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UploadImage(IFormFile uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.Length > 0) {
                using (var ms = new MemoryStream()) {
                    await uploadedFile.CopyToAsync(ms);
                    var mimeType = MimeMapping.MimeUtility.GetMimeMapping(uploadedFile.FileName);
                    var request = new PutObjectRequest()
                    {
                        BucketName = BlogBucketName,
                        Key = $"profile-pictures/{User.Identity.Name}/{uploadedFile.FileName}",
                        InputStream = ms,
                        ContentType = mimeType
                    };

                    var resp = await S3Client.PutObjectAsync(request);
                }
            }
            return RedirectToAction("Profile");
        }
    }
}
