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
    public class ChildWindowImpl : WindowBaseImpl, IEmbeddableWindowImpl, IChildWindowImpl
    {
        private WindowImpl _parent;
        private NSWindow _nsWndParent;
        private IAvnWindow _avnParent;
        private IntPtr _parentHandle;

        private IAvaloniaNativeFactory _factory;

        public IAvnWindowBase Native { get; private set; }

        private NSView _nsView;

        public ITopLevelImpl Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    _parent = (WindowImpl)value;
                    _avnParent = _parent.Native;
                    _parentHandle = (IntPtr)_avnParent;
                    var h1 = _avnParent.NativePointer;
                    try
                    {
                        //TODO: find a way to map
                       _nsWndParent = NSApplication.SharedApplication.Windows.Last();
                    // _nsWndParent = new NSWindow(_parentHandle);
                    }
                    catch(Exception e)
                    {
                    }
                }
            }
        }

        public event Action LostFocus;

        public static ChildWindowImpl Create(ITopLevelImpl parent, IAvaloniaNativeFactory factory, AvaloniaNativePlatformOptions opt)
        {
            var r = new ChildWindowImpl(parent, factory, opt);
            return r;
        }

        public TransparencySupport SupportTransparency => TransparencySupport.Nope;

        private ChildWindowImpl(ITopLevelImpl parent, IAvaloniaNativeFactory factory, AvaloniaNativePlatformOptions opts) : base(opts)
        {
            _factory = factory;
            Parent = parent;

            _nsView = new NSView();
            // _nsWndParent.ContentView = view;
            _nsWndParent.ContentView.AddSubview(_nsView);

            _factory = factory;
            //_opts = opts;
            //using (var e = new WindowBaseEvents(this))
            //{
            //    Native = new IAvnWindowBase(_nsView.Handle);
            //    Init(Native, factory.CreateScreens());
            //}

            Handle = new PlatformHandle(_nsView.Handle, "NSView");
        }

        public override IPopupImpl CreatePopup()
        {
            return Parent.CreatePopup();
        }

        public void Move(PixelPoint point)
        {
            // Position = point;
            _nsView.SetFrameOrigin(new CGPoint(point.X, point.Y));
        }

        public new void Resize(Size clientSize)
        {
            // _nsView.Frame = new MonoMac.CoreGraphics.CGRect(0, 0, ClientSize.Width, ClientSize.Height);
            _nsView.SetFrameSize(new CGSize(ClientSize.Width, ClientSize.Height));
        }

        public new void Show()
        {
            _nsView.Hidden = false;
        }

        public new void Hide()
        {
            _nsView.Hidden = true;
        }

        public bool EnableTransparency(bool enable, Color? tranparencyColor = null)
        {
            return false;
        }

        public new IPlatformHandle Handle { get; }
    }
}