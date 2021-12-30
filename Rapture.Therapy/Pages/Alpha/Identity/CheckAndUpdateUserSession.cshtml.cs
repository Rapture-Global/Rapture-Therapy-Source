using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Eadent.Identity.DataAccess.EadentUserIdentity.Entities;
using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class CheckAndUpdateUserSessionModel : BasePageModel
    {
        public string Message { get; set; }

        public CheckAndUpdateUserSessionModel(ILogger<CheckAndUpdateUserSessionModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string action)
        {
            IActionResult actionResult = Page();

            if (action == "Check")
            {
                (SessionStatus sessionStatusId, UserSessionEntity userSessionEntity) = EadentUserIdentity.CheckAndUpdateUserSession(UserSession.SessionToken, HttpHelper.GetRemoteAddress(Request));

                Message = $"SessionStatusId = {sessionStatusId}";
            }
            else if (action == "Sign Out")
            {
                SignOutStatus signOutStatusId = EadentUserIdentity.SignOutUser(UserSession.SessionToken, HttpHelper.GetRemoteAddress(Request));

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
