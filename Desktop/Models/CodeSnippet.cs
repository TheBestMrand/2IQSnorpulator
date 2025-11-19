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
        },
        new CodeSnippet
        {
            Title = "Generate Timestamp",
            Description = "Create Unix timestamp or ISO format",
            Language = "C#",
            Code = @"// Unix timestamp (seconds)
var unixTimestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
ctx.SetVariable(""timestamp"", unixTimestamp.ToString());

// ISO 8601 format
var isoTime = DateTime.UtcNow.ToString(""yyyy-MM-ddTHH:mm:ssZ"");
ctx.SetVariable(""isoTime"", isoTime);"
        },
        new CodeSnippet
        {
            Title = "Generate HMAC Signature",
            Description = "Create HMAC signature for authentication",
            Language = "C#",
            Code = @"using System.Security.Cryptography;
using System.Text;

var secret = ctx.GetVariable(""apiSecret"");
var message = ctx.Request.Url + ctx.Request.Body;
var key = Encoding.UTF8.GetBytes(secret);
var hmac = new HMACSHA256(key);
var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
var signature = Convert.ToBase64String(hash);
ctx.Request.Headers[""X-Signature""] = signature;"
        },
        new CodeSnippet
        {
            Title = "Conditional Logic",
            Description = "Execute code based on conditions",
            Language = "C#",
            Code = @"// Check environment variable
var env = ctx.GetVariable(""environment"");
if (env == ""production"")
{
    ctx.Request.Url = ctx.GetVariable(""prodUrl"");
}
else
{
    ctx.Request.Url = ctx.GetVariable(""devUrl"");
}

// Conditional header
if (ctx.Request.Method == ""POST"")
{
    ctx.Request.Headers[""Content-Type""] = ""application/json"";
}"
        },
        new CodeSnippet
        {
            Title = "Generate Random Data",
            Description = "Create random strings, numbers, and UUIDs",
            Language = "C#",
            Code = @"using System.Linq;

// Random string
var random = new Random();
var chars = ""ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"";
var randomString = new string(Enumerable.Range(0, 10)
    .Select(_ => chars[random.Next(chars.Length)]).ToArray());
ctx.SetVariable(""randomString"", randomString);

// Random number
var randomNum = random.Next(1000, 9999);
ctx.SetVariable(""randomNumber"", randomNum.ToString());

// UUID
ctx.SetVariable(""uuid"", Guid.NewGuid().ToString());"
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
        },
        new CodeSnippet
        {
            Title = "Generate Timestamp",
            Description = "Create Unix timestamp or ISO format",
            Language = "Python",
            Code = @"import time
from datetime import datetime

# Unix timestamp (seconds)
timestamp = int(time.time())
ctx.SetVariable(""timestamp"", str(timestamp))

# ISO 8601 format
iso_time = datetime.utcnow().strftime(""%Y-%m-%dT%H:%M:%SZ"")
ctx.SetVariable(""isoTime"", iso_time)"
        },
        new CodeSnippet
        {
            Title = "Generate HMAC Signature",
            Description = "Create HMAC signature for authentication",
            Language = "Python",
            Code = @"import hmac
import hashlib
import base64

secret = ctx.GetVariable(""apiSecret"")
message = ctx.Request.Url + ctx.Request.Body
signature = hmac.new(
    secret.encode('utf-8'),
    message.encode('utf-8'),
    hashlib.sha256
).digest()
signature_b64 = base64.b64encode(signature).decode('utf-8')
ctx.Request.Headers[""X-Signature""] = signature_b64"
        },
        new CodeSnippet
        {
            Title = "Conditional Logic",
            Description = "Execute code based on conditions",
            Language = "Python",
            Code = @"# Check environment variable
env = ctx.GetVariable(""environment"")
if env == ""production"":
    ctx.Request.Url = ctx.GetVariable(""prodUrl"")
else:
    ctx.Request.Url = ctx.GetVariable(""devUrl"")

# Conditional header
if ctx.Request.Method == ""POST"":
    ctx.Request.Headers[""Content-Type""] = ""application/json"""
        },
        new CodeSnippet
        {
            Title = "Generate Random Data",
            Description = "Create random strings, numbers, and UUIDs",
            Language = "Python",
            Code = @"import random
import string
import uuid

# Random string
random_string = ''.join(random.choices(
    string.ascii_uppercase + string.digits, k=10
))
ctx.SetVariable(""randomString"", random_string)

# Random number
random_num = random.randint(1000, 9999)
ctx.SetVariable(""randomNumber"", str(random_num))

# UUID
ctx.SetVariable(""uuid"", str(uuid.uuid4()))"
        },
        new CodeSnippet
        {
            Title = "Modify JSON Body",
            Description = "Parse and modify JSON request body",
            Language = "Python",
            Code = @"import json

# Parse JSON body
if ctx.Request.Body:
    data = json.loads(ctx.Request.Body)
    data[""timestamp""] = ctx.GetVariable(""timestamp"")
    data[""id""] = ctx.GetVariable(""randomId"")
    ctx.Request.Body = json.dumps(data)

# Set body from variable
ctx.Request.Body = ctx.GetVariable(""requestBody"")"
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

