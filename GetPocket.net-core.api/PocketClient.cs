using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Pocket.api
{
    /// <summary>
    /// Pocket Rest Client
    /// </summary>
    public partial class PocketClient
    {
        private const string PocketBaseUrl = "https://getpocket.com";
        /// <summary>
        /// Defines URI of service which issues access token.
        /// </summary>
        private const string AccessTokenService = "/v3/oauth/request.php";
        /// <summary> 
        /// Defines URI of service which redirect to login/autorized
        /// </summary>
        private const string AuthorizeIHMService = "/auth/authorize.php";
        /// <summary> 
        /// Defines URI of service which redirect to login/autorized
        /// </summary>
        private const string AuthorizeService = "/v3/oauth/authorize.php";
        /// <summary>
        /// Retrieving a User's Pocket Data
        /// To retrieve item(s) from a user’s Pocket list, you’ll make a request to the /v3/get endpoint.
        /// </summary>
        private const string GetService = "/v3/get.php";
        /// <summary>
        /// Adding a Single Item
        /// To save an item to a user’s Pocket list, you’ll make a single request to the /v3/add endpoint
        /// </summary>
        private const string AddService = "/v3/add.php";
        /// <summary>
        /// Modifying a User's Pocket Data
        /// The /v3/send endpoint allows your application to send a single event or 
        /// multiple events and actions that will modify the user's data in one call.
        /// </summary>
        private const string ModifyService = "/v3/send.php";

        /// <summary>
        /// Persistant Rest client for GetPocket
        /// </summary>
        private RestClient _pocketRestClient;
        /// <summary>
        /// Accessor to a singleton request (restsharp) object
        /// </summary>
        protected RestClient PocketRestClient => this._pocketRestClient ?? (this._pocketRestClient = new RestClient(PocketBaseUrl));

        /// <summary>
        /// Access token returned by provider. Can be used for further calls of provider API.
        /// </summary>
        public string AccessToken { get; private set; }
        /// <summary>
        /// Consumer Key (ID of your application).
        /// </summary>
        public string ConsumerKey { get; set; }
        /// <summary>
        /// Redirect URI (URI user will be redirected to
        /// after authentication using third-party service).
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Initializes a new instance of the GetPocket class.
        /// </summary>
        public PocketClient(string consumerKey, string redirect = null, string token = null)
        {
            if (string.IsNullOrWhiteSpace(consumerKey))
            {
                throw new ArgumentNullException();
            }
            this.ConsumerKey = consumerKey;
            this.RedirectUri = redirect;
            this.AccessToken = token;
        }

        /// <summary>
        /// Returns URI of service which should be called in order to start authentication process.
        /// This URI should be used for rendering login link.
        /// </summary>
        /// <remarks>
        /// Any additional information that will be posted back by service.
        /// </remarks>
        public string PocketAuthorizePageUrl()
        {
            var request = new RestRequest(AuthorizeIHMService);

            request.AddObject(new
            {
                request_token = this.AccessToken,
                redirect_uri = this.RedirectUri
            });

            return this.PocketRestClient.BuildUri(request).ToString();
        }

        /// <summary>
        /// Get an app token
        /// </summary>
        /// <returns>the token</returns>
        public async Task<string> GetToken()
        {
            if (string.IsNullOrWhiteSpace(this.AccessToken))
            {
               await this.GetAccessToken();
            }
            return this.AccessToken;
        }

        /// <summary>
        /// Query for access token and parses response.
        /// </summary>
        public async Task GetAccessToken()
        {
            var result = await this.PocketRequestAsync<AuthRequestResult>(AccessTokenService, Method.POST,
                new AuthRequest
                {
                    consumer_key = this.ConsumerKey,
                    redirect_uri = this.RedirectUri
                },
                false);

            this.AccessToken = result?.code;
        }

        /// <summary>
        /// Retrieve the Token from Pocket API
        /// </summary>
        /// <returns></returns>
        public  Task<PocketAccessToken> PocketAccessToken()
        {
            var result =  this.PocketRequestAsync<PocketAccessToken>(AuthorizeService, Method.POST,
               new PocketAccessTokenRequest
               {
                   consumer_key = this.ConsumerKey,
                   code = this.AccessToken
               });

            return result;
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
        public async Task<RetrieveResult> Retrieve(RetrieveRequest request)
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
        public Task<RetrieveResult> Retrieve(
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
            return this.Retrieve(parameters);
        }

        /// <summary>
        /// Allowing users to add articles, videos, images and URLs to Pocket is most likely the first type of integration that you’ll want to build into your application.
        /// </summary>
        /// <param name="userToken">Authorize Token of the user</param>
        /// <param name="url">The URL of the item you want to save</param>
        /// <param name="title">This can be included for cases where an item does not have a title, which is typical for image or PDF URLs. 
        /// If Pocket detects a title from the content of the page, this parameter will be ignored.</param>
        /// <param name="tags">A comma-separated list of tags to apply to the item</param>
        /// <param name="tweetId">If you are adding Pocket support to a Twitter client, please send along a reference to the tweet status id. 
        /// This allows Pocket to show the original tweet alongside the article.</param>
        /// <returns>The response contains all of the meta information we have resolved about the saved item</returns>
        public Task<AddResult> Add(string userToken, string url, string title = null, string tags = null, string tweetId = null)
        {
            var newArticle = new ArticleRequest
            {
                consumer_key = this.ConsumerKey,
                access_token = userToken,
                url = url,
                title = title,
                tags = tags,
                tweet_id = tweetId
            };
            return this.Add(newArticle);
        }

        /// <summary>
        /// Allowing users to add articles, videos, images and URLs to Pocket is most likely the first type of integration that you’ll want to build into your application.
        /// </summary>
        /// <param name="newArticle">An completed Add Request</param>
        /// <returns>The response contains all of the meta information we have resolved about the saved item</returns>
        public Task<AddResult> Add(ArticleRequest newArticle)
        {
            newArticle.consumer_key = this.ConsumerKey;
            return this.PocketRequestAsync<AddResult>(AddService, Method.POST, newArticle);
        }

        /// <summary>
        /// Allows you to make a change to Pocket data.
        /// </summary>
        /// <param name="userToken">Authorize Token of the user</param>
        /// <param name="action">Action to do</param>
        /// <param name="id">The id of the item to perform the action on.</param>
        /// <param name="time">The time the action occurred</param>
        /// <param name="tags">A comma-delimited list of one or more tags to add.</param>
        /// <returns></returns>
        public async Task<SingleModifyResult> Modify(string userToken, Action action, long id, int? time = null, string tags = null)
        {
            if (action == Action.Add || action == Action.TagRename)
            {
                throw new NotSupportedException(string.Format("{0} action is not supported", EnumHelpers.GetStringValue(action)));
            }

            var newAction = new ModifyRequest
            {
                access_token = userToken,
                consumer_key = this.ConsumerKey,
                actions = new List<ModifyAction>
                {
                    new ModifyAction
                    {
                        action=EnumHelpers.GetStringValue(action),
                        item_id=id,
                        time=time,
                        tags=tags
                    }
                }
            };

            var result = await this.Modify(newAction);
            return new SingleModifyResult { action_result = result.action_results[0], status = result.status };
        }

        /// <summary>
        /// Allows you to make a change or batch several changes to a user’s list or Pocket data.
        /// </summary>
        /// <param name="request">A completed request</param>
        /// <returns>The response you receive back contains a status variable and an action_results array that indicates which action had an issue if the status is 0 (indicating failure)</returns>
        public Task<ModifyResult> Modify(ModifyRequest request)
        {
            request.consumer_key = this.ConsumerKey;
            return this.PocketRequestAsync<ModifyResult>(ModifyService, Method.POST, request);
        }

        private RetrieveResult DecomposeDynamicResult(JObject obj)
        {
            var result = new RetrieveResult
            {
                status = obj.Value<int>("status"),
                complete = obj.Value<int>("complete"),
                since = obj.Value<int>("since"),
                search_meta = obj.GetValue("search_meta").ToObject<SearchMeta>(),
                error = obj.GetValue("error"),
                list = new List<Item>()
            };

            var tmpList = obj.GetValue("list").Children().AsJEnumerable();
            foreach (var item in tmpList)
            {
                // todo : deserialize Images and Videos array
                var realItem = item.First.ToObject<Item>();
                result.list.Add(realItem);
            }
            return result;
        }
    }
}
