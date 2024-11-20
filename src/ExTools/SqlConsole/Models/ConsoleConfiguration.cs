#nullable enable

using System;
using ExTools.Infrastructure;
using ExTools.SqlConsole.QueryExecutor;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ExTools.SqlConsole.Models
{
    public sealed class ConsoleConfiguration(string accentColor, Func<QueryExecutorBase> queryExecutorGenerator, IHighlightingDefinition syntaxHighlighting)
    {
        public string AccentColor { get; } = accentColor;
        public ObjectPool<QueryExecutorBase> QueryExecutorPool { get; } = new(queryExecutorGenerator);
        public IHighlightingDefinition SyntaxHighlighting { get; } = syntaxHighlighting;
    }
}