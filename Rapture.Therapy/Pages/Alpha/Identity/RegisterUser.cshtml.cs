using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Eadent.Identity.DataAccess.EadentUserIdentity.Entities;
using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Mvc;
using Rapture.Therapy.Configuration;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class RegisterUserModel : BasePageModel
    {
        private ILogger<RegisterUserModel> Logger { get; }
        
        public string DomainUrl { get; set; }

        public string Message { get; set; }

        [BindProperty]
        public Role RoleId { get; set; }

        [BindProperty]
        public string EMailAddress { get; set; }

        [BindProperty]
        public string DisplayName { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public RegisterUserModel(ILogger<RegisterUserModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
            Logger = logger;

            UserSession.Clear();
        }

        public void OnGet()
        {
            DomainUrl = $"{Request.Scheme}://{Request.Host}/";
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            IActionResult actionResult = Page();

            DomainUrl = $"{Request.Scheme}://{Request.Host}/";

            (bool success, decimal googleReCaptchaScore) = await GoogleReCaptcha();

            GoogleReCaptchaScore = googleReCaptchaScore;

            Logger.LogInformation($"Google ReCaptcha Score: {GoogleReCaptchaScore}");

            if (googleReCaptchaScore < RaptureTherapySettings.Instance.GoogleReCaptcha.MinimumScore)
            {
                Logger.LogInformation($"Unable to Register because of a poor Google ReCaptcha Score. Google ReCaptcha Score: {GoogleReCaptchaScore}. Minimum Score: {RaptureTherapySettings.Instance.GoogleReCaptcha.MinimumScore}");

                Message = $"You are unable to Register because of a poor Google ReCaptcha Score (Score: {GoogleReCaptchaScore}).";
            }
            else
            {
                if (action == "Cancel")
                {
                    Message = "You chose to Cancel.";

                    EMailAddress = string.Empty;
                    DisplayName = null;
                    Password = null;
                    ConfirmPassword = null;
                }
                else if (action == "Register")
                {
                    if (Password != ConfirmPassword)
                    {
                        Message = "The Passwords Must Match.";
                    }
                    else
                    {
                        int createdByApplicationId = 0;
                        string userGuidString = null;
                        string mobilePhoneNumber = null;

                        (RegisterUserStatus registerUserStatusId, UserEntity userEntity) = await EadentUserIdentity.RegisterUserAsync(createdByApplicationId, userGuidString, RoleId,
                            DisplayName, EMailAddress, mobilePhoneNumber, Password, HttpHelper.GetRemoteIpAddress(Request), googleReCaptchaScore, HttpContext.RequestAborted);

                        if (registerUserStatusId == RegisterUserStatus.Success)
                        {
                            actionResult = Redirect("SignInUser");
                        }
                        else
                        {
                            Message = $"RegisterUserStatusId = {registerUserStatusId}";
                        }
                    }
                }
            }

            return actionResult;
        }
    }
}
