using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Se.Models
{
    public class VMCollection
    {

        public VMCollection()
        {
            this.Search = new SearchModel() {};
        }

        public SearchModel Search { get; set; }
        public IEnumerable<SemVM> SemVMList { get; set; }

        public SelectListItem[] DP => new SelectListItem[]{
                new SelectListItem() {Text = "All", Value="", Selected=true},
                new SelectListItem() {Text = "Otvoren", Value="Otvoren"},
                new SelectListItem() {Text = "Zatvoren", Value="Zatvoren"}};
    }
}