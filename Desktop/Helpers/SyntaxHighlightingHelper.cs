using System.Xml;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;

namespace Desktop.Helpers;

public static class SyntaxHighlightingHelper
{
    private const string VariablePattern = @"\{\$[^{}]+\}|\{![^{}]+\}";
    private const string VariableColor = "#FFD700"; // Gold/Yellow for variables

    public static IHighlightingDefinition GetJsonHighlighting()
    {
        var xshd = $@"<?xml version=""1.0""?>
<SyntaxDefinition name=""JSON"" xmlns=""http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008"">
    <Color name=""String"" foreground=""#CE9178"" />
    <Color name=""Number"" foreground=""#B5CEA8"" />
    <Color name=""Boolean"" foreground=""#569CD6"" />
    <Color name=""Null"" foreground=""#569CD6"" />
    <Color name=""PropertyName"" foreground=""#9CDCFE"" fontWeight=""bold"" />
    <Color name=""Punctuation"" foreground=""#D4D4D4"" />
    <Color name=""Variable"" foreground=""{VariableColor}"" fontWeight=""bold"" />
    
    <RuleSet>
        <!-- Variables -->
        <Rule color=""Variable"">
            {VariablePattern}
        </Rule>

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
            [{{}}[\]:,]
        </Rule>
    </RuleSet>
</SyntaxDefinition>";

        using var reader = new XmlTextReader(new System.IO.StringReader(xshd));
        return HighlightingLoader.Load(reader, HighlightingManager.Instance);
    }
    
