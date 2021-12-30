using Eadent.Common.WebApi.ApiClient;
using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rapture.Therapy.Configuration;
using Rapture.Therapy.DataTransferObjects.Google;
using Rapture.Therapy.Sessions;
using System.Threading.Tasks;

namespace Rapture.Therapy.PagesAdditional
{
    public class BasePageModel : PageModel
    {
        private ILogger Logger { get; }

        protected RaptureTherapySettings RaptureTherapySettings { get; }

        protected IEadentUserIdentity EadentUserIdentity { get; }

        public IUserSession UserSession { get; }

        public string GoogleReCaptchaSiteKey => RaptureTherapySettings.GoogleReCaptcha.SiteKey;

        public decimal GoogleReCaptchaScore { get; set; }

        [BindProperty]
        public string GoogleReCaptchaValue { get; set; }

        protected BasePageModel(ILogger logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity)
        {
            Logger = logger;

            RaptureTherapySettings = configuration.GetSection(RaptureTherapySettings.SectionName).Get<RaptureTherapySettings>();

            UserSession = userSession;

            EadentUserIdentity = eadentUserIdentity;
        }

        protected async Task<(bool success, decimal googleReCaptchaScore)> GoogleReCaptcha()
        {
            var verifyRequestDto = new ReCaptchaVerifyRequestDto()
            {
                secret = RaptureTherapySettings.GoogleReCaptcha.Secret,
                response = GoogleReCaptchaValue,
                remoteip = HttpHelper.GetLocalAddress(Request)
            };

            bool success = false;

            decimal googleReCaptchaScore = -1M;

            IApiClientResponse<ReCaptchaVerifyResponseDto> response = null;

            using (var apiClient = new ApiClientUrlEncoded(Logger, "https://www.google.com/"))
            {
                response = await apiClient.PostAsync<ReCaptchaVerifyRequestDto, ReCaptchaVerifyResponseDto>("/recaptcha/api/siteverify", verifyRequestDto, null);
            }

            if (response.ResponseDto != null)
            {
                googleReCaptchaScore = response.ResponseDto.score;

                success = true;
            }

            return (success, googleReCaptchaScore);
        }
    }
}
