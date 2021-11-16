using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Se.Other
{
    public class ObaveznoAttribute : RequiredAttribute, IClientValidatable
    {
        public ObaveznoAttribute()
        {
            this.ErrorMessage = "{0} Je Obavezno Polje!";
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = string.Format(this.ErrorMessage, metadata.DisplayName == null ? metadata.PropertyName : metadata.DisplayName),
                ValidationType = "required"
            };
        }
    }

}