namespace Pipelines.Services;

public class SenderService
{
    public Task<bool> SenderEmailAsync(string email, string subject, string body)
    {
        // Simulate sending an email
        Console.WriteLine($"Sending email to {email} with subject '{subject}' and body '{body}'");
        return Task.FromResult(true);
    }
}

