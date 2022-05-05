using System.Runtime.Serialization;

namespace Rapture.Therapy.DataTransferObjects.Google
{
    [DataContract]
    public class ReCaptchaVerifyResponseDto
    {
        [DataMember]
        public bool success { get; set; }

        [DataMember]
        public decimal score { get; set; }

        [DataMember]
        public string action { get; set; }

        [DataMember]
        public string challenge_ts { get; set; }

        [DataMember]
        public string hostname { get; set; }

        [DataMember(Name = "error-codes", IsRequired = false)]
        public List<string> errorCodes { get; set; }
    }
}
