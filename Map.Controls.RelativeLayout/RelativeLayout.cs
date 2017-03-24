using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Map.Controls.Exceptions;
using Map.Controls.Extensions;
using Map.Controls.Utilities;

namespace Map.Controls
{
    public class RelativeLayout : Panel
    {
        private readonly DependencyGraph dependencyGraph;
        private FrameworkElement[] sortedVerticalChildren;
        private FrameworkElement[] sortedHorizontalChildren;

        public RelativeLayout()
        {
            dependencyGraph = new DependencyGraph();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (FrameworkElement child in Children)
            {
                double width = child.DesiredSize.Width;
                double height = child.DesiredSize.Height;

                if (child.HorizontalAlignment == HorizontalAlignment.Stretch)
                    width = availableSize.Width;

                if (child.VerticalAlignment == VerticalAlignment.Stretch)
                    height = availableSize.Height;

                child.Measure(new Size(width, height));
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in sortedVerticalChildren)
            {
                if (GetCenterInParent(child))
                {
                    double posX = (finalSize.Width - child.DesiredSize.Width) / 2;
                    double posY = (finalSize.Height - child.DesiredSize.Height) / 2;

                    posX -= child.Margin.Right - child.Margin.Left;
                    posY -= child.Margin.Bottom - child.Margin.Top;

                    child.Arrange(new Rect(new Point(posX, posY), child.DesiredSize));
                    continue;
                }

                double x = 0;
                double y = 0;
                double width = child.DesiredSize.Width;
                double height = child.DesiredSize.Height;

                string toLeftOfDependencyName = GetToLeftOf(child);
                string alignLeftDependencyName = GetAlignLeft(child);

                string toRightOfDependencyName = GetToRightOf(child);
                string alignRightDependencyName = GetAlignRight(child);

                if (GetCenterHorizontal(child))
                {
                    if (alignLeftDependencyName != null || toLeftOfDependencyName != null)
                        throw new InvalidLayoutException("Setting the properties AlignLeft and " +
                            "ToLeftOf of a horizontally centered control is not allowed.");
                    if (alignRightDependencyName != null || toRightOfDependencyName != null)
                        throw new InvalidLayoutException("Setting the properties AlignRight and " +
                            "ToRightOf of a horizontally centered control is not allowed.");

                    x = (finalSize.Width - width) / 2;
                }
                else
                {
                    if ((toLeftOfDependencyName != null && alignLeftDependencyName != null) && toLeftOfDependencyName == alignLeftDependencyName)
                        throw new InvalidLayoutException("The value of ToLeftOf and AlignLeft cannot be the same.");

                    bool isAlignedToLeft = false;
                    bool isAlignedToParentLeft = false;
                    bool isToLeftOfControl = false;

                    if (toLeftOfDependencyName != null)
                    {
                        if (dependencyGraph.KeyNodes.TryGetValue(toLeftOfDependencyName, out DependencyGraph.Node node))
                        {
                            var element = node.Element;
                            x = element.Position().X - element.Margin.Left - width;

                            isToLeftOfControl = true;
                        }
                        else
                            throw new ControlNotFoundException(toLeftOfDependencyName);
                    }
                    if (GetAlignParentLeft(child))
                    {
                        if (alignLeftDependencyName != null)
                            throw new InvalidLayoutException("Setting both AlignLeft and AlignParentLeft is not allowed.");

                        if (isToLeftOfControl)
                        {
                            width = x + width - this.Position().X;
                        }
                        x = this.Position().X;

                        isAlignedToParentLeft = true;
                    }
                    if (alignLeftDependencyName != null)
                    {
                        if (dependencyGraph.KeyNodes.TryGetValue(alignLeftDependencyName, out DependencyGraph.Node node))
                        {
                            var element = node.Element;

                            if (isToLeftOfControl)
                            {
                                width = x + width - element.Position().X;
                            }
                            x = element.Position().X;

                            isAlignedToLeft = true;
                        }
                        else
                            throw new ControlNotFoundException(alignLeftDependencyName);
                    }

                    if (toRightOfDependencyName != null && alignRightDependencyName != null && toRightOfDependencyName == alignRightDependencyName)
                        throw new InvalidLayoutException("The value of ToRightOf and AlignRight cannot be the same.");

                    bool isToRightOfControl = false;

                    if (toRightOfDependencyName != null)
                    {
                        if (dependencyGraph.KeyNodes.TryGetValue(toRightOfDependencyName, out DependencyGraph.Node node))
                        {
                            var element = node.Element;
                            x = element.Position().X + element.DesiredSize.Width + element.Margin.Right;

                            isToRightOfControl = true;
                        }
                        else
                            throw new ControlNotFoundException(toRightOfDependencyName);
                    }
                    if (GetAlignParentRight(child))
                    {
                        if (alignRightDependencyName != null)
                            throw new InvalidLayoutException("Setting both AlignRight and AlignParentRight is not allowed.");

                        if (isToRightOfControl)
                        {
                            width = this.Position().X + this.DesiredSize.Width - x;
                        }
                        else if (!isAlignedToLeft)
                        {
                            x = this.Position().X;
                        }
                        else
                        {
                            width = this.Position().X + this.DesiredSize.Width - x;
                        }
                    }
                    if (alignRightDependencyName != null)
                    {
                        if (dependencyGraph.KeyNodes.TryGetValue(alignRightDependencyName, out DependencyGraph.Node alignRightnode))
                        {
                            var alignRightElement = alignRightnode.Element;
                            double alignRightElementFullWidth = alignRightElement.DesiredSize.Width - alignRightElement.Margin.Right - alignRightElement.Margin.Left;

                            if (isAlignedToParentLeft)
                            {
                                width = (alignRightElement.Position().X + alignRightElementFullWidth) - x;
                            }
                            if (!isAlignedToLeft)
                            {
                                x = (alignRightElement.Position().X + alignRightElementFullWidth) - width;
                            }
                            else if (alignLeftDependencyName == alignRightDependencyName)
                            {
                                x = alignRightElement.Position().X;
                                width = alignRightElementFullWidth;
                            }
                            else
                            {
                                width = (alignRightElement.Position().X + alignRightElementFullWidth) - x;
                            }
                        }
                        else
                            throw new ControlNotFoundException(toRightOfDependencyName);
                    }
                }

                string aboveDependencyName = GetAbove(child);
                string alignTopDependencyName = GetAlignTop(child);

                string belowDependencyName = GetBelow(child);
                string alignBottomDependencyName = GetAlignBottom(child);

                if (GetCenterVertical(child))
                {
                    if (alignTopDependencyName != null || aboveDependencyName != null)
                        throw new InvalidLayoutException("Setting the properties AlignLeft and " +
                            "ToLeftOf of a horizontally centered control is not allowed.");
                    if (alignBottomDependencyName != null || belowDependencyName != null)
                        throw new InvalidLayoutException("Setting the properties AlignRight and " +
                            "ToRightOf of a horizontally centered control is not allowed.");

                    y = (finalSize.Height - height) / 2;
                }
                else
                {
                    if (aboveDependencyName != null && alignTopDependencyName != null && aboveDependencyName == alignTopDependencyName)
                        throw new InvalidLayoutException("The value of Above and AlignTop cannot be the same.");

                    bool isAlignedToTop = false;
                    bool isAlignedToParentTop = false;
                    bool isAboveControl = false;

                    if (aboveDependencyName != null)
                    {
                        if (dependencyGraph.KeyNodes.TryGetValue(aboveDependencyName, out DependencyGraph.Node node))
                        {
                            var element = node.Element;
                            y = element.Position().Y - element.Margin.Top - height;

                            isAboveControl = true;
                        }
                        else
                            throw new ControlNotFoundException(aboveDependencyName);
                    }
                    if (GetAlignParentTop(child))
                    {
                        if (alignTopDependencyName != null)
                            throw new InvalidLayoutException("Setting both AlignTop and AlignParentTop is not allowed.");

                        if (isAboveControl)
                        {
                            height = y + height - this.Position().Y;
                        }
                        y = this.Position().Y;

                        isAlignedToParentTop = true;
                    }
                    if (alignTopDependencyName != null)
                    {
                        if (dependencyGraph.KeyNodes.TryGetValue(alignTopDependencyName, out DependencyGraph.Node node))
                        {
                            var element = node.Element;
                            if (isAboveControl)
                            {
                                height = y + height - element.Position().Y;
                            }
                            y = element.Position().Y;

                            isAlignedToTop = true;
                        }
                        else
                            throw new ControlNotFoundException(alignTopDependencyName);
                    }

                    if (belowDependencyName != null && alignBottomDependencyName != null && belowDependencyName == alignBottomDependencyName)
                        throw new InvalidLayoutException("The value of Below and AlignBottom cannot be the same.");

                    bool isBelowControl = false;

                    if (belowDependencyName != null)
                    {
                        if (dependencyGraph.KeyNodes.TryGetValue(belowDependencyName, out DependencyGraph.Node node))
                        {
                            var element = node.Element;
                            y = element.Position().Y + element.DesiredSize.Height + element.Margin.Bottom;

                            isBelowControl = true;
                        }
                        else
                            throw new ControlNotFoundException(belowDependencyName);
                    }
                    if (GetAlignParentBottom(child))
                    {
                        if (alignBottomDependencyName != null)
                            throw new InvalidLayoutException("Setting both AlignBottom and AlignParentBottom is not allowed.");

                        if (isBelowControl || isAlignedToTop)
                        {
                            height = this.Position().Y + this.DesiredSize.Height - y;
                        }
                        else
                        {
                            y = this.Position().Y;
                        }
                    }
                    if (alignBottomDependencyName != null)
                    {
                        if (dependencyGraph.KeyNodes.TryGetValue(alignBottomDependencyName, out DependencyGraph.Node node))
                        {
                            var element = node.Element;
                            double elementFullHeight = element.DesiredSize.Height - element.Margin.Top - element.Margin.Bottom;

                            if (isAlignedToParentTop)
                            {
                                height = (element.Position().Y + elementFullHeight) - y;
                            }
                            else if (!isAlignedToTop)
                            {
                                y = (element.Position().Y + elementFullHeight) - height;
                            }
                            else if (alignLeftDependencyName == alignRightDependencyName)
                            {
                                y = element.Position().Y;
                                height = elementFullHeight;
                            }
                            else
                            {
                                height = (element.Position().Y + elementFullHeight) - y;
                            }
                        }
                        else
                            throw new ControlNotFoundException(belowDependencyName);
                    }
                }

                child.Arrange(new Rect(x, y,
                    width >= 0 ? width : child.DesiredSize.Width,
                    height >= 0 ? height : child.DesiredSize.Height));
            }

            return finalSize;
        }

