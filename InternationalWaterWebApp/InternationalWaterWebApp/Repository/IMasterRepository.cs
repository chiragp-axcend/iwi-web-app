using System.Data;

namespace InternationalWaterWebApp.Repository
{
    public interface IMasterRepository
    {
        DataSet GetAndSelectTableItems(string sqlQuery);
        DataSet GetAndSelectTableItemsWithoutCursor(string sqlQuery);        
        long? InsertUpdate(string sqlQuery);
        long? InsertUpdate_EmailNotification(string spName);
        bool SendMail(string emailTo, string subject, string body, string emailFrom);
        bool SendMailWithCC(string emailTo, string cc, string subject, string body);
        bool SendMailWithAttachment(string emailTo, string subject, string body, string attachmentPath);
        DataSet GetAndSelectTableItemsRefcursor(string sqlQuery);
        DataSet GetAndSelectMultipleTableItems(string sqlQuery);
    }
} 