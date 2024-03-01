using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Data;
using InternationalWaterWebApp.Library.ViewModel;

namespace InternationalWaterWebApp.Library.ViewModel
{
    internal class CommonViewModels
    {
    }

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "The password field is required.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "The email field is required.")]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? UserRole { get; set; }
        public string UType { get; set; }

        public int? AdvisorId { get; set; }
        public bool IsActive { get; set; }
        public bool IsPassResetRequested { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsFirstLogin { get; set; }
    }

    public class Email
    {
        public string Subject { get; set; }
        public string Body { get; set; }

    }
    public class State
    {
        public int StateId { get; set; }
        public string Name { get; set; }
    }

    public class FinancialPlans
    {
        public int FinancialPlansId { get; set; }
        public string PlansName { get; set; }
    }

    public class Farm
    {
        public int FarmId { get; set; }
        public string FarmName { get; set; }
    }
    public class IrrigationZoneTable
    {
        public int Field_Id { get; set; }
        public string Zone_Name { get; set; }
        public bool Zone_Irrigated { get; set; }
        public int Year { get; set; }
        public string Crop_Name { get; set; }
        public string Filename { get; set; }
    }

    public class MyFields
    {
        public int Field_Id { get; set; }
        [Required(ErrorMessage = "The First Name field is required.")]
        public string Field_Name { get; set; }
        [Required(ErrorMessage = "The Street Address field is required.")]
        public string Street_Address { get; set; }
        [Required(ErrorMessage = "The City field is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "The State field is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "The Zip field is required.")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "The County field is required.")]
        public string County { get; set; }

        [Required(ErrorMessage = "The Township field is required.")]
        public string Township { get; set; }

        [Required(ErrorMessage = "The Range field is required.")]
        public string Range { get; set; }

        [Required(ErrorMessage = "The Section field is required.")]
        public string Section { get; set; }

        [Required(ErrorMessage = "The Section Half field is required.")]
        public string SectionHalf { get; set; }

        [Required(ErrorMessage = "The Section Quarter field is required.")]
        public string Section_Quarter { get; set; }

