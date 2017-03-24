using System;
using System.Windows;

namespace Map.Controls.Extensions
{
    internal static partial class Extensions
    {
        internal static string GetDependencyName(this DependencyObject element, int propertyId)
        {
            string dependencyName;

            if (propertyId == RelativeLayout.ToLeftOfProperty.GlobalIndex)
                dependencyName = RelativeLayout.GetToLeftOf(element);
            else if (propertyId == RelativeLayout.ToRightOfProperty.GlobalIndex)
                dependencyName = RelativeLayout.GetToRightOf(element);
            else if (propertyId == RelativeLayout.AboveProperty.GlobalIndex)
                dependencyName = RelativeLayout.GetAbove(element);
            else if (propertyId == RelativeLayout.BelowProperty.GlobalIndex)
                dependencyName = RelativeLayout.GetBelow(element);
            else if (propertyId == RelativeLayout.AlignLeftProperty.GlobalIndex)
                dependencyName = RelativeLayout.GetAlignLeft(element);
            else if (propertyId == RelativeLayout.AlignRightProperty.GlobalIndex)
                dependencyName = RelativeLayout.GetAlignRight(element);
            else if (propertyId == RelativeLayout.AlignTopProperty.GlobalIndex)
                dependencyName = RelativeLayout.GetAlignTop(element);
            else if (propertyId == RelativeLayout.AlignBottomProperty.GlobalIndex)
                dependencyName = RelativeLayout.GetAlignBottom(element);
            else
                throw new ArgumentException("Invalid property ID");

            return dependencyName;
        }
    }
}
