using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Wordprocessing;
using ExcelDataReader;
using InternationalWaterWebApp.Library.Common;
using InternationalWaterWebApp.Library.DatabaseConnection;
using InternationalWaterWebApp.Library.ViewModel;
using InternationalWaterWebApp.Models;
using InternationalWaterWebApp.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Nancy.Json;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;
using System.Text;
using DataTable = System.Data.DataTable;

namespace InternationalWaterWebApp.Controllers
{
    public class WebAppController : BaseController
    {
        private CommonMethods commonMethods;
        public WebAppController()
        {
            commonMethods = new CommonMethods();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            ClaimsPrincipal User = HttpContext.User;
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        //[Route("Login")]
        public async Task<IActionResult> Login(User model, bool RememberMe = false)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            try
            {
                object[] userParams = { "_ActionType", "_email", "_password" };
                object[] userValues = { "LoginUser", model.Email, model.Password };
                DataSet dsUserData = _masterRepository.GetAndSelectTableItems($"SELECT * from \"Users\".water_getallusers_ref (datarefcursor := 'data',{userParams[0]} => '{userValues[0]}', {userParams[1]} => '{userValues[1]}',{userParams[2]} => '{userValues[2]}');");

                if (dsUserData != null && dsUserData.Tables[0].Rows.Count > 0)
                {
                    User user = new User
                    {
                        Id = (int)dsUserData.Tables[0].Rows[0]["login_id"],
                        UserName = Convert.ToString(dsUserData.Tables[0].Rows[0]["username"]),
                        Password = Convert.ToString(dsUserData.Tables[0].Rows[0]["password"]),
                        FirstName = Convert.ToString(dsUserData.Tables[0].Rows[0]["first_name"]),
                        LastName = Convert.ToString(dsUserData.Tables[0].Rows[0]["last_name"]),
                        Email = Convert.ToString(dsUserData.Tables[0].Rows[0]["email"]),
                        UserRole = (int)dsUserData.Tables[0].Rows[0]["roleid"],
                        UType = Convert.ToString(dsUserData.Tables[0].Rows[0]["rolename"]),
                        AdvisorId = dsUserData.Tables[0].Rows[0]["advisor_id"] is int ? (int)dsUserData.Tables[0].Rows[0]["advisor_id"] : 0
                    };

                    if (user != null)
                    {
                        commonMethods.CreateSession(user, RememberMe, HttpContext);

                        //if (user.IsFirstLogin)
                        //{
                        //    commonMethods.UpdateLastLogin(user.Id);
                        //    return RedirectToAction("ChangePassword", "Common");
                        //}

                        switch ((int)user.UserRole)
                        {
                            case (int)UserTypeEnum.Admin:
                                return RedirectToAction("Landing", "Admin");
                            case (int)UserTypeEnum.Producer:
                                return RedirectToAction("Landing", "Producer");
                            case (int)UserTypeEnum.Agronomist:
                                return RedirectToAction("Landing", "Advisor");
                            default:
                                TempData["Error"] = "User role is not defined.";
                                return RedirectToAction("Login", "WebApp");
                        }
                    }
                    else
                    {
                        TempData["Error"] = "User not found";
                        return RedirectToAction("Login", "WebApp"); ;
                    }
                }
                else
                {
                    TempData["Error"] = "Invalid username/password or user not found.";
                    return RedirectToAction("Login", "WebApp");
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Login", "WebApp");
            }
        }

        public IActionResult Logout()
        {
            try
            {
                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return RedirectToAction("Login", "WebApp");
        }

        #region "Admin Operations - Irr/Precip & Batch Prcocessing"

        [AuthorizationCS((int)UserTypeEnum.Admin)]
        public IActionResult AdminOperations()
        {
            try
            {
                ViewBag.FileName = (from Precipitation fileName in commonMethods.GetFileName()
                                    select new SelectListItem
                                    {
                                        Text = Convert.ToString(fileName.FileName),
                                        Value = Convert.ToString(fileName.FileName),
                                        //Value = Convert.ToString(fileName.Year),

                                    }).ToList();
                ViewBag.Producer = (from Producers producers in commonMethods.GetProducerDropDown()
                                    select new SelectListItem
                                    {
                                        Text = producers.FirstName + " " + producers.LastName,
                                        Value = Convert.ToString(producers.ProducerId)
                                    }).ToList();

                IMasterRepository _masterRepository = new MasterRepository();
                object[] userParams = { "_ActionType", "_producer_Id", "_advisor_id" };
                object[] userValues = { "AllProducers" }; //GetAllProducer
                string query = $"SELECT * from \"Users\".water_producersdata_ref (datarefcursor => 'data',{userParams[0]} => '{userValues[0]}');";
                DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(query);
                Producers producer = new Producers
                {
                    ProducerId = (int)DsProducerData.Tables[0].Rows[0]["producer_id"]
                };
                return View(producer);
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return View();
        }

        [HttpPost]
        [AuthorizationCS((int)UserTypeEnum.Admin)]
        public IActionResult ApplyPrecipAndIrrigationFile(IFormFile irrigationFile, IFormFile precipFile, [FromForm] string File_Name, int ProducerId, int Field_id, string fieldIDs, string isIrrigation, string type)
        {
            string yearsErrors = string.Empty;
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                if (irrigationFile != null)
                {
                    switch (irrigationFile.ContentType)
                    {
                        case "application/vnd.ms-excel":
                        case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                            break;
                        default:
                            return Json(new { success = false, message = "Please select a valid Excel file" });
                    }

                    string fileName = Path.GetFileName(irrigationFile.FileName);
                    string path = Path.Combine(RootDirectoryPath.WebRootPath, "IrrigationFile");

                    // Read the Excel file into a DataSet
                    using (var stream = irrigationFile.OpenReadStream())
                    {
                        // Register encoding provider
                        EncodingProvider provider = CodePagesEncodingProvider.Instance;
                        Encoding.RegisterProvider(provider);

                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true // Use the first row as column names
                                }
                            });

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            string filePath = Path.Combine(path, fileName);

                            using (var str = new FileStream(filePath, FileMode.Create))
                            {
                                irrigationFile.CopyTo(str);
                            }

                            //Exce sheet-1 data update
                            var field_irrigationDT = result.Tables[0];//[0]

                            //NOTE: Get all the years from excel and compare it to the Operation Years.
                            string yearsDTArr = string.Empty;
                            foreach (DataRow item in field_irrigationDT.DefaultView.ToTable(true, "Year").AsEnumerable())
                            {
                                if (!item.IsNull("Year"))
                                    yearsDTArr += item[0] + ",";
                            }

                            if (type == "All")
                            {
                                //NOTEL: Get all the fields and loop thruogh the FielIDs
                                foreach (string fieldId in fieldIDs.Split(","))
                                {
                                    //NOTE: Get all the op years are available for this field.
                                    string query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',datarefcursor2 =>'data2',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {fieldId},_field_option_id=>{1});";
                                    List<string> opYears = new List<string>();

                                    DataSet opYearsDT = _masterRepository.GetAndSelectMultipleTableItems(query);
                                    if (opYearsDT.Tables.Count > 0 && opYearsDT.Tables[0].Rows.Count > 0)
                                    {
                                        opYears.AddRange(opYearsDT.Tables[0].AsEnumerable()
                                            .Select(row => Convert.ToString(row.Field<int>(0)))
                                            .Where(value => !string.IsNullOrWhiteSpace(value)));
                                    }

                                    //List<string> opYears = new List<string>(); //total_years.Split(',').Select(year => year.Trim()).ToList();
                                    List<string> excelYears = yearsDTArr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                          .Select(year => year.Trim())
                                                                          .OrderBy(year => year)
                                                                          .ToList();

                                    if (!excelYears.Any(year => opYears.Contains(year)) && opYears.Count > 0)
                                        yearsErrors += "FieldId: " + fieldId + " - Current years are not matching with the years which are available in excel;";

                                    //NOTE: Check if irrigation data is available then delete it from DB.
                                    string field_irrigation_exists_or_not = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_ActionType =>'field_irrigation_GetByField_id' ,_field_id => {fieldId});";
                                    DataSet existsOrNot = _masterRepository.GetAndSelectTableItems(field_irrigation_exists_or_not);
                                    if (existsOrNot != null && existsOrNot.Tables.Count > 0 && existsOrNot.Tables[0].Rows.Count > 0)
                                    {
                                        //NOTE: Delete all the data from irrigation table for current field.
                                        string queryDelete = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actionType =>'remove_irrigationfile_data', _field_id => {fieldId});";
                                        DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(queryDelete);
                                    }

                                    //NOTE: Replace fieldid from DT and upload data in DB.
                                    foreach (DataRow row in field_irrigationDT.Rows)
                                    {
                                        row[0] = fieldId;
                                    }

                                    string worksheetDT = JsonConvert.SerializeObject(field_irrigationDT, Formatting.None);
                                    string worksheetQuery = $"SELECT * FROM \"FSRCalc\".water_uploadcalculate_fsr_Excelupload(datarefcursor := 'data',_actiontype := 'insert_irrigation_calc_fsr_data_excel',_filename := '{fileName}',jsonstring := '{worksheetDT}');";
                                    DataSet worksheetQueryReturn = _masterRepository.GetAndSelectTableItems(worksheetQuery);

                                    //if (worksheetQueryReturn != null && worksheetQueryReturn.Tables.Count != 0 && int.Parse(worksheetQueryReturn.Tables[0].Rows[0][0].ToString()) == 1)
                                    //{
                                    //    return Json(new { success = true, message = "Irrigation file uploaded Successfully." });
                                    //}
                                }
                            }
                            else
                            {
                                //NOTE: Get all the years from excel and compare it to the Operation Years.
                                foreach (DataRow item in field_irrigationDT.DefaultView.ToTable(true, "Year").AsEnumerable())
                                {
                                    if (!item.IsNull("Year"))
                                        yearsDTArr += item[0] + ",";
                                }

                                //NOTE: Get all the op years are available for this field.
                                string query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',datarefcursor2 =>'data2',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {Field_id},_field_option_id=>{1});";
                                List<string> opYears = new List<string>();

                                DataSet opYearsDT = _masterRepository.GetAndSelectMultipleTableItems(query);
                                if (opYearsDT.Tables.Count > 0 && opYearsDT.Tables[0].Rows.Count > 0)
                                {
                                    opYears.AddRange(opYearsDT.Tables[0].AsEnumerable()
                                        .Select(row => Convert.ToString(row.Field<int>(0)))
                                        .Where(value => !string.IsNullOrWhiteSpace(value)));
                                }

                                List<string> excelYears = yearsDTArr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                      .Select(year => year.Trim())
                                                                      .OrderBy(year => year)
                                                                      .ToList();

                                if (!excelYears.Any(year => opYears.Contains(year)) && opYears.Count > 0)
                                    yearsErrors += "FieldId: " + Field_id + " - Current years are not matching with the years which are available in excel;";

                                //NOTE: Compare selcted fielId data are available in excel.
                                var IrrExcelField_Id = Convert.ToInt32(field_irrigationDT.Rows[0]["Field_id"]);
                                string field_irrigation_exists_or_not = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_ActionType =>'field_irrigation_GetByField_id' ,_field_id => {Field_id});";

                                DataSet existsOrNot = _masterRepository.GetAndSelectTableItems(field_irrigation_exists_or_not);
                                if (existsOrNot != null && existsOrNot.Tables.Count > 0 && existsOrNot.Tables[0].Rows.Count > 0)
                                {
                                    //NOTE: Delete all the data from irrigation table for current field.
                                    string queryDelete = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actionType =>'remove_irrigationfile_data', _field_id => {Field_id});";
                                    DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(queryDelete);
                                }

