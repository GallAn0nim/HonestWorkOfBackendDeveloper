using Microsoft.AspNetCore.Mvc;

namespace HonestWork.Api.Shared;

public record ValidationError(string Field, string Message);

public static class ValidationErrorExtensions
{
    public static ActionResult Invalid(this ControllerBase controller, ValidationError error)
    {
        controller.ModelState.AddModelError(error.Field, error.Message);
        return controller.ValidationProblem(controller.ModelState);
    }
}
