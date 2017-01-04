using System;
using System.Collections.Generic;
using System.Text;

namespace Parascan0
{
    class HelperFunctions
    {

        public void SendEMail(string emailSubject, string emailBody, string[] emailAttachments, string emailRecipient)
        {
            try
            {
                System.Net.Mail.MailMessage objectMessage = new System.Net.Mail.MailMessage();
                //System.Net.Mail.MailAddress objectMessageTo = new System.Net.Mail.MailAddress("mplaskow@yahoo.com");
                System.Net.Mail.MailAddress objectMessageTo;
                if (emailRecipient.Length < 8)
                {
                    new formEMail().ShowDialog();
                    objectMessageTo = new System.Net.Mail.MailAddress(ObjectsStatic.SelectedValue);
                }
                else
                {
                    objectMessageTo = new System.Net.Mail.MailAddress(emailRecipient);
                }
                objectMessage.IsBodyHtml = false;
                objectMessage.From = new System.Net.Mail.MailAddress("info@qmira.com");
                objectMessage.To.Add(objectMessageTo);
                objectMessage.Subject = emailSubject;
                foreach (string emailAttachment in emailAttachments)
                {
                    if (emailAttachment.Length > 4)
                    {
                        objectMessage.Attachments.Add(new System.Net.Mail.Attachment(emailAttachment));
                    }
                }
                objectMessage.Body = emailBody;

                System.Net.Mail.SmtpClient objectClient = new System.Net.Mail.SmtpClient("smtpout.secureserver.net");
                objectClient.Credentials = new System.Net.NetworkCredential("info@qmira.com", "holiday2$");
                objectClient.Send(objectMessage);
            }
            catch 
            {
                System.Windows.Forms.MessageBox.Show("EMail not Sent");
            }
        }

    }
}
