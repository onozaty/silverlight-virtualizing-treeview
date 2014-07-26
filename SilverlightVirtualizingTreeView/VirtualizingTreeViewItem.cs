using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SilverlightVirtualizingTreeView
{
    [TemplatePart(Name = VirtualizingTreeViewItem.ExpanderButtonName, Type = typeof(ToggleButton))]
    public class VirtualizingTreeViewItem : ContentControl
    {
        #region IsExpanded
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                "IsExpanded",
                typeof(bool),
                typeof(VirtualizingTreeViewItem),
                new PropertyMetadata(false, OnIsExpandedPropertyChanged));

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingTreeViewItem source = d as VirtualizingTreeViewItem;
            bool isExpanded = (bool)e.NewValue;

            if (isExpanded)
            {
                source.OnExpanded();
            }
            else
            {
                source.OnCollapsed();
            }
        }
        #endregion

        #region ExpanderVisibility
        public Visibility ExpanderVisibility
        {
            get { return (Visibility)GetValue(ExpanderVisibilityProperty); }
            set { SetValue(ExpanderVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ExpanderVisibilityProperty =
            DependencyProperty.Register(
                "ExpanderVisibility",
                typeof(Visibility),
                typeof(VirtualizingTreeViewItem),
                new PropertyMetadata(Visibility.Visible, OnExpanderVisibilityPropertyChanged));

        private static void OnExpanderVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingTreeViewItem source = d as VirtualizingTreeViewItem;

            source.OnExpanderVisibilityChanged((Visibility)e.NewValue);
        }
        #endregion

        #region Indent
        public int Indent
        {
            get { return (int)GetValue(IndentProperty); }
            set { SetValue(IndentProperty, value); }
        }

        public static readonly DependencyProperty IndentProperty =
            DependencyProperty.Register(
                "Indent",
                typeof(int),
                typeof(VirtualizingTreeViewItem),
                new PropertyMetadata(0, OnIndentPropertyChanged));

        private static void OnIndentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingTreeViewItem source = d as VirtualizingTreeViewItem;

            source.OnIndentChanged((int)e.OldValue, (int)e.NewValue);
        }
        #endregion

        private const string ExpanderButtonName = "ExpanderButton";
        private ToggleButton ExpanderButton { get; set; }

        public VirtualizingTreeViewItem()
            : base()
        {
            DefaultStyleKey = typeof(VirtualizingTreeViewItem);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get the parts
            ExpanderButton = GetTemplateChild("ExpanderButton") as ToggleButton;
            ExpanderButton.Click += ExpanderButton_Click;
            ExpanderButton.IsChecked = IsExpanded;
            ExpanderButton.Visibility = ExpanderVisibility;
        }

        protected virtual void OnExpanded()
        {
            ToggleExpanded();
        }

        protected virtual void OnCollapsed()
        {
            ToggleExpanded();
        }

        private void ToggleExpanded()
        {
            if (ExpanderButton != null)
            {
                ExpanderButton.IsChecked = IsExpanded;
            }
        }

        private void ExpanderButton_Click(object sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        protected virtual void OnIndentChanged(int oldValue, int newValue)
        {
            Margin = new Thickness(15 * newValue, 0, 0, 0);
        }

        protected virtual void OnExpanderVisibilityChanged(Visibility visibility)
        {
            if (ExpanderButton != null)
            {
                ExpanderButton.Visibility = visibility;
            }
        }
    }
}