        [Required(ErrorMessage = "The Farm field is required.")]
        public int Farm_Id { get; set; }
        public int ProducerId { get; set; }

    }

    public class Advisors
    {
        public int? AdvisorsId { get; set; }

        [Required(ErrorMessage = "The First Name field is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Last Name field is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public int? LoginId { get; set; }

        [Required(ErrorMessage = "The Address1 field is required.")]
        public string Address1 { get; set; }

        public string? Address2 { get; set; }

        [Required(ErrorMessage = "The City field is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "The State field is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "The Zip Code field is required.")]
        //[RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zip")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "The Phone  Mobile field is required.")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number")]
        public string PhoneMobile { get; set; }
    }

    public class Producers
    {
        public int? ProducerId { get; set; }

        [Required(ErrorMessage = "The First Name field is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Last Name field is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public int? LoginId { get; set; }

        [Required(ErrorMessage = "The Street Address field is required.")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "The City field is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "The State field is required.")]
        public string StateName { get; set; }

        [Required(ErrorMessage = "The Zip Code field is required.")]
        //[RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zip")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "The Phone Number field is required.")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "The Advisor field is required.")]
        public int AgronomistId { get; set; }
        public int AdvisorId { get; set; }
        public int Field_Id { get; set; }
    }

    public class Management
    {
        public int? ManagementId { get; set; }
        public string? ManagementName { get; set; }

    }

    public class ZoneDetails
    {
        public double Acres { get; set; }
        public double ExpectedYield { get; set; }
        public double OperatorShare { get; set; }
        public double CommoditySalePrice { get; set; }
        public double OtherProductReturn { get; set; }
        public double HedgeGainLoss { get; set; }
        public double CropInsurancePayments { get; set; }
        public double StewardshipPayments { get; set; }
        public double GovernmentPayments { get; set; }
        public double Seed { get; set; }
        public double Nitrogen { get; set; }
        public double Phosphorus { get; set; }
        public double Potash { get; set; }
        public double Sulfur { get; set; }
        public double Lime { get; set; }
        public double SeedOther { get; set; }
        public double Herbicide { get; set; }
        public double Fungicide { get; set; }
        public double Insecticide { get; set; }
        public double CropChemicalOther { get; set; }
        public double Miscellaneous { get; set; }
        public double CoverCrop { get; set; }
        public double Insurance { get; set; }
        public double DryingPropane { get; set; }
        public double EquipmentFuel { get; set; }
        public double Machinery { get; set; }
        public double Buildings { get; set; }
        public double RepairMadoubleenanceOther { get; set; }
        public double Driver { get; set; }
        public double Equipment { get; set; }
        public double CustomApplicationt { get; set; }
        public double CustomHireOther { get; set; }
        public double HiredLabor { get; set; }
        public double Repairs { get; set; }
        public double FuelElectricity { get; set; }
        public double LeasesMachinery { get; set; }
        public double LeasesBuildings { get; set; }
        public double LandRent { get; set; }
        public double StewardshipImplementation { get; set; }
        public double Storage { get; set; }
        public double Supplies { get; set; }
        public double Utilities { get; set; }
        public double FreightTrucking { get; set; }
        public double Marketing { get; set; }
        public double InterestOperating { get; set; }
        public double OtherCosts { get; set; }
        public double OverheadExpensesDriver { get; set; }
        public double OverheadExpensesEquipment { get; set; }
        public double OverheadExpensesCustomApplication { get; set; }
        public double OverheadExpensesOther { get; set; }
        public double OtherExpensesHiredLabor { get; set; }
        public double OtherExpensesMachineryLease { get; set; }
        public double OtherExpensesBuildingLeases { get; set; }
        public double OtherExpensesFarmInsurance { get; set; }
        public double OtherExpensesUtilities { get; set; }
        public double OtherExpensesDuesProfessionalFees { get; set; }
        public double OtherExpensesInterest { get; set; }
        public double OtherExpensesMachineBuildingDepreciation { get; set; }
        public double OtherExpensesRealEstateTaxes { get; set; }
        public double OtherExpensesOtherOverheadExpenses { get; set; }
        public double FinancingIncomeTaxes { get; set; }
        public double FinancingOwnerWithdrawal { get; set; }
        public double FinancingPrincipalPayment { get; set; }



    }

    public class Financial
    {
        public string field_option_name { get; set; }
        public int field_option_id { get; set; }
        
        public List<FinancialDetailsList> FinancialDetailsList { get; set; }
    }

    public class FinancialDetailsList
    {
        public string? year_cropname { get; set; }
        public int? year { get; set; }
        public int? field_option_id { get; set; }
        public int? field_id { get; set; }
        public int? crop1_id { get; set; }
        public double? cashprice { get; set; }
        public double? expectedyield { get; set; }
        public double? TotalGrossRevenue { get; set; }
        public double? TotDirectExp { get; set; }
        public double? TotOverheadExp { get; set; }
        public double? TotExpense { get; set; }
        public double? BrkEvenBushel { get; set; }
        public double? NetReturnNoGovt { get; set; }
        public double? RegionalBenchmark { get; set; }
        public double? BenchmarkTotalExpenses { get; set; }
    }

    public class Zone
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public List<ZoneYearDetails> ZoneYearDetails { get; set; }
        public int FieldOptionId{ get; set; }
        public int ZydId { get; set; }
    }

    public class ZoneYearDetails
    {
        public int Year { get; set; }
        public int Field_Id { get; set; }
        public int ZydId { get; set; }
        public int FieldOptionId { get; set; }
        public string Field_Name { get; set; }
        
        public int Management_Zone_Id { get; set; }

        public string CropName { get; set; }
        public string TillageName { get; set; }
        public string Yield { get; set; }

        [Required(ErrorMessage = "The Acres field is required.")]
        public double? Acres { get; set; } //Gross Revenue
        [Required(ErrorMessage = "The Expected Yield field is required.")]
        public double? ExpectedYield { get; set; }
        [Required(ErrorMessage = "The Operator Sharefield field is required.")]
        public double? OperatorShare { get; set; }
        [Required(ErrorMessage = "The Commodity Sale Price field  is required.")]
        public double? CommoditySalePrice { get; set; }
        [Required(ErrorMessage = "The Other Product Return field is required.")]
        public double? OtherProductReturn { get; set; }
        public double? TotalProductReturn { get; set; }
        //[Required(ErrorMessage = "The Hedge Gain Loss field is required.")]
        public double? HedgeGainLoss { get; set; }
        //[Required(ErrorMessage = "The  Crop Insurance Payments field is required.")]
        public double ?CropInsurancePayments { get; set; }
        public double ?StewardshipPayments { get; set; }
        //[Required(ErrorMessage = "The  Government Payments field is required.")]
        public double ?GovernmentPayments { get; set; }
        public double ?TotalGrossRevenue { get; set; }
        //[Required(ErrorMessage = "The  Seed  is required.")]
        public double ?Seed { get; set; } // Direct Expenses
        public double ?Nitrogen { get; set; }
        public double ?Phosphorus { get; set; }
        public double ?Potash { get; set; }
        public double ?Sulfur { get; set; }
        public double ?Lime { get; set; }
        //[Required(ErrorMessage = "The  Other field  is required.")]
        public double ?SeedOther { get; set; }
        
        public double ?TotalFertilizerExpenses { get; set; }
        public double ?Herbicide { get; set; }
        public double ?Fungicide { get; set; }
        public double ?Insecticide { get; set; }

        //[Required(ErrorMessage = "The  Other Chemicals field  is required.")]
        public double ?CropChemicalOther { get; set; }
       
        public double ?TotalCropChemicalExpenses { get; set; }
        public double ?Miscellaneous { get; set; }
        //[Required(ErrorMessage = "The  Other Cover Crop field  is required.")]
        public double ?CoverCrop { get; set; }
        //[Required(ErrorMessage = "The  Insurance  field  is required.")]
        public double ?Insurance { get; set; }
        //[Required(ErrorMessage = "The  Other  Drying Propane  is required.")]
        public double ?DryingPropane { get; set; }
        //[Required(ErrorMessage = "The  Equipment Fuel  field  is required.")]
        public double ?EquipmentFuel { get; set; }
        public double ?Machinery { get; set; }
        public double ?Buildings { get; set; }
        //[Required(ErrorMessage = "The  Other  Repair Maintenance field  is required.")]
        public double ?RepairMaintenanceOther { get; set; }
       
        public double ?TotalRepairMaintenance { get; set; }
        public double ?Driver { get; set; }
        public double ?Equipment { get; set; }
        public double ?CustomApplicationt { get; set; }

        //[Required(ErrorMessage = "The  Other Custom Hire (Direct) field  is required.")]
        public double ?CustomHireOther { get; set; }
        
        public double ?TotalCustomHire { get; set; }
        //[Required(ErrorMessage = "The Hired Labor  field  is required.")]
        public double ?HiredLabor { get; set; }
        public double ?Repairs { get; set; }
        public double ?FuelElectricity { get; set; }
        //[Required(ErrorMessage = "The Machinery field  is required.")]
        public double ?LeasesMachinery { get; set; }
        //[Required(ErrorMessage = "The Buildings  field  is required.")]
        public double ?LeasesBuildings { get; set; }
        //[Required(ErrorMessage = "The Landrent field  is required.")]
        public double ?LandRent { get; set; }
        public double ?StewardshipImplementation { get; set; }
        //[Required(ErrorMessage = "The Storage  field  is required.")]
        public double ?Storage { get; set; }
        //[Required(ErrorMessage = "The Supplies field  is required.")]
        public double ?Supplies { get; set; }
        //[Required(ErrorMessage = "The Utilities field  is required.")]
        public double ?Utilities { get; set; }
        //[Required(ErrorMessage = "The Freight Trucking  field  is required.")]
        public double ?FreightTrucking { get; set; }
        //[Required(ErrorMessage = "The Marketing  field  is required.")]
        public double ?Marketing { get; set; }
        //[Required(ErrorMessage = "The Interest Operating  field  is required.")]
        public double ?InterestOperating { get; set; }
        //[Required(ErrorMessage = "The Other Costs  field  is required.")]
        public double ?OtherCosts { get; set; }
        public double ?TotalDirectExpense { get; set; }
        public double ?ReturnOverDirectExpenseperAcre { get; set; }
        public double ?OverheadExpensesDriver { get; set; } //Overhead Expenses
        public double ?OverheadExpensesEquipment { get; set; }
        public double ?OverheadExpensesCustomApplication { get; set; }
        //[Required(ErrorMessage = "The other custom hire  field  is required.")]
        public double ?OverheadExpensesOther { get; set; }
        
        public double ?OverheadExpensesTotalCustomHire { get; set; }

        //[Required(ErrorMessage = "The Custom hire  field  is required.")]
        public double ?OtherExpensesHiredLabor { get; set; }

        //[Required(ErrorMessage = "The Machinery Lease  field  is required.")]
        public double ?OtherExpensesMachineryLease { get; set; }

        //[Required(ErrorMessage = "The Building Leases field  is required.")]
        public double ?OtherExpensesBuildingLeases { get; set; }

        //[Required(ErrorMessage = "The Farm Insurance field  is required.")]
        public double ?OtherExpensesFarmInsurance { get; set; }

        //[Required(ErrorMessage = "The Utilities  field  is required.")]
        public double ?OtherExpensesUtilities { get; set; }
        //[Required(ErrorMessage = "The other custom hire  field  is required.")]
        public double ?OtherExpensesDuesProfessionalFees { get; set; }

        //[Required(ErrorMessage = "The Interestield  is required.")]
        public double ?OtherExpensesInterest { get; set; }

        //[Required(ErrorMessage = "The Machine Building Depreciation  field  is required.")]
        public double ?OtherExpensesMachineBuildingDepreciation { get; set; }

        //[Required(ErrorMessage = "The Real Estate Taxes field  is required.")]
        public double ?OtherExpensesRealEstateTaxes { get; set; }
        //[Required(ErrorMessage = "The Other Overhead Expenses field  is required.")]
        public double ?OtherExpensesOtherOverheadExpenses { get; set; }
        public double ?TotalOverheadExpenses { get; set; }

        //[Required(ErrorMessage = "The Financing Income Taxes field  is required.")]
        public double ?FinancingIncomeTaxes { get; set; } //Financing

        //[Required(ErrorMessage = "The Financing OwnerWithdrawal field  is required.")]
        public double ?FinancingOwnerWithdrawal { get; set; }

        //[Required(ErrorMessage = "The Financing Principal Payment field  is required.")]
        public double ?FinancingPrincipalPayment { get; set; }
        public double ?TotalFinancing { get; set; }
        public string ? DataQualityGrade { get; set; }
        public bool IsValuesChanged { get; set; }


    }

    public class ChartData
    {
        public double Acres { get; set; }
        // graph-1
        public double TotalDirectCost { get; set; }
        public double TotalInDirectCost { get; set; }
        public double TotalProductionCost { get; set; }
        // Graph-2
        public double Fartilizer { get; set; }
        public double Crop_Chemicals { get; set; }
        public double Other_Crop { get; set; }
        public double Energy { get; set; }
        public double Repair_Maintenance { get; set; }
        public double Custom_Hire { get; set; }
        public double Hired_Labor { get; set; }
        public double Irrigation { get; set; }
        public double Leases { get; set; }
        public double Other { get; set; }
        // graph-3
        public double Labor { get; set; }
        public double Farm_insurance { get; set; }
        public double Dues_Fees { get; set; }
        public double Utilities { get; set; }
        public double Interest { get; set; }
        public double Machine_bldg_depreciation { get; set; }
        public double Real_estate_taxes { get; set; }
        public double Miscellaneous { get; set; }

        //NetReturn and Repayment box
        public double BrkEvenBushelNetreturn { get; set; }
        public double BrkEvenYieldNetreturn { get; set; }
        public double BrkEvenBushelRepayment { get; set; }
        public double BrkEvenYieldRepayment { get; set; }
        public double NetReturn { get; set; }
        public double OtherExpensesHiredLabor { get; set; }
        public double OtherExpensesMachineryLease { get; set; }
        public double OtherExpensesOtherOverheadExpenses { get; set; }
        public double RegionalBenchmark { get; set; }
    }


    public class ChartDataWithZydid
    {
        public List<ChartData>? chartdata { get; set; }
        public List<ChartData>? chartzydiddata { get; set; }


    }
    public class ExportData
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public int Management_Zone_Id { get; set; }
        public string Management_Zone { get; set; }
        public int Year { get; set; }
        public string OptionName { get; set; }
        public string CropName { get; set; }
        public string TillageMethod { get; set; }
        public string YieldClass { get; set; }
        public bool boundariesexist { get; set; }
        public string topologycheck { get; set; }
        public string topologycolor { get; set; }
        public bool dataentered { get; set; }
        public string exportready { get; set; }
        //new
        public int Field_Option_Id { get; set; }
        public string Crop1_Name { get; set; }
        public string Crop1_Rusle2_Lookup_Value { get; set; }
        public string Primary_Tillage { get; set; }
        public string Primary_Tillage_Rusle2_Lookup { get; set; }
        public string Secondary_Tillage { get; set; }
        public string Secondary_Tillage_Rusle2_Lookup { get; set; }
        public string Net_C_Factor { get; set; }
        public string CN3 { get; set; }


    }

    public class Rusle2ExcelData
    {
        public string FieldID { get; set; }
        public string ProducerID { get; set; }
        public string M_Zone_ID { get; set; }
        public string Farmed { get; set; }
        public string YieldClass { get; set; }
        public string PerContrib { get; set; }
        public string Year { get; set; }
        public string Option_ID { get; set; }
        public string CropType { get; set; }
        public string Tillage { get; set; }
        public string Acres { get; set; }
        public string Contrib_ac { get; set; }
        public string NonCont_ac { get; set; }
        public string R2_Manage { get; set; }
        public string CMZ { get; set; }
        public string MUKEY { get; set; }
        public string Soil_MUNAME { get; set; }
        public string HydrologicSoilGroup { get; set; }
        public string County_Name { get; set; }
        public string SlopePercent { get; set; }
        public string Field_Sediment_Delivery_Ratio { get; set; }
        public string Field_TP_Delivery_Ratio { get; set; }
        public string TP_Mass_leaving_Landscape_in_lbs_yr { get; set; }
        //public string Sediment_Mass_leaving_landscape_in_ton_ac_yr { get; set; }
        //public string Sediment_Priority_Resource_Point_Delivery_Ratio { get; set; }
        //public string Total_P_Priority_Resource_Point_Delivery_Ratio { get; set; }
    }

    public class Precipitation
    {
        public string FileName { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Precipitation_day { get; set; }
        public int Antecedent_rainfall { get; set; }
        public int Temperature { get; set; }
        public int Field_id { get; set; }
    }
    public class FsrViewHostoryChartData
    {
        public int FieldYear { get; set; }
        public string FieldYear_Crop { get; set; }
        public string Crop { get; set; }
        public double FsrVal { get; set; }
    }

    public class SoilIndiciesChartData
    {
        public string ChartType { get; set; }
        public DataTable? SoWaErBeIndata { get; set; }
        public DataTable? SoFoDeInAgdata { get; set; }
        public DataTable? SoWaErInAg { get; set; }
        public DataTable? SoWaErDeInAg { get; set; }
        public DataTable? SoReBeIn { get; set; }
        public DataTable? TpMobinag { get; set; }
        public DataTable? TPMobdeinag { get; set; }
        public DataTable? Tpexinag { get; set; }
        public DataTable? Tpexdeinag { get; set; }
        public DataTable? TPReBeln { get; set; }
        public DataTable? InBeln { get; set; }
        public DataTable? InDeInAg { get; set; }
        public DataTable? RuBeIn { get; set; }
        public DataTable? RuDeInAg { get; set; }
        public DataTable? IrrUseEffBeIn { get; set; }
        public DataTable? Resource_sed_goal_index { get; set; }
        public DataTable? Sed_goal_feasibility_index { get; set; }
        public DataTable? Resource_tp_goal_index { get; set; }
        public DataTable? Tp_goal_feasibility_index { get; set; }
        public Dictionary<string, string>? NitrogenFertilizer { get; set; }
        public Dictionary<string, string>? Option1NitrogenFertilizer { get; set; }
        public Dictionary<string, string>? Option2NitrogenFertilizer { get; set; }
        public Dictionary<string, string>? AlfalfaNitrogenFertilizer { get; set; }
        public Dictionary<string, string>? MoldNitrogenFertilizer { get; set; }
        public Dictionary<string, string>? PhosphorusFertilizer  { get; set; }
        public Dictionary<string, string>? Option1PhosphorusFertilizer { get; set; }
        public Dictionary<string, string>? Option2PhosphorusFertilizer { get; set; }
        public Dictionary<string, string>? AlfalfaPhosphorusFertilizer { get; set; }
        public Dictionary<string, string>? MoldPhosphorusFertilizer { get; set; }
        public Dictionary<string, string>? SoilLossfromField { get; set; }
        public Dictionary<string, string>? AlfalfaSoilLossfromField { get; set; }
        public Dictionary<string, string>? MoldSoilLossfromField { get; set; }
        public Dictionary<string, string>? Option1SoilLossfromField { get; set; }
        public Dictionary<string, string>? Option2SoilLossfromField { get; set; }
        public Dictionary<string, string>? PhosphorusLossfromField { get; set; }
        public Dictionary<string, string>? AlfalfaPhosphorusLossfromField { get; set; }
        public Dictionary<string, string>? MoldPhosphorusLossfromField { get; set; }
        public Dictionary<string, string>? Option1PhosphorusLossfromField { get; set; }
        public Dictionary<string, string>? Option2PhosphorusLossfromField { get; set; }
        public bool IrrUseEffBeInflag { get; set; }
    }

    public class FarmingFinancialData
    {
        public double? ExpectedYield { get; set; }
        public double? Acres { get; set; }
        public double? OperatorShare { get; set; }
        public double? CommoditySalePrice { get; set; }
        public double? OtherProductReturn { get; set; }
        public double? TotalProductReturn { get; set; }
        public double? HedgeGainLoss { get; set; }
        public double? CropInsurancePayments { get; set; }
        public double? StewardshipPayments { get; set; }
        public double? GovernmentPayments { get; set; }
        public double? TotalGrossRevenue { get; set; }
        public double? Seed { get; set; }
        public double? Nitrogen { get; set; }
        public double? Phosphorus { get; set; }
        public double? Potash { get; set; }
        public double? Sulfur { get; set; }
        public double? Lime { get; set; }
        public double? SeedOther { get; set; }
        public double? TotalFertilizerExpenses { get; set; }
        public double? Herbicide { get; set; }
        public double? Fungicide { get; set; }
        public double? Insecticide { get; set; }
        public double? CropChemicalOther { get; set; }
        public double? TotalCropChemicalExpenses { get; set; }
        public double? Miscellaneous { get; set; }
        public double? CoverCrop { get; set; }
        public double? Insurance { get; set; }
        public double? DryingPropane { get; set; }
        public double? EquipmentFuel { get; set; }
        public double? Machinery { get; set; }
        public double? Buildings { get; set; }
        public double? RepairMaintenanceOther { get; set; }
        public double? TotalRepairMaintenance { get; set; }
        public double? Driver { get; set; }
        public double? Equipment { get; set; }
        public double? CustomApplicationt { get; set; }
        public double? CustomHireOther { get; set; }
        public double? TotalCustomHire { get; set; }
        public double? HiredLabor { get; set; }
        public double? Repairs { get; set; }
        public double? FuelElectricity { get; set; }
        public double? LeasesMachinery { get; set; }
        public double? LeasesBuildings { get; set; }
        public double? LandRent { get; set; }
        public double? StewardshipImplementation { get; set; }
        public double? Storage { get; set; }
        public double? Supplies { get; set; }
        public double? Utilities { get; set; }
        public double? FreightTrucking { get; set; }
        public double? Marketing { get; set; }
        public double? InterestOperating { get; set; }
        public double? OtherCosts { get; set; }
        public double? TotalDirectExpense { get; set; }
        public double? OverheadExpensesDriver { get; set; }
        public double? OverheadExpensesEquipment { get; set; }
        public double? OverheadExpensesCustomApplication { get; set; }
        public double? OverheadExpensesOther { get; set; }
        public double? OverheadExpensesTotalCustomHire { get; set; }
        public double? OtherExpensesHiredLabor { get; set; }
        public double? OtherExpensesMachineryLease { get; set; }
        public double? OtherExpensesBuildingLeases { get; set; }
        public double? OtherExpensesFarmInsurance { get; set; }
        public double? OtherExpensesUtilities { get; set; }
        public double? OtherExpensesDuesProfessionalFees { get; set; }
        public double? OtherExpensesInterest { get; set; }
        public double? OtherExpensesMachineBuildingDepreciation { get; set; }
        public double? OtherExpensesRealEstateTaxes { get; set; }
        public double? OtherExpensesOtherOverheadExpenses { get; set; }
        public double? TotalOverheadExpenses { get; set; }
        public double? FinancingIncomeTaxes { get; set; }
        public double? FinancingOwnerWithdrawal { get; set; }
        public double? FinancingPrincipalPayment { get; set; }
        public double? ReturnOverDirectExpenseperAcre { get; set;}
        public double? TotalFinancing { get; set; }
    }

    public class Rusle2Data
    {
        public string? NET_C_FACTOR { get; set; }
        public string? SLOPE_DELIVERY { get; set; }
        public string? SLOPE_DETACH { get; set; }
        public string? NET_K_FACTOR { get; set; }
        public string? SOIL_PTR { get; set; }
        public string? MAN_BASE_PTR { get; set; }
    }
}


