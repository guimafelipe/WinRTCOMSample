using System;
using System.Threading;
using System.Timers;
using Vanara.PInvoke;
using Timer = System.Timers.Timer;

namespace WinRTCOMServer
{
    internal class Program
    {
        private static readonly Timer IdleTimer = new Timer(TimeSpan.FromSeconds(30));
        private static readonly ManualResetEvent ShutdownEvent = new ManualResetEvent(false);

        static Program()
        {
            IdleTimer.AutoReset = false;
            IdleTimer.Elapsed += IdleTimer_Elapsed;
        }

        static void Main(string[] args)
        {
            NativeWin32API.WinRTInitialize().ThrowIfFailed();

            try
            {
                NativeWin32API.RegisterActivationFactories(["WinRTCOMServer.COMService"], out IntPtr Cookie).ThrowIfFailed();

                try
                {
                    ShutdownEvent.WaitOne();
                }
                finally
                {
                    NativeWin32API.RevokeActivationFactories(Cookie).ThrowIfFailed();
                }
            }
            finally
            {
                NativeWin32API.WinRTUninitialize().ThrowIfFailed();
            }
        }

        internal static void AddServerReference()
        {
            Ole32.CoAddRefServerProcess();

            if (IdleTimer.Enabled)
            {
                IdleTimer.Stop();
            }
        }

        internal static void RemoveServerReference()
        {
            if (Ole32.CoReleaseServerProcess() == 0)
            {
                IdleTimer.Start();
            }
        }

        private static void IdleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ShutdownEvent.Set();
        }
    }
}
