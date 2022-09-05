using System.ComponentModel.DataAnnotations;

namespace PaymentApiTest.Models;

public class TestModelHelper
{
    public static IList<ValidationResult> Validate(object model)
    {
        var errors = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, errors, true);
        if (model is IValidatableObject) (model as IValidatableObject).Validate(validationContext);

        return errors;
    }
}
