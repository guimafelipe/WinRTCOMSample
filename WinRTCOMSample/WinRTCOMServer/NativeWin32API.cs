using System;
using System.Linq;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using WinRT;

namespace WinRTCOMServer
{
    internal static partial class NativeWin32API
    {
        private enum RO_INIT_TYPE
        {
            RO_INIT_SINGLETHREADED = 0,
            RO_INIT_MULTITHREADED = 1,
        }

        [LibraryImport("api-ms-win-core-winrt-l1-1-0.dll")]
        private static partial int RoInitialize(RO_INIT_TYPE initType);

        [LibraryImport("api-ms-win-core-winrt-l1-1-0.dll")]
        private static partial void RoUninitialize();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private unsafe delegate int PfnActivationFactoryCallback(IntPtr activatableClassId, out IntPtr activationFactory);

        [LibraryImport("api-ms-win-core-winrt-l1-1-0.dll")]
        private static partial void RoRevokeActivationFactories(IntPtr cookie);

        [LibraryImport("api-ms-win-core-winrt-l1-1-0.dll")]
        private static partial int RoRegisterActivationFactories([In] IntPtr[] activatableClassIds, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), In] PfnActivationFactoryCallback[] activationFactoryCallbacks, uint count, out IntPtr cookie);

        public static HRESULT WinRTInitialize()
        {
            try
            {
                return new HRESULT(RoInitialize(RO_INIT_TYPE.RO_INIT_MULTITHREADED));
            }
            catch (Exception ex)
            {
                return HRESULT.FromException(ex);
            }
        }

        public static HRESULT WinRTUninitialize()
        {
            try
            {
                RoUninitialize();
            }
            catch (Exception ex)
            {
                return HRESULT.FromException(ex);
            }

            return HRESULT.S_OK;
        }

        public static HRESULT RegisterActivationFactories(string[] ActivatableClassIds, out IntPtr Cookie)
        {
            Cookie = IntPtr.Zero;

            if ((ActivatableClassIds?.Length).GetValueOrDefault() == 0)
            {
                return new HRESULT(HRESULT.E_INVALIDARG);
            }

            return new HRESULT(RoRegisterActivationFactories([.. ActivatableClassIds.Select(MarshalString.FromManaged)], [.. ActivatableClassIds.Select<string, PfnActivationFactoryCallback>((_) => GetActivationFactory)], (uint)ActivatableClassIds.Length, out Cookie));
        }

        public static HRESULT RevokeActivationFactories(IntPtr Cookie)
        {
            if (Cookie.IsValid())
            {
                try
                {
                    RoRevokeActivationFactories(Cookie);
                }
                catch (Exception ex)
                {
                    return HRESULT.FromException(ex);
                }

                return HRESULT.S_OK;
            }

            return HRESULT.E_INVALIDARG;
        }

        private static int GetActivationFactory(IntPtr activatableClassId, out IntPtr factory)
        {
            factory = IntPtr.Zero;

            if (!activatableClassId.IsValid())
            {
                return HRESULT.E_INVALIDARG;
            }

            try
            {
                factory = Module.GetActivationFactory(MarshalString.FromAbi(activatableClassId));
            }
            catch (Exception e)
            {
                return ExceptionHelpers.GetHRForException(e);
            }

            return factory.IsValid() ? HRESULT.S_OK : HRESULT.CLASS_E_CLASSNOTAVAILABLE;
        }
    }
}
