using DocumentFormat.OpenXml.Spreadsheet;
using InternationalWaterWebApp.Library.Common;
using InternationalWaterWebApp.Library.DatabaseConnection;
using InternationalWaterWebApp.Library.ViewModel;
using InternationalWaterWebApp.Repository;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.ServiceModel;
using System.ServiceModel;
using ServiceReference4;
using Microsoft.AspNetCore.Mvc;
using Nancy;
using Microsoft.AspNetCore.Http;
using System.Numerics;
using NuGet.Packaging;
using Newtonsoft.Json;
using System.Linq;

namespace InternationalWaterWebApp.Models
{
    public class CommonMethods
    {
        #region "User"

        public void CreateSession(User user, bool RememberMe, HttpContext context)
        {
            try
            {
                if (RememberMe)
                {
                    context.Response.Cookies.Append("id", Convert.ToString(user.Id));
                    context.Response.Cookies.Append("firstname", user.FirstName);
                    context.Response.Cookies.Append("email", user.Email);
                    context.Response.Cookies.Append("username", user.UserName);
                    context.Response.Cookies.Append("password", user.Password);
                    context.Response.Cookies.Append("userrole", Convert.ToString(user.UserRole));
                }
                context.Response.Cookies.Append("rememberme", Convert.ToString(RememberMe));

                context.Session.SetString("Id", Convert.ToString(user.Id));
                context.Session.SetString("FirstName", user.FirstName);
                context.Session.SetString("Email", user.Email);
                context.Session.SetString("UserName", user.UserName);
                context.Session.SetString("UserRole", Convert.ToString(user.UserRole));
                context.Session.SetString("AdvisorId", Convert.ToString(user.AdvisorId));


            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
        }

        public string GetRefererUrl(HttpRequest request)
        {
            if (!string.IsNullOrEmpty(request.Headers["Referer"].ToString()))
            {
                return new Uri(request.Headers["Referer"].ToString()).LocalPath;
            }
            return "";
        }

        public bool ChangePassword(User user)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] userParams = { "_ActionType", "_id", "_password" };
                object[] userValues = { "ChangePassword", user.Id, user.Password };
                object[] userSqlDbTypes = { "nvarchar", "int", "nvarchar" };

                //long? userId = _masterRepository.InsertUpdate($"SELECT * from water_getallusers_ref (datarefcursor := 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => {userValues[1]},{userParams[2]} => '{userValues[2]}');");
                long? userId = _masterRepository.InsertUpdate($"SELECT * from \"Users\".water_getallusers_ref (datarefcursor := 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => {userValues[1]},{userParams[2]} => '{userValues[2]}');");

