using System.Collections.Generic;

namespace Desktop.Models;

public class CodeSnippet
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Code { get; set; } = "";
    public string Language { get; set; } = "C#";
}

public static class CodeSnippets
{
    public static List<CodeSnippet> CSharpPreRequestSnippets { get; } = new()
    {
        new CodeSnippet
        {
            Title = "Set Variable",
            Description = "Set a variable for use in request",
            Language = "C#",
            Code = @"// Set a custom variable
ctx.SetVariable(""myVar"", ""value"");

// Set from timestamp
ctx.SetVariable(""timestamp"", DateTime.UtcNow.ToString(""o""));

// Set random value
ctx.SetVariable(""randomId"", Guid.NewGuid().ToString());"
        },
        new CodeSnippet
        {
            Title = "Modify Headers",
            Description = "Add or modify request headers",
            Language = "C#",
            Code = @"// Add authorization header
ctx.Request.Headers[""Authorization""] = ""Bearer "" + ctx.GetVariable(""token"");

// Add custom header
ctx.Request.Headers[""X-Custom-Header""] = ""MyValue"";

// Remove a header
ctx.Request.Headers.Remove(""Content-Type"");"
        },
        new CodeSnippet
        {
            Title = "Modify URL",
            Description = "Change request URL dynamically",
            Language = "C#",
            Code = @"// Replace part of URL
var baseUrl = ctx.GetVariable(""apiUrl"");
ctx.Request.Url = $""{baseUrl}/users/{ctx.GetVariable(""userId"")}"";

// Add query parameter
ctx.Request.Query[""page""] = ""1"";
ctx.Request.Query[""limit""] = ""10"";"
        },
        new CodeSnippet
        {
            Title = "Modify Body",
            Description = "Change request body before sending",
            Language = "C#",
            Code = @"// Parse and modify JSON body
var json = System.Text.Json.JsonDocument.Parse(ctx.Request.Body);
// ... modify json ...
ctx.Request.Body = System.Text.Json.JsonSerializer.Serialize(json);

// Set body from variable
ctx.Request.Body = ctx.GetVariable(""requestBody"");"
        }
    };

    public static List<CodeSnippet> CSharpPostRequestSnippets { get; } = new()
    {
        new CodeSnippet
        {
            Title = "Extract from Response",
            Description = "Save data from response to variables",
            Language = "C#",
            Code = @"// Extract from JSON response
var json = System.Text.Json.JsonDocument.Parse(ctx.Response.Body);
var token = json.RootElement.GetProperty(""token"").GetString();
ctx.SetVariable(""authToken"", token);

// Extract from headers
if (ctx.Response.Headers.TryGetValue(""X-Session-Id"", out var sessionId))
{
    ctx.SetVariable(""sessionId"", sessionId);
}"
        },
        new CodeSnippet
        {
            Title = "Assert Response",
            Description = "Validate response data",
            Language = "C#",
            Code = @"// Check status code
if (ctx.Response.StatusCode != 200)
{
    throw new Exception($""Expected 200, got {ctx.Response.StatusCode}"");
}

// Check response body
if (!ctx.Response.Body.Contains(""success""))
{
    throw new Exception(""Response does not contain 'success'"");
}"
        },
        new CodeSnippet
        {
            Title = "Log Response",
            Description = "Log response details",
            Language = "C#",
            Code = @"// Log response details
Console.WriteLine($""Status: {ctx.Response.StatusCode}"");
Console.WriteLine($""Time: {ctx.Response.ResponseTime.TotalMilliseconds}ms"");
Console.WriteLine($""Size: {ctx.Response.Size} bytes"");
Console.WriteLine($""Body: {ctx.Response.Body}"");"
        }
    };

    public static List<CodeSnippet> PythonPreRequestSnippets { get; } = new()
    {
        new CodeSnippet
        {
            Title = "Set Variable",
            Description = "Set a variable for use in request",
            Language = "Python",
            Code = @"# Set a custom variable
ctx.SetVariable(""myVar"", ""value"")

# Set from datetime
import datetime
ctx.SetVariable(""timestamp"", datetime.datetime.utcnow().isoformat())

# Set random value
import uuid
ctx.SetVariable(""randomId"", str(uuid.uuid4()))"
        },
        new CodeSnippet
        {
            Title = "Modify Headers",
            Description = "Add or modify request headers",
            Language = "Python",
            Code = @"# Add authorization header
token = ctx.GetVariable(""token"")
ctx.Request.Headers[""Authorization""] = f""Bearer {token}""

# Add custom header
ctx.Request.Headers[""X-Custom-Header""] = ""MyValue""

# Remove a header
if ""Content-Type"" in ctx.Request.Headers:
    del ctx.Request.Headers[""Content-Type""]"
        },
        new CodeSnippet
        {
            Title = "Modify URL",
            Description = "Change request URL dynamically",
            Language = "Python",
            Code = @"# Replace part of URL
base_url = ctx.GetVariable(""apiUrl"")
user_id = ctx.GetVariable(""userId"")
ctx.Request.Url = f""{base_url}/users/{user_id}""

# Add query parameter
ctx.Request.Query[""page""] = ""1""
ctx.Request.Query[""limit""] = ""10"""
        }
    };

    public static List<CodeSnippet> PythonPostRequestSnippets { get; } = new()
    {
        new CodeSnippet
        {
            Title = "Extract from Response",
            Description = "Save data from response to variables",
            Language = "Python",
            Code = @"# Extract from JSON response
import json
data = json.loads(ctx.Response.Body)
token = data.get(""token"")
ctx.SetVariable(""authToken"", token)

# Extract from headers
if ""X-Session-Id"" in ctx.Response.Headers:
    session_id = ctx.Response.Headers[""X-Session-Id""]
    ctx.SetVariable(""sessionId"", session_id)"
        },
        new CodeSnippet
        {
            Title = "Assert Response",
            Description = "Validate response data",
            Language = "Python",
            Code = @"# Check status code
if ctx.Response.StatusCode != 200:
    raise Exception(f""Expected 200, got {ctx.Response.StatusCode}"")

# Check response body
if ""success"" not in ctx.Response.Body:
    raise Exception(""Response does not contain 'success'"")"
        }
    };
}

