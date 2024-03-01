using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

namespace Rusle2API
{
    /// <summary>
    /// Summary description for Controller
    /// </summary>
    [WebService(Namespace = "http://localhost:44334/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  //  [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Controller : System.Web.Services.WebService
    {
       // private static Logger logger = LogManager.GetCurrentClassLogger();

        [WebMethod]
        public List<string> singleRun(string stateId,string CLIMATE_PTR,string SOIL_PTR, string MAN_BASE_PTR, string SLOPE_STEEP, string MajCompName,
            string MAN_BASE_Name, string PrimaryTillageName, string SecondaryTillageName)
        {
            List<string> attributeValues = new List<string>();
            DataTable dataTable = new DataTable("Data");
           
            try
            {
                IntPtr pRome = IntPtr.Zero;
                IntPtr pAppPath = IntPtr.Zero;
                IntPtr pFiles = IntPtr.Zero;
                IntPtr pEngine = IntPtr.Zero;
                IntPtr pDb = IntPtr.Zero;
                IntPtr pProfile = IntPtr.Zero;
                int dbOpen = 0;
                string dbUrl = "";
                string soildbUrl = "";

                string appDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";

                if (stateId != "")
                {
                    dbUrl = Path.Combine(appDirectory + "dll\\", stateId + "-Moses-2016.gdb");
                    soildbUrl = Path.Combine(appDirectory + "dll\\", stateId + "-Moses-2016.gdb");
                }

                if (!System.IO.File.Exists(dbUrl))
                {
                    dbUrl = Path.Combine(appDirectory + "dll\\", "MOSES 2016_Orig.gdb");
                    soildbUrl = Path.Combine(appDirectory + "dll\\", "MOSES 2016_Orig.gdb");
                }

                // dbUrl = "dll\\MOSES 2016_Orig.gdb";
                if (!System.IO.File.Exists(dbUrl))
                {
                    Log.LogMessage("DB File not found: " + dbUrl);
                    return null;
                }
                string finalsoil_ptr = "";
                string finalman_ptr = "";

                if (SOIL_PTR != "")
                {
                   // finalsoil_ptr = getmangptr(soildbUrl, MAN_BASE_PTR, MAN_BASE_NAME);
                    finalsoil_ptr = getsoilptr(soildbUrl, SOIL_PTR.TrimStart().TrimEnd(), MajCompName.TrimStart().TrimEnd());
                }

                if (MAN_BASE_PTR != "")
                {
                    // finalsoil_ptr = getmangptr(soildbUrl, MAN_BASE_PTR, MAN_BASE_NAME);
                    finalman_ptr = getmangptr(soildbUrl, MAN_BASE_PTR.TrimStart().TrimEnd(), MAN_BASE_Name.TrimStart().TrimEnd(), PrimaryTillageName.TrimStart().TrimEnd(), SecondaryTillageName.TrimStart().TrimEnd());
                }

                if (finalsoil_ptr == "")
                {
                    finalsoil_ptr = @"soils\default";
                }

                if (finalman_ptr == "")
                {
                    finalman_ptr = @"managements\Continuously tilled and smoothed";
                }

                if (pRome == IntPtr.Zero)
                {
                    Log.LogMessage("start");
                    pRome = RomeApiProxy.RomeInit(null);
                    Log.LogMessage("ptrRome: " + pRome.ToString());
                   
                    if (pRome == IntPtr.Zero)
                    {
                        logApiError(pRome);
                        return null;
                    }
                }


                int scienceVer = RomeApiProxy.RomeGetScienceVersion(pRome);

                if (pAppPath == IntPtr.Zero)
                {
                    pAppPath = RomeApiProxy.RomeGetPropertyStr(pRome, 3);
                    if (pAppPath == IntPtr.Zero)
                    {
                        logApiError(pRome);
                        return null;
                    }
                    var strAppPath = Marshal.PtrToStringAnsi(pAppPath);
                    Log.LogMessage("strAppPath: " + strAppPath);
                }

                //files = rome.get_files(core)
                if (pFiles == IntPtr.Zero)
                {
                    pFiles = RomeApiProxy.RomeGetFiles(pRome);
                    Log.LogMessage("pFiles: " + pFiles.ToString());
                    if (pFiles == IntPtr.Zero)
                    {
                        logApiError(pRome);
                        return null;
                    }
                }
                //engine = rome.get_engine(core)

                if (pEngine == IntPtr.Zero)
                {
                    pEngine = RomeApiProxy.RomeGetEngine(pRome);
                    Log.LogMessage("pEngine: " + pEngine.ToString());
                    if (pEngine == IntPtr.Zero)
                    {
                        logApiError(pRome);
                        return null;
                    }
                }

                //database = rome.get_database(core)
                if (pDb == IntPtr.Zero)
                {
                    pDb = RomeApiProxy.RomeGetDatabase(pRome);
                    Log.LogMessage("pDb: " + pDb.ToString());
                    if (pDb == IntPtr.Zero)
                    {
                        logApiError(pRome);
                        return null;
                    }
                }

                //        //  dbUrl = ConfigurationManager.AppSettings["db"];
               

                if (dbOpen == 0)
                {
                    dbOpen = RomeApiProxy.RomeDatabaseOpen(pDb, dbUrl);

                    Log.LogMessage("dbOpen: " + dbOpen);
                    if (dbOpen == -1)
                    {
                        logApiError(pRome);
                        return null;
                    }
                    Log.LogMessage("Using DB file: " + dbUrl);
                }

                RomeApiProxy.RomeEngineSetAutorun(pEngine, 0);

                if (pProfile == IntPtr.Zero)
                {
                    UInt32 nFlag = 4;
                    pProfile = RomeApiProxy.RomeFilesOpen(pFiles, "profiles\\Potential Erodibility Profile Template", nFlag);
                    Log.LogMessage("pProfile: " + pProfile.ToString());
                    if (pProfile == IntPtr.Zero)
                    {
                        logApiError(pRome);
                        return null;
                    }
                }

               
                RomeApiProxy.RomeFileSetAttrValue(pProfile, "CLIMATE_PTR", CLIMATE_PTR.TrimStart().TrimEnd(), 0);
                RomeApiProxy.RomeFileSetAttrValue(pProfile, "SOIL_PTR", finalsoil_ptr.TrimStart().TrimEnd(), 0);
                RomeApiProxy.RomeFileSetAttrValue(pProfile, "SLOPE_HORIZ", "150", 0);
                RomeApiProxy.RomeFileSetAttrValue(pProfile, "SLOPE_STEEP", SLOPE_STEEP.TrimStart().TrimEnd(), 0);
                RomeApiProxy.RomeFileSetAttrValue(pProfile, "MAN_BASE_PTR", finalman_ptr.TrimStart().TrimEnd(), 0);

                int engineRun = RomeApiProxy.RomeEngineRun(pEngine);
                if (engineRun == -1)
                {
                    logApiError(pRome);
                    return null;
                }

                //rome.engine_finish_updates(engine)
                int finishUpdates = RomeApiProxy.RomeEngineFinishUpdates(pEngine);
                //logger.Info("finishUpdates: " + finishUpdates);
                if (finishUpdates == -1)
                {
                    logApiError(pRome);
                    return null;
                }

                logAttr(pProfile, "SLOPE_DETACH");
                logAttr(pProfile, "FN_SLOPE_EROD_FRAC_SOIL_LOSS");
                logAttr(pProfile, "SLOPE_DELIVERY");
                logAttr(pProfile, "SLOPE_DEGRAD");
                logAttr(pProfile, "NET_C_FACTOR");
                logAttr(pProfile, "NET_K_FACTOR");
                logAttr(pProfile, "NET_LS_FACTOR");
                logAttr(pProfile, "SLOPE_EVENT_RUNOFF");
                logAttr(pProfile, "SOIL_PTR");
                logAttr(pProfile, "MAN_BASE_PTR");


                // Check if selected parameters were used.  If parameters are passed to the DLL which are not the the current DB, they will not be used.  
                // THe program will use default value
                Log.LogMessage("------------------------------------------------");
                String strClimate = getAttr(pProfile, "CLIMATE_PTR");
                Log.LogMessage("strClimate: " + strClimate);

                String strMangmt = getAttr(pProfile, "MAN_BASE_PTR");
                Log.LogMessage("strMangmt: " + strMangmt);

                String strSoil = getAttr(pProfile, "SOIL_PTR");
                Log.LogMessage("strSoil: " + strSoil);


                Log.LogMessage("Done.");

                string[] attributeNames = {
                    "SLOPE_DETACH",
                    "FN_SLOPE_EROD_FRAC_SOIL_LOSS",
                    "SLOPE_DELIVERY",
                    "SLOPE_DEGRAD",
                    "NET_C_FACTOR",
                    "NET_K_FACTOR",
                    "NET_LS_FACTOR",
                    "SLOPE_EVENT_RUNOFF",
                    "SOIL_PTR",
                    "MAN_BASE_PTR",
                    "CLIMATE_PTR"
                };

                //List<string> attributeValues = new List<string>();
                //DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Attribute", typeof(string));
                dataTable.Columns.Add("Value", typeof(string));

                foreach (string attrName in attributeNames)
                {
                    string attrValue = getAttr(pProfile, attrName);
                    attributeValues.Add(attrName + " :" + attrValue);
                }
                // Set the appropriate path for pFiles
                string res = "API run success";
                return attributeValues;

            }
            catch(Exception ex)
            {
                //DataTable errorTable = new DataTable();
                //dataTable.Columns.Add("ErrorMessage");
                //DataRow errorRow = dataTable.NewRow();
                //errorRow["ErrorMessage"] = ex.Message;
                Log.LogMessage(ex.Message.ToString());
                //dataTable.Rows.Add(errorRow);
                attributeValues.Add(ex.Message);
                 return attributeValues;

                // return ex.Message.ToString();
            }
        }

        private void logApiError(IntPtr pRome)
        {
            IntPtr pApError = RomeApiProxy.RomeGetLastError(pRome);
            var strError = Marshal.PtrToStringAnsi(pApError);
            Log.LogMessage("strError: " + strError);
        }

        String getAttr(IntPtr profile, String attrName)
        {
            IntPtr ptr = RomeApiProxy.RomeFileGetAttrValue(profile, attrName, 0);
            if (ptr == IntPtr.Zero)
            {
                Log.LogMessage("Error retrieving attr: " + attrName);
                return null;
            }
            var str = Marshal.PtrToStringAnsi(ptr);
            return str.ToString();
        }

        void logAttr(IntPtr profile, String attrName)
        {
            Log.LogMessage(attrName + " :" + getAttr(profile, attrName));
        }

        const string SQLITE_21_HEADER = "** This file contains an SQLite 2.1 database **";

        public string getsoilptr(string fileName, string soil_ptr, string MajCompName)
        {
            try
            {

                System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                Log.LogMessage("getsoil"+ fileName + soil_ptr + MajCompName);
                string fileHeader = sr.ReadLine();
                fileHeader = fileHeader.Substring(0, 47);

                if (!SQLITE_21_HEADER.Equals(fileHeader))
                {
                    Log.LogMessage("The selected file is not SQLite 2.1 file");
                    return null;
                }

                IntPtr errBuf = IntPtr.Zero;
                IntPtr pSqlDb = SqlApiProxy.sqlite_open(fileName, 0, ref errBuf);

                if (pSqlDb == IntPtr.Zero)
                {
                    var errMsg = Marshal.PtrToStringAnsi(errBuf);
                    Log.LogMessage("errMsg: " + errMsg);
                    return null;
                }

                // Prepare statement
                String sql =
                 "select * from soils where path like '%" + soil_ptr + "%";
                // "select * from " + tableName;

                IntPtr pzTail = IntPtr.Zero;
                IntPtr ppVm = IntPtr.Zero;
                int res = SqlApiProxy.sqlite_compile(pSqlDb, sql, ref pzTail, ref ppVm, ref errBuf);

                IntPtr pN = IntPtr.Zero;
                IntPtr pazValue = IntPtr.Zero;
                IntPtr pazColName = IntPtr.Zero;


                int counter = 0;

                List<string> resultList = new List<string>();

                while (true)
                {
                    res = SqlApiProxy.sqlite_step(ppVm, ref pN, ref pazValue, ref pazColName);
                    if (res != 100) break;

                    int colCount = (int)pN;

                    IntPtr[] valArr = new IntPtr[10];
                    Marshal.Copy(pazValue, valArr, 0, colCount);

                    string[] strArr = new string[colCount]; // Create an array to hold the string values

                    for (int i = 0; i < 2; i++)
                    {
                        strArr[i] = Marshal.PtrToStringAnsi(valArr[i]);
                    }

                    string rowValue = string.Join("\\", strArr); // Combine the string values with "\\" separator
                    rowValue = rowValue.TrimEnd('\\').Replace("&per;", "%");
                    resultList.Add(rowValue);

                    counter++;
                }

                // Convert the list of results to a string array
                string[] resultArray = resultList.ToArray();
                string finalresult = resultArray[0];
                Log.LogMessage("res" + resultArray);
                if (resultArray.Length > 1)
                {
                    finalresult = filterbyname(fileName, soil_ptr, MajCompName);
                }


                res = SqlApiProxy.sqlite_finalize(ppVm, ref errBuf);

                // Close DB
                SqlApiProxy.sqlite_close(pSqlDb);

                if (finalresult == "")
                {
                    finalresult = @"soils\default";
                }
                return finalresult;
            }
            catch (Exception ex)
            {
                Log.LogMessage("Exception: Controller.run(): " + ex.ToString());
                return "";
            }
        }

        public string getmangptr(string fileName, string path, string name, string PrimaryTillageName, string SecondaryTillage)
        {
            try
            {

                System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                string fileHeader = sr.ReadLine();
                fileHeader = fileHeader.Substring(0, 47);

                if (!SQLITE_21_HEADER.Equals(fileHeader))
                {
                    Log.LogMessage("The selected file is not SQLite 2.1 file");
                    return null;
                }

                IntPtr errBuf = IntPtr.Zero;
                IntPtr pSqlDb = SqlApiProxy.sqlite_open(fileName, 0, ref errBuf);

                if (pSqlDb == IntPtr.Zero)
                {
                    var errMsg = Marshal.PtrToStringAnsi(errBuf);
                    Log.LogMessage("errMsg: " + errMsg);
                    return null;
                }

                // Prepare statement
                  String sql =
                // "select * from managements where path name like '%managements\\CMZ01\\CMZ 01\\a.Single Year/Single Crop Templates\\Soybeans narrow row\\Soybeans; nr%";
                // "select * from managements where path like '%CMZ 01\\a.Single Year/Single Crop Templates\\Soybeans narrow row%' and name like '%soybeans; nr, NT z1%'";
                // "select * from managements where path like '%CMZ 01\\a.Single Year/Single Crop Templates\\Soybeans narrow row%' and name like '%soybeans; nr, SC, //twist%'";
                 "select * from managements where path like '%" + path + "%' AND name like '%" + name + "%' AND name like '%" + PrimaryTillageName + "%' AND  name like '%" + SecondaryTillage + "%'";


                IntPtr pzTail = IntPtr.Zero;
                IntPtr ppVm = IntPtr.Zero;
                int res = SqlApiProxy.sqlite_compile(pSqlDb, sql, ref pzTail, ref ppVm, ref errBuf);
                if (res != 0)
                {
                    // Handle the error, log the errBuf, or take appropriate action
                    var errMsg = Marshal.PtrToStringAnsi(errBuf);
                    Log.LogMessage("errMsg: " + errMsg);

                }
                else
                {
                   
                }
                IntPtr pN = IntPtr.Zero;
                IntPtr pazValue = IntPtr.Zero;
                IntPtr pazColName = IntPtr.Zero;

                int counter = 0;

               // var txtFile = new StringBuilder();

                List<string> resultList = new List<string>();

                while (true)
                {
                    res = SqlApiProxy.sqlite_step(ppVm, ref pN, ref pazValue, ref pazColName);
                    if (res != 100) break;

                    int colCount = (int)pN;

                    IntPtr[] valArr = new IntPtr[10];
                    Marshal.Copy(pazValue, valArr, 0, colCount);

                    string[] strArr = new string[colCount]; // Create an array to hold the string values

                    for (int i = 0; i < 2; i++)
                    {
                        strArr[i] = Marshal.PtrToStringAnsi(valArr[i]);
                    }

                    string rowValue = string.Join("\\", strArr); // Combine the string values with "\\" separator
                    resultList.Add(rowValue.TrimEnd('\\'));
                  //  txtFile.AppendLine(rowValue.TrimEnd('\\'));
                    counter++;
                }

                //string fileName1 = @"C:\Project\ND.txt";
                //System.IO.File.WriteAllText(fileName1, txtFile.ToString());

                // Convert the list of results to a string array
                string[] resultArray = resultList.ToArray();
                string finalresult ="";
                if (resultArray != null && resultArray.Length > 0)
                {
                    finalresult = resultArray[0];
                }
               
                if (resultArray.Length > 1)
                {
                    //finalresult = getmanfilterbySecondaryTillagename(fileName, path, name, PrimaryTillageName, SecondaryTillage);
                   
                        List<string> matchingStrings = resultList.Where(s => s.Replace(" ", "").Contains(SecondaryTillage)).OrderBy(s => s.Length).Take(1).ToList();

                        if (matchingStrings.Count != 0)
                        {
                            finalresult = matchingStrings[0];
                        }
                    else
                    {
                        finalresult = resultList[0];
                    }
                }
                
                if (finalresult == "")
                {
                    finalresult = getmanfilterbonlyprimarytillage(fileName,path,name, PrimaryTillageName);
                }

                res = SqlApiProxy.sqlite_finalize(ppVm, ref errBuf);

                // Close DB
                SqlApiProxy.sqlite_close(pSqlDb);

                return finalresult;
            }
            catch (Exception ex)
            {
                Log.LogMessage("Exception: Controller.run(): " + ex.ToString());
                return "";
            }
        }


        private string getmanfilterbonlyprimarytillage(string fileName, string path, string name, string PrimaryTillageName)
        {
            try
            {
                Log.LogMessage("Using db: " + fileName);

                System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                string fileHeader = sr.ReadLine();
                fileHeader = fileHeader.Substring(0, 47);

                if (!SQLITE_21_HEADER.Equals(fileHeader))
                {
                    Log.LogMessage("The selected file is not SQLite 2.1 file");
                    return null;
                }

                IntPtr errBuf = IntPtr.Zero;
                IntPtr pSqlDb = SqlApiProxy.sqlite_open(fileName, 0, ref errBuf);

                if (pSqlDb == IntPtr.Zero)
                {
                    var errMsg = Marshal.PtrToStringAnsi(errBuf);
                    Log.LogMessage("errMsg: " + errMsg);
                    return null;
                }

                // Prepare statement
                string sql = "select * from managements where path like '%" + path + "%' AND name like '%" + name + "%' AND name like '%" + PrimaryTillageName + "%'";
                 //" AND name like '%" + SecondaryTillage + "%'"; 
                // String sql =
                //// "select * from managements where path like '%" + path + "%' AND name like '%" + name + "%' AND name like '%" + PrimaryTillageName + "%'" +
                //// " AND REPLACE(name, ' ', '') like '%" + SecondaryTillage + "%'";
                //"SELECT * FROM managements WHERE path LIKE '%" + path + "%' AND name LIKE '%" + name + "%' AND name LIKE '%" + PrimaryTillageName + "%' AND REPLACE(name, ' ', '') LIKE '%" + SecondaryTillage + "%'";

                //// "select * from managements where path like '%" + path + "%' AND name like '%" + name + "%' AND name like '%" + PrimaryTillageName + "%'" +
                ////" AND name like '%" + SecondaryTillage + "%'";


                IntPtr pzTail = IntPtr.Zero;
                IntPtr ppVm = IntPtr.Zero;
                int  res = SqlApiProxy.sqlite_compile(pSqlDb, sql, ref pzTail, ref ppVm, ref errBuf);
               

                IntPtr pN = IntPtr.Zero;
                IntPtr pazValue = IntPtr.Zero;
                IntPtr pazColName = IntPtr.Zero;

                int counter = 0;

                List<string> resultList = new List<string>(); // Create a list to store the results

                while (true)
                {
                    res = SqlApiProxy.sqlite_step(ppVm, ref pN, ref pazValue, ref pazColName);
               
                    if (res != 100) break;

                    int colCount = (int)pN;

                    IntPtr[] valArr = new IntPtr[10];
                    Marshal.Copy(pazValue, valArr, 0, colCount);

                    string[] strArr = new string[colCount]; // Create an array to hold the string values

                    for (int i = 0; i < 2; i++)
                    {
                        strArr[i] = Marshal.PtrToStringAnsi(valArr[i]);
                    }

                    string rowValue = string.Join("\\", strArr);
                    rowValue = rowValue.TrimEnd('\\').Replace("&per;", "%");
                    resultList.Add(rowValue);

                    counter++;
                }

                string[] resultArray = resultList.ToArray();
                string finalmanString = "";

                if (resultArray != null && resultArray.Length > 0)
                {
                    finalmanString = resultArray[0];
                }

                if (finalmanString == "")
                {
                    finalmanString = getmanfilterbonlycrop(fileName, path, name);
                }


                return finalmanString;
            }
            catch (Exception ex)
            {
                Log.LogMessage("Exception: Controller.run(): " + ex.ToString());
                return ex.ToString();
            }
        }


        private string getmanfilterbonlycrop(string fileName, string path, string name)
        {
            try
            {
                Log.LogMessage("Using db: " + fileName);

                System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                string fileHeader = sr.ReadLine();
                fileHeader = fileHeader.Substring(0, 47);

                if (!SQLITE_21_HEADER.Equals(fileHeader))
                {
                    Log.LogMessage("The selected file is not SQLite 2.1 file");
                    return null;
                }

                IntPtr errBuf = IntPtr.Zero;
                IntPtr pSqlDb = SqlApiProxy.sqlite_open(fileName, 0, ref errBuf);

                if (pSqlDb == IntPtr.Zero)
                {
                    var errMsg = Marshal.PtrToStringAnsi(errBuf);
                    Log.LogMessage("errMsg: " + errMsg);
                    return null;
                }

                // Prepare statement
                string sql = "select * from managements where path like '%" + path + "%' AND name like '%" + name + "%'";
                //" AND name like '%" + SecondaryTillage + "%'"; 
                // String sql =
                //// "select * from managements where path like '%" + path + "%' AND name like '%" + name + "%' AND name like '%" + PrimaryTillageName + "%'" +
                //// " AND REPLACE(name, ' ', '') like '%" + SecondaryTillage + "%'";
                //"SELECT * FROM managements WHERE path LIKE '%" + path + "%' AND name LIKE '%" + name + "%' AND name LIKE '%" + PrimaryTillageName + "%' AND REPLACE(name, ' ', '') LIKE '%" + SecondaryTillage + "%'";

                //// "select * from managements where path like '%" + path + "%' AND name like '%" + name + "%' AND name like '%" + PrimaryTillageName + "%'" +
                ////" AND name like '%" + SecondaryTillage + "%'";


                IntPtr pzTail = IntPtr.Zero;
                IntPtr ppVm = IntPtr.Zero;
                int res = SqlApiProxy.sqlite_compile(pSqlDb, sql, ref pzTail, ref ppVm, ref errBuf);


                IntPtr pN = IntPtr.Zero;
                IntPtr pazValue = IntPtr.Zero;
                IntPtr pazColName = IntPtr.Zero;

                int counter = 0;

                List<string> resultList = new List<string>(); // Create a list to store the results

                while (true)
                {
                    res = SqlApiProxy.sqlite_step(ppVm, ref pN, ref pazValue, ref pazColName);

                    if (res != 100) break;

                    int colCount = (int)pN;

                    IntPtr[] valArr = new IntPtr[10];
                    Marshal.Copy(pazValue, valArr, 0, colCount);

                    string[] strArr = new string[colCount]; // Create an array to hold the string values

                    for (int i = 0; i < 2; i++)
                    {
                        strArr[i] = Marshal.PtrToStringAnsi(valArr[i]);
                    }

                    string rowValue = string.Join("\\", strArr);
                    rowValue = rowValue.TrimEnd('\\').Replace("&per;", "%");
                    resultList.Add(rowValue);

                    counter++;
                }

                string[] resultArray = resultList.ToArray();
                string finalmanString = "";
                if (resultArray.Length > 0)
                {
                    finalmanString = resultArray[0];
                }

                return finalmanString;
            }
            catch (Exception ex)
            {
                Log.LogMessage("Exception: Controller.run(): " + ex.ToString());
                return ex.ToString();
            }
        }

        private string filterbyname(string fileName, string soil_ptr, string MajCompName)
        {
            try
            {
                Log.LogMessage("Using db: " + fileName);

                System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                string fileHeader = sr.ReadLine();
                fileHeader = fileHeader.Substring(0, 47);

                if (!SQLITE_21_HEADER.Equals(fileHeader))
                {
                    Log.LogMessage("The selected file is not SQLite 2.1 file");
                    return null;
                }

                IntPtr errBuf = IntPtr.Zero;
                IntPtr pSqlDb = SqlApiProxy.sqlite_open(fileName, 0, ref errBuf);

                if (pSqlDb == IntPtr.Zero)
                {
                    var errMsg = Marshal.PtrToStringAnsi(errBuf);
                    Log.LogMessage("errMsg: " + errMsg);
                    return null;
                }

                // Prepare statement
                String sql =
                "select * from soils where path like '%"+ soil_ptr + "%' AND name like '%"+MajCompName+"%'";

                IntPtr pzTail = IntPtr.Zero;
                IntPtr ppVm = IntPtr.Zero;
                int res = SqlApiProxy.sqlite_compile(pSqlDb, sql, ref pzTail, ref ppVm, ref errBuf);

                IntPtr pN = IntPtr.Zero;
                IntPtr pazValue = IntPtr.Zero;
                IntPtr pazColName = IntPtr.Zero;

                int counter = 0;

                List<string> resultList = new List<string>(); // Create a list to store the results

                while (true)
                {
                    res = SqlApiProxy.sqlite_step(ppVm, ref pN, ref pazValue, ref pazColName);
                    if (res != 100) break;

                    int colCount = (int)pN;

                    IntPtr[] valArr = new IntPtr[10];
                    Marshal.Copy(pazValue, valArr, 0, colCount);

                    string[] strArr = new string[colCount]; // Create an array to hold the string values

                    for (int i = 0; i < 2; i++)
                    {
                        strArr[i] = Marshal.PtrToStringAnsi(valArr[i]);
                    }

                    string rowValue = string.Join("\\", strArr); 
                    rowValue = rowValue.TrimEnd('\\').Replace("&per;", "%");
                    resultList.Add(rowValue);

                    counter++;
                }

                string highestPercentageString = resultList[0];
                if (resultList.Count > 1)
                {
                    highestPercentageString = GetHighestPercentageString(resultList.ToArray());
                }


               return highestPercentageString;
            }
            catch (Exception ex)
            {
                Log.LogMessage("Exception: Controller.run(): " + ex.ToString());
                return ex.ToString();
            }
        }

        static string GetHighestPercentageString(string[] soilData)
        {
            string highestPercentageString = null;
            int highestPercentage = -1;

            foreach (string soilInfo in soilData)
            {
                // Split the string by spaces
                string[] parts = soilInfo.Split(' ');

                // Get the last part which should be the percentage
                string percentageStr = parts.LastOrDefault();

                if (percentageStr != null && int.TryParse(percentageStr.Trim('%'), out int percentage))
                {
                    if (percentage > highestPercentage)
                    {
                        highestPercentage = percentage;
                        highestPercentageString = soilInfo;
                    }
                }
            }

            return highestPercentageString;
        }
    }
}
