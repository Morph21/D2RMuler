using System;
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

namespace D2RMuler.Views
{
    /// <summary>
    /// Interaction logic for KeyBindsWindow.xaml
    /// </summary>
    public partial class KeyBindsWindow : Window
    {
        public KeyBindsWindow()
        {
            InitializeComponent();
        }

        private void CloseWindowBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void CloseKeyBindsDialogBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
