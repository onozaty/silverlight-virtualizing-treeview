using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightVirtualizingTreeView
{
    public class VirtualizingTreeViewItemData : INotifyPropertyChanged
    {
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                    NotifyIsExpandedChanged();
                }
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }

        public int Indent
        {
            get
            {
                if (_parent == null)
                {
                    return 0;
                }
                else
                {
                    return _parent.Indent + 1;
                }
            }
        }

        public Visibility ExpanderVisibility
        {
            get
            {
                if (_children == null || _children.Count == 0)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        private VirtualizingTreeViewItemData _parent;
        public VirtualizingTreeViewItemData Parent
        {
            get { return _parent; }
            internal set
            {
                if (value != _parent)
                {
                    _parent = value;
                    NotifyPropertyChanged("Parent");
                    NotifyPropertyChanged("Indent");
                }
            }
        }

        private ObservableCollection<VirtualizingTreeViewItemData> _children;
        public ObservableCollection<VirtualizingTreeViewItemData> Children
        {
            get { return _children; }
            set
            {
                if (value != _children)
                {
                    NotifyChildrenChanged(
                        new ChildrenChangedEventArgs(
                            ChildrenChangedAction.ChangeAll,
                            _children,
                            value));

                    if (_children != null)
                    {
                        _children.CollectionChanged -= Children_CollectionChanged;
                    }

                    _children = value;
                    if (_children != null)
                    {
                        _children.CollectionChanged += Children_CollectionChanged;
                    }

                    NotifyPropertyChanged("Children");
                    NotifyPropertyChanged("ExpanderVisibility");
                }
            }
        }

        void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    NotifyChildrenChanged(
                        new ChildrenChangedEventArgs(
                            ChildrenChangedAction.Add,
                            e.NewStartingIndex,
                            e.NewItems));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    NotifyChildrenChanged(
                        new ChildrenChangedEventArgs(
                            ChildrenChangedAction.Remove,
                            e.OldStartingIndex,
                            e.OldItems));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    NotifyChildrenChanged(
                        new ChildrenChangedEventArgs(
                            ChildrenChangedAction.Replace,
                            e.OldStartingIndex,
                            e.OldItems,
                            e.NewItems));
                    break;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event EventHandler<ChildrenChangedEventArgs> ChildrenChanged;

        private void NotifyChildrenChanged(ChildrenChangedEventArgs args)
        {
            if (ChildrenChanged != null)
            {
                ChildrenChanged(this, args);
            }
        }

        public event EventHandler IsExpandedChanged;

        private void NotifyIsExpandedChanged()
        {
            if (IsExpandedChanged != null)
            {
                IsExpandedChanged(this, EventArgs.Empty);
            }
        }

        public bool IsParentExpanded
        {
            get
            {
                return Parent == null
                    || (Parent.IsExpanded && Parent.IsParentExpanded);
            }
        }
    }

}
