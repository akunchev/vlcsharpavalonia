using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Embedding;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace LibVLCSharp.Avalonia
{
    public class ChildWindow : TopLevel, IStyleHost, IStyleable, IDisposable
    {
        public TopLevel ParentTopLevel { get; }
        public Control ParentControl { get; }

        //use magenta or some custom value ???
        public Color OverlayTransparencyColor { get; } = Colors.Magenta;

        private bool _isTransparent = false;

        public bool IsTransparent
        {
            get => _isTransparent;
            private set
            {
                if (_isTransparent != value)
                {
                    _isTransparent = value;

                    if (PlatformImpl.SupportTransparency == TransparencySupport.Nope)
                    {
                        throw new Exception("Platform doesn't support transparency!");
                    }

                    PlatformImpl.EnableTransparency(_isTransparent, OverlayTransparencyColor);
                }
            }
        }

        private IDisposable _subscriptions;

        public new IChildWindowImpl PlatformImpl => base.PlatformImpl as IChildWindowImpl;

        public ChildWindow(TopLevel parentWnd, Control parentControl, bool isTransparent = false) : base(PlatformEx.Current.CreateChildWindow(parentWnd.PlatformImpl))
        {
            ParentTopLevel = parentWnd;
            ParentControl = parentControl;

            Background = new SolidColorBrush(OverlayTransparencyColor);

            var wnd = ParentTopLevel as Window;

            IsTransparent = isTransparent;

            if (parentControl is IPanel panel)
            {
                //may be add to visual tree but will break parent window rendering
                //and need to override attached to visualtree so TopLEvel don't throw exception
                //panel.Children.Add(this);
            }
            var subs = new CompositeDisposable()
            {
                Observable.FromEventPattern(ParentControl, nameof(ParentControl.LayoutUpdated)).Subscribe(_ => OnParentResized()),
                ParentControl.GetObservable(TransformedBoundsProperty).Subscribe(_ => OnParentResized()),
                ParentControl.GetObservable(IsVisibleProperty).Subscribe(v => IsVisible = v),
                Observable.FromEventPattern(ParentControl, nameof(ParentControl.AttachedToVisualTree)).Subscribe(_ => UpdatePlatformVisibility()),
                Observable.FromEventPattern(ParentControl, nameof(ParentControl.DetachedFromVisualTree)).Subscribe(_ => UpdatePlatformVisibility()),
                this.GetObservable(IsVisibleProperty).Skip(1).Subscribe(v => UpdatePlatformVisibility()),
                Observable.FromEventPattern(ParentControl, nameof(ParentControl.DetachedFromLogicalTree)).Subscribe(_ => Dispose()),
                Observable.FromEventPattern(ParentTopLevel, nameof(ParentTopLevel.Closed)).Subscribe(_ => Dispose()),
            };
            _subscriptions = subs;

            Bind(DataContextProperty, ParentControl.GetObservable(DataContextProperty));

            if (parentControl.IsArrangeValid)
            {
                Prepare();
            }
            else
            {
                subs.Add(Observable.FromEventPattern(ParentControl, nameof(ParentControl.LayoutUpdated)).Take(1).Subscribe(_ => Prepare()));
            }
        }

        private bool _firstTimeShow = true;

        private void UpdatePlatformVisibility()
        {
            if (!IsInitialized)
            {
                return;
            }

            var p = PlatformImpl;
            var v = IsVisible;

            if ((ParentControl as IVisual).IsAttachedToVisualTree)
            {
                if (v)
                {
                    p.Show();
                    if (_firstTimeShow)
                    {
                        _firstTimeShow = false;
                        //on windows first time child window is not invalidated
                        Dispatcher.UIThread.InvokeAsync(() => p.Invalidate(Bounds.WithX(0).WithY(0)));
                    }
                }
                else
                {
                    p.Hide();
                }
            }
            else
            {
                p.Hide();
            }
        }

        protected virtual void OnParentResized(Rect? bounds = null)
        {
            var b = bounds ?? ParentControl.Bounds;
            var p = PlatformImpl;

            var newPosition = PixelPoint.FromPoint(ParentControl.TranslatePoint(new Point(), ParentTopLevel) ?? new Point(), p.Scaling);

            if (newPosition != p.Position)
            {
                p.Move(newPosition);
            }

            var newSize = b.Size;

            if (p.ClientSize != newSize)
            {
                p.Resize(b.Size);
            }
        }

        IStyleHost IStyleHost.StylingParent => (ParentControl as IStyleHost) ??
            AvaloniaLocator.Current.GetService<IGlobalStyles>();

        private bool EnforceClientSize { get; set; } = true;

        protected virtual void Prepare()
        {
            EnsureInitialized();
            ApplyTemplate();
            LayoutManager.ExecuteInitialLayoutPass(this);

            UpdatePlatformVisibility();
        }

        private void EnsureInitialized()
        {
            if (!this.IsInitialized)
            {
                var init = (ISupportInitialize)this;
                init.BeginInit();
                init.EndInit();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (EnforceClientSize)
                availableSize = PlatformImpl?.ClientSize ?? default(Size);
            var rv = base.MeasureOverride(availableSize);
            if (EnforceClientSize)
                return availableSize;
            return rv;
        }

        //temp todo add style
        Type IStyleable.StyleKey => typeof(EmbeddableControlRoot);

        public void Dispose()
        {
            _subscriptions?.Dispose();
            _subscriptions = null;
            PlatformImpl.Dispose();
        }
    }
}