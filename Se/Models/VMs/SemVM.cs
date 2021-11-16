using Se.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Se.Models
{
    public class SemVM
    {
        public SemVM()
        {
            this.DatumString = DateTime.Now.ToString("yyyy-MM-dd");
        }

        public SemVM(Seminari semOrigi)
        {
            this.id = semOrigi.id;
            this.Naziv = semOrigi.Naziv;
            this.Opis = semOrigi.Opis;
            this.PopunjenDaNe = semOrigi.PopunjenDaNe;
            this.Predbiljezba = semOrigi.Predbiljezba;
            this.Datum = semOrigi.Datum;
            this.DatumString = semOrigi.Datum.ToString("yyyy-MM-dd");
        }

        public int id { get; set; }

        [Obavezno]
        public string Naziv { get; set; }
        [Obavezno]
        public string Opis { get; set; }

        public DateTime Datum { get; set; }
        public string DatumDisplayString => this.Datum.ToString("MMM.dd.yyyy");

        [DataType(DataType.Date)]
        [DatumValidacija(rodjendan: false)]
        [Obavezno]
        [Display(Name = "Datum")]
        public string DatumString { get; set; }

        [Display(Name = "Status")]
        public bool PopunjenDaNe { get; set; }
        public string PopunjenDaNeText => this.PopunjenDaNe == false ? "Otvoren" : "Zatvoren";

        public virtual ICollection<Predbiljezba> Predbiljezba { get; set; }
        [Display(Name = "Broj Upisa")]
        public string BrojPredbiljezbi => this.Predbiljezba.Count.Equals(0) ? "Nema" : this.Predbiljezba.Count.ToString();

    }
}