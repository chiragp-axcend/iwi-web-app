using InternationalWaterWebApp.Library.Common;
using InternationalWaterWebApp.Library.ViewModel;
using InternationalWaterWebApp.Models;
using InternationalWaterWebApp.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace InternationalWaterWebApp.Controllers
{
    [AuthorizationCS((int)UserTypeEnum.Agronomist)]
    public class AdvisorController : BaseController
    {
        private CommonMethods commonMethods;
        private CommonController _CommonController;
        private EncryptDecryptData encryptDecryptData;

        public AdvisorController()
        {
            commonMethods = new CommonMethods();
            _CommonController = new CommonController();
            encryptDecryptData = new EncryptDecryptData();
        }

        public IActionResult Landing(string? id,int? field)
        {
            ViewBag.fieldtab = field;
            
            IMasterRepository _masterRepository = new MasterRepository();
            object[] userParams = { "_ActionType", "_Advisor_Id"};
            object[] userFieldValues = { "GetAllFieldsForAdvisor" , LoginUserId };
            object[] userPrdoucerValues = { "GetAllProducersForAdvisor" , LoginUserId };
            DataSet DsFieldData = null;
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.fieldtab = 1;
                string producerId = encryptDecryptData.Decryptdata(id);

                object[] userParams1 = { "_ActionType", "_producer_id" };
                object[] userValues = { "GetFieldByProducerId", producerId };
                string query = $"SELECT * from  \"FSRCalc\".water_producer_ref (datarefcursor => 'data',{userParams1[0]} => '{userValues[0]}',{userParams1[1]} => {userValues[1]});";
                DsFieldData = _masterRepository.GetAndSelectTableItems(query);
                ViewBag.Field = DsFieldData;
            }
            else
            {
                string QueryField = $"SELECT * from \"FSRCalc\".water_advisorfield_ref (datarefcursor => 'data',{userParams[0]} => '{userFieldValues[0]}', {userParams[1]} => '{userFieldValues[1]}');";
                DsFieldData = _masterRepository.GetAndSelectTableItems(QueryField);
                ViewBag.Field = DsFieldData; // fields data

            }
                string QueryProducer = $"SELECT * from \"FSRCalc\".water_advisorfield_ref (datarefcursor => 'data',{userParams[0]} => '{userPrdoucerValues[0]}', {userParams[1]} => '{userPrdoucerValues[1]}');";
                DataSet DsProducerData = _masterRepository.GetAndSelectTableItems(QueryProducer);
                ViewBag.Producer = DsProducerData;
            return View();
        }
    }
}