    public static IHighlightingDefinition GetXmlHighlighting()
    {
        var xshd = $@"<?xml version=""1.0""?>
<SyntaxDefinition name=""XML"" xmlns=""http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008"">
    <Color name=""Element"" foreground=""#569CD6"" />
    <Color name=""Attribute"" foreground=""#9CDCFE"" />
    <Color name=""String"" foreground=""#CE9178"" />
    <Color name=""Comment"" foreground=""#6A9955"" />
    <Color name=""Variable"" foreground=""{VariableColor}"" fontWeight=""bold"" />
    
    <RuleSet>
        <!-- Variables -->
        <Rule color=""Variable"">
            {VariablePattern}
        </Rule>

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

    public static IHighlightingDefinition GetCSharpHighlighting()
    {
        var xshd = $@"<?xml version=""1.0""?>
<SyntaxDefinition name=""C#"" xmlns=""http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008"">
    <Color name=""Comment"" foreground=""#6A9955"" />
    <Color name=""String"" foreground=""#CE9178"" />
    <Color name=""Keyword"" foreground=""#569CD6"" />
    <Color name=""Variable"" foreground=""{VariableColor}"" fontWeight=""bold"" />

    <RuleSet>
        <Span color=""Comment"" begin=""//"" />
        <Span color=""Comment"" multiline=""true"" begin=""/\*"" end=""\*/"" />
        
        <Span color=""String"">
            <Begin>""</Begin>
            <End>""</End>
        </Span>

        <!-- Variables -->
        <Rule color=""Variable"">
            {VariablePattern}
        </Rule>

        <Keywords color=""Keyword"">
            <Word>abstract</Word>
            <Word>as</Word>
            <Word>async</Word>
            <Word>await</Word>
            <Word>base</Word>
            <Word>bool</Word>
            <Word>break</Word>
            <Word>byte</Word>
            <Word>case</Word>
            <Word>catch</Word>
            <Word>char</Word>
            <Word>checked</Word>
            <Word>class</Word>
            <Word>const</Word>
            <Word>continue</Word>
            <Word>decimal</Word>
            <Word>default</Word>
            <Word>delegate</Word>
            <Word>do</Word>
            <Word>double</Word>
            <Word>else</Word>
            <Word>enum</Word>
            <Word>event</Word>
            <Word>explicit</Word>
            <Word>extern</Word>
            <Word>false</Word>
            <Word>finally</Word>
            <Word>fixed</Word>
            <Word>float</Word>
            <Word>for</Word>
            <Word>foreach</Word>
            <Word>goto</Word>
            <Word>if</Word>
            <Word>implicit</Word>
            <Word>in</Word>
            <Word>int</Word>
            <Word>interface</Word>
            <Word>internal</Word>
            <Word>is</Word>
            <Word>lock</Word>
            <Word>long</Word>
            <Word>namespace</Word>
            <Word>new</Word>
            <Word>null</Word>
            <Word>object</Word>
            <Word>operator</Word>
            <Word>out</Word>
            <Word>override</Word>
            <Word>params</Word>
            <Word>private</Word>
            <Word>protected</Word>
            <Word>public</Word>
            <Word>readonly</Word>
            <Word>ref</Word>
            <Word>return</Word>
            <Word>sbyte</Word>
            <Word>sealed</Word>
            <Word>short</Word>
            <Word>sizeof</Word>
            <Word>stackalloc</Word>
            <Word>static</Word>
            <Word>string</Word>
            <Word>struct</Word>
            <Word>switch</Word>
            <Word>this</Word>
            <Word>throw</Word>
            <Word>true</Word>
            <Word>try</Word>
            <Word>typeof</Word>
            <Word>uint</Word>
            <Word>ulong</Word>
            <Word>unchecked</Word>
            <Word>unsafe</Word>
            <Word>ushort</Word>
            <Word>using</Word>
            <Word>virtual</Word>
            <Word>void</Word>
            <Word>volatile</Word>
            <Word>while</Word>
            <Word>var</Word>
        </Keywords>
    </RuleSet>
</SyntaxDefinition>";
        
        using var reader = new XmlTextReader(new System.IO.StringReader(xshd));
        return HighlightingLoader.Load(reader, HighlightingManager.Instance);
    }

    public static IHighlightingDefinition GetPythonHighlighting()
    {
        var xshd = $@"<?xml version=""1.0""?>
<SyntaxDefinition name=""Python"" xmlns=""http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008"">
    <Color name=""Comment"" foreground=""#6A9955"" />
    <Color name=""String"" foreground=""#CE9178"" />
    <Color name=""Keyword"" foreground=""#569CD6"" />
    <Color name=""Variable"" foreground=""{VariableColor}"" fontWeight=""bold"" />

    <RuleSet>
        <Span color=""Comment"" begin=""#"" />
        
        <Span color=""String"">
            <Begin>""</Begin>
            <End>""</End>
        </Span>
        <Span color=""String"">
            <Begin>'</Begin>
            <End>'</End>
        </Span>

        <!-- Variables -->
        <Rule color=""Variable"">
            {VariablePattern}
        </Rule>

        <Keywords color=""Keyword"">
            <Word>False</Word>
            <Word>None</Word>
            <Word>True</Word>
            <Word>and</Word>
            <Word>as</Word>
            <Word>assert</Word>
            <Word>async</Word>
            <Word>await</Word>
            <Word>break</Word>
            <Word>class</Word>
            <Word>continue</Word>
            <Word>def</Word>
            <Word>del</Word>
            <Word>elif</Word>
            <Word>else</Word>
            <Word>except</Word>
            <Word>finally</Word>
            <Word>for</Word>
            <Word>from</Word>
            <Word>global</Word>
            <Word>if</Word>
            <Word>import</Word>
            <Word>in</Word>
            <Word>is</Word>
            <Word>lambda</Word>
            <Word>nonlocal</Word>
            <Word>not</Word>
            <Word>or</Word>
            <Word>pass</Word>
            <Word>raise</Word>
            <Word>return</Word>
            <Word>try</Word>
            <Word>while</Word>
            <Word>with</Word>
            <Word>yield</Word>
        </Keywords>
    </RuleSet>
</SyntaxDefinition>";
        
        using var reader = new XmlTextReader(new System.IO.StringReader(xshd));
        return HighlightingLoader.Load(reader, HighlightingManager.Instance);
    }

    public static IHighlightingDefinition GetVariableOnlyHighlighting()
    {
        var xshd = $@"<?xml version=""1.0""?>
<SyntaxDefinition name=""Variables"" xmlns=""http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008"">
    <Color name=""Variable"" foreground=""{VariableColor}"" fontWeight=""bold"" />
    <RuleSet>
        <Rule color=""Variable"">
            {VariablePattern}
        </Rule>
    </RuleSet>
</SyntaxDefinition>";
        
        using var reader = new XmlTextReader(new System.IO.StringReader(xshd));
        return HighlightingLoader.Load(reader, HighlightingManager.Instance);
    }
}
