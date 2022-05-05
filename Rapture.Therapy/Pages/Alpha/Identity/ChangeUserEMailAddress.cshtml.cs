using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Rapture.Therapy.PagesAdditional;
using System.Threading.Tasks;
using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Eadent.Identity.DataAccess.EadentUserIdentity.Entities;
using Microsoft.Extensions.Logging;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class ChangeUserEMailAddressModel : BasePageModel
    {
        public string Message { get; set; }

        [BindProperty]
        public string Password{ get; set; }

        [BindProperty]
        public string NewEMailAddress { get; set; }

        public ChangeUserEMailAddressModel(ILogger<ChangeUserEMailAddressModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
        }

        public async Task<IActionResult> OnPost(string action)
        {
            IActionResult actionResult = Page();

            (bool success, decimal googleReCaptchaScore) = await GoogleReCaptcha();

            GoogleReCaptchaScore = googleReCaptchaScore;

            if (googleReCaptchaScore < RaptureTherapySettings.GoogleReCaptcha.MinimumScore)
            {
                Message = "You are unable to Change the E-Mail Address because of a poor Google ReCaptcha Score.";
            }
            else if (action == "Change E-Mail Address")
            {
                (ChangeUserEMailStatus changeUserEMailStatusId, UserSessionEntity userSessionEntity) = EadentUserIdentity.ChangeUserEMailAddress(UserSession.SessionToken, Password, UserSession.EMailAddress, NewEMailAddress, HttpHelper.GetRemoteIpAddress(Request), googleReCaptchaScore);

                Message = $"ChangeUserEMailStatusId = {changeUserEMailStatusId}";
            }

            return actionResult;
        }
    }
}
