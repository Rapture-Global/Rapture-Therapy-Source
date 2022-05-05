using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Eadent.Identity.DataAccess.EadentUserIdentity.Entities;
using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class ResetPasswordModel : BasePageModel
    {
        private const string ResetTokenName = "RaptureTherapy.Identity.ResetToken";

        public string Message { get; set; }

        public string ResetToken { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public ResetPasswordModel(ILogger<ResetPasswordModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
        }
        
        public void OnGet(string urlResetToken)
        {
            var resetToken = Encoding.Unicode.GetString(Convert.FromBase64String(WebUtility.UrlDecode(urlResetToken)));

            ResetToken = resetToken;

            HttpContext.Session.SetString(ResetTokenName, resetToken);

            (UserPasswordResetRequestStatus passwordResetRequestStatusId, UserPasswordResetEntity passwordResetEntity) = EadentUserIdentity.CheckAndUpdateUserPasswordReset(resetToken, HttpHelper.GetRemoteIpAddress(Request));

            Message = $"PasswordResetRequestStatusId = {passwordResetRequestStatusId}";
        }

        public async Task OnPost()
        {
            var resetToken = HttpContext.Session.GetString(ResetTokenName);

            ResetToken = resetToken;

            (bool success, decimal googleReCaptchaScore) = await GoogleReCaptcha();

            GoogleReCaptchaScore = googleReCaptchaScore;

            if (googleReCaptchaScore < RaptureTherapySettings.GoogleReCaptcha.MinimumScore)
            {
                Message = "You are unable to Commit a Password Reset because of a poor Google ReCaptcha Score.";
            }
            else if (NewPassword != ConfirmPassword)
            {
                Message = "Both Passwords Must Match.";
            }
            else
            {
                (UserPasswordResetRequestStatus passwordResetRequestStatusId, UserPasswordResetEntity passwordResetEntity) = EadentUserIdentity.CommitUserPasswordReset(resetToken, NewPassword, HttpHelper.GetRemoteIpAddress(Request), googleReCaptchaScore);

                Message = $"PasswordResetRequestStatusId = {passwordResetRequestStatusId}";
            }
        }
    }
}
