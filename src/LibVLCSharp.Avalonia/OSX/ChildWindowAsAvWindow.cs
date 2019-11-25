using Avalonia;
using Avalonia.Media;
using Avalonia.Native;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using System;
using System.Linq;

namespace LibVLCSharp.Avalonia.OSX
{
    public class ChildWindowAsAvWindow : WindowImpl, IChildWindowImpl
    {
        private ITopLevelImpl _parent;
        private IAvaloniaNativeFactory _factory;
        private AvaloniaNativePlatformOptions _opt;
        private NSWindow _nsWindow;

        private ChildWindowAsAvWindow(ITopLevelImpl parent, IAvaloniaNativeFactory factory, AvaloniaNativePlatformOptions opt)
            : base(factory, opt)
        {
            this.Parent = parent;
            this._factory = factory;
            this._opt = opt;
        }

        public TransparencySupport SupportTransparency => TransparencySupport.Nope;

        public ITopLevelImpl Parent { get => _parent; set => _parent = value; }

        public event Action LostFocus;

        public static IChildWindowImpl Create(ITopLevelImpl parent, IAvaloniaNativeFactory factory, AvaloniaNativePlatformOptions opt)
        {
            var old = NSApplication.SharedApplication.Windows.ToArray();
            var res = new ChildWindowAsAvWindow(parent, factory, opt);

            var nsWnd = NSApplication.SharedApplication.Windows.First(w => !old.Contains(w));

            res._nsWindow = nsWnd;
            var view = nsWnd.ContentView;
            var nsView = new NSView(new CGRect(0, 0, 500, 500));
            //nsView.se
            view.AddSubview(nsView);
            res.Handle = new PlatformHandle(nsView.Handle, "NSView");

            return res;
        }

        public bool EnableTransparency(bool enable, Color? tranparencyColor = null)
        {
            return false;
        }

        public IPlatformHandle Handle { get; private set; }
    }
}