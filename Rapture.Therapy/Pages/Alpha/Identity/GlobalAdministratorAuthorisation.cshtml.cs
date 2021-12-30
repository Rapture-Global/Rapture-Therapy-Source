using Eadent.Identity.Access;
using Eadent.Identity.Definitions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha.Identity
{
    public class GlobalAdministratorAuthorisationModel : BasePageModel
    {
        public string Message { get; set; }

        public GlobalAdministratorAuthorisationModel(ILogger<GlobalAdministratorAuthorisationModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
        }

        public void OnGet()
        {
            (bool hasRole, IUserSession.IRole role) = UserSession.HasRole(Role.GlobalAdministrator);

            if (hasRole)
            {
                Message = "User Has Global Administrator Authorisation and hence has Full Access to this Page.";
            }
            else
            {
                Message = "User *DOES NOT* have Global Administrator Authorisation and hence *DOES NOT* have Full Access to this Page.";
            }
        }
    }
}
