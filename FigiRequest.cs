using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Naklih.Com.FigiClassLib
{

    public class FigiRequest
    {
        public FigiRequest(string id, FigiIdentifierType idType)
        {
            this.Identifier = id;
            this.IdentifierType = idType;
            this.MICCode = null;    
            this.ExchangeCode = null;
            this.MarketSecDes = null;
            this.Currency = null;
        }

        [JsonProperty("idType", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public FigiIdentifierType IdentifierType { get; set; }
        [JsonProperty("idValue", Required = Required.Always)]
        public string Identifier { get; set; }
        [JsonProperty("exchCode",  NullValueHandling =NullValueHandling.Ignore)]
        public string ExchangeCode { get; set; }
        [JsonProperty("micCode", NullValueHandling = NullValueHandling.Ignore)]
        public string MICCode { get; set; }
        [JsonProperty("currency",  NullValueHandling = NullValueHandling.Ignore)]
        public string Currency { get; set; }
        [JsonProperty("marketSecDes", NullValueHandling = NullValueHandling.Ignore)]
        public string MarketSecDes { get; set; }

        public string toJSON()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            StringBuilder sb = new StringBuilder();
            
            using (System.IO.TextWriter tw = new System.IO.StringWriter(sb))
            {
                using (JsonWriter writer = new JsonTextWriter(tw))
                {
                    serializer.Serialize(writer, this);
                }
            }
            return sb.ToString();
        }
    }



}
