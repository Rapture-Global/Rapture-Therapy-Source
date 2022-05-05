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
        private const string ResetTokenName = "RaptureTherapy.Identity.ResetToken";

        public string Message { get; set; }

        public string ResetToken { get; set; }

        public string ResetPasswordUrl { get; set; }

        [BindProperty]
        public string EMailAddress { get; set; }

        public UserPasswordResetActionsModel(ILogger<UserPasswordResetActionsModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string action)
        {
            IActionResult actionResult = Page();

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
                    (UserPasswordResetRequestStatus passwordResetRequestStatusId, string resetToken, UserEntity userEntity) = EadentUserIdentity.BeginUserPasswordReset(EMailAddress, HttpHelper.GetRemoteIpAddress(Request), googleReCaptchaScore);

                    Message = $"PasswordResetRequestStatusId = {passwordResetRequestStatusId}";

                    HttpContext.Session.SetString(ResetTokenName, resetToken);

                    ResetToken = resetToken;

                    var urlResetToken = WebUtility.UrlEncode(Convert.ToBase64String(Encoding.Unicode.GetBytes(resetToken)));

                    ResetPasswordUrl = $"{Request.Scheme}://{Request.Host}/Alpha/Identity/ResetPassword/{urlResetToken}";
                }
            }
            else if (action == "Check Password Reset")
            {
                string resetToken = HttpContext.Session.GetString(ResetTokenName);

                ResetToken = resetToken;

                (UserPasswordResetRequestStatus passwordResetRequestStatusId, UserPasswordResetEntity passwordResetEntity) = EadentUserIdentity.CheckAndUpdateUserPasswordReset(resetToken, HttpHelper.GetRemoteIpAddress(Request));

                Message = $"PasswordResetRequestStatusId = {passwordResetRequestStatusId}";
            }
            else if (action == "Abort Password Reset")
            {
                string resetToken = HttpContext.Session.GetString(ResetTokenName);

                ResetToken = resetToken;

                (UserPasswordResetRequestStatus passwordResetRequestStatusId, UserPasswordResetEntity passwordResetEntity) = EadentUserIdentity.AbortUserPasswordReset(resetToken, HttpHelper.GetRemoteIpAddress(Request));

                Message = $"PasswordResetRequestStatusId = {passwordResetRequestStatusId}";
            }

            return actionResult;
        }
    }
}
