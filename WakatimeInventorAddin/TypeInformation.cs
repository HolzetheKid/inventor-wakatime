using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace WakatimeInventorAddIn
{
    public static class TypeInformation
    {
        public static string GetNames(object comObject)
        {
            var dispatch = comObject as IDispatch;

            if (dispatch == null)
            {
                return null;
            }

            var pTypeInfo = dispatch.GetTypeInfo(0, 1033);

            string[] names = new String[] { "Name" };
            int[] ids = new int[names.Length];

            pTypeInfo.GetIDsOfNames(names, names.Length, ids);



            return null;
        }

        public static string GetTypeName(object comObject)
        {
            var dispatch = comObject as IDispatch;

            if (dispatch == null)
            {
                return null;
            }

            var pTypeInfo = dispatch.GetTypeInfo(0, 1033);
            
            string pBstrName;
            string pBstrDocString;
            int pdwHelpContext;
            string pBstrHelpFile;
            pTypeInfo.GetDocumentation(-1, out pBstrName, out pBstrDocString, out pdwHelpContext, out pBstrHelpFile);

            string str = pBstrName;
            if (str[0] == 95)
            {
                // remove leading '_'
                str = str.Substring(1);
            }

            return str;
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("00020400-0000-0000-C000-000000000046")]
        private interface IDispatch
        {
            int GetTypeInfoCount();

            [return: MarshalAs(UnmanagedType.Interface)]
            ITypeInfo GetTypeInfo(
                [In, MarshalAs(UnmanagedType.U4)] int iTInfo,
                [In, MarshalAs(UnmanagedType.U4)] int lcid);

            void GetIDsOfNames(
                [In] ref Guid riid,
                [In, MarshalAs(UnmanagedType.LPArray)] string[] rgszNames,
                [In, MarshalAs(UnmanagedType.U4)] int cNames,
                [In, MarshalAs(UnmanagedType.U4)] int lcid,
                [Out, MarshalAs(UnmanagedType.LPArray)] int[] rgDispId);
        }
    }


    /// <summary>
    /// https://stackoverflow.com/questions/4159843/how-to-enumerate-members-of-com-object-in-c/14208030#14208030
    /// </summary>
    public static class DispatchUtility
    {
        private const int S_OK = 0; //From WinError.h
        private const int LOCALE_SYSTEM_DEFAULT = 2 << 10; //From WinNT.h == 2048 == 0x800

        public static bool ImplementsIDispatch(object obj)
        {
            bool result = obj is IDispatchInfo;
            return result;
        }

        public static Type GetType(object obj, bool throwIfNotFound)
        {
            RequireReference(obj, "obj");
            Type result = GetType((IDispatchInfo)obj, throwIfNotFound);
            return result;
        }

        public static bool TryGetDispId(object obj, string name, out int dispId)
        {
            RequireReference(obj, "obj");
            bool result = TryGetDispId((IDispatchInfo)obj, name, out dispId);
            return result;
        }

        public static object Invoke(object obj, int dispId, object[] args)
        {
            string memberName = "[DispId=" + dispId + "]";
            object result = Invoke(obj, memberName, args);
            return result;
        }

        public static object Invoke(object obj, string memberName, object[] args)
        {
            RequireReference(obj, "obj");
            Type type = obj.GetType();
            object result = type.InvokeMember(memberName,
                BindingFlags.InvokeMethod | BindingFlags.GetProperty,
                null, obj, args, null);
            return result;
        }

        private static void RequireReference<T>(T value, string name) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        private static Type GetType(IDispatchInfo dispatch, bool throwIfNotFound)
        {
            RequireReference(dispatch, "dispatch");

            Type result = null;
            int typeInfoCount;
            int hr = dispatch.GetTypeInfoCount(out typeInfoCount);
            if (hr == S_OK && typeInfoCount > 0)
            {
                dispatch.GetTypeInfo(0, LOCALE_SYSTEM_DEFAULT, out result);
            }

            if (result == null && throwIfNotFound)
            {
                // If the GetTypeInfoCount called failed, throw an exception for that.
                Marshal.ThrowExceptionForHR(hr);

                // Otherwise, throw the same exception that Type.GetType would throw.
                throw new TypeLoadException();
            }

            return result;
        }

        private static bool TryGetDispId(IDispatchInfo dispatch, string name, out int dispId)
        {
            RequireReference(dispatch, "dispatch");
            RequireReference(name, "name");

            bool result = false;

            Guid iidNull = Guid.Empty;
            int hr = dispatch.GetDispId(ref iidNull, ref name, 1, LOCALE_SYSTEM_DEFAULT, out dispId);

            const int DISP_E_UNKNOWNNAME = unchecked((int)0x80020006); //From WinError.h
            const int DISPID_UNKNOWN = -1; //From OAIdl.idl
            if (hr == S_OK)
            {
                result = true;
            }
            else if (hr == DISP_E_UNKNOWNNAME && dispId == DISPID_UNKNOWN)
            {
                result = false;
            }
            else
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            return result;
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("00020400-0000-0000-C000-000000000046")]
        private interface IDispatchInfo
        {
            [PreserveSig]
            int GetTypeInfoCount(out int typeInfoCount);

            void GetTypeInfo(int typeInfoIndex, int lcid, [MarshalAs(UnmanagedType.CustomMarshaler,
            MarshalTypeRef = typeof(System.Runtime.InteropServices.CustomMarshalers.TypeToTypeInfoMarshaler))] out Type typeInfo);

            [PreserveSig]
            int GetDispId(ref Guid riid, ref string name, int nameCount, int lcid, out int dispId);

            // NOTE: The real IDispatch also has an Invoke method next, but we don't need it.
        }
    }
}