                                //NOTE: Replace fieldid from DT and upload data in DB.
                                foreach (DataRow row in field_irrigationDT.Rows)
                                {
                                    row[0] = Field_id;
                                }

                                string worksheetDT = JsonConvert.SerializeObject(field_irrigationDT, Formatting.None);
                                string worksheetQuery = $"SELECT * FROM \"FSRCalc\".water_uploadcalculate_fsr_Excelupload(datarefcursor := 'data',_actiontype := 'insert_irrigation_calc_fsr_data_excel',_filename := '{fileName}',jsonstring := '{worksheetDT}');";
                                DataSet worksheetQueryReturn = _masterRepository.GetAndSelectTableItems(worksheetQuery);

                                ////if (worksheetQueryReturn != null && worksheetQueryReturn.Tables.Count != 0 && int.Parse(worksheetQueryReturn.Tables[0].Rows[0][0].ToString()) == 1)
                                ////{
                                ////    return Json(new { success = true, message = "Irrigation file uploaded Successfully." });
                                ////}
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(File_Name))
                {
                    if (!string.IsNullOrWhiteSpace(File_Name))
                    {
                        string fileName = Path.GetFileName(File_Name);
                        if (type == "All")
                        {
                            //NOTEL: Get all the fields and loop thruogh the FieldIDs
                            foreach (string fieldId in fieldIDs.Split(","))
                            {
                                //NOTE: Assign file to the field
                                string update_query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_ActionType =>'assign_precip_file_to_field',_filename => '{fileName}',_field_id=>{fieldId});";
                                var returnData = _masterRepository.GetAndSelectTableItemsWithoutCursor(update_query);
                            }
                        }
                        else
                        {
                            //NOTE: Assign file to the field
                            string update_query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_ActionType =>'assign_precip_file_to_field',_filename => '{fileName}',_field_id=>{Field_id});";
                            var returnData = _masterRepository.GetAndSelectTableItemsWithoutCursor(update_query);
                        }
                    }
                }
                else if (precipFile != null)
                {
                    //NOTE: Common code set here
                    switch (precipFile.ContentType)
                    {
                        case "application/vnd.ms-excel":
                        case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                            break;
                        default:
                            return Json(new { success = false, message = "Please select a valid Excel file" });
                    }

                    string fileName = Path.GetFileName(precipFile.FileName);
                    string path = Path.Combine(RootDirectoryPath.WebRootPath, "PrecipFile");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string filePath = Path.Combine(path, fileName);

                    // File existing delete and new added(replace)
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        Console.WriteLine("File deleted successfully.");
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        precipFile.CopyTo(stream);
                        Console.WriteLine("New File :{0} create", fileName);
                    }
                    if (type == "All")
                    {
                        foreach (string fieldId in fieldIDs.Split(","))
                        {
                            // Read the Excel file into a DataSet
                            using (var stream = precipFile.OpenReadStream())
                            {
                                // Register encoding provider
                                EncodingProvider provider = CodePagesEncodingProvider.Instance;
                                Encoding.RegisterProvider(provider);

                                using (var reader = ExcelReaderFactory.CreateReader(stream))
                                {
                                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                    {
                                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                        {
                                            UseHeaderRow = true // Use the first row as column names
                                        }
                                    });
                                    //Exce sheet-1 data update
                                    var worksheet1 = result.Tables[0];
                                    string worksheet1DT = JsonConvert.SerializeObject(worksheet1, Formatting.None);
                                    string worksheetQuery = $"SELECT * FROM \"FSRCalc\".water_uploadcalculate_fsr_Excelupload(datarefcursor := 'data',_actiontype := 'insert_precip_calc_fsr_data_excel',_filename := '{fileName}',jsonstring := '{worksheet1DT}');";
                                    DataSet worksheetQueryReturn = _masterRepository.GetAndSelectTableItems(worksheetQuery);

                                    if (worksheetQueryReturn != null && worksheetQueryReturn.Tables.Count != 0 && int.Parse(worksheetQueryReturn.Tables[0].Rows[0][0].ToString()) == 1)
                                    {
                                        //NOTE: Assign file to the field
                                        string update_query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_ActionType =>'assign_precip_file_to_field',_filename => '{fileName}',_field_id=>{fieldId});";
                                        var returnData = _masterRepository.GetAndSelectTableItemsWithoutCursor(update_query);
                                        //return Json(new { success = true, message = "Precipitation file uploaded Successfully.", filename = fileName });
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Read the Excel file into a DataSet
                        using (var stream = precipFile.OpenReadStream())
                        {
                            // Register encoding provider
                            EncodingProvider provider = CodePagesEncodingProvider.Instance;
                            Encoding.RegisterProvider(provider);

                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                    {
                                        UseHeaderRow = true // Use the first row as column names
                                    }
                                });
                                //Exce sheet-1 data update
                                var worksheet1 = result.Tables[0];
                                string worksheet1DT = JsonConvert.SerializeObject(worksheet1, Formatting.None);
                                string worksheetQuery = $"SELECT * FROM \"FSRCalc\".water_uploadcalculate_fsr_Excelupload(datarefcursor := 'data',_actiontype := 'insert_precip_calc_fsr_data_excel',_filename := '{fileName}',jsonstring := '{worksheet1DT}');";
                                DataSet worksheetQueryReturn = _masterRepository.GetAndSelectTableItems(worksheetQuery);

                                if (worksheetQueryReturn != null && worksheetQueryReturn.Tables.Count != 0 && int.Parse(worksheetQueryReturn.Tables[0].Rows[0][0].ToString()) == 1)
                                {
                                    //NOTE: Assign file to the field
                                    string update_query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_ActionType =>'assign_precip_file_to_field',_filename => '{fileName}',_field_id=>{Field_id});";
                                    var returnData = _masterRepository.GetAndSelectTableItemsWithoutCursor(update_query);
                                    //return Json(new { success = true, message = "Precipitation file uploaded Successfully.", filename = fileName });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Json(new { success = false, yearsErrors });
            }
            if (!string.IsNullOrWhiteSpace(yearsErrors))
                return Json(new { success = true, yearsErrors });
            else
                return Json(new { success = true, yearsErrors = string.Empty, successMessage = "Data uploaded succesfully." });
        }

        #endregion

        #region "Admin Operations - Calculate Benchmark and Calculate FSR Batch Prcocessing"

        [HttpPost]
        [AuthorizationCS((int)UserTypeEnum.Admin)]
        public async Task<IActionResult> BulkBenchmarkAndCalculateFSRProcess(int ProducerId, int Field_id, string type, string options)
        {
            string Error = string.Empty;
            try
            {
                string Message = string.Empty;
                string rusle2ErrorMessage = "Rusle2 not responding";
                bool BenchmarkSuccess = false;
                bool CalcFSRSuccess = false;
                bool CalcFSRSOption1uccess = false;
                bool CalcFSRSOption2uccess = false;
                int FieldOptionId = 1; //default 1
                int Option1Id = 3; //default 1
                int Option2Id = 4; //default 1
                DataSet result = null;
                DataTable Rows = null;
                int F_id = 0;
                int Prodcr_id = 0;

                IMasterRepository _masterRepository = new MasterRepository();
                List<Dictionary<string, object>> ResponsesList = new List<Dictionary<string, object>>();
                //Apply For All Producers or  Selected Producer All Fields
                if (type == "Apply For All Producers" || type == "Apply For Selected Producer All Fields")
                {
                    if (type == "Apply For All Producers")
                    {
                        string query = $"select * from  \"FSRCalc\".water_admin_new_ref(datarefcursor => 'data',_ActionType => 'GetAllFieldsForAdmin');";
                        result = _masterRepository.GetAndSelectTableItems(query);
                    }
                    else if (type == "Apply For Selected Producer All Fields")
                    {
                        result = commonMethods.GetFieldByproducerId(ProducerId);
                    }
                    Rows = result.Tables[0];
                    Console.WriteLine($"------------------------FSR BULK PROCESS START at {DateTime.Now}--------------------");
                    foreach (DataRow row in Rows.Rows)
                    {
                        Dictionary<string, object> Rusle2ResponseList1 = new Dictionary<string, object>();
                        Dictionary<string, object> iterationResponse = new Dictionary<string, object>();
                        F_id = int.Parse(row["field_id"].ToString());
                        if ((options == "opCalculateAll" || options == "opCurrent_Benchmarks") && (type == "Apply For All Producers" || type == "Apply For Selected Producer All Fields"))
                        {
                            if (type == "Apply For All Producers")
                            {
                                Prodcr_id = int.Parse(row["producer_id"].ToString());
                            }
                            else//if (type == "Apply For Selected Producer All Fields")
                            {
                                Prodcr_id = ProducerId;
                            }
                            Console.WriteLine($"Field_id - Producer_id :  {F_id} - {Prodcr_id} - START at {DateTime.Now}");
                            //calculate benchmark
                            var Result = await commonMethods.CalculateBenchmarkCommon(F_id, FieldOptionId);
                            if (Result["success"] == "true")
                            {
                                BenchmarkSuccess = true;
                            }
                            if (Result["message"] == rusle2ErrorMessage)
                            {
                                Message = rusle2ErrorMessage;
                                break;
                            }
                            Rusle2ResponseList1.Add("Rusle2ResponseBench", JsonConvert.SerializeObject(Result["rusle2response"]));
                            //calculate FSR
                            var Result2 = await commonMethods.CalculatePreviewFSRCommon(F_id, FieldOptionId);
                            if (Result2["success"] == "true")
                            {
                                CalcFSRSuccess = true;
                            }
                            if (Result2["message"] == rusle2ErrorMessage)
                            {
                                Message = rusle2ErrorMessage;
                                break;
                            }
                        }
                        string query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',datarefcursor2 =>'data2',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {F_id});";
                        DataSet queryDs = _masterRepository.GetAndSelectMultipleTableItems(query);
                        //var IsOptionIdAvilableOrNot = queryDs.Any(item => item.field_option_id == 1);
                        bool IsOption1IdAvailable = queryDs.Tables.Count > 0 && queryDs.Tables[1].AsEnumerable().Any(row => row.Field<int>("field_option_id") == Option1Id);

                        bool IsOption2IdAvailable = queryDs.Tables.Count > 0 && queryDs.Tables[1].AsEnumerable().Any(row => row.Field<int>("field_option_id") == Option2Id);
                        if ((options == "opOptions" || options == "opCalculateAll") && (type == "Apply For All Producers" || type == "Apply For Selected Producer All Fields"))
                        {
                            if (IsOption1IdAvailable)
                            {
                                var Result3 = await commonMethods.CalculatePreviewFSRCommon(F_id, Option1Id);
                                if (Result3["success"] == "true")
                                {
                                    CalcFSRSOption1uccess = true;
                                }
                                if (Result3["message"] == rusle2ErrorMessage)
                                {
                                    Message = rusle2ErrorMessage;
                                    break;
                                }
                                Rusle2ResponseList1.Add("Rusle2ResponseOp1", JsonConvert.SerializeObject(Result3["rusle2response"]));
                            }
                            if (IsOption2IdAvailable)
                            {
                                var Result4 = await commonMethods.CalculatePreviewFSRCommon(F_id, Option2Id);
                                if (Result4["success"] == "true")
                                {
                                    CalcFSRSOption2uccess = true;
                                }
                                if (Result4["message"] == rusle2ErrorMessage)
                                {
                                    Message = rusle2ErrorMessage;
                                    break;
                                }
                                Rusle2ResponseList1.Add("Rusle2ResponseOp2", JsonConvert.SerializeObject(Result4["rusle2response"]));
                            }
                        }
                        iterationResponse.Add("BenchmarkSuccess", $"{BenchmarkSuccess}");
                        iterationResponse.Add("CalcFSRSuccess", $"{CalcFSRSuccess}");
                        iterationResponse.Add("CalcFSROption1Success", $"{CalcFSRSOption1uccess}");
                        iterationResponse.Add("CalcFSROption2Success", $"{CalcFSRSOption2uccess}");
                        iterationResponse.Add("Field_id", $"{F_id}");
                        iterationResponse.Add("Producer_id", $"{Prodcr_id}");
                        iterationResponse.Add("Rusle2ResponseListBulk", Rusle2ResponseList1);
                        ResponsesList.Add(iterationResponse);
                        Console.WriteLine($"BenchmarkSuccess: {BenchmarkSuccess}, CalcFSRSuccess: {CalcFSRSuccess}, CalcFSROption1Success: {CalcFSRSOption1uccess}, CalcFSROption2Success: {CalcFSRSOption2uccess}");
                        Console.WriteLine();
                    }
                    Console.WriteLine($"------------------------FSR BULK PROCESS END at at {DateTime.Now}--------------------");
                    if (Message == rusle2ErrorMessage)
                    {
                        return Json(new { success = false, message = Message, responsesList = ResponsesList });
                    }
                    return Json(new { success = true, message = Message, responsesList = ResponsesList });
                }

                //Apply For Selected Field
                else if (type == "Apply For Selected Field")
                {
                    Dictionary<string, object> Rusle2ResponseList = new Dictionary<string, object>();
                    Dictionary<string, object> iterationResponse = new Dictionary<string, object>();
                    Console.WriteLine($"------------------------SINGLE FIELD FSR PROCESS START at {DateTime.Now}--------------------");

                    F_id = Field_id;
                    Console.WriteLine($"Field_id - Producer_id :  {F_id} - {ProducerId}...");
                    if (options == "opCalculateAll" || options == "opCurrent_Benchmarks")
                    {
                        //calculate benchmark
                        var Result = await commonMethods.CalculateBenchmarkCommon(F_id, FieldOptionId);
                        if (Result["success"] == "true")
                        {
                            BenchmarkSuccess = true;
                        }
                        if (Result["message"] == rusle2ErrorMessage)
                        {
                            return Json(new { success = false, message = rusle2ErrorMessage, responsesList = ResponsesList });
                        }
                        Rusle2ResponseList.Add("Rusle2ResponseBench", Convert.ToString(Result["rusle2response"]));
                        //calculate FSR
                        var Result2 = await commonMethods.CalculatePreviewFSRCommon(F_id, FieldOptionId);
                        if (Result2["success"] == "true")
                        {
                            CalcFSRSuccess = true;
                        }
                        if (Result2["message"] == rusle2ErrorMessage)
                        {
                            return Json(new { success = false, message = rusle2ErrorMessage, responsesList = ResponsesList });
                        }
                    }
                    string query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',datarefcursor2 =>'data2',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {F_id});";
                    DataSet queryDs = _masterRepository.GetAndSelectMultipleTableItems(query);
                    bool IsOption1IdAvailable = queryDs.Tables.Count > 0 && queryDs.Tables[1].AsEnumerable().Any(row => row.Field<int>("field_option_id") == Option1Id);

                    bool IsOption2IdAvailable = queryDs.Tables.Count > 0 && queryDs.Tables[1].AsEnumerable().Any(row => row.Field<int>("field_option_id") == Option2Id);

                    if (options == "opOptions" || options == "opCalculateAll")
                    {
                        if (IsOption1IdAvailable)
                        {
                            var Result3 = await commonMethods.CalculatePreviewFSRCommon(F_id, Option1Id);
                            if (Result3["success"] == "true")
                            {
                                CalcFSRSOption1uccess = true;
                            }
                            if (Result3["message"] == rusle2ErrorMessage)
                            {
                                return Json(new { success = false, message = rusle2ErrorMessage, responsesList = ResponsesList });
                            }
                            Rusle2ResponseList.Add("Rusle2ResponseOp1", Convert.ToString(Result3["rusle2response"]));
                        }
                        if (IsOption2IdAvailable)
                        {
                            var Result4 = await commonMethods.CalculatePreviewFSRCommon(F_id, Option2Id);
                            if (Result4["success"] == "true")
                            {
                                CalcFSRSOption2uccess = true;
                            }
                            if (Result4["message"] == rusle2ErrorMessage)
                            {
                                return Json(new { success = false, message = rusle2ErrorMessage, responsesList = ResponsesList });
                            }
                            Rusle2ResponseList.Add("Rusle2ResponseOp2",  Convert.ToString(Result4["rusle2response"]));
                        }
                    }
                    iterationResponse.Add("BenchmarkSuccess", $"{BenchmarkSuccess}");
                    iterationResponse.Add("CalcFSRSuccess", $"{CalcFSRSuccess}");
                    iterationResponse.Add("CalcFSROption1Success", $"{CalcFSRSOption1uccess}");
                    iterationResponse.Add("CalcFSROption2Success", $"{CalcFSRSOption2uccess}");
                    iterationResponse.Add("Field_id", $"{F_id}");
                    iterationResponse.Add("Producer_id", $"{ProducerId}");
                    iterationResponse.Add("Rusle2ResponseListBulk", Rusle2ResponseList);
                    ResponsesList.Add(iterationResponse);
                    Console.WriteLine($"BenchmarkSuccess: {BenchmarkSuccess}, CalcFSRSuccess: {CalcFSRSuccess}, CalcFSROption1Success: {CalcFSRSOption1uccess}, CalcFSROption2Success: {CalcFSRSOption2uccess}");
                    Message = $"Benchmark & Calculate FSR is Complete for {F_id}";
                    Console.WriteLine($"------------------------SINGLE FIELD FSR PROCESS END at {DateTime.Now}--------------------");
                    return Json(new { success = true, message = Message, responsesList = ResponsesList });
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                Error = Convert.ToString(ex);
            }
            return Json(new { success = false, message = "Benchmark & Calculate FSR is incomplete", errorLog = Error });
        }

        #endregion
    }
}