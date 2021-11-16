using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Se.Models
{

    public class SearchModel
    {
        public SearchModel(){}


        //Seminari
        [Display(Name = "Filter By Naziv:")]
        public string Naziv { get; set; }
        [Display(Name = "Filter By Opis:")]
        public string Opis { get; set; }
        [Display(Name = "Filter By Status:")]
        public string Status { get; set; } = "";
        [Display(Name = "Filter By Broj Upisa:")]
        public string Broj { get; set; }
        public SelectListItem[] DP_Seminari_Status => new SelectListItem[]{
                new SelectListItem() {Text = "Svi", Value="", Selected=true},
                new SelectListItem() {Text = "Otvoreni", Value="Otvoreni"},
                new SelectListItem() {Text = "Zatvoreni", Value="Zatvoreni"}
        };


        //Predbiljezbe
        [Display(Name = "Filter By Ime:")]
        public string Ime { get; set; }
        [Display(Name = "Filter By Prezime:")]
        public string Prezime { get; set; }
        public SelectListItem[] DP_Predbiljezbe_Status => new SelectListItem[]{
                new SelectListItem() {Text = "Sve", Value="", Selected=true},
                new SelectListItem() {Text = "Odobrene", Value="Odobrene"},
                new SelectListItem() {Text = "Odbijene", Value="Odbijene"},
                new SelectListItem() {Text = "Neobradjene", Value="Neobradjene"}
        };








        //--------------------------------------------------------------------//
        //Za Practice

        public IEnumerable<SelectListItem> VjezbaSelected
        {
            get {
                return new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Svi", Value="", Selected = this.Status == "" },
                    new SelectListItem() {Text = "Otvoreni", Value="Otvoreni", Selected = this.Status == "Otvoreni" },
                    new SelectListItem() {Text = "Zatvoreni", Value="Zatvoreni", Selected = this.Status == "Zatvoreni"}
                };
            }
        }

        //--------------------------------------------------------------------//


        #region Using somewhere
        //public IEnumerable<SemVM> semVMLista { get; set; } //netreba u SearchModelOnly jer koristim ViewBag ali za one prije treba mislim

        //public object RouteReflect => new { Naziv = this.Naziv, Broj = this.Broj, Opis = this.Opis, Status = this.Status };
        #endregion


        //public string[] StatusDP => new string[] { "Otvoren", "Zatvoren", "All" };
    }
}