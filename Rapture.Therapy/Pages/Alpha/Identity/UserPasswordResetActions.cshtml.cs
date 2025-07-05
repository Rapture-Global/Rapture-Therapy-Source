using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Eadent.Identity.DataAccess.EadentUserIdentity.Entities;
using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Mvc;
using Rapture.Therapy.Configuration;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;
using System.Net;
using System.Text;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class UserPasswordResetActionsModel : BasePageModel
    {
        private const string SessionEMailAddress = "RaptureTherapy.EMailAddress";

        private const string SessionsUserPasswordResetCode = "RaptureTherapy.UserPasswordResetCode";

        public string Message { get; set; }

        public string ResetPasswordUrl { get; set; }

        [BindProperty]
        public string EMailAddress { get; set; }

        public UserPasswordResetActionsModel(ILogger<UserPasswordResetActionsModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            IActionResult actionResult = Page();

            if (!string.IsNullOrWhiteSpace(EMailAddress))
            {
                HttpContext.Session.SetString(SessionEMailAddress, EMailAddress);
            }

            if (action == "Begin Password Reset")
            {
                (bool success, decimal googleReCaptchaScore) = await GoogleReCaptcha();

                GoogleReCaptchaScore = googleReCaptchaScore;

                if (googleReCaptchaScore < RaptureTherapySettings.Instance.GoogleReCaptcha.MinimumScore)
                {
                    Message = "You are unable to Begin a Password Reset because of a poor Google ReCaptcha Score.";
                }
                else
                {
                    (UserPasswordResetStatus userPasswordResetStatusId, string displayName, string userPasswordResetCode) = await EadentUserIdentity.BeginUserPasswordResetAsync(EMailAddress, HttpHelper.GetRemoteIpAddress(Request), googleReCaptchaScore);

                    if (!string.IsNullOrWhiteSpace(userPasswordResetCode))
                    {
                        HttpContext.Session.SetString(SessionsUserPasswordResetCode, userPasswordResetCode);
                    }

                    Message = $"UserPasswordResetStatusId = {userPasswordResetStatusId}";

                    ResetPasswordUrl = $"{Request.Scheme}://{Request.Host}/Alpha/Identity/ResetPassword";
                }
            }
            else if (action == "Roll Back Password Reset")
            {
                (bool success, decimal googleReCaptchaScore) = await GoogleReCaptcha();

                GoogleReCaptchaScore = googleReCaptchaScore;

                if (googleReCaptchaScore < RaptureTherapySettings.Instance.GoogleReCaptcha.MinimumScore)
                {
                    Message = "You are unable to Roll Back a Password Reset because of a poor Google ReCaptcha Score.";
                }
                else
                {
                    var userPasswordResetCode = HttpContext.Session.GetString(SessionsUserPasswordResetCode);

                    UserPasswordResetStatus passwordResetStatusId = await EadentUserIdentity.RollBackUserPasswordResetAsync(EMailAddress, userPasswordResetCode, HttpHelper.GetRemoteIpAddress(Request), GoogleReCaptchaScore);

                    Message = $"PasswordResetRequestStatusId = {passwordResetStatusId}";
                }
            }

            return actionResult;
        }
    }
}
