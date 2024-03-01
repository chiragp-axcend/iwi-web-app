using InternationalWaterWebApp.Library.Common;
using InternationalWaterWebApp.Library.DatabaseConnection;
using System.Data;
using System.Net.Mail;
using System.Net;

namespace InternationalWaterWebApp.Repository
{
    public class MasterRepository : IMasterRepository
    {
        public static string SMTPServer = DefaultValueFromWebConfig.SMTPSERVER;
        public static string SMTPUserName = DefaultValueFromWebConfig.SMTPUsername;
        public static string SMTPPassword = DefaultValueFromWebConfig.SMTPPassword;
        public static string SMTPSSL = DefaultValueFromWebConfig.SMTPSSL;
        public static string SMTPPort = DefaultValueFromWebConfig.SMTPPort;
        public static string EmailFrom = DefaultValueFromWebConfig.EmailFrom;

        public DataSet GetAndSelectTableItems(string sqlQuery)
        {
            DataSet result = null;
            try
            {
                result = WaterDBContext.GetData(sqlQuery);
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return result;
        }
        public DataSet GetAndSelectMultipleTableItems(string sqlQuery)
        {
            DataSet result = null;
            try
            {
                result = WaterDBContext.GetDataWithMultipleTables(sqlQuery);
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return result;
        }

        public DataSet GetAndSelectTableItemsWithoutCursor(string sqlQuery)
        {
            DataSet result = null;
            try
            {
                result = WaterDBContext.GetDataWithoutCursor(sqlQuery);
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return result;
        }

        public DataSet GetAndSelectTableItemsRefcursor(string sqlQuery)
        {
            DataSet result = null;
            try
            {
                result = WaterDBContext.GetSelectItemDataset(sqlQuery);
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return result;
        }

        public long? InsertUpdate(string sqlQuery)
        {
            long? result = null;
            try
            {
                result = WaterDBContext.InsertUpdateData(sqlQuery);

            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return result;
        }

        public long? InsertUpdate_EmailNotification(string spName)
        {
            long? result = null;
            try
            {
                result = WaterDBContext.InsertUpdateData(spName);
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return result;
        }

        public bool SendMail(string emailTo, string subject, string body , string emailFrom)
        {
            bool result = false;
            try
            {
                MailMessage message = new MailMessage();
                foreach (var address in emailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(address);
                }

                message.From = new MailAddress(EmailFrom);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(SMTPUserName, SMTPPassword);
                client.Port = Convert.ToInt32(SMTPPort);
                client.Host = SMTPServer;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.EnableSsl = true;

                client.Send(message);
                result = true;
                Console.WriteLine("Mail Sent Successfully.");
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                result = false;
            }
            return result;
        }

        public bool SendMailWithCC(string emailTo, string cc, string subject, string body)
        {
            bool result = false;
            try
            {
                MailMessage message = new MailMessage();
                foreach (var address in emailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(address);
                }
                foreach (var cce in cc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.CC.Add(cce);
                }

                message.From = new MailAddress(EmailFrom);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(SMTPUserName, SMTPPassword);
                client.Port = 587;
                client.Host = "smtp.office365.com";
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;

                client.Send(message);
                result = true;
                Console.WriteLine("Mail Sent Successfully.");
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                result = false;
            }
            return result;
        }

        public bool SendMailWithAttachment(string emailTo, string subject, string body, string attachmentPath)
        {
            bool result = false;
            try
            {
                MailMessage message = new MailMessage();
                foreach (var address in emailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(address);
                }

                message.From = new MailAddress(EmailFrom);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                if (!string.IsNullOrWhiteSpace(attachmentPath))
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(attachmentPath);
                    message.Attachments.Add(attachment);
                }

                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(SMTPUserName, SMTPPassword);
                client.Port = Convert.ToInt32(SMTPPort);
                client.Host = SMTPServer;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;

                client.Send(message);
                result = true;
                Console.WriteLine("Mail Sent Successfully.");
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                result = false;
            }
            return result;
        }
    }
}