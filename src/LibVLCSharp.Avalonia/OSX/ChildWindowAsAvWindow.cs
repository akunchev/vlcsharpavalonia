using Avalonia;
using Avalonia.Media;
using Avalonia.Native;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using System;

namespace LibVLCSharp.Avalonia.OSX
{
    public class ChildWindowAsAvWindow : WindowImpl, IChildWindowImpl
    {
        private ITopLevelImpl _parent;
        private IAvaloniaNativeFactory _factory;
        private AvaloniaNativePlatformOptions _opt;

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
            return new ChildWindowAsAvWindow(parent, factory, opt);
        }

        public bool EnableTransparency(bool enable, Color? tranparencyColor = null)
        {
            return false;
        }
    }
}