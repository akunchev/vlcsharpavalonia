using Avalonia;
using Avalonia.Media;
using Avalonia.Native;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using MonoMac.AppKit;
using System;

namespace LibVLCSharp.Avalonia.OSX
{
    public class ChildWindowImpl : WindowBaseImpl, IEmbeddableWindowImpl, IChildWindowImpl
    {
        private WindowImpl _parent;
        private NSWindow _nsWndParent;
        private IAvnWindow _avnParent;
        private IntPtr _parentHandle;

        private IAvaloniaNativeFactory _factory;
        private static IAvaloniaNativeFactory _defaultFactory;
        private static AvaloniaNativePlatformOptions _defaultOptions;

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
                    _nsWndParent = new NSWindow(_parentHandle);
                }
            }
        }

        public event Action LostFocus;

        public static ChildWindowImpl Create(ITopLevelImpl parent)
        {
            if (_defaultFactory == null)
            {
                //we don't ahve access to factory in any way so we can obtain it in a hacky way atm
                var f = parent.GetType().GetField("_factory", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                _defaultFactory = (IAvaloniaNativeFactory)f.GetValue(parent);
            }

            if (_defaultOptions == null)
            {
                //we don't ahve access to factory in any way so we can obtain it in a hacky way atm
                var f = parent.GetType().GetField("_opts", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                _defaultOptions = (AvaloniaNativePlatformOptions)f.GetValue(parent);
            }

            var r = new ChildWindowImpl(parent, _defaultFactory, _defaultOptions);
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
            using (var e = new WindowBaseEvents(this))
            {
                Native = new IAvnWindowBase(_nsView.Handle);
                Init(Native, factory.CreateScreens());
            }

            Init(Native, _factory.CreateScreens());

            Handle = new PlatformHandle(_nsView.Handle, "NSView");
        }

        public override IPopupImpl CreatePopup()
        {
            return Parent.CreatePopup();
        }

        public void Move(PixelPoint point)
        {
            // Position = point;
        }

        public bool EnableTransparency(bool enable, Color? tranparencyColor = null)
        {
            return false;
        }

        public new IPlatformHandle Handle { get; }
    }
}