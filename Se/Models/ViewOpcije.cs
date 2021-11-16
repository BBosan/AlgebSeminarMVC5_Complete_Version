using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Se.Controllers;
using System.Web.Mvc.Ajax;
using X.PagedList.Mvc;
using X.PagedList.Mvc.Common;
using X.PagedList;
using System.Web.Mvc;

namespace Se.Models
{
    public static class ViewOpcije
    {
        public static AjaxOptions ajaxOptions = new AjaxOptions() { HttpMethod = "POST", UpdateTargetId = "Ajax", InsertionMode = InsertionMode.Replace/*, OnSuccess = "fnSuccess" */};
        public static Dictionary<string, object> glyphiconSort = new Dictionary<string, object> { { "class", "glyphicon glyphicon-sort" } };

        public static PagedListRenderOptions pagedListRenderOpcije = new PagedListRenderOptions
        {
            LinkToFirstPageFormat = "1",
            DisplayLinkToFirstPage = PagedListDisplayMode.IfNeeded,
            DisplayLinkToLastPage = PagedListDisplayMode.Never,
            Display = PagedListDisplayMode.IfNeeded,
            LinkToPreviousPageFormat = "❮",
            LinkToNextPageFormat = "❯",
            MaximumPageNumbersToDisplay = 6,
        };

        //public static object glyphiconSort = new { @class = "glyphicon glyphicon-sort" };
        //public static object generalRouteValues(SearchModel search) => new { Naziv = search.Naziv, Broj = search.Broj, Status = search.Status, Opis = search.Opis };

        #region MetodaTest
        //public static AjaxOptions ajaxOptionsMetoda(string divID)
        //{
        //    AjaxOptions ajaxOptions = new AjaxOptions() { HttpMethod = "POST", UpdateTargetId = divID, InsertionMode = InsertionMode.Replace, OnSuccess = "fnSuccess" };
        //    return ajaxOptions;
        //} 
        #endregion


    }
}