using System.Net;
using Application.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    public class BuggyController : BaseController
    {
        [AllowAnonymous]
        [HttpGet("notfound")]
        public ActionResult GetNotFoundRequest()
        {
            return NotFound(new ApiResponse(HttpStatusCode.NotFound));
        }

        [AllowAnonymous]
        [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
            string thing = null;

            var thingToReturn = thing.ToString();
            
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(HttpStatusCode.BadRequest));
        }

        [AllowAnonymous]
        [HttpGet("badrequest/{id}")]
        public ActionResult GetNotFoundRequest(int id)
        {
            return BadRequest();
        }

    }
}