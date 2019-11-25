using Avalonia;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using System;

namespace LibVLCSharp.Avalonia
{
    public interface IPlatformEx
    {
        IChildWindowImpl CreateChildWindow(ITopLevelImpl parent);
    }

    internal class Win32PlatformEx : IPlatformEx
    {
        public IChildWindowImpl CreateChildWindow(ITopLevelImpl parent) => Win.ChildWindowImpl.Create(parent);
    }

    internal class OSXPlatformEx : IPlatformEx
    {
        private IAvaloniaNativeFactory _defaultFactory;
        private AvaloniaNativePlatformOptions _defaultOptions;

        public IChildWindowImpl CreateChildWindow(ITopLevelImpl parent)
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

            // return   OSX.ChildWindowImpl.Create(parent, _defaultFactory, _defaultOptions);
            return OSX.ChildWindowAsAvWindow.Create(parent, _defaultFactory, _defaultOptions);
        }
    }

    public static class PlatformEx
    {
        public static IPlatformEx Current => AvaloniaLocator.Current.GetService<IPlatformEx>();

        static PlatformEx()
        {
            var platform = AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo();
            if (platform.OperatingSystem == OperatingSystemType.WinNT)
            {
                AvaloniaLocator.CurrentMutable.Bind<IPlatformEx>().ToConstant(new Win32PlatformEx());
            }
            else if(platform.OperatingSystem == OperatingSystemType.OSX)
            {
                AvaloniaLocator.CurrentMutable.Bind<IPlatformEx>().ToConstant(new OSXPlatformEx());
            }
            else
            {
                throw new NotSupportedException($"Platform {platform.OperatingSystem} not supported!");
            }
        }
    }
}