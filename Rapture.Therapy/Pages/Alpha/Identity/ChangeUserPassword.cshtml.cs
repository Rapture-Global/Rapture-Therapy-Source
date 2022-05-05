using System.Threading.Tasks;
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
    public class ChangeUserPasswordModel : BasePageModel
    {
        public string Message { get; set; }

        [BindProperty]
        public string OldPassword { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public ChangeUserPasswordModel(ILogger<ChangeUserPasswordModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string action)
        {
            IActionResult actionResult = Page();

            (bool success, decimal googleReCaptchaScore) = await GoogleReCaptcha();

            GoogleReCaptchaScore = googleReCaptchaScore;

            if (googleReCaptchaScore < RaptureTherapySettings.GoogleReCaptcha.MinimumScore)
            {
                Message = "You are unable to Change the Password because of a poor Google ReCaptcha Score.";
            }
            else if (action == "Change Password")
            {
                if (NewPassword != ConfirmPassword)
                {
                    Message = "Both New Passwords Must Match.";
                }
                else
                {
                    (ChangeUserPasswordStatus changeUserPasswordStatusId, UserSessionEntity userSessionEntity) = EadentUserIdentity.ChangeUserPassword(UserSession.SessionToken, OldPassword, NewPassword, HttpHelper.GetRemoteIpAddress(Request), googleReCaptchaScore);

                    Message = $"ChangeUserPasswordStatusId = {changeUserPasswordStatusId}";
                }
            }

            return actionResult;
        }
    }
}
