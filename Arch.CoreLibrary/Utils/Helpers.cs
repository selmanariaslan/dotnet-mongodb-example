using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Utils
{
    public static class Helpers
    {
        public static bool SendMail(
            NetworkCredential credential,
            MailAddress from,
            string host,
            List<string> toList,
            List<string> ccList,
            List<string> bccList,
            string subject,
            string body)
        {
            using (var message = new MailMessage())
            {
                using (var smtpClient = new SmtpClient())
                {
                    try
                    {
                        smtpClient.Credentials = credential;
                        smtpClient.EnableSsl = false;
                        smtpClient.Port = 587;
                        smtpClient.Host = host;

                        if (toList?.Count > 0)
                        {
                            foreach (var to in toList)
                            {
                                message.To.Add(to);
                            }
                        }

                        if (ccList?.Count > 0)
                        {
                            foreach (var cc in ccList)
                            {
                                message.CC.Add(cc);
                            }
                        }

                        if (bccList?.Count > 0)
                        {
                            foreach (var bcc in bccList)
                            {
                                message.Bcc.Add(bcc);
                            }
                        }

                        message.From = from;
                        message.Subject = subject;
                        message.SubjectEncoding = Encoding.UTF8;
                        message.BodyEncoding = Encoding.UTF8;
                        message.IsBodyHtml = true;
                        message.Body = body;

                        smtpClient.Send(message);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }



        public static string GetIP()
        {
            try
            {
                IHttpContextAccessor _accessor = new HttpContextAccessor();
                string str = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                if (str == "::1")
                {
                    str = "192.168.1.1";
                }
                return str;
            }
            catch
            {
                return "";
            }
        }


        public static string ToQueryString(this object request, string separator = "&")
        {
            if (request == null)
                throw new ArgumentNullException("request");

            // Get all properties on the object
            var properties = request.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(request, null) != null)
                .ToDictionary(x => x.Name, x => x.GetValue(request, null));

            // Get names for all IEnumerable properties (excl. string)
            var propertyNames = properties
                .Where(x => !(x.Value is string) && x.Value is System.Collections.IEnumerable)
                .Select(x => x.Key)
                .ToList();

            // Concat all IEnumerable properties into a comma separated string
            foreach (var key in propertyNames)
            {
                var valueType = properties[key].GetType();
                var valueElemType = valueType.IsGenericType
                                        ? valueType.GetGenericArguments()[0]
                                        : valueType.GetElementType();
                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                {
                    var enumerable = properties[key] as System.Collections.IEnumerable;
                    properties[key] = string.Join(separator, enumerable.Cast<object>());
                }
            }

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&", properties
                .Select(x => string.Concat(
                    Uri.EscapeDataString(x.Key), "=",
                    Uri.EscapeDataString(x.Value.ToString()))));
        }
    }
}
