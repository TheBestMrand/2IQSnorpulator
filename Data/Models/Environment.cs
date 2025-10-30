namespace Data.Models;

public class Environment : CollectionLite
{
    public Dictionary<string, string> Variables { get; set; } = new();
}