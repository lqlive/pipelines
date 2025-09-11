namespace Pipelines.Core.Storage;
public interface IStorageService
{
    Task<StoragePutResult> PutAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default);
}

public enum StoragePutResult
{
    /// <summary>
    /// The given path is already used to store different content.
    /// </summary>
    Conflict,

    /// <summary>
    /// This content is already stored at the given path.
    /// </summary>
    AlreadyExists,

    /// <summary>
    /// The content was sucessfully stored.
    /// </summary>
    Success,
}