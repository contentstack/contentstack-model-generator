using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Contentstack.Model.Generator.Model;
using contentstack.model.generator.Model;

namespace contentstack.CMA
{
    public class ContentstackClient
    {
        public JsonSerializerOptions SerializerOptions { get; set; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            AllowTrailingCommas = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        #region Internal Variables

        internal string StackApiKey
        {
            get;
            set;
        }

        internal string _SyncUrl
         {
            get
            {
                return String.Format("{0}/stacks/sync",
                                     config.BaseUrl);
            }
        }
        private readonly Dictionary<string, object> UrlQueries = new Dictionary<string, object>();

        private string _StackUrl
        {
            get
            {
                return String.Format("{0}/stacks/", config.BaseUrl);
            }
        }
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
        private readonly Dictionary<string, object> _StackHeaders = new Dictionary<string, object>();

        // deepcode ignore NoHardcodedCredentials: false positive - method signature/parameter names, no actual hardcoded credential
        public void SetHeader(string key, string value)
        {
            if (key != null && value != null)
            {
                if (this._LocalHeaders.ContainsKey(key))
                    this._LocalHeaders.Remove(key);
                this._LocalHeaders.Add(key, value);
            }

        }
        public ContentstackClient(ContentstackOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
                
            ContentstackOptions _options = options;
            this.StackApiKey = _options.ApiKey;
            this._LocalHeaders = new Dictionary<string, object>();
            this.SetHeader("api_key", _options.ApiKey);
            
            if (_options.IsOAuth && !string.IsNullOrEmpty(_options.Authorization))
            {
                this.SetHeader("Authorization", _options.Authorization);
            }
            else
            {
                this.SetHeader("authtoken", _options.Authtoken);
            }

            Config cnfig = new Config();
            if (_options.Host != null)
            {
                cnfig.Host = _options.Host;
            }
            if (_options.Version != null)
            {
                cnfig.Version = _options.Version;
            }

            if (_options.Branch != null) {
                this.SetHeader("branch", _options.Branch);
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
        internal static ContentstackException GetContentstackException(Exception ex)
        {
            Int32 errorCode = 0;
            string errorMessage = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            ContentstackException ContentstackException = new ContentstackException(ex);
            Dictionary<string, object> errors = null;

            try
            {
                System.Net.WebException webEx = (System.Net.WebException)ex;

                using (var exResp = webEx.Response)
                using (var stream = exResp.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    errorMessage = reader.ReadToEnd();
                    var node = JsonNode.Parse(errorMessage.Replace("\r\n", ""));
                    if (node is JsonObject data)
                    {
                        if (data["error_code"] is JsonValue ec)
                            errorCode = ec.GetValue<int>();

                        if (data["error_message"] is JsonValue em)
                            errorMessage = em.GetValue<string>();

                        if (data["errors"] is JsonObject errs)
                            errors = JsonSerializer.Deserialize<Dictionary<string, object>>(errs.ToJsonString());
                    }

                    var response = exResp as HttpWebResponse;
                    if (response != null)
                        statusCode = response.StatusCode;
                }
            }
            catch
            {
                errorMessage = ex.Message;
            }

            ContentstackException = new ContentstackException()
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                StatusCode = statusCode,
                Errors = errors
            };

            return ContentstackException;
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

        public async Task<StackResponse> GetStack()
        {
            Dictionary<String, Object> headers = GetHeader(_LocalHeaders);
            Dictionary<String, object> headerAll = new Dictionary<string, object>();
            Dictionary<string, object> mainJson = new Dictionary<string, object>();

            if (headers != null && headers.Count > 0)
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
           
            try
            {
                HttpRequestHandler RequestHandler = new HttpRequestHandler();
                var outputResult = await RequestHandler.ProcessRequest(_StackUrl, headers, mainJson);
                var root = JsonNode.Parse(outputResult.Replace("\r\n", ""))!.AsObject();
                return JsonSerializer.Deserialize<StackResponse>(root["stack"]!.ToJsonString(), this.SerializerOptions);
            }
            catch (Exception ex)
            {
                throw GetContentstackException(ex);
            }
        }

        #region Public Functions
        
        public async Task<ContentstackResponse> GetContentTypes(int skip = 0)
        {
            Dictionary<String, Object> headers = GetHeader(_LocalHeaders);
            Dictionary<String, object> headerAll = new Dictionary<string, object>();
            Dictionary<string, object> mainJson = new Dictionary<string, object>();

            if (headers != null && headers.Count > 0)
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
            mainJson.Add("include_global_field_schema", "true");
            mainJson.Add("skip", $"{skip}");
            try
            {
                HttpRequestHandler RequestHandler = new HttpRequestHandler();
                var outputResult = await RequestHandler.ProcessRequest(_Url, headers, mainJson);
                var root = JsonNode.Parse(outputResult.Replace("\r\n", ""))!.AsObject();
                ContentstackResponse contentstackResponse = new ContentstackResponse();
                contentstackResponse.listContentTypes = JsonSerializer.Deserialize<List<Contenttype>>(root["content_types"]!.ToJsonString(), this.SerializerOptions);
                if (root["count"] is JsonValue count) {
                    contentstackResponse.Count = count.GetValue<int>();
                }
                return contentstackResponse;
            }
            catch (Exception ex)
            {
                throw GetContentstackException(ex);
            }
        }

        
        public async Task<ContentstackResponse> GetGlobalFields(int skip = 0)
        {
            Dictionary<String, Object> headers = GetHeader(_LocalHeaders);
            Dictionary<String, object> headerAll = new Dictionary<string, object>();
            Dictionary<string, object> mainJson = new Dictionary<string, object>();
            
            if (headers != null && headers.Count > 0)
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
                HttpRequestHandler RequestHandler = new HttpRequestHandler();
                var outputResult = await RequestHandler.ProcessRequest(_GlobalFieldsUrl, headers, mainJson);
                var root = JsonNode.Parse(outputResult.Replace("\r\n", ""))!.AsObject();
                ContentstackResponse contentstackResponse = new ContentstackResponse();
                contentstackResponse.listContentTypes = JsonSerializer.Deserialize<List<Contenttype>>(root["global_fields"]!.ToJsonString(), this.SerializerOptions);
                if (root["count"] is JsonValue count) {
                    contentstackResponse.Count = count.GetValue<int>();
                }
                return contentstackResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw GetContentstackException(ex);
            }
        }
       
        internal Dictionary<string, object> GetHeader(Dictionary<string, object> localHeader)
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

                return localHeader;
            }

            return _StackHeaders;
        }
        #endregion

    }
}
