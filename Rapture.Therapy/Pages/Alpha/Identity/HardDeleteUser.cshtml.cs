using Eadent.Identity.Access;
using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Rapture.Therapy.PagesAdditional;
using System;
using System.Threading.Tasks;
using Eadent.Common.WebApi.Helpers;
using Microsoft.Extensions.Logging;
using NLog;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class HardDeleteUserModel : BasePageModel
    {
        private ILogger<HardDeleteUserModel> Logger { get; }

        public string Message { get; set; }

        [BindProperty]
        public string UserGuid { get; set; }

        public HardDeleteUserModel(ILogger<HardDeleteUserModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
            Logger = logger;
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
                Message = "You are unable to Hard Delete because of a poor Google ReCaptcha Score.";
            }
            else
            {
                if (action == "Hard Delete")
                {
                    if (!Guid.TryParse(UserGuid, out var userGuid))
                    {
                        Message = "Invalid User Guid.";
                    }
                    else
                    {
                        DeleteUserStatus deleteUserStatusId = EadentUserIdentity.HardDeleteUser(UserSession.SessionToken, userGuid, HttpHelper.GetRemoteAddress(Request));

                        Message = $"DeleteUserStatusId = {deleteUserStatusId}";
                    }
                }
            }

            return actionResult;
        }
    }
}
