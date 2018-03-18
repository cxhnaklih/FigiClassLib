using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.ComponentModel;

namespace Naklih.Com.FigiClassLib
{
    public class FigiAPI
    {
        public static IList<FigiResponse> makeFigiRequest(string json, string figiApiUrl = "https://api.openfigi.com/v1/mapping", string apiKey = null)
        {
            IList<FigiRequest>  requestList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FigiRequest>>(json, new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore });
            return makeFigiRequest(requestList, figiApiUrl, apiKey);
        }

        public static IList<FigiResponse> makeFigiRequest(IList<FigiRequest> requestList, string figiApiUrl = "https://api.openfigi.com/v1/mapping", string apiKey = null)
        { 
          
            int requests = 5;
            if (apiKey != null)
            {
                requests = 99;
            }
            List<FigiResponse> allResponses = new List<FigiResponse>();

            if(requestList.Count > requests)
            {
                List<IList<FigiRequest>> chunks = new List<IList<FigiRequest>>();

                for (int i=0;  i<(((requestList.Count)/ requests) +1); i++)
                {
                    int length = Math.Min( requests, requestList.Count- i*requests);
                    int start = i * requests;
                    IList<FigiRequest> subList = requestList.Skip(start).Take(length).ToList<FigiRequest>();
                    chunks.Add(subList);
                    

                }
                Parallel.For(0, chunks.Count-1, i=>
                {
                    IList<FigiResponse> responses = makeBatchFigiRequest(chunks[i], figiApiUrl, apiKey);
                    allResponses.AddRange(responses);
                });
            }
            else
            {
                IList<FigiResponse> responses = makeBatchFigiRequest(requestList, figiApiUrl, apiKey);
                allResponses.AddRange(responses);
            }

            return allResponses;
        }

        

        public static IList<FigiResponse> makeFigiRequestForSingleFigiID(IList<FigiRequest> requestList, string figiApiUrl = "https://api.openfigi.com/v1/mapping", string apiKey = null)
        {

            IList<FigiResponse> responses = makeFigiRequest(requestList, figiApiUrl, apiKey);
            List<FigiResponse> singleFigiIDResponses = new List<FigiResponse>();

            foreach (FigiResponse response in responses)
            {
                if(response.FigiResponseItems.Count <=1)
                {
                    if (response.FigiResponseItems.Count > 0)
                        response.FigiResponseItems.First().MappingConfidencePct = 1;
                    singleFigiIDResponses.Add(response);
                }
                else
                {
                    var composites = from c in response.FigiResponseItems
                                     where c.IsCompositeLine == true
                                     select c;
                    FigiResponse r = new FigiResponse();
                    r.Request = response.Request;
                    int compositeLines = composites.Count();
                    if (composites.Count() == 1)
                    {
                       
                        r.FigiResponseItems.Add(composites.First());
                        r.FigiResponseItems.First().MappingConfidencePct = 1;
                    }
                    else
                    {
                        if (compositeLines > 1)
                        {
                            var filteredComposites = composites;
                            if(r.Request.ExchangeTieBreakOnly != null)
                            {
                                filteredComposites = composites.Where<FigiResponseLine>(x => x.ExchangeCode == r.Request.ExchangeTieBreakOnly);
                                
                            }
                            

                            r.FigiResponseItems.Add(filteredComposites.OrderBy(c => CompositeFigiHelper.Instance.CompositePriority(c.ExchangeCode)).ThenByDescending(c => c.UniqueID).First());
                            System.Diagnostics.Debug.WriteLine(string.Format("This Stock Has More than one Composite {0}", r.Request.Identifier));
                            r.FigiResponseItems.First().MappingConfidencePct = 1.0/ compositeLines;
                        }
                        else
                        {
                            r.FigiResponseItems.Add(response.FigiResponseItems.First());
                            r.FigiResponseItems.First().MappingConfidencePct = 1.0 / response.FigiResponseItems.Count;
                        }
                    }
                    singleFigiIDResponses.Add(r);
                }

            }
            return singleFigiIDResponses;
        }


        protected static  IList<FigiResponse> makeBatchFigiRequest(IList<FigiRequest> requestList,string figiApiUrl , string apiKey = null)
        {
            string requestJson = prepareRequestJson(requestList);

            string responseJson = makeJsonRequest(requestJson, figiApiUrl, apiKey);

            IList<FigiResponse> response = UnpackJson(responseJson);

            int i = 0;

            foreach (FigiResponse r in response)
            {
                r.Request = requestList[i];
                i++;
            }

            return response;
        }

        protected static string prepareRequestJson(IList<FigiRequest> requestList)
        {
            StringBuilder requestJson = new StringBuilder();
            requestJson.Append("[");

            foreach (FigiRequest r in requestList)
            {
                requestJson.Append(r.toJSON());
                requestJson.Append(",");
            }

            requestJson.Remove(requestJson.Length-1, 1);
            requestJson.Append("]");

            return (requestJson.ToString());
        }

        protected static string makeJsonRequest(string json, string figiApiUrl , string apiKey = null, int requestsLeft = 10)
        {
            try
            {

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(figiApiUrl);
                httpWebRequest.ContentType = "text/json";
                if (apiKey != null)
                {
                    httpWebRequest.Headers.Add("X-OPENFIGI-APIKEY", apiKey);
                }
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }
                

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();

                    return responseText;
                }

            }
            catch (Exception ex)
            {
                if(requestsLeft >0)
                {
                    System.Threading.Thread.CurrentThread.Join(5000);
                        
                    requestsLeft -= 1;
                    return makeJsonRequest(json, figiApiUrl, apiKey, requestsLeft);
                }
                else
                {
                    throw (ex);
                }
            }
        }

        

        protected static List<FigiResponse> UnpackJson(string response)
        {
            
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<FigiResponse>>(response, new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All }); ;
        }

        public static DataTable ToDataTable(IList<FigiResponse> responseList)
        {
           
            PropertyDescriptorCollection props =
            TypeDescriptor.GetProperties(typeof(FigiResponseLine));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            table.Columns.Add("RequestId", typeof(string));
            table.Columns.Add("RequestType", typeof(string));
            table.Columns.Add("RequestorsIdentifier", typeof(string));

            object[] values = new object[props.Count+3];
            foreach (FigiResponse item in responseList)
            {

                if (item.FigiResponseItems != null)
                {
                    
                    foreach (FigiResponseLine line in item.FigiResponseItems)
                    {
                        int i;
                        for ( i = 0; i < values.Length-3; i++)
                        {
                            values[i] = props[i].GetValue(line);
                        }
                        if (item.Request != null)
                        {
                            values[values.Length-3] = item.Request.OriginalIdentifier;
                            values[values.Length -2] = Enum.GetName(typeof(FigiIdentifierType), item.Request.IdentifierType);
                            values[values.Length - 1] = item.Request.RequestorsIdentifier;
                        }

                        table.Rows.Add(values);
                    }
                    // if we didn't get any responses just return the request...
                    if (item.FigiResponseItems.Count == 0)
                    {
                        if (item.Request != null)
                        {
                            values[values.Length - 3] = item.Request.OriginalIdentifier;
                            values[values.Length - 2] = Enum.GetName(typeof(FigiIdentifierType), item.Request.IdentifierType);
                            values[values.Length - 1] = item.Request.RequestorsIdentifier;
                        }

                        table.Rows.Add(values);
                    }


                }
               
                
            }
            
            return table;
        }

        


    }

}
