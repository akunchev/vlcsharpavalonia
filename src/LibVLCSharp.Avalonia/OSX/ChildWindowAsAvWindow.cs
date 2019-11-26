using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Native;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.VisualTree;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using MonoMac.WebKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Avalonia.OSX
{
    public class OSXUtils
    {
        public static NSWindow From(TopLevel impl)
        {
            var aw = impl as Window;
            return NSApplication.SharedApplication.Windows.First(w => w.Title == aw.Title);
        }
    }

    public class ChildWindowAsAvWindow : WindowImpl, IChildWindowImpl
    {
        private ITopLevelImpl _parent;
        private IAvaloniaNativeFactory _factory;
        private AvaloniaNativePlatformOptions _opt;
        private NSWindow _nsWindow;

        private ChildWindowAsAvWindow(ITopLevelImpl parent, NSWindow nsParent, IAvaloniaNativeFactory factory, AvaloniaNativePlatformOptions opt)
            : base(factory, opt)
        {
            this.Parent = parent;
            _nsParent = nsParent;
            this._factory = factory;
            this._opt = opt;
        }

        public TransparencySupport SupportTransparency => TransparencySupport.Nope;

        public ITopLevelImpl Parent { get => _parent; set => _parent = value; }

        private NSWindow _nsParent;

        public event Action LostFocus;

        public static IChildWindowImpl Create(ITopLevelImpl parent, NSWindow nsWnd, IAvaloniaNativeFactory factory, AvaloniaNativePlatformOptions opt)
        {
            //var old = NSApplication.SharedApplication.Windows.ToArray();
            var res = new ChildWindowAsAvWindow(parent, nsWnd, factory, opt);

            //var nsWnd = NSApplication.SharedApplication.Windows.First(w => !old.Contains(w));

            res.Init();

            return res;
        }
        private static int count = 0;
        private void Init()
        {
           // var rr = Utils.From(_parent as WindowImpl);
            var app = NSApplication.SharedApplication;
            count++;
            var tmpTitle = $"{_nsParent.Title}#childwindow#{count}";
            SetTitle(tmpTitle);
            var wnd = app.Windows.FirstOrDefault(w => w.Title == tmpTitle);
            _nsWindow = wnd;

            var ppp = Native.NativePointer;

            // var view = nsWnd.ContentView;
            var view = new WebView();
            _nsWindow.ContentView.AddSubview(view);
            view.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
            _nsWindow.ContentView.AutoresizesSubviews = true;
            view.MainFrame.LoadRequest(new MonoMac.Foundation.NSUrlRequest(new NSUrl("https://dir.bg")));

            //_nsParent.AddChildWindow(_nsWindow, NSWindowOrderingMode.Above);
            //_nsParent.att

            _nsWindow.ParentWindow = _nsParent;

            //SetSystemDecorations(false);
            
            Handle = new PlatformHandle(_nsWindow.ContentView.Handle, "NSView");
        }

        public bool EnableTransparency(bool enable, Color? tranparencyColor = null)
        {
            return false;
        }

        public IPlatformHandle Handle { get; private set; }

        public new void Move(PixelPoint point)
        {
            var screen = _nsParent.ConvertBaseToScreen(new CGPoint(point.X, point.Y));
            base.Move(PixelPoint.FromPoint(new Point(screen.X, screen.Y), 1));
        }

        //IRenderer ITopLevelImpl.CreateRenderer(IRenderRoot root)
        //{
        //    return new NullRenderer();
        //}

        //public new void Invalidate(Rect rect)
        //{

        //}

        private class NullRenderer : IRenderer
        {
            public bool DrawFps { get; set; }
            public bool DrawDirtyRects { get; set; }

            public event EventHandler<SceneInvalidatedEventArgs> SceneInvalidated;

            public void AddDirty(IVisual visual)
            {
            }

            public void Dispose()
            {
            }

            public IEnumerable<IVisual> HitTest(Point p, IVisual root, Func<IVisual, bool> filter)
            {
                return Array.Empty<IVisual>();
            }

            public void Paint(Rect rect)
            {
            }

            public void RecalculateChildren(IVisual visual)
            {
            }

            public void Resized(Size size)
            {
            }

            public void Start()
            {
            }

            public void Stop()
            {
            }
        }
    }
}