namespace Data.Models;

public class Environment
{
    public int Id { get; init; }
    public string Name { get; set; }
    
    public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();
}