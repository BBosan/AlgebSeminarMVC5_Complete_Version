using Se.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Se.Other
{
    public class MojaDatumValidacija2Attribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "{0} Mora Biti < Od Danasnjeg";
        public MojaDatumValidacija2Attribute() : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(_defaultErrorMessage, name);
            //return base.FormatErrorMessage(name);
        }

        public override bool IsValid(object value)
        {
            DateTime dateTime = Convert.ToDateTime(value);
            return dateTime <= DateTime.Now;
        }



    }
}