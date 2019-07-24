﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MVC121.Helpers.Utilities
{
    /// <summary>
    /// در این کلاس نحوه لاگ کردن در وبسایت را بطور کامل مورد بررسی قرار داده ایم
    /// </summary>
    public static class LogHandler
    {
        #region Public Enumerations
        public enum LogTypes
        {
            Both = 0,
            LogToFile = 1,
            SendByEmail = 2
        }
        #endregion /Public Enumerations

        private static string GetErrorMessage(System.Type type, System.Collections.Hashtable parameters, System.Exception ex)
        {
            System.Text.StringBuilder oResult = new System.Text.StringBuilder();

            oResult.Append(System.Environment.NewLine);
            oResult.Append(System.Environment.NewLine);
            oResult.Append("**************************************************");

            oResult.Append(System.Environment.NewLine);
            oResult.Append("Timestamp: " + System.DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss"));

            if ((System.Web.HttpContext.Current != null) && (System.Web.HttpContext.Current.Request != null))
            {
                if (string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.UserHostAddress) == false)
                {
                    oResult.Append(System.Environment.NewLine);
                    oResult.Append("User IP: " + System.Web.HttpContext.Current.Request.UserHostAddress);
                }

                if (string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Url.AbsoluteUri) == false)
                {
                    oResult.Append(System.Environment.NewLine);
                    oResult.Append("URL: " + System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
                }

                if (string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"]) == false)
                {
                    oResult.Append(System.Environment.NewLine);
                    oResult.Append("Referer: " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"]);
                }
            }

            if (type != null)
            {
                oResult.Append(System.Environment.NewLine);
                oResult.Append("Type: " + type.ToString());
            }

            if (parameters != null)
            {
                oResult.Append(System.Environment.NewLine);
                oResult.Append(System.Environment.NewLine + "***** Parameter(s) *****");

                foreach (System.Collections.DictionaryEntry oEntry in parameters)
                {
                    oResult.Append(System.Environment.NewLine);

                    if (oEntry.Key != null)
                    {
                        oResult.Append("Key: " + oEntry.Key.ToString());

                        if (oEntry.Value == null)
                        {
                            oResult.Append(" - Value: null");
                        }
                        else
                        {
                            oResult.Append(" - Value: " + oEntry.Value.ToString());
                        }
                    }
                }

                oResult.Append(System.Environment.NewLine);
                oResult.Append("***** /Parameter(s) *****");
            }

            if ((ex != null) && (string.IsNullOrEmpty(ex.Message) == false))
            {
                oResult.Append(System.Environment.NewLine);
                oResult.Append("Exception Message: " + ex.Message);
            }

            if ((ex != null) && (string.IsNullOrEmpty(ex.StackTrace) == false))
            {
                oResult.Append(System.Environment.NewLine);
                oResult.Append("Stack Trace: " + ex.StackTrace);
            }

            oResult.Append(System.Environment.NewLine);
            oResult.Append("**************************************************");

            return (oResult.ToString());
        }

        private static bool LogToFile(string message)
        {
            bool blnResult = false;

            string strLogPathName =
                ApplicationEdit.GetValue("RootRelativeApplicationLogPathName");

            if (string.IsNullOrEmpty(strLogPathName) == false)
            {
                // **************************************************
                if ((System.Web.HttpContext.Current != null) && (System.Web.HttpContext.Current.Application != null))
                {
                    strLogPathName =
                        System.Web.HttpContext.Current.Server.MapPath(strLogPathName);

                    System.Web.HttpContext.Current.Application.Lock();
                }
                // **************************************************

                System.IO.StreamWriter oStreamWriter = null;
                try
                {
                    oStreamWriter =
                        new System.IO.StreamWriter(strLogPathName, true, System.Text.Encoding.UTF8);

                    oStreamWriter.WriteLine(message);
                    oStreamWriter.Close();

                    blnResult = true;
                }
                catch { }
                finally
                {
                    if (oStreamWriter != null)
                    {
                        oStreamWriter.Dispose();
                        oStreamWriter = null;
                    }
                }

                // **************************************************
                if ((System.Web.HttpContext.Current != null) && (System.Web.HttpContext.Current.Application != null))
                {
                    System.Web.HttpContext.Current.Application.UnLock();
                }
                // **************************************************
            }
            return (blnResult);
        }

        private static bool SendByEmail(string message)
        {
            bool blnResult = false;

            string strBody = "";

            message = MailMessage.ConvertTextForBodyEmail(message);

            strBody += "<div style='background-color: #CCCCCC;color: #0000FF;direction: ltr;font-size: 10pt;font-family: Verdana;'>";
            strBody += message;
            strBody += "</div>";

            string strSubject = "Error Notification!";

            try
            {
                MailMessage.Send(strSubject, strBody);

                blnResult = true;
            }
            finally
            {
            }

            return (blnResult);
        }

        public static void Report(System.Type type, System.Collections.Hashtable parameters, System.Exception ex)
        {
            LogTypes enmLogType = LogTypes.Both;

            try
            {
                byte bytLogType = System.Convert.ToByte(ApplicationEdit.GetValue("DefaultLogType", "0"));
                enmLogType = (LogTypes)bytLogType;
            }
            catch { }

            Report(type, parameters, ex, enmLogType);
        }

        public static void Report
            (System.Type type, System.Collections.Hashtable parameters, System.Exception ex, LogHandler.LogTypes logType)
        {
            string strErrorMessage =
                GetErrorMessage(type, parameters, ex);

            switch (logType)
            {
                case LogTypes.Both:
                    {
                        LogToFile(strErrorMessage);
                        SendByEmail(strErrorMessage);
                        break;
                    }

                case LogTypes.SendByEmail:
                    {
                        if (SendByEmail(strErrorMessage) == false)
                        {
                            LogToFile(strErrorMessage);
                        }
                        break;
                    }

                case LogTypes.LogToFile:
                    {
                        // Note: Never Write Below Codes!
                        //if (LogToFile(strErrorMessage) == false)
                        //{
                        //    SendByEmail(strErrorMessage);
                        //}

                        LogToFile(strErrorMessage);
                        break;
                    }
            }
        }
    }
}
