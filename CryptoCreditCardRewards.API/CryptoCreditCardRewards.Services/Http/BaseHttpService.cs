using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Services.Http
{
    public class BaseHttpService
    {
        /// <summary>
        /// The web client used to make Http calls
        /// </summary>
        protected readonly HttpClient _httpClient;

        /// <summary>
        /// Base Http service
        /// </summary>
        /// <param name="client">Web client</param>
        public BaseHttpService(HttpClient client)
        {
            _httpClient = client;
        }

        protected async Task<T> GetAsync<T>(string url, List<KeyValuePair<string, string>> additionalHeaders = null) where T : class
        {
            AddHeaders(additionalHeaders);

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<T>(content);
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            throw new Exception($"[{response.StatusCode}]:{response.ReasonPhrase}");
        }

        protected async Task<T1> PostAsync<T1, T2>(string uri, T2 model, List<KeyValuePair<string, string>> additionalHeaders = null,
            string authenticationScheme = "Bearer")
            where T1 : class
        {
            AddHeaders(additionalHeaders, authenticationScheme);

            var serializedObject = JsonConvert.SerializeObject(model);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, content);
            var responseAsString = await response?.Content?.ReadAsStringAsync();

            if (typeof(T1) == typeof(string))
                return responseAsString as T1;

            return JsonConvert.DeserializeObject<T1>(responseAsString);
        }

        protected async Task<T> PutFileAsync<T>(string uri, Stream stream, List<KeyValuePair<string, string>> additionalHeaders = null)
        {
            AddHeaders(additionalHeaders);

            // Create content
            var content = new StreamContent(stream);

            // Set content type
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            // Get response
            var response = await _httpClient.PutAsync(uri, content);
            var responseAsString = await response?.Content?.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseAsString);
        }

        public async Task<T1> PutAsync<T1, T2>(string uri, T2 model, List<KeyValuePair<string, string>> additionalHeaders)
        {
            AddHeaders(additionalHeaders);

            var serializedObject = JsonConvert.SerializeObject(model);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(uri, content);
            var responseAsString = await response?.Content?.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T1>(responseAsString);
        }

        protected async Task<T1> PatchAsync<T1, T2>(string uri, T2 model, List<KeyValuePair<string, string>> additionalHeaders = null)
        {
            AddHeaders(additionalHeaders);

            var serializedObject = JsonConvert.SerializeObject(model);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(uri, content);
            var responseAsString = await response?.Content?.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T1>(responseAsString);
        }

        protected async Task<T> DeleteAsync<T>(string uri, List<KeyValuePair<string, string>> additionalHeaders = null)
        {
            AddHeaders(additionalHeaders);

            var response = await _httpClient.DeleteAsync(uri);
            var responseAsString = await response?.Content?.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseAsString);
        }

        protected async Task<T> PostStringAsync<T>(string model, List<KeyValuePair<string, string>> additionalHeaders = null)
        {
            AddHeaders(additionalHeaders);

            var content = new StringContent(model, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(string.Empty, content);
            var responseAsString = await response?.Content?.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseAsString);
        }

        #region Authentication

        /// <summary>
        /// Gets the headers to make authenticated call 
        /// </summary>
        /// <returns>Http headers</returns>
        public virtual async Task<List<KeyValuePair<string, string>>> GetAuthenticatedHeadersAsync()
        {
            var token = await GetAccessTokenAsync();

            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Content-Type", "application/json"),
                new KeyValuePair<string, string>("Authorization", token)
            };
        }

        /// <summary>
        /// Get an access token - override mandatory
        /// </summary>
        /// <returns>Access token</returns>
        public virtual async Task<string> GetAccessTokenAsync()
        {
            return string.Empty;
        }

        #endregion

        #region Helpers

        private void AddHeaders(List<KeyValuePair<string, string>> additionalHeaders = null, string authenticationScheme = "Bearer")
        {
            if (additionalHeaders == null)
                additionalHeaders = new List<KeyValuePair<string, string>>();

            foreach (var header in additionalHeaders)
            {
                if (header.Key == "Authorization")
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationScheme, header.Value);
                else
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

        }

        #endregion
    }
}