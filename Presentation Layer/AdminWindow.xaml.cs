﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            // Select first menu by default
            if (MenuListBox.Items.Count > 0)
            {
                MenuListBox.SelectedIndex = 0;
            }
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuListBox.SelectedItem is ListBoxItem selectedItem)
            {
                string pagePath = selectedItem.Tag.ToString();
                if (pagePath == "Logout")
                {
                    var result = MessageBox.Show("Bạn có chắc muốn đăng xuất không?",
                                                 "Xác nhận đăng xuất",
                                                 MessageBoxButton.YesNo,
                                                 MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Show login window
                        LoginWindow loginWindow = new LoginWindow();
                        loginWindow.Show();
                        // Close current admin window
                        this.Close();
                    }

                    // Cancel logout if No is selected
                    selectedItem.IsSelected = false;
                    return;
                }
                try
                {                   
                    ContentFrame.Navigate(new Uri(pagePath, UriKind.Relative));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Trang này hiện không khả dụng", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

