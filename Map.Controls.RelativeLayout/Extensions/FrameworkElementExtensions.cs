using System.Windows;

namespace Map.Controls.Extensions
{
    internal static partial class Extensions
    {
        internal static Point Position(this FrameworkElement element)
        {
            return element.TranslatePoint(new Point(0, 0), null);
        }
    }
}
