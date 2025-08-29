using ErrorOr;

namespace Pipelines.Errors;

public static class RepositoryErrors
{
    public static Error RepositoryNotFound => Error.NotFound(
        code: "Repository.NotFound",
        description: "Repository not found");
}
