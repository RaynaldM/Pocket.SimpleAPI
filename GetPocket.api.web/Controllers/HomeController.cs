using System.Configuration;
using System.Web.Mvc;
using GetPocket.api.web.Models;
using Pocket.api;

namespace GetPocket.api.web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var consumerKey = ConfigurationManager.AppSettings["GetPocketConsumerKey"];

            var model = new IndexViewModel() { ConsumerKey = consumerKey };

            ViewBag.Connected = this.Session["myToken"] != null;
            ViewBag.PocketUser = this.Session["myToken"] != null ? (this.Session["myToken"] as PocketAccessToken).username : null;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                //  In case of difficulty for the callbac, set up your .hosts (Windows) or etc/hosts file to point a live domain to your localhost IP. such as:
                //  127.0.0.1 xyz.com
                //  where xyz.com is your real domain.

                // following auth process : http://getpocket.com/developer/docs/authentication
                var myPocket = new PocketClient(model.ConsumerKey, Request.Url.AbsoluteUri + Url.Action("Authorized"));

                // Step 2: Obtain a request token
                //To begin the Pocket authorization process, 
                //your application must obtain a request token from our servers by making a POST request.
                myPocket.GetAccessToken();
                if (myPocket.AccessToken != null)
                {
                    //Step 3: Redirect user to Pocket to continue authorization
                    //Once you have a request token, 
                    //you need to redirect the user to Pocket to authorize your application's request token.
                    var url = myPocket.PocketAuthorizePageUrl();
                    this.Session["GetPocketObject"] = myPocket;

                    return Redirect(url);
                }
                return new HttpUnauthorizedResult();
            }
            return View(model);
        }

        public ActionResult Authorized()
        {
            /* Step 4: Receive the callback from Pocket
               When the user has authorized (or rejected) your application's request token,
               Pocket will return the user to your application by opening the redirect_uri
               that you provided in your call to /v3/oauth/request (Step 2) */
            var myPocket = this.Session["GetPocketObject"] as PocketClient;

            /* Step 5: Convert a request token into a Pocket access token
               The final step to authorize Pocket with your application is to convert 
               the request token into a Pocket access token. The Pocket access token is
               the user specific token that you will use to make further calls to the Pocket API. */

            var result = myPocket.PocketAccessToken();
            this.Session["myToken"] = result;
            return RedirectToAction("index");
        }

        public JsonResult RetrieveGetPocket()
        {
            var myPocket = this.Session["GetPocketObject"] as PocketClient;
            var result = myPocket.Retrieve((this.Session["myToken"] as PocketAccessToken).access_token);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


    }
}