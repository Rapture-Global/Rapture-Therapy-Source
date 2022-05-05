using System;
using Eadent.Identity.Access;
using Eadent.Identity.DataAccess.EadentUserIdentity.Entities;
using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Rapture.Therapy.PagesAdditional;
using System.Threading.Tasks;
using Eadent.Common.WebApi.Helpers;
using Microsoft.Extensions.Logging;
using NLog;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class SignInUserModel : BasePageModel
    {
        private ILogger<SignInUserModel> Logger { get; }

        public string Message { get; set; }

        [BindProperty]
        public string EMailAddress { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public SignInUserModel(ILogger <SignInUserModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
            Logger = logger;

            UserSession.Clear();
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
                Message = "You are unable to Sign In because of a poor Google ReCaptcha Score.";
            }
            else
            {
                if (action == "Cancel")
                {
                    Message = "You chose to Cancel.";

                    EMailAddress = string.Empty;
                    Password = null;
                }
                else if (action == "Sign In")
                {
                    (SignInStatus signInStatusId, UserSessionEntity userSessionEntity, DateTime? previousUserSignInDateTimeUtc) = EadentUserIdentity.SignInUser(SignInType.WebSite, EMailAddress, Password, HttpHelper.GetRemoteIpAddress(Request), googleReCaptchaScore);

                    if (signInStatusId == SignInStatus.Success)
                    {
                        UserSession.SignIn(userSessionEntity);

                        actionResult = Redirect("CheckAndUpdateUserSession");
                    }
                    else
                    {
                        Message = $"SignInStatusId = {signInStatusId} : PreviousUserSignInDateTimeUtc = {previousUserSignInDateTimeUtc}";
                    }
                }
            }

            return actionResult;
        }
    }
}
