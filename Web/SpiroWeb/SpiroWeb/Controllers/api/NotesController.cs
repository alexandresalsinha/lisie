using ClassLibrary1;
using SpiroWeb.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SpiroWeb.Controllers
{
    public class NotesController : ApiController
    {
        // GET: api/Notes/Get?userId=
        public HttpResponseMessage Get(string userId)
        {
            Managers.InteractionsManager.Add(userId, "api/Notes/Get", userId);

            if (userId != string.Empty)
            {
                UserNotes _UserNotes = Managers.NotesManager.Get(userId);
                if (_UserNotes != null)
                    return Request.CreateResponse(HttpStatusCode.OK, _UserNotes);
                return Request.CreateResponse(HttpStatusCode.NotFound);

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // POST: api/Notes/Post
        [HttpPost]
        public HttpResponseMessage Post([FromBody] UserNotesPostModel model)
        {

            if (model != null)
            {
                Managers.InteractionsManager.Add(model.UserId, "api/Notes/Post", "");

                UserNotes _UserNotes = Managers.NotesManager.Save(model.UserId, model.Notes);
                if (_UserNotes != null)
                    return Request.CreateResponse(HttpStatusCode.OK, _UserNotes);
                return Request.CreateResponse(HttpStatusCode.NotFound);

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
