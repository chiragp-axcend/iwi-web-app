using Microsoft.AspNetCore.Mvc;
using InternationalWaterWebApp.Library.Common;
using InternationalWaterWebApp.Library.ViewModel;
using InternationalWaterWebApp.Models;
using System.Data;
using InternationalWaterWebApp.Repository;
using InternationalWaterWebApp.Library.DatabaseConnection;
using System.Text;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using DocumentFormat.OpenXml;

namespace InternationalWaterWebApp.Controllers
{
    [AuthorizationCS((int)UserTypeEnum.Admin)]
    public class AdminController : BaseController
    {
        private CommonMethods commonMethods;
        private CommonController _CommonController;
        private EncryptDecryptData encryptDecryptData;

        public AdminController()
        {
            commonMethods = new CommonMethods();
            _CommonController = new CommonController();
            encryptDecryptData = new EncryptDecryptData();
        }

        public IActionResult Landing(string id)
        {
            IMasterRepository _masterRepository = new MasterRepository();

            DataSet DsFieldData = null;
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.fieldtab = 1;
                string producerId = encryptDecryptData.Decryptdata(id);

                object[] userParams1 = { "_ActionType", "_producer_id" };
                object[] userValues = { "GetFieldByProducerIdAdmin", producerId };
                string query = $"SELECT * from  \"FSRCalc\".water_admin_new_ref (datarefcursor => 'data',{userParams1[0]} => '{userValues[0]}',{userParams1[1]} => {userValues[1]});";
                DsFieldData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.Field = DsFieldData;
            }
            else
            {
                string query = $"select * from  \"FSRCalc\".water_admin_new_ref(datarefcursor => 'data',_ActionType => 'GetAllFieldsForAdmin');";
                DsFieldData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.Field = DsFieldData;
            }
            //get all producer
            string QueryProducer = $"SELECT * from \"FSRCalc\".water_admin_new_ref (datarefcursor => 'data',_ActionType => 'GetAllProducersForAdmin');";
            DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(QueryProducer);
            ViewBag.Producer = DsProducerData;

            return View();
        }

        public IActionResult MyFields(string id)
        {
            IMasterRepository _masterRepository = new MasterRepository();
            object[] userParams = { "_ActionType" };
            object[] userValues = { "GetAllFieldsForAdmin" };
            DataSet DsFieldData = null;

            if (id != null)
            {
                encryptDecryptData = new EncryptDecryptData();
                string producerId = encryptDecryptData.Decryptdata(id);

                object[] userParams1 = { "_ActionType", "_producer_id" };
                object[] userValues1 = { "GetFieldByProducerId", producerId };
                string query = $"SELECT * from  \"FSRCalc\".water_producer_ref (datarefcursor => 'data',{userParams1[0]} => '{userValues1[0]}',{userParams1[1]} => {userValues1[1]});";
                DsFieldData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.Field = DsFieldData;
            }
            else
            {
                string query = $"select * from  \"FSRCalc\".water_admin_new_ref(datarefcursor => 'data',{userParams[0]} => '{userValues[0]}');";
                DsFieldData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.Field = DsFieldData;
            }
            return View();
        }

