using System;
using System.Collections;

namespace SilverlightVirtualizingTreeView
{
    public class ChildrenChangedEventArgs : EventArgs
    {
        public ChildrenChangedAction Action { get; private set; }

        public IList NewChildren { get; private set; }

        public IList OldChildren { get; private set; }

        public int NewStartingIndex { get; private set; }

        public int OldStartingIndex { get; private set; }

        public ChildrenChangedEventArgs(ChildrenChangedAction action, int index, IList children)
        {
            Action = action;

            switch (action)
            {
                case ChildrenChangedAction.Add:

                    OldStartingIndex = -1;
                    NewStartingIndex = index;

                    NewChildren = children;
                    break;

                case ChildrenChangedAction.Remove:

                    OldStartingIndex = index;
                    NewStartingIndex = -1;

                    OldChildren = children;
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public ChildrenChangedEventArgs(
            ChildrenChangedAction action, IList oldChildren, IList newChildren)
        {
            if (action != ChildrenChangedAction.ChangeAll)
            {
                throw new NotSupportedException();
            }

            Action = action;

            OldChildren = oldChildren;
            NewChildren = newChildren;
        }

        public ChildrenChangedEventArgs(
            ChildrenChangedAction action, int index, IList oldChildren, IList newChildren)
        {
            if (action != ChildrenChangedAction.Replace)
            {
                throw new NotSupportedException();
            }

            Action = action;

            OldStartingIndex = index;
            NewStartingIndex = index;

            OldChildren = oldChildren;
            NewChildren = newChildren;
        }
    }

    public enum ChildrenChangedAction
    {
        Add,
        Remove,
        Replace,
        ChangeAll
    }
}
