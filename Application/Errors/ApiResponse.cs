using System;
using System.Net;

namespace Application.Errors
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        

        public ApiResponse(HttpStatusCode statusCode, string message = null)
        {
            StatusCode = statusCode;
            ErrorMessage = message ?? GetDefaultErrorMessageForStatusCode(statusCode);
        }

        private string GetDefaultErrorMessageForStatusCode(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.Forbidden => "Access denied",
                HttpStatusCode.BadRequest => "A bad request, you have maid",
                HttpStatusCode.Unauthorized => "You are not authorized",
                HttpStatusCode.NotFound => "Resource not found",
                HttpStatusCode.Conflict =>"Conflicts of Data",
                HttpStatusCode.MethodNotAllowed => "Method Not Allowed",
                HttpStatusCode.InternalServerError =>
                    "Errors are the path to the dark side. Errors leads to anger. Anger leads to hate. Hate leads to career change",
                _ => null
            };
        }
    }
}