        public IActionResult UploadRusle2Data()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadRusle2Data(IFormFile file)
        {
            try
            {
                if (file != null && (file.ContentType == "application/vnd.ms-excel" || file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                {
                    // Register the encoding provider for code page 1252
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    // Check if the file is an Excel file
                    if (Path.GetExtension(file.FileName) != ".xlsx" && Path.GetExtension(file.FileName) != ".xls")
                    {
                        return Json(new { success = false, message = "Please select a valid Excel file" });
                    }

                    // Read the Excel file into a DataSet
                    using (var stream = file.OpenReadStream())
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true // Use the first row as column names
                                }
                            });
                            IMasterRepository _masterRepository = new MasterRepository();
                            //Exce sheet-1 data update
                            var worksheet1 = result.Tables.Cast<DataTable>().FirstOrDefault(table => string.Equals(table.TableName, "zones", StringComparison.OrdinalIgnoreCase));


                            //autometicall addd update year data---

                            //var selectedColumns = worksheet1.AsEnumerable()
                            //    .Select(row => new
                            //    {
                            //        FieldID = Convert.ToDouble(row["FieldID"]),
                            //        M_Zone_ID = Convert.ToDouble(row["M_Zone_ID"]),
                            //        Year = Convert.ToDouble(row["Year"]),
                            //        Option_ID = Convert.ToDouble(row["Option_ID"]) ,
                            //    })
                            //    .ToList();

                            //var query1 = worksheet1.AsEnumerable()
                            //            .Where(row => row.Field<int>("FieldID") == 217);


                            //var col1 = selectedColumns[0].FieldID;
                            //var col2 = selectedColumns[0].M_Zone_ID;
                            //var col3 = selectedColumns[0].Year;
                            //var col4 = selectedColumns[0].Option_ID;

                            //-------

                            List<string> fieldIDs = new List<string>();
                            var data = new List<Rusle2ExcelData>();
                            foreach (DataRow row in worksheet1.Rows)
                            {
                                var FieldID = row.Table.Columns.Contains("FieldID") ? row["FieldID"].ToString() : string.Empty;
                                if (string.IsNullOrEmpty(FieldID) || !int.TryParse(FieldID, out int fieldIdInt))
                                {
                                    break;
                                }
                                //var ProducerID = row["ProducerID"].ToString();
                                var M_Zone_ID = row.Table.Columns.Contains("M_Zone_ID") ? row["M_Zone_ID"].ToString() : string.Empty;
                                // var Farmed_ExcelRowValue = row["Farmed"].ToString();
                                // var YieldClass = row["YieldClass"].ToString();
                                var Year = row.Table.Columns.Contains("Year") ? row["Year"].ToString() : string.Empty;

                                //NOTE: Add in dictionary
                                fieldIDs.Add("FieldID:" + FieldID + ";" + "M_Zone_ID:" + M_Zone_ID + ";" + "Year:" + Year);

                                var Option_ID = row.Table.Columns.Contains("Option_ID") ? row["Option_ID"].ToString() : string.Empty;
                                var State_Name = row.Table.Columns.Contains("State_Name") ? row["State_Name"].ToString() : string.Empty;

                                //used in  R2_manage
                                var CropType = row.Table.Columns.Contains("CropType1") ? row["CropType1"].ToString().Trim() : string.Empty;
                                var Tillage_ExcelRowValue = row.Table.Columns.Contains("Tillage") ? row["Tillage"].ToString().Trim() : string.Empty;
                                var Tillage = Tillage_ExcelRowValue.Split(',')[0].Trim(); //select first comma value

                                // var Acres = row["Acres"].ToString();
                                //var Contrib_ac = row["Contrib_ac"].ToString();
                                //var NonCont_ac = row["NonCont_ac"].ToString();
                                var CMZ = row.Table.Columns.Contains("CMZ") ? row["CMZ"].ToString() : string.Empty;
                                var MUSYM = row.Table.Columns.Contains("MUSYM") ? row["MUSYM"].ToString() : string.Empty;
                                var MUKEY = row.Table.Columns.Contains("MUKEY") ? row["MUKEY"].ToString() : string.Empty;
                                var Soil_MUNAME = row.Table.Columns.Contains("Soil_MUNAME") ? row["Soil_MUNAME"].ToString().Trim() : string.Empty;

                                //remove
                                //var Soil_MUPerc = row.Table.Columns.Contains("Soil_MUPerc") && double.TryParse(row["Soil_MUPerc"].ToString(), out var numericValue) ? numericValue.ToString() : "0";

                                var HydrologicSoilGroup = row.Table.Columns.Contains("HydrologicSoilGroup") ? row["HydrologicSoilGroup"].ToString().Trim() : string.Empty;
                                var County_Name = row.Table.Columns.Contains("County_Name") ? row["County_Name"].ToString().Trim() : string.Empty;
                                var Slope = row.Table.Columns.Contains("slope") && !string.IsNullOrEmpty(row["slope"].ToString()) ? row["slope"].ToString() : "0";
                                var SlopePercent = row.Table.Columns.Contains("SlopePercent") && !string.IsNullOrEmpty(row["SlopePercent"].ToString()) ? row["SlopePercent"].ToString() : "0";
                                var Field_Sediment_Delivery_Ratio = row.Table.Columns.Contains("Field_Sediment_Delivery_Ratio") && !string.IsNullOrEmpty(row["Field_Sediment_Delivery_Ratio"].ToString()) ? row["Field_Sediment_Delivery_Ratio"].ToString() : "0";
                                var Field_TP_Delivery_Ratio = row.Table.Columns.Contains("Field_TP_Delivery_Ratio") && !string.IsNullOrEmpty(row["Field_TP_Delivery_Ratio"].ToString()) ? row["Field_TP_Delivery_Ratio"].ToString() : "0";
                                //var TP_Mass_leaving_Landscape_in_lbs_yr = row["TP Mass leaving Landscape in lbs/yr"].ToString();
                                //remove
                                //var R2_Manage = CropType + "," + Tillage + "," + County_Name;
                                //new added column 
                                var MajCompName = row.Table.Columns.Contains("MajCompName") ? row["MajCompName"].ToString() : string.Empty;
                                var CompPerc = row.Table.Columns.Contains("CompPerc") && !string.IsNullOrEmpty(row["CompPerc"].ToString()) ? row["CompPerc"].ToString() : "0";
                                // var PerContribExcelRowValue = float.Parse(row["PerContrib"].ToString());
                                //PerContrib Condition
                                //string PerContrib = PerContribExcelRowValue == 1 ? "True" : "False";
                                //float acre = float.TryParse(Acres, out float acresValue) ? acresValue : 0;
                                // Assuming 'Acres' and 'PerContribExcelRowValue' are already declared and contain valid values
                                //float perContribValue = float.TryParse(Convert.ToString(PerContribExcelRowValue), out float parsedPerContribValue) ? parsedPerContribValue : 0;

                                // var Contrib_ac = acre * perContribValue;

                                // isexclusion zone condition
                                //var Farmed = (Farmed_ExcelRowValue.ToLower() == "true" || Farmed_ExcelRowValue.ToLower() == "yes") ? "1" : "0";

                                // upadte query
                                string query = $"select * from  \"FSRCalc\".water_resle2upadte_data(datarefcursor => 'data', _ActionType => 'updateRusle2fields',_management_zones_id => {M_Zone_ID},_field_id => {FieldID},_cmz => '{CMZ}',_mukey => '{MUKEY}',_soil_muname => '{Soil_MUNAME}',_soil_group => '{HydrologicSoilGroup}',_slope_percentage_ => {SlopePercent},_slope => {Slope},_field_sediment_delivery_ratio => {Field_Sediment_Delivery_Ratio},_field_tp_delivery_ratio => {Field_TP_Delivery_Ratio},_county => '{County_Name}',_year => {Year},_option_id => '{Option_ID}',_state_name => '{State_Name}',_majcompname =>'{MajCompName}',_compperc=>{CompPerc},_musym => '{MUSYM}');";
                                try
                                {
                                    DataSet rusle2data = _masterRepository.GetAndSelectTableItems(query);
                                    if (rusle2data != null && rusle2data.Tables.Count != 0 && int.Parse(rusle2data.Tables[0].Rows[0][0].ToString()) == 1)
                                    {
                                        Console.WriteLine("row insert where field_id: " + FieldID + " ,management_zone_id is :" + M_Zone_ID + " is updated");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.LogError(ex);
                                }
                            }
                            //Exce sheet-2 data update
                            var worksheet2 = result.Tables.Cast<DataTable>().FirstOrDefault(table => string.Equals(table.TableName, "watershed", StringComparison.OrdinalIgnoreCase));
                            var data2 = new List<Rusle2ExcelData>();
                            foreach (DataRow row in worksheet2.Rows)
                            {
                                var FieldID = row.Table.Columns.Contains("FieldID") ? row["FieldID"].ToString() : string.Empty;
                                if (string.IsNullOrEmpty(FieldID) || !int.TryParse(FieldID, out int fieldIdInt))
                                {
                                    break;
                                }
                                var Total_Watershed_Area_upstream_of_PRP_acres = row.Table.Columns.Contains("Total_Watershed_Area_upstream_of_PRP_acres") ? row["Total_Watershed_Area_upstream_of_PRP_acres"].ToString() : "0";
                                var Upstream_Watershed_Area_contributing_to_PRP_acres = row.Table.Columns.Contains("Upstream_Watershed_Area_contributing_to_PRP_acres") ? row["Upstream_Watershed_Area_contributing_to_PRP_acres"].ToString() : "0";
                                var Field_Area_PRP_Watershed_acres = row.Table.Columns.Contains("Field_Area_Located_Within_PRP_Watershed_acres") ? row["Field_Area_Located_Within_PRP_Watershed_acres"].ToString() : "0";
                                var Waterbody_Name = row.Table.Columns.Contains("Waterbody_Name") ? row["Waterbody_Name"].ToString() : "0";
                                var Stream_Reach = row.Table.Columns.Contains("Stream_Reach_or_Lake") ? row["Stream_Reach_or_Lake"].ToString() : "0";
                                var PRP_Sediment_Delivery_Ratio = row.Table.Columns.Contains("PRP_Sediment_Delivery_Ratio") ? row["PRP_Sediment_Delivery_Ratio"].ToString() : "0";
                                var PRP_TP_Delivery_Ratio = row.Table.Columns.Contains("PRP_TP_Delivery_Ratio") ? row["PRP_TP_Delivery_Ratio"].ToString() : "0";

                                var Sed_prp_nonpointSource_t = row.Table.Columns.Contains("PRP_Sed_NonpointSource_t") ? row["PRP_Sed_NonpointSource_t"].ToString() : "0";
                                var Tp_prp_nonpointSource_lb = row.Table.Columns.Contains("PRP_TP_NonpointSource_lbs") ? row["PRP_TP_NonpointSource_lbs"].ToString() : "0";

                                string percentWatershedLoadReductionCropla = row.Table.Columns.Contains("Percent_Watershed_Area_Required_to_Achieve_Load_Reduction_Cropla") ? row["Percent_Watershed_Area_Required_to_Achieve_Load_Reduction_Cropla"].ToString() : "0";
                                double loadReductionCropla = double.TryParse(percentWatershedLoadReductionCropla, out double parsedValue) ? parsedValue : 100;

                                var Percentage_cropland_contributing_upstream_of_PRP = row.Table.Columns.Contains("Percentage_cropland_contributing_upstream_of_PRP") ? row["Percentage_cropland_contributing_upstream_of_PRP"].ToString() : "0";
                                if (Percentage_cropland_contributing_upstream_of_PRP == string.Empty) { Percentage_cropland_contributing_upstream_of_PRP = "100"; };
                                // upadte query
                                string query = $"select * from \"FSRCalc\".water_resle2upadte_data (datarefcursor => 'data',_ActionType=>'updateRusle2fieldsExcelSheet2',_total_area_upstream_prp => '{Total_Watershed_Area_upstream_of_PRP_acres}',_upstream_drainage_prp_contr =>'{Upstream_Watershed_Area_contributing_to_PRP_acres}',_field_area_upstream_prp => '{Field_Area_PRP_Watershed_acres}',_watershed_name => '{Waterbody_Name}',_stream_reach =>'{Stream_Reach}',_sed_prp_del_ratio =>{PRP_Sediment_Delivery_Ratio},_tp_prp_del_ratio => {PRP_TP_Delivery_Ratio},_sed_prp_nonpointSource_t => {Sed_prp_nonpointSource_t},_tp_prp_nonpointSource_lb => {Tp_prp_nonpointSource_lb},_perc_contr_upstream_prp =>{loadReductionCropla},_perc_contr_upstream_prp2 => {Percentage_cropland_contributing_upstream_of_PRP},_field_id => {FieldID});";
                                try
                                {
                                    DataSet rusle2data = _masterRepository.GetAndSelectTableItems(query);
                                    if (rusle2data != null && rusle2data.Tables.Count != 0 && int.Parse(rusle2data.Tables[0].Rows[0][0].ToString()) == 1)
                                    {
                                        Console.WriteLine("Sheet 2 in field_id: " + FieldID + " row is updated");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.LogError(ex);
                                }
                            }


                            var worksheet3 = result.Tables.Cast<DataTable>().FirstOrDefault(table => string.Equals(table.TableName, "prp", StringComparison.OrdinalIgnoreCase));
                            var data3 = new List<Rusle2ExcelData>();

                            if (worksheet3 != null)
                            {
                                foreach (DataRow row in worksheet3.Rows)
                                {
                                    var FieldID = row.Table.Columns.Contains("FieldID") ? row["FieldID"].ToString() : string.Empty;
                                    if (string.IsNullOrEmpty(FieldID) || !int.TryParse(FieldID, out int fieldIdInt))
                                    {
                                        break;
                                    }

                                    var POINT_X = row.Table.Columns.Contains("POINT_X") ? row["POINT_X"].ToString() : "0";
                                    var POINT_Y = row.Table.Columns.Contains("POINT_Y") ? row["POINT_Y"].ToString() : "0";
                                    //string combinedPoint = (POINT_X != "0" ? POINT_X : "") + (POINT_X != "0" && POINT_Y != "0" ? "," : "") + (POINT_Y != "0" ? POINT_Y : "");
                                    string combinedPoint = POINT_X + "," + POINT_Y;
                                    string query = $"select * from \"FSRCalc\".water_resle2upadte_data (datarefcursor => 'data',_ActionType=>'updateRusle2Fielddata',_Watershed_Coords => '{combinedPoint}',_field_id => {FieldID});";
                                    try
                                    {
                                        DataSet rusle2data = _masterRepository.GetAndSelectTableItems(query);
                                        if (rusle2data != null && rusle2data.Tables.Count != 0 && int.Parse(rusle2data.Tables[0].Rows[0][0].ToString()) == 1)
                                        {
                                            Console.WriteLine("Sheet 3 in field_id: " + FieldID + " row is updated");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.LogError(ex);
                                    }
                                }
                            }

                            return Json(new { success = true, message = "The update process has been completed successfully.", data, fieldIDs });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Json(new { success = false, message = ex.Message.ToString() });
            }

            return Json(new { success = false, message = "Please select a valid Excel file" });
        }

        [HttpPost]
        public IActionResult UploadBulkRusle2Data(IFormFile file)
        {
            try
            {
                if (file != null && (file.ContentType == "application/vnd.ms-excel" || file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                {
                    // Register the encoding provider for code page 1252
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    // Check if the file is an Excel file
                    if (Path.GetExtension(file.FileName) != ".xlsx" && Path.GetExtension(file.FileName) != ".xls")
                    {
                        return Json(new { success = false, message = "Please select a valid Excel file" });
                    }

                    // Read the Excel file into a DataSet
                    using (var stream = file.OpenReadStream())
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true // Use the first row as column names
                                }
                            });
                            IMasterRepository _masterRepository = new MasterRepository();
                            //Exce sheet-1 data update
                            var worksheet1 = result.Tables.Cast<DataTable>().FirstOrDefault(table => string.Equals(table.TableName, "zones", StringComparison.OrdinalIgnoreCase));


                            //autometicall addd update year data---

                            //var selectedColumns = worksheet1.AsEnumerable()
                            //    .Select(row => new
                            //    {
                            //        FieldID = Convert.ToDouble(row["FieldID"]),
                            //        M_Zone_ID = Convert.ToDouble(row["M_Zone_ID"]),
                            //        Year = Convert.ToDouble(row["Year"]),
                            //        Option_ID = Convert.ToDouble(row["Option_ID"]) ,
                            //    })
                            //    .ToList();

                            //var query1 = worksheet1.AsEnumerable()
                            //            .Where(row => row.Field<int>("FieldID") == 217);


                            //var col1 = selectedColumns[0].FieldID;
                            //var col2 = selectedColumns[0].M_Zone_ID;
                            //var col3 = selectedColumns[0].Year;
                            //var col4 = selectedColumns[0].Option_ID;

                            //-------

                            List<string> fieldIDs = new List<string>();
                            var data = new List<Rusle2ExcelData>();
                            foreach (DataRow row in worksheet1.Rows)
                            {
                                var FieldID = row.Table.Columns.Contains("FieldID") ? row["FieldID"].ToString() : string.Empty;
                                if (string.IsNullOrEmpty(FieldID) || !int.TryParse(FieldID, out int fieldIdInt))
                                {
                                    break;
                                }
                                //var ProducerID = row["ProducerID"].ToString();
                                var M_Zone_ID = row.Table.Columns.Contains("M_Zone_ID") ? row["M_Zone_ID"].ToString() : string.Empty;
                                // var Farmed_ExcelRowValue = row["Farmed"].ToString();
                                // var YieldClass = row["YieldClass"].ToString();
                                var Year = row.Table.Columns.Contains("Year") ? row["Year"].ToString() : string.Empty;

                                //NOTE: Add in dictionary
                                fieldIDs.Add("FieldID:" + FieldID + ";" + "M_Zone_ID:" + M_Zone_ID + ";" + "Year:" + Year);

                                var Option_ID = row.Table.Columns.Contains("Option_ID") ? row["Option_ID"].ToString() : string.Empty;
                                var State_Name = row.Table.Columns.Contains("State_Name") ? row["State_Name"].ToString() : string.Empty;

                                //used in  R2_manage
                                var CropType = row.Table.Columns.Contains("CropType1") ? row["CropType1"].ToString().Trim() : string.Empty;
                                var Tillage_ExcelRowValue = row.Table.Columns.Contains("Tillage") ? row["Tillage"].ToString().Trim() : string.Empty;
                                var Tillage = Tillage_ExcelRowValue.Split(',')[0].Trim(); //select first comma value

                                // var Acres = row["Acres"].ToString();
                                //var Contrib_ac = row["Contrib_ac"].ToString();
                                //var NonCont_ac = row["NonCont_ac"].ToString();
                                var CMZ = row.Table.Columns.Contains("CMZ") ? row["CMZ"].ToString() : string.Empty;
                                var MUSYM = row.Table.Columns.Contains("MUSYM") ? row["MUSYM"].ToString() : string.Empty;
                                var MUKEY = row.Table.Columns.Contains("MUKEY") ? row["MUKEY"].ToString() : string.Empty;
                                var Soil_MUNAME = row.Table.Columns.Contains("Soil_MUNAME") ? row["Soil_MUNAME"].ToString().Trim() : string.Empty;

                                //remove
                                //var Soil_MUPerc = row.Table.Columns.Contains("Soil_MUPerc") && double.TryParse(row["Soil_MUPerc"].ToString(), out var numericValue) ? numericValue.ToString() : "0";

                                var HydrologicSoilGroup = row.Table.Columns.Contains("HydrologicSoilGroup") ? row["HydrologicSoilGroup"].ToString().Trim() : string.Empty;
                                var County_Name = row.Table.Columns.Contains("County_Name") ? row["County_Name"].ToString().Trim() : string.Empty;
                                var Slope = row.Table.Columns.Contains("slope") && !string.IsNullOrEmpty(row["slope"].ToString()) ? row["slope"].ToString() : "0";
                                var SlopePercent = row.Table.Columns.Contains("SlopePercent") && !string.IsNullOrEmpty(row["SlopePercent"].ToString()) ? row["SlopePercent"].ToString() : "0";
                                var Field_Sediment_Delivery_Ratio = row.Table.Columns.Contains("Field_Sediment_Delivery_Ratio") && !string.IsNullOrEmpty(row["Field_Sediment_Delivery_Ratio"].ToString()) ? row["Field_Sediment_Delivery_Ratio"].ToString() : "0";
                                var Field_TP_Delivery_Ratio = row.Table.Columns.Contains("Field_TP_Delivery_Ratio") && !string.IsNullOrEmpty(row["Field_TP_Delivery_Ratio"].ToString()) ? row["Field_TP_Delivery_Ratio"].ToString() : "0";

                                //var TP_Mass_leaving_Landscape_in_lbs_yr = row["TP Mass leaving Landscape in lbs/yr"].ToString();

                                //remove
                                //var R2_Manage = CropType + "," + Tillage + "," + County_Name;

                                //new added column 
                                var MajCompName = row.Table.Columns.Contains("MajCompName") ? row["MajCompName"].ToString() : string.Empty;
                                var CompPerc = row.Table.Columns.Contains("CompPerc") && !string.IsNullOrEmpty(row["CompPerc"].ToString()) ? row["CompPerc"].ToString() : "0";

                                // var PerContribExcelRowValue = float.Parse(row["PerContrib"].ToString());
                                //PerContrib Condition
                                //string PerContrib = PerContribExcelRowValue == 1 ? "True" : "False";
                                //float acre = float.TryParse(Acres, out float acresValue) ? acresValue : 0;
                                // Assuming 'Acres' and 'PerContribExcelRowValue' are already declared and contain valid values
                                //float perContribValue = float.TryParse(Convert.ToString(PerContribExcelRowValue), out float parsedPerContribValue) ? parsedPerContribValue : 0;

                                // var Contrib_ac = acre * perContribValue;

                                // isexclusion zone condition
                                //var Farmed = (Farmed_ExcelRowValue.ToLower() == "true" || Farmed_ExcelRowValue.ToLower() == "yes") ? "1" : "0";

                                // upadte query
                                string query = $"select * from  \"FSRCalc\".water_resle2upadte_data(datarefcursor => 'data', _ActionType => 'updateRusle2fields',_management_zones_id => {M_Zone_ID},_field_id => {FieldID},_cmz => '{CMZ}',_mukey => '{MUKEY}',_soil_muname => '{Soil_MUNAME}',_soil_group => '{HydrologicSoilGroup}',_slope_percentage_ => {SlopePercent},_slope => {Slope},_field_sediment_delivery_ratio => {Field_Sediment_Delivery_Ratio},_field_tp_delivery_ratio => {Field_TP_Delivery_Ratio},_county => '{County_Name}',_year => {Year},_option_id => '{Option_ID}',_state_name => '{State_Name}',_majcompname =>'{MajCompName}',_compperc=>{CompPerc},_musym => '{MUSYM}');";
                                try
                                {
                                    DataSet rusle2data = _masterRepository.GetAndSelectTableItems(query);
                                    if (rusle2data != null && rusle2data.Tables.Count != 0 && int.Parse(rusle2data.Tables[0].Rows[0][0].ToString()) == 1)
                                    {
                                        string bulkquery = $"select * from  \"FSRCalc\".water_resle2upadte_data(datarefcursor => 'data', _ActionType => 'updateBulkRusle2fields',_management_zones_id => {M_Zone_ID},_field_id => {FieldID},_cmz => '{CMZ}',_mukey => '{MUKEY}',_soil_muname => '{Soil_MUNAME}',_soil_group => '{HydrologicSoilGroup}',_slope_percentage_ => {SlopePercent},_slope => {Slope},_field_sediment_delivery_ratio => {Field_Sediment_Delivery_Ratio},_field_tp_delivery_ratio => {Field_TP_Delivery_Ratio},_county => '{County_Name}',_year => {Year},_option_id => '{Option_ID}',_state_name => '{State_Name}',_majcompname =>'{MajCompName}',_compperc=>{CompPerc},_musym => '{MUSYM}');";
                                        try
                                        {
                                            DataSet bulkrusle2data = _masterRepository.GetAndSelectTableItems(bulkquery);
                                            if (bulkrusle2data != null && bulkrusle2data.Tables.Count != 0 && bulkrusle2data.Tables[0].Rows.Count != 0)
                                            {
                                                for (int i = 0; i < bulkrusle2data.Tables[0].Rows.Count; i++)
                                                {
                                                    fieldIDs.Add("FieldID:" + FieldID + ";" + "M_Zone_ID:" + M_Zone_ID + ";" + "Year:" + bulkrusle2data.Tables[0].Rows[i]["year"]);
                                                }
                                                Console.WriteLine("row insert where field_id: " + FieldID + " ,management_zone_id is :" + M_Zone_ID + " is updated");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.LogError(ex);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.LogError(ex);
                                }
                            }
                            //Exce sheet-2 data update
                            var worksheet2 = result.Tables.Cast<DataTable>().FirstOrDefault(table => string.Equals(table.TableName, "watershed", StringComparison.OrdinalIgnoreCase));
                            var data2 = new List<Rusle2ExcelData>();
                            foreach (DataRow row in worksheet2.Rows)
                            {
                                var FieldID = row.Table.Columns.Contains("FieldID") ? row["FieldID"].ToString() : string.Empty;
                                if (string.IsNullOrEmpty(FieldID) || !int.TryParse(FieldID, out int fieldIdInt))
                                {
                                    break;
                                }
                                var Total_Watershed_Area_upstream_of_PRP_acres = row.Table.Columns.Contains("Total_Watershed_Area_upstream_of_PRP_acres") ? row["Total_Watershed_Area_upstream_of_PRP_acres"].ToString() : "0";
                                var Upstream_Watershed_Area_contributing_to_PRP_acres = row.Table.Columns.Contains("Upstream_Watershed_Area_contributing_to_PRP_acres") ? row["Upstream_Watershed_Area_contributing_to_PRP_acres"].ToString() : "0";
                                var Field_Area_PRP_Watershed_acres = row.Table.Columns.Contains("Field_Area_Located_Within_PRP_Watershed_acres") ? row["Field_Area_Located_Within_PRP_Watershed_acres"].ToString() : "0";
                                var Waterbody_Name = row.Table.Columns.Contains("Waterbody_Name") ? row["Waterbody_Name"].ToString() : "0";
                                var Stream_Reach = row.Table.Columns.Contains("Stream_Reach_or_Lake") ? row["Stream_Reach_or_Lake"].ToString() : "0";
                                var PRP_Sediment_Delivery_Ratio = row.Table.Columns.Contains("PRP_Sediment_Delivery_Ratio") ? row["PRP_Sediment_Delivery_Ratio"].ToString() : "0";
                                var PRP_TP_Delivery_Ratio = row.Table.Columns.Contains("PRP_TP_Delivery_Ratio") ? row["PRP_TP_Delivery_Ratio"].ToString() : "0";

                                var Sed_prp_nonpointSource_t = row.Table.Columns.Contains("PRP_Sed_NonpointSource_t") ? row["PRP_Sed_NonpointSource_t"].ToString() : "0";
                                var Tp_prp_nonpointSource_lb = row.Table.Columns.Contains("PRP_TP_NonpointSource_lbs") ? row["PRP_TP_NonpointSource_lbs"].ToString() : "0";

                                string percentWatershedLoadReductionCropla = row.Table.Columns.Contains("Percent_Watershed_Area_Required_to_Achieve_Load_Reduction_Cropla") ? row["Percent_Watershed_Area_Required_to_Achieve_Load_Reduction_Cropla"].ToString() : "0";
                                double loadReductionCropla = double.TryParse(percentWatershedLoadReductionCropla, out double parsedValue) ? parsedValue : 100;

                                var Percentage_cropland_contributing_upstream_of_PRP = row.Table.Columns.Contains("Percentage_cropland_contributing_upstream_of_PRP") ? row["Percentage_cropland_contributing_upstream_of_PRP"].ToString() : "0";
                                if (Percentage_cropland_contributing_upstream_of_PRP == string.Empty) { Percentage_cropland_contributing_upstream_of_PRP = "100"; };
                                // upadte query
                                string query = $"select * from \"FSRCalc\".water_resle2upadte_data (datarefcursor => 'data',_ActionType=>'updateRusle2fieldsExcelSheet2',_total_area_upstream_prp => '{Total_Watershed_Area_upstream_of_PRP_acres}',_upstream_drainage_prp_contr =>'{Upstream_Watershed_Area_contributing_to_PRP_acres}',_field_area_upstream_prp => '{Field_Area_PRP_Watershed_acres}',_watershed_name => '{Waterbody_Name}',_stream_reach =>'{Stream_Reach}',_sed_prp_del_ratio =>{PRP_Sediment_Delivery_Ratio},_tp_prp_del_ratio => {PRP_TP_Delivery_Ratio},_sed_prp_nonpointSource_t => {Sed_prp_nonpointSource_t},_tp_prp_nonpointSource_lb => {Tp_prp_nonpointSource_lb},_perc_contr_upstream_prp =>{loadReductionCropla},_perc_contr_upstream_prp2 => {Percentage_cropland_contributing_upstream_of_PRP},_field_id => {FieldID});";
                                try
                                {
                                    DataSet rusle2data = _masterRepository.GetAndSelectTableItems(query);
                                    if (rusle2data != null && rusle2data.Tables.Count != 0 && int.Parse(rusle2data.Tables[0].Rows[0][0].ToString()) == 1)
                                    {
                                        Console.WriteLine("Sheet 2 in field_id: " + FieldID + " row is updated");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.LogError(ex);
                                }
                            }

                            var worksheet3 = result.Tables.Cast<DataTable>().FirstOrDefault(table => string.Equals(table.TableName, "prp", StringComparison.OrdinalIgnoreCase));
                            var data3 = new List<Rusle2ExcelData>();

                            if (worksheet3 != null)
                            {
                                foreach (DataRow row in worksheet3.Rows)
                                {
                                    var FieldID = row.Table.Columns.Contains("FieldID") ? row["FieldID"].ToString() : string.Empty;
                                    if (string.IsNullOrEmpty(FieldID) || !int.TryParse(FieldID, out int fieldIdInt))
                                    {
                                        break;
                                    }

                                    var POINT_X = row.Table.Columns.Contains("POINT_X") ? row["POINT_X"].ToString() : "0";
                                    var POINT_Y = row.Table.Columns.Contains("POINT_Y") ? row["POINT_Y"].ToString() : "0";
                                    //string combinedPoint = (POINT_X != "0" ? POINT_X : "") + (POINT_X != "0" && POINT_Y != "0" ? "," : "") + (POINT_Y != "0" ? POINT_Y : "");
                                    string combinedPoint = POINT_X + "," + POINT_Y;
                                    string query = $"select * from \"FSRCalc\".water_resle2upadte_data (datarefcursor => 'data',_ActionType=>'updateRusle2Fielddata',_Watershed_Coords => '{combinedPoint}',_field_id => {FieldID});";
                                    try
                                    {
                                        DataSet rusle2data = _masterRepository.GetAndSelectTableItems(query);
                                        if (rusle2data != null && rusle2data.Tables.Count != 0 && int.Parse(rusle2data.Tables[0].Rows[0][0].ToString()) == 1)
                                        {
                                            Console.WriteLine("Sheet 3 in field_id: " + FieldID + " row is updated");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.LogError(ex);
                                    }
                                }
                            }

                            return Json(new { success = true, message = "The update process has been completed successfully.", data, fieldIDs });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Json(new { success = false, message = ex.Message.ToString() });
            }

            return Json(new { success = false, message = "Please select a valid Excel file" });
        }

        [HttpPost]
        public async Task<IActionResult> GetRusle2Results(string fieldIDs)
        {
            try
            {
                int Rusele2ResponseLength = 5;
                IMasterRepository _masterRepository = new MasterRepository();

                string[] values = fieldIDs.Split(',');

                List<string> fieldIDss = new List<string>();
                List<string> mZoneIDs = new List<string>();
                List<string> years = new List<string>();

                DataTable dataTable = new DataTable();

                dataTable.Columns.Add(new DataColumn("field_option_id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("field_id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("management_zones_id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("year", typeof(int)));
                dataTable.Columns.Add(new DataColumn("NET_C_FACTOR", typeof(double)));
                dataTable.Columns.Add(new DataColumn("total_sed_mass_leaving", typeof(double)));
                dataTable.Columns.Add(new DataColumn("Zone_name", typeof(string)));
                dataTable.Columns.Add(new DataColumn("crop_name", typeof(string)));
                dataTable.Columns.Add(new DataColumn("tillage_name", typeof(string)));
                dataTable.Columns.Add(new DataColumn("option", typeof(string)));
                dataTable.Columns.Add(new DataColumn("field_sediment_delivery_ratio", typeof(double)));
                dataTable.Columns.Add(new DataColumn("field_tp_delivery_ratio", typeof(double)));
                dataTable.Columns.Add(new DataColumn("CLIMATE_PTR", typeof(string)));
                dataTable.Columns.Add(new DataColumn("SOIL_PTR", typeof(string)));
                dataTable.Columns.Add(new DataColumn("MAN_BASE_PTR", typeof(string)));
                dataTable.Columns.Add(new DataColumn("slope_percentage", typeof(string)));
                #region Commented code 
                foreach (string entry in values)
                {
                    string[] components = entry.Split(';');

                    string fieldID = null;
                    string mZoneID = null;
                    string year = null;

                    foreach (string component in components)
                    {
                        if (component.StartsWith("FieldID:"))
                        {
                            fieldID = component.Split(':')[1];
                        }
                        else if (component.StartsWith("M_Zone_ID:"))
                        {
                            mZoneID = component.Split(':')[1];
                        }
                        else if (component.StartsWith("Year:"))
                        {
                            year = component.Split(':')[1];
                        }
                    }

                    string query = $"select * from  \"FSRCalc\".water_getrusle2api_data(datarefcursor => 'data',_field_id => {fieldID} , _management_zones_id => {mZoneID}, _year => {year}, _field_option_id=>1, _actiontype=>'GetRusle2APIParm');";
                    try
                    {
                        DataSet rusle2data = _masterRepository.GetAndSelectTableItems(query);
                        if (rusle2data != null && rusle2data.Tables.Count != 0 && rusle2data.Tables[0].Rows.Count != 0)
                        {
                            string netCFactorValue = "";
                            try
                            {
                                var response = await commonMethods.CommonRusleValue(Convert.ToInt32(fieldID), Convert.ToInt32(mZoneID), Convert.ToInt32(year), 1, "");
                                if (response.Length > Rusele2ResponseLength)
                                {

                                    netCFactorValue = response.FirstOrDefault(line => line.StartsWith("NET_C_FACTOR"))?.Split(':', 2)?[1]?.Trim() ?? "";
                                    string SLOPEDeliveryValue = response.FirstOrDefault(line => line.StartsWith("SLOPE_DELIVERY"))?.Split(':', 2)?[1]?.Trim() ?? "";
                                    string SOIL_PTRValue = response.FirstOrDefault(line => line.StartsWith("SOIL_PTR"))?.Split(':', 2)?[1]?.Trim() ?? "";
                                    string MAN_BASE_PTRValue = response.FirstOrDefault(line => line.StartsWith("MAN_BASE_PTR"))?.Split(':', 2)?[1]?.Trim() ?? "";
                                    string CLIMATE_PTRValue = response.FirstOrDefault(line => line.StartsWith("CLIMATE_PTR"))?.Split(':', 2)?[1]?.Trim() ?? "";

                                    DataRow row = dataTable.NewRow();
                                    row["NET_C_FACTOR"] = (netCFactorValue == "") ? 0 : netCFactorValue;
                                    row["total_sed_mass_leaving"] = (SLOPEDeliveryValue == "") ? 0 : SLOPEDeliveryValue;
                                    row["field_option_id"] = rusle2data.Tables[0].Rows[0]["field_option_id"].ToString();
                                    row["field_id"] = fieldID;
                                    row["management_zones_id"] = mZoneID;
                                    row["year"] = year;
                                    row["Zone_name"] = rusle2data.Tables[0].Rows[0]["zone_name"].ToString();
                                    row["crop_name"] = rusle2data.Tables[0].Rows[0]["name"].ToString();
                                    row["tillage_name"] = rusle2data.Tables[0].Rows[0]["tillagename"].ToString();
                                    row["option"] = rusle2data.Tables[0].Rows[0]["option"].ToString();
                                    row["field_sediment_delivery_ratio"] = Convert.ToDecimal(rusle2data.Tables[0].Rows[0]["field_sediment_delivery_ratio"].ToString());
                                    row["field_tp_delivery_ratio"] = Convert.ToDecimal(rusle2data.Tables[0].Rows[0]["field_tp_delivery_ratio"].ToString());
                                    row["CLIMATE_PTR"] = CLIMATE_PTRValue;
                                    row["SOIL_PTR"] = SOIL_PTRValue;
                                    row["MAN_BASE_PTR"] = MAN_BASE_PTRValue;
                                    row["slope_percentage"] = rusle2data.Tables[0].Rows[0]["slope_percentage"].ToString();
                                    dataTable.Rows.Add(row);
                                }
                                else
                                {
                                    return Json(new { success = false, message = "Rusle2 not responding", datatable = "" });
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.LogError(ex);
                                Console.WriteLine("getrow insert where field_id: " + netCFactorValue + fieldID + " ,management_zone_id is :" + mZoneID + "year" + year + " is updated");
                                return Json(new { success = false, message = ex.Message.ToString(), datatable = "" });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.LogError(ex);
                        return Json(new { success = false, message = ex.Message.ToString(), datatable = "" });
                    }

                    //// Add the extracted values to the respective lists
                    //fieldIDss.Add(fieldID);
                    //mZoneIDs.Add(mZoneID);
                    //years.Add(year);
                }
                #endregion

                return Json(new { success = true, message = "Rusle 2 results fetched successfully.", datatable = JsonConvert.SerializeObject(dataTable) });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Json(new { success = false, message = ex.Message.ToString(), datatable = "" });
            }
        }

        public IActionResult ExportFieldData()
        {
            try
            {
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
        public IActionResult GetFieldsByProducerIdDropDown(int producerId)
        {
            DataSet result = null;
            try
            {
                if (producerId > 0)
                {
                    result = commonMethods.GetFieldByproducerId(producerId);
                    List<MyFields> FieldsData = new List<MyFields>();
                    bool hasRows = result.Tables.Cast<DataTable>()
                                   .Any(table => table.Rows.Count != 0);
                    if (hasRows)
                    {
                        FieldsData = (from DataRow dr in result.Tables[0].Rows
                                      select new MyFields()
                                      {
                                          Field_Id = int.Parse(dr["field_id"].ToString()),
                                          Field_Name = dr["name"].ToString(),
                                      }).ToList();

                        ViewBag.Field = result;
                    }
                    if (FieldsData.Count > 0)
                    {
                        return Json(new { success = true, Data = FieldsData });
                    }
                    else
                    {
                        return Json(new { success = false, message = "No Data" });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false, message = "No Data" });
        }

        [HttpPost]
        public IActionResult GetFieldsTableForExportData(int fieldId, int producerId, string Actiontype)
        {
            DataSet result = null;
            try
            {
                if (Actiontype == "SingleField")
                {
                    result = commonMethods.GetFieldByIdExportData(fieldId);
                }
                else if (Actiontype == "AllField" || Actiontype == "AllFieldDownload")
                {
                    IMasterRepository _masterRepository = new MasterRepository();
                    object[] userParams = { "" };
                    object[] userValues = { "" };
                    string query = $"select * from \"FSRCalc\".water_get_fields_producer(datarefcursor => 'data',_actiontype => 'GetAllFieldsByProducerId',_producer_id=>{producerId});";
                    result = _masterRepository.GetAndSelectTableItems(query);
                }
                List<ExportData> FieldsData = new List<ExportData>();
                bool hasRows = result.Tables.Cast<DataTable>()
                               .Any(table => table.Rows.Count != 0);
                if (hasRows)
                {
                    FieldsData = (from DataRow dr in result.Tables[0].Rows
                                  select new ExportData()
                                  {
                                      FieldId = int.Parse(dr["field_id"].ToString()),
                                      FieldName = dr["field_name"].ToString(),
                                      Management_Zone_Id = int.Parse(dr["management_zones_id"].ToString()),
                                      Management_Zone = dr["zone_name"].ToString(),
                                      boundariesexist = (dr["boundariesexist"] != DBNull.Value) ? Convert.ToBoolean(dr["boundariesexist"]) : false,
                                      topologycheck = (dr["topologycheck"] != DBNull.Value) ? Convert.ToString(dr["topologycheck"]) : "N/A",
                                      topologycolor = (dr["topologycolor"] != DBNull.Value) ? Convert.ToString(dr["topologycolor"]) : "N/A",
                                      dataentered = (dr["dataentered"] != DBNull.Value) ? Convert.ToBoolean(dr["dataentered"]) : false,
                                      exportready = (dr["exportready"] != DBNull.Value) ? Convert.ToString(dr["exportready"]) : "No",
                                      Year = int.TryParse(dr["year"].ToString(), out int yearValue) ? yearValue : 0,

                                      //new added
                                      Field_Option_Id = (dr["field_option_id"] != DBNull.Value) ? Convert.ToInt32(dr["field_option_id"]) : 0,
                                      Crop1_Name = (dr["crop1_name"] != DBNull.Value) ? Convert.ToString(dr["crop1_name"]) : "N/A",
                                      Crop1_Rusle2_Lookup_Value = (dr["crop1_rusle2_lookup_value"] != DBNull.Value) ? Convert.ToString(dr["crop1_rusle2_lookup_value"]) : "N/A",
                                      Primary_Tillage = (dr["primary_tillage"] != DBNull.Value) ? Convert.ToString(dr["primary_tillage"]) : "N/A",
                                      Primary_Tillage_Rusle2_Lookup = (dr["primary_tillage_rusle2_lookup"] != DBNull.Value) ? Convert.ToString(dr["primary_tillage_rusle2_lookup"]) : "N/A",
                                      Secondary_Tillage = (dr["secondary_tillage"] != DBNull.Value) ? Convert.ToString(dr["secondary_tillage"]) : "N/A",
                                      Secondary_Tillage_Rusle2_Lookup = (dr["secondary_tillage_rusle2_lookup"] != DBNull.Value) ? Convert.ToString(dr["secondary_tillage_rusle2_lookup"]) : "N/A",
                                      Net_C_Factor = (dr["net_c_factor"] != DBNull.Value) ? Convert.ToString(dr["net_c_factor"]) : "0",
                                      CN3 = (dr["cn3"] != DBNull.Value) ? Convert.ToString(dr["cn3"]) : "0"


                                  }).ToList();


                }
                if (FieldsData.Count > 0)
                {
                    return Json(new { success = true, Data = FieldsData });
                }
                else
                {
                    //TempData["Danger"] = "Data not available for this field";
                    return Json(new { success = false, message = "No Data" });
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false, message = "No Data" });
        }

        public IActionResult CalculateFSRData()
        {
            try
            {
                //get data filename dropdown  

                ViewBag.FileName = (from Precipitation fileName in commonMethods.GetFileName()
                                    select new SelectListItem
                                    {
                                        Text = Convert.ToString(fileName.FileName),
                                        Value = Convert.ToString(fileName.FileName),
                                        //Value = Convert.ToString(fileName.Year),

                                    }).ToList();
                //get data producer dropdown  
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

        //NOTE: Precip file upload
        [HttpPost]
        public IActionResult CalculateFSRPrecipFileUpload(IFormFile file, [FromForm] string? Field_Id)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                Field_Id = string.IsNullOrEmpty(Field_Id) || Field_Id == "null" ? "0" : Field_Id;
                int field_id = int.Parse(Field_Id);
                if (file != null)
                {
                    switch (file.ContentType)
                    {
                        case "application/vnd.ms-excel":
                        case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                            break;
                        default:
                            return Json(new { success = false, message = "Please select a valid Excel file" });
                    }

                    string fileName = Path.GetFileName(file.FileName);
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
                        file.CopyTo(stream);
                        Console.WriteLine("New File :{0} create", fileName);
                    }

                    // Read the Excel file into a DataSet
                    using (var stream = file.OpenReadStream())
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
                            var worksheet1 = result.Tables[0];//[0]
                            string worksheet1DT = JsonConvert.SerializeObject(worksheet1, Formatting.None);
                            string worksheetQuery = $"SELECT * FROM \"FSRCalc\".water_uploadcalculate_fsr_Excelupload(datarefcursor := 'data',_actiontype := 'insert_precip_calc_fsr_data_excel',_filename := '{fileName}',jsonstring := '{worksheet1DT}');";
                            DataSet worksheetQueryReturn = _masterRepository.GetAndSelectTableItems(worksheetQuery);

                            if (worksheetQueryReturn != null && worksheetQueryReturn.Tables.Count != 0 && int.Parse(worksheetQueryReturn.Tables[0].Rows[0][0].ToString()) == 1)
                            {
                                //NOTE: Assign file to the field
                                string update_query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_ActionType =>'assign_precip_file_to_field',_filename => '{fileName}',_field_id=>{Field_Id});";
                                var returnData = _masterRepository.GetAndSelectTableItemsWithoutCursor(update_query);
                                return Json(new { success = true, message = "Precipitation file uploaded Successfully.", filename = fileName });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false, message = "Please select a valid Excel file" });
        }

        [HttpPost]
        public IActionResult ApplyExistingPrecipFileDropdownData([FromForm] string? Field_Id, [FromForm] string File_Name)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                Field_Id = string.IsNullOrEmpty(Field_Id) || Field_Id == "null" ? "0" : Field_Id;
                int field_id = int.Parse(Field_Id);
                if (File_Name != "")
                {
                    string fileName = Path.GetFileName(File_Name);

                    //NOTE: Assign file to the field
                    string update_query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_ActionType =>'assign_precip_file_to_field',_filename => '{fileName}',_field_id=>{Field_Id});";
                    var returnData = _masterRepository.GetAndSelectTableItemsWithoutCursor(update_query);


                    return Json(new { success = true, message = "Precipitation file upload Successfully." });
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                return Json(new { success = false, message = "Please select a valid Excel file" });
            }
            return Json(new { success = false, message = "Please check column name or select a valid Excel file" });
        }
        //NOTE: Irrigation file upload
        [HttpPost]
        public IActionResult CalculateFSRDUploadIrrigation(IFormFile file, string total_years, int Field_id)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                if (file != null)
                {
                    switch (file.ContentType)
                    {
                        case "application/vnd.ms-excel":
                        case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                            break;
                        default:
                            return Json(new { success = false, message = "Please select a valid Excel file" });
                    }

                    string fileName = Path.GetFileName(file.FileName);
                    string path = Path.Combine(RootDirectoryPath.WebRootPath, "IrrigationFile");

                    // Read the Excel file into a DataSet
                    using (var stream = file.OpenReadStream())
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
                                file.CopyTo(str);
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

                            List<string> opYears = total_years.Split(',').Select(year => year.Trim()).ToList();
                            List<string> excelYears = yearsDTArr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                  .Select(year => year.Trim())
                                                                  .OrderBy(year => year)
                                                                  .ToList();

                            if (!excelYears.Any(year => opYears.Contains(year)))
                                return Json(new { success = false, message = "Current years are not matching with the years which are available in excel." });

                            var IrrExcelField_Id = Convert.ToInt32(field_irrigationDT.Rows[0]["Field_id"]);
                            string field_irrigation_exists_or_not = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_ActionType =>'field_irrigation_GetByField_id' ,_field_id => {IrrExcelField_Id});";

                            DataSet existsOrNot = _masterRepository.GetAndSelectTableItems(field_irrigation_exists_or_not);
                            if (existsOrNot != null && existsOrNot.Tables.Count > 0 && existsOrNot.Tables[0].Rows.Count > 0)
                            {
                                if (IrrExcelField_Id != Field_id)
                                {
                                    return Json(new { success = false, message = $"This Excel file field id and selected field id does not match, please check '{fileName}' file." });
                                }
                                else
                                {
                                    return Json(new { success = false, message = "This Excel file data are already belongs to the selected field." });
                                }
                            }
                            //insert execl in database
                            string worksheetDT = JsonConvert.SerializeObject(field_irrigationDT, Formatting.None);
                            string worksheetQuery = $"SELECT * FROM \"FSRCalc\".water_uploadcalculate_fsr_Excelupload(datarefcursor := 'data',_actiontype := 'insert_irrigation_calc_fsr_data_excel',_filename := '{fileName}',jsonstring := '{worksheetDT}');";
                            DataSet worksheetQueryReturn = _masterRepository.GetAndSelectTableItems(worksheetQuery);

                            if (worksheetQueryReturn != null && worksheetQueryReturn.Tables.Count != 0 && int.Parse(worksheetQueryReturn.Tables[0].Rows[0][0].ToString()) == 1)
                            {
                                return Json(new { success = true, message = "Irrigation file uploaded Successfully." });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false, message = "Please select a valid Excel file" });
        }

        [HttpPost]
        public IActionResult Get_OperationYearsAndNameFromFieldId(int field_Id, int field_Option_Id)
        {
            DataSet result = null;
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                string query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',datarefcursor2 =>'data2',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {field_Id},_field_option_id=>{field_Option_Id});";
                //result = _masterRepository.GetAndSelectTableItems(query);

                result = _masterRepository.GetAndSelectMultipleTableItems(query);
                string FieldOptionName = JsonConvert.SerializeObject(result.Tables[1], Formatting.None);


                List<IrrigationZoneTable> ZoneData = new List<IrrigationZoneTable>();
                bool hasRows = result.Tables.Cast<DataTable>()
                               .Any(table => table.Rows.Count != 0);
                if (hasRows)
                {
                    ZoneData = (from DataRow dr in result.Tables[0].Rows
                                select new IrrigationZoneTable()
                                {
                                    //Field_Id = (int)dr["field_id"],
                                    //Zone_Name = dr["zone_name"].ToString(),
                                    // Zone_Irrigated = Convert.IsDBNull(dr["zone_irrigated"]) ? false : ((bool)dr["zone_irrigated"] == false ? false : true),
                                    Year = (int)dr["year"],
                                    Filename = Convert.ToString(dr["precip_file"])
                                    // Crop_Name = dr["crop_name"].ToString()
                                }).ToList();
                }
                if (ZoneData.Count != 0 || FieldOptionName != string.Empty)
                {
                    return Json(new { success = true, Data = ZoneData, OptionDropdown = FieldOptionName });
                }
                else
                {
                    return Json(new { scuccess = false, message = "No Data" });
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false, message = "No Data" });

        }

        [HttpPost]
        public IActionResult GetIrrigatedFilesFromFieldId(int field_Id)
        {
            DataSet result = null;
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                string query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actiontype => 'get_IrrigatedFiles_For_FieldId',_field_id => {field_Id});";
                result = _masterRepository.GetAndSelectTableItems(query);

                List<IrrigationZoneTable> ZoneData = new List<IrrigationZoneTable>();
                bool hasRows = result.Tables.Cast<DataTable>()
                               .Any(table => table.Rows.Count != 0);
                if (hasRows)
                {
                    ZoneData = (from DataRow dr in result.Tables[0].Rows
                                select new IrrigationZoneTable()
                                {
                                    Filename = dr["fileName"].ToString()
                                }).ToList();
                }
                if (ZoneData.Count != 0)
                {
                    return Json(new { success = true, Data = ZoneData });
                }
                else
                {
                    return Json(new { scuccess = false, message = "No Data" });
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false, message = "No Data" });
        }

        [HttpGet]
        public async Task<IActionResult> CalculateBenchmark(int field_Id, int FieldOptionId)
        {
            Dictionary<string, string> Result = new Dictionary<string, string>();
            try
            {

                Result = await commonMethods.CalculateBenchmarkCommon(field_Id, FieldOptionId);
                if (Result["success"] == "true")
                {
                    return Json(new { success = true, message = Result["message"] });
                }
                else
                {
                    return Json(new { success = false, message = Result["message"] });
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false, message = Result["message"] });

        }

        [HttpGet]
        public async Task<IActionResult> CalculatePreviewFSR(int field_Id, int field_Option_Id)
        {
            Dictionary<string, string> Result = new Dictionary<string, string>();

            try
            {
                Result = await commonMethods.CalculatePreviewFSRCommon(field_Id, field_Option_Id);
                if (Result["success"] == "true")
                {
                    return Json(new { success = true, message = Result["message"] });
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false, message = Result["message"] });

        }

        [HttpGet]
        public IActionResult PreviewFSRDataTable(int field_Id, int field_Option_Id)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                //int startYear = 2018;
                //int endYear = 2023;


                #region Publish frs table code 

                //string currentyear = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {field_Id});";
                //DataSet currentyear_data = _masterRepository.GetAndSelectTableItems(currentyear);
                //DataTable currentyear_data_DT = currentyear_data.Tables.Count > 0 ? currentyear_data.Tables[0] : null;
                //var years = currentyear_data_DT.AsEnumerable().Select(row => row.Field<int?>("year")).Where(year => year.HasValue).Select(year => year.Value);

                //int startYear = years.Min();
                //int endYear = years.Max();


                ////field_details publish fsr data:not publish only admin can view (flag=admin)
                //string queryforfield_details = $"select * from \"FSRCalc\".frs_new_get_fields_details(datarefcursor :='data' ,_field_id =>{field_Id}, _field_option_id =>1 , yearfrom => {startYear} , yearto => {endYear},flag=>'admin');";
                ////string query = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValues[0]}',{userParams[1]} => '{userValues[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                //DataSet field_details_data = _masterRepository.GetAndSelectTableItems(queryforfield_details);
                //DataRow field_details_data_FirstRow = field_details_data.Tables[0].Rows[0];
                ////ViewBag.arces = int.TryParse(field_details_data_FirstRow["arces"]?.ToString().Trim(), out int arces) ? arces : 0;
                //ViewBag.arces = field_details_data_FirstRow["arces"];
                //ViewBag.crop_names = field_details_data_FirstRow["crop_names"];
                //ViewBag.tillag_names = field_details_data_FirstRow["tillag_names"];
                //ViewBag.cover_crop_names = field_details_data_FirstRow["cover_crop_names"];
                //ViewBag.RFactorLevels = field_details_data_FirstRow["r4_n_fert_level"];
                //ViewBag.RFactorLevelsp = field_details_data_FirstRow["r4_p_fert_level"];
                //ViewBag.fieldRiskSediment = field_details_data_FirstRow["_risk_level_sed"];
                //ViewBag.fieldPRiskSediment = field_details_data_FirstRow["_risk_level_p"];
                //ViewBag.quality_grade = field_details_data_FirstRow["_quality_grade"];
                //ViewBag.management_conversation_practice = field_details_data_FirstRow["management_conversation_practice"];
                //ViewBag.structural_conversation_practice = field_details_data_FirstRow["structural_conversation_practice"];
                //ViewBag.drainage = field_details_data_FirstRow["drainage"];

                ////field report card
                //string query = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{field_Id},  _start_year => {startYear} , _end_year => {endYear}, _field_option_id => 1);";
                //DataSet DsfsrData = _masterRepository.GetAndSelectTableItems(query);
                //ViewBag.FSRdata = DsfsrData;
                /////NOTE: Calculate Field Stewardship Rating
                //double fsrRating = 0;
                //double sumofallgradefsr = 0;
                //if (DsfsrData.Tables.Count > 0 && DsfsrData.Tables[0].Rows.Count > 0)
                //{
                //    object fsrValue = DsfsrData.Tables[0].Rows[0]["fsr"];
                //    fsrRating = (fsrValue != DBNull.Value) ? Math.Round(Convert.ToDouble(fsrValue), 2) : 0;
                //    sumofallgradefsr = (DsfsrData.Tables[0].Rows[0]["sumofallgrade"] != DBNull.Value) ? Math.Round(Convert.ToDouble(DsfsrData.Tables[0].Rows[0]["sumofallgrade"]), 2) : 0;
                //}
                //ViewBag.FSR = fsrRating;
                //ViewBag.FSRsumofallgrade = sumofallgradefsr;

                ////enviromment outcome data
                //// Nfertin
                //Dictionary<string, string> factorLevels = commonMethods.four_R_chart_Function(field_Id, Convert.ToInt32(startYear), Convert.ToInt32(endYear), 1, 1, "value");
                //List<string> factorKeys = new List<string>() { "source", "rate", "timing", "placement" };
                //double nfertin = factorKeys.Average(key => commonMethods.nFetinAndpnertin(factorLevels[key]));
                //// Pfetrin
                //Dictionary<string, string> factorLevelsp = commonMethods.four_R_chart_Function(field_Id, Convert.ToInt32(startYear), Convert.ToInt32(endYear), 1, 2, "value");
                //double pfertin = factorKeys.Average(key => commonMethods.nFetinAndpnertin(factorLevelsp[key]));
                //#region Environment outcome get data
                ////first column score
                ////field_option_id=1
                //string EnvrmtOut_scoreQuery = $"select * from \"FSRCalc\".environment_outcomedata(datarefcursor :='data' ,_field_id =>{field_Id}, _field_option_id =>{1} ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {startYear} , _end_year => {endYear},flag => 'admin');";
                //DataSet EnvrmtOut_scoreQueryData = _masterRepository.GetAndSelectTableItems(EnvrmtOut_scoreQuery);
                //ViewBag.DsEnvCurrentOpScore = EnvrmtOut_scoreQueryData != null && EnvrmtOut_scoreQueryData.Tables.Count > 0 && EnvrmtOut_scoreQueryData.Tables[0].Rows.Count > 0 ? EnvrmtOut_scoreQueryData : null;
                ////alfalfa column score
                ////field_option_id=5
                //string EnvrmtOut_AlfalfaScoreQuery = $"select * from \"FSRCalc\".environment_outcomedata(datarefcursor :='data' ,_field_id =>{field_Id}, _field_option_id =>{5} ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {startYear} , _end_year => {endYear},flag => 'admin');";
                //DataSet EnvrmtOut_AlfalfaScoreQueryData = _masterRepository.GetAndSelectTableItems(EnvrmtOut_AlfalfaScoreQuery);
                //ViewBag.DsEnvAlfalfaScore = EnvrmtOut_AlfalfaScoreQueryData != null && EnvrmtOut_AlfalfaScoreQueryData.Tables.Count > 0 && EnvrmtOut_AlfalfaScoreQueryData.Tables[0].Rows.Count > 0 ? EnvrmtOut_AlfalfaScoreQueryData : null;
                ////moldboad column score
                ////field_option_id=6
                //string EnvrmtOut_MoldboadScoreQuery = $"select * from \"FSRCalc\".environment_outcomedata(datarefcursor :='data' ,_field_id =>{field_Id}, _field_option_id =>{6} ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {startYear} , _end_year => {endYear},flag => 'admin');";
                //DataSet EnvrmtOut_MoldboadScoreQueryData = _masterRepository.GetAndSelectTableItems(EnvrmtOut_MoldboadScoreQuery);
                //ViewBag.DsEnvMoldboadScore = EnvrmtOut_MoldboadScoreQueryData != null && EnvrmtOut_MoldboadScoreQueryData.Tables.Count > 0 && EnvrmtOut_MoldboadScoreQueryData.Tables[0].Rows.Count > 0 ? EnvrmtOut_MoldboadScoreQueryData : null;
                //string EnvrmtOut_opt1ScoreQuery = $"select * from \"FSRCalc\".environment_outcomedata(datarefcursor :='data' ,_field_id =>{field_Id}, _field_option_id =>{3} ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {startYear} , _end_year => {endYear},flag => 'admin');";
                //DataSet EnvrmtOut_opt1boadScoreQueryData = _masterRepository.GetAndSelectTableItems(EnvrmtOut_opt1ScoreQuery);
                //ViewBag.DsEnvopt1boadScore = EnvrmtOut_opt1boadScoreQueryData != null && EnvrmtOut_opt1boadScoreQueryData.Tables.Count > 0 && EnvrmtOut_opt1boadScoreQueryData.Tables[0].Rows.Count > 0 ? EnvrmtOut_opt1boadScoreQueryData : null;
                //string EnvrmtOut_opt2ScoreQuery = $"select * from \"FSRCalc\".environment_outcomedata(datarefcursor :='data' ,_field_id =>{field_Id}, _field_option_id =>{4} ,_nfertin => {nfertin}, _pfertin => {pfertin}, _start_year => {startYear} , _end_year => {endYear},flag => 'admin');";
                //DataSet EnvrmtOut_opt2boadScoreQueryData = _masterRepository.GetAndSelectTableItems(EnvrmtOut_opt2ScoreQuery);
                //ViewBag.DsEnvopt2boadScore = EnvrmtOut_opt2boadScoreQueryData != null && EnvrmtOut_opt2boadScoreQueryData.Tables.Count > 0 && EnvrmtOut_opt2boadScoreQueryData.Tables[0].Rows.Count > 0 ? EnvrmtOut_opt2boadScoreQueryData : null;
                //#endregion

                #region Option 1/2, Alfala & Mold
                //// Option 1
                //// string queryopt1 = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValuesopt1[0]}',{userParams[1]} => '{userValuesopt1[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                //string queryopt1 = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{field_Id},  _start_year => {startYear} , _end_year => {endYear}, _field_option_id => 3,flag => 'admin');";
                //DataSet opt1Data = _masterRepository.GetAndSelectTableItems(queryopt1);
                //double opt1fsrRating = 0;
                //double sumofallgradeopt1 = 0;
                //if (opt1Data.Tables.Count > 0 && opt1Data.Tables[0].Rows.Count > 0)
                //{
                //    object opt1fsrValue = opt1Data.Tables[0].Rows[0]["fsr"];
                //    opt1fsrRating = (opt1fsrValue != DBNull.Value) ? Math.Round(Convert.ToDouble(opt1fsrValue), 2) : 0;
                //    sumofallgradeopt1 = (opt1Data.Tables[0].Rows[0]["sumofallgrade"] != DBNull.Value) ? Math.Round(Convert.ToDouble(opt1Data.Tables[0].Rows[0]["sumofallgrade"]), 2) : 0;
                //}
                //ViewBag.opt1FSR = opt1fsrRating;
                //ViewBag.opt1sumofallgrade = sumofallgradeopt1;
                //ViewBag.opt1data = opt1Data;
                //ViewBag.opt1title = (opt1Data.Tables[0].Rows[0]["_option_name"] != DBNull.Value) ? opt1Data.Tables[0].Rows[0]["_option_name"] : "Option1";
                //ViewBag.opt1description = (opt1Data.Tables[0].Rows[0]["_option_description"] != DBNull.Value) ? opt1Data.Tables[0].Rows[0]["_option_description"] : "";
                //// Option 2
                ////  string queryopt2 = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValuesopt2[0]}',{userParams[1]} => '{userValuesopt2[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                //string queryopt2 = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{field_Id},   _start_year => {startYear} , _end_year => {endYear}, _field_option_id => 4,flag => 'admin');";
                //DataSet opt2Data = _masterRepository.GetAndSelectTableItems(queryopt2);
                //double opt2fsrRating = 0;
                //double sumofallgradeopt2 = 0;
                //if (opt2Data.Tables.Count > 0 && opt2Data.Tables[0].Rows.Count > 0)
                //{
                //    object opt2fsrValue = opt2Data.Tables[0].Rows[0]["fsr"];
                //    opt2fsrRating = (opt2fsrValue != DBNull.Value) ? Math.Round(Convert.ToDouble(opt2fsrValue), 2) : 0;
                //    sumofallgradeopt2 = (opt2Data.Tables[0].Rows[0]["sumofallgrade"] != DBNull.Value) ? Math.Round(Convert.ToDouble(opt2Data.Tables[0].Rows[0]["sumofallgrade"]), 2) : 0;
                //}
                //ViewBag.opt2FSR = opt2fsrRating;
                //ViewBag.opt2sumofallgrade = sumofallgradeopt2;
                //ViewBag.opt2data = opt2Data;
                //ViewBag.opt2title = (opt2Data.Tables[0].Rows[0]["_option_name"] != DBNull.Value) ? opt2Data.Tables[0].Rows[0]["_option_name"] : "Option2";
                //ViewBag.opt2description = (opt2Data.Tables[0].Rows[0]["_option_description"] != DBNull.Value) ? opt2Data.Tables[0].Rows[0]["_option_description"] : "";
                //// Alfalfa
                //// string queryalf = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValuesalf[0]}',{userParams[1]} => '{userValuesalf[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                //string queryalf = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{field_Id},  _start_year => {startYear} , _end_year => {endYear}, _field_option_id => 5,flag => 'admin');";
                //DataSet alfData = _masterRepository.GetAndSelectTableItems(queryalf);
                //double alffsrRating = 0;
                //double sumofallgradealf = 0;
                //if (alfData.Tables.Count > 0 && alfData.Tables[0].Rows.Count > 0)
                //{
                //    object alffsrValue = alfData.Tables[0].Rows[0]["fsr"];
                //    alffsrRating = (alffsrValue != DBNull.Value) ? Math.Round(Convert.ToDouble(alffsrValue), 2) : 0;
                //    sumofallgradealf = (alfData.Tables[0].Rows[0]["sumofallgrade"] != DBNull.Value) ? Math.Round(Convert.ToDouble(alfData.Tables[0].Rows[0]["sumofallgrade"]), 2) : 0;
                //}
                //ViewBag.alfFSR = alffsrRating;
                //ViewBag.alfsumofallgrade = sumofallgradealf;
                //ViewBag.alfData = alfData;
                //// Mold
                //// string querymol = $"SELECT * from \"FSRCalc\".water_latest_fsr_calc_2_new (datarefcursor := 'data',{userParams[0]} => '{userValuesmold[0]}',{userParams[1]} => '{userValuesmold[1]}', {userParams[2]} => {nfertin}, {userParams[3]} => {pfertin});";
                //string querymol = $"select * from \"FSRCalc\".fsr_new_calculate_FSR(datarefcursor :='data' ,_field_id =>{field_Id},   _start_year => {startYear} , _end_year => {endYear}, _field_option_id => 6,flag => 'admin');";
                //DataSet molData = _masterRepository.GetAndSelectTableItems(querymol);
                //double molfsrRating = 0;
                //double sumofallgrademol = 0;
                //if (molData.Tables.Count > 0 && molData.Tables[0].Rows.Count > 0)
                //{
                //    object molfsrValue = molData.Tables[0].Rows[0]["fsr"];
                //    molfsrRating = (molfsrValue != DBNull.Value) ? Math.Round(Convert.ToDouble(molfsrValue), 2) : 0;
                //    sumofallgrademol = (molData.Tables[0].Rows[0]["sumofallgrade"] != DBNull.Value) ? Math.Round(Convert.ToDouble(molData.Tables[0].Rows[0]["sumofallgrade"]), 2) : 0;
                //}
                //ViewBag.molFSR = molfsrRating;
                //ViewBag.molsumofallgrade = sumofallgrademol;
                //ViewBag.molData = molData;
                #endregion

                #endregion
                //NOTE: in this query  _field_option_id_ => null means get (1,5,6,7) Option data, get indivisual option id data specify option id like: _field_option_id_ => 5
                string GetPreviewFSRDataQuery = $"select * from \"FSRCalc\".fsr_calculate_AdjustInputValue_step_5(datarefcursor => 'data', _actiontype => 'getpreviewfsrtable', _field_id_ => {field_Id}, _field_option_id_ => {field_Option_Id});";
                var GetPreviewFSRDataDs = _masterRepository.GetAndSelectTableItems(GetPreviewFSRDataQuery);

                string GetPreviewFSRTable2Query = $"select * from \"FSRCalc\".fsr_calculate_AdjustInputValue_step_5(datarefcursor => 'data', _actiontype => 'getpreviewfsrtable2', _field_id_ => {field_Id}, _field_option_id_ => {field_Option_Id});";
                var GetPreviewFSRTable2Ds = _masterRepository.GetAndSelectTableItems(GetPreviewFSRTable2Query);
                if (GetPreviewFSRTable2Ds != null && GetPreviewFSRTable2Ds.Tables.Count > 0 && GetPreviewFSRTable2Ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.PreviewFSRTable2Ds = GetPreviewFSRTable2Ds;
                }
                else
                {
                    ViewBag.PreviewFSRTable2Ds = null;
                }
                if (GetPreviewFSRDataDs != null && GetPreviewFSRDataDs.Tables.Count > 0 && GetPreviewFSRDataDs.Tables[0].Rows.Count > 0)
                {
                    ViewBag.PreviewFSRDs = GetPreviewFSRDataDs;
                    //return Json(new { success = true, message = "Preview FSR successfully run completed", data = JsonConvert.SerializeObject(GePreviewFSRDataDs, Formatting.None) });
                }
                else
                    ViewBag.PreviewFSRDs = null;
                return PartialView("~/Views/Admin/_PreviewFSRTable.cshtml");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Common");
            }
        }

        [HttpPost]
        public IActionResult GetStep567AdjustInputValue(int field_Id, string? field_option_id)
        {
            try
            {

                IMasterRepository _masterRepository = new MasterRepository();
                //this regex check to field_option_id in string/number
                bool check = (field_option_id != null) ? Regex.IsMatch(field_option_id, @"^[0-9]+$") : false;

                if (field_option_id == string.Empty || field_option_id == "" || check == false)
                {
                    field_option_id = "1";
                }
                //field_Id = 47;
                //Hydrology value get
                string query = $"select * from \"FSRCalc\".fsr_calculate_AdjustInputValue_step_5(datarefcursor => 'data',_actiontype => 'gethydrologyvalue',_field_id_ => {field_Id}, _field_option_id_ => {field_option_id});";
                DataSet GetHydrologyValue = _masterRepository.GetAndSelectTableItems(query);
                //Total p value get
                string query2 = $"select * from \"FSRCalc\".fsr_calculate_AdjustInputValue_step_5(datarefcursor => 'data',_actiontype => 'gettotalpvalue',_field_id_ => {field_Id}, _field_option_id_ => {field_option_id});";
                DataSet GetTotalpValue = _masterRepository.GetAndSelectTableItems(query2);

                string query3 = $"select * from \"FSRCalc\".fsr_calculate_AdjustInputValue_step_5(datarefcursor => 'data',_actiontype  => 'getsoillossvalue',_field_id_ => {field_Id},_field_option_id_ => {field_option_id});";
                DataSet GetSoillossValue = _masterRepository.GetAndSelectTableItems(query3);

                //step6 get
                string query4 = $"select * from \"FSRCalc\".fsr_calculate_adjustinputvalue_step_6_and_7_get_values(datarefcursor => 'data',_actiontype => 'getstep6values',_field_id_ => {field_Id});";
                DataSet GetStep6Value = _masterRepository.GetAndSelectTableItems(query4);

                //step7 get
                string query5 = $"select * from \"FSRCalc\".fsr_calculate_adjustinputvalue_step_6_and_7_get_values(datarefcursor => 'data',_actiontype => 'getstep7values',_field_id_ => {field_Id});";
                DataSet GetStep7Value = _masterRepository.GetAndSelectTableItems(query5);

                //step5 mz input box value set
                string query6 = $"select * from \"FSRCalc\".fsr_calculate_adjustinputvalue_step_5(datarefcursor =>'data', _actiontype => 'getmzinputboxvalue', _field_id_=>{field_Id});";
                DataSet GetStep5MzInputValue = _masterRepository.GetAndSelectTableItems(query6);

                if (GetHydrologyValue.Tables[0].Rows.Count != 0 && GetTotalpValue.Tables[0].Rows.Count != 0 && GetSoillossValue.Tables[0].Rows.Count != 0 && GetStep6Value.Tables[0].Rows.Count != 0 && GetStep7Value.Tables[0].Rows.Count != 0 && GetStep5MzInputValue.Tables[0].Rows.Count != 0)
                {
                    string hydrology = JsonConvert.SerializeObject(GetHydrologyValue, Formatting.None);
                    string totalp = JsonConvert.SerializeObject(GetTotalpValue, Formatting.None);
                    string soilloss = JsonConvert.SerializeObject(GetSoillossValue, Formatting.None);
                    string Step6Value = JsonConvert.SerializeObject(GetStep6Value, Formatting.None);
                    string Step7Value = JsonConvert.SerializeObject(GetStep7Value, Formatting.None);
                    string Step5MzInputValue = JsonConvert.SerializeObject(GetStep5MzInputValue, Formatting.None);
                    return Json(new { success = true, Hydrology = hydrology, Totalp = totalp, SoilLoss = soilloss, Step6Value = Step6Value, Step7Value = Step7Value, Step5MzInputValue = Step5MzInputValue });
                }
                else
                {
                    return Json(new { success = false, message = "No Data" });
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }

            return Json(new { success = false, message = "No Data" });
        }

        [HttpPost]
        public IActionResult UpdateStep567AdjustInputValue(string AllInputData)
        {
            try
            {
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(AllInputData);
                var jObject = JObject.FromObject(jsonObject);
                var soilLoss = jsonObject.SoilLoss;
                var totalp = jsonObject.TotalP;
                var hydrology = jsonObject.Hydrology;
                var step6 = jsonObject.Step6;
                var step7 = jsonObject.Step7;
                var Step5MzInput = jsonObject.Step5MzInput;
                var Step5FieldInput = jsonObject.Step5FieldInput;
                //Note : hydrology value upadte in fied and mz update in

                //fiels_id for globle
                var hydrologyData = hydrology["Hydrology_data"];
                var field_id = hydrologyData["FieldId"]?.ToObject<int?>();

                if (step6 != null)  // Add null check for step6
                {
                    var step6Data = step6["Step6_data"];  // Retrieve the Step6_data object

                    if (step6Data != null)
                    {
                        var downstreamIssue = step6Data["DownstreamIssue"]?.ToObject<bool?>() ?? true;
                        var streamReach = step6Data["StreamReach"]?.ToObject<string>() ?? "";
                        var watershedName = step6Data["WatershedName"]?.ToObject<string>() ?? "";
                        var maxAllowedGov = step6Data["MaxAllowedGov"]?.ToObject<bool?>() ?? false;
                        var sedimentGoal = step6Data["SedimentGoal"]?.ToObject<double?>() ?? 0;
                        var tpGoal = step6Data["TpGoal"]?.ToObject<double?>() ?? 0;
                        var percContrUpstreamPrp = step6Data["PercContrUpstreamPrp"]?.ToObject<double?>() ?? 0;

                        IMasterRepository _masterRepository = new MasterRepository();
                        //var field_id = 47;
                        object[] Params = { "_actiontype", "_field_id_", "_downstream_new", "_stream_reach", "_watershed_name", "_max_allowed_gov", "_tp_goal", "_perc_contr_upstream_prp", "_sediment_goal" };
                        object[] Values = { "setstep6values", field_id, downstreamIssue, streamReach, watershedName, maxAllowedGov, tpGoal, percContrUpstreamPrp, sedimentGoal };

                        string Setstep6 = $"SELECT * FROM \"FSRCalc\".fsr_calculate_adjustinputvalue_step_6_set_values( {Params[0]} => '{Values[0]}', {Params[1]} => {Values[1]}, {Params[2]} => {Values[2]}, {Params[3]} => '{Values[3]}', {Params[4]} => '{Values[4]}', {Params[5]} => {Values[5]}, {Params[6]} => {Values[6]}, {Params[7]} => {Values[7]}, {Params[8]} => {Values[8]});";
                        var returnData6 = _masterRepository.GetAndSelectTableItemsWithoutCursor(Setstep6);
                    }
                }

                if (step7 != null)  // Add null check for step7
                {
                    var step7Data = step7["Step7_data"];  // Retrieve the Step7_data object

                    if (step7Data != null)
                    {
                        var percFieldPrpContr = step7Data["PercFieldPrpContr"]?.ToObject<double?>() ?? 0;
                        var upstreamDrainagePrpContr = step7Data["UpstreamdrainagePrpContr"]?.ToObject<string>() ?? "0";
                        var totalAreaUpstreamDrainagePr = step7Data["TotalAreaUpstreamDrainagePr"]?.ToObject<double?>() ?? 0;
                        var totalArea = step7Data["TotalArea"]?.ToObject<double?>() ?? 0;
                        var areaContrPrp = step7Data["AreaContrPrp"]?.ToObject<double?>() ?? 0;
                        var sedPrpDelRatio = step7Data["SedPrpDelRatio"]?.ToObject<double?>() ?? 0;
                        var tpPrpDelRatio = step7Data["TpPrpDelRatio"]?.ToObject<double?>() ?? 0;
                        var sedPrpNonpointSourceT = step7Data["SedPrpNonpointSourceT"]?.ToObject<double?>() ?? 0;
                        var tpPrpNonpointSourceLb = step7Data["TpPrpNonpointSourceLb"]?.ToObject<double?>() ?? 0;

                        IMasterRepository _masterRepository = new MasterRepository();
                        //var field_id = 47;
                        object[] Params = { "_actiontype", "_field_id_", "_perc_field_prp_contr", "_upstream_drainage_prp_contr", "_total_area_upstream_drainage_prp_contr", "_total_area", "_area_contr_prp", "_sed_prp_del_ratio", "_tp_prp_del_ratio", "_sed_prp_nonpointsource_t", "_tp_prp_nonpointsource_lb" };
                        object[] Values = { "getstep7values", field_id, percFieldPrpContr, upstreamDrainagePrpContr, totalAreaUpstreamDrainagePr, totalArea, areaContrPrp, sedPrpDelRatio, tpPrpDelRatio, sedPrpNonpointSourceT, tpPrpNonpointSourceLb };

                        string Setstep7 = $"SELECT * FROM \"FSRCalc\".fsr_calculate_adjustinputvalue_step_7_set_values( {Params[0]} => '{Values[0]}', {Params[1]} => {Values[1]}, {Params[2]} => {Values[2]}, {Params[3]} => '{Values[3]}', {Params[4]} => {Values[4]}, {Params[5]} => {Values[5]}, {Params[6]} => {Values[6]}, {Params[7]} => {Values[7]}, {Params[8]} => {Values[8]}, {Params[9]} => {Values[9]}, {Params[10]} => {Values[10]});";
                        var returnData7 = _masterRepository.GetAndSelectTableItemsWithoutCursor(Setstep7);
                    }
                }

                if (soilLoss != null)
                {
                    //var field_id = 47;
                    IMasterRepository _masterRepository = new MasterRepository();
                    foreach (var soilLossRow in soilLoss)
                    {
                        var zydid = soilLossRow.Value["Zydid"]?.ToString() ?? "0";
                        // var mzid = soilLossRow.Value["Mzid"]?.ToString();
                        var soilLossSedManagement = soilLossRow.Value["Soilloss_SedManagement"]?.ToObject<double?>() ?? 0;
                        var soilLossSedDrainageFactor = soilLossRow.Value["Soilloss_SedDrainageFactor"]?.ToObject<double?>() ?? 0;
                        var soilLossSedStructureFactor = soilLossRow.Value["Soilloss_SedStructural"]?.ToObject<double?>() ?? 0;
                        var fiSoilUnitWeight = soilLossRow.Value["fi_Soilloss_SoilUnitWeight"]?.ToObject<double?>() ?? 0;
                        var fiSoilFormationRate = soilLossRow.Value["fi_Soilloss_SoilFormationRate"]?.ToObject<double?>() ?? 0;

                        // Process the retrieved values for each array element of soilLoss
                        object[] Params = { "_actiontype", "_id", "_sed_drainage_factor", "_sed_structure", "_sed_management", "_soil_unit_weight", "_soil_formation_rate" };
                        object[] Values = { "setstep5valuessoilloss", zydid, soilLossSedManagement, soilLossSedDrainageFactor, soilLossSedStructureFactor, fiSoilUnitWeight, fiSoilFormationRate };

                        string Setstep5_soiloss = $"SELECT * FROM \"FSRCalc\".fsr_calculate_adjustinputvalue_step_5_set_values( _actiontype => 'setstep5valuessoilloss', _id => {zydid}, _sed_drainage_factor => {soilLossSedDrainageFactor}, _sed_structure => {soilLossSedStructureFactor}, _sed_management => {soilLossSedManagement});";
                        var returnData5_soilloss = _masterRepository.GetAndSelectTableItemsWithoutCursor(Setstep5_soiloss);
                    }
                }


                if (totalp != null)
                {
                    //var field_id = 47;
                    IMasterRepository _masterRepository = new MasterRepository();
                    foreach (var TotalpRow in totalp)
                    {
                        var zydid = TotalpRow.Value["Zydid1"]?.ToString() ?? "0";
                        var mzid = TotalpRow.Value["Mzid1"]?.ToString() ?? "0";
                        var TotalpTPStructural = TotalpRow.Value["Totalp_TPStructural"]?.ToObject<double?>() ?? 0;
                        var TotalpTPManagement = TotalpRow.Value["Totalp_TPManagement"]?.ToObject<double?>() ?? 0;
                        var TotalpTPDrainageFactor = TotalpRow.Value["Totalp_TPDrainageFactor"]?.ToObject<double?>() ?? 0;
                        var mz_TotalpTPDissolved = TotalpRow.Value["mz_Totalp_TPDissolved"]?.ToObject<double?>() ?? 0;
                        var TotalpTPSediment = TotalpRow.Value["Totalp_TPSediment"]?.ToObject<double?>() ?? 0;
                        //var mz_TotalpFieldTPDeleveryRatio = TotalpRow.Value["mz_Totalp_FieldTPDeleveryRatio"]?.ToObject<double?>() ?? 0;
                        var mz_TotalpFieldTPDeleveryRatio = 0;
                        //Process the retrieved values for each array element of Total p

                        object[] Params = { "_actiontype", "_id", "_tp_structure", "_tp_management", "_tp_drainage_factor" };
                        object[] Values = { "setstep5valuestotalpgood", zydid, TotalpTPStructural, TotalpTPManagement, TotalpTPDrainageFactor };

                        string Setstep5_soiloss = $"SELECT * FROM \"FSRCalc\".fsr_calculate_adjustinputvalue_step_5_set_values( {Params[0]} => '{Values[0]}', {Params[1]} => {Values[1]}, {Params[2]} => {Values[2]}, {Params[3]} => {Values[3]}, {Params[4]} => {Values[4]});";
                        var returnData5_soilloss = _masterRepository.GetAndSelectTableItemsWithoutCursor(Setstep5_soiloss);
                    }
                }
                // All field update
                if (Step5FieldInput != null)  // Add null check for step6
                {
                    IMasterRepository _masterRepository = new MasterRepository();
                    var FieldId = Step5FieldInput["FieldId"]?.ToObject<double?>() ?? 0;
                    var Soilloss_SoilUnitWeight = Step5FieldInput["Soilloss_SoilUnitWeight"]?.ToObject<double?>() ?? 0;
                    var Soilloss_SoilFormationRate = Step5FieldInput["Soilloss_SoilFormationRate"]?.ToObject<double?>() ?? 0;
                    var HydrologyLambda = Step5FieldInput["HydrologyLambda"]?.ToObject<double?>() ?? 0;

                    //string updatefieldsQuery = "select * from \"FSRCalc\".fsr_calculate_adjustinputvalue_step_5_set_values(_actiontype =>'update5fieldinputvalues',_lambda =>0.05,_soil_unit_weight =>100 ,_soil_formation_rate => 3.32,_field_id => 47);\r\n";
                    string updatefieldsQuery = $"select * from \"FSRCalc\".fsr_calculate_adjustinputvalue_step_5_set_values(_actiontype =>'update5fieldinputvalues',_lambda =>{HydrologyLambda},_soil_unit_weight =>{Soilloss_SoilUnitWeight} ,_soil_formation_rate => {Soilloss_SoilFormationRate},_field_id => {FieldId});";
                    var updatefields = _masterRepository.GetAndSelectTableItemsWithoutCursor(updatefieldsQuery);
                    //string unitWeight = (string)soilLossRow0["fi_Soilloss_SoilUnitWeight"]; string formationRate = (string)soilLossRow0["fi_Soilloss_SoilFormationRate"]; Console.WriteLine("fi_Soilloss_SoilUnitWeight: " + unitWeight); Console.WriteLine("fi_Soilloss_SoilFormationRate: " + formationRate);
                }
                // step5 mz input update

                if (Step5MzInput != null)  // Add null check for step6
                {
                    IMasterRepository _masterRepository = new MasterRepository();
                    foreach (var MzInputRow in Step5MzInput)
                    {
                        var HydrologySlope_mzId = MzInputRow.Value["HydrologySlope_mzId"]?.ToObject<int?>() ?? 0;
                        var TotalpMzinputBoxRow = MzInputRow.Value["TotalpMzinputBoxRow"]?.ToObject<int?>() ?? 0;
                        var Soilloss_SedimentDeleveryRatio_mzId = MzInputRow.Value["Soilloss_SedimentDeleveryRatio_mzId"]?.ToObject<int?>() ?? 0;
                        var HydrologySlope = MzInputRow.Value["HydrologySlope"]?.ToObject<double?>() ?? 0;
                        var Totalp_TPDissolved = MzInputRow.Value["Totalp_TPDissolved"]?.ToObject<double?>() ?? 0;
                        var Totalp_TPSediment = MzInputRow.Value["Totalp_TPSediment"]?.ToObject<double?>() ?? 0;
                        var Totalp_FieldTPDeleveryRatio = MzInputRow.Value["Totalp_FieldTPDeleveryRatio"]?.ToObject<double?>() ?? 0;
                        var Soilloss_SedimentDeleveryRatio = MzInputRow.Value["Soilloss_SedimentDeleveryRatio"]?.ToObject<double?>() ?? 0;

                        string QueryMZUpdate = $"Select * from \"FSRCalc\".fsr_calculate_adjustinputvalue_step_5_set_values(_actiontype => 'update5mzinputvalues',_mgmt_id => {HydrologySlope_mzId},_slope => {HydrologySlope} ,_perc_tp_dissolved=>{Totalp_TPDissolved},_perc_tp_sediment => {Totalp_TPSediment},_field_tp_delivery_ratio => '{Totalp_FieldTPDeleveryRatio}',_field_sediment_delivery_ratio => {Soilloss_SedimentDeleveryRatio});";
                        var dataReturn = _masterRepository.GetAndSelectTableItemsWithoutCursor(QueryMZUpdate);
                    }
                }
                //TempData["Success"] = "Data update successfully.";
                return Json(new { success = true, message = "Data update in database" });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }

            return Json(new { success = false, message = "No Data" });
        }
        [HttpGet]
        public IActionResult ManageFieldLocationData()
        {
            try
            {
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

        [HttpGet]
        public IActionResult GetManageFieldLocationImage(int Field_Id)
        {
            try
            {
                // Field_Id = 47;
                IMasterRepository _masterRepository = new MasterRepository();
                string query = $"select * from \"FSRCalc\".field_location_data(datarefcursor =>'data',_actionType =>'getfieldlocationdata', _field_id => {Field_Id});";
                DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(query);
                DataTable dtProducerData;
                string json = null;
                if (DsProducerData != null && DsProducerData.Tables.Count > 0 && DsProducerData.Tables[0].Rows.Count > 0)
                {
                    dtProducerData = DsProducerData.Tables[0];
                    if (dtProducerData.Rows.Count > 0 &&
                              (dtProducerData.Rows[0]["field_location_img"] != DBNull.Value || dtProducerData.Rows[0]["field_location_img"] != "" ||
                         dtProducerData.Rows[0]["field_location_overview"] != DBNull.Value || dtProducerData.Rows[0]["field_location_overview"] != "" ||
                           dtProducerData.Rows[0]["mz_overview"] != DBNull.Value || dtProducerData.Rows[0]["mz_overview"] != ""))
                    {
                        json = JsonConvert.SerializeObject(DsProducerData, Formatting.None);
                        return Json(new { success = true, data = json });
                    }
                    return Json(new { success = false, data = json });

                }

                return Json(new { success = false, data = json });
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false, Message = "data not available" });
        }

        [HttpPost]
        public IActionResult ManageFieldLocationData(int Field_Id, IFormFile myImage, string FieldLocationOverview, string MzOverview, string previewImageName)
        {
            try
            {
                //Field_Id = 47;
                string fileName = string.Empty;
                if (myImage != null && myImage.Length > 0)
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    fileName = $"Img_{timestamp}_{Path.GetFileName(myImage.FileName)}";
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "OtherImages");

                    // Create the folder if it doesn't exist
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var filePath = Path.Combine(folderPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        myImage.CopyTo(fileStream);
                    }
                }
                if (fileName == string.Empty)
                {
                    fileName = previewImageName;
                }

                IMasterRepository _masterRepository = new MasterRepository();
                string query = $"select * from \"FSRCalc\".field_location_data(datarefcursor =>'data',_actionType =>'updatefieldlocationdata', _field_id => {Field_Id},_field_location_overview => '{FieldLocationOverview}',_field_location_img => '{fileName}',_mz_overview => '{MzOverview}');";
                DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(query);
                if (DsProducerData != null && DsProducerData.Tables.Count > 0 && DsProducerData.Tables[0].Rows.Count > 0)
                {
                    TempData["Success"] = "Field Location Data updated";
                }
                else
                {
                    TempData["Success"] = "Field location data not update please try again";
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return RedirectToAction("ManageFieldLocationData", "Admin");
        }

        [HttpPost]
        public IActionResult RemoveFSRIrrigationFile(int Field_Id, string fileName)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();

                string path = Path.Combine(RootDirectoryPath.WebRootPath, "IrrigationFile");
                string filePath = Path.Combine(path, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    //file delete on directory
                    System.IO.File.Delete(filePath);
                    //delete fields on database
                    string query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor =>'data',_actionType =>'remove_irrigationfile_data', _field_id => {Field_Id});";
                    DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(query);
                    DataTable dtProducerData;
                    if (DsProducerData != null && DsProducerData.Tables.Count > 0 && DsProducerData.Tables[0].Rows.Count > 0)
                    {
                        return Json(new { success = true, data = JsonConvert.SerializeObject(DsProducerData, Formatting.None) });
                    }
                    Console.WriteLine("File deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false });
        }


        [HttpGet]
        public IActionResult CheckBenchmarkByFieldIdIndOptionId(int Field_Id, int field_Option_Id)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                string query = $"select * from \"FSRCalc\".water_uploadcalculate_fsr_data(datarefcursor2 =>'data2',_actiontype => 'get_OperationYearsFromFieldId',_field_id => {Field_Id});";
                DataSet DsQueryData = _masterRepository.GetAndSelectMultipleTableItems(query);
                if (DsQueryData != null && DsQueryData.Tables.Count > 0 && DsQueryData.Tables[1].Rows.Count > 0)
                {
                    DataTable dataTable = DsQueryData.Tables[1];
                    //true -> 5,6,7 are avilable in this field
                    bool allValuesExist = new[] { 5, 6, 7 }.All(value =>
                    dataTable.AsEnumerable().Any(row => row.Field<int>("field_option_id") == value));

                    return Json(new { success = true, BenchmarkCheck = allValuesExist });
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false, Message = "data not available" });
        }

        [HttpGet]
        public IActionResult BenchmarkForAllFields()
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                string query = $"select * from \"FSRCalc\".water_get_fields_producer(datarefcursor => 'data', _ActionType=>'getallfieldsforbenchmark');";
                DataSet result = _masterRepository.GetAndSelectTableItems(query);
                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    return Json(new { success = true, data = JsonConvert.SerializeObject(result) });
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false });

        }
        [HttpGet]
        public IActionResult IsPublishFsr(int fieldId)
        {
            try
            {
                IMasterRepository _masterRepository = new MasterRepository();
                bool IsFsrPublish = true;
                string query = $"select * from \"FSRCalc\".water_fields_ref(datarefcursor => 'data',_field_id => {fieldId},_actiontype => 'isfsrpublishupdate',_isfsrpublish=>{IsFsrPublish});\r\n";
                DataSet result = _masterRepository.GetAndSelectTableItems(query);
                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    return Json(new { success = true, data = JsonConvert.SerializeObject(result) });
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return Json(new { success = false });

        }

    }
}