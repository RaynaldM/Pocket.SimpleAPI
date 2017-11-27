using System;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Deserializers;

namespace Pocket.api
{
    public partial class PocketClient
    {
        /// <summary>
        /// Query in asynchrone mode GetPocket and parse JSON response in define object
        /// </summary>
        /// <typeparam name="T">Object type Expected</typeparam>
        /// <param name="restService">Service uri to query</param>
        /// <param name="method">Get or Post</param>
        /// <param name="objectForRequest">Other parameters embedded in an object</param>
        /// <param name="getAuth">Try to get token</param>
        /// <returns>The JSON response parse in T type</returns>
        public Task<T> PocketRequestAsync<T>(String restService, Method method = Method.GET, Object objectForRequest = null, Boolean getAuth = true)
            where T : class
        {
            if (getAuth && String.IsNullOrWhiteSpace(this.AccessToken))
            {
                this.GetAccessToken();
            }

            var request = new RestRequest(restService, method)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = { ContentType = "application/json; charset=UTF-8" },
            };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddHeader("X-Accept", "application/json");

            if (objectForRequest != null)
            {
                request.AddBody(objectForRequest);
            }

            var tcs = new TaskCompletionSource<T>();

            this.PocketRestClient.ExecuteAsync(request, reponseasync =>
            {
                if (reponseasync.ErrorException != null)
                {
                    const string message = "Error retrieving async response.  Check inner details for more info.";
                    var requestException = new ApplicationException(message, reponseasync.ErrorException);
                    throw requestException;
                }

                if (typeof(T) == typeof(JObject))
                {
                    // due to the JSON format come from Pocket
                    // we should do a "manual" deserialization
                    var dynamicResult = JsonConvert.DeserializeObject(reponseasync.Content);
                    tcs.SetResult(dynamicResult as T);
                }
                else
                {
                    // response sent in JSON format and deserialized
                    var deserializer = new JsonDeserializer();
                    var ret = deserializer.Deserialize<T>(reponseasync);
                    tcs.SetResult(ret);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// Pocket's /v3/get endpoint is a single call that is incredibly versatile. A few examples of the types of requests you can make:
        ///      Retrieve a user’s list of unread items
        ///      Sync data that has changed since the last time your app checked
        ///      Retrieve paged results sorted by the most recent saves
        ///      Retrieve just videos that the user has saved
        ///      Search for a given keyword in item’s title and url
        ///      Retrieve all items for a given domain
        ///      and more
        /// </summary>
        /// <param name="request">A completed Retrieve Request Object (except the consumer Key)</param>
        /// <returns>The JSON response will include a list object. This object will contain all of the items that matched your retrieval request.</returns>
        public async Task<RetrieveResult> RetrieveAsync(RetrieveRequest request)
        {
            request.consumer_key = this.ConsumerKey;
            var tmpResult = await this.PocketRequestAsync<JObject>(GetService, Method.POST, request);
            return this.DecomposeDynamicResult(tmpResult);
        }

        /// <summary>
        /// Pocket's /v3/get endpoint is a single call that is incredibly versatile. A few examples of the types of requests you can make:
        ///      Retrieve a user’s list of unread items
        ///      Sync data that has changed since the last time your app checked
        ///      Retrieve paged results sorted by the most recent saves
        ///      Retrieve just videos that the user has saved
        ///      Search for a given keyword in item’s title and url
        ///      Retrieve all items for a given domain
        ///      and more
        /// </summary>
        /// <param name="userToken">Authorize Token of the user</param>
        /// <param name="count">Only return count number of items</param>
        /// <param name="detailType">
        ///      simple = only return the titles and urls of each item
        ///      complete = return all data about each item, including tags, images, authors, videos and more
        /// </param>
        /// <param name="tag">
        ///   tag_name = only return items tagged with tag_name
        ///   _untagged_ = only return untagged items
        /// </param>
        /// <param name="sort">
        ///    newest = return items in order of newest to oldest
        ///    oldest = return items in order of oldest to newest
        ///    title = return items in order of title alphabetically
        ///    site = return items in order of url alphabetically
        /// </param>
        /// <param name="state">
        ///   unread = only return unread items (default)
        ///   archive = only return archived items
        ///   all = return both unread and archived items
        /// </param>
        /// <returns>The JSON response will include a list object. This object will contain all of the items that matched your retrieval request.</returns>
        public Task<RetrieveResult> RetrieveAsync(
            string userToken,
            int count = 0,
            string detailType = "complete",
            string tag = null,
            string sort = "newest",
            string state = "unread"
            )
        {
            var parameters = new RetrieveRequest
            {
                access_token = userToken,
                detailType = detailType,
                count = count == 0 ? null : count.ToString(CultureInfo.InvariantCulture),
                sort = sort,
                state = state,
                tag = tag
            };
            return this.RetrieveAsync(parameters);
        }

        /// <summary>
        /// Allows you to make a change or batch several changes to a user’s list or Pocket data.
        /// </summary>
        /// <param name="request">A completed request</param>
        /// <returns>The response you receive back contains a status variable and an action_results array that indicates which action had an issue if the status is 0 (indicating failure)</returns>
        public async Task<ModifyResult> ModifyAsync(ModifyRequest request)
        {
            request.consumer_key = this.ConsumerKey;
            return await this.PocketRequestAsync<ModifyResult>(ModifyService, Method.POST, request);
        }
    }
}
