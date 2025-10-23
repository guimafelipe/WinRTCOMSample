using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

namespace WinRTCOMServer
{
    public sealed class COMService : IDisposable
    {
        public IAsyncOperation<string> HelloAsync(string Name)
        {
            return AsyncInfo.FromResult($"Hello {Name}, I'm OOP COM Service");
        }

        public IAsyncOperation<bool> CheckStatusAsync()
        {
            return AsyncInfo.FromResult(true);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Program.RemoveServerReference();
        }

        public COMService()
        {
            Program.AddServerReference();
        }

        ~COMService()
        {
            Dispose();
        }
    }
}
