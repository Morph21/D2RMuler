using D2RMuler.Db;
using D2RMuler.Utils;
using D2RMuler.Utils.KeyListener;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        Controller ctrl;


        public MainWindow()
        {
            DB.Instance().InitializeDb();
            InitializeComponent();
            ReloadCharactersList();
            ctrl = new Controller();

            ctrl.SetupKeyboardHooks(new KeyBindAction(this));
        }

        private void ScreenShot_Click(object sender, RoutedEventArgs e)
        {
            ScreenshotStash();
        }

        internal void ScreenshotStash()
        {
            CharacterStash? stash = CharactersListView.SelectedItem as CharacterStash;
            if (stash != null)
            {
                string screen = Utils.ScreenShot.CaptureApplication("D2R");
                if (!string.IsNullOrEmpty(stash.ImageSrc))
                {
                    if (File.Exists(stash.ImageSrc))
                    {
                        string fimage = stash.ImageSrc;
                        StashImage.Source = null;
                        File.Delete(fimage);
                    }
                }
                if (!string.IsNullOrEmpty(screen))
                {
                    stash.ImageSrc = screen;
                    stash.Save();

                    StashImage.Source = getImage(screen);
                    ReloadCharactersList();
                    CharactersListView.SelectedIndex = 0;

                }
                else
                {
                    //TODO show alert
                }
            }
            else
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
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                CharacterStash? item = e.AddedItems?[0] as CharacterStash;

                if (item != null)
                {
                    if (!string.IsNullOrEmpty(item.ImageSrc))
                    {
                        if (File.Exists(item.ImageSrc))
                        {
                            StashImage.Source = getImage(item.ImageSrc);

                            repaintCanvas();

                        } else
                        {
                            //TODO Nie znaleziono pliku
                        }

                    } else
                    {
                        if (File.Exists("Themes/ImageNotFound.png"))
                        {
                            StashImage.Source = getImage("Themes/ImageNotFound.png");
                        }
                    }
                }
            }
            
        }

        private void DeleteCharacterBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var stash = btn?.DataContext as CharacterStash;

            if (stash != null)
            {
                var dialogResult = MessageBox.Show("Are you sure that you want to delete " + stash.Name, "Delete", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    stash.Delete();
                    ReloadCharactersList();
                }
            }

        }

        private void repaintCanvas()
        {
            return;
            Rectangle rect = new Rectangle();
            rect.Width = 20;
            rect.Height = 20;
            rect.StrokeThickness = 2;
            rect.Fill = Brushes.Sienna;
            Cnv.Children.Clear();
            Cnv.Children.Add(rect);
            Cnv.Width = StashImage.ActualWidth;
            Cnv.Height = StashImage.ActualHeight;
            Canvas.SetLeft(rect, StashImage.ActualWidth / 2 - 10);
            Canvas.SetTop(rect, StashImage.ActualHeight / 2 - 10);
        }

        private BitmapImage getImage(string path)
        {
            //create new stream and create bitmap frame
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new FileStream(path, FileMode.Open, FileAccess.Read);
            //bitmapImage.DecodePixelWidth = (int)_decodePixelWidth;
            //bitmapImage.DecodePixelHeight = (int)_decodePixelHeight;
            //load the image now so we can immediately dispose of the stream
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            //clean up the stream to avoid file access exceptions when attempting to delete images
            bitmapImage.StreamSource.Dispose();
            return bitmapImage;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            repaintCanvas();
        }

        private void MenuBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseWindowBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void WindowMaxize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            } else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void SelectionDown()
        {
            if (CharactersListView == null || CharactersListView.Items.IsEmpty)
            {
                return;
            }
            var index = CharactersListView.SelectedIndex;
            var count = CharactersListView.Items.Count;
            if (index == -1 || index == count - 1)
            {
                CharactersListView.SelectedIndex = 0;
            } else
            {
                CharactersListView.SelectedIndex = index + 1;
            }            
        }

        private void SelectionUp()
        {
            if (CharactersListView == null || CharactersListView.Items.IsEmpty)
            {
                return;
            }
            var index = CharactersListView.SelectedIndex;
            var count = CharactersListView.Items.Count;
            if (index == -1 || index == 0)
            {
                CharactersListView.SelectedIndex = count - 1;
            }
            else
            {
                CharactersListView.SelectedIndex = index - 1;
            }
        }

        class KeyBindAction : IKeybindActions
        {
            MainWindow _window;

            public KeyBindAction(MainWindow window)
            {
                _window = window;
            }

            public void ScreenshotStash()
            {
                _window.ScreenshotStash();
            }

            public void SelectionDown()
            {
                _window.SelectionDown();
            }

            public void SelectionUp()
            {
                _window.SelectionUp();
            }
        }
    }
}
