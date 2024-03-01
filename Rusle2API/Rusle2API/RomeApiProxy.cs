using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Rusle2API
{
    public class RomeApiProxy
    {

        // static string appDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
        // public const string DLL_URL2 = HttpContext.Current.Server.MapPath("~/bin/dll/RomeDLL.dll");
       // string DLL_URL1 = Path.Combine(appDirectory + "dll\\", "RomeDLL.dll");
        public const string DLL_URL = "dll\\RomeDLL.dll";
        // public const string DLL_URL = @"C:\\Project\\Rusle2API 3.5\\Rusle2API\\Rusle2API\\RomeDLL.dll";
        // public const string DLL_URL1 = DLL_URL;


        public const string DLL_URL2 = "~/bin/dll/RomeDLL.dll";

        //--------------------------------------------------------------------------------------
        // Rome 
        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RomeInit(String args);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RomeExit(IntPtr pApp);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RomeGetTitle(IntPtr pApp);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RomeGetScienceVersion(IntPtr pApp);


        //--------------------------------------------------------------------------------------
        // Engine
        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RomeGetEngine(IntPtr pApp);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RomeEngineRun(IntPtr pEngine);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RomeEngineSetAutorun(IntPtr pEngine, int bAutorun);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RomeEngineFinishUpdates(IntPtr pEngine);

        //--------------------------------------------------------------------------------------
        // Database 
        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RomeGetDatabase(IntPtr pApp);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RomeDatabaseOpen(IntPtr pDb, [MarshalAs(UnmanagedType.LPStr)] string dbName);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RomeDatabaseClose(IntPtr pDb, [MarshalAs(UnmanagedType.LPStr)] string dbName);

        //--------------------------------------------------------------------------------------
        // Files
        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RomeGetFiles(IntPtr pApp);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RomeFilesOpen(IntPtr pFiles, [MarshalAs(UnmanagedType.LPStr)] string dbName, UInt32 nFlags);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RomeFilesCloseAll(IntPtr pFiles, int nFlags);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern string RomeDatabaseFileInfo(IntPtr pDb, string strDbBame, int intInfoType);

        //--------------------------------------------------------------------------------------
        // Properties
        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RomeGetPropertyStr(IntPtr pApp, int intProp);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RomeFileSetAttrValue(IntPtr pProfile,
            [MarshalAs(UnmanagedType.LPStr)] string attrName,
            [MarshalAs(UnmanagedType.LPStr)] string attrValue,
            int nIndex);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RomeFileGetAttrValue(IntPtr pProfile,
                [MarshalAs(UnmanagedType.LPStr)] string attrName,
                int nIndex);

        //--------------------------------------------------------------------------------------
        // Misc
        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RomeGetLastError(IntPtr pApp);
    }
}