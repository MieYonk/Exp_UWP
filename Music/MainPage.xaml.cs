using System;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Music
{
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _progressTimer;
        private bool _isUserDragging = false;

        public MainPage()
        {
            this.InitializeComponent();
            InitializeMediaPlayer();
        }

        private void InitializeMediaPlayer()
        {
            // Initialize progress timer
            _progressTimer = new DispatcherTimer();
            _progressTimer.Interval = TimeSpan.FromMilliseconds(500);
            _progressTimer.Tick += ProgressTimer_Tick;

            // Set initial volume
            mediaElement.Volume = 0.5;
            volumeSlider.Value = 50;

            // Bind media events
            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.MediaEnded += MediaElement_MediaEnded;
        }

        private async void playList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playList.SelectedItem is ListBoxItem selectedItem && selectedItem.Tag is string videoPath)
            {
                try
                {
                    // 加载视频文件
                    StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(
                        new Uri($"ms-appx:///Assets/{videoPath}"));

                    // 将文件路径转换为Uri
                    Uri videoUri = new Uri(file.Path);

                    // 设置MediaElement的Source为Uri
                    mediaElement.Source = videoUri;
                }
                catch (Exception ex)
                {
                    ShowErrorDialog($"无法加载视频: {ex.Message}");
                }
            }
        }




        private void ProgressTimer_Tick(object sender, object e)
        {
            if (!_isUserDragging && mediaElement.NaturalDuration.HasTimeSpan)
            {
                slider1.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                slider1.Value = mediaElement.Position.TotalSeconds;
            }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            _progressTimer.Start();
            slider1.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            _progressTimer.Stop();
            mediaElement.Position = TimeSpan.Zero;
        }

        private void Slider1_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_isUserDragging)
            {
                mediaElement.Position = TimeSpan.FromSeconds(slider1.Value);
            }
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement.CurrentState == MediaElementState.Playing)
            {
                mediaElement.Pause();
            }
            else
            {
                mediaElement.Play();
            }
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Position -= TimeSpan.FromSeconds(10);
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Position += TimeSpan.FromSeconds(10);
        }

        private void volumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            mediaElement.Volume = volumeSlider.Value / 100;
        }


        private async void ShowErrorDialog(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }
    }
}
