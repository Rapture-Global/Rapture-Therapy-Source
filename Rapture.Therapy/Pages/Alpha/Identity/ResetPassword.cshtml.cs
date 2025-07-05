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
    public class ResetPasswordModel : BasePageModel
    {
        private const string SessionEMailAddress = "RaptureTherapy.EMailAddress";

        private const string SessionUserPasswordResetCode = "RaptureTherapy.UserPasswordResetCode";

        public string Message { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public ResetPasswordModel(ILogger<ResetPasswordModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
        }
        
        public void OnGet()
        {
        }

        public async Task OnPostAsync()
        {
            (bool success, decimal googleReCaptchaScore) = await GoogleReCaptcha();

            GoogleReCaptchaScore = googleReCaptchaScore;

            if (googleReCaptchaScore < RaptureTherapySettings.Instance.GoogleReCaptcha.MinimumScore)
            {
                Message = "You are unable to Commit a Password Reset because of a poor Google ReCaptcha Score.";
            }
            else if (NewPassword != ConfirmPassword)
            {
                Message = "Both Passwords Must Match.";
            }
            else
            {
                var eMailAddress = HttpContext.Session.GetString(SessionEMailAddress);

                var userPasswordResetCode = HttpContext.Session.GetString(SessionUserPasswordResetCode);

                UserPasswordResetStatus userPasswordResetStatusId = await EadentUserIdentity.CommitUserPasswordResetAsync(eMailAddress, userPasswordResetCode, NewPassword, HttpHelper.GetRemoteIpAddress(Request), googleReCaptchaScore);

                Message = $"UserPasswordResetStatusId = {userPasswordResetStatusId}";
            }
        }
    }
}
