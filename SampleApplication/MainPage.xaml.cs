using Silverlight.VirtualizingTreeView;
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

            Datas.Add(new VirtualizingTreeViewItemBasicData
            {
                Content = "Name1 (# of Children: 200)",
                Children = CreateChildren(200, "Name1-"),
                IsChecked = true
            });
            Datas.Add(new VirtualizingTreeViewItemBasicData
            {
                Content = "Name2 (# of Children: 1)",
                Children = new ObservableCollection<VirtualizingTreeViewItemData>() {
                    new VirtualizingTreeViewItemBasicData() {
                        Content = "Name2-1",
                        Children = new ObservableCollection<VirtualizingTreeViewItemData>() {
                            new VirtualizingTreeViewItemBasicData() {
                                Content = "Name2-1-1"
                            },
                            new VirtualizingTreeViewItemBasicData() {
                                Content = "Name2-1-2"
                            }
                        }
                    }
                }
            });
            Datas.Add(new VirtualizingTreeViewItemBasicData
            {
                Content = "Name3 (# of Children: 2000)",
                Children = CreateChildren(2000, "Name3-")
            });

            treeView.ItemsSource = Datas;
            vTreeView.ItemsSource = Datas;
            vTreeViewCheckBox.ItemsSource = Datas;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            VirtualizingTreeViewItemData selectedItemData = vTreeViewCheckBox.SelectedItem;
            if (selectedItemData != null)
            {
                if (selectedItemData.Children == null)
                {
                    selectedItemData.Children = new ObservableCollection<VirtualizingTreeViewItemData>();
                }

                selectedItemData.Children.Add(new VirtualizingTreeViewItemBasicData()
                {
                    Content = "Add+" + DateTime.Now.ToString()
                });
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            VirtualizingTreeViewItemData selectedItemData = vTreeViewCheckBox.SelectedItem;
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
                children.Add(new VirtualizingTreeViewItemBasicData()
                {
                    Content = prefix + i
                });
            }

            return children;
        }
    }

}