        private void SortChildren()
        {
            int count = Children.Count;

            if (sortedVerticalChildren == null || sortedVerticalChildren.Length != count)
                sortedVerticalChildren = new FrameworkElement[count];

            if (sortedHorizontalChildren == null || sortedHorizontalChildren.Length != count)
                sortedHorizontalChildren = new FrameworkElement[count];

            dependencyGraph.Clear();

            foreach (FrameworkElement child in Children)
                dependencyGraph.Add(child);

            dependencyGraph.GetSortedViews(sortedVerticalChildren, IDS_VERTICAL);
            dependencyGraph.GetSortedViews(sortedHorizontalChildren, IDS_HORIZONTAL);
        }

        public static readonly DependencyProperty ToRightOfProperty = DependencyProperty.RegisterAttached
            ("ToRightOf", typeof(string), typeof(RelativeLayout), new PropertyMetadata(null, InvalidateLayoutCallback));
        public static readonly DependencyProperty ToLeftOfProperty = DependencyProperty.RegisterAttached
            ("ToLeftOf", typeof(string), typeof(RelativeLayout), new PropertyMetadata(null, InvalidateLayoutCallback));
        public static readonly DependencyProperty AboveProperty = DependencyProperty.RegisterAttached
            ("Above", typeof(string), typeof(RelativeLayout), new PropertyMetadata(null, InvalidateLayoutCallback));
        public static readonly DependencyProperty BelowProperty = DependencyProperty.RegisterAttached
            ("Below", typeof(string), typeof(RelativeLayout), new PropertyMetadata(null, InvalidateLayoutCallback));

