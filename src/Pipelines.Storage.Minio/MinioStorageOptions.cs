using System.ComponentModel.DataAnnotations;

namespace Pipelines.Storage.Minio;
public class MinioStorageOptions : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        throw new NotImplementedException();
    }
}
