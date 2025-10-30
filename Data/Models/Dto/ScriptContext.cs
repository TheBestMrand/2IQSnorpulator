namespace Data.Models.Dto;

public class ScriptContext
{
    public Dictionary<string, string> Environment { get; set; } = new();
    public ApiResponse? Response { get; set; }
    public Request? Request { get; set; }
}