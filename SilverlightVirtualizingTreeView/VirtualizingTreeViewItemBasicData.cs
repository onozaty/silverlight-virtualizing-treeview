
namespace Silverlight.VirtualizingTreeView
{
    public class VirtualizingTreeViewItemBasicData : VirtualizingTreeViewItemData
    {
        private object _content;
        public object Content
        {
            get { return _content; }
            set
            {
                if (value != _content)
                {
                    _content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }
    }
}
