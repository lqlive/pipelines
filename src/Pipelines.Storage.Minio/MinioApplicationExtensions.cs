using Pipelines.Core;

namespace Pipelines.Storage.Minio;
public static class MinioApplicationExtensions
{
    public static PipelinesApplication AddMinioStorage(this PipelinesApplication app,
        Action<MinioStorageOptions> configure)
    {
        return app;
    }
}