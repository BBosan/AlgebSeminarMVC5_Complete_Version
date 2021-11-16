using Se.Other;
using System.ComponentModel.DataAnnotations;

namespace Se.Models
{
    public class PrijavaVM
    {
        [Obavezno]
        [Display(Name = "Korisnik:")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Obavezno]
        [Display(Name = "Lozinka:")]
        public string Password { get; set; }

        [Display(Name = "Zapamti?")]
        public bool Zapamti { get; set; }
    }
}