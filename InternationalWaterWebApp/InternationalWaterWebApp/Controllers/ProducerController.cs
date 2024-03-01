using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using InternationalWaterWebApp.Library.Common;
using InternationalWaterWebApp.Library.ViewModel;
using InternationalWaterWebApp.Models;
using InternationalWaterWebApp.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace InternationalWaterWebApp.Controllers
{
    [AuthorizationCS((int)UserTypeEnum.Producer)]
    public class ProducerController : BaseController
    {
        private CommonMethods commonMethods;
        private EncryptDecryptData encryptDecryptData;
        public ProducerController()
        {
            commonMethods = new CommonMethods();
            encryptDecryptData = new EncryptDecryptData();
        }
        public IActionResult Landing(string DropdownFrom, string DropdownTo)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            try
            {
                string yearfrom = string.Empty, yearto = string.Empty;

                var Producer_Id = commonMethods.GetProducerByUserID(LoginUserId);
                DataSet currentyeardata = null;
                string currentyear = $"select * from \"FSRCalc\".water_frs_get_years_by_producer_id(datarefcursor =>'data',_producer_id => {Producer_Id});";
                currentyeardata = _masterRepository.GetAndSelectTableItems(currentyear);
                DataTable currentyear_data_DT = currentyeardata.Tables.Count > 0 ? currentyeardata.Tables[0] : null;
                if (currentyear_data_DT != null && currentyear_data_DT.Rows.Count > 0)
                {
                    yearfrom = currentyear_data_DT.Rows[0][0].ToString();
                    int lastIndex = currentyear_data_DT.Rows.Count - 1;
                    yearto = currentyear_data_DT.Rows[lastIndex][0].ToString();
                }

                string valuefrom = DropdownFrom ?? yearfrom;
                string valueto = DropdownTo ?? yearto;

                List<SelectListItem> li = new List<SelectListItem>();
                foreach (DataRow row in currentyeardata.Tables[0].Rows)
                {
                    li.Add(new SelectListItem() { Text = row["year"].ToString(), Value = row["year"].ToString() });
                }
                ViewBag.Dropdownfrom = new SelectList(li, "Value", "Text", valuefrom);
                ViewBag.Dropdownto = new SelectList(li, "Value", "Text", valueto);

                object[] agrParams = { "_ActionType", "_user_id" };
                object[] agrValues = { "GetAdvisorByProducer", LoginUserId };

                DataSet dsAdvisorData = _masterRepository.GetAndSelectTableItems($"SELECT * from \"Users\".water_advisors_ref (datarefcursor => 'data',{agrParams[0]} => '{agrValues[0]}',{agrParams[1]}=>{agrValues[1]});");
                ViewBag.AdvisorInfo = dsAdvisorData;
                if (dsAdvisorData != null && dsAdvisorData.Tables[0].Rows.Count > 0)
                {
                    var advisoremail = Convert.ToString(dsAdvisorData.Tables[0].Rows[0]["email"]);
                    HttpContext.Session.SetString("AdvisorEmail", advisoremail);
                }

                object[] userParams = { "_ActionType", "_producer_id" };
                object[] userFarmValues = { "GetFarmByProducerId", Producer_Id };
                object[] userFieldValues = { "GetFieldByProducerId", Producer_Id };

                //string QueryFarm = $"SELECT * from  \"FSRCalc\".water_producer_ref (datarefcursor => 'data',{userParams[0]} => '{userFarmValues[0]}',{userParams[1]} => {userFarmValues[1]});";
                //DataSet DsFarmData = _masterRepository.GetAndSelectTableItems(QueryFarm, userParams, userFarmValues);
                //ViewBag.Farm = DsFarmData;

                int firstValue = Convert.ToInt32(valuefrom == string.Empty ? 0 : valuefrom);
                int secondValue = Convert.ToInt32(valueto == string.Empty ? 0 : valueto);

                string QueryFarm = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details1(_producer_id => {Producer_Id});";
                DataSet DsFarmData = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm);
                ViewBag.Farm = DsFarmData;

                string QueryFarm2 = string.Empty;
                DataSet DsFarmData2 = new DataSet();
                if (firstValue != 0 && secondValue != 0)
                {
                    QueryFarm2 = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details2(_producer_id => {Producer_Id} , yearfrom => {firstValue} , yearto => {secondValue},_actiontype=>'GetWithYear');";
                    DsFarmData2 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm2);
                }
                else
                {
                    QueryFarm2 = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details2(_producer_id => {Producer_Id} , yearfrom => {firstValue} , yearto => {secondValue},_actiontype=>'GetWithoutYear');";
                    DsFarmData2 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm2);

                }
                ViewBag.Farm2 = DsFarmData2;
                //string QueryFarm2 = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details2(_producer_id => {Producer_Id} , yearfrom => {firstValue} , yearto => {secondValue},_actiontype=>'GetWithYear');";
                //DataSet DsFarmData2 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm2);
                //ViewBag.Farm2 = DsFarmData2;

                string QueryFarm3 = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details3(_producer_id => {Producer_Id} , yearfrom => {firstValue} , yearto => {secondValue},_actiontype=>'GetWithYear');";
                DataSet DsFarmData3 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm3);
                ViewBag.Farm3 = DsFarmData3;


                DataTable fsr_data = DsFarmData.Tables[0];
                decimal FarmaerSRatingBox = 0;
                decimal FarmaerareaBox = 0;
                decimal totalarea;
                if (DsFarmData != null && DsFarmData.Tables.Count > 0 && DsFarmData.Tables[0] != null)
                {
                    for (int i = 0; i < DsFarmData3.Tables[0]?.Rows.Count; i++)
                    {

                        decimal FarmerSRatingBox = DsFarmData3.Tables[0]?.Rows[i]["total_fsr"].ToString() != null ? Convert.ToDecimal(DsFarmData3.Tables[0]?.Rows[i]["total_fsr"].ToString()) : 0.0M;

                        decimal TotalArea = DsFarmData3.Tables[0]?.Rows[i]["total_area"].ToString() != null ? Convert.ToDecimal(DsFarmData3.Tables[0]?.Rows[i]["total_area"].ToString()) : 0.0M;

                        decimal result = FarmerSRatingBox * TotalArea;

                        FarmaerSRatingBox += result;
                        if (decimal.TryParse(DsFarmData3.Tables[0]?.Rows[i]["total_area"].ToString(), out totalarea))
                        {
                            FarmaerareaBox += totalarea;
                        }
                    }
                }

                ViewBag.FSRBox = FarmaerareaBox != 0.00M ? FarmaerSRatingBox / FarmaerareaBox : 0.00M;

                string QueryField = $"SELECT * from  \"FSRCalc\".water_producer_ref (datarefcursor => 'data',{userParams[0]} => '{userFieldValues[0]}',{userParams[1]} => {userFieldValues[1]});";
                DataSet DsFieldData = _masterRepository.GetAndSelectTableItems(QueryField);
                ViewBag.Field = DsFieldData;
                return View();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Login", "WebApp");
            }
        }

        public IActionResult MyFields(string DropdownFrom, string DropdownTo)
        {
            try
            {
                var Producer_Id = commonMethods.GetProducerByUserID(LoginUserId);
                IMasterRepository _masterRepository = new MasterRepository();

                DataSet currentyeardata = null;
                string currentyear = $"select * from \"FSRCalc\".water_frs_get_years_by_producer_id(datarefcursor =>'data',_producer_id => {Producer_Id});";
                currentyeardata = _masterRepository.GetAndSelectTableItems(currentyear);

                DataTable currentyeardataDT = currentyeardata?.Tables[0];
                string yearfrom = currentyeardataDT?.Rows.Count > 0 ? currentyeardataDT.Rows[0][0]?.ToString() : string.Empty;
                string yearto = currentyeardataDT?.Rows.Count > 0 ? currentyeardataDT.Rows[^1][0]?.ToString() : string.Empty;

                string valuefrom = DropdownFrom ?? yearfrom ?? string.Empty;
                string valueto = DropdownTo ?? yearto ?? string.Empty;

                int firstValue = Convert.ToInt32(valuefrom == string.Empty ? 0 : valuefrom);
                int secondValue = Convert.ToInt32(valueto == string.Empty ? 0 : valueto);

                List<SelectListItem> li = currentyeardataDT?.AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Text = row["year"].ToString(),
                    Value = row["year"].ToString()
                }).ToList() ?? new List<SelectListItem>();

                //ViewBag.Dropdownfrom = new SelectList(li, "Value", "Text", valuefrom);
                //ViewBag.Dropdownto = new SelectList(li, "Value", "Text", valueto);

                object[] userParams = { "_ActionType", "_producer_id" };
                object[] userValues = { "GetFieldByProducerId", Producer_Id };
                //  string query = $"SELECT * from  \"FSRCalc\".water_producer_Fieldref (datarefcursor => 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => {userValues[1]});";
                string query = $"SELECT * from  \"FSRCalc\".water_producer_Fieldref (datarefcursor => 'data',_producer_id => {Producer_Id} , yearfrom => {firstValue} , yearto => {secondValue});";
                DataSet DsFieldData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.Field = DsFieldData;

                string QueryFarm2 = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details2(_producer_id => {Producer_Id} , yearfrom => {firstValue} , yearto => {secondValue},_actiontype=>'GetWithYear');";
                DataSet DsFarmData2 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm2);
                ViewBag.Farm2 = DsFarmData2;

                string QueryField3 = $"SELECT * from \"FSRCalc\".frs_new_get_my_fields_yearly(_producer_id => {Producer_Id} ,_field_option_id =>1 , yearfrom => {firstValue} , yearto => {secondValue},_actiontype =>'GetWithoutFieldoption');";
                DataSet DsFieldData3 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryField3);
                ViewBag.Field3 = DsFieldData3;

                return View();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                ViewBag.Error = "No Data Found.";
                return View();
            }
        }

        public IActionResult MyFarms(string DropdownFrom, string DropdownTo)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            try
            {
                var Producer_Id = commonMethods.GetProducerByUserID(LoginUserId);
                DataSet currentyeardata = null;
                string currentyear = $"select * from \"FSRCalc\".water_frs_get_years_by_producer_id(datarefcursor =>'data',_producer_id => {Producer_Id});";
                currentyeardata = _masterRepository.GetAndSelectTableItems(currentyear);

                DataTable currentyeardataDT = currentyeardata?.Tables[0];

                string yearfrom = currentyeardataDT?.Rows.Count > 0 ? currentyeardataDT.Rows[0][0]?.ToString() : "";
                string yearto = currentyeardataDT?.Rows.Count > 0 ? currentyeardataDT.Rows[^1][0]?.ToString() : "";

                string valuefrom = DropdownFrom ?? yearfrom;
                string valueto = DropdownTo ?? yearto;

                List<SelectListItem> li = currentyeardataDT?.AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Text = row["year"].ToString(),
                    Value = row["year"].ToString()
                }).ToList() ?? new List<SelectListItem>();

                ViewBag.Dropdownfrom = new SelectList(li, "Value", "Text", valuefrom);
                ViewBag.Dropdownto = new SelectList(li, "Value", "Text", valueto);

                object[] agrParams = { "_ActionType", "_user_id" };
                object[] agrValues = { "GetAdvisorByProducer", LoginUserId };

                DataSet dsAdvisorData = _masterRepository.GetAndSelectTableItems($"SELECT * from \"Users\".water_advisors_ref (datarefcursor => 'data',{agrParams[0]} => '{agrValues[0]}',{agrParams[1]}=>{agrValues[1]});");
                ViewBag.AdvisorInfo = dsAdvisorData;
                if (dsAdvisorData != null && dsAdvisorData.Tables[0].Rows.Count > 0)
                {
                    var advisoremail = Convert.ToString(dsAdvisorData.Tables[0].Rows[0]["email"]);
                    HttpContext.Session.SetString("AdvisorEmail", advisoremail);
                }

                object[] userParams = { "_ActionType", "_producer_id" };
                object[] userFarmValues = { "GetFarmByProducerId", Producer_Id };
                object[] userFieldValues = { "GetFieldByProducerId", Producer_Id };

                int firstValue = Convert.ToInt32(valuefrom == string.Empty ? 0 : valuefrom);
                int secondValue = Convert.ToInt32(valueto == string.Empty ? 0 : valueto);

                string QueryFarm = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details1(_producer_id => {Producer_Id});";
                DataSet DsFarmData = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm);
                ViewBag.Farm = DsFarmData;

                string QueryFarm2 = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details2(_producer_id => {Producer_Id} , yearfrom => {firstValue} , yearto => {secondValue},_actiontype=>'GetWithoutYear');";
                DataSet DsFarmData2 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm2);
                ViewBag.Farm2 = DsFarmData2;

                string QueryFarm3 = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details3(_producer_id => {Producer_Id} , yearfrom => {firstValue} , yearto => {secondValue},_actiontype=>'GetWithYear');";
                DataSet DsFarmData3 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm3);
                ViewBag.Farm3 = DsFarmData3;


                return View();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return RedirectToAction("Login", "WebApp");
            }
        }

        public IActionResult MyFarmDetails(string id, string name, string DropdownFrom, string DropdownTo)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                var Producer_Id = commonMethods.GetProducerByUserID(LoginUserId);
                DataSet currentyeardata = null;
                string currentyear = $"select * from \"FSRCalc\".water_frs_get_years_by_producer_id(datarefcursor =>'data',_producer_id => {Producer_Id});";
                currentyeardata = _masterRepository.GetAndSelectTableItems(currentyear);

                string yearfrom;
                string yearto;
                if (currentyeardata != null && currentyeardata.Tables.Count > 0 && currentyeardata.Tables[0].Rows.Count > 0)
                {
                    yearfrom = currentyeardata.Tables[0].Rows[0][0].ToString();
                    int lastIndex = currentyeardata.Tables[0].Rows.Count - 1;
                    yearto = currentyeardata.Tables[0].Rows[lastIndex][0].ToString();
                }
                else
                {
                    yearfrom = "0";
                    yearto = "0";
                }

                string valuefrom = "";
                string valueto = "";

                if (DropdownFrom == null && DropdownTo == null)
                {
                    valuefrom = yearfrom;
                    valueto = yearto;
                }
                else
                {
                    valuefrom = DropdownFrom;
                    valueto = DropdownTo;
                }


                List<SelectListItem> li = new List<SelectListItem>();
                foreach (DataRow row in currentyeardata.Tables[0].Rows)
                {
                    li.Add(new SelectListItem() { Text = row["year"].ToString(), Value = row["year"].ToString() });
                }
                ViewBag.Dropdownfrom = new SelectList(li, "Value", "Text", valuefrom);
                ViewBag.Dropdownto = new SelectList(li, "Value", "Text", valueto);


                int firstValue = Convert.ToInt32(valuefrom);
                int secondValue = Convert.ToInt32(valueto);

                int farm_id = int.Parse(encryptDecryptData.Decryptdata(id));
                ViewBag.FarmName = name;

                //string QueryFarm = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details_page1(_producer_id => {Producer_Id}, _farm_id => {farm_id});";
                //DataSet DsFarmData = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm);
                //ViewBag.FarmDeatils1 = DsFarmData;

                //Get farm's field
                //string QueryFarm3 = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details3(_producer_id => {Producer_Id} , yearfrom => {firstValue} , yearto => {secondValue},_actiontype=>'GetWithYear');";
                //DataSet DsFarmData3 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm3);


                //string QueryFarm2 = $"SELECT * from \"FSRCalc\".frs_new_get_farms_details_page2(_producer_id => {Producer_Id},_farm_id => {farm_id},yearfrom => {firstValue},toyear => {secondValue});";
                //DataSet DsFarmData2 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryFarm2);
                //ViewBag.FarmDeatils2 = DsFarmData2;

                //get my farms fields table

                object[] userParams = { "_ActionType", "_producer_id" };
                object[] userValues = { "GetFieldByProducerId", Producer_Id };

                string query = $"SELECT * from  \"FSRCalc\".water_producer_field_farm_ref (datarefcursor => 'data',_producer_id => {Producer_Id} ,_farm_id=>{farm_id}, yearfrom => {firstValue} , yearto => {secondValue});";
                DataSet DsFieldData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.Field = DsFieldData;

                string QueryField3 = $"SELECT * from \"FSRCalc\".frs_new_get_my_fields_yearly_farm(_producer_id => {Producer_Id} ,_farm_id=>{farm_id},_field_option_id =>1 , yearfrom => {firstValue} , yearto => {secondValue});";
                DataSet DsFieldData3 = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryField3);
                ViewBag.Field3 = DsFieldData3;
                int totalFieldCount = DsFieldData3.Tables[0].Rows.Count;

                double soil_management = 0;
                double water_management = 0;
                double nutrient_retention = 0;
                double surface_water = 0;
                double fertilizer_management = 0;
                double allover = 0;
                int field_count = 0;
                double fsrValue = 0;
                double totalarces = 0;
                for (int i = 0; i < totalFieldCount; i++)
                {
                    var table = DsFieldData3.Tables[0];
                    if (i < table.Rows.Count)
                    {
                        var row = table.Rows[i];
                        int FieldIds = Convert.ToInt32(row["field_id"]);
                        double acres = Convert.ToDouble(row["acres"]);
                        double fsr = row["fsr"] != null ? Math.Round(double.Parse(row["fsr"].ToString()), 2) : 0.0;
                        if (FieldIds != 0)
                        {

                            string query1 = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{FieldIds}, _start_year => {firstValue} , _end_year => {secondValue}, _field_option_id => 1);";
                            DataSet DsfsrData = _masterRepository.GetAndSelectTableItems(query1);

                            if (DsfsrData != null && DsfsrData.Tables[0].Rows.Count > 0 &&
                                DsfsrData.Tables[0].Rows[0]["rating_soil"].ToString() != "N/A" || DsfsrData.Tables[0].Rows[0]["rating_soil"].ToString() != "-" && DsfsrData.Tables[0].Rows[0]["rating_water"].ToString() != "N/A" || DsfsrData.Tables[0].Rows[0]["rating_water"].ToString() != "-")
                            {
                                soil_management += double.Parse(DsfsrData.Tables[0].Rows[0]["final_soil"].ToString());
                                water_management += double.Parse(DsfsrData.Tables[0].Rows[0]["final_water"].ToString());
                                nutrient_retention += double.Parse(DsfsrData.Tables[0].Rows[0]["final_nutrient"].ToString());
                                surface_water += double.Parse(DsfsrData.Tables[0].Rows[0]["final_surface"].ToString());
                                fertilizer_management += double.Parse(DsfsrData.Tables[0].Rows[0]["final_fertilizer"].ToString());
                                allover += double.Parse(DsfsrData.Tables[0].Rows[0]["sumofallgrade"].ToString());
                                double acresValue = double.Parse(acres.ToString());
                                double fsryearwise = double.Parse(DsfsrData.Tables[0].Rows[0]["fsr"].ToString());
                                double fsravgvalue = fsryearwise * acresValue;


                                fsrValue += fsravgvalue;

                                var rowToModify = DsFieldData3.Tables[0].AsEnumerable().Where(row => row.Field<int>("field_id") == FieldIds);

                                foreach (var row1 in rowToModify)
                                {
                                    row1.SetField("rating_fsr", DsfsrData.Tables[0].Rows[0]["rating_allover"].ToString());
                                }
                            }

                            totalarces += double.Parse(acres.ToString());
                            field_count++;
                        }

                    }

                }

                //calculate FSR
                double[] fsrCalculated_value = new double[]
                 {
                    soil_management / field_count,
                    water_management / field_count,
                    nutrient_retention / field_count,
                    surface_water / field_count,
                    fertilizer_management / field_count,
                    allover / field_count,
                 };
                for (int i = 0; i < fsrCalculated_value.Length; i++)
                {
                    if (Double.IsNaN(fsrCalculated_value[i]))
                    {
                        fsrCalculated_value[i] = 0.0;
                    }
                }
                ViewBag.FSR = fsrValue != null && totalarces != null ? Math.Round(fsrValue, 2) / totalarces : 0;
                List<string> grades = new List<string>();
                foreach (double averageRow in fsrCalculated_value)
                {
                    decimal inputValue = decimal.Round((decimal)averageRow, 1);

                    string grade = string.Empty;

                    if (inputValue == 4.0m)
                        grade = "A+";
                    else if (inputValue > 3.7m)
                        grade = "A";
                    else if (inputValue > 3.3m)
                        grade = "A-";
                    else if (inputValue > 3.0m)
                        grade = "B+";
                    else if (inputValue > 2.7m)
                        grade = "B";
                    else if (inputValue > 2.3m)
                        grade = "B-";
                    else if (inputValue > 2.0m)
                        grade = "C+";
                    else if (inputValue > 1.7m)
                        grade = "C";
                    else if (inputValue > 1.3m)
                        grade = "C-";
                    else if (inputValue > 1.0m)
                        grade = "D+";
                    else if (inputValue > 0.7m)
                        grade = "D";
                    else if (inputValue >= 0.0m)
                        grade = "D-";
                    else
                        grade = "-";

                    grades.Add(grade);
                }

                ViewBag.Grades = grades;

                return View();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);


            }
            return View();
        }

        //[HttpGet("{type}")]
        //[Route("{controller}/{action}")]
        public IActionResult GetAdvisorMessageForm()
        {
            try
            {
                return PartialView("~/Views/Producer/_AdvisorMessageForm.cshtml", new Email());
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return new JsonResult(new object());
            }
        }

        [HttpPost]
        public IActionResult SendEmailToAdvisor(Email email)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                var emailTo = HttpContext.Session.GetString("AdvisorEmail");
                if (email != null)
                {
                    bool result = _masterRepository.SendMail(emailTo, email.Subject, email.Body, emailTo);
                    if (result)
                    {
                        TempData["Success"] = "Email Sent Successfully.";
                        return Json(new { success = true, responseText = "Email Sent Successfully." });
                    }
                    else
                    {
                        TempData["Error"] = "An error has occured while sending an email.";
                        return Json(new { success = true, responseText = "An error has occured while sending an email." });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = true, responseText = "An error has occured while sending an email." });
        }
    }
}