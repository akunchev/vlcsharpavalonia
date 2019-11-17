using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using LibVLCSharp.Shared;
using System;

namespace LibVLCSharp.Avalonia
{
    //for mac check https://github.com/JBildstein/SpiderEye/blob/master/Source/SpiderEye.Mac/Native/Dispatch.cs#L14-L15
    public class NativeVideoView : ContentControl, IVideoView
    {
        static NativeVideoView()
        {
            MediaPlayerProperty.Changed.AddClassHandler<NativeVideoView>((s, e) => s.OnMediaPlayerChanged(e));
            ContentProperty.Changed.AddClassHandler<NativeVideoView>((s, e) => s.OnContentChanged());
        }

        private void OnContentChanged()
        {
            if (_native != null)
            {
                _native.OverlayChild = Content as IControl;
            }
        }

        public static readonly DirectProperty<NativeVideoView, MediaPlayer> MediaPlayerProperty =
            VideoView.MediaPlayerProperty.AddOwner<NativeVideoView>(v => v.MediaPlayer, (s, v) => s.MediaPlayer = v);

        private MediaPlayer _mediaPlayer;

        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            set => SetAndRaise(MediaPlayerProperty, ref _mediaPlayer, value);
        }

        private void OnMediaPlayerChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.OldValue is MediaPlayer oldMediaPlayer)
            {
                oldMediaPlayer.Hwnd = IntPtr.Zero;
            }
            if (e.NewValue is MediaPlayer newMediaPlayer)
            {
                TrySetHandle();
            }
        }

        private Panel PART_PlayerHost;

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            PART_PlayerHost = e.NameScope.Get<Panel>("PART_PlayerHost");
            TrySetHandle();
        }

        private VlcNativeHost _native;

        private void TrySetHandle()
        {
            if (PART_PlayerHost == null || MediaPlayer == null || _native != null) return;

            _native = new VlcNativeHost(VisualRoot as Window, PART_PlayerHost) { Background = Brushes.Black };

            _native.OverlayChild = Content as IControl;

            //newMediaPlayer.Hwnd -> for windows
            //newMediaPlayer.XWindow -> forlinux
            //newMediaPlayer.NsObject -> for mac

            MediaPlayer.Hwnd = _native.VLCPlayerHandle;
        }
    }
}