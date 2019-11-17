using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;

namespace LibVLCSharp.Avalonia
{
    public enum TransparencySupport
    {
        Nope,
        Supported,
        SupportedWithMaskColor
    }

    public interface IChildWindowImpl : IEmbeddableWindowImpl
    {
        /// <summary>
        /// Shows the top level.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the window.
        /// </summary>
        void Hide();

        /// <summary>
        /// Gets the platform window handle.
        /// </summary>
        IPlatformHandle Handle { get; }

        /// <summary>
        /// Sets the client size of the top level.
        /// </summary>
        void Resize(Size clientSize);

        /// <summary>
        /// Sets the client size of the top level.
        /// </summary>
        void Move(PixelPoint point);

        /// <summary>
        /// Gets the position of the window in device pixels.
        /// </summary>
        PixelPoint Position { get; }

        TransparencySupport SupportTransparency { get; }

        bool EnableTransparency(bool enable, Color? tranparencyColor = null);

        ITopLevelImpl Parent { get; set; }
    }
}