using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Rusle2API
{
    public class SqlApiProxy
    {
        // public const string DLL_URL = @"C:\\Project\\Rusle2API 3.5\\Rusle2API\\Rusle2API\\bin\\dll\\sqlite.dll";
        public const string DLL_URL = "dll\\sqlite.dll";

        //extern "C" char   IMPEXP __stdcall service_GetParameter ( const char* parameter, const int value_lenght, char** value );
        //public static extern sbyte service_GetParameter ( String parameter, Int32 length, ref IntPtr val);

        //--------------------------------------------------------------------------------------
        // sqlite *sqlite_open(const char *filename, int mode, char **errmsg);
        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sqlite_open(String fileName, Int32 mode, ref IntPtr errMsg);


        //void sqlite_close(sqlite* db);
        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sqlite_close(IntPtr dbPtr);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
       public static extern int sqlite_column_text( IntPtr ppVm, int errMsg);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        /* int sqlite_compile(
            sqlite* db,              The open database 
            const char* zSql,        SQL statement to be compiled
            const char** pzTail,     OUT: uncompiled tail of zSql
            sqlite_vm **ppVm,        OUT: the virtual machine to execute zSql 
            char** pzErrmsg          OUT: Error message. 
        */
        public static extern int sqlite_compile(IntPtr dbPtr, String sql,
            ref IntPtr pzTail, ref IntPtr ppVm, ref IntPtr errMsg);

        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        /*  int sqlite_step(
          sqlite_vm* pVm,          The virtual machine to execute
          int* pN,                 OUT: Number of columns in result
          const char*** pazValue,  OUT: Column data
          const char*** pazColName OUT: Column names and datatypes
          */
        public static extern int sqlite_step(IntPtr ppVm, ref IntPtr pN, ref IntPtr pazValue, ref IntPtr pazColName);
        [DllImport(DLL_URL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        /* int sqlite_finalize(
            sqlite_vm* pVm,           The virtual machine to be finalized 
            char** pzErrMsg           OUT: Error message 
        */
        public static extern int sqlite_finalize(IntPtr ppVm, ref IntPtr errMsg);
    }
}