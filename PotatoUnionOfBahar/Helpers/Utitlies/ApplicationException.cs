using System;
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
    [System.Serializable]
    public class ApplicationException : System.ApplicationException
    {
        //تعریف پراپرتی نامبر
        public int Number { get; set; }

        /// <summary>
        /// سازنده کلاس با یک ورودی رشته
        /// </summary>
        /// <param name="message"></param>
        public ApplicationException(string message)
            : base(message)
        {
            Number = -1;
        }

        /// <summary>
        /// سازنده با دو ورودی رشته و نامبر
        /// </summary>
        /// <param name="message"></param>
        /// <param name="number"></param>
        public ApplicationException(string message, int number)
            : base(message)
        {
            Number = number;
        }
    }
}