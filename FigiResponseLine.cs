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
    public class FigiResponseLine
    {
        [JsonProperty("figi")]
        public string Figi { get; set; }
        [JsonProperty("marketSector")]
        public string MarketSector { get; set; }
        [JsonProperty("securityType")]
        public string SecurityType { get; set; }
        [JsonProperty("ticker")]
        public string Ticker { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("exchCode")]
        public string ExchangeCode { get; set; }
        [JsonProperty("securityDescription")]
        public string SecurityDescription { get; set; }
        [JsonProperty("securityType2")]
        public string SecurityType2 { get; set; }
        [JsonProperty("uniqueID")]
        public string UniqueID { get; set; }
        [JsonProperty("compositeFIGI")]
        public string CompositeFigi { get; set; }
        [JsonProperty("shareClassFIGI")]
        public string ShareClassFigi { get; set; }
        [JsonProperty("uniqueIDFutOpt")]
        public string UniqueIDFutureOption { get; set; }

        public bool IsCompositeLine
        {
            get
            {
                if (CompositeFigi == Figi)
                {
                    return true;
                }
                else
                {
                    bool isComp = Naklih.Com.FigiClassLib.CompositeFigiHelper.Instance.IsComposite(this.ExchangeCode);
                    return isComp;
                }
            }
        }

        public string BloombergId
        {
            get
            {
                return string.Format("{0} {1} {2}", this.Ticker, this.ExchangeCode, this.MarketSector);
            }
        }


        public double MappingConfidencePct
        {
            get; internal set;
        }
    }
}
