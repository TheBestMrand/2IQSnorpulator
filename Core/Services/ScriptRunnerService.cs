using Data.Models;
using Data.Models.Dto;
using IronPython.Hosting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Scripting.Hosting;

namespace Core.Services;

public class ScriptRunnerService
{
    private readonly ScriptEngine _pythonEngine;

    public ScriptRunnerService()
    {
        _pythonEngine = Python.CreateEngine();
    }

    // --- C# Script Runner ---
    public async Task<ScriptExecutionResult> RunCSharpScriptAsync(string script, ScriptContext context)
    {
        try
        {
            var globals = new ScriptGlobals { pm = context };
            var scriptOptions = ScriptOptions.Default
                .WithImports(
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Text",
                    "System.Security.Cryptography",
                    "System.Net.Http",
                    "System.IO",
                    "System.Threading.Tasks",
                    "Data.Models",
                    "Data.Models.Dto"
                )
                .WithReferences(
                    typeof(ApiResponse).Assembly,  // Data.Models
                    typeof(System.Security.Cryptography.HMACSHA256).Assembly,  // System.Security.Cryptography
                    typeof(System.Text.Encoding).Assembly,  // System.Text
                    typeof(System.Net.Http.HttpClient).Assembly  // System.Net.Http
                );
            
            ScriptState<object> endState = await CSharpScript.RunAsync(script, options: scriptOptions, globals: globals);
            
            return ScriptExecutionResult.Success(globals.pm, endState.ReturnValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"C# SCRIPT ERROR: {ex.Message}");
            return ScriptExecutionResult.Failure(context, ex.Message);
        }
    }

    // --- Python Script Runner ---
    public ScriptExecutionResult RunPythonScriptAsync(string script, ScriptContext context)
    {
        try
        {
            var scope = _pythonEngine.CreateScope();
            scope.SetVariable("pm", context);
            
            var returnValue = _pythonEngine.Execute(script, scope);
            
            var updatedContext = scope.GetVariable<ScriptContext>("pm");
            
            return ScriptExecutionResult.Success(updatedContext, returnValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PYTHON SCRIPT ERROR: {ex.Message}");
            return ScriptExecutionResult.Failure(context, ex.Message);
        }
    }
}

public class ScriptGlobals
{
    public ScriptContext pm { get; set; } = new();
}