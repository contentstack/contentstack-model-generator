using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections;
using Contentstack.Model.Generator.Model;
using contentstack.model.generator.Model;

namespace contentstack.CMA
{
    /// <summary>
    /// To fetch stack level information of your application from Contentstack server.
    /// </summary>
    public class ContentstackClient
    {
        public JsonSerializerSettings SerializerSettings { get; set; } = new JsonSerializerSettings();

        #region Internal Variables

        internal string StackApiKey
        {
            get;
            set;
        }
        private ContentstackOptions _options;

        internal string _SyncUrl
         {
            get
            {
                Config config = this.config;
                return String.Format("{0}/stacks/sync",
                                     config.BaseUrl);
            }
        }
        private Dictionary<string, object> UrlQueries = new Dictionary<string, object>();
        private Dictionary<string, object> _Headers = new Dictionary<string, object>();
        private string _Url
        {
         get { 
                return String.Format("{0}/content_types/", config.BaseUrl);
            }
        }
        private string _GlobalFieldsUrl
        {
            get
            {
                return String.Format("{0}/global_fields/", config.BaseUrl);
            }
        }
        private Dictionary<string, object> _StackHeaders = new Dictionary<string, object>();

        public void SetHeader(string key, string value)
        {
            if (key != null & value != null)
            {
                if (this._LocalHeaders.ContainsKey(key))
                    this._LocalHeaders.Remove(key);
                this._LocalHeaders.Add(key, value);
            }

        }
        public ContentstackClient(ContentstackOptions options)
        {
            _options = options;
            this.StackApiKey = _options.ApiKey;
            this._LocalHeaders = new Dictionary<string, object>();
            this.SetHeader("api_key", _options.ApiKey);
            this.SetHeader("access_token", _options.AccessToken);
            Config cnfig = new Config();
            if (_options.Host != null)
            {
                cnfig.Host = _options.Host;
            }
            if (_options.Version != null)
            {
                cnfig.Version = _options.Version;
            }
            this.SetConfig(cnfig);

        }

        internal string Version { get; set; }

        internal ContentstackConstants _Constants { get; set; }
        internal Dictionary<string, object> _LocalHeaders = new Dictionary<string, object>();
        internal Config config;
        #endregion

        #region Private Constructor
        private ContentstackClient() { }
        #endregion

        #region Internal Constructor
        internal static ContentstackError GetContentstackError(Exception ex)
        {
            Int32 errorCode = 0;
            string errorMessage = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            ContentstackError contentstackError = new ContentstackError(ex);
            Dictionary<string, object> errors = null;
            //ContentstackError.OtherErrors errors = null;

            try
            {
                System.Net.WebException webEx = (System.Net.WebException)ex;

                using (var exResp = webEx.Response)
                using (var stream = exResp.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    errorMessage = reader.ReadToEnd();
                    JObject data = JObject.Parse(errorMessage.Replace("\r\n", ""));
                    //errorCode = ContentstackConvert.ToInt32(data.Property("error_code").Value);
                    //errorMessage = ContentstackConvert.ToString(data.Property("error_message").Value);

                    JToken token = data["error_code"];
                    if (token != null)
                        errorCode = token.Value<int>();

                    token = data["error_message"];
                    if (token != null)
                        errorMessage = token.Value<string>();

                    token = data["errors"];
                    if (token != null)
                        errors = token.ToObject<Dictionary<string, object>>();

                    var response = exResp as HttpWebResponse;
                    if (response != null)
                        statusCode = response.StatusCode;
                }
            }
            catch
            {
                errorMessage = ex.Message;
            }

            contentstackError = new ContentstackError()
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                StatusCode = statusCode,
                Errors = errors
            };

            return contentstackError;
        }
        internal ContentstackClient(String stackApiKey)
        {
            this.StackApiKey = stackApiKey;
            this._LocalHeaders = new Dictionary<string, object>();

        }
        #endregion

        #region Internal Functions
        internal void SetConfig(Config cnfig)
        {
            this.config = cnfig;

        }


        #endregion

