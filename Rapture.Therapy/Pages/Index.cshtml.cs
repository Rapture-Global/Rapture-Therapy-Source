using Eadent.Common.WebApi.ApiClient;
using Eadent.Identity.Access;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rapture.Therapy.DataAccess.RaptureTherapy.Repositories;

namespace Rapture.Therapy.Pages
{
    public class IndexModel : PageModel
    {
        // Attributes/Properties.
        private ILogger<IndexModel> Logger { get; }

        private IEadentUserIdentity EadentUserIdentity { get; }

        private IRaptureTherapyDatabaseVersionsRepository RaptureTherapyDatabaseVersionsRepository { get; }

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, IEadentUserIdentity eadentUserIdentity, IRaptureTherapyDatabaseVersionsRepository raptureTherapyDatabaseVersionsRepository)
        {
            Logger = logger;

            EadentUserIdentity = eadentUserIdentity;

            RaptureTherapyDatabaseVersionsRepository = raptureTherapyDatabaseVersionsRepository;
        }

        private class PostDto
        {
            public string title { get; set; }

            public string body { get; set; }

            public long userId { get; set; }
        }

        private async Task ApiClientTests()
        {
            using (var apiClient = new ApiClientJson(Logger, "https://jsonplaceholder.typicode.com/"))
            {
                var requestDto = new PostDto()
                {
                    title = "Eamonn",
                    body = "Jennifer",
                    userId = 69
                };

                var result = await apiClient.PostAsync<PostDto, dynamic>("/posts", requestDto, null);
            }

            using (var apiClient = new ApiClientJson(Logger, "https://jsonplaceholder.typicode.com/"))
            {
                var result = await apiClient.GetAsync<dynamic>("/posts/1", null);
            }

            using (var apiClient = new ApiClientJson(Logger, "https://jsonplaceholder.typicode.com/"))
            {
                var requestDto = new PostDto()
                {
                    title = "Eamonn",
                    body = "Jennifer",
                    userId = 69
                };

                var result = await apiClient.PutAsync<PostDto, dynamic>("/posts", requestDto, null);
            }

            using (var apiClient = new ApiClientJson(Logger, "https://jsonplaceholder.typicode.com/"))
            {
                var result = await apiClient.DeleteAsync<dynamic>("/posts/1", null);
            }
        }

        public async Task OnGet()
        {
#if false
            var list = RaptureTherapyDatabaseVersionsRepository.GetAll().ToList();

            //var urlEncodedResetToken = WebUtility.UrlEncode(resetToken);

            //var urlDecodedResetToken = WebUtility.UrlDecode(urlEncodedResetToken);

            //if (urlDecodedResetToken != resetToken)
            //{
            //    int breakPoint = 1;
            //}

            var role = Role.User;

            var eMailAddress = "Eamonn.Duffy@Eadent.com";

            var displayName = "Éamonn Duffy";

            var plainTextPassword = "Éamonn Ó Dubhthaigh!";

            var remoteAddressString = HttpHelper.GetRemoteAddress(Request);

            var registeredUserEntity = EadentUserIdentity.RegisterUser(role, eMailAddress, displayName, plainTextPassword, remoteAddressString);

            SignInStatus signInStatusId = SignInStatus.Error;

            UserSessionEntity userSessionEntity = null;

            string userSessionToken = null;

            SignOutStatus signOutStatusId = SignOutStatus.Error;

            (signInStatusId, userSessionEntity) = EadentUserIdentity.SignInUser(eMailAddress, plainTextPassword, remoteAddressString);

            signOutStatusId = EadentUserIdentity.SignOutUser(userSessionEntity.UserSessionToken, remoteAddressString);

            (signInStatusId, userSessionEntity) = EadentUserIdentity.SignInUser(eMailAddress, "Bad Password", remoteAddressString);

            signOutStatusId = EadentUserIdentity.SignOutUser(userSessionEntity.UserSessionToken, remoteAddressString);

            (signInStatusId, userSessionEntity) = EadentUserIdentity.SignInUser("Does.Not.Exist@Eadent.com", "Bad Password", remoteAddressString);

            signOutStatusId = EadentUserIdentity.SignOutUser(userSessionEntity.UserSessionToken, remoteAddressString);

            var passwordResetRequestStatusId = UserPasswordResetRequestStatus.Error;

            string resetToken = null;

            UserEntity userEntity = null;

            (passwordResetRequestStatusId, resetToken, userEntity) = EadentUserIdentity.BeginUserPasswordReset(eMailAddress, remoteAddressString);
#endif

            //InterimEadentIdentityTests();

            //await ApiClientTests();
        }
    }
}