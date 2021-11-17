using D2RMuler.Db;
using D2RMuler.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace D2RMuler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<CharacterStash> CharacterStashes = new List<CharacterStash>();
        List<CharacterStash> MockCharacterStashes = new List<CharacterStash>() { new CharacterStash("test")};

        public MainWindow()
        {
            DB.Instance().InitializeDb();
            InitializeComponent();
            ReloadCharactersList();
        }

        private void ScreenShot_Click(object sender, RoutedEventArgs e)
        {
            CharacterStash? stash = CharactersListView.SelectedItem as CharacterStash;
            if (stash != null)
            {
                string screen = Utils.ScreenShot.CaptureApplication("D2R");
                if (!string.IsNullOrEmpty(screen))
                {
                    stash.ImageSrc = screen;
                    stash.Save();

                    StashImage.Source = new BitmapImage(new Uri(screen));

                } else
                {
                    //TODO show alert
                }
            } else
            {
                //TODO show alert
            }
            
        }

        private void NewCharacterSubmit_Click(object sender, RoutedEventArgs e)
        {
            CharacterStash character = new CharacterStash(NewCharacterInput.Text);
            character.Save();
            NewCharacterInput.Text = "";
            ReloadCharactersList();
        }

        private void ReloadCharactersList()
        {
            CharacterStashes = CharacterStash.FindAll();
            CharactersListView.ItemsSource = CharacterStashes;
        }

        private void CharactersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CharacterStash? item = e.AddedItems[0] as CharacterStash;

            if (item != null)
            {
                if (!string.IsNullOrEmpty(item.ImageSrc))
                {
                    StashImage.Source = new BitmapImage(new Uri(item.ImageSrc));

                } else
                {
                    //TODO: Show alert with info about lacking image
                }
            }
            
        }
    }
}