        public static readonly DependencyProperty AlignLeftProperty = DependencyProperty.RegisterAttached
            ("AlignLeft", typeof(string), typeof(RelativeLayout), new PropertyMetadata(null, InvalidateLayoutCallback));
        public static readonly DependencyProperty AlignRightProperty = DependencyProperty.RegisterAttached
            ("AlignRight", typeof(string), typeof(RelativeLayout), new PropertyMetadata(null, InvalidateLayoutCallback));
        public static readonly DependencyProperty AlignTopProperty = DependencyProperty.RegisterAttached
            ("AlignTop", typeof(string), typeof(RelativeLayout), new PropertyMetadata(null, InvalidateLayoutCallback));
        public static readonly DependencyProperty AlignBottomProperty = DependencyProperty.RegisterAttached
            ("AlignBottom", typeof(string), typeof(RelativeLayout), new PropertyMetadata(null, InvalidateLayoutCallback));

        public static readonly DependencyProperty AlignParentLeftProperty = DependencyProperty.RegisterAttached
            ("AlignParentLeft", typeof(bool), typeof(RelativeLayout), new PropertyMetadata(false, InvalidateLayoutCallback));
        public static readonly DependencyProperty AlignParentRightProperty = DependencyProperty.RegisterAttached
            ("AlignParentRight", typeof(bool), typeof(RelativeLayout), new PropertyMetadata(false, InvalidateLayoutCallback));
        public static readonly DependencyProperty AlignParentTopProperty = DependencyProperty.RegisterAttached
            ("AlignParentTop", typeof(bool), typeof(RelativeLayout), new PropertyMetadata(false, InvalidateLayoutCallback));
        public static readonly DependencyProperty AlignParentBottomProperty = DependencyProperty.RegisterAttached
            ("AlignParentBottom", typeof(bool), typeof(RelativeLayout), new PropertyMetadata(false, InvalidateLayoutCallback));

