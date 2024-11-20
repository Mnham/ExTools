#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExTools.Connection
{
    public sealed partial class ConnectionEditor : UserControl
    {
        public ConnectionEditor() => InitializeComponent();

        private IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
            {
                yield return (T)Enumerable.Empty<T>();
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null)
                {
                    continue;
                }

                if (ithChild is T t)
                {
                    yield return t;
                }

                foreach (T childOfChild in FindVisualChilds<T>(ithChild))
                {
                    yield return childOfChild;
                }
            }
        }

        private void SetPassword(object sender, RoutedEventArgs e)
        {
            DbConnectionData x = FindVisualChilds<DbConnectionData>(ConnectionTypes).FirstOrDefault();
            x?.SetPassword();
        }
    }
}