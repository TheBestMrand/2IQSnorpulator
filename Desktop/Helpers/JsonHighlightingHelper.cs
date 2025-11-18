using System.Xml;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using Avalonia.Media;

namespace Desktop.Helpers;

public static class JsonHighlightingHelper
{
    public static IHighlightingDefinition GetJsonHighlighting()
    {
        // Create custom JSON highlighting with BRIGHT, readable colors for dark theme
        var xshd = @"<?xml version=""1.0""?>
<SyntaxDefinition name=""JSON"" xmlns=""http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008"">
    <Color name=""String"" foreground=""#CE9178"" />
    <Color name=""Number"" foreground=""#B5CEA8"" />
    <Color name=""Boolean"" foreground=""#569CD6"" />
    <Color name=""Null"" foreground=""#569CD6"" />
    <Color name=""PropertyName"" foreground=""#9CDCFE"" fontWeight=""bold"" />
    <Color name=""Punctuation"" foreground=""#D4D4D4"" />
    
    <RuleSet>
        <!-- Property names (keys) -->
        <Rule color=""PropertyName"">
            ""[^""\\]*(?:\\.[^""\\]*)*""\s*:
        </Rule>
        
        <!-- Strings -->
        <Rule color=""String"">
            ""[^""\\]*(?:\\.[^""\\]*)*""
        </Rule>
        
        <!-- Numbers -->
        <Rule color=""Number"">
            -?\d+\.?\d*([eE][+-]?\d+)?
        </Rule>
        
        <!-- Booleans -->
        <Rule color=""Boolean"">
            \b(true|false)\b
        </Rule>
        
        <!-- Null -->
        <Rule color=""Null"">
            \bnull\b
        </Rule>
        
        <!-- Punctuation -->
        <Rule color=""Punctuation"">
            [{}[\]:,]
        </Rule>
    </RuleSet>
</SyntaxDefinition>";

        using var reader = new XmlTextReader(new System.IO.StringReader(xshd));
        return HighlightingLoader.Load(reader, HighlightingManager.Instance);
    }
    
    public static IHighlightingDefinition GetXmlHighlighting()
    {
        // Create custom XML highlighting with BRIGHT colors
        var xshd = @"<?xml version=""1.0""?>
<SyntaxDefinition name=""XML"" xmlns=""http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008"">
    <Color name=""Element"" foreground=""#569CD6"" />
    <Color name=""Attribute"" foreground=""#9CDCFE"" />
    <Color name=""String"" foreground=""#CE9178"" />
    <Color name=""Comment"" foreground=""#6A9955"" />
    
    <RuleSet>
        <!-- Comments -->
        <Span color=""Comment"" begin=""&lt;!--"" end=""--&gt;"" />
        
        <!-- Strings -->
        <Span color=""String"" begin=""&quot;"" end=""&quot;"" />
        
        <!-- Attributes -->
        <Rule color=""Attribute"">
            [\w:]+(?=\s*=)
        </Rule>
        
        <!-- Elements -->
        <Rule color=""Element"">
            &lt;/?[\w:]+
        </Rule>
        
        <Rule color=""Element"">
            /?&gt;
        </Rule>
    </RuleSet>
</SyntaxDefinition>";

        using var reader = new XmlTextReader(new System.IO.StringReader(xshd));
        return HighlightingLoader.Load(reader, HighlightingManager.Instance);
    }
}