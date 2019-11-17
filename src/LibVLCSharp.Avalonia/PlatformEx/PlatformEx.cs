using Avalonia;
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
            else
            {
                throw new NotSupportedException($"Platform {platform.OperatingSystem} not supported!");
            }
        }
    }
}