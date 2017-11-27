using System.Collections.Generic;
using System.Web.Http;

namespace GetPocket.api.web.Controllers
{
    /// <summary>
    /// Use just for demonstate document capabilies of WebApi HelpPages
    /// </summary>
    public class ValuesControllerMock : ApiController
    {
        /// <summary>
        /// Gets some very important data from the server.
        /// </summary>
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        /// <summary>
        /// Looks up some data by ID.
        /// </summary>
        /// <param name="id">The ID of the data.</param>
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}