                return userId > 0;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return false;
            }
        }


        public List<State> GetStates(bool includeCode = false)
        {
            List<State> states = new List<State>();
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] parameters = { "@Actiontype" };
                object[] values = { "GetStates" };
                string columnName = includeCode ? "code" : "name";

                foreach (System.Data.DataRow mydataRow in _masterRepository.GetAndSelectTableItems($"select * \"FSRCalc\".from water_getstate_ref(datarefcursor => 'data');").Tables[0].Rows)
                {
                    states.Add(new State()
                    {
                        StateId = Convert.ToInt32(mydataRow["Id"]),
                        Name = mydataRow[columnName].ToString()
                    });
                }
                return states;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return states;
        }

        // calculate fsr data get filename
        public List<Precipitation> GetFileName(bool includeCode = false)
        {
            List<Precipitation> FilesNames = new List<Precipitation>();
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] parameters = { };
                object[] values = { };
                string columnName = includeCode ? "code" : "name";

                foreach (System.Data.DataRow mydataRow in _masterRepository.GetAndSelectTableItems($"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_ActionType =>'get_calc_fsr_data_dropdown_filename');").Tables[0].Rows)
                {
                    FilesNames.Add(new Precipitation()
                    {
                        FileName = mydataRow["filename"].ToString(),
                        //Year = Convert.ToInt32(mydataRow["year"]),
                        //Month = Convert.ToInt32(mydataRow["month"]),
                        //Day = Convert.ToInt32(mydataRow["day"]),
                    });
                }
                return FilesNames;
            }
            catch (Exception ex)
            {
                //Log.LogError(ex);
            }
            return FilesNames;
        }

        public List<Farm> GetFarmByPro(int ProducerId)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] Params = { "_id" };
                object[] Values = { ProducerId };

                DataSet dsFarmData = _masterRepository.GetAndSelectTableItems($"SELECT * from \"FSRCalc\".water_getfarmbypro_ref(datarefcursor => 'data',{Params[0]} => '{Values[0]}');");

                //DataSet dsFarmData = _masterRepository.GetAndSelectTableItems($"select farm_id , farm_name from farms where producer_id ={ProducerId}");
                return (from DataRow Row in dsFarmData.Tables[0].Rows
                        select new Farm
                        {
                            FarmId = Convert.ToInt32(Row["farm_id"]),
                            FarmName = Convert.ToString(Row["farm_name"]),

                        }).ToList();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return null;
            }
        }
        public int GetProducerByUserID(int UserId)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] Params = { "_ActionType", "_producer_id" };
                object[] Values = { "GetProducerByUserId", UserId };

                DataSet dsFarmData = _masterRepository.GetAndSelectTableItems($"SELECT * from \"FSRCalc\".water_producer_ref (datarefcursor => 'data',{Params[0]}=>'{Values[0]}', {Params[1]} =>{Values[1]})");

                int id = Convert.ToInt32(dsFarmData.Tables[0].Rows[0]["Producer_id"]);
                return id;
            }


            catch (Exception ex)
            {
                Log.LogError(ex);
                return 1;
            };
        }

        public List<Advisors> GetAdvisorDropDown()
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] Params = { "_ActionType" };
                object[] Values = { "getadvisors_dropdown" };

                DataSet dsAdvisorData = _masterRepository.GetAndSelectTableItems($"SELECT * from \"Users\".water_advisors_ref (datarefcursor => 'data', {Params[0]} => '{Values[0]}');");
                return (from DataRow Row in dsAdvisorData.Tables[0].Rows
                        select new Advisors
                        {
                            AdvisorsId = Convert.ToInt32(Row["advisor_id"]),
                            FirstName = Convert.ToString(Row["first_name"]),
                            LastName = Convert.ToString(Row["last_name"])

                        }).ToList();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return null;
            }
        }
        public static string GetEnumDescription(Enum value)
        {
            var descriptionAttribute = value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DescriptionAttribute>(); return descriptionAttribute?.Description ?? value.ToString();
        }

        private static readonly Dictionary<string, List<SelectListItem>> _stateOptionsCache = new Dictionary<string, List<SelectListItem>>();

        public static List<SelectListItem> GetStateOptions(bool includeCode = false)
        {
            var cacheKey = includeCode ? "StateOptionsWithCode" : "StateOptionsWithoutCode";
            if (_stateOptionsCache.TryGetValue(cacheKey, out List<SelectListItem> stateOptions))
            {
                return stateOptions;
            }

            stateOptions = CommonMethods.GetStateNames(includeCode);
            _stateOptionsCache.Add(cacheKey, stateOptions);
            return stateOptions;
        }

        public static List<SelectListItem> GetStateNames(bool includeCode = false)
        {
            List<SelectListItem> stateOptions = new List<SelectListItem>();
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] parameters = { "@Actiontype" };
                object[] values = { "GetStates" };
                string columnName = includeCode ? "code" : "name";

                foreach (System.Data.DataRow mydataRow in _masterRepository.GetAndSelectTableItems($"select * from \"FSRCalc\".water_getstate_ref(datarefcursor => 'data');").Tables[0].Rows)
                {
                    stateOptions.Add(new SelectListItem()
                    {
                        Text = mydataRow[columnName].ToString(),
                        Value = includeCode ? mydataRow["code"].ToString() : mydataRow[columnName].ToString()
                    });
                }
                return stateOptions;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return stateOptions;
        }

        #endregion

        #region "Chart"
        public DataSet FinancialDetailsChart(int fieldId)
        {
            DataSet result = null;
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] parameters = { "dataref", "_ActionType", "_id" };
                object[] values = { "dataref", "GetDataByFieldId", fieldId };
                result = _masterRepository.GetAndSelectTableItems($"select * from \"FSRCalc\".water_Financial_Chart_Box_calculation({parameters[0]} := '{values[0]}',{parameters[1]} := '{values[1]}', {parameters[2]} := {values[2]});");
                return result;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return result;
        }
        #endregion

        public List<Producers> GetProducerDropDown()
        {
            DataSet result = null;
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] userParams = { "_ActionType", "_producer_Id", "_advisor_id" };
                object[] userValues = { "GetAllProducersForDropdown" };  //AllProducers
                string query = $"SELECT * from \"Users\".water_producersdata_ref (datarefcursor => 'data',{userParams[0]} => '{userValues[0]}');";
                result = _masterRepository.GetAndSelectTableItems(query);

                return (from DataRow Row in result.Tables[0].Rows
                        select new Producers
                        {
                            ProducerId = Convert.ToInt32(Row["producer_id"]),
                            FirstName = Convert.ToString(Row["first_name"]),
                            LastName = Convert.ToString(Row["last_name"])

                        }).ToList();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return null;
            }
        }

        public DataSet GetFieldByproducerId(int producerId)
        {
            DataSet result = null;
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] userParams = { "_ActionType", "_producer_id" };
                object[] userValues = { "GetFieldByProducerIdDropdown", producerId };//GetFieldByProducerId
                string query = $"SELECT * from  \"FSRCalc\".water_producer_ref (datarefcursor => 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => {userValues[1]});";
                result = _masterRepository.GetAndSelectTableItems(query);
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return null;
            }
            return result;
        }

        public DataSet GetFieldByIdExportData(int fieldId)
        {
            DataSet result = null;
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] userParams = { "" };
                object[] userValues = { "" };
                string query = $"select * from \"FSRCalc\".water_get_fields_producer(datarefcursor => 'data',_actiontype => 'GetFieldsByProducerId',_field_id=>{fieldId});";
                result = _masterRepository.GetAndSelectTableItems(query);
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return null;
            }
            return result;
        }

        public Dictionary<string, string> four_R_Function(int id, int fr_id, int field_option_id = 1)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            string querytogetmgmt = $"SELECT * FROM \"FSRCalc\".water_4rn_management_zonesum(_field_id => {id},_fertilizer_id_ =>{fr_id},_field_option_id_=>{field_option_id});";
            DataSet mgmtdataDs = _masterRepository.GetAndSelectTableItemsWithoutCursor(querytogetmgmt);
            DataTable mgmtdataDT = mgmtdataDs.Tables.Count > 0 ? mgmtdataDs.Tables[0] : null;

            if (mgmtdataDT != null && mgmtdataDT.Rows.Count > 0)
            {
                string GetFactorLevel(double inputValue)
                {
                    string factorLevel;

                    switch (inputValue)
                    {
                        case double val when val <= 0.5:
                            factorLevel = "FEW BASIC FACTORS";
                            break;
                        case double val when val < 1:
                            factorLevel = "MOST OF BASIC FACTORS";
                            break;
                        case 1:
                            factorLevel = "BASIC";
                            break;
                        case double val when val < 2:
                            factorLevel = "BASIC PLUS";
                            break;
                        case double val when val < 3:
                            factorLevel = "INTERMEDIATE";
                            break;
                        case 3:
                            factorLevel = "ADVANCED";
                            break;
                        default:
                            factorLevel = "ERROR";
                            break;
                    }

                    return factorLevel;
                }

                Dictionary<string, string> factorLevels = new Dictionary<string, string>();
                factorLevels.Add("source", GetFactorLevel((mgmtdataDT.Rows[0]["source"] != DBNull.Value) ? Convert.ToDouble(mgmtdataDT.Rows[0]["source"].ToString()) : 0));
                factorLevels.Add("rate", GetFactorLevel(mgmtdataDT.Rows[0]["rate"] != DBNull.Value ? Convert.ToDouble(mgmtdataDT.Rows[0]["rate"].ToString()) : 0));
                factorLevels.Add("timing", GetFactorLevel(mgmtdataDT.Rows[0]["timing"] != DBNull.Value ? Convert.ToDouble(mgmtdataDT.Rows[0]["timing"]) : 0));
                factorLevels.Add("placement", GetFactorLevel(mgmtdataDT.Rows[0]["placement"] != DBNull.Value ? Convert.ToDouble(mgmtdataDT.Rows[0]["placement"]) : 0));

                //  ViewBag.FactorLevels = factorLevels;

                return factorLevels;

            }
            else
            {
                Dictionary<string, string> factorLevels = new Dictionary<string, string>();
                factorLevels.Add("source", "ERROR");
                factorLevels.Add("rate", "ERROR");
                factorLevels.Add("timing", "ERROR");
                factorLevels.Add("placement", "ERROR");
                return factorLevels;
            }
        }

        public string four_R_Functionlevel(int id, int fr_id, int field_option_id = 1)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            string querytogetmgmt = $"SELECT * FROM \"FSRCalc\".water_4rn_management_zonesum(_field_id => {id},_fertilizer_id_ =>{fr_id},_field_option_id_=>{field_option_id});";
            DataSet mgmtdataDs = _masterRepository.GetAndSelectTableItemsWithoutCursor(querytogetmgmt);
            DataTable mgmtdataDT = mgmtdataDs.Tables.Count > 0 ? mgmtdataDs.Tables[0] : null;

            if (mgmtdataDT != null && mgmtdataDT.Rows.Count > 0)
            {

                double avginput = (
                                    ((mgmtdataDT.Rows[0]["source"] != DBNull.Value) ? Convert.ToDouble(mgmtdataDT.Rows[0]["source"].ToString()) : 0) +
                                    ((mgmtdataDT.Rows[0]["rate"] != DBNull.Value) ? Convert.ToDouble(mgmtdataDT.Rows[0]["rate"]) : 0) +
                                    ((mgmtdataDT.Rows[0]["timing"] != DBNull.Value) ? Convert.ToDouble(mgmtdataDT.Rows[0]["timing"]) : 0) +
                                    ((mgmtdataDT.Rows[0]["placement"] != DBNull.Value) ? Convert.ToDouble(mgmtdataDT.Rows[0]["placement"]) : 0)
                                    ) / 4;

                string factorLevel;

                string GetFactorLevel(double inputValue)
                {
                    switch (inputValue)
                    {
                        case <= 5:
                            factorLevel = "Basic";
                            break;
                        case > 5 and <= 7.5:
                            factorLevel = "Intermediate";
                            break;
                        case > 7.5:
                            factorLevel = "Advanced";
                            break;
                        default:
                            factorLevel = "N/A";
                            break;
                    }

                    return factorLevel;
                }

                string factorLevels = GetFactorLevel(avginput);

                return factorLevels;

            }
            else
            {

                return "N/A";
            }
        }

        public Dictionary<string, string> Surface_chart_Function(int id, int startyear, int endyear, int field_option_id, string calctype)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            string querytogetmgmt = $"SELECT * FROM \"FSRCalc\".fsr_sedrisk_tprisk_chart(_field_id_ => {id},_field_option_id=>{field_option_id},_start_year =>{startyear},_end_year =>{endyear},calctype =>'{calctype}');";
            DataSet DsSurfaceWaterQuality_indexData = _masterRepository.GetAndSelectTableItemsWithoutCursor(querytogetmgmt);
            DataTable mgmtdataDT = DsSurfaceWaterQuality_indexData.Tables.Count > 0 ? DsSurfaceWaterQuality_indexData.Tables[0] : null;

            if (mgmtdataDT != null && mgmtdataDT.Rows.Count > 0)
            {
                DataRow row = mgmtdataDT.Rows[0];
                Dictionary<string, string> sed_goal_indexdata = new Dictionary<string, string>();
                sed_goal_indexdata.Add("field_goal", row["_sed_load_greater_field_goaltext"] != DBNull.Value ? row["_sed_load_greater_field_goaltext"].ToString() : "No");
                // sed_goal_indexdata.Add("field_goal", row["_sed_load_greater_field_goal"] != DBNull.Value ? row["_sed_load_greater_field_goal"].ToString() : "NO");
                sed_goal_indexdata.Add("amount_in_excess", row["_sed_perc_amount_in_excess"] != DBNull.Value ? Convert.ToDouble(row["_sed_perc_amount_in_excess"]).ToString("0.0") : "0");
                sed_goal_indexdata.Add("risk_level", row["_risk_level_sed"] != DBNull.Value ? row["_risk_level_sed"].ToString() : "");
                sed_goal_indexdata.Add("resource_index", row["_resource_sed_goal_index"] != DBNull.Value ? Convert.ToDouble(row["_resource_sed_goal_index"]).ToString("0.0") : "0");
                sed_goal_indexdata.Add("prob_practices", row["_sed_prob_practices"] != DBNull.Value ? row["_sed_prob_practices"].ToString() : "");
                sed_goal_indexdata.Add("goal_feasibility_index", row["_sed_goal_feasibility_index"] != DBNull.Value ? Convert.ToDouble(row["_sed_goal_feasibility_index"]).ToString("0.0") : "0");

                //  ViewBag.FactorLevels = factorLevels;

                return sed_goal_indexdata;

            }
            else
            {
                Dictionary<string, string> factorLevels = new Dictionary<string, string>();
                factorLevels.Add("field_goal", "ERROR");
                factorLevels.Add("amount_in_excess", "ERROR");
                factorLevels.Add("risk_level", "ERROR");
                factorLevels.Add("resource_index", "ERROR");
                factorLevels.Add("prob_practices", "ERROR");
                factorLevels.Add("goal_feasibility_index", "ERROR");
                return factorLevels;
            }
        }

        public Dictionary<string, string> four_R_chart_Function(int id, int startyear, int endyear, int field_option_id, int fertilizer_id, string type = "rate")
        {
            IMasterRepository _masterRepository = new MasterRepository();
            string querytogetmgmt = $"SELECT * FROM \"FSRCalc\".water_4rn_management_Chart(_field_id => {id},_fertilizer_id_ =>{fertilizer_id},_field_option_id_=>{field_option_id},_start_year_ =>{startyear},_end_year_ =>{endyear});";
            DataSet DsSurfaceWaterQuality_indexData = _masterRepository.GetAndSelectTableItemsWithoutCursor(querytogetmgmt);
            DataTable mgmtdataDT = DsSurfaceWaterQuality_indexData.Tables.Count > 0 ? DsSurfaceWaterQuality_indexData.Tables[0] : null;

            if (mgmtdataDT != null && mgmtdataDT.Rows.Count > 0)
            {
                DataRow row = mgmtdataDT.Rows[0];
                Dictionary<string, string> factorLevels = new Dictionary<string, string>();
                if (type == "rate")
                {
                    factorLevels.Add("source", row["rate_source"] != DBNull.Value ? row["rate_source"].ToString() : "");
                    factorLevels.Add("rate", row["rate_rate"] != DBNull.Value ? row["rate_rate"].ToString() : "");
                    factorLevels.Add("timing", row["rate_timing"] != DBNull.Value ? row["rate_timing"].ToString() : "");
                    factorLevels.Add("placement", row["rate_placement"] != DBNull.Value ? row["rate_placement"].ToString() : "");
                }
                else if (type == "value")
                {
                    factorLevels.Add("source", row["source"] != DBNull.Value ? row["source"].ToString() : "");
                    factorLevels.Add("rate", row["rate"] != DBNull.Value ? row["rate"].ToString() : "");
                    factorLevels.Add("timing", row["timing"] != DBNull.Value ? row["timing"].ToString() : "");
                    factorLevels.Add("placement", row["placement"] != DBNull.Value ? row["placement"].ToString() : "");
                }

                return factorLevels;
            }
            else
            {
                Dictionary<string, string> factorLevels = new Dictionary<string, string>();
                factorLevels.Add("source", "ERROR");
                factorLevels.Add("rate", "ERROR");
                factorLevels.Add("timing", "ERROR");
                factorLevels.Add("placement", "ERROR");
                return factorLevels;
            }
        }

        public DataSet CalculateFSRPrecipFileUpload_exists_or_not(string fileName)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            string Checkquery = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_ActionType =>'exists_or_not_calc_fsr_data_excel',_filename => '{fileName}');";
            return _masterRepository.GetAndSelectTableItems(Checkquery);
        }

        public double nFetinAndpnertin(string input_value)
        {
            switch (input_value)
            {
                case "FEW BASIC FACTORS":
                    return 0;
                case "MOST OF BASIC FACTORS":
                    return 2.5;
                case "BASIC":
                    return 5;
                case "BASIC PLUS":
                    return 6.25;
                case "INTERMEDIATE":
                    return 7.5;
                case "ADVANCED":
                    return 10;
                case "ERROR":
                    return 0;
                default:
                    return 0;
            }


        }

        //----rusle2 gte and update common code
        public async Task<string[]> CommonRusleValue(int fieldID, int mZoneID, int year, int field_option_id, string optionname)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            Dictionary<string, object> Rusle2Response = new Dictionary<string, object>();
            string[] resultvalue = new string[0];
            int RusleRetryCount = 3;
            bool failed = false;
            bool sucessresult = false;
            int AlfalfaRusleRetryCount = 3;
            bool Alfalfafailed = false;
            bool Alfalfasucessresult = false;
            int MoldRusleRetryCount = 3;
            bool Moldfailed = false;
            bool Moldsucessresult = false;
            int RuselResponseLength = 5;

            string query = $"select * from  \"FSRCalc\".water_getrusle2api_data(datarefcursor => 'data',_field_id => {fieldID} , _management_zones_id => {mZoneID}, _year => {year}, _field_option_id=> {field_option_id}, _actiontype=>'GetRusle2APIParm');";
            try
            {
                DataSet rusle2data = _masterRepository.GetAndSelectTableItems(query);
                if (rusle2data != null && rusle2data.Tables.Count != 0 && rusle2data.Tables[0].Rows.Count != 0)
                {
                    string CLIMATE_PTR = rusle2data.Tables[0].Rows[0]["climate_ptr"].ToString() ?? "";
                    string SOIL_PTR = rusle2data.Tables[0].Rows[0]["soil_ptr"].ToString() ?? "";
                    string MAN_BASE_PTR = rusle2data.Tables[0].Rows[0]["man_base_ptr"].ToString() ?? "";
                    string slope_percentage = rusle2data.Tables[0].Rows[0]["slope_percentage"].ToString() ?? "";
                    string stateId = rusle2data.Tables[0].Rows[0]["state"].ToString() ?? "";
                    string majcompname = rusle2data.Tables[0].Rows[0]["majcompname"].ToString() ?? "";
                    string MAN_Crop_Name = rusle2data.Tables[0].Rows[0]["MAN_Crop_Name"].ToString() ?? "";
                    string PrimaryTillageName = rusle2data.Tables[0].Rows[0]["PrimaryTillageName"].ToString() ?? "";
                    string SecondaryTillage = rusle2data.Tables[0].Rows[0]["SecondaryTillage"].ToString() ?? "";
                    //  EndpointAddress endpointAddress = new EndpointAddress("https://localhost:44334/Controller.asmx");
                    //  EndpointAddress endpointAddress = new EndpointAddress("http://rusle2apipublish.com:8058/Controller.asmx");
                    // EndpointAddress endpointAddress = new EndpointAddress("http://40.121.131.85/Controller.asmx");
                    // EndpointAddress endpointAddress = new EndpointAddress(DefaultValueFromWebConfig.Rusle2APIURL);
                    EndpointAddress endpointAddress = new EndpointAddress(DefaultValueFromWebConfig.Rusle2APIURL);

                    BasicHttpBinding basicHttpBinding = new BasicHttpBinding(endpointAddress.Uri.Scheme.ToLower() == "http" ?
                                    BasicHttpSecurityMode.None : BasicHttpSecurityMode.Transport);
                    basicHttpBinding.OpenTimeout = TimeSpan.MaxValue;
                    basicHttpBinding.CloseTimeout = TimeSpan.MaxValue;
                    basicHttpBinding.ReceiveTimeout = TimeSpan.MaxValue;
                    basicHttpBinding.SendTimeout = TimeSpan.MaxValue;
                    do
                    {

                        try
                        {
                            var client = await Task.Run(() => new ControllerSoapClient(basicHttpBinding, endpointAddress));
                            var result = await client.singleRunAsync(stateId, CLIMATE_PTR, SOIL_PTR, MAN_BASE_PTR, slope_percentage, majcompname, MAN_Crop_Name,
                               PrimaryTillageName, SecondaryTillage);


                            failed = false;
                            var response = result;
                            // var response = result.Body.singleRunResult;

                            if ((response != null) && (response.Length > RuselResponseLength))
                            {
                                string netCFactorValue = response.FirstOrDefault(line => line.StartsWith("NET_C_FACTOR"))?.Split(':', 2)?[1]?.Trim() ?? "";
                                string SLOPEDeliveryValue = response.FirstOrDefault(line => line.StartsWith("SLOPE_DELIVERY"))?.Split(':', 2)?[1]?.Trim() ?? "";
                                sucessresult = true;
                                if (optionname == "Alfalfa")
                                {
                                    if (Convert.ToDouble(netCFactorValue) > 0.996)
                                    {
                                        MAN_BASE_PTR = "managements\\CMZ 01\\a.Single Year/Single Crop Templates\\Corn grain";
                                        MAN_Crop_Name = "corn grain;";
                                        PrimaryTillageName = "NT,";
                                        SecondaryTillage = "";

                                        do
                                        {

                                            try
                                            {
                                                var Alfalfaresult = await client.singleRunAsync(stateId, CLIMATE_PTR, SOIL_PTR, MAN_BASE_PTR, slope_percentage, majcompname, MAN_Crop_Name,
                                                PrimaryTillageName, SecondaryTillage);
                                                Alfalfafailed = false;
                                                var Alfalfaresponse = Alfalfaresult;

                                                if (Alfalfaresponse != null)
                                                {
                                                    Alfalfasucessresult = true;
                                                    string AlfalfanetCFactorValue = Alfalfaresponse.FirstOrDefault(line => line.StartsWith("NET_C_FACTOR"))?.Split(':', 2)?[1]?.Trim() ?? "";
                                                    string AlfalfaSLOPEDeliveryValue = Alfalfaresponse.FirstOrDefault(line => line.StartsWith("SLOPE_DELIVERY"))?.Split(':', 2)?[1]?.Trim() ?? "";

                                                    netCFactorValue = AlfalfanetCFactorValue != null ? AlfalfanetCFactorValue : "0";
                                                    SLOPEDeliveryValue = AlfalfaSLOPEDeliveryValue != null ? AlfalfaSLOPEDeliveryValue : "0";
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Alfalfafailed = true;
                                                AlfalfaRusleRetryCount--;
                                                Log.LogError(ex);
                                                Alfalfasucessresult = false;
                                            }
                                        } while (Alfalfafailed && AlfalfaRusleRetryCount != 0);
                                    }
                                }

                                if (optionname == "MoldBoard")
                                {
                                    if (Convert.ToDouble(netCFactorValue) > 0.996)
                                    {
                                        MAN_BASE_PTR = "managements\\CMZ 01\\a.Single Year/Single Crop Templates\\Corn grain";
                                        MAN_Crop_Name = "corn grain;";
                                        PrimaryTillageName = "FP,";
                                        SecondaryTillage = "";
                                        do
                                        {
                                            try
                                            {
                                                var MoldBoardresult1 = await client.singleRunAsync(stateId, CLIMATE_PTR, SOIL_PTR, MAN_BASE_PTR, slope_percentage, majcompname, MAN_Crop_Name, PrimaryTillageName, SecondaryTillage);

                                                var MoldBoardresponse1 = MoldBoardresult1;

                                                string MoldBoardnetCFactorValue1 = MoldBoardresponse1.FirstOrDefault(line => line.StartsWith("NET_C_FACTOR"))?.Split(':', 2)?[1]?.Trim() ?? "";
                                                string MoldBoardSLOPEDeliveryValue1 = MoldBoardresponse1.FirstOrDefault(line => line.StartsWith("SLOPE_DELIVERY"))?.Split(':', 2)?[1]?.Trim() ?? "";

                                                string MAN_BASE_PTR1 = "managements\\CMZ 01\\a.Single Year/Single Crop Templates\\Soybeans before winter wheat\\narrow row";
                                                MAN_Crop_Name = "soybeans; nr,";
                                                PrimaryTillageName = "FP,";
                                                SecondaryTillage = "";
                                                var MoldBoardresult2 = await client.singleRunAsync(stateId, CLIMATE_PTR, SOIL_PTR, MAN_BASE_PTR1, slope_percentage, majcompname, MAN_Crop_Name, PrimaryTillageName, SecondaryTillage);

                                                var MoldBoardresponse2 = MoldBoardresult2;

                                                if (MoldBoardresponse2 != null)
                                                {
                                                    Moldfailed = false;
                                                    Moldsucessresult = false;
                                                    string MoldBoardnetCFactorValue2 = MoldBoardresponse2.FirstOrDefault(line => line.StartsWith("NET_C_FACTOR"))?.Split(':', 2)?[1]?.Trim() ?? "";
                                                    string MoldBoardSLOPEDeliveryValue2 = MoldBoardresponse2.FirstOrDefault(line => line.StartsWith("SLOPE_DELIVERY"))?.Split(':', 2)?[1]?.Trim() ?? "";

                                                    double MoldBoaraveragenetCFactor = ((string.IsNullOrEmpty(MoldBoardnetCFactorValue1) ? 0 : double.Parse(MoldBoardnetCFactorValue1)) +
                                                                                      (string.IsNullOrEmpty(MoldBoardnetCFactorValue2) ? 0 : double.Parse(MoldBoardnetCFactorValue2))) / 2.0;

                                                    double MoldBoaraverageSLOPEDelivery = ((string.IsNullOrEmpty(MoldBoardSLOPEDeliveryValue1) ? 0 : double.Parse(MoldBoardSLOPEDeliveryValue1)) +
                                                                                       (string.IsNullOrEmpty(MoldBoardSLOPEDeliveryValue2) ? 0 : double.Parse(MoldBoardSLOPEDeliveryValue2))) / 2.0;

                                                    //netCFactorValue = MoldBoaraveragenetCFactor.ToString();
                                                    //SLOPEDeliveryValue = MoldBoaraverageSLOPEDelivery.ToString();
                                                    netCFactorValue = MoldBoaraveragenetCFactor != null ? MoldBoaraveragenetCFactor.ToString() : "0";
                                                    SLOPEDeliveryValue = MoldBoaraverageSLOPEDelivery != null ? MoldBoaraverageSLOPEDelivery.ToString() : "0";
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Moldfailed = true;
                                                MoldRusleRetryCount--;
                                                Log.LogError(ex);
                                                Moldsucessresult = false;
                                            }
                                        } while (Moldfailed && MoldRusleRetryCount != 0);
                                    }
                                }

                                try
                                {
                                    if (Convert.ToDouble(netCFactorValue) != 0)
                                    {
                                        //if(Convert.ToDouble(netCFactorValue) > 0.4)
                                        //{
                                        string updateRusle2zoneyearlydata = $"select * from  \"FSRCalc\".water_resle2upadte_data(datarefcursor => 'data', _ActionType => 'updateRusle2zoneyearlydata' , _management_zones_id => {mZoneID}, _year => {year}, _total_sed_mass_leaving => {SLOPEDeliveryValue}, _net_c_factor => {netCFactorValue}, _field_option_id=>  {field_option_id});";
                                        DataSet updateRusle2zoneyearlydatadata = _masterRepository.GetAndSelectTableItems(updateRusle2zoneyearlydata);
                                        Console.WriteLine($"rusle2 upadte fieldID: {fieldID}, mZoneID: {mZoneID},field_option_id: {field_option_id},year: {year} , _total_sed_mass_leaving => {SLOPEDeliveryValue}, _net_c_factor => {netCFactorValue}");
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine($"CLIMATE_PTR =>{CLIMATE_PTR},SOIL_PTR=>{SOIL_PTR},MAN_BASE_PTR =>{MAN_BASE_PTR}");
                                        Console.ResetColor(); // Reset the console color to default

                                        Rusle2Response.Add("fieldID", fieldID);
                                        Rusle2Response.Add("mZoneID", mZoneID);
                                        Rusle2Response.Add("field_option_id", field_option_id);
                                        Rusle2Response.Add("year", year);
                                        Rusle2Response.Add("total_sed_mass_leaving", SLOPEDeliveryValue);
                                        Rusle2Response.Add("net_c_factor", netCFactorValue);
                                        //Console.WriteLine("Crow insert where field_id: " + fieldID + " ,management_zone_id is :" + mZoneID + "year" + year + " is updated");
                                        var combinedResponse = response.Concat(new[] { JsonConvert.SerializeObject(Rusle2Response) }).ToArray();
                                        return combinedResponse;
                                        //}
                                    }
                                    else
                                    {
                                        Console.WriteLine($"rusle2 upadte fieldID: {fieldID}, mZoneID: {mZoneID},field_option_id: {field_option_id},year: {year} , _total_sed_mass_leaving => {SLOPEDeliveryValue}, _net_c_factor => {netCFactorValue}");
                                        resultvalue = new string[] { "rusle2NetCFactorError" };
                                        return resultvalue;

                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Console.WriteLine("Crow insert where field_id: " + fieldID + " ,management_zone_id is :" + mZoneID + "year" + year + " is error");
                                    Log.LogError(ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            failed = true;
                            RusleRetryCount--;
                            Log.LogError(ex);
                            sucessresult = false;
                        }
                    } while (failed && RusleRetryCount != 0);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("water_getrusle2api_data : this function not return any record, CommonMethod:CommonRusleValue (so Rusle2 execution is stop)");
                    Console.ResetColor(); // Reset the console color to default

                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return resultvalue;
        }

        // FSRBenchmark common methods
        public async Task<Dictionary<string, string>> CalculateBenchmarkCommon(int field_Id, int FieldOptionId)
        {

            Dictionary<string, string> Response = new Dictionary<string, string>();
            List<Dictionary<string, object>> Rusle2ResponseList = new List<Dictionary<string, object>>();
            bool ruseleResponse = true;
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                object[] Params = { "_field_id", "_field_option_id" };
                object[] Values = { field_Id, 1 };
                string Rusle2Message = "Rusle2 not responding";
                //rusle 2 Api code 
                //benchmark functions call
                var querySteps = new List<string>
                    {
                        $"fsr_new_initializefieldvalues({Params[0]} => {Values[0]})", //1
                        $"fsr_new_calculate_daily_moisture_field({Params[0]} => {Values[0]})",//2
                        $"fsr_new_initializezonevalues({Params[0]} => {Values[0]})",//3
                        $"fsr_new_getpracticelookupsforzyd_update({Params[0]} => {Values[0]}, {Params[1]} => {Values[1]})",//4
                        $"fsr_new_create_new_rows_in_zone_yearly_data({Params[0]} => {Values[0]}, {Params[1]} => {Values[1]})",//5
                        $"fsr_new_benchmark_function_for_zyd_rows({Params[0]} => {Values[0]}, {Params[1]} => {Values[1]})",//7 // optional function no need to remove : add because curve_number update for option1
                        $"fsr_new_benchmark_function_for_zyd_rows({Params[0]} => {Values[0]}, {Params[1]} => 5)",//8
                        $"fsr_new_benchmark_function_for_zyd_new({Params[0]} => {Values[0]}, {Params[1]} => {Values[1]})",//6 //change position
                        $"fsr_new_benchmark_function_for_zyd_rows({Params[0]} => {Values[0]}, {Params[1]} => 6)",//9
                        $"fsr_new_benchmark_function_for_zyd_rows({Params[0]} => {Values[0]}, {Params[1]} => 7)",//10
                        //Note: added just zone_indices tabel in op1 and op2 data insert
                        $"fsr_new_calculate_hydrology_indices({Params[0]} => {Values[0]}, {Params[1]} => 3)",
                        $"fsr_new_calculate_hydrology_indices({Params[0]} => {Values[0]}, {Params[1]} => 4)",
                         //---
                        $"fsr_calculatemoisturerunoff_field_yearly_data_update({Params[0]} => {Values[0]}, {Params[1]} => 5)",//11
                        $"fsr_calculatemoisturerunoff_field_yearly_data_update({Params[0]} => {Values[0]}, {Params[1]} => 6)",//12
                        $"fsr_calculatemoisturerunoff_field_yearly_data_update({Params[0]} => {Values[0]}, {Params[1]} => 7)",//13
                        //Note: added just field_yearly_data tabel in op1 and op2 data insert
                        $"fsr_calculatemoisturerunoff_field_yearly_data_update({Params[0]} => {Values[0]}, {Params[1]} => 3)",//13
                        $"fsr_calculatemoisturerunoff_field_yearly_data_update({Params[0]} => {Values[0]}, {Params[1]} => 4)",//13


                    };
                int numbrOfFunc = 1;
                foreach (var queryStep in querySteps)
                {
                    if (numbrOfFunc == 6)
                    {
                        int Rusele2ResponseLength = 5;
                        #region rusle2 data upadte TOTAL_SED_MASS_LEAVING and  NET_C_FACTOR
                        string Fieldidquery = $"select * from  \"FSRCalc\".water_getrusle2api_data(datarefcursor => 'data',_field_id => {field_Id}, _actiontype=>'GetManagementZoneIdAndYear',_field_option_id=>5);";
                        DataSet dsFieldidquery = _masterRepository.GetAndSelectTableItems(Fieldidquery);
                        if (dsFieldidquery != null && dsFieldidquery.Tables.Count != 0 && dsFieldidquery.Tables[0].Rows.Count != 0)
                        {
                            for (int i = 0; i < dsFieldidquery.Tables[0].Rows.Count; i++)
                            {
                                Dictionary<string, object> Rusle2Response5 = new Dictionary<string, object>();
                                int mZoneID = Convert.ToInt32(dsFieldidquery.Tables[0].Rows[i]["management_zones_id"].ToString());
                                int year = Convert.ToInt32(dsFieldidquery.Tables[0].Rows[i]["year"].ToString());
                                var afRusleResults = await CommonRusleValue(field_Id, Convert.ToInt32(mZoneID), Convert.ToInt32(year), 5, "Alfalfa");
                                if ((afRusleResults.Length == 0) || (afRusleResults.Length < Rusele2ResponseLength))
                                {
                                    Rusle2Message = "Rusle2 not responding";
                                    if ((afRusleResults.Length != 0) && (afRusleResults[0] == "rusle2NetCFactorError"))
                                    {
                                        Rusle2Message = "Please run RUSLE 2 results prior to FSR Calculation";
                                    }
                                    ruseleResponse = false;
                                    Response.Add("message", Rusle2Message);
                                    Response.Add("success", "false");
                                    return Response;
                                }
                                else
                                {
                                    Rusle2Response5.Add("rusle2response", afRusleResults[11]);
                                    Rusle2ResponseList.Add(Rusle2Response5);
                                }
                            }
                        }
                        string mbpRusleResultsquery = $"select * from  \"FSRCalc\".water_getrusle2api_data(datarefcursor => 'data',_field_id => {field_Id}, _actiontype=>'GetManagementZoneIdAndYear',_field_option_id=>6);";
                        DataSet dsmbpRusleResultquery = _masterRepository.GetAndSelectTableItems(mbpRusleResultsquery);
                        if (dsmbpRusleResultquery != null && dsmbpRusleResultquery.Tables.Count != 0 && dsmbpRusleResultquery.Tables[0].Rows.Count != 0)
                        {
                            for (int i = 0; i < dsmbpRusleResultquery.Tables[0].Rows.Count; i++)
                            {
                                Dictionary<string, object> Rusle2Response6 = new Dictionary<string, object>();
                                int mbpmZoneID = Convert.ToInt32(dsmbpRusleResultquery.Tables[0].Rows[i]["management_zones_id"].ToString());
                                int mbpyear = Convert.ToInt32(dsmbpRusleResultquery.Tables[0].Rows[i]["year"].ToString());
                                var mbpRusleResults = await CommonRusleValue(field_Id, Convert.ToInt32(mbpmZoneID), Convert.ToInt32(mbpyear), 6, "MoldBoard");
                                if ((mbpRusleResults.Length == 0) || (mbpRusleResults.Length < Rusele2ResponseLength))
                                {
                                    Rusle2Message = "Rusle2 not responding";

                                    if ((mbpRusleResults.Length != 0) && (mbpRusleResults[0] == "rusle2NetCFactorError"))
                                    {
                                        Rusle2Message = "Please run RUSLE 2 results prior to FSR Calculation";
                                    }
                                    ruseleResponse = false;
                                    Response.Add("success", "false");
                                    Response.Add("message", Rusle2Message);
                                    return Response;
                                }
                                else
                                {
                                    Rusle2Response6.Add("rusle2response", mbpRusleResults[11]);
                                    Rusle2ResponseList.Add(Rusle2Response6);
                                }
                            }
                        }
                        string cpRusleResultsquery = $"select * from  \"FSRCalc\".water_getrusle2api_data(datarefcursor => 'data',_field_id => {field_Id}, _actiontype=>'GetManagementZoneIdAndYear',_field_option_id=>7);";
                        DataSet dscpRusleResultsquery = _masterRepository.GetAndSelectTableItems(cpRusleResultsquery);
                        if (dscpRusleResultsquery != null && dscpRusleResultsquery.Tables.Count != 0 && dscpRusleResultsquery.Tables[0].Rows.Count != 0)
                        {
                            for (int i = 0; i < dscpRusleResultsquery.Tables[0].Rows.Count; i++)
                            {
                                Dictionary<string, object> Rusle2Response7 = new Dictionary<string, object>();
                                int cpmZoneID = Convert.ToInt32(dscpRusleResultsquery.Tables[0].Rows[i]["management_zones_id"].ToString());
                                int cpyear = Convert.ToInt32(dscpRusleResultsquery.Tables[0].Rows[i]["year"].ToString());
                                var cpRusleResults = await CommonRusleValue(field_Id, Convert.ToInt32(cpmZoneID), Convert.ToInt32(cpyear), 7, "");
                                if ((cpRusleResults.Length == 0) || (cpRusleResults.Length < Rusele2ResponseLength))
                                {
                                    Rusle2Message = "Rusle2 not responding";
                                    if ((cpRusleResults.Length != 0) && (cpRusleResults[0] == "rusle2NetCFactorError"))
                                    {
                                        Rusle2Message = "Please run RUSLE 2 results prior to FSR Calculation";
                                    }
                                    ruseleResponse = false;
                                    Response.Add("success", "false");
                                    Response.Add("message", Rusle2Message);
                                    return Response;
                                }
                                else
                                {
                                    Rusle2Response7.Add("rusle2response", cpRusleResults[11]);
                                    Rusle2ResponseList.Add(Rusle2Response7);
                                }
                            }
                        }
                        #endregion

                    }

                    string query = $"SELECT * FROM \"FSRCalc\".{queryStep};";
                    var result = _masterRepository.GetAndSelectTableItemsWithoutCursor(query);
                    if (result == null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                    {
                        Response.Add("success", "false");
                        Response.Add("message", $"Benchmark Function {numbrOfFunc} error throw");
                        return Response;
                    }
                    numbrOfFunc++;
                }
                //Note IsPublish FSR, default set true when "bublish FSR" button implement remove tis code
                string queryIsPublishFSR = $"select * from \"FSRCalc\".water_fields_ref(datarefcursor => 'data',_field_id => {field_Id},_actiontype => 'isfsrpublishupdate',_isfsrpublish=>{true});";
                _masterRepository.GetAndSelectTableItems(queryIsPublishFSR);

                Response.Add("success", "true");
                Response.Add("message", "Calculate Benchmark Completed");
                Response.Add("rusle2response", JsonConvert.SerializeObject(Rusle2ResponseList));
                return Response;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            if (ruseleResponse)
            {
                Response.Add("success", "false");
                Response.Add("message", "Benchmark Function error throw");
                return Response;
            }
            else
            {
                Response.Add("success", "false");
                Response.Add("message", "Rusle2 not responding");
                return Response;
            }
        }

        public async Task<Dictionary<string, string>> CalculatePreviewFSRCommon(int field_Id, int field_Option_Id)
        {
            Dictionary<string, string> Response = new Dictionary<string, string>();
            List<Dictionary<string, object>> Rusle2ResponseList = new List<Dictionary<string, object>>();
            bool ruseleResponse = true;
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                string[] queries = new string[] { };
                if (field_Option_Id == 3 || field_Option_Id == 4)
                {
                    queries = new string[]{
                                    $"select * from \"FSRCalc\".fsr_new_getpracticelookupsforzyd_update(_field_id =>{field_Id}, _field_option_id =>{field_Option_Id})", //0
                                    $"SELECT * FROM \"FSRCalc\".fsr_new_benchmark_function_for_zyd_rows(_field_id => {field_Id}, _field_option_id => {field_Option_Id})",//9
                                    
                        $"SELECT * FROM \"FSRCalc\".fsr_new_moisture_daily_zone_update(_field_id => {field_Id}, _field_option_id => {field_Option_Id});",//1
                                    // rusle2 call //2
                                    $"SELECT * FROM \"FSRCalc\".fsr_calculatesoilloss_update_zyd_data(_field_id => {field_Id}, _field_option_id => {field_Option_Id});",//3
                                    $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_moisture_runoff_1_update(_field_id => {field_Id}, _field_option_id => {field_Option_Id});",//4
                                    $"SELECT * FROM \"FSRCalc\".fsr_calculatemoisturerunoff_field_yearly_data_update(_field_id => {field_Id}, _field_option_id => {field_Option_Id});",//5
                                    $"SELECT * FROM \"FSRCalc\".fsr_new_function_for_risk_module_main (_field_id=> {field_Id}, _field_option_id => {field_Option_Id}, _actiontype => 'FieldRiskSediment');",//6
                                    $"SELECT * FROM \"FSRCalc\".fsr_new_function_for_risk_module_main (_field_id=> {field_Id}, _field_option_id => {field_Option_Id}, _actiontype => 'FieldTPRisk');",//7
                                    $"SELECT * FROM \"FSRCalc\".fsr_calculatemoisturerunoff_field_yd_risk2column_update(_field_id => {field_Id}, _field_option_id => {field_Option_Id});",//8

                                    $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_field_yearly_update(_field_id => {field_Id}, _field_option_id => {field_Option_Id})",//10
                                    $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_fieldupdate(_field_id=> {field_Id}, _field_option_id =>1)"
                                    //---option3 missing data fill required function
                                    //$"SELECT * FROM \"FSRCalc\".fsr_new_moisture_daily_zone_update(_field_id => {field_Id}, _field_option_id => {field_Option_Id});",
                    };
                    //opton1 and option2 db funcion call
                }
                else
                {

                    string actionTypeFieldRiskSediment = "_actiontype => 'FieldRiskSediment'";
                    string actionTypeFieldTPRisk = "_actiontype => 'FieldTPRisk'";
                    object[] Params = { "_field_id", "_field_option_id" };
                    object[] Values = { field_Id, 1 };

                    queries = new string[]{
                        $"SELECT * FROM \"FSRCalc\".fsr_new_function_for_risk_module_main ({Params[0]} => {Values[0]}, {Params[1]} => 5, {actionTypeFieldRiskSediment});",
                        $"SELECT * FROM \"FSRCalc\".fsr_new_function_for_risk_module_main ({Params[0]} => {Values[0]}, {Params[1]} => 5, {actionTypeFieldTPRisk});",
                        $"SELECT * FROM \"FSRCalc\".fsr_new_function_for_risk_module_main ({Params[0]} => {Values[0]}, {Params[1]} => 6, {actionTypeFieldRiskSediment});",
                        $"SELECT * FROM \"FSRCalc\".fsr_new_function_for_risk_module_main ({Params[0]} => {Values[0]}, {Params[1]} => 6, {actionTypeFieldTPRisk});",
                        $"SELECT * FROM \"FSRCalc\".fsr_new_benchmark_function_for_zyd_rows ({Params[0]} => {Values[0]}, {Params[1]} => {Values[1]});",
                        $"SELECT * FROM \"FSRCalc\".fsr_new_function_for_risk_module_main ({Params[0]} => {Values[0]}, {Params[1]} => {Values[1]}, {actionTypeFieldRiskSediment});",
                        $"SELECT * FROM \"FSRCalc\".fsr_new_function_for_risk_module_main ({Params[0]} => {Values[0]}, {Params[1]} => {Values[1]}, {actionTypeFieldTPRisk});",
                        $"SELECT * FROM \"FSRCalc\".fsr_calculatemoisturerunoff_field_yearly_data_update ({Params[0]} => {Values[0]}, {Params[1]} => {Values[1]});",
                        //NOTE: Remove pseudocode in this risk fyd column update , pseudo code func: FSR_CalculateRisk_FieldAverages
                        
                        $"SELECT * FROM \"FSRCalc\".fsr_calculatemoisturerunoff_field_yd_risk2column_update ({Params[0]} => {Values[0]}, {Params[1]} => {Values[1]});", //1
                        $"SELECT * FROM \"FSRCalc\".fsr_calculatemoisturerunoff_field_yd_risk2column_update ({Params[0]} => {Values[0]}, {Params[1]} => 5);", //2
                        $"SELECT * FROM \"FSRCalc\".fsr_calculatemoisturerunoff_field_yd_risk2column_update ({Params[0]} => {Values[0]}, {Params[1]} => 6);", //3

                       $"SELECT * FROM \"FSRCalc\".water_Update_4rn_management(_actiontype =>'UpdateNFertLevel',_field_id => {field_Id},_fertilizer_id_ =>{1},_field_option_id_=>{1});",
                        $"SELECT * FROM \"FSRCalc\".water_Update_4rn_management(_actiontype =>'UpdatePFertLevel',_field_id => {field_Id},_fertilizer_id_ =>{2},_field_option_id_=>{1});",
                        $"SELECT * FROM \"FSRCalc\".water_Update_4rn_management(_actiontype =>'UpdateFieldRiskSediment',_field_id => {field_Id},_field_option_id_=>{1});",
                        $"SELECT * FROM \"FSRCalc\".water_Update_4rn_management(_actiontype =>'UpdateFieldTPRisk',_field_id => {field_Id},_field_option_id_=>{1});",
                        //added for update tp_leaving_peracre_untreated column (also added in bechmarks)
                        $"SELECT * FROM \"FSRCalc\".fsr_new_benchmark_zyd_tp_leaving_peracre_untreated_update({Params[0]} => {Values[0]}, {Params[1]} => {Values[1]})",//6 //change position
                        //benchmark TP option 6 value replce (above not calculated value re update)
                         $"SELECT * FROM \"FSRCalc\".fsr_new_benchmark_function_for_zyd_rows({Params[0]} => {Values[0]}, {Params[1]} => 6)",

                        $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_total_p_indices ({Params[0]} => {Values[0]}, {Params[1]} => 1)",
                        $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_total_p_indices ({Params[0]} => {Values[0]}, {Params[1]} => 5)",
                        $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_total_p_indices ({Params[0]} => {Values[0]}, {Params[1]} => 6)",
                        $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_total_p_indices ({Params[0]} => {Values[0]}, {Params[1]} => 7)",

                        $"SELECT * FROM \"FSRCalc\".fsr_calculatemoisturerunoff_field_yearly_data_update({Params[0]} => {Values[0]}, {Params[1]} => 6)", //12
                        
                        //field_perc_total_load_prp_p column update for field_option_id=6  -- diwali added not upadted column function
                        $"SELECT * FROM \"FSRCalc\".fsr_new_tprisk_module_set_2_latest_withrc ({Params[0]} => {Values[0]}, {Params[1]} => 6);", //1
                        $"SELECT * FROM \"FSRCalc\".fsr_new_tprisk_module_set_3_update_withrc ({Params[0]} => {Values[0]}, {Params[1]} => 6);", //2
                        $"SELECT * FROM \"FSRCalc\".fsr_calculatemoisturerunoff_field_yd_risk2column_update ({Params[0]} => {Values[0]}, {Params[1]} => 6);", //3
                        //
                        $"SELECT * FROM \"FSRCalc\".fsr_sedrisk_tprisk_feturecolumn_calculate (_field_id_ => {Values[0]},{Params[1]} => 6, calctype =>'tp');",

                        // $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_update({Params[0]} => {Values[0]}, {Params[1]} => 1)", //13
                         $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_field_yearly_update({Params[0]} => {Values[0]}, {Params[1]} => 1)", //13
                         $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_field_yearly_update({Params[0]} => {Values[0]}, {Params[1]} => 3)", //14
                         $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_field_yearly_update({Params[0]} => {Values[0]}, {Params[1]} => 4)", //15
                         $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_field_yearly_update({Params[0]} => {Values[0]}, {Params[1]} => 5)", //16
                         $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_field_yearly_update({Params[0]} => {Values[0]}, {Params[1]} => 6)", //17
                         $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_field_yearly_update({Params[0]} => {Values[0]}, {Params[1]} => 7)", //18
                        //$"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_update({Params[0]} => {Values[0]}, {Params[1]} => 3)", //14
                        //$"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_update({Params[0]} => {Values[0]}, {Params[1]} => 4)", //15
                        //$"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_update({Params[0]} => {Values[0]}, {Params[1]} => 5)", //16
                        //$"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_update({Params[0]} => {Values[0]}, {Params[1]} => 6)", //17
                        //$"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_update({Params[0]} => {Values[0]}, {Params[1]} => 7)", //18
                        $"SELECT * FROM \"FSRCalc\".fsr_new_calculate_fsr_fieldupdate({Params[0]} => {Values[0]}, {Params[1]} => 1)", //19                       
                    };
                }
                bool allQueriesSuccessful = true;
                int numbrOfFunc = 1;
                foreach (string query in queries)
                {
                    if (numbrOfFunc == 3 && (field_Option_Id == 3 || field_Option_Id == 4))
                    {
                        int Rusele2ResponseLength = 5;
                        //rusle2 call for Option
                        string Op1RusleResultsquery = $"select * from  \"FSRCalc\".water_getrusle2api_data(datarefcursor => 'data',_field_id => {field_Id}, _actiontype=>'GetManagementZoneIdAndYear',_field_option_id=>{field_Option_Id});";
                        DataSet dsOp1RusleResultsquery = _masterRepository.GetAndSelectTableItems(Op1RusleResultsquery);
                        if (dsOp1RusleResultsquery != null && dsOp1RusleResultsquery.Tables.Count != 0 && dsOp1RusleResultsquery.Tables[0].Rows.Count != 0)
                        {
                            for (int i = 0; i < dsOp1RusleResultsquery.Tables[0].Rows.Count; i++)
                            {
                                Dictionary<string, object> Rusle2Response = new Dictionary<string, object>();
                                int Op1mZoneID = Convert.ToInt32(dsOp1RusleResultsquery.Tables[0].Rows[i]["management_zones_id"].ToString());
                                int Op1year = Convert.ToInt32(dsOp1RusleResultsquery.Tables[0].Rows[i]["year"].ToString());
                                var Op1RusleResults = await CommonRusleValue(field_Id, Convert.ToInt32(Op1mZoneID), Convert.ToInt32(Op1year), field_Option_Id, "");

                                if ((Op1RusleResults.Length == 0) || (Op1RusleResults.Length < Rusele2ResponseLength))
                                {
                                    string Rusle2Message = "Rusle2 not responding";
                                    if ((Op1RusleResults.Length != 0) && (Op1RusleResults[0] == "rusle2NetCFactorError"))
                                    {
                                        Rusle2Message = "Please run RUSLE 2 results prior to FSR Calculation";
                                    }
                                    ruseleResponse = false;
                                    Response.Add("message", Rusle2Message);
                                    Response.Add("success", "false");
                                    return Response;
                                }
                                else
                                {
                                    Rusle2Response.Add("rusle2response", Op1RusleResults[11]);
                                    Rusle2ResponseList.Add(Rusle2Response);
                                }
                            }
                        }
                    }
                    var returnData = _masterRepository.GetAndSelectTableItemsWithoutCursor(query);
                    if (returnData.Tables[0].Rows[0][0] == null)
                    {
                        allQueriesSuccessful = false;
                        break;
                    }
                    numbrOfFunc++;
                }

                if (allQueriesSuccessful)
                {
                    Response.Add("success", "true");
                    Response.Add("message", "FSR calculation successfully completed");
                    Response.Add("rusle2response", JsonConvert.SerializeObject(Rusle2ResponseList));
                    return Response;
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            if(ruseleResponse)
            {
                Response.Add("success", "false");
                Response.Add("message", "Calculate FSR incomplete");
                return Response;
            }
            else
            {
                Response.Add("success", "false");
                Response.Add("message", "Rusle2 not responding");
                return Response;
            }

        }
    }

    public static class LinqContainLists // Or whatever
    {
        public static bool ContainsAllItems<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            return !b.Except(a).Any();
        }
    }
}

