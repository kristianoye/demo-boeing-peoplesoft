using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace demo_boeing_peoplesoft.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class AccountController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("createAccount")]
        public void CreateAccount()
        {

        }

        [AllowAnonymous]
        [HttpGet("emailExists/{emailAddress}")]
        public async Task<JsonResult> EmailExists(string emailAddress)
        {
            try
            {
                var result = await _blogContext.Users.AnyAsync(u => u.Email == emailAddress);
                return new JsonResult(new
                {
                    emailAddress,
                    emailExists = result
                });
            }
            catch(Exception ex)
            {
                return new JsonResult(new
                {
                    emailAddress,
                    error = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("userExists/{username}")]
        public async Task<JsonResult> UserExists(string username)
        {
            try
            {
                var result = await _blogContext.Users.AnyAsync(u => u.Username == username);
                return new JsonResult(new
                {
                    username,
                    usernameExists = result
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    username,
                    error = ex.Message
                });
            }
        }
    }
}
