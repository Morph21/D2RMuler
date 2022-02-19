using D2RMuler.Db;
using D2RMuler.Utils.KeyListener;
using D2RMuler.Views;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace D2RMuler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool DragWindowPossible = false;
        List<CharacterStash> CharacterStashes = new List<CharacterStash>();
        Controller ctrl;


        public MainWindow()
        {
            DB.Instance().InitializeDb();
            InitializeComponent();
            ReloadCharactersList();
            SelectFirstCharacter();
            ctrl = new Controller();

            ctrl.SetupKeyboardHooks(new KeyBindAction(this));

            //Dont hide taskbar on maximize
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
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

        private void SelectFirstCharacter()
        {
            if (CharactersListView.Items.Count > 0)
            {
                CharactersListView.SelectedIndex = 0;
            }
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

                        }
                        else
                        {
                            StashImage.Source = getImage("Themes/ImageNotFound.png");
                        }

                    }
                    else
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
        
        private BitmapImage getImage(byte[] src)
        {
            //create new stream and create bitmap frame
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(src);
            //bitmapImage.DecodePixelWidth = (int)_decodePixelWidth;
            //bitmapImage.DecodePixelHeight = (int)_decodePixelHeight;
            //load the image now so we can immediately dispose of the stream
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            //clean up the stream to avoid file access exceptions when attempting to delete images
            bitmapImage.StreamSource.Dispose();
            return bitmapImage;
        }

        private void MenuBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragWindowPossible = true;
            }
        }

        private void MenuBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && DragWindowPossible)
            {
                if (WindowState == WindowState.Maximized)
                {
                    Point mouseP = PointToScreen(Mouse.GetPosition(this));
                    Top = mouseP.Y - MenuBar.ActualHeight / 2;
                    Left = mouseP.X - Width / 2;
                    WindowState = WindowState.Normal;
                    //Top = mouseP.Y - MenuBar.ActualHeight/2;
                    //Left = mouseP.X - Width/2;
                }
                DragMove();
            }
        }

        private void MenuBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DragWindowPossible = false;
        }

        private void CloseWindowBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            e.Handled = true;
        }

        private void WindowMaxize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
            e.Handled = true;
        }

        private void WindowMinimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new System.Windows.Thickness(6);
            }
            else
            {
                this.BorderThickness = new System.Windows.Thickness(0);
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
            }
            else
            {
                CharactersListView.SelectedIndex = index + 1;
            }
            CharactersListView.ScrollIntoView(CharactersListView.SelectedItem);
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
            CharactersListView.ScrollIntoView(CharactersListView.SelectedItem);
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

        private void KeyBindsButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            KeyBindsWindow keyBindsWindow = new KeyBindsWindow();
            keyBindsWindow.Owner = this;
            keyBindsWindow.ShowDialog();
        }

        private void SearchForRalButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CharacterStash item = CharactersListView.SelectedItem as CharacterStash;


            Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> source = new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(item.ImageSrc); // Bigger image

            Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> template = new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>("ral_rune.png"); // smaller image
            Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> imageToShow = source.Copy();


            const double minTreshold = 0.9;
            double actualTreshold = 1;
            

            while (actualTreshold > minTreshold)
            {
                Emgu.CV.Image<Emgu.CV.Structure.Gray, float> result = imageToShow.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
                double[] minValues, maxValues;
                System.Drawing.Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                actualTreshold = maxValues[0];
                if (maxValues[0] > minTreshold)
                {
                    // This is a match. Do something with it, for example draw a rectangle around it.
                    System.Drawing.Rectangle match = new System.Drawing.Rectangle(maxLocations[0], template.Size);
                    imageToShow.Draw(match, new Emgu.CV.Structure.Bgr(System.Drawing.Color.Red), 3);
                    
                }
            }

            StashImage.Source = getImage(imageToShow.ToJpegData());
        }

        private void test2(object sender, MouseButtonEventArgs e)
        {
            CharacterStash item = CharactersListView.SelectedItem as CharacterStash;
            string imageSrc = item.ImageSrc;
            File.Exists("ral_rune.png");
            
            Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> source = new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(item.ImageSrc); // Bigger image
           
            Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> template = new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>("ral_rune.png"); // smaller image
            Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> imageToShow = source.Copy();

            using (Emgu.CV.Image<Emgu.CV.Structure.Gray, float> result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
            {
                var listOfMaxs = new List<System.Drawing.Point>();
                var resultWithPadding = new Emgu.CV.Image<Emgu.CV.Structure.Gray, float>(source.Size);
                var heightOfPadding = (source.Height - result.Height) / 2;
                var widthOfPadding = (source.Width - result.Width) / 2;
                resultWithPadding.ROI = new System.Drawing.Rectangle() { X = heightOfPadding, Y = widthOfPadding, Width = result.Width, Height = result.Height };
                result.CopyTo(resultWithPadding);
                resultWithPadding.ROI = System.Drawing.Rectangle.Empty;

                for (int i = 0; i < resultWithPadding.Width; i++)
                {
                    for (int j = 0; j < resultWithPadding.Height; j++)
                    {
                        var centerOfRoi = new System.Drawing.Point() { X = i + template.Width / 2, Y = j + template.Height / 2 };
                        var roi = new System.Drawing.Rectangle() { X = i, Y = j, Width = template.Width, Height = template.Height };
                        resultWithPadding.ROI = roi;
                        double[] minValues, maxValues;
                        System.Drawing.Point[] minLocations, maxLocations;
                        resultWithPadding.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                        resultWithPadding.ROI = System.Drawing.Rectangle.Empty;
                        var maxLocation = maxLocations[0]; // First();
                        if (maxLocation.X == roi.Width / 2 && maxLocation.Y == roi.Height / 2)
                        {
                            if (maxValues[0] > 0.9)
                            {
                                var point = new System.Drawing.Point() { X = i, Y= j };
                                listOfMaxs.Add(point);

                                System.Drawing.Rectangle match = new System.Drawing.Rectangle(point, template.Size);
                                
                                imageToShow.Draw(match, new Emgu.CV.Structure.Bgr(System.Drawing.Color.Red), 10);
                            }
                           
                        }

                    }
                }

            }

            // Show imageToShow in an ImageBox (here assumed to be called imageBox1)

            StashImage.Source = getImage(imageToShow.ToJpegData());
            //imageBox1.Image = imageToShow;

        }

        private void test()
        {
            CharacterStash item = CharactersListView.SelectedItem as CharacterStash;
            string imageSrc = item.ImageSrc;
            File.Exists("ral_rune.png");

            Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> source = new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(item.ImageSrc); // Bigger image

            Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> template = new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>("ral_rune.png"); // smaller image
            Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> imageToShow = source.Copy();

            using (Emgu.CV.Image<Emgu.CV.Structure.Gray, float> result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
            {
                double[] minValues, maxValues;
                System.Drawing.Point[] minLocations, maxLocations;



                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                // You can try different values of the threshold. I guess somewhere between 0.75 and 0.95 would be good.
                if (maxValues[0] > 0.9)
                {
                    // This is a match. Do something with it, for example draw a rectangle around it.
                    System.Drawing.Rectangle match = new System.Drawing.Rectangle(maxLocations[0], template.Size);
                    imageToShow.Draw(match, new Emgu.CV.Structure.Bgr(System.Drawing.Color.Red), 3);
                }
            }

            // Show imageToShow in an ImageBox (here assumed to be called imageBox1)

            StashImage.Source = getImage(imageToShow.ToJpegData());
            //imageBox1.Image = imageToShow;
        }

    }
}
