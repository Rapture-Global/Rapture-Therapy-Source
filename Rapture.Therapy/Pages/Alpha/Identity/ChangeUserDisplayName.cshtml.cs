using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Eadent.Identity.DataAccess.EadentUserIdentity.Entities;
using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rapture.Therapy.Configuration;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class ChangeUserDisplayNameModel : BasePageModel
    {
        public string Message { get; set; }

        [BindProperty]
        public string NewDisplayName { get; set; }

        public ChangeUserDisplayNameModel(ILogger<ChangeUserDisplayNameModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            IActionResult actionResult = Page();

            (bool success, decimal googleReCaptchaScore) = await GoogleReCaptcha();

            GoogleReCaptchaScore = googleReCaptchaScore;

            if (googleReCaptchaScore < RaptureTherapySettings.Instance.GoogleReCaptcha.MinimumScore)
            {
                Message = "You are unable to Change the Display Name because of a poor Google ReCaptcha Score.";
            }
            else if (action == "Change Display Name")
            {
                (ChangeUserDisplayNameStatus changeUserDisplayNameStatusId, UserSessionEntity userSessionEntity) = await EadentUserIdentity.ChangeUserDisplayNameAsync(UserSession.SessionToken, NewDisplayName, HttpHelper.GetRemoteIpAddress(Request), googleReCaptchaScore, HttpContext.RequestAborted);

                Message = $"ChangeUserDisplayNameStatusId = {changeUserDisplayNameStatusId}";

                RefreshUserSession(userSessionEntity);
            }

            return actionResult;
        }
    }
}
