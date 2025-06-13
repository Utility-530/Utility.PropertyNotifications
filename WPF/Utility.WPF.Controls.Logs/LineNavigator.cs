using System;

namespace openStudioFileLine
{
    using EnvDTE;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using System.Security;

    public static class LineNavigator
    {
        //static void Main(string filename, int fileline)
        //{
        //    try
        //    {
        //        EnvDTE80.DTE2 dte2;
        //        dte2 = (EnvDTE80.DTE2)Marshal2.GetActiveObject("VisualStudio.DTE");
        //        dte2.MainWindow.Activate();
        //        EnvDTE.Window w = dte2.ItemOperations.OpenFile(filename, EnvDTE.Constants.vsViewKindTextView);
        //        ((EnvDTE.TextSelection)dte2.ActiveDocument.Selection).GotoLine(fileline, true);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write(e.Message);
        //    }
        //}

        public static void OpenFileAtLine(string filePath, int line)
        {
            DTE dte = (DTE)Marshal2.GetActiveObject("VisualStudio.DTE");
            dte.MainWindow.Visible = true;
            dte.ExecuteCommand("Edit.OpenFile", $"\"{filePath}\"");
            dte.ExecuteCommand("Edit.GoTo", line.ToString());
        }
    }

    public static class Marshal2
    {
        internal const String OLEAUT32 = "oleaut32.dll";
        internal const String OLE32 = "ole32.dll";

        [System.Security.SecurityCritical]  // auto-generated_required
        public static Object GetActiveObject(String progID)
        {
            Object obj = null;
            Guid clsid;

            // Call CLSIDFromProgIDEx first then fall back on CLSIDFromProgID if
            // CLSIDFromProgIDEx doesn't exist.
            try
            {
                CLSIDFromProgIDEx(progID, out clsid);
            }
            //            catch
            catch (Exception)
            {
                CLSIDFromProgID(progID, out clsid);
            }

            GetActiveObject(ref clsid, IntPtr.Zero, out obj);
            return obj;
        }

        //[DllImport(Microsoft.Win32.Win32Native.OLE32, PreserveSig = false)]
        [DllImport(OLE32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] String progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLE32, PreserveSig = false)]
        [DllImport(OLE32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] String progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLEAUT32, PreserveSig = false)]
        [DllImport(OLEAUT32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void GetActiveObject(ref Guid rclsid, IntPtr reserved, [MarshalAs(UnmanagedType.Interface)] out Object ppunk);
    }
}