        #region Public Functions
        /// <summary>
        /// This method returns comprehensive information of all the content types of a particular stack in your account.
        /// </summary>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     var result = await stack.GetContentTypes();
        /// </code>
        /// </example>
        /// <returns>The List of content types schema.</returns>
        public async Task<ContentstackResponse> GetContentTypes(int skip = 0)
        {
            Dictionary<String, Object> headers = GetHeader(_LocalHeaders);
            Dictionary<String, object> headerAll = new Dictionary<string, object>();
            Dictionary<string, object> mainJson = new Dictionary<string, object>();

            if (headers != null && headers.Count() > 0)
            {
                foreach (var header in headers)
                {
                    headerAll.Add(header.Key, (String)header.Value);
                }
            }

            foreach (var kvp in UrlQueries)
            {
                mainJson.Add(kvp.Key, kvp.Value);
            }
            mainJson.Add("include_count", "true");
            mainJson.Add("skip", $"{skip}");
            try
            {
                HTTPRequestHandler RequestHandler = new HTTPRequestHandler();
                var outputResult = await RequestHandler.ProcessRequest(_Url, headers, mainJson);
                JObject data = JsonConvert.DeserializeObject<JObject>(outputResult.Replace("\r\n", ""), this.SerializerSettings);
                IList contentTypes = (IList)data["content_types"];
                ContentstackResponse contentstackResponse = new ContentstackResponse();
                var ContentTypeJson = JsonConvert.SerializeObject(contentTypes);
                contentstackResponse.listContentTypes = JsonConvert.DeserializeObject<List<Contenttype>>(ContentTypeJson);
                if (data["count"] != null) {
                    contentstackResponse.Count = (int)data["count"];
                }
                return contentstackResponse;
            }
            catch (Exception ex)
            {
                throw GetContentstackError(ex);
            }
        }

        /// <summary>
        /// This method returns comprehensive information of all the global fields available in a particular stack in your account.
        /// </summary>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     var result = await stack.GetGlobalFields();
        /// </code>
        /// </example>
        /// <returns>The List of Global Fields schema.</returns>
        public async Task<ContentstackResponse> GetGlobalFields(int skip = 0)
        {
            Dictionary<String, Object> headers = GetHeader(_LocalHeaders);
            Dictionary<String, object> headerAll = new Dictionary<string, object>();
            Dictionary<string, object> mainJson = new Dictionary<string, object>();
            
            if (headers != null && headers.Count() > 0)
            {
                foreach (var header in headers)
                {
                    headerAll.Add(header.Key, (String)header.Value);
                }
            }

            foreach (var kvp in UrlQueries)
            {
                mainJson.Add(kvp.Key, kvp.Value);
            }
            mainJson.Add("include_count", "true");
            mainJson.Add("skip", $"{skip}");
            try
            {
                HTTPRequestHandler RequestHandler = new HTTPRequestHandler();
                var outputResult = await RequestHandler.ProcessRequest(_GlobalFieldsUrl, headers, mainJson);
                JObject data = JsonConvert.DeserializeObject<JObject>(outputResult.Replace("\r\n", ""), this.SerializerSettings);
                IList globalFields = (IList)data["global_fields"];
                ContentstackResponse contentstackResponse = new ContentstackResponse();
                var ContentTypeJson = JsonConvert.SerializeObject(globalFields);
                contentstackResponse.listContentTypes = JsonConvert.DeserializeObject<List<Contenttype>>(ContentTypeJson);
                if (data["count"] != null) {
                    contentstackResponse.Count = (int)data["count"];
                }
                return contentstackResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw GetContentstackError(ex);
            }
        }
       
        private Dictionary<string, object> GetHeader(Dictionary<string, object> localHeader)
        {
            Dictionary<string, object> mainHeader = _StackHeaders;
            Dictionary<string, object> classHeaders = new Dictionary<string, object>();

            if (localHeader != null && localHeader.Count > 0)
            {
                if (mainHeader != null && mainHeader.Count > 0)
                {
                    foreach (var entry in localHeader)
                    {
                        String key = entry.Key;
                        classHeaders.Add(key, entry.Value);
                    }

                    foreach (var entry in mainHeader)
                    {
                        String key = entry.Key;
                        if (!classHeaders.ContainsKey(key))
                        {
                            classHeaders.Add(key, entry.Value);
                        }
                    }

                    return classHeaders;

                }
                else
                {
                    return localHeader;
                }

            }
            else
            {
                return _StackHeaders;
            }
        }

        private Dictionary<string, object> GetHeader()
        {

            Dictionary<string, object> mainHeader = _LocalHeaders;

            return mainHeader;
        }

        #endregion

    }
}
