using System.Security.Claims;

namespace demo_boeing_peoplesoft.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Get the numeric user ID for the current user
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static int GetUserID(this ClaimsPrincipal principal)
        {
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value ?? "";

            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            return 0;
        }

        public static bool IsAdmin(this ClaimsPrincipal principal)
         {
            var roleClaims = principal.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            if (roleClaims == null || roleClaims.Count == 0)
                return false;
            var isAdmin = roleClaims.Any(r => r.Value == "admin");
            return isAdmin;
        }
    }
}
