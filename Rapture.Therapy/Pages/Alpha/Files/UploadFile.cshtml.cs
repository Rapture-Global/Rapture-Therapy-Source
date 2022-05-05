using Eadent.Identity.Access;
using Microsoft.AspNetCore.Mvc;
using Rapture.Therapy.Configuration;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha.Files
{
    public class UploadFileModel : BasePageModel
    {
        private ILogger<UploadFileModel> Logger { get; }

        private IWebHostEnvironment WebHostEnvironment { get; }

        public string Message { get; set; }

        public bool UserIsAuthorised { get; set; }

        [BindProperty]
        public string MetaData { get; set; }

        [BindProperty]
        public IFormFile FileToUpload { get; set; }

        public UploadFileModel(ILogger<UploadFileModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity, IWebHostEnvironment webHostEnvironment) : base(logger, configuration, userSession, eadentUserIdentity)
        {
            Logger = logger;
            WebHostEnvironment = webHostEnvironment;

            if (UserSession.IsPrivileged && (UserSession.EMailAddress == "Eamonn@Duffy.global"))
            {
                UserIsAuthorised = true;
            }
            else
            {
                Message = "You are not Authorised to use this Page.";
            }
        }

        public void OnGet()
        {
        }

        public async Task OnPost(string action)
        {
            (bool success, decimal googleReCaptchaScore) = await GoogleReCaptcha();

            GoogleReCaptchaScore = googleReCaptchaScore;

            if (googleReCaptchaScore < RaptureTherapySettings.Instance.GoogleReCaptcha.MinimumScore)
            {
                Message = "You are unable to Upload A File because of a poor Google ReCaptcha Score.";
            }
            else
            {
                if (UserIsAuthorised)
                {
                    try
                    {
                        var file = Path.Combine(WebHostEnvironment.ContentRootPath, "Uploads", FileToUpload.FileName);

                        using (var fileStream = new FileStream(file, FileMode.Create))
                        {
                            FileToUpload.CopyTo(fileStream);
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.LogError(exception, "An Exception occurred.");
                    }
                }
            }
        }
    }
}
