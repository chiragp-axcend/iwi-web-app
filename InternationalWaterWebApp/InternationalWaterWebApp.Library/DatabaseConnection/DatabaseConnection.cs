using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternationalWaterWebApp.Library.DatabaseConnection
{
    public static class DatabaseConnection
    {
        public static string connectionString = string.Empty;
    }

    public static class RootDirectoryPath
    {
        public static string SiteURL = string.Empty;
        public static string ContentRootPath = string.Empty;
        public static string WebRootPath = string.Empty;
        
    }
    public static class DefaultValueFromWebConfig
    {
        public static string SMTPSERVER { get; set; }
        public static string SMTPUsername { get; set; }
        public static string SMTPPassword { get; set; }
        public static string SMTPSSL { get; set; }
        public static string SMTPPort { get; set; }
        public static string EmailFrom { get; set; }
        public static string ExcelConString { get; set; }
        public static string Rusle2APIURL = string.Empty;
    }
}