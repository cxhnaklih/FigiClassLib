using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Naklih.Com.FigiClassLib
{
    public class FigiResponse
    {
        public FigiResponse()
        {
            this.FigiResponseItems = new List<FigiResponseLine>();
        }

        [JsonProperty("data")]
        public List<FigiResponseLine> FigiResponseItems { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        public FigiRequest Request { get; set; }
    }
}
