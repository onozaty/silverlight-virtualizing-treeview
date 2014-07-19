using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SilverlightVirtualizingTreeView
{
    public partial class VirtualizingTreeView : UserControl
    {
        #region ItemsSource
        public IEnumerable<VirtualizingTreeViewItemData> ItemsSource
        {
            get { return (IEnumerable<VirtualizingTreeViewItemData>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable<VirtualizingTreeViewItemData>),
                typeof(VirtualizingTreeView),
                new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        private static void OnItemsSourcePropertyChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingTreeView source = d as VirtualizingTreeView;

            source.OnItemsSourceChanged(
                (IEnumerable<VirtualizingTreeViewItemData>)e.OldValue,
                (IEnumerable<VirtualizingTreeViewItemData>)e.NewValue);
        }
        #endregion

        private ObservableCollection<VirtualizingTreeViewItemData> _flatItems;

        protected virtual void OnItemsSourceChanged(
            IEnumerable<VirtualizingTreeViewItemData> oldValue, IEnumerable<VirtualizingTreeViewItemData> newValue)
        {
            _flatItems = new ObservableCollection<VirtualizingTreeViewItemData>();

            foreach (VirtualizingTreeViewItemData item in FlattenItems(null, newValue))
            {
                item.ChildrenChanged -= Item_ChildrenChanged;
                item.ChildrenChanged += Item_ChildrenChanged;

                item.IsExpandedChanged -= Item_IsExpandedChanged;
                item.IsExpandedChanged += Item_IsExpandedChanged;

                _flatItems.Add(item);
            }

            INotifyCollectionChanged oldCollection = oldValue as INotifyCollectionChanged;
            INotifyCollectionChanged newCollection = newValue as INotifyCollectionChanged;

            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= ItemsSource_CollectionChanged;
            }

            if (newCollection != null)
            {
                newCollection.CollectionChanged -= ItemsSource_CollectionChanged;
                newCollection.CollectionChanged += ItemsSource_CollectionChanged;
            }

            PagedCollectionView collectionView = new PagedCollectionView(_flatItems);
            collectionView.Filter = FilterItems;
            InnerListBox.ItemsSource = collectionView;
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    AddFlatItems(
                        _flatItems.IndexOf(ItemsSource.ElementAt(e.NewStartingIndex)),
                        FlattenItems(null, e.NewItems.Cast<VirtualizingTreeViewItemData>()));

                    break;

                case NotifyCollectionChangedAction.Remove:

                    RemoveItems(
                        FlattenItems(null, e.OldItems.Cast<VirtualizingTreeViewItemData>()));

                    break;

                case NotifyCollectionChangedAction.Replace:

                    RemoveItems(
                        FlattenItems(null, e.OldItems.Cast<VirtualizingTreeViewItemData>()));

                    AddFlatItems(
                        _flatItems.IndexOf(ItemsSource.ElementAt(e.NewStartingIndex)),
                        FlattenItems(null, e.NewItems.Cast<VirtualizingTreeViewItemData>()));

                    break;
            }
        }

        public VirtualizingTreeViewItemData SelectedItemData
        {
            get
            {
                return (VirtualizingTreeViewItemData)InnerListBox.SelectedValue;
            }
        }

        public VirtualizingTreeView()
        {
            InitializeComponent();
        }

        private IEnumerable<VirtualizingTreeViewItemData> FlattenItems(
            VirtualizingTreeViewItemData parent, IEnumerable<VirtualizingTreeViewItemData> children)
        {
            if (children == null)
            {
                yield break;
            }

            foreach (var child in children)
            {
                child.Parent = parent;

                yield return child;

                foreach (VirtualizingTreeViewItemData grandchild in FlattenItems(child, child.Children))
                {
                    yield return grandchild;
                }
            }
        }

        private bool FilterItems(object obj)
        {
            return ((VirtualizingTreeViewItemData)obj).IsParentExpanded;
        }

        private void Item_ChildrenChanged(object sender, ChildrenChangedEventArgs e)
        {
            VirtualizingTreeViewItemData parent = (VirtualizingTreeViewItemData)sender;

            switch (e.Action)
            {
                case ChildrenChangedAction.Add:

                    AddFlatItems(
                        GetFlatItemIndex(parent, e.NewStartingIndex),
                        FlattenItems(parent, e.NewChildren.Cast<VirtualizingTreeViewItemData>()));

                    break;

                case ChildrenChangedAction.ChangeAll:

                    if (e.OldChildren != null)
                    {
                        RemoveItems(
                            FlattenItems(parent, e.OldChildren.Cast<VirtualizingTreeViewItemData>()));
                    }

                    if (e.NewChildren != null)
                    {
                        AddFlatItems(
                            GetFlatItemIndex(parent, 0),
                            FlattenItems(parent, e.NewChildren.Cast<VirtualizingTreeViewItemData>()));
                    }

                    break;

                case ChildrenChangedAction.Remove:

                    RemoveItems(
                        FlattenItems(parent, e.OldChildren.Cast<VirtualizingTreeViewItemData>()));

                    break;

                case ChildrenChangedAction.Replace:

                    RemoveItems(
                        FlattenItems(parent, e.OldChildren.Cast<VirtualizingTreeViewItemData>()));

                    AddFlatItems(
                        GetFlatItemIndex(parent, e.NewStartingIndex),
                        FlattenItems(parent, e.NewChildren.Cast<VirtualizingTreeViewItemData>()));

                    break;
            }

            ((PagedCollectionView)InnerListBox.ItemsSource).Refresh();
        }

        private int GetFlatItemIndex(VirtualizingTreeViewItemData parent, int startIndex)
        {
            if (startIndex == 0)
            {
                return _flatItems.IndexOf(parent) + 1;
            }

            return _flatItems.IndexOf(GetLastChild(parent.Children[startIndex - 1])) + 1;
        }

        private VirtualizingTreeViewItemData GetLastChild(VirtualizingTreeViewItemData child)
        {
            if (child.Children == null || child.Children.Count == 0)
            {
                return child;
            }

            return GetLastChild(child.Children.Last());
        }

        private void AddFlatItems(int startIndex, IEnumerable<VirtualizingTreeViewItemData> items)
        {
            foreach (VirtualizingTreeViewItemData item in items)
            {
                item.ChildrenChanged -= Item_ChildrenChanged;
                item.ChildrenChanged += Item_ChildrenChanged;

                item.IsExpandedChanged -= Item_IsExpandedChanged;
                item.IsExpandedChanged += Item_IsExpandedChanged;

                _flatItems.Insert(startIndex++, item);
            }
        }

        private void RemoveItems(IEnumerable<VirtualizingTreeViewItemData> items)
        {
            foreach (VirtualizingTreeViewItemData item in items)
            {
                item.ChildrenChanged -= Item_ChildrenChanged;
                item.IsExpandedChanged -= Item_IsExpandedChanged;

                _flatItems.Remove(item);
            }
        }

        private void Item_IsExpandedChanged(object sender, EventArgs e)
        {
            ((PagedCollectionView)InnerListBox.ItemsSource).Refresh();
        }

    }
}
