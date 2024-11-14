using System.Security.Claims;

namespace demo_boeing_peoplesoft.Data
{
    public class BoeingClaimsIdentity: ClaimsIdentity
    {
        public BoeingClaimsIdentity(int userId, IEnumerable<Claim>? claims, string? authenticationType)
            : base(claims, authenticationType)
        {
            UserID = userId;
        }

        public int UserID { get; private set; }
    }
}
