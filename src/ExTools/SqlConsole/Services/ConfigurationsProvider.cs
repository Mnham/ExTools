#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Xml;
using ExTools.Connection.Models;
using ExTools.SqlConsole.Models;
using ExTools.SqlConsole.QueryExecutor;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace ExTools.SqlConsole.Services
{
    public sealed class ConfigurationsProvider
    {
        private readonly Dictionary<ConnectionType, ConsoleConfiguration> _configurations = [];

        private readonly (ConnectionType Type, string AccentColor)[] _settings =
        [
            (ConnectionType.Excel, "#227447"),
            (ConnectionType.Vertica, "#0078F0"),
            (ConnectionType.PostgreSQL, "#31648C"),
        ];

        public ConfigurationsProvider()
        {
            string assemblyName = GetType().Assembly.GetName().Name;

            foreach ((ConnectionType Type, string AccentColor) item in _settings)
            {
                string enumName = item.Type.ToString();
                string queryExecutorType = $"{assemblyName}.SqlConsole.QueryExecutor.{enumName}QueryExecutor";

                ObjectHandle obj = Activator.CreateInstance(assemblyName, queryExecutorType);
                var queryExecutor = (QueryExecutorBase)obj.Unwrap();

                string editorThemePath = $"{assemblyName}.SqlConsole.Highlighting.{enumName}DarkTheme.xshd";
                IHighlightingDefinition highlighting = LoadHighlighting(editorThemePath);

                _configurations[item.Type] = new ConsoleConfiguration(item.AccentColor, queryExecutor.CreateQueryExecutor, highlighting);
            }
        }

        public ConsoleConfiguration GetConfiguration(ConnectionType type) => _configurations[type];

        private IHighlightingDefinition LoadHighlighting(string path)
        {
            using Stream stream = GetType().Assembly.GetManifestResourceStream(path);
            using XmlTextReader reader = new(stream);

            return HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }
    }
}