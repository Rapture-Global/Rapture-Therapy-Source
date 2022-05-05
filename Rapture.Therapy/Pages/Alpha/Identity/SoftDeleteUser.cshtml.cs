using Eadent.Identity.Access;
using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rapture.Therapy.Configuration;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;
using System;
using System.Threading.Tasks;
using Eadent.Common.WebApi.Helpers;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class SoftDeleteUserModel : BasePageModel
    {
        public string Message { get; set; }

        [BindProperty]
        public string UserGuid { get; set; }

        public SoftDeleteUserModel(ILogger<SoftDeleteUserModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
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

            if (googleReCaptchaScore<RaptureTherapySettings.GoogleReCaptcha.MinimumScore)
            {
                Message = "You are unable to Soft Delete because of a poor Google ReCaptcha Score.";
            }
            else
            {
                if (action == "Soft Delete")
                {
                    if (!Guid.TryParse(UserGuid, out var userGuid))
                    {
                        Message = "Invalid User Guid.";
                    }
                    else
                    {
                        DeleteUserStatus deleteUserStatusId = EadentUserIdentity.SoftDeleteUser(UserSession.SessionToken, userGuid, HttpHelper.GetRemoteIpAddress(Request));

                        Message = $"DeleteUserStatusId = {deleteUserStatusId}";
                    }
                }
            }

            return actionResult;
        }
    }
}