        public static readonly DependencyProperty CenterInParentProperty = DependencyProperty.RegisterAttached
            ("CenterInParent", typeof(bool), typeof(RelativeLayout), new PropertyMetadata(false, InvalidateLayoutCallback));
        public static readonly DependencyProperty CenterVerticalProperty = DependencyProperty.RegisterAttached
            ("CenterVertical", typeof(bool), typeof(RelativeLayout), new PropertyMetadata(false, InvalidateLayoutCallback));
        public static readonly DependencyProperty CenterHorizontalProperty = DependencyProperty.RegisterAttached
            ("CenterHorizontal", typeof(bool), typeof(RelativeLayout), new PropertyMetadata(false, InvalidateLayoutCallback));

        public static string GetToLeftOf(DependencyObject obj)
        {
            return (string)obj.GetValue(ToLeftOfProperty);
        }

        public static void SetToLeftOf(DependencyObject obj, string value)
        {
            obj.SetValue(ToLeftOfProperty, value);
        }
        public static string GetToRightOf(DependencyObject obj)
        {
            return (string)obj.GetValue(ToRightOfProperty);
        }

        public static void SetToRightOf(DependencyObject obj, string value)
        {
            obj.SetValue(ToRightOfProperty, value);
        }

        public static string GetAbove(DependencyObject obj)
        {
            return (string)obj.GetValue(AboveProperty);
        }

        public static void SetAbove(DependencyObject obj, string value)
        {
            obj.SetValue(AboveProperty, value);
        }

        public static string GetBelow(DependencyObject obj)
        {
            return (string)obj.GetValue(BelowProperty);
        }

