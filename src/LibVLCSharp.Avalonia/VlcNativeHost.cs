using Avalonia;
using Avalonia.Controls;
using System;

namespace LibVLCSharp.Avalonia
{
    public class VlcNativeHost : ChildWindow
    {
        private ChildWindow _overplayerControl;

        private IControl _overlayChild;

        public IControl OverlayChild
        {
            get => _overlayChild;
            set
            {
                if (_overlayChild != value)
                {
                    _overlayChild = value;
                    if (value != null)
                    {
                        EnsureOverPlayerCreated();
                        _overplayerControl.Content = _overlayChild;
                    }
                    else if (_overplayerControl != null)
                    {
                        _overplayerControl.Content = _overlayChild;
                    }
                }
            }
        }

        public VlcNativeHost(Window parentWnd, Panel parentControl) : base(parentWnd, parentControl)
        {
            //may be or not
            // parentControl.Children.Add(this);

            // Show();
            // Content
            VLCPlayerHandle = this.PlatformImpl.Handle.Handle;
        }

        private void EnsureOverPlayerCreated()
        {
            if (_overplayerControl == null)
            {
                _overplayerControl = new ChildWindow(ParentTopLevel, ParentControl, true);
            }
        }

        public IntPtr VLCPlayerHandle { get; }

        private void _parent_LayoutUpdated(object sender, EventArgs e)
        {
            //var b = _parentControl.Bounds.WithWidth(_parentControl.Bounds.Width - 20);
            //var wnd = this.PlatformImpl as IWindowImpl;
            //var p = _parentControl.TranslatePoint(new Point(20, 0), _parentWnd) ?? new Point();
            //var newPosition = PixelPoint.FromPoint(p, _parentWnd.PlatformImpl.Scaling);
            //var newSize = b.Size;

            //if (wnd.ClientSize != newSize)
            //{
            //    wnd.Resize(b.Size);
            //}

            //if (_overplayerControl != null)
            //{
            //    _overplayerControl.PlatformImpl.Resize(_parentControl.Bounds.Size);
            //}
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            //don't call base
            // base.OnAttachedToVisualTree(e);
        }
    }
}