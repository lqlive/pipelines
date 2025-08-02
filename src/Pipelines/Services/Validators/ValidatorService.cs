using ErrorOr;
using Microsoft.Extensions.Caching.Distributed;
using Pipelines.Errors;

namespace Pipelines.Services.Validators;
public class ValidatorService(IDistributedCache cache, ILogger<ValidatorService> logger)
{

    public async Task<ErrorOr<Success>> SendCodeAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        var code = "11";


        var cacheKey = $"Validator-{email}";
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        try
        {
            await cache.SetStringAsync(cacheKey, code, cacheOptions, cancellationToken);

            logger.LogInformation("Verification code sent to {Email}", email);
            return Result.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to store verification code for {Email}", email);
            return ValidatorErrors.SendFailed;
        }
    }

    public async Task<ErrorOr<Success>> ValidateAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        var cacheKey = $"Validator-{key}";

        try
        {
            var cachedValue = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (cachedValue == null)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                await cache.SetStringAsync(cacheKey, value, options, cancellationToken);

                return Result.Success;
            }

            if (cachedValue == value)
            {
                return Result.Success;
            }

            logger.LogWarning("Validator: Value mismatch for key {Key}. Expected: {Expected}, Actual: {Actual}",
                key, cachedValue, value);
            return ValidatorErrors.ValueMismatch;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Validator: Error during validation for key {Key}", key);
            return ValidatorErrors.ValidationFailed;
        }
    }
}