        public static void SetBelow(DependencyObject obj, string value)
        {
            obj.SetValue(BelowProperty, value);
        }
        public static string GetAlignLeft(DependencyObject obj)
        {
            return (string)obj.GetValue(AlignLeftProperty);
        }

        public static void SetAlignLeft(DependencyObject obj, string value)
        {
            obj.SetValue(AlignLeftProperty, value);
        }

        public static string GetAlignRight(DependencyObject obj)
        {
            return (string)obj.GetValue(AlignRightProperty);
        }

        public static void SetAlignRight(DependencyObject obj, string value)
        {
            obj.SetValue(AlignRightProperty, value);
        }

        public static string GetAlignTop(DependencyObject obj)
        {
            return (string)obj.GetValue(AlignTopProperty);
        }

        public static void SetAlignTop(DependencyObject obj, string value)
        {
            obj.SetValue(AlignTopProperty, value);
        }

        public static string GetAlignBottom(DependencyObject obj)
        {
            return (string)obj.GetValue(AlignBottomProperty);
        }

        public static void SetAlignBottom(DependencyObject obj, string value)
        {
            obj.SetValue(AlignBottomProperty, value);
        }

        public static bool GetAlignParentLeft(DependencyObject obj)
        {
            return (bool)obj.GetValue(AlignParentLeftProperty);
        }

        public static void SetAlignParentLeft(DependencyObject obj, bool value)
        {
            obj.SetValue(AlignParentLeftProperty, value);
        }

        public static bool GetAlignParentRight(DependencyObject obj)
        {
            return (bool)obj.GetValue(AlignParentRightProperty);
        }

        public static void SetAlignParentRight(DependencyObject obj, bool value)
        {
            obj.SetValue(AlignParentRightProperty, value);
        }

        public static bool GetAlignParentTop(DependencyObject obj)
        {
            return (bool)obj.GetValue(AlignParentTopProperty);
        }

        public static void SetAlignParentTop(DependencyObject obj, bool value)
        {
            obj.SetValue(AlignParentTopProperty, value);
        }

        public static bool GetAlignParentBottom(DependencyObject obj)
        {
            return (bool)obj.GetValue(AlignParentBottomProperty);
        }

        public static void SetAlignParentBottom(DependencyObject obj, bool value)
        {
            obj.SetValue(AlignParentBottomProperty, value);
        }

        public static bool GetCenterInParent(DependencyObject obj)
        {
            return (bool)obj.GetValue(CenterInParentProperty);
        }

        public static void SetCenterInParent(DependencyObject obj, bool value)
        {
            obj.SetValue(CenterInParentProperty, value);
        }

        public static bool GetCenterHorizontal(DependencyObject obj)
        {
            return (bool)obj.GetValue(CenterHorizontalProperty);
        }

        public static void SetCenterHorizontal(DependencyObject obj, bool value)
        {
            obj.SetValue(CenterHorizontalProperty, value);
        }

        public static bool GetCenterVertical(DependencyObject obj)
        {
            return (bool)obj.GetValue(CenterVerticalProperty);
        }

        public static void SetCenterVertical(DependencyObject obj, bool value)
        {
            obj.SetValue(CenterVerticalProperty, value);
        }
        
        private static readonly int[] IDS_HORIZONTAL =
        {
            ToRightOfProperty.GlobalIndex, ToLeftOfProperty.GlobalIndex, AlignLeftProperty.GlobalIndex, AlignRightProperty.GlobalIndex
        };

        private static readonly int[] IDS_VERTICAL =
        {
            AboveProperty.GlobalIndex, BelowProperty.GlobalIndex, AlignTopProperty.GlobalIndex, AlignBottomProperty.GlobalIndex
        };

        private static void InvalidateLayoutCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement child)
            {
                if (VisualTreeHelper.GetParent(child) is RelativeLayout panel)
                {
                    panel.SortChildren();
                    panel.InvalidateMeasure();
                    panel.InvalidateArrange();
                }
            }
        }
    }
}
