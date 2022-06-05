using ExTools.Infrastructure;
using ExTools.SqlConsole.QueryExecutor;

using ICSharpCode.AvalonEdit.Highlighting;

using System;

namespace ExTools.SqlConsole.Models
{
    public sealed class ConsoleConfiguration
    {
        public string AccentColor { get; }
        public ObjectPool<QueryExecutorBase> QueryExecutorPool { get; }
        public IHighlightingDefinition SyntaxHighlighting { get; }

        public ConsoleConfiguration(string accentColor, Func<QueryExecutorBase> queryExecutorGenerator, IHighlightingDefinition syntaxHighlighting)
        {
            AccentColor = accentColor;
            QueryExecutorPool = new ObjectPool<QueryExecutorBase>(queryExecutorGenerator);
            SyntaxHighlighting = syntaxHighlighting;
        }
    }
}