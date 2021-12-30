namespace Rapture.Therapy.Configuration
{
    public class RaptureTherapySettings
    {
        public const string SectionName = "RaptureTherapy";

        public class DatabaseSettings
        {
            public string DatabaseServer { get; set; }

            public string DatabaseName { get; set; }

            public string DatabaseSchema { get; set; }

            public string ApplicationName { get; set; }

            public string UserName { get; set; }

            public string Password { get; set; }
        }

        public class GoogleReCaptchaSettings
        {
            public string SiteKey { get; set; }

            public string Secret { get; set; }

            public decimal MinimumScore { get; set; }
        }

        public DatabaseSettings Database { get; set; }

        public GoogleReCaptchaSettings GoogleReCaptcha { get; set; }
    }
}
