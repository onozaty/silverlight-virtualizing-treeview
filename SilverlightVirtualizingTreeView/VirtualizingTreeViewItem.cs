using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Silverlight.VirtualizingTreeView
{
    [TemplatePart(Name = VirtualizingTreeViewItem.ExpanderButtonName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = VirtualizingTreeViewItem.ItemCheckBoxName, Type = typeof(CheckBox))]
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
            VirtualizingTreeViewItem treeViewItem = d as VirtualizingTreeViewItem;
            treeViewItem.ToggleExpanded();
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
                new PropertyMetadata(Visibility.Visible, null));
        #endregion

        #region CheckBoxType
        public enum CheckBoxTypes
        {
            None,
            Single,
            RefrectCheckChild
        }

        public CheckBoxTypes CheckBoxType
        {
            get { return (CheckBoxTypes)GetValue(CheckBoxTypeProperty); }
            set { SetValue(CheckBoxTypeProperty, value); }
        }

        public static readonly DependencyProperty CheckBoxTypeProperty =
            DependencyProperty.Register(
                "CheckBoxType",
                typeof(CheckBoxTypes),
                typeof(VirtualizingTreeViewItem),
                new PropertyMetadata(CheckBoxTypes.None, null));

        private static void OnCheckBoxTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingTreeViewItem treeViewItem = d as VirtualizingTreeViewItem;
            treeViewItem.ChangeCheckBoxVisibility();
        }
        #endregion

        #region IsChecked
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(
                "IsChecked",
                typeof(bool),
                typeof(VirtualizingTreeViewItem),
                new PropertyMetadata(false, OnIsCheckedPropertyChanged));

        private static void OnIsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingTreeViewItem treeViewItem = d as VirtualizingTreeViewItem;

            if (treeViewItem.ItemCheckBox != null)
            {
                treeViewItem.ItemCheckBox.IsChecked = (bool)e.NewValue;
            }
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
            VirtualizingTreeViewItem treeViewItem = d as VirtualizingTreeViewItem;
            treeViewItem.ApplyControlIndent();
        }
        #endregion

        private const string ExpanderButtonName = "ExpanderButton";
        private ToggleButton ExpanderButton { get; set; }

        private const string ItemCheckBoxName = "ItemCheckBox";
        private CheckBox ItemCheckBox { get; set; }

        public VirtualizingTreeViewItem()
            : base()
        {
            DefaultStyleKey = typeof(VirtualizingTreeViewItem);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get the parts
            ExpanderButton = GetTemplateChild(ExpanderButtonName) as ToggleButton;
            ExpanderButton.Click += ExpanderButton_Click;
            
            ItemCheckBox = GetTemplateChild(ItemCheckBoxName) as CheckBox;
            ItemCheckBox.Click += ItemCheckBox_Click;
            ItemCheckBox.IsChecked = IsChecked;

            ToggleExpanded();
            ChangeCheckBoxVisibility();
        }

        private void ExpanderButton_Click(object sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        private void ItemCheckBox_Click(object sender, RoutedEventArgs e)
        {
            IsChecked = !IsChecked;

            if (CheckBoxType == CheckBoxTypes.RefrectCheckChild)
            {
                VirtualizingTreeViewItemData itemData = this.DataContext as VirtualizingTreeViewItemData;
                itemData.CheckChildren(IsChecked);
            }
        }

        private void ToggleExpanded()
        {
            if (ExpanderButton != null)
            {
               ExpanderButton.IsChecked = IsExpanded;
            }
        }

        private void ChangeCheckBoxVisibility()
        {
            if (ItemCheckBox != null)
            {
                ItemCheckBox.Visibility =
                    (CheckBoxType == CheckBoxTypes.None)
                        ? Visibility.Collapsed
                        : Visibility.Visible;
            }
        }

        private void ApplyControlIndent()
        {
            Margin = new Thickness(15 * Indent, 0, 0, 0);
        }
    }
}
