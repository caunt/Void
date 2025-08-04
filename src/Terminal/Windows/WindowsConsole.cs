using System.Runtime.InteropServices;

namespace Void.Terminal;

public static partial class WindowsConsole
{
    private const int StdOutputHandle = -11;
    private const int EnableVirtualTerminalProcessing = 0x4;

    private static partial class NativeMethods
    {
        private const string Kernel32 = "kernel32.dll";

        [LibraryImport(Kernel32, EntryPoint = "GetVersion", SetLastError = true)]
        public static partial int GetVersion();

        [LibraryImport(Kernel32, EntryPoint = "SetConsoleMode", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

        [LibraryImport(Kernel32, EntryPoint = "GetConsoleMode", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetConsoleMode(IntPtr handle, out int mode);

        [LibraryImport(Kernel32, EntryPoint = "GetStdHandle", SetLastError = true)]
        public static partial IntPtr GetStdHandle(int handle);
    }

    public static bool TryEnableVirtualTerminalProcessing()
    {
        if (Console.IsOutputRedirected)
            return true;

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return true;

        try
        {
            var handle = NativeMethods.GetStdHandle(StdOutputHandle);

            NativeMethods.GetConsoleMode(handle, out var mode);
            NativeMethods.SetConsoleMode(handle, mode | EnableVirtualTerminalProcessing);
            NativeMethods.GetConsoleMode(handle, out mode);

            return (mode & EnableVirtualTerminalProcessing) == EnableVirtualTerminalProcessing;
        }
        catch (DllNotFoundException)
        {
            return false;
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }
    }
}
