using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Mvc;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class SignOutUserModel : BasePageModel
    {
        public string Message { get; set; }

        public SignOutUserModel(ILogger<SignOutUserModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            IActionResult actionResult = Page();

            if (action == "Sign Out")
            {
                SignOutStatus signOutStatusId = await EadentUserIdentity.SignOutUserAsync(UserSession.SessionToken, HttpHelper.GetRemoteIpAddress(Request), HttpContext.RequestAborted);

                if (signOutStatusId != SignOutStatus.Error)
                {
                    UserSession.SignOut();
                }

                Message = $"SignOutStatusId = {signOutStatusId}";
            }

            return actionResult;
        }
    }
}
