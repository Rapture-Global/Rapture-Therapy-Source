using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Eadent.Identity.DataAccess.EadentUserIdentity.Entities;
using Eadent.Identity.Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rapture.Therapy.Configuration;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;
using System.Threading.Tasks;

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

        public async Task<IActionResult> OnPost(string action)
        {
            IActionResult actionResult = Page();

            DomainUrl = $"{Request.Scheme}://{Request.Host}/";

            (bool success, decimal googleReCaptchaScore) = await GoogleReCaptcha();

            GoogleReCaptchaScore = googleReCaptchaScore;

            Logger.LogInformation($"Google ReCaptcha Score: {GoogleReCaptchaScore}");

            if (googleReCaptchaScore < RaptureTherapySettings.GoogleReCaptcha.MinimumScore)
            {
                Logger.LogInformation($"Unable to Register because of a poor Google ReCaptcha Score. Google ReCaptcha Score: {GoogleReCaptchaScore}. Minimum Score: {RaptureTherapySettings.GoogleReCaptcha.MinimumScore}");

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
                        (RegisterUserStatus registerStatusId, UserEntity userEntity) = EadentUserIdentity.RegisterUser(RoleId, EMailAddress, DisplayName, Password, HttpHelper.GetRemoteAddress(Request), googleReCaptchaScore);

                        if (registerStatusId == RegisterUserStatus.Success)
                        {
                            actionResult = Redirect("SignInUser");
                        }
                        else
                        {
                            Message = $"RegisterStatusId = {registerStatusId}";
                        }
                    }
                }
            }

            return actionResult;
        }
    }
}
