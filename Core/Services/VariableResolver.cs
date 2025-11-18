using System.Text;
using System.Text.RegularExpressions;
using Data.Models.Dto;

namespace Core.Services;

/// <summary>
/// High-performance variable resolver supporting {$var} and {!var} syntax
/// - {$var} = Deferred resolution (after pre-request scripts)
/// - {!var} = Immediate resolution (before pre-request scripts)
/// </summary>
public partial class VariableResolver
{
    private readonly StandardVariablesProvider _standardVariables;
    
    // Compiled regex patterns for performance
    [GeneratedRegex(@"\{\!(\w+)\}", RegexOptions.Compiled)]
    private static partial Regex ImmediateVariablePattern();
    
    [GeneratedRegex(@"\{\$(\w+)\}", RegexOptions.Compiled)]
    private static partial Regex DeferredVariablePattern();

    public VariableResolver(StandardVariablesProvider standardVariables)
    {
        _standardVariables = standardVariables;
    }
    
    /// <summary>
    /// Resolves immediate variables {!var} - executed BEFORE pre-request scripts
    /// </summary>
    public string ResolveImmediateVariables(string? text, ScriptContext context)
    {
        if (string.IsNullOrEmpty(text)) return text ?? string.Empty;
        
        return ImmediateVariablePattern().Replace(text, match =>
        {
            var variableName = match.Groups[1].Value;
            return GetVariableValue(variableName, context);
        });
    }
    
    /// <summary>
    /// Resolves deferred variables {$var} - executed AFTER pre-request scripts
    /// </summary>
    public string ResolveDeferredVariables(string? text, ScriptContext context)
    {
        if (string.IsNullOrEmpty(text)) return text ?? string.Empty;
        
        return DeferredVariablePattern().Replace(text, match =>
        {
            var variableName = match.Groups[1].Value;
            return GetVariableValue(variableName, context);
        });
    }
    
    /// <summary>
    /// Resolves immediate variables in a dictionary of key-value pairs
    /// </summary>
    public Dictionary<string, string> ResolveImmediateVariables(Dictionary<string, string> items, ScriptContext context)
    {
        var resolved = new Dictionary<string, string>();
        
        foreach (var kvp in items)
        {
            resolved[ResolveImmediateVariables(kvp.Key, context)] = 
                ResolveImmediateVariables(kvp.Value, context);
        }
        
        return resolved;
    }
    
    /// <summary>
    /// Resolves deferred variables in a dictionary of key-value pairs
    /// </summary>
    public Dictionary<string, string> ResolveDeferredVariables(Dictionary<string, string> items, ScriptContext context)
    {
        var resolved = new Dictionary<string, string>();
        
        foreach (var kvp in items)
        {
            resolved[ResolveDeferredVariables(kvp.Key, context)] = 
                ResolveDeferredVariables(kvp.Value, context);
        }
        
        return resolved;
    }
    
    /// <summary>
    /// Gets a variable value from various sources with priority:
    /// 1. ResolvedVariables cache (for consistency within same request)
    /// 2. Script-set Variables
    /// 3. Environment variables
    /// 4. Standard variables (guid, timestamp, etc.)
    /// </summary>
    private string GetVariableValue(string variableName, ScriptContext context)
    {
        var key = variableName.ToLowerInvariant();
        
        // 1. Check if already resolved (for consistency)
        if (context.ResolvedVariables.TryGetValue(key, out var cachedValue))
        {
            return cachedValue;
        }
        
        // 2. Check script-set variables (case-insensitive)
        var scriptVar = context.Variables.FirstOrDefault(
            kvp => kvp.Key.Equals(variableName, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrEmpty(scriptVar.Key))
        {
            var value = scriptVar.Value;
            context.ResolvedVariables[key] = value;
            return value;
        }
        
        // 3. Check environment variables (case-insensitive)
        var envVar = context.Environment.FirstOrDefault(
            kvp => kvp.Key.Equals(variableName, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrEmpty(envVar.Key))
        {
            var value = envVar.Value;
            context.ResolvedVariables[key] = value;
            return value;
        }
        
        // 4. Check standard variables
        if (_standardVariables.IsStandardVariable(variableName))
        {
            var value = _standardVariables.GetValue(variableName);
            context.ResolvedVariables[key] = value; // Cache it
            return value;
        }
        
        // Variable not found - return placeholder
        var placeholder = $"{{UNRESOLVED:{variableName}}}";
        context.ResolvedVariables[key] = placeholder;
        return placeholder;
    }
}
