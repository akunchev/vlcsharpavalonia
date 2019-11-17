using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Win32;
using LibVLCSharp.Avalonia.Win.Private;
using System;

namespace LibVLCSharp.Avalonia.Win
{
    internal class ChildWindowImpl : WindowImpl, IWindowImpl, IEmbeddableWindowImpl, IChildWindowImpl
    {
        private static IntPtr contextParentHandle;

        public TransparencySupport SupportTransparency => TransparencySupport.SupportedWithMaskColor;

        public bool EnableTransparency(bool enable, Color? tranparencyColor = null)
        {
            //set transparency
            var h = Handle.Handle;
            var style = N.GetWindowLong(h, N.F.GWL_EXSTYLE);
            bool succeed;

            if (enable)
            {
                style |= N.F.WS_EX_LAYERED;
                var par = N.GetParent(h);

                N.SetParent(h, IntPtr.Zero);

                var r = N.SetWindowLong(h, N.F.GWL_EXSTYLE, style);
                succeed = r.ToInt32() != 0;

                N.SetParent(h, par);

                Color c = tranparencyColor ?? Colors.Magenta;

                //setlayered is bgr format
                N.SetLayeredWindowAttributes(h, (uint)c.B << 16 | (uint)c.G << 8 | c.R, 0, N.F.LWA_COLORKEY);
            }
            else
            {
                style &= ~N.F.WS_EX_LAYERED;
                succeed = N.SetWindowLong(h, N.F.GWL_EXSTYLE, style).ToInt32() > 0;
            }

            return succeed;
        }

        public static ChildWindowImpl Create(ITopLevelImpl parent)
        {
            contextParentHandle = (parent as IWindowImpl).Handle.Handle;
            var r = new ChildWindowImpl(parent);
            contextParentHandle = IntPtr.Zero;
            return r;
        }

        private ITopLevelImpl _parent;
        private IntPtr _parentHandle;

        protected ChildWindowImpl(ITopLevelImpl parent)
        {
            Parent = parent;

            //make child not needed right now
            // N.SetWindowLong(handle, N.F.GWL_STYLE, N.F.WS_CHILD);
        }

        protected override IntPtr CreateWindowOverride(ushort atom)
        {
            // return N.CreateWindowEx(0, atom, null, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            var r = N.CreateWindowEx(0, atom, null, N.F.WS_CHILD, 0, 0, 0, 0, contextParentHandle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            return r;
        }

        protected override IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == (uint)N.F.WM_KILLFOCUS)
                LostFocus?.Invoke();
            return base.WndProc(hWnd, msg, wParam, lParam);
        }

        public event Action LostFocus;

        void IWindowImpl.ShowTaskbarIcon(bool value)
        {
        }

        void IWindowImpl.SetSystemDecorations(bool enabled)
        {
        }

        void IWindowImpl.CanResize(bool value)
        {
        }

        public new void Show()
        {
            N.ShowWindow(Handle.Handle, N.ShowWindowCommand.Show);
        }

        public new void Hide()
        {
            N.ShowWindow(Handle.Handle, N.ShowWindowCommand.Hide);
        }

        public new IRenderer CreateRenderer(IRenderRoot root)
        {
            //for now in window defered rendering not invalidating properly let's use Immediate Renderer
            var loop = AvaloniaLocator.Current.GetService<IRenderLoop>();
            var customRendererFactory = AvaloniaLocator.Current.GetService<IRendererFactory>();

            if (customRendererFactory != null)
                return customRendererFactory.Create(root, loop);

            return new ImmediateRenderer(root);
            //return Win32Platform.UseDeferredRendering ?
            //    (IRenderer)new DeferredRenderer(root, loop, rendererLock: _rendererLock) :
            //    new ImmediateRenderer(root);
        }

        public new PixelPoint Position
        {
            get
            {
                N.RECT rc;
                var handle = Handle.Handle;
                N.GetWindowRect(handle, out rc);
                //transfer x,y from desktop coordinates to parent window coordinates
                N.MapWindowPoints(IntPtr.Zero, _parentHandle, ref rc, 2);
                return new PixelPoint(rc.left, rc.top);
            }
        }

        public ITopLevelImpl Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    _parentHandle = (_parent as IWindowImpl).Handle.Handle;

                    //not needed right now but might be
                    var pstyle = N.GetWindowLong(_parentHandle, N.F.GCL_STYLE);
                    N.SetWindowLong(_parentHandle, N.F.GCL_STYLE, pstyle | N.F.WS_CLIPCHILDREN);

                    //set owner
                    N.SetWindowLong(Handle.Handle, N.F.GWL_HWNDPARENT, (uint)_parentHandle);

                    //set parent
                    N.SetParent(Handle.Handle, _parentHandle);
                }
            }
        }
    }
}