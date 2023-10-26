using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ValidateModelOnActionExecuting
{
    public class InvalidModelStateHandlerAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var data = context.ModelState.Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => new FieldError(
                    x.Key,
                    x.Value?.Errors.Select(x => x.ErrorMessage).ToArray() ?? new string[] { "" }
                    )).ToArray();
                var resData = new ResponseData<FieldError[]>()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Data = data,
                    Message = "Model error"
                };
                context.Result = new JsonResult(resData)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    ContentType = "application/json"
                };
            }
            base.OnActionExecuting(context);
        }

    }

    public class FieldError
    {
        public FieldError(string property, string[] errors)
        {
            Property = property;
            Errors = errors;
        }

        public FieldError(string property, string error)
        {
            Property = property;
            Errors = new string[] { error };
        }

        public string Property { get; set; }
        public string[]? Errors { get; set; }
    }

    public class ResponseData<T>
    {
        public int? Status { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }
    public class ResponseData
    {
        public int? Status { get; set; }
        public string? Message { get; set; }
    }

    public class User
    {
        //[RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-]+$")]
        public string Email { get; set; }
        [Required]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Password Contain 6 Characters include number 1 special character & 1 Uppercase Letter.")]
        public string Password { get; set; }
    }
    [InvalidModelStateHandler]
    public class BaseController : Controller
    { 
    }
}
