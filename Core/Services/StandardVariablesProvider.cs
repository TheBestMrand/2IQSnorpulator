using System.Text;

namespace Core.Services;

/// <summary>
/// Provides built-in dynamic variables like guid, timestamp, randomInt, etc.
/// </summary>
public class StandardVariablesProvider
{
    private static readonly Random _random = new();
    private static readonly string[] _emailDomains = { "example.com", "test.com", "mail.com", "email.com" };
    
    public static readonly string[] SupportedVariables = 
    { 
        "guid", "uuid", "timestamp", "isotimestamp", 
        "randomint", "randomstring", "randomemail", 
        "datenow", "timenow" 
    };

    /// <summary>
    /// Checks if a variable name is a standard/built-in variable
    /// </summary>
    public bool IsStandardVariable(string name)
    {
        return SupportedVariables.Contains(name.ToLowerInvariant());
    }
    
    /// <summary>
    /// Gets the value for a standard variable
    /// </summary>
    public string GetValue(string variableName)
    {
        return variableName.ToLowerInvariant() switch
        {
            "guid" or "uuid" => Guid.NewGuid().ToString(),
            "timestamp" => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            "isotimestamp" => DateTime.Now.ToString("o"), // ISO 8601
            "randomint" => _random.Next(0, 1000000).ToString(),
            "randomstring" => GenerateRandomString(10),
            "randomemail" => GenerateRandomEmail(),
            "datenow" => DateTime.Now.ToString("yyyy-MM-dd"),
            "timenow" => DateTime.Now.ToString("HH:mm:ss"),
            _ => $"{{UNKNOWN_VAR:{variableName}}}"
        };
    }
    
    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var result = new StringBuilder(length);
        
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[_random.Next(chars.Length)]);
        }
        
        return result.ToString();
    }
    
    private static string GenerateRandomEmail()
    {
        var username = GenerateRandomString(8).ToLower();
        var domain = _emailDomains[_random.Next(_emailDomains.Length)];
        return $"{username}@{domain}";
    }
}
