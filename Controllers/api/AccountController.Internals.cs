using demo_boeing_peoplesoft.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace demo_boeing_peoplesoft.Controllers.api
{
    public partial class AccountController : ControllerBase
    {
        public AccountController(BoeingDbContext blogContext) {
            _blogContext = blogContext;
        }

        private readonly BoeingDbContext _blogContext;
    }
}
