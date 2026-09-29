using System;
using System.Collections.Generic;
using System.Linq;

namespace PSCIMS.jobs
{
    public partial class AdvertStatusGlobal : System.Web.UI.Page
    {
        private class AdvertStatus
        {
            public string Reference { get; set; }
            public string Title { get; set; }
            public string Ministry { get; set; }
            public DateTime ClosesOn { get; set; }
            public bool IsOpen { get; set; }
        }

        private List<AdvertStatus> _all;

        protected void Page_Load(object sender, EventArgs e)
        {
            _all = SampleData();
            if (!IsPostBack) Bind();
        }

        protected void btnFilter_Click(object sender, EventArgs e) { Bind(); }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlStatus.SelectedIndex = 0;
            ddlSort.SelectedIndex = 0;
            Bind();
        }

        private void Bind()
        {
            string search = (txtSearch.Text ?? "").Trim().ToLowerInvariant();
            string status = ddlStatus.SelectedValue;
            string sort = ddlSort.SelectedValue;

            var q = _all.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                q = q.Where(a =>
                    (a.Title ?? "").ToLowerInvariant().Contains(search) ||
                    (a.Reference ?? "").ToLowerInvariant().Contains(search) ||
                    (a.Ministry ?? "").ToLowerInvariant().Contains(search));
            }

            if (status == "open")   q = q.Where(a => a.IsOpen);
            if (status == "closed") q = q.Where(a => !a.IsOpen);

            switch (sort)
            {
                case "new":  q = q.OrderByDescending(a => a.ClosesOn); break;
                case "ref":  q = q.OrderBy(a => a.Reference); break;
                default:     q = q.OrderBy(a => a.ClosesOn); break;
            }

            var list = q.ToList();
            phEmpty.Visible = list.Count == 0;
            phList.Visible  = list.Count > 0;
            rptStatus.DataSource = list;
            rptStatus.DataBind();
        }

        private List<AdvertStatus> SampleData()
        {
            return new List<AdvertStatus>
            {
                new AdvertStatus { Reference="PSC/HRM/01/2026", Title="Senior Human Resource Management Officer", Ministry="Public Service Commission", ClosesOn=DateTime.Today.AddDays(14), IsOpen=true },
                new AdvertStatus { Reference="MOH/CS/02/2026",   Title="Chief Pharmacist",                        Ministry="Ministry of Health",          ClosesOn=DateTime.Today.AddDays(7),  IsOpen=true },
                new AdvertStatus { Reference="MOE/TCH/03/2026",  Title="Senior Teacher — Mathematics",            Ministry="Ministry of Education",       ClosesOn=DateTime.Today.AddDays(21), IsOpen=true },
                new AdvertStatus { Reference="TNT/ACC/04/2026",  Title="Principal Accountant",                    Ministry="The National Treasury",       ClosesOn=DateTime.Today.AddDays(3),  IsOpen=true },
                new AdvertStatus { Reference="MOI/ICT/05/2026",  Title="Information Communication Technology Officer", Ministry="Ministry of Interior", ClosesOn=DateTime.Today.AddDays(10), IsOpen=true },
                new AdvertStatus { Reference="PSC/ICT/06/2026",  Title="ICT Officer I",                           Ministry="Public Service Commission",   ClosesOn=DateTime.Today.AddDays(-2), IsOpen=false },
                new AdvertStatus { Reference="MOH/NUR/07/2025",  Title="Senior Nursing Officer",                  Ministry="Ministry of Health",          ClosesOn=DateTime.Today.AddDays(-30), IsOpen=false }
            };
        }
    }
}