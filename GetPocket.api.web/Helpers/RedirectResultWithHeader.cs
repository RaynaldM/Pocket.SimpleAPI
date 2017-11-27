using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace GetPocket.api.web.Helpers
{
    public class RedirectWithHeaderResult : ActionResult
    {
        private readonly Uri _location;
        private readonly Dictionary<String, String> _headerDico;
        private readonly String _contentType;
        private readonly String _xAccept;

        public RedirectWithHeaderResult(Uri location, Dictionary<String, String> headerDictionary, String contenType = null, String xAccept = null)
        {
            this._location = location;
            this._headerDico = headerDictionary;
            this._contentType = contenType;
            this._xAccept = xAccept;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            //HttpResponseBase response = context.HttpContext.Response;
            //HttpRequestBase request = context.HttpContext.Request;

            if (this._contentType != null)
            {
                context.HttpContext.Response.ContentType = this._contentType;
            }
            if (this._xAccept != null)
            {
                context.HttpContext.Response.Headers.Add("X-Accept", this._xAccept);
            }
            foreach (var item in this._headerDico)
            {
                context.HttpContext.Response.Headers.Add(item.Key, item.Value);
            }
            //context.HttpContext.Response.
            context.HttpContext.Response.Redirect(this._location.AbsoluteUri);
            
          
        }
    }
}