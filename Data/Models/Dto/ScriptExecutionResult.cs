namespace Data.Models.Dto;

public class ScriptExecutionResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public ScriptContext Context { get; private set; } = new();
    public object? ReturnValue { get; private set; }
    
    public static ScriptExecutionResult Success(ScriptContext context, object? returnValue = null)
    {
        return new ScriptExecutionResult 
        { 
            IsSuccess = true, 
            Context = context, 
            ReturnValue = returnValue 
        };
    }
    
    public static ScriptExecutionResult Failure(ScriptContext context, string errorMessage)
    {
        return new ScriptExecutionResult 
        { 
            IsSuccess = false, 
            Context = context, 
            ErrorMessage = errorMessage 
        };
    }
}