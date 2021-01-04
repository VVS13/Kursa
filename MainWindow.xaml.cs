using System;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
namespace AudioVideo
{
    public partial class MediaPlayerProject : Window
    {
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;

        private bool fullscreen = false;

        private bool reverseTime = false;
        private bool isPausedByClick = false;
        private bool file_created = false;

        private string video_name = "";

        public MediaPlayerProject()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        private void timer_Tick(object sender, EventArgs e)
        {

            if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliderProgress.Minimum = 0;
                sliderProgress.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliderProgress.Value = mePlayer.Position.TotalSeconds;
            }

        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg;*.mp4)|*.mp3;*.mpg;*.mpeg;*.mp4|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                mePlayer.Source = new Uri(openFileDialog.FileName);
                video_name = openFileDialog.SafeFileName; //?
            //Console.WriteLine(video_name); works

        }

        private void PPbtn_Click (object sender, RoutedEventArgs e)
        {


            if (mediaPlayerIsPlaying == false && video_name!= "")
            {
                mePlayer.Play();
                Play_Pause.Content = FindResource("Pause");
                mediaPlayerIsPlaying = true;
            }
            else
            {
                mePlayer.Pause();
                Play_Pause.Content = FindResource("Play");
                mediaPlayerIsPlaying = false;
            }
        }

        //private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = (mePlayer != null) && (mePlayer.Source != null);
        //}

        //private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    mePlayer.Play();
        //    mediaPlayerIsPlaying = true;
        //}

        //private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = mediaPlayerIsPlaying;
        //}

        //private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    mePlayer.Pause();
        //}

        //private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = mediaPlayerIsPlaying;
        //}

        //private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    mePlayer.Stop();
        //    mediaPlayerIsPlaying = false;
        //}
        
        //------------------------------------------------------------------------------

        private void Backwards_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Backwards_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            mePlayer.Position -= TimeSpan.FromSeconds(5);
        }

        private void Forwards_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Forwards_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            mePlayer.Position += TimeSpan.FromSeconds(5);
        }

        //------------------------------------------------------------------------------

        private void Restart_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Restart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Stop();
            mePlayer.Play();
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (mediaPlayerIsPlaying)
                {
                    mePlayer.Pause();
                    mediaPlayerIsPlaying = false;
                }
                else
                {
                    mePlayer.Play();
                    mediaPlayerIsPlaying = true;
                }
            }
        }

        void OnMouseDownMedia(object sender, MouseButtonEventArgs args)
        {
            if (isPausedByClick == false)
            {
                mePlayer.Pause();
                isPausedByClick = true;
            }
            else
            {
                mePlayer.Play();
                isPausedByClick = false;
            }
        }

        //------------------------------------------------------------------------------

        private void sliderProgress_DragStarted(object sender, DragStartedEventArgs e)
        {

            userIsDraggingSlider = true;

            int SliderValue = (int)sliderProgress.Value;

            // Overloaded constructor takes the arguments days, hours, minutes, seconds, milliseconds.
            // Create a TimeSpan with miliseconds equal to the slider value.
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            mePlayer.Position = ts;
        }

        private void sliderProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mePlayer.Position = TimeSpan.FromSeconds(sliderProgress.Value);
        }

        private void sliderProgress_ValueChanged(object sender,RoutedPropertyChangedEventArgs<double> e)
        {
            

            timeUpdate();
        }

        private void lblProgressStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            reverseTime = !reverseTime;
            timeUpdate();

        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mePlayer.Volume += (e.Delta > 0) ? 0.05 : -0.05;
        }

        private void timeUpdate()
        { 
            if (!reverseTime)
            {
                lblProgressStatus.Text = TimeSpan.FromSeconds(sliderProgress.Value).ToString(@"hh\:mm\:ss");
            }
            else
            {
                lblProgressStatus.Text = "-" + TimeSpan.FromSeconds(sliderProgress.Maximum - sliderProgress.Value).ToString(@"hh\:mm\:ss");
            }
        }

        //---------------------------------------------------------------------
        private void btnSC_Click(object sender,RoutedEventArgs e)
        {
            if (video_name != "")
            {
                byte[] screenshot = mePlayer.getScreenshot(96);
                FileStream fileStream = new FileStream(@"screenshot.jpg", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                binaryWriter.Write(screenshot);
                binaryWriter.Close();
                    
            }
            
        }

        private void btnPlusTime_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Position += TimeSpan.FromSeconds(0.5);
        }

        private void btnMinusTime_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Position -= TimeSpan.FromSeconds(0.5);
        }


        private void ChangeMediaSpeedRatio(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            mePlayer.SpeedRatio = (double)speedRatioSlider.Value / 10;
        }

        //--------------------------------------------------------------------- other methods i tried

        //private void btnSaveFlag_Click(object sender, EventArgs e)
        //{
        //    string path = @"c:\temp\MyTest.txt";

        //    if (!File.Exists(path))
        //    {
        //        string[] createText = { "Hello", "And", "Welcome" };
        //        File.WriteAllLines(path, createText);
        //    }
        //    string appendText = "This is extra text" + Environment.NewLine;
        //    File.AppendAllText(path, appendText);

        //    //SaveFileDialog dlg = new SaveFileDialog();
        //    //dlg.Filter = "*.txt|*.txt";
        //    //dlg.RestoreDirectory = true;
        //    //File.WriteAllText(dlg.FileName, text1.Text);

        //    //TextWriter txt = new StreamWriter("C:\\demo.txt");
        //    //txt.WriteLine(text1.Text);
        //    //txt.Close();
        //}


        public void SaveFileDialogSample()
        {
            InitializeComponent();
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog(); //to save flags durring session

        private void btnSaveFlag_Click(object sender, RoutedEventArgs e)
        {

            if (mediaPlayerIsPlaying == true)
            {
                if (file_created == false)
                {
                    string line_to_be_writen = video_name + " -  Time:" + lblProgressStatus.Text + " -  Name:" + text1.Text;

                    if (saveFileDialog.ShowDialog() == true)
                        File.WriteAllText(saveFileDialog.FileName, line_to_be_writen);
                    file_created = true;
                }
                else
                {
                    string line_to_be_writen ="\n" + video_name + " -  Time:" + lblProgressStatus.Text + " -  Name:" + text1.Text;

                    File.AppendAllText(saveFileDialog.FileName, line_to_be_writen);
                }
                
                   
            }
            else
            {
                //maybe popup here
            }
        }
        //---------------------------------------------------------------------

        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            if (!fullscreen)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
            }

            fullscreen = !fullscreen;

        }

        private void pbVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }

    public static class Screenshot
    {
        public static byte[] getScreenshot(this UIElement source, int quality)
        {
            double scale = 1;
            //dont know how to make scale work 

            double sourceH = source.RenderSize.Height;
            double sourceW = source.RenderSize.Width;
            double renderH = sourceH * scale;
            double renderW = sourceW * scale;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderW, (int)renderH, 96, 96, PixelFormats.Pbgra32);
            VisualBrush sourceBrush = new VisualBrush(source);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                //drawingContext.PushTransform(new ScaleTranform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(sourceW,sourceH)));

            }


                renderTarget.Render(drawingVisual);

            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder
            {
                QualityLevel = quality
            };

            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            Byte[] imageArray;

            using (MemoryStream outputStream = new MemoryStream())
            {
                jpgEncoder.Save(outputStream);
                imageArray = outputStream.ToArray();
            }
            return imageArray;
        }
    }
}