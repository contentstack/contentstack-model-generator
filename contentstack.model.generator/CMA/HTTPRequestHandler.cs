using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using contentstack.CMA.Http;

namespace contentstack.CMA
{
    internal class HttpRequestHandler
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializer _serializer;

        public HttpRequestHandler()
        {
            _httpClient = new HttpClient();
            _serializer = new JsonSerializer();
        }

        public async Task<string> ProcessRequest(string Url, Dictionary<string, object> Headers, Dictionary<string, object> BodyJson, string FileName = null) 
        {
            // Build query parameters
            String queryParam = String.Join("&", BodyJson.Select(kvp => {
                var value = "";
                if (kvp.Value is string[])
                {
                    string[] vals = (string[])kvp.Value;
                    value = String.Join("&", vals.Select(item =>
                    {
                        return String.Format("{0}={1}", kvp.Key, item);
                    }));
                    return value;
                }
                else if (kvp.Value is Dictionary<string, object>)
                    value = JsonConvert.SerializeObject(kvp.Value);
                else
                    return String.Format("{0}={1}", kvp.Key, kvp.Value);

                return String.Format("{0}={1}", kvp.Key, value);
            }));

            var uri = new Uri(Url + "?" + queryParam);

            // Create HTTP request
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("x-user-agent", "contentstack-model-generator/0.4.2");

            if (Headers != null) 
            {
                foreach (var header in Headers) 
                {
                    try 
                    {
                        // Handle OAuth Bearer token
                        if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                        {
                            // Check if the value already contains "Bearer" prefix
                            string authValue = header.Value.ToString();
                            if (authValue.StartsWith("Bearer "))
                            {
                                string token = authValue.Substring(7); // Remove "Bearer " prefix
                                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                            }
                            else
                            {
                                // Value doesn't have Bearer prefix, add it
                                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authValue);
                            }
                        }
                        else
                        {
                            request.Headers.Add(header.Key, header.Value.ToString());
                        }
                    } 
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            try 
            {
                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode} - {errorContent}");
                }
            } 
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
