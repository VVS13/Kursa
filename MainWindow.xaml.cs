using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
namespace AudioVideo
{
    public partial class MediaPlayerProject : Window
    {
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;

        private bool reverseTime = false;
        private bool isPausedByClick = false;

        public MediaPlayerProject()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
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

        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mePlayer != null) && (mePlayer.Source != null);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Play();
            mediaPlayerIsPlaying = true;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Pause();
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Stop();
            mediaPlayerIsPlaying = false;
        }
        
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

    }
}