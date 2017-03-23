using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WPFServer
{
    public class Certificate
    {
        [JsonProperty("expirationDate")]
        public string ExpirationDate { get; set; }
        [JsonProperty("issuer")]
        public string Issuer { get; set; }
        [JsonProperty("effectiveDate")]
        public string EffectiveDate { get; set; }
        [JsonProperty("nameInfo")]
        public string NameInfo { get; set; }
        [JsonProperty("hasPrivateKey")]
        public bool HasPrivateKey { get; set; }
        [JsonProperty("subjectName")]
        public string SubjectName { get; set; }
    }
}
