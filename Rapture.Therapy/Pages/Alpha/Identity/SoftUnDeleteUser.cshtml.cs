using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Mvc;
using Rapture.Therapy.Configuration;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class SoftUnDeleteUserModel : BasePageModel
    {
        public string Message { get; set; }

        [BindProperty]
        public string UserGuid { get; set; }

        public SoftUnDeleteUserModel(ILogger<SoftUnDeleteUserModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            IActionResult actionResult = Page();

            (bool success, decimal googleReCaptchaScore) = await GoogleReCaptcha();

            GoogleReCaptchaScore = googleReCaptchaScore;

            if (googleReCaptchaScore < RaptureTherapySettings.Instance.GoogleReCaptcha.MinimumScore)
            {
                Message = "You are unable to Soft Un-Delete because of a poor Google ReCaptcha Score.";
            }
            else
            {
                if (action == "Soft Un-Delete")
                {
                    if (!Guid.TryParse(UserGuid, out var userGuid))
                    {
                        Message = "Invalid User Guid.";
                    }
                    else
                    {
                        DeleteUserStatus deleteUserStatusId = await EadentUserIdentity.SoftUnDeleteUserAsync(UserSession.SessionToken, userGuid, HttpHelper.GetRemoteIpAddress(Request), HttpContext.RequestAborted);

                        Message = $"DeleteUserStatusId = {deleteUserStatusId}";
                    }
                }
            }

            return actionResult;
        }
    }
}
