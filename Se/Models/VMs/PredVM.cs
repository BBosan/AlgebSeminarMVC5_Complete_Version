using Se.Other;
using Se.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Se.Models
{
    public class PredVM
    {
        #region Konstruktori
        public PredVM(){}

        public PredVM(Predbiljezba predOrigi)
        {
            this.idPredbiljezbe = predOrigi.idPredbiljezbe;
            this.Ime = predOrigi.Ime;
            this.Prezime = predOrigi.Prezime;
            this.Adresa = predOrigi.Adresa;
            this.Email = predOrigi.Email;
            this.BrojTelefona = predOrigi.BrojTelefona;
            this.idSeminar = predOrigi.idSeminar;
            this.StatusDaNe = predOrigi.StatusDaNe;
            this.Seminar = predOrigi.Seminari;
            this.Datum = predOrigi.Datum;
            this.DatumString = predOrigi.Datum.ToString("yyyy-MM-dd");
        } 
        #endregion

        public int idPredbiljezbe { get; set; }

        public DateTime? Datum { get; set; }
        public string DatumDisplayString => this.Datum.Value.ToString("MMM.dd.yyyy");

        [DataType(DataType.Date)]
        [DatumValidacija]
        [Obavezno]
        [Display(Name = "Datum Rođ.")]
        public string DatumString { get; set; }

        [MinLength(2, ErrorMessage = "{0} mora biti duze od 1")]
        [StringLength(maximumLength: 10, ErrorMessage = "{0} je predugacko")]
        [Obavezno]
        public string Ime { get; set; }

        [MinLength(2, ErrorMessage = "{0} mora biti duze od 1")]
        [StringLength(maximumLength: 20, ErrorMessage = "{0} je predugacko")]
        [Obavezno]
        public string Prezime { get; set; }

        [Obavezno]
        public string Adresa { get; set; }

        [EmailAddress(ErrorMessage = "Krivi Format Emaila!!")]
        [Obavezno]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Krivi Format Broja!! (npr: 098-234-7782)")]
        [Obavezno]
        [Display(Name = "Br. Tel.")]
        public string BrojTelefona { get; set; }

        [Obavezno]
        [Display(Name = "Seminar")]
        public int idSeminar { get; set; }

        [Display(Name = "Status")]
        public bool? StatusDaNe { get; set; }
        public string StatusDaNeText => this.StatusDaNe == false ? "Odbijen" : !this.StatusDaNe.HasValue ? "Neobradjen" : "Odobren";

        #region Seminari DropDown Lista
        public IEnumerable<SelectListItem> SeminariDropDown
        {
            get
            {
                using (ZavrsniIspitDBKoristeciGenerateDatacomEntities db = new ZavrsniIspitDBKoristeciGenerateDatacomEntities())
                {
                    //ovo i concat koristim samo ako bas zelim da prvo DEFAULT polje bude disabled, inace moze samo u dropdownlist for please select... i onda ovdje samo lista bez concat
                    List<SelectListItem> dplist = new List<SelectListItem>()
                    {
                        new SelectListItem() { Text="----Odaberite----", Value="0", Disabled=true, Selected=true}, //mora biti broj(0) inace ne radi model binding
                    };

                    #region MaxTest
                    //SelectListItem dodatno = new SelectListItem() { Text = "Autocad", Disabled = true };
                    //IEnumerable<SelectListItem> listatest = db.Seminari.Select(f => new SelectListItem
                    //{
                    //    Value = f.id.ToString(),
                    //    Text = f.Naziv,
                    //    Disabled = f.PopunjenDaNe,
                    //}).ToList();

                    //dodatno.Value = (listatest.Max(x => int.Parse(x.Value)) + 1).ToString(); // Autocadu ce dodati Value 14 jer u bazi seminari ima MAX 13 seminara, 0 je odaberite, Value pretvaram prvo u int da bi prebrojio i onda natrag u string
                    //dplist.Add(dodatno);
                    #endregion

                    return dplist.Concat(
                            db.Seminari.Select(f => new SelectListItem
                            {
                                Value = f.id.ToString(),
                                Text = f.id != this.idSeminar ? f.Naziv : f.Naziv+" (Upisani)", //ako je "current" id, append (Upisani) tekst na Naziv
                                Disabled = f.PopunjenDaNe && f.id != this.idSeminar, //Zatvoren-Crvena, Otvoren-Plava, ako je "current" id onda Zelena
                            })
                        ).ToList(); //Mora ToList kada se koristi Concat + IEnumerable
                }
            }
        }
        #endregion

        public virtual Seminari Seminar { get; set; }








        /*--------------------Practice------------------------*/

        #region Seminari DropDown Lista 2 Koristeci Interface Repository Bez Usinga (PracticeController)
        public IEnumerable<SelectListItem> SeminariDropDown2
        {
            get
            {
                ISeminarRepository db = new SeminarRepository(new ZavrsniIspitDBKoristeciGenerateDatacomEntities());
                return db.GetAll().Select(f => new SelectListItem
                        {
                            Value = f.id.ToString(),
                            Text =  f.Naziv,
                            Disabled = f.PopunjenDaNe && f.id != this.idSeminar, //Zatvoren-Crvena, Otvoren-Plava, ako je "current" id onda Zelena
                        }).ToList();
                }

        }
        #endregion

    }



}