using System.Text.Json;

namespace Data.Models;

public class Session : CollectionLite
{
    public JsonElement Json { get; set; }
}