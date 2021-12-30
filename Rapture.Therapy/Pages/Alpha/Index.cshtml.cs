using Eadent.Identity.Access;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rapture.Therapy.PagesAdditional;
using Rapture.Therapy.Sessions;

namespace Rapture.Therapy.Pages.Alpha
{
    public class IndexModel : BasePageModel
    {
        private ILogger<IndexModel> Logger { get; }

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, IUserSession userSession, IEadentUserIdentity eadentUserIdentity) : base(logger, configuration, userSession, eadentUserIdentity)
        {
            Logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
