using System;
using System.ComponentModel.DataAnnotations;

namespace Se.Other
{
    public class DatumValidacijaAttribute : ValidationAttribute
    {
        public DatumValidacijaAttribute(){} //konstruktor 1
        public bool _rodjendan=true; 
        public DatumValidacijaAttribute(bool rodjendan) //konstruktor 2
        {
            _rodjendan = rodjendan;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                string strValue = value.ToString();
                DateTime datum;
                if (DateTime.TryParse(strValue, out datum))
                {
                    if (!_rodjendan)
                    {
                        if (datum.Year < DateTime.Today.AddYears(-10).Year)
                        {
                            return new ValidationResult("Datum Ne Smije Biti Manji Od 10 Godina!");
                        }
                        else
                        {
                             return ValidationResult.Success;
                        }
                    }
                    else
                    {
                        if (datum > DateTime.Today)
                        {
                            throw new FormatException();
                        }
                        else if (datum == DateTime.Today)
                        {
                            return new ValidationResult("Danas Ste Se Rodili?");
                        }
                        else if (datum.Starost() < 18)
                        {
                            return new ValidationResult("Mladji Od 18?");
                        }
                        else if (datum.Starost() >= 110)
                        {
                            return new ValidationResult($"{datum.Starost()} godina, orly?");
                        }
                        else
                        {
                            return ValidationResult.Success;
                        }
                    }

                }
                else
                {
                    throw new FormatException();
                }

            }
            catch (Exception)
            {
                return new ValidationResult($"Pogresan Format! (Unesite Tocan {validationContext.DisplayName})");
            }


        }


    }
}