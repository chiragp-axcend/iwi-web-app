using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using InternationalWaterWebApp.Library.Common;
using InternationalWaterWebApp.Library.DatabaseConnection;
using InternationalWaterWebApp.Library.ViewModel;
using InternationalWaterWebApp.Models;
using InternationalWaterWebApp.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Data;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace InternationalWaterWebApp.Controllers
{
    [AuthorizationCS(new int[] { (int)UserTypeEnum.Admin, (int)UserTypeEnum.Producer, (int)UserTypeEnum.Agronomist })]
    public class CommonController : BaseController
    {
        private CommonMethods commonMethods;
        private EncryptDecryptData encryptDecryptData;
        private static HttpClient _httpClient = new HttpClient();

        public CommonController()
        {
            commonMethods = new CommonMethods();
            encryptDecryptData = new EncryptDecryptData();
        }

        public IActionResult SplashPage()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("Id")))
                {
                    return RedirectToAction("Login");
                }

                ViewBag.UserRole = LoginUserRoleId;

                string RefererUrl = commonMethods.GetRefererUrl(Request);

                if (!string.IsNullOrEmpty(RefererUrl))
                    if (RefererUrl != "/" && RefererUrl.ToLower() != "/login")
                    {
                        ViewBag.ReturnUrl = RefererUrl;
                    }

                return View();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public IActionResult ChangePassword(User user, string Confirm)
        {
            try
            {
                if (LoginUserId > 0)
                {
                    if (user.Password == Confirm)
                    {
                        user.Id = LoginUserId;
                        bool changed = commonMethods.ChangePassword(user);

                        user.UserRole = LoginUserRoleId;
                        TempData["Success"] = "Password has been changed successfully.";

                        if (user.UserRole == (int)UserTypeEnum.Admin)
                            return Redirect("/Admin/Landing");
                        else if (user.UserRole == (int)UserTypeEnum.Producer)
                            return Redirect("/Producer/Landing");
                        else
                            return RedirectToAction("Error");
                    }
                    else
                    {
                        TempData["Error"] = "Password and Confirm Password should be matched";
                        return View();
                    }
                }
                else
                    return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error");
            }
        }

        public IActionResult ResetPassword()
        {
            try
            {
                string SiteUrl = RootDirectoryPath.SiteURL;
                string RefererUrl = commonMethods.GetRefererUrl(Request);
                ViewBag.ReturnUrl = SiteUrl + RefererUrl;
                return View();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public IActionResult ResetPassword(User user)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                if (user != null)
                {

                    // check to user exist or not
                    string SelectQuery = $"SELECT * from  \"Users\".water_getallusers_ref (datarefcursor => 'data',_ActionType => 'GetUserByEmail', _email => '{user.Email}')";
                    DataSet DsData = _masterRepository.GetAndSelectTableItems(SelectQuery);

                    if (DsData.Tables[0].Rows.Count > 0)
                    {
                        string Password = RandomPasswordGenerator.GeneratePassword();
                        object[] userParams = { "_ActionType", "_email", "_password" };
                        object[] userValues = { "ChangePasswordByEmail", user.Email, Password };
                        object[] userSqlDbTypes = { "nvarchar", "int", "nvarchar" };

                        //update password on database
                        //long? data = _masterRepository.InsertUpdate($"SELECT * from water_getallusers_ref (datarefcursor := 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => '{userValues[1]}',{userParams[2]} => '{userValues[2]}');");
                        DataSet data = _masterRepository.GetAndSelectTableItems($"SELECT * from  \"Users\".water_getallusers_ref (datarefcursor := 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => '{userValues[1]}',{userParams[2]} => '{userValues[2]}');");

                        if (data != null)
                        {
                            // Reset Password mail send for new generated password
                            bool MailProce = SendEmailToResetPassowrd(user.Email, Password, user.FirstName, user.LastName);
                            if (MailProce == true)
                            {
                                TempData["Success"] = "Password has been reset successfully, Plesae check your email";
                                return RedirectToAction("Login", "WebApp");
                            }
                            else
                            {
                                TempData["Error"] = "Password has been reset successfully, Error has occured while sending an email.";
                                return View();
                            }
                        }
                        return RedirectToAction("Login", "WebApp");
                    }
                    else
                    {
                        TempData["Error"] = "User not found with current email, Please check your email.";
                        return View();
                    }
                }
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error");
            }
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult LoginSelection(string userEmail, string userPass, int? userType, string userTypes)
        {
            try
            {
                User user = new User();
                //ViewBag.userTypes = userTypes;
                user.Email = userEmail;
                user.Password = userPass;
                return View(user);
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        [Route("/{controller}/{action}/{Id}")]
        public IActionResult MyFieldEdit(string id)
        {
            string row = encryptDecryptData.Decryptdata(id);
            ViewBag.UserRole = LoginUserRoleId;
            IMasterRepository _masterRepository = new MasterRepository();
            try
            {

                ViewBag.StateOptions = CommonMethods.GetStateOptions(includeCode: true);

                object[] agrParams = { "_ActionType", "_field_id" };
                object[] agrValues = { "GetMyFieldsEditData", row };
                //DataSet FieldData = _masterRepository.GetAndSelectTableItems($"select producer_id, field_id , name, street_address, city , state_name , zip , county , township , range , section , section_quarter , farm_id from fields where field_id = {row}");
                DataSet FieldData = _masterRepository.GetAndSelectTableItems($"SELECT * from  \"FSRCalc\".water_fields_ref (datarefcursor := 'data',{agrParams[0]} => '{agrValues[0]}', {agrParams[1]} => {agrValues[1]});");
                MyFields field = new MyFields
                {
                    Field_Id = (int)FieldData.Tables[0].Rows[0]["field_id"],
                    Field_Name = Convert.ToString(FieldData.Tables[0].Rows[0]["name"]),
                    Street_Address = Convert.ToString(FieldData.Tables[0].Rows[0]["street_address"]),
                    City = Convert.ToString(FieldData.Tables[0].Rows[0]["city"]),
                    State = Convert.ToString(FieldData.Tables[0].Rows[0]["state"]),
                    Zip = FieldData.Tables[0].Rows[0]["zip"] != DBNull.Value ? Convert.ToString(FieldData.Tables[0].Rows[0]["zip"]) : "",
                    County = Convert.ToString(FieldData.Tables[0].Rows[0]["county"]),
                    Township = Convert.ToString(FieldData.Tables[0].Rows[0]["township"]),
                    Range = Convert.ToString(FieldData.Tables[0].Rows[0]["range"]),
                    Section = Convert.ToString(FieldData.Tables[0].Rows[0]["section"]),
                    Section_Quarter = Convert.ToString(FieldData.Tables[0].Rows[0]["section_quarter"]),
                    SectionHalf = Convert.ToString(FieldData.Tables[0].Rows[0]["section_half"]),
                    Farm_Id = (int)FieldData.Tables[0].Rows[0]["farm_id"],
                    ProducerId = (int)FieldData.Tables[0].Rows[0]["producer_id"]
                };

                ViewBag.FieldName = field.Field_Name;
                var Field_ProducerId = field.ProducerId;
                var User_id = LoginUserId;
                var Session_UserRole = LoginUserRoleId;
                var ProducerId = 1;
                if (Session_UserRole == (int)UserTypeEnum.Admin || Session_UserRole == (int)UserTypeEnum.Agronomist)
                {
                    ProducerId = Field_ProducerId;
                }
                else
                {
                    ProducerId = User_id;
                }

                //This viewbag use only when producer login and edit field
                ViewBag.ProducerId = Field_ProducerId;
                ViewBag.Farm = (from Farm farm in commonMethods.GetFarmByPro(Field_ProducerId)
                                select new SelectListItem
                                {
                                    Text = Convert.ToString(farm.FarmName),
                                    Value = Convert.ToString(farm.FarmId),

                                }).ToList();

                ViewBag.Producer = (from Producers producers in commonMethods.GetProducerDropDown()
                                    select new SelectListItem
                                    {
                                        Text = producers.FirstName + " " + producers.LastName,
                                        Value = Convert.ToString(producers.ProducerId)
                                    }).ToList();


                return View(field);
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        [HttpPost]
        public IActionResult MyFieldUpdate(MyFields field)
        {
            if (ModelState.IsValid)
            {
                IMasterRepository _masterRepository = new MasterRepository();
                try
                {
                    object[] agrParams = { "_ActionType", "_producer_id", "_field_id", "_farm_id", "_name", "_street_address", "_city", "_state_name", " _zip", "_county", "_township", "_range", "_section", "_section_half", "_section_quarter" };
                    object[] agrValues = { "UpdateMyFields", field.ProducerId, field.Field_Id, field.Farm_Id, field.Field_Name, field.Street_Address, field.City, field.State, Convert.ToDecimal(field.Zip), field.County, field.Township, field.Range, field.Section, field.SectionHalf, field.Section_Quarter };

                    DataSet EditFieldData = _masterRepository.GetAndSelectTableItems($"SELECT * from  \"FSRCalc\".water_fields_ref (datarefcursor := 'data',{agrParams[0]} =>'{agrValues[0]}',{agrParams[1]} =>{agrValues[1]},{agrParams[2]} =>{agrValues[2]},{agrParams[3]} =>{agrValues[3]},{agrParams[4]} =>'{agrValues[4]}',{agrParams[5]} =>'{agrValues[5]}',{agrParams[6]} =>'{agrValues[6]}',{agrParams[7]} =>'{agrValues[7]}',{agrParams[8]} =>'{agrValues[8]}',{agrParams[9]} =>'{agrValues[9]}',{agrParams[10]} =>'{agrValues[10]}',{agrParams[11]} =>'{agrValues[11]}',{agrParams[12]} =>'{agrValues[12]}',{agrParams[13]} =>'{agrValues[13]}',{agrParams[14]} =>'{agrValues[14]}');");

                    if (EditFieldData.Tables[0].Rows.Count != null)
                    {
                        TempData["Success"] = "The field saved successfully.";
                        switch (LoginUserRoleId)
                        {
                            case (int)UserTypeEnum.Admin:
                                return RedirectToAction("MyFields", "Admin");
                            case (int)UserTypeEnum.Producer:
                                return RedirectToAction("MyFields", "Producer");
                            case (int)UserTypeEnum.Agronomist:
                                return RedirectToAction("Landing", "Advisor", new { field = 1 });
                        }
                    }
                    return RedirectToAction("Error", "Common");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                    Log.LogError(ex);
                    return RedirectToAction("Error", "Common");
                }
            }
            else
            {
                TempData["Error"] = "Please enter proper values.";
                return View();
            }
        }

        public IActionResult MyFieldsDetails(string id, string name, string DropdownFrom, string DropdownTo, string Outcomechart)
        {
            try
            {
                int FieldId = int.Parse(encryptDecryptData.Decryptdata(id));
                ViewBag.FieldId = FieldId;
                ViewBag.FieldName = name;
                // FieldId = 47;
                ViewBag.UserRole = LoginUserRoleId;

                IMasterRepository _masterRepository = new MasterRepository();
                #region other code



                #region global variables

                int lastIndex = 0;
                string yearfrom = "0", yearto = "0", valuefrom = "0", valueto = "0";

                #endregion

                //NOTE: string currentyear = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {encryptDecryptData.Decryptdata(id)});";
                string currentyear = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {FieldId});";
                DataSet currentyear_data = _masterRepository.GetAndSelectTableItems(currentyear);
                DataTable currentyear_data_DT = currentyear_data.Tables.Count > 0 ? currentyear_data.Tables[0] : null;
                if (currentyear_data_DT != null && currentyear_data_DT.Rows.Count > 0)
                {
                    yearfrom = currentyear_data_DT.Rows[0][0].ToString();
                    lastIndex = currentyear_data_DT.Rows.Count - 1;
                    yearto = currentyear_data_DT.Rows[lastIndex][0].ToString();
                }

                valuefrom = DropdownFrom ?? yearfrom;
                valueto = DropdownTo ?? (DropdownFrom ?? yearto);

                //NOTE: Getting dynamic years from DB
                List<SelectListItem> li = currentyear_data_DT.AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Text = row["year"].ToString(),
                    Value = row["year"].ToString()
                }).ToList();

                ViewBag.Dropdownfrom = new SelectList(li, "Value", "Text", valuefrom);
                ViewBag.Dropdownto = new SelectList(li, "Value", "Text", valueto);

                // New function call
                object[] Params = { "_field_id", "_field_option_id" };
                object[] Values = { FieldId, 1 };
                object[] ValuesforAlfalfa = { FieldId, 5 };
                object[] ValuesforMold = { FieldId, 6 };
                object[] ValuesforOption1 = { FieldId, 3 };
                object[] ValuesforOption2 = { FieldId, 4 };

                ViewBag.FactorLevels = commonMethods.four_R_chart_Function(FieldId, Convert.ToInt32(valuefrom == "" ? "0" : valuefrom), Convert.ToInt32(valueto == "" ? "0" : valueto), 1, 1); // get the final output for nitrogren , fertilizer_id = 1
                ViewBag.FactorLevelsp = commonMethods.four_R_chart_Function(FieldId, Convert.ToInt32(valuefrom == "" ? "0" : valuefrom), Convert.ToInt32(valueto == "" ? "0" : valueto), 1, 2); // get the final output for phodpohorus , fertilizer_id = 2

                // Current Opertaion start
                string newquery = $"SELECT * from  \"FSRCalc\".water_func_stewardship_indices_new_ref( datarefcursor => 'data' ,{Params[0]} => '{Values[0]}',{Params[1]} => '{Values[1]}');";
                DataSet newDsfsrData = _masterRepository.GetAndSelectTableItems(newquery);
                ViewBag.Opertiondata = newDsfsrData;

                // Alfalfa start
                string queryforalfalfa = $"SELECT * from  \"FSRCalc\".water_func_stewardship_indices_new_ref( datarefcursor => 'data' ,{Params[0]} => '{Values[0]}',{Params[1]} => '{ValuesforAlfalfa[1]}');";
                DataSet newDsfsrDataforAlfalfa = _masterRepository.GetAndSelectTableItems(queryforalfalfa);
                ViewBag.AlfalfaData = newDsfsrDataforAlfalfa;

                // Mold plow  start 
                string queryforMold = $"SELECT * from \"FSRCalc\".water_func_stewardship_indices_new_ref( datarefcursor => 'data' ,{Params[0]} => '{Values[0]}',{Params[1]} => '{ValuesforMold[1]}');";
                DataSet DsfsrDataforMold = _masterRepository.GetAndSelectTableItems(queryforMold);
                ViewBag.MoldData = DsfsrDataforMold;

                // Environment function alfalfa  call
                string envqueryalf = $"SELECT * from \"FSRCalc\".water_func_environment_new_ref( datarefcursor => 'data' ,{Params[0]} => '{ValuesforAlfalfa[0]}',{Params[1]} => '{ValuesforAlfalfa[1]}');";
                DataSet envDsfsrDataalf = _masterRepository.GetAndSelectTableItems(envqueryalf);
                ViewBag.envAlfalaDatas = envDsfsrDataalf;

                // Environment function mold call
                string envquerymold = $"SELECT * from \"FSRCalc\".water_func_environment_new_ref( datarefcursor => 'data' ,{Params[0]} => '{ValuesforMold[0]}',{Params[1]} => '{ValuesforMold[1]}');";
                DataSet envDsfsrDatamold = _masterRepository.GetAndSelectTableItems(envquerymold);
                ViewBag.envMoldDatas = envDsfsrDatamold;

                object[] userParams = { "_field_id", "_field_option_id", "_nfertin", "_pfertin" };
                object[] userValues = { FieldId, 1 };
                object[] userValuesopt1 = { FieldId, 3 };
                object[] userValuesopt2 = { FieldId, 4 };
                object[] userValuesalf = { FieldId, 5 };
                object[] userValuesmold = { FieldId, 6 };

                // Nfertin
                // Dictionary<string, string> factorLevels = commonMethods.four_R_Function(FieldId, 1, 1);
                Dictionary<string, string> factorLevels = commonMethods.four_R_chart_Function(FieldId, Convert.ToInt32(valuefrom == "" ? "0" : valuefrom), Convert.ToInt32(valueto == "" ? "0" : valueto), 1, 1, "value");
                List<string> factorKeys = new List<string>() { "source", "rate", "timing", "placement" };
                double nfertin = factorKeys.Average(key => commonMethods.nFetinAndpnertin(factorLevels[key]));

                // Pfetrin
                //Dictionary<string, string> factorLevelsp = commonMethods.four_R_Function(FieldId, 2, 1);
                Dictionary<string, string> factorLevelsp = commonMethods.four_R_chart_Function(FieldId, Convert.ToInt32(valuefrom == "" ? "0" : valuefrom), Convert.ToInt32(valueto == "" ? "0" : valueto), 1, 2, "value");
                double pfertin = factorKeys.Average(key => commonMethods.nFetinAndpnertin(factorLevelsp[key]));

                // get the final output for nitrogren , fertilizer_id = 1


                int firstValue = valuefrom != string.Empty ? Convert.ToInt32(valuefrom) : 0;
                int secondValue = valueto != string.Empty ? Convert.ToInt32(valueto) : 0;

                string queryforfield_details = $"select * from \"FSRCalc\".frs_new_get_fields_details(datarefcursor :='data' ,_field_id =>{FieldId}, _field_option_id =>1 , yearfrom => {firstValue} , yearto => {secondValue});";
                //string query = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => '{userValues[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                DataSet field_details_data = _masterRepository.GetAndSelectTableItems(queryforfield_details);
                DataRow field_details_data_FirstRow = field_details_data.Tables[0].Rows[0];
                //ViewBag.arces = int.TryParse(field_details_data_FirstRow["arces"]?.ToString().Trim(), out int arces) ? arces : 0;
                ViewBag.arces = field_details_data_FirstRow["arces"];
                ViewBag.crop_names = field_details_data_FirstRow["crop_names"];
                ViewBag.tillag_names = field_details_data_FirstRow["tillag_names"];
                ViewBag.cover_crop_names = field_details_data_FirstRow["cover_crop_names"];
                ViewBag.RFactorLevels = field_details_data_FirstRow["r4_n_fert_level"];
                ViewBag.RFactorLevelsp = field_details_data_FirstRow["r4_p_fert_level"];
                ViewBag.fieldRiskSediment = field_details_data_FirstRow["_risk_level_sed"];
                ViewBag.fieldPRiskSediment = field_details_data_FirstRow["_risk_level_p"];
                ViewBag.quality_grade = field_details_data_FirstRow["_quality_grade"];
                ViewBag.management_conversation_practice = field_details_data_FirstRow["management_conversation_practice"];
                ViewBag.structural_conversation_practice = field_details_data_FirstRow["structural_conversation_practice"];
                ViewBag.drainage = field_details_data_FirstRow["drainage"];

                //string query = $"select * from \"FSRCalc\".fsr_new_calc_fsr_calc_latest(datarefcursor :='data' ,_field_id =>{FieldId}, _field_option_id =>1 ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {firstValue} , _end_year => {secondValue});";
                string query = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{FieldId},  _start_year => {firstValue} , _end_year => {secondValue}, _field_option_id => 1);";
                DataSet DsfsrData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.FSRdata = DsfsrData;

                ///NOTE: Calculate Field Stewardship Rating
                double fsrRating = 0;
                double sumofallgradefsr = 0;
                if (DsfsrData.Tables.Count > 0 && DsfsrData.Tables[0].Rows.Count > 0)
                {
                    object fsrValue = DsfsrData.Tables[0].Rows[0]["fsr"];
                    fsrRating = (fsrValue != DBNull.Value) ? Math.Round(Convert.ToDouble(fsrValue), 2) : 0;
                    sumofallgradefsr = (DsfsrData.Tables[0].Rows[0]["sumofallgrade"] != DBNull.Value) ? Math.Round(Convert.ToDouble(DsfsrData.Tables[0].Rows[0]["sumofallgrade"]), 2) : 0;
                }

                ViewBag.FSR = fsrRating;
                ViewBag.FSRsumofallgrade = sumofallgradefsr;

                //NOTE: Get Chart DataView History
                //string query = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => '{userValues[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";


                #endregion

                #region Field_Information_Tab_Data

                string getImgQueryFieldLocation = $"select * from \"FSRCalc\".field_location_data(datarefcursor =>'data',_actionType =>'getfieldlocationdata', _field_id => {FieldId});";
                DataSet DsgetImgQueryFieldLocation = _masterRepository.GetAndSelectTableItems(getImgQueryFieldLocation);
                string? LocationImg = DsgetImgQueryFieldLocation != null && DsgetImgQueryFieldLocation.Tables.Count > 0 && DsgetImgQueryFieldLocation.Tables[0].Rows.Count > 0
                 ? DsgetImgQueryFieldLocation.Tables[0].Rows[0]["field_location_img"].ToString() : string.Empty;
                string? LocationOverview = DsgetImgQueryFieldLocation != null && DsgetImgQueryFieldLocation.Tables.Count > 0 && DsgetImgQueryFieldLocation.Tables[0].Rows.Count > 0
                 ? DsgetImgQueryFieldLocation.Tables[0].Rows[0]["field_location_overview"].ToString() : string.Empty;
                string? MzOverview = DsgetImgQueryFieldLocation != null && DsgetImgQueryFieldLocation.Tables.Count > 0 && DsgetImgQueryFieldLocation.Tables[0].Rows.Count > 0
                 ? DsgetImgQueryFieldLocation.Tables[0].Rows[0]["mz_overview"].ToString() : string.Empty;

                ViewBag.LocationImg = LocationImg;
                ViewBag.LocationOverview = LocationOverview;
                ViewBag.MzOverview = MzOverview;

                #endregion

                #region Environment outcome get data
                //first column score
                //field_option_id=1
                string EnvrmtOut_scoreQuery = $"select * from \"FSRCalc\".environment_outcomedata(datarefcursor :='data' ,_field_id =>{FieldId}, _field_option_id =>{1} ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {firstValue} , _end_year => {secondValue});";
                DataSet EnvrmtOut_scoreQueryData = _masterRepository.GetAndSelectTableItems(EnvrmtOut_scoreQuery);
                ViewBag.DsEnvCurrentOpScore = EnvrmtOut_scoreQueryData != null && EnvrmtOut_scoreQueryData.Tables.Count > 0 && EnvrmtOut_scoreQueryData.Tables[0].Rows.Count > 0 ? EnvrmtOut_scoreQueryData : null;
                //alfalfa column score
                //field_option_id=5
                string EnvrmtOut_AlfalfaScoreQuery = $"select * from \"FSRCalc\".environment_outcomedata(datarefcursor :='data' ,_field_id =>{FieldId}, _field_option_id =>{5} ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {firstValue} , _end_year => {secondValue});";
                DataSet EnvrmtOut_AlfalfaScoreQueryData = _masterRepository.GetAndSelectTableItems(EnvrmtOut_AlfalfaScoreQuery);
                ViewBag.DsEnvAlfalfaScore = EnvrmtOut_AlfalfaScoreQueryData != null && EnvrmtOut_AlfalfaScoreQueryData.Tables.Count > 0 && EnvrmtOut_AlfalfaScoreQueryData.Tables[0].Rows.Count > 0 ? EnvrmtOut_AlfalfaScoreQueryData : null;
                //moldboad column score
                //field_option_id=6
                string EnvrmtOut_MoldboadScoreQuery = $"select * from \"FSRCalc\".environment_outcomedata(datarefcursor :='data' ,_field_id =>{FieldId}, _field_option_id =>{6} ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {firstValue} , _end_year => {secondValue});";
                DataSet EnvrmtOut_MoldboadScoreQueryData = _masterRepository.GetAndSelectTableItems(EnvrmtOut_MoldboadScoreQuery);
                ViewBag.DsEnvMoldboadScore = EnvrmtOut_MoldboadScoreQueryData != null && EnvrmtOut_MoldboadScoreQueryData.Tables.Count > 0 && EnvrmtOut_MoldboadScoreQueryData.Tables[0].Rows.Count > 0 ? EnvrmtOut_MoldboadScoreQueryData : null;

                string EnvrmtOut_opt1ScoreQuery = $"select * from \"FSRCalc\".environment_outcomedata(datarefcursor :='data' ,_field_id =>{FieldId}, _field_option_id =>{3} ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {firstValue} , _end_year => {secondValue});";
                DataSet EnvrmtOut_opt1boadScoreQueryData = _masterRepository.GetAndSelectTableItems(EnvrmtOut_opt1ScoreQuery);
                ViewBag.DsEnvopt1boadScore = EnvrmtOut_opt1boadScoreQueryData != null && EnvrmtOut_opt1boadScoreQueryData.Tables.Count > 0 && EnvrmtOut_opt1boadScoreQueryData.Tables[0].Rows.Count > 0 ? EnvrmtOut_opt1boadScoreQueryData : null;

                string EnvrmtOut_opt2ScoreQuery = $"select * from \"FSRCalc\".environment_outcomedata(datarefcursor :='data' ,_field_id =>{FieldId}, _field_option_id =>{4} ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {firstValue} , _end_year => {secondValue});";
                DataSet EnvrmtOut_opt2boadScoreQueryData = _masterRepository.GetAndSelectTableItems(EnvrmtOut_opt2ScoreQuery);
                ViewBag.DsEnvopt2boadScore = EnvrmtOut_opt2boadScoreQueryData != null && EnvrmtOut_opt2boadScoreQueryData.Tables.Count > 0 && EnvrmtOut_opt2boadScoreQueryData.Tables[0].Rows.Count > 0 ? EnvrmtOut_opt2boadScoreQueryData : null;

                #endregion

                #region Option 1/2, Alfala & Mold
                // Option 1
                // string queryopt1 = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValuesopt1[0]}',{userParams[1]} => '{userValuesopt1[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                string queryopt1 = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{FieldId},  _start_year => {firstValue} , _end_year => {secondValue}, _field_option_id => 3);";
                DataSet opt1Data = _masterRepository.GetAndSelectTableItems(queryopt1);

                double opt1fsrRating = 0;
                double sumofallgradeopt1 = 0;
                if (opt1Data.Tables.Count > 0 && opt1Data.Tables[0].Rows.Count > 0)
                {
                    object opt1fsrValue = opt1Data.Tables[0].Rows[0]["fsr"];
                    opt1fsrRating = (opt1fsrValue != DBNull.Value) ? Math.Round(Convert.ToDouble(opt1fsrValue), 2) : 0;
                    sumofallgradeopt1 = (opt1Data.Tables[0].Rows[0]["sumofallgrade"] != DBNull.Value) ? Math.Round(Convert.ToDouble(opt1Data.Tables[0].Rows[0]["sumofallgrade"]), 2) : 0;
                }
                ViewBag.opt1FSR = opt1fsrRating;
                ViewBag.opt1sumofallgrade = sumofallgradeopt1;
                ViewBag.opt1data = opt1Data;
                ViewBag.opt1title = (opt1Data.Tables[0].Rows[0]["_option_name"] != DBNull.Value) ? opt1Data.Tables[0].Rows[0]["_option_name"] : "Option1";
                ViewBag.opt1description = (opt1Data.Tables[0].Rows[0]["_option_description"] != DBNull.Value) ? opt1Data.Tables[0].Rows[0]["_option_description"] : "";

                // Option 2
                //  string queryopt2 = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValuesopt2[0]}',{userParams[1]} => '{userValuesopt2[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                string queryopt2 = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{FieldId},   _start_year => {firstValue} , _end_year => {secondValue}, _field_option_id => 4);";
                DataSet opt2Data = _masterRepository.GetAndSelectTableItems(queryopt2);
                double opt2fsrRating = 0;
                double sumofallgradeopt2 = 0;
                if (opt2Data.Tables.Count > 0 && opt2Data.Tables[0].Rows.Count > 0)
                {
                    object opt2fsrValue = opt2Data.Tables[0].Rows[0]["fsr"];
                    opt2fsrRating = (opt2fsrValue != DBNull.Value) ? Math.Round(Convert.ToDouble(opt2fsrValue), 2) : 0;
                    sumofallgradeopt2 = (opt2Data.Tables[0].Rows[0]["sumofallgrade"] != DBNull.Value) ? Math.Round(Convert.ToDouble(opt2Data.Tables[0].Rows[0]["sumofallgrade"]), 2) : 0;
                }
                ViewBag.opt2FSR = opt2fsrRating;
                ViewBag.opt2sumofallgrade = sumofallgradeopt2;
                ViewBag.opt2data = opt2Data;
                ViewBag.opt2title = (opt2Data.Tables[0].Rows[0]["_option_name"] != DBNull.Value) ? opt2Data.Tables[0].Rows[0]["_option_name"] : "Option2";
                ViewBag.opt2description = (opt2Data.Tables[0].Rows[0]["_option_description"] != DBNull.Value) ? opt2Data.Tables[0].Rows[0]["_option_description"] : "";

                // Alfalfa
                // string queryalf = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValuesalf[0]}',{userParams[1]} => '{userValuesalf[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                string queryalf = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{FieldId},  _start_year => {firstValue} , _end_year => {secondValue}, _field_option_id => 5);";
                DataSet alfData = _masterRepository.GetAndSelectTableItems(queryalf);
                double alffsrRating = 0;
                double sumofallgradealf = 0;
                if (alfData.Tables.Count > 0 && alfData.Tables[0].Rows.Count > 0)
                {
                    object alffsrValue = alfData.Tables[0].Rows[0]["fsr"];
                    alffsrRating = (alffsrValue != DBNull.Value) ? Math.Round(Convert.ToDouble(alffsrValue), 2) : 0;
                    sumofallgradealf = (alfData.Tables[0].Rows[0]["sumofallgrade"] != DBNull.Value) ? Math.Round(Convert.ToDouble(alfData.Tables[0].Rows[0]["sumofallgrade"]), 2) : 0;
                }
                ViewBag.alfFSR = alffsrRating;
                ViewBag.alfsumofallgrade = sumofallgradealf;
                ViewBag.alfData = alfData;

                // Mold
                // string querymol = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValuesmold[0]}',{userParams[1]} => '{userValuesmold[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                string querymol = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{FieldId},   _start_year => {firstValue} , _end_year => {secondValue}, _field_option_id => 6);";
                DataSet molData = _masterRepository.GetAndSelectTableItems(querymol);
                double molfsrRating = 0;
                double sumofallgrademol = 0;
                if (molData.Tables.Count > 0 && molData.Tables[0].Rows.Count > 0)
                {
                    object molfsrValue = molData.Tables[0].Rows[0]["fsr"];
                    molfsrRating = (molfsrValue != DBNull.Value) ? Math.Round(Convert.ToDouble(molfsrValue), 2) : 0;
                    sumofallgrademol = (molData.Tables[0].Rows[0]["sumofallgrade"] != DBNull.Value) ? Math.Round(Convert.ToDouble(molData.Tables[0].Rows[0]["sumofallgrade"]), 2) : 0;
                }
                ViewBag.molFSR = molfsrRating;
                ViewBag.molsumofallgrade = sumofallgrademol;
                ViewBag.molData = molData;


                #endregion

                //NOTE: Financial table
                string Financial_tabelQuery = $"select * from \"FSRCalc\".water_myfielddetails_financial_table(datarefcursor=>'data',_actiontype =>'GetFinancialByFieldId',_field_id=>{FieldId}, _startyear =>{valuefrom},_endyear =>{valueto});";
                DataSet financialReturn_data = _masterRepository.GetAndSelectTableItems(Financial_tabelQuery);
                DataTable financialReturn_data_DT = financialReturn_data.Tables.Count > 0 ? financialReturn_data.Tables[0] : null;
                if (financialReturn_data_DT != null && financialReturn_data_DT.Rows.Count > 0 && (int)financialReturn_data_DT.Rows[0][0] != 0)
                {
                    var groupedData = financialReturn_data_DT.AsEnumerable()
                                            .GroupBy(row => row.Field<string>("field_option_name"))
                                            .Select(g => new
                                            {
                                                field_option_name = g.Key,
                                                financialList = g.ToList()
                                            });

                    var financial = new List<Financial>();

                    var groupedFinancial = financialReturn_data_DT.AsEnumerable()
    .GroupBy(row => new
    {
        field_option_name = Convert.ToString(row["field_option_name"]),
        field_option_id = Convert.ToInt32(row["field_option_id"])
    })
    .Select(group => new Financial
    {
        field_option_name = group.Key.field_option_name,
        field_option_id = group.Key.field_option_id,
        FinancialDetailsList = group.Select(row => new FinancialDetailsList
        {
            year_cropname = row.Field<string>("year_cropname") ?? string.Empty,
            year = row.Field<int?>("year") ?? 0,
            field_id = row.Field<int?>("field_id") ?? 0,
            crop1_id = row.Field<int?>("crop1_id") ?? 0,
            field_option_id = row.Field<int?>("field_option_id") ?? 0,
            expectedyield = row.Field<double?>("expectedyield") ?? 0.0,
            cashprice = row.Field<double?>("cashprice") ?? 0.0,
            TotalGrossRevenue = row.Field<double?>("TotalGrossRevenue") ?? 0.0,
            TotDirectExp = row.Field<double?>("TotDirectExp") ?? 0.0,
            TotOverheadExp = row.Field<double?>("TotOverheadExp") ?? 0.0,
            TotExpense = row.Field<double?>("TotExpense") ?? 0.0,
            BrkEvenBushel = row.Field<double?>("BrkEvenBushel") ?? 0.0,
            NetReturnNoGovt = row.Field<double?>("NetReturnNoGovt") ?? 0.0,
            RegionalBenchmark = row.Field<double?>("RegionalBenchmark") ?? 0.0,
            BenchmarkTotalExpenses = row.Field<double?>("BenchmarkTotalExpenses") ?? 0.0
        }).ToList()
    }); ;

                    financial.AddRange(groupedFinancial);
                    ViewBag.financial_groupedData = financial;
                    ViewBag.financial_tbl_data = financialReturn_data;
                }
                else
                    ViewBag.financial_tbl_data = null;

                string averageFinancial_tabelQuery = $"select * from \"FSRCalc\".water_myfielddetails_averagefinancial_table(datarefcursor=>'data',_field_id=>{FieldId},_field_option_id => 1 , _startyear => {valuefrom},_lastyear=> {valueto} );";
                DataSet averagefinancialReturn_data = _masterRepository.GetAndSelectTableItems(averageFinancial_tabelQuery);
                DataTable averagefinancialReturn_data_DT = averagefinancialReturn_data.Tables.Count > 0 ? averagefinancialReturn_data.Tables[0] : null;
                if (averagefinancialReturn_data_DT != null && averagefinancialReturn_data_DT.Rows.Count > 0)
                {
                    ViewBag.StartYear = valuefrom;
                    ViewBag.ToYear = valueto;
                    ViewBag.Yield = averagefinancialReturn_data_DT?.Columns.Contains("expectedyield") == true ? (averagefinancialReturn_data_DT.Rows[0]["expectedyield"] != DBNull.Value ? Math.Round(Convert.ToDouble(averagefinancialReturn_data_DT.Rows[0]["expectedyield"]), 2) : 0) : 0;
                    ViewBag.CommodityPrice = averagefinancialReturn_data_DT?.Columns.Contains("cashprice") == true ? (averagefinancialReturn_data_DT.Rows[0]["cashprice"] != DBNull.Value ? Math.Round(Convert.ToDouble(averagefinancialReturn_data_DT.Rows[0]["cashprice"]), 2) : 0) : 0;
                    ViewBag.GrossReturn = averagefinancialReturn_data_DT?.Columns.Contains("GrossRevenue") == true ? (averagefinancialReturn_data_DT.Rows[0]["GrossRevenue"] != DBNull.Value ? Math.Round(Convert.ToDouble(averagefinancialReturn_data_DT.Rows[0]["GrossRevenue"]), 2) : 0) : 0;
                    ViewBag.TotalExpenses = averagefinancialReturn_data_DT?.Columns.Contains("TotExpense") == true ? (averagefinancialReturn_data_DT.Rows[0]["TotExpense"] != DBNull.Value ? Math.Round(Convert.ToDouble(averagefinancialReturn_data_DT.Rows[0]["TotExpense"]), 2) : 0) : 0;
                    ViewBag.BreakEvenBushel = averagefinancialReturn_data_DT?.Columns.Contains("BrkEvenBushel") == true ? (averagefinancialReturn_data_DT.Rows[0]["BrkEvenBushel"] != DBNull.Value ? Math.Round(Convert.ToDouble(averagefinancialReturn_data_DT.Rows[0]["BrkEvenBushel"]), 2) : 0) : 0;
                    ViewBag.NetReturn = averagefinancialReturn_data_DT?.Columns.Contains("NetReturn") == true ? (averagefinancialReturn_data_DT.Rows[0]["NetReturn"] != DBNull.Value ? Math.Round(Convert.ToDouble(averagefinancialReturn_data_DT.Rows[0]["NetReturn"]), 2) : 0) : 0;
                }
                else
                {
                    ViewBag.StartYear = valuefrom;
                    ViewBag.ToYear = valueto;
                    ViewBag.Yield = 0;
                    ViewBag.CommodityPrice = 0;
                    ViewBag.GrossReturn = 0;
                    ViewBag.TotalExpenses = 0;
                    ViewBag.BreakEvenBushel = 0;
                    ViewBag.NetReturn = 0;
                }
                return View();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult FertilizerandPhosphorusPopup(string id, string actiontype)
        {
            try
            {
                int FieldId = int.Parse(encryptDecryptData.Decryptdata(id));
                ViewBag.FieldId = FieldId;

                //NOTE: nitrogren, fertilizer_id = 1 AND phodpohorus, fertilizer_id = 2
                int fertilizerId = (actiontype == "Fertilizeropen") ? 1 : 2;
                //ViewBag.FactorLevels = commonMethods.four_R_Function(FieldId, fertilizerId);
                ViewBag.FactorLevels = commonMethods.four_R_chart_Function(FieldId, 0, 0, 1, fertilizerId);

                return PartialView("~/Views/Admin/_FertilizerOpenPopup.cshtml");
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult FieldRiskPopup(string id, string actiontype)
        {
            try
            {
                int FieldId = int.Parse(encryptDecryptData.Decryptdata(id));
                ViewBag.FieldId = FieldId;
                string action = (actiontype == "tp") ? "tpfromfield" : "sedfromfield";
                ViewBag.SoilLossLevels = commonMethods.Surface_chart_Function(FieldId, 0, 0, 1, action);
                ViewBag.actiontype = actiontype;

                return PartialView("~/Views/Admin/_FieldRiskOpenPopup.cshtml");
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult ViewFSRViewHistoryPopup(string id, int firstValue, int secondValue)
        {
            try
            {
                int FieldId = int.Parse(encryptDecryptData.Decryptdata(id));

                IMasterRepository _masterRepository = new MasterRepository();

                string ChartYearQaury = $"select* from \"FSRCalc\".fsr_viewhistory_chart_data(datarefcursor => 'data', _field_id => {FieldId}, _field_option_id => 1, _start_year => {firstValue} , _end_year => {secondValue}, actiontype=>'ChartData');";
                DataSet ChartYearQauryData = _masterRepository.GetAndSelectTableItems(ChartYearQaury);

                DataTable table = ChartYearQauryData.Tables[0];
                List<FsrViewHostoryChartData> fsrChartDataList = new List<FsrViewHostoryChartData>();

                foreach (DataRow row in table.Rows)
                {
                    int FieldYear = Convert.ToInt32(row["year"]);
                    string FieldYear_Crop = Convert.ToString(row["year_crop"]);
                    string Crop = Convert.ToString(row["name"]);

                    // string GetFsrChartQuery = $"select * from \"FSRCalc\".fsr_new_calc_fsr_calc_latest(datarefcursor :='data' ,_field_id =>{FieldId}, _field_option_id //=>1 ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {FieldYear} , _end_year => {FieldYear});";
                    string GetFsrChartQuery = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{FieldId},  _start_year => {FieldYear} , _end_year => {FieldYear}, _field_option_id => 1);";
                    //string GetFsrChartQuery = $"select * from \"FSRCalc\".fsr_viewhistory_chart_data(datarefcursor :='data' ,_field_id =>{FieldId},  _start_year => {FieldYear} , _end_year => {FieldYear}, _field_option_id => 1, actiontype=>'ChartDatayearwise');";
                    DataSet GetFsrChartQueryData = _masterRepository.GetAndSelectTableItems(GetFsrChartQuery);

                    double fsrVal = 0;
                    if (GetFsrChartQueryData.Tables.Count > 0 && GetFsrChartQueryData.Tables[0].Rows.Count > 0)
                    {
                        fsrVal = Convert.ToDouble(GetFsrChartQueryData.Tables[0].Rows[0]["fsr"]);
                        //fsrVal = Convert.ToDouble(GetFsrChartQueryData.Tables[0].Rows[0]["fsr_rating"]);
                    }

                    fsrChartDataList.Add(new FsrViewHostoryChartData
                    {
                        FieldYear = FieldYear,
                        FieldYear_Crop = FieldYear_Crop,
                        FsrVal = fsrVal,
                        Crop = Crop
                    });
                }

                return Json(new { success = true, data = JsonConvert.SerializeObject(fsrChartDataList) });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult FinancialYearPopup(string id, int year, int crop, int field_option_id)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                int FieldId = int.Parse(encryptDecryptData.Decryptdata(id));
                ViewBag.FieldId = FieldId;

                string myfielddetails_financial_details = $"select * from \"FSRCalc\".water_myfielddetails_financial_table(datarefcursor :='data',_actiontype =>'GetFinancialByYear' ,_field_id =>{FieldId}, _year => {year} , _crop_id => {crop},_field_option_id => {field_option_id});";
                DataSet myfielddetails_financial_data = _masterRepository.GetAndSelectTableItems(myfielddetails_financial_details);
                DataTable myfielddetails_financial_data_DT = myfielddetails_financial_data.Tables.Count > 0 ? myfielddetails_financial_data.Tables[0] : null;
                if (myfielddetails_financial_data_DT != null && myfielddetails_financial_data_DT.Rows.Count > 0)
                    ViewBag.myfielddetails_financial_tbl_data = myfielddetails_financial_data;
                else
                    ViewBag.myfielddetails_financial_tbl_data = null;


                return PartialView("~/Views/Common/_viewzonefinancialspopup.cshtml");


            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult FinancialDetailsEditcropname(string id, string name, string DropdownFrom, string DropdownTo, int field_option_id)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                int FieldId = int.Parse(encryptDecryptData.Decryptdata(id));
                ViewBag.FieldId = FieldId;
                ViewBag.FieldId = name;

                int lastIndex = 0;
                string yearfrom = string.Empty;
                string yearto = string.Empty;
                string valuefrom = string.Empty;
                string valueto = string.Empty;

                //NOTE: string currentyear = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {encryptDecryptData.Decryptdata(id)});";
                string currentyear = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {FieldId});";
                DataSet currentyear_data = _masterRepository.GetAndSelectTableItems(currentyear);
                DataTable currentyear_data_DT = currentyear_data.Tables.Count > 0 ? currentyear_data.Tables[0] : null;
                if (currentyear_data != null && currentyear_data.Tables.Count > 0 && currentyear_data.Tables[0].Rows.Count > 0)
                {
                    yearfrom = currentyear_data_DT.Rows[0][0].ToString();
                    lastIndex = currentyear_data_DT.Rows.Count - 1;
                    yearto = currentyear_data_DT.Rows[lastIndex][0].ToString();
                }

                valuefrom = DropdownFrom ?? yearfrom;
                valueto = DropdownTo ?? (DropdownFrom ?? yearto);

                //NOTE: Getting dynamic years from DB
                List<SelectListItem> li = currentyear_data_DT.AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Text = row["year"].ToString(),
                    Value = row["year"].ToString()
                }).ToList();

                ViewBag.Dropdownfrom = new SelectList(li, "Value", "Text", valuefrom);
                ViewBag.Dropdownto = new SelectList(li, "Value", "Text", valueto);

                int firstValue = string.IsNullOrEmpty(valuefrom) ? 0 : Convert.ToInt32(valuefrom);
                int secondValue = string.IsNullOrEmpty(valueto) ? 0 : Convert.ToInt32(valueto);

                string queryforfield_details = $"select * from \"FSRCalc\".frs_new_get_fields_details(datarefcursor :='data' ,_field_id =>{FieldId}, _field_option_id =>{field_option_id} , yearfrom => {firstValue} , yearto => {secondValue});";
                //string query = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => '{userValues[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                DataSet field_details_data = _masterRepository.GetAndSelectTableItems(queryforfield_details);
                DataRow field_details_data_FirstRow = field_details_data.Tables[0].Rows[0];
                ViewBag.crop_names = field_details_data_FirstRow["crop_names"];
                ViewBag.tillag_names = field_details_data_FirstRow["tillag_names"];
                ViewBag.cover_crop_names = field_details_data_FirstRow["cover_crop_names"];

                return PartialView("~/Views/Common/_FinancialDetails.cshtml");


            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult PrintCropCertificationPopup(string id, string DropdownFrom)
        {
            try
            {
                int FieldId = int.Parse(encryptDecryptData.Decryptdata(id));
                ViewBag.FieldId = FieldId;
                //  FieldId = 47;

                int lastIndex = 0;
                string yearfrom = string.Empty;
                string yearto = string.Empty;
                string valuefrom = string.Empty;
                string valueto = string.Empty;

                IMasterRepository _masterRepository = new MasterRepository();

                string currentyear = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {FieldId});";
                DataSet currentyear_data = _masterRepository.GetAndSelectTableItems(currentyear);
                DataTable currentyear_data_DT = currentyear_data.Tables.Count > 0 ? currentyear_data.Tables[0] : null;
                if (currentyear_data_DT != null && currentyear_data_DT.Rows.Count > 0)
                {
                    yearfrom = currentyear_data_DT.Rows[0][0].ToString();
                    lastIndex = currentyear_data_DT.Rows.Count - 1;
                    yearto = currentyear_data_DT.Rows[lastIndex][0].ToString();
                }

                valuefrom = DropdownFrom ?? yearfrom;

                //NOTE: Getting dynamic years from DB
                List<SelectListItem> li = currentyear_data_DT.AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Text = row["year"].ToString(),
                    Value = row["year"].ToString()
                }).ToList();

                ViewBag.Dropdownfrom = new SelectList(li, "Value", "Text", valuefrom);

                return PartialView("~/Views/Admin/_printcropcertificationPopup.cshtml");
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult PrintCropCertification(int id, string name, string DropdownFrom, string DropdownTo)
        {
            try
            {
                //int FieldId = int.Parse(encryptDecryptData.Decryptdata(id));
                ViewBag.FieldId = id;
                ViewBag.FieldName = name;
                // FieldId = 47;

                IMasterRepository _masterRepository = new MasterRepository();

                Dictionary<string, string> factorLevels = commonMethods.four_R_Function(id, 1, 1);

                List<string> factorKeys = new List<string>() { "source", "rate", "timing", "placement" };
                double nfertin = 0;
                foreach (string key in factorKeys)
                {
                    nfertin += commonMethods.nFetinAndpnertin(factorLevels[key]);
                }
                nfertin /= factorKeys.Count;

                // Pfetrin
                Dictionary<string, string> factorLevelsp = commonMethods.four_R_Function(id, 2, 1);

                double pfertin = 0;
                foreach (string key in factorKeys)
                {
                    pfertin += commonMethods.nFetinAndpnertin(factorLevelsp[key]);
                }
                pfertin /= factorKeys.Count;

                int firstValue = Convert.ToInt32(DropdownFrom); //asign 1
                int secondValue = Convert.ToInt32(DropdownTo); //asign 1

                string queryforfield_details = $"select * from \"FSRCalc\".frs_new_get_fields_details(datarefcursor :='data' ,_field_id =>{id}, _field_option_id =>1 , yearfrom => {firstValue} , yearto => {firstValue});";
                //string query = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => '{userValues[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                DataSet field_details_data = _masterRepository.GetAndSelectTableItems(queryforfield_details);

                DataRow field_details_data_FirstRow = field_details_data.Tables[0].Rows[0];
                ViewBag.arces = int.TryParse(field_details_data_FirstRow["arces"]?.ToString().Trim(), out int arces) ? arces : 0;
                ViewBag.crop_names = field_details_data_FirstRow["crop_names"];
                ViewBag.tillag_names = field_details_data_FirstRow["tillag_names"];
                ViewBag.cover_crop_names = field_details_data_FirstRow["cover_crop_names"];

                // string query = $"select * from \"FSRCalc\".fsr_new_calc_fsr_calc_latest(datarefcursor :='data' ,_field_id =>{id}, _field_option_id =>1 ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {firstValue} , _end_year => {firstValue});";
                string query = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{id},  _start_year => {firstValue} , _end_year => {secondValue}, _field_option_id => 1);";
                DataSet DsfsrData = _masterRepository.GetAndSelectTableItems(query);

                ViewBag.FSRdata = DsfsrData;

                double fsrRating = 0;
                if (DsfsrData.Tables.Count > 0 && DsfsrData.Tables[0].Rows.Count > 0)
                {
                    object fsrValue = DsfsrData.Tables[0].Rows[0]["fsr"];
                    if (fsrValue != DBNull.Value)
                    {
                        fsrRating = Math.Round(Convert.ToDouble(fsrValue), 2);
                    }
                }
                ViewBag.FSR = fsrRating;

                string QueryFieldsPrint = $"SELECT * from \"FSRCalc\".frs_new_get_fieldsprint_details(_field_id => {id}, yearfrom => {firstValue} , yearto => {firstValue});";
                DataSet DsFieldsPrintData2 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFieldsPrint);
                ViewBag.Fromyear = DropdownFrom;
                ViewBag.FieldsPrintDeatils2 = DsFieldsPrintData2;

                string QueryField3 = $"SELECT * from \"FSRCalc\".frs_new_get_my_fields_yearly_cropCertification(_field_id=>{id},_field_option_id =>1 , yearfrom => {firstValue} , yearto => {firstValue});";
                DataSet DsFieldData3 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryField3);
                ViewBag.Field3 = DsFieldData3;

                ViewBag.RFactorLevels = commonMethods.four_R_Functionlevel(id, 1);
                ViewBag.RFactorLevelsp = commonMethods.four_R_Functionlevel(id, 2);

                if (DropdownFrom != null)
                    return PartialView("~/Views/Common/_Print.cshtml");
                else
                    return null;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Content("error");
            }
        }

        [HttpGet]
        public IActionResult Get_EnvironmentalOutcomesChart(string id, string name, string DropdownFrom, string DropdownTo, string Outcomechart)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                int FieldId = int.Parse(encryptDecryptData.Decryptdata(id));
                ViewBag.FieldId = FieldId;
                ViewBag.FieldName = name;
                // FieldId = 47;
                DataSet currentyeardata = null;

                string valuefrom = string.Empty;
                string valueto = string.Empty;

                string currentyear = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {FieldId});";
                currentyeardata = _masterRepository.GetAndSelectTableItems(currentyear);

                string yearfrom = currentyeardata.Tables[0].Rows[0][0].ToString();
                int lastIndex = currentyeardata.Tables[0].Rows.Count - 1;
                string yearto = currentyeardata.Tables[0].Rows[lastIndex][0].ToString();

                if (DropdownFrom == null && DropdownTo == null)
                {
                    valuefrom = yearfrom;
                    valueto = yearto;
                }
                else if (DropdownFrom != null && DropdownTo == null)
                {
                    valuefrom = DropdownFrom;
                    valueto = DropdownFrom;
                }
                else
                {
                    valuefrom = DropdownFrom;
                    valueto = DropdownTo;
                }

                Dictionary<string, string> factorLevels = commonMethods.four_R_Function(FieldId, 1, 1);

                List<string> factorKeys = new List<string>() { "source", "rate", "timing", "placement" };
                double nfertin = 0;
                foreach (string key in factorKeys)
                {
                    nfertin += commonMethods.nFetinAndpnertin(factorLevels[key]);
                }
                nfertin /= factorKeys.Count;

                // Pfetrin
                Dictionary<string, string> factorLevelsp = commonMethods.four_R_Function(FieldId, 2, 1);


                double pfertin = 0;
                foreach (string key in factorKeys)
                {
                    pfertin += commonMethods.nFetinAndpnertin(factorLevelsp[key]);
                }
                pfertin /= factorKeys.Count;

                int firstValue = Convert.ToInt32(valuefrom);
                int secondValue = Convert.ToInt32(valueto);

                if (Outcomechart == null)
                {
                    Outcomechart = "field_stewardship_rating";
                }
                string environmentChartYearQaury = $"select* from \"FSRCalc\".environment_outcome_Viewhistory_chart(datarefcursor => 'data',_outcomes_column => '{Outcomechart}', _field_id => {FieldId}, _field_option_id => 1,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {firstValue} , _end_year => {secondValue});";
                DataSet environmentChartYearQauryData = _masterRepository.GetAndSelectTableItems(environmentChartYearQaury);

                if (environmentChartYearQauryData != null)
                {
                    return Json(new { success = true, data = JsonConvert.SerializeObject(environmentChartYearQauryData) });
                }
                else
                {
                    return Json(new { success = false, message = "No Data" });
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Json(new { success = false, message = "No Data" });
            }


        }

        public IActionResult FSRChartPopup(string id, string DropdownFrom, string DropdownTo, string chartType)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                int FieldId = int.Parse(encryptDecryptData.Decryptdata(id));
                ViewBag.FieldId = FieldId;
                DataSet currentyeardata = null;

                string valuefrom = string.Empty;
                string valueto = string.Empty;
                string yearfrom = string.Empty;
                int lastIndex = 0;
                string yearto = string.Empty;
                string currentyear = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {FieldId});";
                currentyeardata = _masterRepository.GetAndSelectTableItems(currentyear);

                if (currentyeardata != null && currentyeardata.Tables.Count > 0 && currentyeardata.Tables[0].Rows.Count > 0)
                {
                    yearfrom = currentyeardata.Tables[0].Rows[0][0].ToString();
                    lastIndex = currentyeardata.Tables[0].Rows.Count - 1;
                    yearto = currentyeardata.Tables[0].Rows[lastIndex][0].ToString();
                }

                valuefrom = (DropdownFrom == null && DropdownTo == null) ? yearfrom : (DropdownFrom != null && DropdownTo == null) ? DropdownFrom : DropdownFrom;
                valueto = (DropdownFrom == null && DropdownTo == null) ? yearto : (DropdownFrom != null && DropdownTo == null) ? DropdownFrom : DropdownTo;

                Dictionary<string, string> factorLevels = commonMethods.four_R_Function(FieldId, 1, 1);

                List<string> factorKeys = new List<string>() { "source", "rate", "timing", "placement" };
                double nfertin = 0;
                foreach (string key in factorKeys)
                {
                    nfertin += commonMethods.nFetinAndpnertin(factorLevels[key]);
                }
                nfertin /= factorKeys.Count;

                // Pfetrin
                Dictionary<string, string> factorLevelsp = commonMethods.four_R_Function(FieldId, 2, 1);

                double pfertin = 0;
                foreach (string key in factorKeys)
                {
                    pfertin += commonMethods.nFetinAndpnertin(factorLevelsp[key]);
                }
                pfertin /= factorKeys.Count;

                int firstValue = string.IsNullOrEmpty(valuefrom) ? 0 : Convert.ToInt32(valuefrom);
                int secondValue = string.IsNullOrEmpty(valueto) ? 0 : Convert.ToInt32(valueto);

                #region SoilIndiciesChartData

                DataTable dtSoWaErBeIndata = null;
                DataTable dtSoWaErDeInAgdata = null;
                DataTable dtSoReBeIndata = null;

                if (chartType == "rating_soil")
                {
                    string SoWaErBeInquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'SoWaErBeIn',  _start_year => {firstValue} , _end_year => {secondValue});";
                    DataSet DsSoWaErBeInData = _masterRepository.GetAndSelectTableItems(SoWaErBeInquery);


                    if (DsSoWaErBeInData != null && DsSoWaErBeInData.Tables.Count > 0 && DsSoWaErBeInData.Tables[0].Rows.Count > 0)
                    {
                        dtSoWaErBeIndata = DsSoWaErBeInData.Tables[0];
                    }

                    //string SoFoDeInAgquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'SoFoDeInAg',  _start_year => {firstValue} , _end_year => {secondValue});";
                    //DataSet DsSoFoDeInAgData = _masterRepository.GetAndSelectTableItems(SoFoDeInAgquery);

                    //DataTable dtSoFoDeInAgdata = null;

                    //if (DsSoFoDeInAgData != null && DsSoFoDeInAgData.Tables.Count > 0 && DsSoFoDeInAgData.Tables[0].Rows.Count > 0)
                    //{
                    //    dtSoFoDeInAgdata = DsSoFoDeInAgData.Tables[0];
                    //}

                    //string SoWaErInAgquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'SoWaErInAg',  _start_year => {firstValue} , _end_year => {secondValue});";
                    //DataSet DsSoWaErInAgData = _masterRepository.GetAndSelectTableItems(SoWaErInAgquery);

                    //DataTable dtSoWaErInAgdata = null;

                    //if (DsSoWaErInAgData != null && DsSoWaErInAgData.Tables.Count > 0 && DsSoWaErInAgData.Tables[0].Rows.Count > 0)
                    //{
                    //    dtSoWaErInAgdata = DsSoWaErInAgData.Tables[0];
                    //}

                    string SoWaErDeInAgquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'SoWaErDeInAg',  _start_year => {firstValue} , _end_year => {secondValue});";
                    DataSet DsSoWaErDeInAgData = _masterRepository.GetAndSelectTableItems(SoWaErDeInAgquery);



                    if (DsSoWaErDeInAgData != null && DsSoWaErDeInAgData.Tables.Count > 0 && DsSoWaErDeInAgData.Tables[0].Rows.Count > 0)
                    {
                        dtSoWaErDeInAgdata = DsSoWaErDeInAgData.Tables[0];
                    }

                    string SoReBeInquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'SoReBeIn',  _start_year => {firstValue} , _end_year => {secondValue});";
                    DataSet DsSoReBeInData = _masterRepository.GetAndSelectTableItems(SoReBeInquery);



                    if (DsSoReBeInData != null && DsSoReBeInData.Tables.Count > 0 && DsSoReBeInData.Tables[0].Rows.Count > 0)
                    {
                        dtSoReBeIndata = DsSoReBeInData.Tables[0];
                    }
                }

                #endregion SoilIndiciesChartData

                #region NutrientChartData

                DataTable DtTpMobinagdata = null;
                DataTable dtTpexdeinagdata = null;
                DataTable dtTPReBelndata = null;

                if (chartType == "rating_nutrient")
                {
                    string TpMobinagquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'TpMobinag',  _start_year => {firstValue} , _end_year => {secondValue});";
                    DataSet DsTpMobinagData = _masterRepository.GetAndSelectTableItems(TpMobinagquery);


                    if (DsTpMobinagData != null && DsTpMobinagData.Tables.Count > 0 && DsTpMobinagData.Tables[0].Rows.Count > 0)
                    {
                        DtTpMobinagdata = DsTpMobinagData.Tables[0];
                    }

                    //string TPMobdeinagquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'TPMobdeinag',  _start_year => {firstValue} , _end_year => {secondValue});";
                    //DataSet DsTPMobdeinagData = _masterRepository.GetAndSelectTableItems(TPMobdeinagquery);

                    //DataTable dtTPMobdeinagdata = null;

                    //if (DsTPMobdeinagData != null && DsTPMobdeinagData.Tables.Count > 0 && DsTPMobdeinagData.Tables[0].Rows.Count > 0)
                    //{
                    //    dtTPMobdeinagdata = DsTPMobdeinagData.Tables[0];
                    //}

                    //string Tpexinagquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'Tpexinag',  _start_year => {firstValue} , _end_year => {secondValue});";
                    //DataSet DsTpexinagData = _masterRepository.GetAndSelectTableItems(Tpexinagquery);

                    //DataTable dtTpexinagdata = null;

                    //if (DsTpexinagData != null && DsTpexinagData.Tables.Count > 0 && DsTpexinagData.Tables[0].Rows.Count > 0)
                    //{
                    //    dtTpexinagdata = DsTpexinagData.Tables[0];
                    //}

                    string Tpexdeinagquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'Tpexdeinag',  _start_year => {firstValue} , _end_year => {secondValue});";
                    DataSet DsTpexdeinagData = _masterRepository.GetAndSelectTableItems(Tpexdeinagquery);



                    if (DsTpexdeinagData != null && DsTpexdeinagData.Tables.Count > 0 && DsTpexdeinagData.Tables[0].Rows.Count > 0)
                    {
                        dtTpexdeinagdata = DsTpexdeinagData.Tables[0];
                    }

                    string TPReBelnquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'TPReBeln',  _start_year => {firstValue} , _end_year => {secondValue});";
                    DataSet DsTPReBelnData = _masterRepository.GetAndSelectTableItems(TPReBelnquery);



                    if (DsTPReBelnData != null && DsTPReBelnData.Tables.Count > 0 && DsTPReBelnData.Tables[0].Rows.Count > 0)
                    {
                        dtTPReBelndata = DsTPReBelnData.Tables[0];
                    }
                }

                #endregion NutrientChartData

                #region WaterChartData

                DataTable DtInBelndata = null;
                DataTable dtRuDeInAgdata = null;
                DataTable dtIrrUseEffBeIndata = null;
                bool IrrUseEffBeInflag = false;

                if (chartType == "rating_water")
                {
                    string InBelnquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'InBeln',  _start_year => {firstValue} , _end_year => {secondValue});";
                    DataSet DsInBelnData = _masterRepository.GetAndSelectTableItems(InBelnquery);


                    if (DsInBelnData != null && DsInBelnData.Tables.Count > 0 && DsInBelnData.Tables[0].Rows.Count > 0)
                    {
                        DtInBelndata = DsInBelnData.Tables[0];
                    }

                    //string InDeInAgquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'InDeInAg',  _start_year => {firstValue} , _end_year => {secondValue});";
                    //DataSet DsInDeInAgData = _masterRepository.GetAndSelectTableItems(InDeInAgquery);

                    //DataTable dtInDeInAgdata = null;

                    //if (DsInDeInAgData != null && DsInDeInAgData.Tables.Count > 0 && DsInDeInAgData.Tables[0].Rows.Count > 0)
                    //{
                    //    dtInDeInAgdata = DsInDeInAgData.Tables[0];
                    //}

                    //string RuBeInquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'RuBeIn',  _start_year => {firstValue} , _end_year => {secondValue});";
                    //DataSet DsRuBeInData = _masterRepository.GetAndSelectTableItems(RuBeInquery);

                    //DataTable dtRuBeIndata = null;

                    //if (DsRuBeInData != null && DsRuBeInData.Tables.Count > 0 && DsRuBeInData.Tables[0].Rows.Count > 0)
                    //{
                    //    dtRuBeIndata = DsRuBeInData.Tables[0];
                    //}

                    string RuDeInAgquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'RuDeInAg',  _start_year => {firstValue} , _end_year => {secondValue});";
                    DataSet DsRuDeInAgData = _masterRepository.GetAndSelectTableItems(RuDeInAgquery);



                    if (DsRuDeInAgData != null && DsRuDeInAgData.Tables.Count > 0 && DsRuDeInAgData.Tables[0].Rows.Count > 0)
                    {
                        dtRuDeInAgdata = DsRuDeInAgData.Tables[0];
                    }


                    string IrrUseEffBeInquery = $"select * from \"FSRCalc\".fsr_new_calc_Soil_Indicies_chart(datarefcursor :='data' ,_field_id =>{FieldId},_bubble_type =>'IrrUseEffBeIn',  _start_year => {firstValue} , _end_year => {secondValue});";
                    DataSet DsIrrUseEffBeInData = _masterRepository.GetAndSelectTableItems(IrrUseEffBeInquery);



                    if (DsIrrUseEffBeInData != null && DsIrrUseEffBeInData.Tables.Count > 0 && DsIrrUseEffBeInData.Tables[0].Rows.Count > 0)
                    {
                        dtIrrUseEffBeIndata = DsIrrUseEffBeInData.Tables[0];
                        int irrigatedValue = Convert.ToInt32(dtIrrUseEffBeIndata.Rows[0]["irrigated"]);
                        IrrUseEffBeInflag = irrigatedValue == 1 ? true : false;

                    }
                }

                #endregion WaterChartData

                #region SurfaceWaterQualityChartData


                Dictionary<string, string> sed_goal_indexdata = new Dictionary<string, string>();
                Dictionary<string, string> Option1sed_goal_indexdata = new Dictionary<string, string>();
                Dictionary<string, string> Option2sed_goal_indexdata = new Dictionary<string, string>();
                Dictionary<string, string> Alfalfased_goal_indexdata = new Dictionary<string, string>();
                Dictionary<string, string> Moldsed_goal_indexdata = new Dictionary<string, string>();
                Dictionary<string, string> tp_goal_indexdata = new Dictionary<string, string>();
                Dictionary<string, string> Option1tp_goal_indexdata = new Dictionary<string, string>();
                Dictionary<string, string> Option2tp_goal_indexdata = new Dictionary<string, string>();
                Dictionary<string, string> Alfalfatp_goal_indexdata = new Dictionary<string, string>();
                Dictionary<string, string> Moldtp_goal_indexdata = new Dictionary<string, string>();
                if (chartType == "rating_surface")
                {
                    sed_goal_indexdata = commonMethods.Surface_chart_Function(FieldId, firstValue, secondValue, 1, "sedfromfield");
                    Option1sed_goal_indexdata = commonMethods.Surface_chart_Function(FieldId, firstValue, secondValue, 3, "sedfromfield");
                    Option2sed_goal_indexdata = commonMethods.Surface_chart_Function(FieldId, firstValue, secondValue, 4, "sedfromfield");
                    Alfalfased_goal_indexdata = commonMethods.Surface_chart_Function(FieldId, firstValue, secondValue, 5, "sedfromfield");
                    Moldsed_goal_indexdata = commonMethods.Surface_chart_Function(FieldId, firstValue, secondValue, 6, "sedfromfield");
                    tp_goal_indexdata = commonMethods.Surface_chart_Function(FieldId, firstValue, secondValue, 1, "tpfromfield");
                    Option1tp_goal_indexdata = commonMethods.Surface_chart_Function(FieldId, firstValue, secondValue, 3, "tpfromfield");
                    Option2tp_goal_indexdata = commonMethods.Surface_chart_Function(FieldId, firstValue, secondValue, 4, "tpfromfield");
                    Alfalfatp_goal_indexdata = commonMethods.Surface_chart_Function(FieldId, firstValue, secondValue, 5, "tpfromfield");
                    Moldtp_goal_indexdata = commonMethods.Surface_chart_Function(FieldId, firstValue, secondValue, 6, "tpfromfield");
                }
                #endregion SurfaceWaterQualityChartData

                #region Fertilizer Management
                Dictionary<string, string> DtNitrogenFertilizerLevels = null;
                Dictionary<string, string> DtOption1NitrogenFertilizerLevels = null;
                Dictionary<string, string> DtOption2NitrogenFertilizerLevels = null;
                Dictionary<string, string> DtAlfalfaNitrogenFertilizerLevels = null;
                Dictionary<string, string> DtMoldNitrogenFertilizerLevels = null;
                Dictionary<string, string> DtPhosphorusFactorLevels = null;
                Dictionary<string, string> DtOption1PhosphorusFertilizerLevels = null;
                Dictionary<string, string> DtOption2PhosphorusFertilizerLevels = null;
                Dictionary<string, string> DtAlfalfaPhosphorusFertilizerLevels = null;
                Dictionary<string, string> DtMoldPhosphorusFertilizerLevels = null;
                if (chartType == "rating_fertilizer")
                {
                    DtNitrogenFertilizerLevels = commonMethods.four_R_chart_Function(FieldId,firstValue,secondValue,1, 1);
                    DtOption1NitrogenFertilizerLevels = commonMethods.four_R_chart_Function(FieldId, firstValue, secondValue, 3,1);
                    DtOption2NitrogenFertilizerLevels = commonMethods.four_R_chart_Function(FieldId, firstValue, secondValue, 4,1);
                    DtAlfalfaNitrogenFertilizerLevels = commonMethods.four_R_chart_Function(FieldId, firstValue, secondValue, 5, 1);
                    DtMoldNitrogenFertilizerLevels = commonMethods.four_R_chart_Function(FieldId, firstValue, secondValue, 6, 1);
                    DtPhosphorusFactorLevels = commonMethods.four_R_chart_Function(FieldId, firstValue, secondValue,1, 2);
                    DtOption1PhosphorusFertilizerLevels = commonMethods.four_R_chart_Function(FieldId, firstValue, secondValue, 3,2);
                    DtOption2PhosphorusFertilizerLevels = commonMethods.four_R_chart_Function(FieldId, firstValue, secondValue, 4,2);
                    DtAlfalfaPhosphorusFertilizerLevels = commonMethods.four_R_chart_Function(FieldId, firstValue, secondValue, 5, 2);
                    DtMoldPhosphorusFertilizerLevels = commonMethods.four_R_chart_Function(FieldId, firstValue, secondValue, 6, 2);
                }

                #endregion Fertilizer Management

                SoilIndiciesChartData myObject = new SoilIndiciesChartData();
                myObject.ChartType = chartType;
                myObject.SoWaErBeIndata = dtSoWaErBeIndata;
                //myObject.SoFoDeInAgdata = dtSoFoDeInAgdata;
                //myObject.SoWaErInAg = dtSoWaErInAgdata;
                myObject.SoWaErDeInAg = dtSoWaErDeInAgdata;
                myObject.SoReBeIn = dtSoReBeIndata;
                myObject.TpMobinag = DtTpMobinagdata;
                //myObject.TPMobdeinag = dtTPMobdeinagdata;
                //myObject.Tpexinag = dtTpexinagdata;
                myObject.Tpexdeinag = dtTpexdeinagdata;
                myObject.TPReBeln = dtTPReBelndata;
                myObject.InBeln = DtInBelndata;
                //myObject.InDeInAg = dtInDeInAgdata;
                //myObject.RuBeIn = dtRuBeIndata;
                myObject.RuDeInAg = dtRuDeInAgdata;
                myObject.IrrUseEffBeIn = dtIrrUseEffBeIndata;
                myObject.IrrUseEffBeInflag = IrrUseEffBeInflag;
                myObject.SoilLossfromField = sed_goal_indexdata;
                myObject.Option1SoilLossfromField = Option1sed_goal_indexdata;
                myObject.Option2SoilLossfromField = Option2sed_goal_indexdata;
                myObject.AlfalfaSoilLossfromField = Alfalfased_goal_indexdata;
                myObject.MoldSoilLossfromField = Moldsed_goal_indexdata;
                myObject.PhosphorusLossfromField = tp_goal_indexdata;
                myObject.Option1PhosphorusLossfromField = Option1tp_goal_indexdata;
                myObject.Option2PhosphorusLossfromField = Option2tp_goal_indexdata;
                myObject.AlfalfaPhosphorusLossfromField = Alfalfatp_goal_indexdata;
                myObject.MoldPhosphorusLossfromField = Moldtp_goal_indexdata;
                myObject.NitrogenFertilizer = DtNitrogenFertilizerLevels;
                myObject.Option1NitrogenFertilizer = DtOption1NitrogenFertilizerLevels;
                myObject.Option2NitrogenFertilizer = DtOption2NitrogenFertilizerLevels;
                myObject.AlfalfaNitrogenFertilizer = DtAlfalfaNitrogenFertilizerLevels;
                myObject.MoldNitrogenFertilizer = DtMoldNitrogenFertilizerLevels;
                myObject.PhosphorusFertilizer = DtPhosphorusFactorLevels;
                myObject.Option1PhosphorusFertilizer = DtOption1PhosphorusFertilizerLevels;
                myObject.Option2PhosphorusFertilizer = DtOption2PhosphorusFertilizerLevels;
                myObject.AlfalfaPhosphorusFertilizer = DtAlfalfaPhosphorusFertilizerLevels;
                myObject.MoldPhosphorusFertilizer = DtMoldPhosphorusFertilizerLevels;

                if (chartType == "rating_nutrient")
                {
                    if (myObject.TpMobinag != null && myObject.TpMobinag.Rows.Count > 0 ||
                             myObject.Tpexdeinag != null && myObject.Tpexdeinag.Rows.Count > 0 ||
                        myObject.TPReBeln != null && myObject.TPReBeln.Rows.Count > 0)
                    {
                        // Return the partial view to display the calculated data
                        return PartialView("~/Views/Admin/_SoliIndicieschart.cshtml", myObject);
                    }
                    else
                    {
                        return null;
                    }
                }

                if (chartType == "rating_water")
                {
                    if (myObject.InBeln != null && myObject.InBeln.Rows.Count > 0 ||
                        myObject.RuDeInAg != null && myObject.RuDeInAg.Rows.Count > 0 ||
                        myObject.IrrUseEffBeIn != null && myObject.IrrUseEffBeIn.Rows.Count > 0)
                    {
                        // Return the partial view to display the calculated data
                        return PartialView("~/Views/Admin/_SoliIndicieschart.cshtml", myObject);
                    }
                    else
                    {
                        return null;
                    }
                }

                if (chartType == "rating_surface")
                {
                    if (myObject.SoilLossfromField != null || myObject.Option1SoilLossfromField != null || myObject.Option2SoilLossfromField != null || myObject.AlfalfaSoilLossfromField != null || myObject.MoldSoilLossfromField != null ||
                        myObject.PhosphorusLossfromField != null || myObject.Option1PhosphorusLossfromField != null || myObject.Option2PhosphorusLossfromField != null || myObject.AlfalfaPhosphorusLossfromField != null || myObject.MoldPhosphorusLossfromField != null)
                    {
                        // Return the partial view to display the calculated data
                        return PartialView("~/Views/Admin/_SoliIndicieschart.cshtml", myObject);
                    }
                    else
                    {
                        return null;
                    }
                }

                if (chartType == "rating_fertilizer")
                {
                    if (myObject.NitrogenFertilizer != null || myObject.Option1NitrogenFertilizer != null || myObject.Option2NitrogenFertilizer != null || myObject.AlfalfaNitrogenFertilizer != null || myObject.MoldNitrogenFertilizer != null ||
                       myObject.PhosphorusFertilizer != null || myObject.Option1PhosphorusFertilizer != null || myObject.Option2PhosphorusFertilizer != null || myObject.AlfalfaPhosphorusFertilizer != null || myObject.MoldPhosphorusFertilizer != null)
                    {
                        // Return the partial view to display the calculated data
                        return PartialView("~/Views/Admin/_SoliIndicieschart.cshtml", myObject);
                    }
                    else
                    {
                        return null;
                    }
                }


                if (myObject.SoWaErBeIndata != null && myObject.SoWaErBeIndata.Rows.Count > 0 ||
                myObject.SoWaErDeInAg != null && myObject.SoWaErDeInAg.Rows.Count > 0 ||
                myObject.SoReBeIn != null && myObject.SoReBeIn.Rows.Count > 0)
                {
                    // Return the partial view to display the calculated data
                    return PartialView("~/Views/Admin/_SoliIndicieschart.cshtml", myObject);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool SendEmailToNewlyCreatedUser(string emails, string pass, string fName, string lName)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            try
            {
                string subject = string.Empty;
                string body = string.Empty;
                string emailFrom = string.Empty;

                string SiteUrl = RootDirectoryPath.SiteURL;
                string LogoImgUrl = String.Empty;
                //string LogoImgUrl = SiteUrl + "/images/logo_wayfindr.png";

                using (StreamReader reader = new StreamReader(RootDirectoryPath.WebRootPath + "\\EmailTemplates\\NewUserCreationEmailTemplate.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{SiteUrl}", SiteUrl).Replace("{LogoImgUrl}", LogoImgUrl);

                subject = "Account Created for " + emails;
                body = body.Replace("{Title}", "NewUser Account");
                body = body.Replace("{Name}", fName + " " + lName);
                //body = body.Replace("{Body}", "Welcome! " + (fName + ' ' + lName) + ", your account is ready for use. Use the Password " + pass);

                //body = body.Replace("{PrimaryButtonUrl}", SiteUrl + "/Login").Replace("{PrimaryButtonText}", "Log In");
                body = body.Replace("{Link}", SiteUrl + "/Login").Replace("{Link}", "Log In");

                //body = body.Replace("{AlternateActionUrl}", SiteUrl + "/Login");

                //body = body.Replace("{FooterBody}", "");

                int emailRetryCount = 5;
                bool failed = false;
                do
                {
                    try
                    {
                        failed = false;
                        return _masterRepository.SendMail(emails, subject, body, emailFrom);
                    }
                    catch (Exception ex)
                    {
                        failed = true;
                        emailRetryCount--;
                        Log.LogError(ex);
                    }
                } while (failed && emailRetryCount != 0);
                return true;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return false;
            }
        }

        public bool SendEmailToNewlyCreatedProducer(string emails, string pass, string fName, string lName)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            try
            {
                string subject = string.Empty;
                string body = string.Empty;
                string emailFrom = string.Empty;

                string SiteUrl = RootDirectoryPath.SiteURL;
                string LogoImgUrl = String.Empty;
                //string LogoImgUrl = SiteUrl + "/images/logo_wayfindr.png";

                using (StreamReader reader = new StreamReader(RootDirectoryPath.WebRootPath + "\\EmailTemplates\\NewProducerCreationEmailTemplate.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{SiteUrl}", SiteUrl).Replace("{LogoImgUrl}", LogoImgUrl);

                subject = "Wayfindr - Producer Account Created";
                body = body.Replace("{Title}", "NewProducer Account");
                body = body.Replace("{Name}", fName + " " + lName);
                //body = body.Replace("{Body}", "Welcome! " + (fName + ' ' + lName) + ", your account is ready for use. Use the Password " + pass);
                body = body.Replace("{UserName}", emails);
                body = body.Replace("{UserPassword}", pass);

                //body = body.Replace("{PrimaryButtonUrl}", SiteUrl + "/Login").Replace("{PrimaryButtonText}", "Log In");
                body = body.Replace("{Link}", SiteUrl + "/Login").Replace("{Link}", "Log In");

                //body = body.Replace("{AlternateActionUrl}", SiteUrl + "/Login");

                //body = body.Replace("{FooterBody}", "");

                int emailRetryCount = 5;
                bool failed = false;
                do
                {
                    try
                    {
                        failed = false;
                        return _masterRepository.SendMail(emails, subject, body, emailFrom);
                    }
                    catch (Exception ex)
                    {
                        failed = true;
                        emailRetryCount--;
                        Log.LogError(ex);
                    }
                } while (failed && emailRetryCount != 0);
                return true;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return false;
            }
        }

        public IActionResult Advisors()
        {
            IMasterRepository _masterRepository = new MasterRepository();
            object[] userParams = { "_ActionType" };
            object[] userValues = { "AllAdvisor" };

            string query = $"SELECT * from \"Users\".water_advisors_ref (datarefcursor => 'data',{userParams[0]} => '{userValues[0]}');";
            DataSet DsAdvisorData = _masterRepository.GetAndSelectTableItems(query);
            ViewBag.Advisor = DsAdvisorData;
            ViewBag.UserRole = LoginUserRoleId;
            return View();

        }

        [HttpGet]
        //[Route("/{controller}/{action}/{Id}")]
        public IActionResult AdvisorAddEdit(string id)
        {
            if (id == "0") { ViewBag.UserId = id; }
            else { ViewBag.UserId = encryptDecryptData.Decryptdata(id); }

            IMasterRepository _masterRepository = new MasterRepository();
            try
            {
                ViewBag.StateOptions = CommonMethods.GetStateOptions(includeCode: true);

                if (Convert.ToInt32(ViewBag.UserId) > 0)
                {
                    object[] userParams = { "_ActionType", "_advisor_id" };
                    object[] userValues = { "GetAdvisorById", ViewBag.UserId };

                    string query = $"SELECT* from \"Users\".water_advisors_ref(datarefcursor => 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => '{userValues[1]}');";

                    DataSet DsAdvisorData = _masterRepository.GetAndSelectTableItems(query);

                    Advisors advisor = new Advisors
                    {
                        AdvisorsId = (int)DsAdvisorData.Tables[0].Rows[0]["advisor_id"],
                        FirstName = Convert.ToString(DsAdvisorData.Tables[0].Rows[0]["first_name"]),
                        LastName = Convert.ToString(DsAdvisorData.Tables[0].Rows[0]["last_name"]),
                        Email = Convert.ToString(DsAdvisorData.Tables[0].Rows[0]["email"]),
                        LoginId = (int)DsAdvisorData.Tables[0].Rows[0]["login_id"],
                        Address1 = Convert.ToString(DsAdvisorData.Tables[0].Rows[0]["address1"]),
                        Address2 = Convert.ToString(DsAdvisorData.Tables[0].Rows[0]["address2"]),
                        City = Convert.ToString(DsAdvisorData.Tables[0].Rows[0]["city"]),
                        State = Convert.ToString(DsAdvisorData.Tables[0].Rows[0]["state"]),
                        ZipCode = Convert.ToString(DsAdvisorData.Tables[0].Rows[0]["zip_code"]),
                        PhoneMobile = Convert.ToString(DsAdvisorData.Tables[0].Rows[0]["phone_mobile"]),
                    };
                    return View(advisor);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        [HttpPost]
        public IActionResult AdvisorAddEdit(Advisors advisor)
        {
            if (ModelState.IsValid)
            {
                IMasterRepository _masterRepository = new MasterRepository();
                try
                {
                    if (string.IsNullOrEmpty(advisor.AdvisorsId.ToString()) || advisor.AdvisorsId.ToString() == "0")
                    {
                        advisor.AdvisorsId = 0;
                    }
                    if (string.IsNullOrEmpty(advisor.LoginId.ToString()) || advisor.LoginId.ToString() == "0")
                    {
                        advisor.LoginId = 0;
                    }
                    //split _phone_mobile
                    string _phone_mobile = Regex.Replace(advisor.PhoneMobile, @"[^0-9]", "");
                    object[] userParams = { "_advisor_id", "_first_name", "_last_name", "_email", "_address1", "_city", "_state", "_zip_code", "_phone_mobile", "_login_id" };
                    object[] userValues = { advisor.AdvisorsId, advisor.FirstName, advisor.LastName, advisor.Email, advisor.Address1, advisor.City, advisor.State, advisor.ZipCode, _phone_mobile, advisor.LoginId };
                    object[] sqlDbTypes = { "abc" };
                    string query = $"SELECT \"Users\".water_updateadvisors_ref(datarefcursor => 'data',{userParams[0]} => {userValues[0]},{userParams[1]} => '{userValues[1]}'" +
                        $",{userParams[2]} => '{userValues[2]}',{userParams[3]} => '{userValues[3]}',{userParams[4]} => '{userValues[4]}'," +
                        $"{userParams[5]} => '{userValues[5]}',{userParams[6]} => '{userValues[6]}',{userParams[7]} => '{userValues[7]}',{userParams[8]} => '{userValues[8]}' ,{userParams[9]} => {userValues[9]});";
                    long? EditAdvisorData = _masterRepository.InsertUpdate(query);


                    //// new user created mail---
                    //if (Convert.ToBoolean(EditAdvisorData))
                    //{
                    //    _CommonController.SendEmailToNewlyCreatedUser(advisor.Email, string.Empty, advisor.FirstName, advisor.LastName);
                    //    Console.WriteLine("Email send by chnage password method Done.");
                    //}
                    if (EditAdvisorData == -2)
                    {
                        ModelState.AddModelError("Email", CommonMethods.GetEnumDescription(ValidationMessages.EmailAlreadyExists));
                        ViewBag.StateOptions = CommonMethods.GetStateOptions(includeCode: true);
                        return View(advisor);
                    }
                    else if (EditAdvisorData > 0)
                    {
                        var enumValue = ValidationMessages.AdvisorSavedSuccessfully;
                        TempData["Success"] = CommonMethods.GetEnumDescription(enumValue);
                        return RedirectToAction("Advisors", "Common");
                    }
                    else
                    {
                        return RedirectToAction("Error", "Common");
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError(ex);
                    return RedirectToAction("Error", "Common");
                }
            }
            else
            {
                TempData["Error"] = CommonMethods.GetEnumDescription(ValidationMessages.ProperValues);
                ViewBag.StateOptions = CommonMethods.GetStateOptions(includeCode: true);
                return View();
            }
        }

        public IActionResult Producers(string id)
        {
            int UserRole = LoginUserRoleId;
            ViewBag.UserRole = LoginUserRoleId;
            IMasterRepository _masterRepository = new MasterRepository();
            object[] userParams = { "_ActionType", "_producer_Id", "_advisor_id" };
            object[] userValues = { "GetAllProducers" }; //GetProducerById
            object[] userValuesbyadv = { "GetAllProducersForAdvisor", LoginUserId }; //Producerbyadvisor

            if (id != null)
            {
                encryptDecryptData = new EncryptDecryptData();
                string advisorId = encryptDecryptData.Decryptdata(id);
                object[] userParams1 = { "_ActionType", "_advisor_id" };
                object[] userValues1 = { "GetAllProducersForAdvisor", advisorId };

                string query = $"SELECT * from \"Users\".water_producersdata_ref (datarefcursor => 'data',{userParams1[0]} => '{userValues1[0]}',{userParams1[1]} => {userValues1[1]});";
                DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.Producer = DsProducerData;
                return View();
            }
            else if (UserRole == (int)UserTypeEnum.Admin)
            {
                string query = $"SELECT * from \"Users\".water_producersdata_ref (datarefcursor => 'data',{userParams[0]} => '{userValues[0]}');";
                DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.Producer = DsProducerData;
                return View();
            }
            else if (UserRole == (int)UserTypeEnum.Agronomist)
            {
                string query = $"SELECT * from \"Users\".water_producersdata_ref (datarefcursor => 'data',{userParams[0]} => '{userValuesbyadv[0]}',{userParams[2]} => '{userValuesbyadv[2]}');";
                DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.Producer = DsProducerData;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "WebApp");
            }
        }

        [HttpGet]
        public IActionResult ProducersAddEdit(string id)
        {
            if (id == "0") { ViewBag.UserId = id; }
            else { ViewBag.UserId = encryptDecryptData.Decryptdata(id); }

            ViewBag.UserRole = LoginUserRoleId;
            IMasterRepository _masterRepository = new MasterRepository();
            try
            {
                ViewBag.StateOptions = CommonMethods.GetStateOptions(includeCode: true);

                ViewBag.Advisors = (from Advisors advisor in commonMethods.GetAdvisorDropDown()
                                    select new SelectListItem
                                    {
                                        Text = advisor.FirstName + " " + advisor.LastName,
                                        Value = Convert.ToString(advisor.AdvisorsId)
                                    }).ToList();
                if (Convert.ToInt32(ViewBag.UserId) > 0)
                {
                    object[] userParams = { "_ActionType", "_producer_id" };
                    object[] userValues = { "GetProducerById", ViewBag.UserId };

                    string query = $"SELECT * from \"Users\".water_producersdata_ref (datarefcursor => 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => '{userValues[1]}');";
                    DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(query);

                    Producers producer = new Producers
                    {
                        ProducerId = (int)DsProducerData.Tables[0].Rows[0]["producer_id"],
                        FirstName = Convert.ToString(DsProducerData.Tables[0].Rows[0]["first_name"]),
                        LastName = Convert.ToString(DsProducerData.Tables[0].Rows[0]["last_name"]),
                        Email = Convert.ToString(DsProducerData.Tables[0].Rows[0]["email"]),
                        LoginId = (int)DsProducerData.Tables[0].Rows[0]["login_id"],
                        StreetAddress = Convert.ToString(DsProducerData.Tables[0].Rows[0]["street_address"]),
                        City = Convert.ToString(DsProducerData.Tables[0].Rows[0]["city"]),
                        StateName = Convert.ToString(DsProducerData.Tables[0].Rows[0]["state"]),
                        Zip = Convert.ToString(DsProducerData.Tables[0].Rows[0]["zip"]),
                        PhoneNumber = Convert.ToString(DsProducerData.Tables[0].Rows[0]["phone_number"]),
                        //AgronomistId = (int)DsProducerData.Tables[0].Rows[0]["agronomist_id"],
                        AgronomistId = (int)DsProducerData.Tables[0].Rows[0]["advisor_id"],
                    };
                    return View(producer);
                }
                else
                {
                    if (LoginUserRoleId == (int)UserTypeEnum.Admin)
                    {
                        Producers producer = new Producers
                        {
                            AdvisorId = 0
                        };
                        return View(producer);
                    }
                    else
                    {
                        Producers producer = new Producers
                        {
                            AgronomistId = AdvisorId
                        };
                        return View(producer);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        [HttpPost]
        public IActionResult ProducersAddEdit(Producers producer)
        {
            if (ModelState.IsValid)
            {
                IMasterRepository _masterRepository = new MasterRepository();
                try
                {
                    if (string.IsNullOrEmpty(producer.ProducerId.ToString()) || producer.ProducerId.ToString() == "0")
                    {
                        producer.ProducerId = 0;
                    }
                    if (string.IsNullOrEmpty(producer.LoginId.ToString()) || producer.LoginId.ToString() == "0")
                    {
                        producer.LoginId = 0;
                    }
                    if (LoginUserRoleId == (int)UserTypeEnum.Agronomist)
                    {
                        producer.AgronomistId = AdvisorId;
                    }
                    //split _phone_number
                    string _phone_number = Regex.Replace(producer.PhoneNumber, @"[^0-9]", "");
                    object[] userParams = { "_producer_id", "_first_name", "_last_name", "_email", "_street_address", "_city", "_state", "_zip", "_phone_number", "_login_id", "_agronomist_id" };
                    object[] userValues = { producer.ProducerId, producer.FirstName, producer.LastName, producer.Email, producer.StreetAddress, producer.City, producer.StateName, producer.Zip, _phone_number, producer.LoginId, producer.AgronomistId };
                    object[] sqlDbTypes = { "abc" };
                    string query = $"SELECT * FROM \"Users\".water_updateproducers_ref(datarefcursor := 'data',{userParams[0]} => {userValues[0]},{userParams[1]} => '{userValues[1]}'" +
                        $",{userParams[2]} => '{userValues[2]}',{userParams[3]} => '{userValues[3]}',{userParams[4]} => '{userValues[4]}'," +
                        $"{userParams[5]} => '{userValues[5]}',{userParams[6]} => '{userValues[6]}',{userParams[7]} => '{userValues[7]}',{userParams[8]} => '{userValues[8]}' ,{userParams[9]} => {userValues[9]},{userParams[10]} => {userValues[10]});";

                    long? EditAdvisorData = _masterRepository.InsertUpdate(query);

                    //// new user created mail---
                    //if (Convert.ToBoolean(EditAdvisorData))
                    //{
                    //    _CommonController.SendEmailToNewlyCreatedUser(advisor.Email, string.Empty, advisor.FirstName, advisor.LastName);
                    //    Console.WriteLine("Email send by chnage password method Done.");
                    //}
                    int UserRole = LoginUserRoleId;
                    if (EditAdvisorData == -2)
                    {
                        ViewBag.UserRole = LoginUserRoleId;
                        ViewBag.StateOptions = CommonMethods.GetStateOptions(includeCode: true);

                        ViewBag.Advisors = (from Advisors advisor in commonMethods.GetAdvisorDropDown()
                                            select new SelectListItem
                                            {
                                                Text = advisor.FirstName + " " + advisor.LastName,
                                                Value = Convert.ToString(advisor.AdvisorsId)
                                                //Value = advisor.FirstName
                                            }).ToList();

                        ModelState.AddModelError("Email", CommonMethods.GetEnumDescription(ValidationMessages.EmailAlreadyExists));
                        return View(producer);
                    }
                    else if (EditAdvisorData > 0 && UserRole == (int)UserTypeEnum.Admin)
                    {
                        TempData["Success"] = CommonMethods.GetEnumDescription(ValidationMessages.ProducerSavedSuccessfully);
                        return RedirectToAction("Producers", "Common");
                    }
                    else if (EditAdvisorData > 0 && UserRole == (int)UserTypeEnum.Agronomist)
                    {
                        TempData["Success"] = CommonMethods.GetEnumDescription(ValidationMessages.ProducerSavedSuccessfully);
                        return RedirectToAction("Landing", "Advisor");
                    }
                    else
                    {
                        return RedirectToAction("Error", "Common");
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError(ex);
                    return RedirectToAction("Error", "Common");
                }
            }
            else
            {
                TempData["Error"] = CommonMethods.GetEnumDescription(ValidationMessages.ProperValues);
                ViewBag.StateOptions = CommonMethods.GetStateOptions(includeCode: true);
                return View();
            }
        }

        public bool SendEmailToResetPassowrd(string emails, string pass, string fName, string lName)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            try
            {
                string subject = string.Empty;
                string body = string.Empty;
                string emailFrom = string.Empty;

                string SiteUrl = RootDirectoryPath.SiteURL;
                string LogoImgUrl = String.Empty;
                //string LogoImgUrl = SiteUrl + "/images/logo_wayfindr.png";

                using (StreamReader reader = new StreamReader(RootDirectoryPath.WebRootPath + "\\EmailTemplates\\ResetPasswordEmailTemplate.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{SiteUrl}", SiteUrl).Replace("{LogoImgUrl}", LogoImgUrl);

                subject = "Wayfindr - Reset password";
                body = body.Replace("{Title}", "Reset Password");
                body = body.Replace("{UserPassword}", pass);
                body = body.Replace("{Name}", fName + " " + lName);

                body = body.Replace("{Link}", SiteUrl + "/Login");
                //body = body.Replace("{Link}", SiteUrl + "login");

                //body = body.Replace("{AlternateActionUrl}", SiteUrl + "/Login");

                //body = body.Replace("{FooterBody}", "");

                int emailRetryCount = 5;
                bool failed = false;
                do
                {
                    try
                    {
                        failed = false;
                        return _masterRepository.SendMail(emails, subject, body, emailFrom);
                    }
                    catch (Exception ex)
                    {
                        failed = true;
                        emailRetryCount--;
                        Log.LogError(ex);
                    }
                } while (failed && emailRetryCount != 0);
                return true;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return false;
            }
        }

        public IActionResult AdvisorPlans()
        {
            return View();
        }

        public IActionResult Resources()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FinancialResultChartsData(int fieldId, int zydid)
        {
            try
            {
                DataSet data = commonMethods.FinancialDetailsChart(fieldId);
                IMasterRepository _masterRepository = new MasterRepository();

                List<ChartData> chartdata = new List<ChartData>();
                double total = 0, totalzydid = 0;
                bool hasRows = data.Tables.Cast<DataTable>()
                               .Any(table => table.Rows.Count != 0);
                if (data != null && data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
                {
                    if (hasRows)
                    {
                        chartdata = (from DataRow dr in data.Tables[0].Rows
                                     select new ChartData()
                                     {
                                         Acres = dr["acre"] != DBNull.Value ? Convert.ToDouble(dr["acre"]) : 0,

                                         TotalDirectCost = dr["totaldirectcost"] != DBNull.Value ? Convert.ToDouble(dr["totaldirectcost"]) : 0,
                                         TotalInDirectCost = dr["totalindirectcost"] != DBNull.Value ? Convert.ToDouble(dr["totalindirectcost"]) : 0,
                                         TotalProductionCost = dr["totalproductioncost"] != DBNull.Value ? Convert.ToDouble(dr["totalproductioncost"]) : 0,

                                         //Graph-IndirectCost
                                         Labor = dr["labor"] != DBNull.Value ? Convert.ToDouble(dr["labor"]) : 0,
                                         Leases = dr["leases"] != DBNull.Value ? Convert.ToDouble(dr["leases"]) : 0,
                                         Farm_insurance = dr["farm_insurance"] != DBNull.Value ? Convert.ToDouble(dr["farm_insurance"]) : 0,
                                         Dues_Fees = dr["Dues_Fees"] != DBNull.Value ? Convert.ToDouble(dr["Dues_Fees"]) : 0,
                                         Utilities = dr["utilities"] != DBNull.Value ? Convert.ToDouble(dr["utilities"]) : 0,
                                         Interest = dr["interest"] != DBNull.Value ? Convert.ToDouble(dr["interest"]) : 0,
                                         Machine_bldg_depreciation = dr["machine_bldg_depreciation"] != DBNull.Value ? Convert.ToDouble(dr["machine_bldg_depreciation"]) : 0,
                                         Real_estate_taxes = dr["real_estate_taxes"] != DBNull.Value ? Convert.ToDouble(dr["real_estate_taxes"]) : 0,
                                         Miscellaneous = dr["miscellaneous"] != DBNull.Value ? Convert.ToDouble(dr["miscellaneous"]) : 0,

                                         //Graph- DirectCost
                                         Fartilizer = dr["fartilizer"] != DBNull.Value ? Convert.ToDouble(dr["fartilizer"]) : 0,
                                         Crop_Chemicals = dr["crop_Chemicals"] != DBNull.Value ? Convert.ToDouble(dr["crop_Chemicals"]) : 0,
                                         Other_Crop = dr["other_Crop"] != DBNull.Value ? Convert.ToDouble(dr["other_Crop"]) : 0,
                                         Energy = dr["energy"] != DBNull.Value ? Convert.ToDouble(dr["energy"]) : 0,
                                         Repair_Maintenance = dr["repair_maintenance"] != DBNull.Value ? Convert.ToDouble(dr["repair_maintenance"]) : 0,
                                         Custom_Hire = dr["custom_hire"] != DBNull.Value ? Convert.ToDouble(dr["custom_hire"]) : 0,
                                         Hired_Labor = dr["hired_labor"] != DBNull.Value ? Convert.ToDouble(dr["hired_labor"]) : 0,
                                         Irrigation = dr["irrigation"] != DBNull.Value ? Convert.ToDouble(dr["irrigation"]) : 0,
                                         Other = dr["other"] != DBNull.Value ? Convert.ToDouble(dr["other"]) : 0,

                                         //NetReturn and Repayment box
                                         BrkEvenBushelNetreturn = dr["brkevenbushelnetreturn"] != DBNull.Value ? Convert.ToDouble(dr["brkevenbushelnetreturn"]) : 0,
                                         BrkEvenYieldNetreturn = dr["brkevenyieldnetreturn"] != DBNull.Value ? Convert.ToDouble(dr["brkevenyieldnetreturn"]) : 0,
                                         BrkEvenBushelRepayment = dr["brkevenbushelrepayment"] != DBNull.Value ? Convert.ToDouble(dr["brkevenbushelrepayment"]) : 0,
                                         BrkEvenYieldRepayment = dr["brkevenyieldrepayment"] != DBNull.Value ? Convert.ToDouble(dr["brkevenyieldrepayment"]) : 0
                                     }).ToList();
                    }
                    total = chartdata.Sum(x => x.TotalDirectCost + x.TotalInDirectCost + x.TotalProductionCost + x.Labor + x.Leases + x.Farm_insurance + x.Dues_Fees + x.Utilities + x.Interest + x.Machine_bldg_depreciation + x.Real_estate_taxes + x.Miscellaneous + x.Fartilizer + x.Crop_Chemicals + x.Other_Crop + x.Energy + x.Repair_Maintenance + x.Custom_Hire + x.Hired_Labor + x.Irrigation + x.Other + x.BrkEvenBushelNetreturn + x.BrkEvenYieldNetreturn + x.BrkEvenBushelRepayment + x.BrkEvenYieldRepayment);
                }

                string Financial_tabelQuery = $"select * from \"FSRCalc\".water_myfielddetails_financial_table(datarefcursor=>'data',_actiontype =>'GetFinancialByZId', _field_id=>{fieldId},_Zyd_id=>{zydid});";
                DataSet financialReturn_data = _masterRepository.GetAndSelectTableItems(Financial_tabelQuery);
                List<ChartData> chartzydiddata = new List<ChartData>();
                bool hasZydidRows = financialReturn_data.Tables.Cast<DataTable>()
                               .Any(table => table.Rows.Count != 0);
                if (financialReturn_data != null && financialReturn_data.Tables.Count > 0 && financialReturn_data.Tables[0].Rows.Count > 0)
                {
                    if (hasZydidRows)
                    {
                        chartzydiddata = (from DataRow dr in financialReturn_data.Tables[0].Rows
                                          select new ChartData()
                                          {

                                              TotalDirectCost = dr["totaldirectcost"] != DBNull.Value ? Convert.ToDouble(dr["totaldirectcost"]) : 0,
                                              TotalInDirectCost = dr["totalindirectcost"] != DBNull.Value ? Convert.ToDouble(dr["totalindirectcost"]) : 0,
                                              TotalProductionCost = dr["totalproductioncost"] != DBNull.Value ? Convert.ToDouble(dr["totalproductioncost"]) : 0,

                                              //Graph- DirectCost
                                              Fartilizer = dr["fartilizer"] != DBNull.Value ? Convert.ToDouble(dr["fartilizer"]) : 0,
                                              Crop_Chemicals = dr["crop_Chemicals"] != DBNull.Value ? Convert.ToDouble(dr["crop_Chemicals"]) : 0,
                                              Other_Crop = dr["other_Crop"] != DBNull.Value ? Convert.ToDouble(dr["other_Crop"]) : 0,
                                              Energy = dr["energy"] != DBNull.Value ? Convert.ToDouble(dr["energy"]) : 0,
                                              Repair_Maintenance = dr["repair_maintenance"] != DBNull.Value ? Convert.ToDouble(dr["repair_maintenance"]) : 0,
                                              Custom_Hire = dr["custom_hire"] != DBNull.Value ? Convert.ToDouble(dr["custom_hire"]) : 0,
                                              Hired_Labor = dr["hired_labor"] != DBNull.Value ? Convert.ToDouble(dr["hired_labor"]) : 0,
                                              Irrigation = dr["irrigation"] != DBNull.Value ? Convert.ToDouble(dr["irrigation"]) : 0,
                                              Other = dr["other"] != DBNull.Value ? Convert.ToDouble(dr["other"]) : 0,
                                              Leases = dr["leases"] != DBNull.Value ? Convert.ToDouble(dr["leases"]) : 0,

                                              //Graph-IndirectCost
                                              Labor = dr["labor"] != DBNull.Value ? Convert.ToDouble(dr["labor"]) : 0,
                                              Farm_insurance = dr["farm_insurance"] != DBNull.Value ? Convert.ToDouble(dr["farm_insurance"]) : 0,
                                              Dues_Fees = dr["Dues_Fees"] != DBNull.Value ? Convert.ToDouble(dr["Dues_Fees"]) : 0,
                                              Utilities = dr["otherExpensesutilities"] != DBNull.Value ? Convert.ToDouble(dr["otherExpensesutilities"]) : 0,
                                              Interest = dr["interest"] != DBNull.Value ? Convert.ToDouble(dr["interest"]) : 0,
                                              Machine_bldg_depreciation = dr["machine_bldg_depreciation"] != DBNull.Value ? Convert.ToDouble(dr["machine_bldg_depreciation"]) : 0,
                                              Real_estate_taxes = dr["real_estate_taxes"] != DBNull.Value ? Convert.ToDouble(dr["real_estate_taxes"]) : 0,
                                              Miscellaneous = dr["miscellaneous"] != DBNull.Value ? Convert.ToDouble(dr["miscellaneous"]) : 0,
                                              OtherExpensesHiredLabor = dr["otherExpensesHiredLabor"] != DBNull.Value ? Convert.ToDouble(dr["otherExpensesHiredLabor"]) : 0,
                                              OtherExpensesMachineryLease = dr["otherExpensesMachineryLease"] != DBNull.Value ? Convert.ToDouble(dr["otherExpensesMachineryLease"]) : 0,
                                              OtherExpensesOtherOverheadExpenses = dr["otherExpensesOtherOverheadExpenses"] != DBNull.Value ? Convert.ToDouble(dr["otherExpensesOtherOverheadExpenses"]) : 0,

                                              //NetReturn and Repayment box
                                              BrkEvenBushelRepayment = dr["brkevenbushelrepayment"] != DBNull.Value ? Convert.ToDouble(dr["brkevenbushelrepayment"]) : 0,
                                              BrkEvenYieldRepayment = dr["brkevenyieldacrerepayment"] != DBNull.Value ? Convert.ToDouble(dr["brkevenyieldacrerepayment"]) : 0,
                                              NetReturn = dr["NetReturn"] != DBNull.Value ? Convert.ToDouble(dr["NetReturn"]) : 0,

                                              BrkEvenBushelNetreturn = dr["brkevenbushelnetreturn"] != DBNull.Value ? Convert.ToDouble(dr["brkevenbushelnetreturn"]) : 0,
                                              BrkEvenYieldNetreturn = dr["brkevenyieldacrenetreturn"] != DBNull.Value ? Convert.ToDouble(dr["brkevenyieldacrenetreturn"]) : 0,
                                              RegionalBenchmark = dr["regionalBenchmark"] != DBNull.Value ? Convert.ToDouble(dr["regionalBenchmark"]) : 0,
                                          }).ToList();
                    }
                    totalzydid = chartzydiddata.Sum(x => x.TotalDirectCost + x.TotalInDirectCost + x.TotalProductionCost + x.BrkEvenBushelRepayment);
                }

                //NOTE: Check chartdata all values sum 0 or not

                ChartDataWithZydid chartDataWithZydid = new ChartDataWithZydid();
                chartDataWithZydid.chartdata = chartdata;
                chartDataWithZydid.chartzydiddata = chartzydiddata;

                if (chartdata.Count > 0 && total != 0 ||
                    chartzydiddata.Count > 0 && totalzydid != 0)
                {
                    return Json(new { success = true, ChartDataValues = chartDataWithZydid });
                }
                else
                {
                    return Json(new { success = false, message = "No Data" });
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Json(new { success = false, message = "No Data" });
            }
        }

        public IActionResult FinancialDetails(string id, string name)
        {
            ViewBag.FieldId = encryptDecryptData.Decryptdata(id);
            ViewBag.FieldName = name;
            return View();
        }

        public IActionResult FinancialDetailsEdit(string id, string name,bool aplyoptioncalc)
        {
            ViewBag.FieldId = encryptDecryptData.Decryptdata(id);
            ViewBag.FieldName = name;
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] Params = { "_actionType", "_id" };
                object[] Values = { "getManagementByFieldId", ViewBag.FieldId };
                DataSet dsZoneData = new DataSet();
                if (aplyoptioncalc == true)
                {
                    dsZoneData = _masterRepository.GetAndSelectTableItems($"SELECT * from \"FSRCalc\".water_getmanagementbyfieldid_ref (datarefcursor => 'data',{Params[0]}=>'{Values[0]}', {Params[1]} =>{Values[1]})");
                }
                else
                {
                    dsZoneData = _masterRepository.GetAndSelectTableItems($"SELECT * from \"FSRCalc\".water_getmanagementbyfieldid_ref (datarefcursor => 'data',{Params[0]}=>'getManagementByFieldId_withoutcalc', {Params[1]} =>{Values[1]})");
                }

                string FieldOptionIdNamequery = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',datarefcursor2 =>'data2',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {Values[1]});";
                DataSet result = _masterRepository.GetAndSelectMultipleTableItems(FieldOptionIdNamequery);
                Dictionary<int, string> FieldoptionNameDictionary = new Dictionary<int, string>();
                //FieldoptionNameDictionary = null;
                if (result != null && result.Tables.Count > 0 && result.Tables[1].Rows.Count > 0)
                {

                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        int optionId = Convert.ToInt32(row["field_option_id"]);
                        string optionName = row["name"].ToString();

                        FieldoptionNameDictionary.Add(optionId, optionName);
                    }
                }
                ViewBag.Fieldoption_id_Name = FieldoptionNameDictionary;

                var zones = new List<Zone>();

                var groupedZones = dsZoneData.Tables[0].AsEnumerable()
                .GroupBy(row => new
                {
                    ZoneId = (int)row["management_zones_id"],
                    ZoneName = Convert.ToString(row["zone_name"]),
                    FieldOptionId = (int)row["field_option_id"]
                })
                .Select(group => new Zone
                {
                    ZoneId = group.Key.ZoneId,
                    ZoneName = group.Key.ZoneName,
                    FieldOptionId = group.Key.FieldOptionId,
                    ZoneYearDetails = group.Select(row => new ZoneYearDetails
                    {
                        //IsValuesChanged = row.Field<bool?>("isvalueschanged") ?? false,
                        Year = row.Field<int>("year"),
                        Field_Id = row.Field<int>("field_id"),
                        FieldOptionId = row.Field<int>("field_option_id"),
                        Management_Zone_Id = row.Field<int>("management_zones_id"),
                        ZydId = row.Field<int>("zydid"),
                        Acres = row.Field<double?>("acre") ?? 0.0,
                        CropName = row.Field<string>("crop_name") ?? string.Empty,
                        TillageName = row.Field<string>("tillage_name") ?? string.Empty,
                        Yield = row.Field<string>("yield") ?? string.Empty,
                        ExpectedYield = row.Field<double?>("expectedyield") ?? 0.0,
                        OperatorShare = row.Field<double?>("operatorpercent") ?? 0.0,
                        CommoditySalePrice = row.Field<double?>("cashprice") ?? 0.0,
                        OtherProductReturn = row.Field<double?>("otherreturn") ?? 0.0,
                        TotalProductReturn = row.Field<double?>("croprevenue") ?? 0.0,
                        HedgeGainLoss = row.Field<double?>("hedge_gain_loss") ?? 0.0,
                        CropInsurancePayments = row.Field<double?>("cropinspayment") ?? 0.0,
                        StewardshipPayments = row.Field<double?>("stewardship_payments") ?? 0.0,
                        GovernmentPayments = row.Field<double?>("govtpayments") ?? 0.0,
                        TotalGrossRevenue = row.Field<double?>("TotalGrossRevenue") ?? 0.0,
                        Seed = row.Field<double?>("seed_direct") ?? 0.0,
                        Nitrogen = row.Field<double?>("nfert_direct") ?? 0.0,
                        Phosphorus = row.Field<double?>("pfert_direct") ?? 0.0,
                        Potash = row.Field<double?>("potfert_direct") ?? 0.0,
                        Sulfur = row.Field<double?>("sfert_direct") ?? 0.0,
                        Lime = row.Field<double?>("lime_direct") ?? 0.0,
                        SeedOther = row.Field<double?>("otherfert_direct") ?? 0.0,
                        Herbicide = row.Field<double?>("Herbicide_direct") ?? 0.0,
                        Fungicide = row.Field<double?>("Funicide_direct") ?? 0.0,
                        Insecticide = row.Field<double?>("Insecticide_direct") ?? 0.0,
                        CropChemicalOther = row.Field<double?>("otherchem_direct") ?? 0.0,
                        TotalCropChemicalExpenses = row.Field<double?>("TotFert_direct") ?? 0.0,
                        Miscellaneous = row.Field<double?>("cropmisc_direct") ?? 0.0,
                        CoverCrop = row.Field<double?>("covercrop_direct") ?? 0.0,
                        Insurance = row.Field<double?>("cropinsur_direct") ?? 0.0,
                        DryingPropane = row.Field<double?>("DryPropane_direct") ?? 0.0,
                        EquipmentFuel = row.Field<double?>("EquipFuel_direct") ?? 0.0,
                        Machinery = row.Field<double?>("MachRepair_direct") ?? 0.0,
                        Buildings = row.Field<double?>("BldgRepair_direct") ?? 0.0,
                        RepairMaintenanceOther = row.Field<double?>("otherrepair_direct") ?? 0.0,
                        TotalRepairMaintenance = row.Field<double?>("totothercrop_direct") ?? 0.0,
                        Driver = row.Field<double?>("CustomDriver_direct") ?? 0.0,
                        Equipment = row.Field<double?>("CustomEquipHire_direct") ?? 0.0,
                        CustomApplicationt = row.Field<double?>("CustomApp_direct") ?? 0.0,
                        CustomHireOther = row.Field<double?>("OtherCustomHire_direct") ?? 0.0,
                        TotalCustomHire = row.Field<double?>("TotCustomHire_oexp") ?? 0.0,
                        HiredLabor = row.Field<double?>("HiredLabor_direct") ?? 0.0,
                        Repairs = row.Field<double?>("IrrigationRepair_direct") ?? 0.0,
                        FuelElectricity = row.Field<double?>("IrrigPower_direct") ?? 0.0,
                        LeasesMachinery = row.Field<double?>("Machinery_direct") ?? 0.0,
                        LeasesBuildings = row.Field<double?>("Building_direct") ?? 0.0,
                        LandRent = row.Field<double?>("LandRent_direct") ?? 0.0,
                        StewardshipImplementation = row.Field<double?>("Stewardship_direct") ?? 0.0,
                        Storage = row.Field<double?>("Storage_direct") ?? 0.0,
                        Supplies = row.Field<double?>("Supplies_direct") ?? 0.0,
                        Utilities = row.Field<double?>("Utilities_direct") ?? 0.0,
                        FreightTrucking = row.Field<double?>("FreightTrucking_direct") ?? 0.0,
                        Marketing = row.Field<double?>("Marketing") ?? 0.0,
                        InterestOperating = row.Field<double?>("Interest_direct") ?? 0.0,
                        OtherCosts = row.Field<double?>("Other_direct") ?? 0.0,
                        TotalDirectExpense = row.Field<double?>("totdirectexp") ?? 0.0,
                        ReturnOverDirectExpenseperAcre = row.Field<double?>("returnovrdirect") ?? 0.0,
                        OverheadExpensesDriver = row.Field<double?>("CustomDriver_oexp") ?? 0.0,
                        OverheadExpensesEquipment = row.Field<double?>("CustomEquipHire_oexp") ?? 0.0,
                        OverheadExpensesCustomApplication = row.Field<double?>("CustomApp_oexp") ?? 0.0,
                        OverheadExpensesOther = row.Field<double?>("OtherCustomHire_oexp") ?? 0.0,
                        OverheadExpensesTotalCustomHire = row.Field<double?>("totcustomhire_direct") ?? 0.0,
                        OtherExpensesHiredLabor = row.Field<double?>("HiredLabor_oexp") ?? 0.0,
                        OtherExpensesMachineryLease = row.Field<double?>("Mach_Lease_oepx") ?? 0.0,
                        OtherExpensesBuildingLeases = row.Field<double?>("Build_Leas_oepx") ?? 0.0,
                        OtherExpensesFarmInsurance = row.Field<double?>("FarmInsurance_oexp") ?? 0.0,
                        OtherExpensesUtilities = row.Field<double?>("Utility_oexp") ?? 0.0,
                        OtherExpensesDuesProfessionalFees = row.Field<double?>("Dues_oexp") ?? 0.0,
                        OtherExpensesInterest = row.Field<double?>("Interest_oexp") ?? 0.0,
                        OtherExpensesMachineBuildingDepreciation = row.Field<double?>("Depreciation_oexp") ?? 0.0,
                        OtherExpensesRealEstateTaxes = row.Field<double?>("RealEstTax_oexp") ?? 0.0,
                        OtherExpensesOtherOverheadExpenses = row.Field<double?>("Misc_oexp") ?? 0.0,
                        TotalOverheadExpenses = row.Field<double?>("totoverheadexp") ?? 0.0,
                        FinancingIncomeTaxes = row.Field<double?>("IncTax_finance") ?? 0.0,
                        FinancingOwnerWithdrawal = row.Field<double?>("OwnerWithdrawl_finance") ?? 0.0,
                        FinancingPrincipalPayment = row.Field<double?>("princpayment_finance") ?? 0.0,
                        TotalFertilizerExpenses = row.Field<double?>("totalfertilizerexpenses") ?? 0.0,
                        TotalFinancing = row.Field<double?>("totalfinancial") ?? 0.0,
                        DataQualityGrade = row.Field<string?>("quality_grade") ?? "A"

                    }).ToList()
                });
                zones.AddRange(groupedZones);

                string query = $"select * from \"FSRCalc\".financial_plans_ref(datarefcursor => 'data', _actiontype => 'GetFinancialPlans',_field_id=>{ViewBag.FieldId});";
                DataSet financialPlansData = _masterRepository.GetAndSelectTableItems(query);
                List<FinancialPlans> FieldsData = new List<FinancialPlans>();
                bool hasRows = financialPlansData.Tables.Cast<DataTable>()
                               .Any(table => table.Rows.Count != 0);

                if (hasRows)
                {
                    FieldsData = (from DataRow dr in financialPlansData.Tables[0].Rows
                                  select new FinancialPlans()
                                  {
                                      FinancialPlansId = int.Parse(dr["financial_plans_id"].ToString()),
                                      PlansName = dr["plans_name"].ToString(),
                                  }).ToList();

                    ViewBag.financialPlansData = FieldsData;
                }
                else
                {
                    ViewBag.financialPlansData = null;
                }

                ViewBag.UserRole = LoginUserRoleId;
                return View(zones);
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return View();
            };
        }

        public IActionResult SaveFinancialData(ZoneYearDetails zoneyeardetails)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                bool IsValuesChanged = false;
                IsValuesChanged = zoneyeardetails.IsValuesChanged;
                object[] userParams = {
                    "_management_zone_id", "_year","_field_option_id", "_expectedyield", "_operatorpercent", "_cashprice",
                    "_otherreturn","_croprevenue","_hedgegainloss", "_cropinspayment", "_stewardshippayments",
                    "_govtpayments","_totalgrossrevenue" , "_seed_direct", "_nfert_direct", "_pfert_direct",
                    "_potfert_direct", "_sfert_direct", "_lime_direct", "_otherfert_direct",  "_herbicide_direct",
                    "_funicide_direct","_insecticide_direct","_otherchem_direct" ,"_totfert_direct","_cropmisc_direct",
                    "_covercrop_direct", "_cropinsur_direct", "_DryPropane_direct", "_EquipFuel_direct", "_MachRepair_direct",
                    "_BldgRepair_direct", "_otherrepair_direct","_totothercrop_direct", "_CustomDriver_direct", "_CustomEquipHire_direct",
                    "_CustomApp_direct", "_othercustomhire_direct","_totcustomhire_oexp", "_HiredLabor_direct", "_IrrigationRepair_direct",
                    "_IrrigPower_direct", "_Machinery_direct", "_Building_direct", "_LandRent_direct", "_Stewardship_direct",
                    "_Storage_direct","_Supplies_direct", "_Utilities_direct", "_FreightTrucking_direct", "_Marketing", "_Other_direct",
                    "_Interest_direct", "_CustomDriver_oexp", "_CustomEquipHire_oexp", "_CustomApp_oexp","_othercustomhire_oexp",
                    "_totcustomhire_direct", "_HiredLabor_oexp", "_Mach_Lease_oepx", "_Build_Leas_oepx", "_FarmInsurance_oexp",
                    "_Utility_oexp", "_Dues_oexp", "_Interest_oexp", "_Depreciation_oexp","_RealEstTax_oexp",
                    "_Misc_oexp","_totoverheadexp", "_IncTax_finance", "_OwnerWithdrawl_finance", "_princpayment_finance","_totalFertilizerExpenses",
                    "_totdirectexp" , "_returnovrdirect","_totalfinancial","_isvalueschanged"
                };

                object[] userValues = {
                    zoneyeardetails.Management_Zone_Id, zoneyeardetails.Year,zoneyeardetails.FieldOptionId, Convert.ToDouble(zoneyeardetails.ExpectedYield),  Convert.ToDouble(zoneyeardetails.OperatorShare), Convert.ToDouble(zoneyeardetails.CommoditySalePrice),
                    Convert.ToDouble(zoneyeardetails.OtherProductReturn) , Convert.ToDouble(zoneyeardetails.TotalProductReturn) , Convert.ToDouble(zoneyeardetails.HedgeGainLoss), Convert.ToDouble(zoneyeardetails.CropInsurancePayments) , Convert.ToDouble(zoneyeardetails.StewardshipPayments),
                    Convert.ToDouble(zoneyeardetails.GovernmentPayments) ,  Convert.ToDouble(zoneyeardetails.TotalGrossRevenue) , Convert.ToDouble(zoneyeardetails.Seed) ,  Convert.ToDouble(zoneyeardetails.Nitrogen) , Convert.ToDouble(zoneyeardetails.Phosphorus),
                    Convert.ToDouble(zoneyeardetails.Potash), Convert.ToDouble(zoneyeardetails.Sulfur), Convert.ToDouble(zoneyeardetails.Lime) , Convert.ToDouble(zoneyeardetails.SeedOther), Convert.ToDouble(zoneyeardetails.Herbicide),
                    Convert.ToDouble(zoneyeardetails.Fungicide) , Convert.ToDouble(zoneyeardetails.Insecticide) ,Convert.ToDouble(zoneyeardetails.CropChemicalOther) , Convert.ToDouble(zoneyeardetails.TotalCropChemicalExpenses), Convert.ToDouble(zoneyeardetails.Miscellaneous) ,
                    Convert.ToDouble(zoneyeardetails.CoverCrop), Convert.ToDouble(zoneyeardetails.Insurance),Convert.ToDouble(zoneyeardetails.DryingPropane) , Convert.ToDouble(zoneyeardetails.EquipmentFuel) , Convert.ToDouble(zoneyeardetails.Machinery),
                    Convert.ToDouble(zoneyeardetails.Buildings) ,  Convert.ToDouble(zoneyeardetails.RepairMaintenanceOther),  Convert.ToDouble(zoneyeardetails.TotalRepairMaintenance),  Convert.ToDouble(zoneyeardetails.Driver) ,  Convert.ToDouble(zoneyeardetails.Equipment),
                    Convert.ToDouble(zoneyeardetails.CustomApplicationt) , Convert.ToDouble(zoneyeardetails.CustomHireOther) ,Convert.ToDouble(zoneyeardetails.TotalCustomHire) ,Convert.ToDouble(zoneyeardetails.HiredLabor) , Convert.ToDouble(zoneyeardetails.Repairs),
                    Convert.ToDouble(zoneyeardetails.FuelElectricity),   Convert.ToDouble(zoneyeardetails.LeasesMachinery) ,  Convert.ToDouble(zoneyeardetails.LeasesBuildings) ,  Convert.ToDouble(zoneyeardetails.LandRent) ,  Convert.ToDouble(zoneyeardetails.StewardshipImplementation),
                    Convert.ToDouble(zoneyeardetails.Storage) , Convert.ToDouble(zoneyeardetails.Supplies) , Convert.ToDouble(zoneyeardetails.Utilities) , Convert.ToDouble(zoneyeardetails.FreightTrucking) , Convert.ToDouble(zoneyeardetails.Marketing) , Convert.ToDouble(zoneyeardetails.OtherCosts),
                    Convert.ToDouble(zoneyeardetails.InterestOperating),  Convert.ToDouble(zoneyeardetails.OverheadExpensesDriver) , Convert.ToDouble(zoneyeardetails.OverheadExpensesEquipment) , Convert.ToDouble(zoneyeardetails.OverheadExpensesCustomApplication) , Convert.ToDouble(zoneyeardetails.OverheadExpensesOther),
                    Convert.ToDouble(zoneyeardetails.OverheadExpensesTotalCustomHire),  Convert.ToDouble(zoneyeardetails.OtherExpensesHiredLabor), Convert.ToDouble(zoneyeardetails.OtherExpensesMachineryLease) , Convert.ToDouble(zoneyeardetails.OtherExpensesBuildingLeases) , Convert.ToDouble(zoneyeardetails.OtherExpensesFarmInsurance)  ,
                    Convert.ToDouble(zoneyeardetails.OtherExpensesUtilities), Convert.ToDouble(zoneyeardetails.OtherExpensesDuesProfessionalFees) , Convert.ToDouble(zoneyeardetails.OtherExpensesInterest) , Convert.ToDouble(zoneyeardetails.OtherExpensesMachineBuildingDepreciation) , Convert.ToDouble(zoneyeardetails.OtherExpensesRealEstateTaxes) ,
                    Convert.ToDouble(zoneyeardetails.OtherExpensesOtherOverheadExpenses) , Convert.ToDouble(zoneyeardetails.TotalOverheadExpenses),Convert.ToDouble(zoneyeardetails.FinancingIncomeTaxes),Convert.ToDouble(zoneyeardetails.FinancingOwnerWithdrawal) , Convert.ToDouble(zoneyeardetails.FinancingPrincipalPayment),Convert.ToDouble(zoneyeardetails.TotalFertilizerExpenses),
                    Convert.ToDouble(zoneyeardetails.TotalDirectExpense), Convert.ToDouble(zoneyeardetails.ReturnOverDirectExpenseperAcre), Convert.ToDouble(zoneyeardetails.TotalFinancing),IsValuesChanged
                };

                //string updateArea_Query = $"select * from \"FSRCalc\".water_field_total_area_acre_update(_field_id=>{zoneyeardetails.Field_Id}, _total_area=>{zoneyeardetails.Acres});";
                //DataSet resutdata = _masterRepository.GetAndSelectTableItemsWithoutCursor(updateArea_Query);

                object[] sqlDbTypes = { "abc" };

                string query = $"SELECT * from \"FSRCalc\".water_update_management_zone_financial_new_testing17_ref(datarefcursor => 'data', _quality_grade =>'{zoneyeardetails.DataQualityGrade}',";
                for (int i = 0; i < userParams.Length; i++)
                {
                    query += $"{userParams[i]} => {userValues[i]},";
                }
                query = query.TrimEnd(',') + ");";

                DataSet EditZoneData = _masterRepository.GetAndSelectTableItems(query);

                int UserRole = LoginUserRoleId;
                var fielId = encryptDecryptData.Encryptdata(Convert.ToString(zoneyeardetails.Field_Id));
                if ((int)EditZoneData.Tables.Count > 0 && (int)EditZoneData.Tables[0].Rows.Count > 0 && UserRole == (int)UserTypeEnum.Admin)
                {
                    TempData["Success"] = "The zone financial details update successfully.";// "The Producer saved successfully.";
                    return RedirectToAction("FinancialDetailsEdit", "Common", new { id = fielId, name = zoneyeardetails.Field_Name });

                }
                else if ((int)EditZoneData.Tables.Count > 0 && (int)EditZoneData.Tables[0].Rows.Count > 0 && UserRole == (int)UserTypeEnum.Agronomist)
                {
                    TempData["Success"] = CommonMethods.GetEnumDescription(ValidationMessages.ZoneSavedSuccessfully); // "The Producer saved successfully.";
                    return RedirectToAction("FinancialDetailsEdit", "Common", new { id = fielId, name = zoneyeardetails.Field_Name });
                }
                else if ((int)EditZoneData.Tables.Count > 0 && (int)EditZoneData.Tables[0].Rows.Count > 0 && UserRole == (int)UserTypeEnum.Producer)
                {
                    TempData["Success"] = "The zone financial details update successfully.";// "The Producer saved successfully.";
                    return RedirectToAction("FinancialDetailsEdit", "Common", new { id = fielId, name = zoneyeardetails.Field_Name });
                }
                else
                {
                    return RedirectToAction("Error", "Common");
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            };
        }

        [HttpGet]
        public IActionResult PrecipitationTableYear(int field_Id)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            try
            {
                string query = $"SELECT * FROM \"FSRCalc\".water_uploadcalculate_fsr_data(_ActionType=> 'get_precipitation_year',datarefcursor =>'data',_field_id =>{field_Id});";
                DataSet DsPricipYearData = _masterRepository.GetAndSelectTableItems(query);

                return Json(new { success = true, data = JsonConvert.SerializeObject(DsPricipYearData) });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Json(new { success = false, message = "No Data" });
            }
        }

        public IActionResult FSRCalculation()
        {
            IMasterRepository _masterRepository = new MasterRepository();
            object[] userParams = { "_ActionType" };
            object[] userValues = { "AllData" };

            try
            {
                string query = $"SELECT * FROM \"FSRCalc\".water_get_avg_indices_testing1({userParams[0]} => '{userValues[0]}');";
                DataSet DsfsrData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.FsrData = DsfsrData;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                ViewBag.ErrorMessage = ex.Message;
            }
            return View();
        }

        [HttpGet]
        public IActionResult Get_FieldInformationGeometry(string Field_Id)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                //field_geometry
                string getFieldGeometryQuery = $"select* from  \"FSRCalc\".get_field_mz_geomerty(_field_id => {Field_Id},actiontype => 'GetField_geometry', datarefcursor => 'data');";
                DataSet DsFieldGeometry = _masterRepository.GetAndSelectTableItems(getFieldGeometryQuery);
                string? FieldGeometry = DsFieldGeometry != null && DsFieldGeometry.Tables.Count > 0 && DsFieldGeometry.Tables[0].Rows.Count > 0
                ? DsFieldGeometry.Tables[0].Rows[0]["geojson"].ToString() : string.Empty;

                string? pointx = DsFieldGeometry != null && DsFieldGeometry.Tables.Count > 0 && DsFieldGeometry.Tables[0].Rows.Count > 0
               ? DsFieldGeometry.Tables[0].Rows[0]["pointx"].ToString() : string.Empty;

                string? pointy = DsFieldGeometry != null && DsFieldGeometry.Tables.Count > 0 && DsFieldGeometry.Tables[0].Rows.Count > 0
               ? DsFieldGeometry.Tables[0].Rows[0]["pointy"].ToString() : string.Empty;

                // get field zone
                string getFielZoneQuery = $"select* from  \"FSRCalc\".get_field_mz_geomerty(_field_id => {Field_Id},actiontype=>'GetField_zone', datarefcursor => 'data');";
                DataSet DsgetFielZone = _masterRepository.GetAndSelectTableItems(getFielZoneQuery);

                List<string> stringsGeometryMz = new List<string>();
                if (DsgetFielZone != null && DsgetFielZone.Tables.Count > 0 && DsgetFielZone.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow zone_id in DsgetFielZone.Tables[0].Rows)
                    {
                        string getGeometry = $"select * from  \"FSRCalc\".get_field_mz_geomerty(_field_id=>{Field_Id},_mz_id=>{zone_id["management_zones_id"]},actiontype=>'Getmz_geometry', datarefcursor=>'data');";
                        DataSet DsgetGeometryMz = _masterRepository.GetAndSelectTableItems(getGeometry);
                        string? GeometryMz = DsgetGeometryMz != null && DsgetGeometryMz.Tables.Count > 0 && DsgetGeometryMz.Tables[0].Rows.Count > 0
                        ? DsgetGeometryMz.Tables[0].Rows[0]["geojson"].ToString() : string.Empty;
                        if (getGeometry != string.Empty)
                        {
                            stringsGeometryMz.Add(GeometryMz);
                        }
                    }
                }
                return Json(new { success = true, MzGeometry = stringsGeometryMz, fieldGeometry = FieldGeometry, pointx = pointx, pointy = pointy });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Json(new { success = false, message = "No Data" });
            }
        }

        public IActionResult CopysameProducerfields(string id, string name, string copiedData, string cropname)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                var zoneyeardetails = JsonConvert.DeserializeObject<FarmingFinancialData>(copiedData);

                object[] userParams = {
                     "_expectedyield", "_operatorpercent", "_cashprice",
                    "_otherreturn","_croprevenue","_hedgegainloss", "_cropinspayment", "_stewardshippayments",
                    "_govtpayments","_totalgrossrevenue" , "_seed_direct", "_nfert_direct", "_pfert_direct",
                    "_potfert_direct", "_sfert_direct", "_lime_direct", "_otherfert_direct",  "_herbicide_direct",
                    "_funicide_direct","_insecticide_direct","_otherchem_direct" ,"_totfert_direct","_cropmisc_direct",
                    "_covercrop_direct", "_cropinsur_direct", "_DryPropane_direct", "_EquipFuel_direct", "_MachRepair_direct",
                    "_BldgRepair_direct", "_otherrepair_direct","_totothercrop_direct", "_CustomDriver_direct", "_CustomEquipHire_direct",
                    "_CustomApp_direct", "_othercustomhire_direct","_totcustomhire_oexp", "_HiredLabor_direct", "_IrrigationRepair_direct",
                    "_IrrigPower_direct", "_Machinery_direct", "_Building_direct", "_LandRent_direct", "_Stewardship_direct",
                    "_Storage_direct","_Supplies_direct", "_Utilities_direct", "_FreightTrucking_direct", "_Marketing", "_Other_direct",
                    "_Interest_direct", "_CustomDriver_oexp", "_CustomEquipHire_oexp", "_CustomApp_oexp","_othercustomhire_oexp",
                    "_totcustomhire_direct", "_HiredLabor_oexp", "_Mach_Lease_oepx", "_Build_Leas_oepx", "_FarmInsurance_oexp",
                    "_Utility_oexp", "_Dues_oexp", "_Interest_oexp", "_Depreciation_oexp","_RealEstTax_oexp",
                    "_Misc_oexp","_totoverheadexp", "_IncTax_finance", "_OwnerWithdrawl_finance", "_princpayment_finance","_totalFertilizerExpenses",
                    "_totdirectexp" , "_returnovrdirect","_totalfinancial"
                };

                object[] userValues = {
                    Convert.ToDouble(zoneyeardetails.ExpectedYield),  Convert.ToDouble(zoneyeardetails.OperatorShare), Convert.ToDouble(zoneyeardetails.CommoditySalePrice),
                    Convert.ToDouble(zoneyeardetails.OtherProductReturn) , Convert.ToDouble(zoneyeardetails.TotalProductReturn) , Convert.ToDouble(zoneyeardetails.HedgeGainLoss), Convert.ToDouble(zoneyeardetails.CropInsurancePayments) , Convert.ToDouble(zoneyeardetails.StewardshipPayments),
                    Convert.ToDouble(zoneyeardetails.GovernmentPayments) ,  Convert.ToDouble(zoneyeardetails.TotalGrossRevenue) , Convert.ToDouble(zoneyeardetails.Seed) ,  Convert.ToDouble(zoneyeardetails.Nitrogen) , Convert.ToDouble(zoneyeardetails.Phosphorus),
                    Convert.ToDouble(zoneyeardetails.Potash), Convert.ToDouble(zoneyeardetails.Sulfur), Convert.ToDouble(zoneyeardetails.Lime) , Convert.ToDouble(zoneyeardetails.SeedOther), Convert.ToDouble(zoneyeardetails.Herbicide),
                    Convert.ToDouble(zoneyeardetails.Fungicide) , Convert.ToDouble(zoneyeardetails.Insecticide) ,Convert.ToDouble(zoneyeardetails.CropChemicalOther) , Convert.ToDouble(zoneyeardetails.TotalCropChemicalExpenses), Convert.ToDouble(zoneyeardetails.Miscellaneous) ,
                    Convert.ToDouble(zoneyeardetails.CoverCrop), Convert.ToDouble(zoneyeardetails.Insurance),Convert.ToDouble(zoneyeardetails.DryingPropane) , Convert.ToDouble(zoneyeardetails.EquipmentFuel) , Convert.ToDouble(zoneyeardetails.Machinery),
                    Convert.ToDouble(zoneyeardetails.Buildings) ,  Convert.ToDouble(zoneyeardetails.RepairMaintenanceOther),  Convert.ToDouble(zoneyeardetails.TotalRepairMaintenance),  Convert.ToDouble(zoneyeardetails.Driver) ,  Convert.ToDouble(zoneyeardetails.Equipment),
                    Convert.ToDouble(zoneyeardetails.CustomApplicationt) , Convert.ToDouble(zoneyeardetails.CustomHireOther) ,Convert.ToDouble(zoneyeardetails.TotalCustomHire) ,Convert.ToDouble(zoneyeardetails.HiredLabor) , Convert.ToDouble(zoneyeardetails.Repairs),
                    Convert.ToDouble(zoneyeardetails.FuelElectricity),   Convert.ToDouble(zoneyeardetails.LeasesMachinery) ,  Convert.ToDouble(zoneyeardetails.LeasesBuildings) ,  Convert.ToDouble(zoneyeardetails.LandRent) ,  Convert.ToDouble(zoneyeardetails.StewardshipImplementation),
                    Convert.ToDouble(zoneyeardetails.Storage) , Convert.ToDouble(zoneyeardetails.Supplies) , Convert.ToDouble(zoneyeardetails.Utilities) , Convert.ToDouble(zoneyeardetails.FreightTrucking) , Convert.ToDouble(zoneyeardetails.Marketing) , Convert.ToDouble(zoneyeardetails.OtherCosts),
                    Convert.ToDouble(zoneyeardetails.InterestOperating),  Convert.ToDouble(zoneyeardetails.OverheadExpensesDriver) , Convert.ToDouble(zoneyeardetails.OverheadExpensesEquipment) , Convert.ToDouble(zoneyeardetails.OverheadExpensesCustomApplication) , Convert.ToDouble(zoneyeardetails.OverheadExpensesOther),
                    Convert.ToDouble(zoneyeardetails.OverheadExpensesTotalCustomHire),  Convert.ToDouble(zoneyeardetails.OtherExpensesHiredLabor), Convert.ToDouble(zoneyeardetails.OtherExpensesMachineryLease) , Convert.ToDouble(zoneyeardetails.OtherExpensesBuildingLeases) , Convert.ToDouble(zoneyeardetails.OtherExpensesFarmInsurance)  ,
                    Convert.ToDouble(zoneyeardetails.OtherExpensesUtilities), Convert.ToDouble(zoneyeardetails.OtherExpensesDuesProfessionalFees) , Convert.ToDouble(zoneyeardetails.OtherExpensesInterest) , Convert.ToDouble(zoneyeardetails.OtherExpensesMachineBuildingDepreciation) , Convert.ToDouble(zoneyeardetails.OtherExpensesRealEstateTaxes) ,
                    Convert.ToDouble(zoneyeardetails.OtherExpensesOtherOverheadExpenses) , Convert.ToDouble(zoneyeardetails.TotalOverheadExpenses),Convert.ToDouble(zoneyeardetails.FinancingIncomeTaxes),Convert.ToDouble(zoneyeardetails.FinancingOwnerWithdrawal) , Convert.ToDouble(zoneyeardetails.FinancingPrincipalPayment),Convert.ToDouble(zoneyeardetails.TotalFertilizerExpenses),
                    Convert.ToDouble(zoneyeardetails.TotalDirectExpense), Convert.ToDouble(zoneyeardetails.ReturnOverDirectExpenseperAcre), Convert.ToDouble(zoneyeardetails.TotalFinancing)
                };

                string updateArea_Query = $"select * from \"FSRCalc\".water_updatesameproducer_management_zone_financial(_field_id => {Convert.ToInt32(id)}, _crop_name =>'{cropname}',";

                for (int i = 0; i < userParams.Length; i++)
                {
                    updateArea_Query += $"{userParams[i]} => {userValues[i]},";
                }
                updateArea_Query = updateArea_Query.TrimEnd(',') + ");";

                DataSet EditZoneData = _masterRepository.GetAndSelectTableItemsWithoutCursor(updateArea_Query);

                return Json(new { Success = true, Message = "The zone financial details update successfully." });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult Copysamefields(string id, string name, string copiedData, string cropname)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                var zoneyeardetails = JsonConvert.DeserializeObject<FarmingFinancialData>(copiedData);

                object[] userParams = {
                     "_expectedyield", "_operatorpercent", "_cashprice",
                    "_otherreturn","_croprevenue","_hedgegainloss", "_cropinspayment", "_stewardshippayments",
                    "_govtpayments","_totalgrossrevenue" , "_seed_direct", "_nfert_direct", "_pfert_direct",
                    "_potfert_direct", "_sfert_direct", "_lime_direct", "_otherfert_direct",  "_herbicide_direct",
                    "_funicide_direct","_insecticide_direct","_otherchem_direct" ,"_totfert_direct","_cropmisc_direct",
                    "_covercrop_direct", "_cropinsur_direct", "_DryPropane_direct", "_EquipFuel_direct", "_MachRepair_direct",
                    "_BldgRepair_direct", "_otherrepair_direct","_totothercrop_direct", "_CustomDriver_direct", "_CustomEquipHire_direct",
                    "_CustomApp_direct", "_othercustomhire_direct","_totcustomhire_oexp", "_HiredLabor_direct", "_IrrigationRepair_direct",
                    "_IrrigPower_direct", "_Machinery_direct", "_Building_direct", "_LandRent_direct", "_Stewardship_direct",
                    "_Storage_direct","_Supplies_direct", "_Utilities_direct", "_FreightTrucking_direct", "_Marketing", "_Other_direct",
                    "_Interest_direct", "_CustomDriver_oexp", "_CustomEquipHire_oexp", "_CustomApp_oexp","_othercustomhire_oexp",
                    "_totcustomhire_direct", "_HiredLabor_oexp", "_Mach_Lease_oepx", "_Build_Leas_oepx", "_FarmInsurance_oexp",
                    "_Utility_oexp", "_Dues_oexp", "_Interest_oexp", "_Depreciation_oexp","_RealEstTax_oexp",
                    "_Misc_oexp","_totoverheadexp", "_IncTax_finance", "_OwnerWithdrawl_finance", "_princpayment_finance","_totalFertilizerExpenses",
                    "_totdirectexp" , "_returnovrdirect","_totalfinancial"
                };

                object[] userValues = {
                    Convert.ToDouble(zoneyeardetails.ExpectedYield),  Convert.ToDouble(zoneyeardetails.OperatorShare), Convert.ToDouble(zoneyeardetails.CommoditySalePrice),
                    Convert.ToDouble(zoneyeardetails.OtherProductReturn) , Convert.ToDouble(zoneyeardetails.TotalProductReturn) , Convert.ToDouble(zoneyeardetails.HedgeGainLoss), Convert.ToDouble(zoneyeardetails.CropInsurancePayments) , Convert.ToDouble(zoneyeardetails.StewardshipPayments),
                    Convert.ToDouble(zoneyeardetails.GovernmentPayments) ,  Convert.ToDouble(zoneyeardetails.TotalGrossRevenue) , Convert.ToDouble(zoneyeardetails.Seed) ,  Convert.ToDouble(zoneyeardetails.Nitrogen) , Convert.ToDouble(zoneyeardetails.Phosphorus),
                    Convert.ToDouble(zoneyeardetails.Potash), Convert.ToDouble(zoneyeardetails.Sulfur), Convert.ToDouble(zoneyeardetails.Lime) , Convert.ToDouble(zoneyeardetails.SeedOther), Convert.ToDouble(zoneyeardetails.Herbicide),
                    Convert.ToDouble(zoneyeardetails.Fungicide) , Convert.ToDouble(zoneyeardetails.Insecticide) ,Convert.ToDouble(zoneyeardetails.CropChemicalOther) , Convert.ToDouble(zoneyeardetails.TotalCropChemicalExpenses), Convert.ToDouble(zoneyeardetails.Miscellaneous) ,
                    Convert.ToDouble(zoneyeardetails.CoverCrop), Convert.ToDouble(zoneyeardetails.Insurance),Convert.ToDouble(zoneyeardetails.DryingPropane) , Convert.ToDouble(zoneyeardetails.EquipmentFuel) , Convert.ToDouble(zoneyeardetails.Machinery),
                    Convert.ToDouble(zoneyeardetails.Buildings) ,  Convert.ToDouble(zoneyeardetails.RepairMaintenanceOther),  Convert.ToDouble(zoneyeardetails.TotalRepairMaintenance),  Convert.ToDouble(zoneyeardetails.Driver) ,  Convert.ToDouble(zoneyeardetails.Equipment),
                    Convert.ToDouble(zoneyeardetails.CustomApplicationt) , Convert.ToDouble(zoneyeardetails.CustomHireOther) ,Convert.ToDouble(zoneyeardetails.TotalCustomHire) ,Convert.ToDouble(zoneyeardetails.HiredLabor) , Convert.ToDouble(zoneyeardetails.Repairs),
                    Convert.ToDouble(zoneyeardetails.FuelElectricity),   Convert.ToDouble(zoneyeardetails.LeasesMachinery) ,  Convert.ToDouble(zoneyeardetails.LeasesBuildings) ,  Convert.ToDouble(zoneyeardetails.LandRent) ,  Convert.ToDouble(zoneyeardetails.StewardshipImplementation),
                    Convert.ToDouble(zoneyeardetails.Storage) , Convert.ToDouble(zoneyeardetails.Supplies) , Convert.ToDouble(zoneyeardetails.Utilities) , Convert.ToDouble(zoneyeardetails.FreightTrucking) , Convert.ToDouble(zoneyeardetails.Marketing) , Convert.ToDouble(zoneyeardetails.OtherCosts),
                    Convert.ToDouble(zoneyeardetails.InterestOperating),  Convert.ToDouble(zoneyeardetails.OverheadExpensesDriver) , Convert.ToDouble(zoneyeardetails.OverheadExpensesEquipment) , Convert.ToDouble(zoneyeardetails.OverheadExpensesCustomApplication) , Convert.ToDouble(zoneyeardetails.OverheadExpensesOther),
                    Convert.ToDouble(zoneyeardetails.OverheadExpensesTotalCustomHire),  Convert.ToDouble(zoneyeardetails.OtherExpensesHiredLabor), Convert.ToDouble(zoneyeardetails.OtherExpensesMachineryLease) , Convert.ToDouble(zoneyeardetails.OtherExpensesBuildingLeases) , Convert.ToDouble(zoneyeardetails.OtherExpensesFarmInsurance)  ,
                    Convert.ToDouble(zoneyeardetails.OtherExpensesUtilities), Convert.ToDouble(zoneyeardetails.OtherExpensesDuesProfessionalFees) , Convert.ToDouble(zoneyeardetails.OtherExpensesInterest) , Convert.ToDouble(zoneyeardetails.OtherExpensesMachineBuildingDepreciation) , Convert.ToDouble(zoneyeardetails.OtherExpensesRealEstateTaxes) ,
                    Convert.ToDouble(zoneyeardetails.OtherExpensesOtherOverheadExpenses) , Convert.ToDouble(zoneyeardetails.TotalOverheadExpenses),Convert.ToDouble(zoneyeardetails.FinancingIncomeTaxes),Convert.ToDouble(zoneyeardetails.FinancingOwnerWithdrawal) , Convert.ToDouble(zoneyeardetails.FinancingPrincipalPayment),Convert.ToDouble(zoneyeardetails.TotalFertilizerExpenses),
                    Convert.ToDouble(zoneyeardetails.TotalDirectExpense), Convert.ToDouble(zoneyeardetails.ReturnOverDirectExpenseperAcre), Convert.ToDouble(zoneyeardetails.TotalFinancing)
                };

                string updateArea_Query = $"select * from \"FSRCalc\".water_updatesamefield_management_zone_financial(_field_id => {Convert.ToInt32(id)}, _crop_name =>'{cropname}',";

                for (int i = 0; i < userParams.Length; i++)
                {
                    updateArea_Query += $"{userParams[i]} => {userValues[i]},";
                }
                updateArea_Query = updateArea_Query.TrimEnd(',') + ");";

                DataSet EditZoneData = _masterRepository.GetAndSelectTableItemsWithoutCursor(updateArea_Query);

                return Json(new { Success = true, Message = "The zone financial details update successfully." });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult FinancialPlansSave(string id, string name, string copiedData, string plansname)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                var zoneyeardetails = JsonConvert.DeserializeObject<FarmingFinancialData>(copiedData);

                object[] userParams = {
                     "_expectedyield", "_operatorpercent", "_cashprice",
                    "_otherreturn","_croprevenue","_hedgegainloss", "_cropinspayment", "_stewardshippayments",
                    "_govtpayments","_totalgrossrevenue" , "_seed_direct", "_nfert_direct", "_pfert_direct",
                    "_potfert_direct", "_sfert_direct", "_lime_direct", "_otherfert_direct",  "_herbicide_direct",
                    "_funicide_direct","_insecticide_direct","_otherchem_direct" ,"_totfert_direct","_cropmisc_direct",
                    "_covercrop_direct", "_cropinsur_direct", "_DryPropane_direct", "_EquipFuel_direct", "_MachRepair_direct",
                    "_BldgRepair_direct", "_otherrepair_direct","_totothercrop_direct", "_CustomDriver_direct", "_CustomEquipHire_direct",
                    "_CustomApp_direct", "_othercustomhire_direct","_totcustomhire_oexp", "_HiredLabor_direct", "_IrrigationRepair_direct",
                    "_IrrigPower_direct", "_Machinery_direct", "_Building_direct", "_LandRent_direct", "_Stewardship_direct",
                    "_Storage_direct","_Supplies_direct", "_Utilities_direct", "_FreightTrucking_direct", "_Marketing", "_Other_direct",
                    "_Interest_direct", "_CustomDriver_oexp", "_CustomEquipHire_oexp", "_CustomApp_oexp","_othercustomhire_oexp",
                    "_totcustomhire_direct", "_HiredLabor_oexp", "_Mach_Lease_oepx", "_Build_Leas_oepx", "_FarmInsurance_oexp",
                    "_Utility_oexp", "_Dues_oexp", "_Interest_oexp", "_Depreciation_oexp","_RealEstTax_oexp",
                    "_Misc_oexp","_totoverheadexp", "_IncTax_finance", "_OwnerWithdrawl_finance", "_princpayment_finance","_totalFertilizerExpenses",
                    "_totdirectexp" , "_returnovrdirect","_totalfinancial"

                };

                object[] userValues = {
                    Convert.ToDouble(zoneyeardetails.ExpectedYield),  Convert.ToDouble(zoneyeardetails.OperatorShare), Convert.ToDouble(zoneyeardetails.CommoditySalePrice),
                    Convert.ToDouble(zoneyeardetails.OtherProductReturn) , Convert.ToDouble(zoneyeardetails.TotalProductReturn) , Convert.ToDouble(zoneyeardetails.HedgeGainLoss), Convert.ToDouble(zoneyeardetails.CropInsurancePayments) , Convert.ToDouble(zoneyeardetails.StewardshipPayments),
                    Convert.ToDouble(zoneyeardetails.GovernmentPayments) ,  Convert.ToDouble(zoneyeardetails.TotalGrossRevenue) , Convert.ToDouble(zoneyeardetails.Seed) ,  Convert.ToDouble(zoneyeardetails.Nitrogen) , Convert.ToDouble(zoneyeardetails.Phosphorus),
                    Convert.ToDouble(zoneyeardetails.Potash), Convert.ToDouble(zoneyeardetails.Sulfur), Convert.ToDouble(zoneyeardetails.Lime) , Convert.ToDouble(zoneyeardetails.SeedOther), Convert.ToDouble(zoneyeardetails.Herbicide),
                    Convert.ToDouble(zoneyeardetails.Fungicide) , Convert.ToDouble(zoneyeardetails.Insecticide) ,Convert.ToDouble(zoneyeardetails.CropChemicalOther) , Convert.ToDouble(zoneyeardetails.TotalCropChemicalExpenses), Convert.ToDouble(zoneyeardetails.Miscellaneous) ,
                    Convert.ToDouble(zoneyeardetails.CoverCrop), Convert.ToDouble(zoneyeardetails.Insurance),Convert.ToDouble(zoneyeardetails.DryingPropane) , Convert.ToDouble(zoneyeardetails.EquipmentFuel) , Convert.ToDouble(zoneyeardetails.Machinery),
                    Convert.ToDouble(zoneyeardetails.Buildings) ,  Convert.ToDouble(zoneyeardetails.RepairMaintenanceOther),  Convert.ToDouble(zoneyeardetails.TotalRepairMaintenance),  Convert.ToDouble(zoneyeardetails.Driver) ,  Convert.ToDouble(zoneyeardetails.Equipment),
                    Convert.ToDouble(zoneyeardetails.CustomApplicationt) , Convert.ToDouble(zoneyeardetails.CustomHireOther) ,Convert.ToDouble(zoneyeardetails.TotalCustomHire) ,Convert.ToDouble(zoneyeardetails.HiredLabor) , Convert.ToDouble(zoneyeardetails.Repairs),
                    Convert.ToDouble(zoneyeardetails.FuelElectricity),   Convert.ToDouble(zoneyeardetails.LeasesMachinery) ,  Convert.ToDouble(zoneyeardetails.LeasesBuildings) ,  Convert.ToDouble(zoneyeardetails.LandRent) ,  Convert.ToDouble(zoneyeardetails.StewardshipImplementation),
                    Convert.ToDouble(zoneyeardetails.Storage) , Convert.ToDouble(zoneyeardetails.Supplies) , Convert.ToDouble(zoneyeardetails.Utilities) , Convert.ToDouble(zoneyeardetails.FreightTrucking) , Convert.ToDouble(zoneyeardetails.Marketing) , Convert.ToDouble(zoneyeardetails.OtherCosts),
                    Convert.ToDouble(zoneyeardetails.InterestOperating),  Convert.ToDouble(zoneyeardetails.OverheadExpensesDriver) , Convert.ToDouble(zoneyeardetails.OverheadExpensesEquipment) , Convert.ToDouble(zoneyeardetails.OverheadExpensesCustomApplication) , Convert.ToDouble(zoneyeardetails.OverheadExpensesOther),
                    Convert.ToDouble(zoneyeardetails.OverheadExpensesTotalCustomHire),  Convert.ToDouble(zoneyeardetails.OtherExpensesHiredLabor), Convert.ToDouble(zoneyeardetails.OtherExpensesMachineryLease) , Convert.ToDouble(zoneyeardetails.OtherExpensesBuildingLeases) , Convert.ToDouble(zoneyeardetails.OtherExpensesFarmInsurance)  ,
                    Convert.ToDouble(zoneyeardetails.OtherExpensesUtilities), Convert.ToDouble(zoneyeardetails.OtherExpensesDuesProfessionalFees) , Convert.ToDouble(zoneyeardetails.OtherExpensesInterest) , Convert.ToDouble(zoneyeardetails.OtherExpensesMachineBuildingDepreciation) , Convert.ToDouble(zoneyeardetails.OtherExpensesRealEstateTaxes) ,
                    Convert.ToDouble(zoneyeardetails.OtherExpensesOtherOverheadExpenses) , Convert.ToDouble(zoneyeardetails.TotalOverheadExpenses),Convert.ToDouble(zoneyeardetails.FinancingIncomeTaxes),Convert.ToDouble(zoneyeardetails.FinancingOwnerWithdrawal) , Convert.ToDouble(zoneyeardetails.FinancingPrincipalPayment),Convert.ToDouble(zoneyeardetails.TotalFertilizerExpenses),
                    Convert.ToDouble(zoneyeardetails.TotalDirectExpense), Convert.ToDouble(zoneyeardetails.ReturnOverDirectExpenseperAcre), Convert.ToDouble(zoneyeardetails.TotalFinancing)
                };

                string updateArea_Query = $"select * from \"FSRCalc\".financial_plans_save(_field_id => {Convert.ToInt32(id)}, _plans_name =>'{plansname}',";

                for (int i = 0; i < userParams.Length; i++)
                {
                    updateArea_Query += $"{userParams[i]} => {userValues[i]},";
                }
                updateArea_Query = updateArea_Query.TrimEnd(',') + ");";

                DataSet EditZoneData = _masterRepository.GetAndSelectTableItemsWithoutCursor(updateArea_Query);

                return Json(new { Success = true, Message = "The financial Plans save successfully." });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Json(new { Success = false, Message = "No Data" });
            }
        }

        public IActionResult GetFinancialPlans(int financialPlansSelect)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                string GetFinancialPlans_Query = $"select * from \"FSRCalc\".financial_plans_ref(datarefcursor => 'data', _actiontype => 'GetFinancialPlansById', _financial_plans_id =>'{financialPlansSelect}');";

                DataSet FinancialPlansData = _masterRepository.GetAndSelectTableItems(GetFinancialPlans_Query);

                DataTable dtFinancialPlansData = FinancialPlansData.Tables.Count > 0 ? FinancialPlansData.Tables[0] : null;

                if (FinancialPlansData != null && FinancialPlansData.Tables.Count > 0 && dtFinancialPlansData.Rows.Count > 0)
                {
                    return Json(new { success = true, data = JsonConvert.SerializeObject(dtFinancialPlansData) });
                }
                else
                {
                    return Json(new { scuccess = false, data = "No Data" });
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Json(new { success = false, data = "No Data" });
            }
        }

        [HttpGet]
        public IActionResult DeleteProducer(int ProducerId)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                string query = $"SELECT * FROM \"FSRCalc\".DeleteAdvisor_Prodcuer_Fields_ref(_actiontype=> 'DeleteProducers',datarefcursor =>'data', _producer_id =>{ProducerId});";
                DataSet DsPricipYearData = _masterRepository.GetAndSelectTableItems(query);

                TempData["Success"] = "Producer delete successfully.";
                // return Redirect("Producers");
                return Json(new { Success = true, Message = "Producer delete successfully." });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        [HttpGet]
        public IActionResult DeleteAdvisor(int AdvisorsId)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                string query = $"SELECT * FROM \"FSRCalc\".DeleteAdvisor_Prodcuer_Fields_ref(_actiontype=> 'DeleteAdvisor',datarefcursor =>'data', _advisor_id =>{AdvisorsId});";
                DataSet DsPricipYearData = _masterRepository.GetAndSelectTableItems(query);

                TempData["Success"] = "Advisor delete successfully.";
                // return Redirect("Advisors");
                return Json(new { Success = true, Message = "Advisor delete successfully." });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult DeleteField(int FieldId)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                string query = $"SELECT * FROM \"FSRCalc\".DeleteAdvisor_Prodcuer_Fields_ref(_actiontype=> 'DeleteField',datarefcursor =>'data', _field_id =>{FieldId});";
                DataSet DsPricipYearData = _masterRepository.GetAndSelectTableItems(query);

                TempData["Success"] = "Field delete successfully.";
                // return Redirect("/Admin/MyFields");
                return Json(new { Success = true, Message = "Field delete successfully." });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult UnderstandingtheFSRCalculation()
        {
            return View();
        }

        public IActionResult InterpretingEnvironmentalOutcomes()
        {
            return View();
        }

        public IActionResult Whatdoesanadvisordo()
        {
            return View();
        }

        public IActionResult Interpretingyourfinancialresults()
        {
            return View();
        }

        public IActionResult YourFieldPlans()
        {
            return View();
        }

        public IActionResult ImproveyourFieldStewardshipRating()
        {
            return View();
        }
        public IActionResult StewardshipIndicesOverview()
        {
            return View();
        }

        public IActionResult FarmPopup(int ProducerId)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                ViewBag.Farm = commonMethods.GetFarmByPro(ProducerId);

                return PartialView("~/Views/Common/_viewfarmpopup.cshtml");


            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        public IActionResult SaveFarmName(int FarmId, string farmname)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                // ViewBag.Farm = commonMethods.GetFarmByPro(ProducerId);

                string query = $"SELECT * FROM \"FSRCalc\".water_editfarm_ref(_actiontype=> 'UpdateFarm',datarefcursor =>'data', _farm_id =>{FarmId}, _name =>'{farmname}');";
                DataSet dsFarmData = _masterRepository.GetAndSelectTableItems(query);

                ViewBag.Farm = (from DataRow Row in dsFarmData.Tables[0].Rows
                                select new Farm
                                {
                                    FarmId = Convert.ToInt32(Row["farm_id"]),
                                    FarmName = Convert.ToString(Row["farm_name"]),

                                }).ToList();

                if (dsFarmData.Tables[0].Rows.Count != null)
                {
                    TempData["Success"] = "The farm saved successfully.";
                }
                return PartialView("~/Views/Common/_viewfarmpopup.cshtml");


            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }

        [HttpPost]
        public IActionResult SaveAllFinancialData(string formsStringObject)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] userParams = {
                        "_management_zone_id", "_year","_field_option_id", "_expectedyield", "_operatorpercent", "_cashprice",
                        "_otherreturn","_croprevenue","_hedgegainloss", "_cropinspayment", "_stewardshippayments",
                        "_govtpayments","_totalgrossrevenue" , "_seed_direct", "_nfert_direct", "_pfert_direct",
                        "_potfert_direct", "_sfert_direct", "_lime_direct", "_otherfert_direct",  "_herbicide_direct",
                        "_funicide_direct","_insecticide_direct","_otherchem_direct" ,"_totfert_direct","_cropmisc_direct",
                        "_covercrop_direct", "_cropinsur_direct", "_DryPropane_direct", "_EquipFuel_direct", "_MachRepair_direct",
                        "_BldgRepair_direct", "_otherrepair_direct","_totothercrop_direct", "_CustomDriver_direct", "_CustomEquipHire_direct",
                        "_CustomApp_direct", "_othercustomhire_direct","_totcustomhire_oexp", "_HiredLabor_direct", "_IrrigationRepair_direct",
                        "_IrrigPower_direct", "_Machinery_direct", "_Building_direct", "_LandRent_direct", "_Stewardship_direct",
                        "_Storage_direct","_Supplies_direct", "_Utilities_direct", "_FreightTrucking_direct", "_Marketing", "_Other_direct",
                        "_Interest_direct", "_CustomDriver_oexp", "_CustomEquipHire_oexp", "_CustomApp_oexp","_othercustomhire_oexp",
                        "_totcustomhire_direct", "_HiredLabor_oexp", "_Mach_Lease_oepx", "_Build_Leas_oepx", "_FarmInsurance_oexp",
                        "_Utility_oexp", "_Dues_oexp", "_Interest_oexp", "_Depreciation_oexp","_RealEstTax_oexp",
                        "_Misc_oexp","_totoverheadexp", "_IncTax_finance", "_OwnerWithdrawl_finance", "_princpayment_finance","_totalFertilizerExpenses",
                        "_totdirectexp" , "_returnovrdirect","_totalfinancial","_isvalueschanged"
                    };
                var formData = JsonConvert.DeserializeObject<dynamic>(formsStringObject);
                foreach (var formDataRow in formData)
                {
                    Dictionary<string, string> formDataDictionary = new Dictionary<string, string>();
                    foreach (var formDataRowItem in formDataRow)
                    {
                        string name = formDataRowItem["name"];
                        string valueFilter = formDataRowItem["value"];
                        string value = valueFilter.Replace(",", "");
                        formDataDictionary[name] = value;

                    }

                    //create query
                    object[] userValues = {
                     formDataDictionary["Management_Zone_Id"], formDataDictionary["Year"],formDataDictionary["FieldOptionId"], formDataDictionary["ExpectedYield"],  formDataDictionary["OperatorShare"], formDataDictionary["CommoditySalePrice"],
                     formDataDictionary["OtherProductReturn"] , formDataDictionary["TotalProductReturn"] , formDataDictionary["HedgeGainLoss"], formDataDictionary["CropInsurancePayments"] , formDataDictionary["StewardshipPayments"],
                     formDataDictionary["GovernmentPayments"] ,  formDataDictionary["TotalGrossRevenue"] , formDataDictionary["Seed"] ,  formDataDictionary["Nitrogen"] , formDataDictionary["Phosphorus"],
                     formDataDictionary["Potash"], formDataDictionary["Sulfur"], formDataDictionary["Lime"] , formDataDictionary["SeedOther"], formDataDictionary["Herbicide"],
                     formDataDictionary["Fungicide"] , formDataDictionary["Insecticide"] ,formDataDictionary["CropChemicalOther"] , formDataDictionary["TotalCropChemicalExpenses"], formDataDictionary["Miscellaneous"] ,
                     formDataDictionary["CoverCrop"], formDataDictionary["Insurance"],formDataDictionary["DryingPropane"] , formDataDictionary["EquipmentFuel"] , formDataDictionary["Machinery"],
                     formDataDictionary["Buildings"] ,  formDataDictionary["RepairMaintenanceOther"],  formDataDictionary["TotalRepairMaintenance"],  formDataDictionary["Driver"] ,  formDataDictionary["Equipment"],
                     formDataDictionary["CustomApplicationt"] , formDataDictionary["CustomHireOther"] ,formDataDictionary["TotalCustomHire"] ,formDataDictionary["HiredLabor"] , formDataDictionary["Repairs"],
                     formDataDictionary["FuelElectricity"],   formDataDictionary["LeasesMachinery"] ,  formDataDictionary["LeasesBuildings"] ,  formDataDictionary["LandRent"] ,  formDataDictionary["StewardshipImplementation"],
                     formDataDictionary["Storage"] , formDataDictionary["Supplies"] , formDataDictionary["Utilities"] , formDataDictionary["FreightTrucking"] , formDataDictionary["Marketing"] , formDataDictionary["OtherCosts"],
                     formDataDictionary["InterestOperating"],  formDataDictionary["OverheadExpensesDriver"] , formDataDictionary["OverheadExpensesEquipment"] , formDataDictionary["OverheadExpensesCustomApplication"] , formDataDictionary["OverheadExpensesOther"],
                     formDataDictionary["OverheadExpensesTotalCustomHire"],  formDataDictionary["OtherExpensesHiredLabor"], formDataDictionary["OtherExpensesMachineryLease"] , formDataDictionary["OtherExpensesBuildingLeases"] , formDataDictionary["OtherExpensesFarmInsurance"]  ,
                     formDataDictionary["OtherExpensesUtilities"], formDataDictionary["OtherExpensesDuesProfessionalFees"] , formDataDictionary["OtherExpensesInterest"] , formDataDictionary["OtherExpensesMachineBuildingDepreciation"] , formDataDictionary["OtherExpensesRealEstateTaxes"] ,
                     formDataDictionary["OtherExpensesOtherOverheadExpenses"] , formDataDictionary["TotalOverheadExpenses"],formDataDictionary["FinancingIncomeTaxes"],formDataDictionary["FinancingOwnerWithdrawal"] , formDataDictionary["FinancingPrincipalPayment"],formDataDictionary["TotalFertilizerExpenses"],
                     formDataDictionary["TotalDirectExpense"], formDataDictionary["ReturnOverDirectExpenseperAcre"], formDataDictionary["TotalFinancing"],false
                 };

                    string query = $"SELECT * from \"FSRCalc\".water_update_management_zone_financial_new_testing17_ref(datarefcursor => 'data', _quality_grade =>'{formDataDictionary["DataQualityGrade"]}',";
                    for (int i = 0; i < userParams.Length; i++)
                    {
                        query += $"{userParams[i]} => {userValues[i]},";
                    }
                    query = query.TrimEnd(',') + ");";

                    DataSet EditZoneData = _masterRepository.GetAndSelectTableItems(query);
                    if(EditZoneData == null && EditZoneData.Tables.Count > 0 && EditZoneData.Tables[0].Rows.Count < 0)
                    {
                        return Json(new { success = false, message = "Failed to save data" });
                        Console.WriteLine($"financila data saving: Management_Zone_Id:{formDataDictionary["Management_Zone_Id"]}, Year:{formDataDictionary["Year"]}, FieldOptionId:{formDataDictionary["FieldOptionId"]}");
                    }
                }

                TempData["Success"] = "All forms data saved successfully.";
                return Json(new { success = true, message = "data saved successfully" });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Error", "Common");
            }
        }
    }
}

