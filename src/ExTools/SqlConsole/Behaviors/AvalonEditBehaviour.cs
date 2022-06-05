using ICSharpCode.AvalonEdit;

using Microsoft.Xaml.Behaviors;

using System;
using System.Windows;

namespace ExTools.SqlConsole.Behaviors
{
    public sealed class AvalonEditBehaviour : Behavior<TextEditor>
    {
        public static readonly DependencyProperty ScriptProperty = DependencyProperty.Register(
            nameof(Script),
            typeof(string),
            typeof(AvalonEditBehaviour),
            new FrameworkPropertyMetadata(string.Empty,
                                          FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                          new PropertyChangedCallback(ScriptChangedHandler)));

        public static readonly DependencyProperty SelectedScriptProperty = DependencyProperty.Register(
            nameof(SelectedScript),
            typeof(string),
            typeof(AvalonEditBehaviour),
            new PropertyMetadata(string.Empty));

        public string Script
        {
            get => (string)GetValue(ScriptProperty);
            set => SetValue(ScriptProperty, value);
        }

        public string SelectedScript
        {
            get => (string)GetValue(SelectedScriptProperty);
            set => SetValue(SelectedScriptProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Document.Text = Script;
            AssociatedObject.TextChanged += TextChangedHandler;
            AssociatedObject.TextArea.SelectionChanged += SelectionChangedHandler;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= TextChangedHandler;
            AssociatedObject.TextArea.SelectionChanged -= SelectionChangedHandler;
        }

        private static void ScriptChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AvalonEditBehaviour behaviour = sender as AvalonEditBehaviour;
            string text = e.NewValue as string;
            if (behaviour.AssociatedObject.Document.Text != text)
            {
                behaviour.AssociatedObject.Document.Text = text;
            }
        }

        private void SelectionChangedHandler(object sender, EventArgs e) =>
            SelectedScript = AssociatedObject.TextArea.Selection.GetText();

        private void TextChangedHandler(object sender, EventArgs eventArgs) =>
            Script = AssociatedObject.Document.Text;
    }
}