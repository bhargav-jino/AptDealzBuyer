using Newtonsoft.Json;

namespace AptDealzBuyer.Model.Request
{
    public class FileUpload
    {
        [JsonProperty("fileUploadCategory")]
        public int FileUploadCategory;

        [JsonProperty("fileName")]
        public string FileName;

        [JsonProperty("base64String")]
        public string Base64String;

        [JsonProperty("oldFileUri")]
        public string OldFileUri;
    }
}
