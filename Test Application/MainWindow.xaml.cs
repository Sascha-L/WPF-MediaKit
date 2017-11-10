using Microsoft.Win32;
using System;
using System.Windows;
using WPFMediaKit.DirectShow.Controls;
using System.Linq;

namespace Test_Application
{
    public partial class MainWindow : Window
    {
        private bool sliderDrag;
        private bool sliderMediaChange;

        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing;
            this.mediaUriElement.MediaFailed += MediaUriElement_MediaFailed;
            this.mediaUriElement.MediaUriPlayer.MediaPositionChanged += MediaUriPlayer_MediaPositionChanged;

            if (MultimediaUtil.VideoInputDevices.Any())
            {
                cobVideoSource.ItemsSource = MultimediaUtil.VideoInputNames;
            }
            SetCameraCaptureElementVisible(false);
        }

        private void SetCameraCaptureElementVisible(bool visible)
        {
            cameraCaptureElement.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            mediaUriElement.Visibility = !visible ? Visibility.Visible : Visibility.Collapsed;
            btnStop.IsEnabled = !visible;
            btnPause.IsEnabled = !visible;
            slider.IsEnabled = !visible;
            if (visible)
            {
                btnStop_Click(null, null);
            }
            else
            {
                cobVideoSource.SelectedIndex = -1;
            }
        }

        private void SetPlayButtons(bool playing)
        {
            if (playing)
            {
                btnPause.Content = "Pause";
            }
            else
            {
                btnPause.Content = "Play";
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            var result = dlg.ShowDialog();
            if (result != true)
                return;
            errorText.Text = null;
            SetCameraCaptureElementVisible(false);
            mediaUriElement.Source = new Uri(dlg.FileName);
            SetPlayButtons(true);
        }

        private void MediaUriElement_MediaFailed(object sender, WPFMediaKit.DirectShow.MediaPlayers.MediaFailedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => errorText.Text = e.Message));
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mediaUriElement.Close();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaUriElement.Stop();
            SetPlayButtons(false);
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            bool playing = mediaUriElement.IsPlaying;
            if (playing)
                mediaUriElement.Pause();
            else
                mediaUriElement.Play();
            SetPlayButtons(!playing);
        }

        private void MediaUriPlayer_MediaPositionChanged(object sender, EventArgs e)
        {
            if (sliderDrag)
                return;
            this.Dispatcher.BeginInvoke(new Action(ChangeSlideValue), null);
        }

        private void ChangeSlideValue()
        {
            if (sliderDrag)
                return;

            sliderMediaChange = true;
            double perc = (double)mediaUriElement.MediaPosition / mediaUriElement.MediaDuration;
            slider.Value = slider.Maximum * perc;
            sliderMediaChange = false;
        }

        private void ChangeMediaPosition()
        {
            if (sliderMediaChange)
                return;

            sliderDrag = true;
            double perc = slider.Value / slider.Maximum;
            mediaUriElement.MediaPosition = (long)(mediaUriElement.MediaDuration * perc);
            sliderDrag = false;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderMediaChange)
                return;

            this.Dispatcher.BeginInvoke(new Action(ChangeMediaPosition), null);
        }

        private void cobVideoSource_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cobVideoSource.SelectedIndex < 0)
                return;
            SetCameraCaptureElementVisible(true);
            cameraCaptureElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[cobVideoSource.SelectedIndex];
        }
        
        private void btnFrameStep_Click(object sender, RoutedEventArgs e)
        {
            if  (mediaUriElement.Source != null)
            {
                mediaUriElement.FrameStep(1);
            }
        }

        private void Window_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (mediaUriElement.Source != null)
            {
                mediaUriElement.FrameStep(1);
            }
        }
    }
}