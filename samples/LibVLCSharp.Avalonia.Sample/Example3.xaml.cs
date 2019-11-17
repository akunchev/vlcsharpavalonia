using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Avalonia.Sample
{
    public class Example3 : Window
    {
        private IVideoView VideoView;
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;

        public Example3()
        {
            this.InitializeComponent();
            //Renderer.DrawFps = true;

            VideoView = this.Get<IVideoView>("VideoView");

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            VideoView.MediaPlayer = _mediaPlayer;
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideoView.MediaPlayer.IsPlaying)
            {
                VideoView.MediaPlayer.Stop();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!VideoView.MediaPlayer.IsPlaying)
            {
                VideoView.MediaPlayer.Play(new Media(_libVLC,
                    "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", FromType.FromLocation));
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            VideoView.MediaPlayer.Pause();
        }
    }
}