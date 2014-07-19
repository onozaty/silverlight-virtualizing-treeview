using SilverlightVirtualizingTreeView;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SampleApplication
{
    public partial class MainPage : UserControl
    {
        private ObservableCollection<VirtualizingTreeViewItemData> Datas { get; set; }
        private VirtualizingTreeViewItemData ItemData { get; set; }

        public MainPage()
        {
            InitializeComponent();

            addButton.Click += AddButton_Click;
            removeButton.Click += RemoveButton_Click;

            Datas = new ObservableCollection<VirtualizingTreeViewItemData>();

            Datas.Add(new VirtualizingTreeViewItemData
            {
                Text = "Name1",
                Children = CreateChildren(100, "Name1-")
            });
            Datas.Add(new VirtualizingTreeViewItemData
            {
                Text = "Name2",
                Children = new ObservableCollection<VirtualizingTreeViewItemData>() {
                    new VirtualizingTreeViewItemData() {
                        Text = "Name2-1"
                    }
                }
            });
            Datas.Add(new VirtualizingTreeViewItemData
            {
                Text = "Name3",
                Children = CreateChildren(2000, "Name3-")
            });

            treeView.ItemsSource = Datas;
            vTreeView.ItemsSource = Datas;
        }

        void AddButton_Click(object sender, RoutedEventArgs e)
        {
            VirtualizingTreeViewItemData selectedItemData = vTreeView.SelectedItemData;
            if (selectedItemData != null)
            {
                if (selectedItemData.Children == null)
                {
                    selectedItemData.Children = new ObservableCollection<VirtualizingTreeViewItemData>();
                }

                selectedItemData.Children.Add(new VirtualizingTreeViewItemData()
                {
                    Text = "Add+" + DateTime.Now.ToString()
                });
            }
        }

        void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            VirtualizingTreeViewItemData selectedItemData = vTreeView.SelectedItemData;
            if (selectedItemData != null)
            {
                if (selectedItemData.Parent == null)
                {
                    Datas.Remove(selectedItemData);
                }
                else
                {
                    selectedItemData.Parent.Children.Remove(selectedItemData);
                }
            }
        }

        private ObservableCollection<VirtualizingTreeViewItemData> CreateChildren(int count, string prefix)
        {
            ObservableCollection<VirtualizingTreeViewItemData> children = 
                new ObservableCollection<VirtualizingTreeViewItemData>();

            for (int i = 0; i < count; i++)
            {
                children.Add(new VirtualizingTreeViewItemData()
                {
                    Text = prefix + i
                });
            }

            return children;
        }
    }

}
