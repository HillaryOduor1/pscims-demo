using System;
using System.Collections.Generic;
using System.Linq;

namespace PSCIMS.jobs
{
    public partial class ActiveAdvertsInternsInternshipExt : System.Web.UI.Page
    {
        private class Internship
        {
            public int Id { get; set; }
            public string Reference { get; set; }
            public string Title { get; set; }
            public string Ministry { get; set; }
            public string Location { get; set; }
            public DateTime ClosesOn { get; set; }
            public bool IsOpen { get; set; }
        }

        private List<Internship> _all;

        protected void Page_Load(object sender, EventArgs e)
        {
            _all = SampleData();
            if (!IsPostBack) Bind();
        }

        protected void btnFilter_Click(object sender, EventArgs e) { Bind(); }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlMinistry.SelectedIndex = 0;
            ddlSort.SelectedIndex = 0;
            Bind();
        }

        private void Bind()
        {
            string search = (txtSearch.Text ?? "").Trim().ToLowerInvariant();
            string ministry = ddlMinistry.SelectedValue;
            string sort = ddlSort.SelectedValue;

            var q = _all.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                q = q.Where(j =>
                    (j.Title ?? "").ToLowerInvariant().Contains(search) ||
                    (j.Reference ?? "").ToLowerInvariant().Contains(search) ||
                    (j.Ministry ?? "").ToLowerInvariant().Contains(search));
            }

            if (!string.IsNullOrEmpty(ministry))
            {
                q = q.Where(j => (j.Ministry ?? "").IndexOf(ministry, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            switch (sort)
            {
                case "closing": q = q.OrderBy(j => j.ClosesOn); break;
                case "title":   q = q.OrderBy(j => j.Title); break;
                default:        q = q.OrderByDescending(j => j.ClosesOn); break;
            }

            var list = q.ToList();
            phEmpty.Visible = list.Count == 0;
            phList.Visible  = list.Count > 0;
            litCount.Text   = list.Count.ToString();
            rptInternships.DataSource = list;
            rptInternships.DataBind();
        }

        private List<Internship> SampleData()
        {
            return new List<Internship>
            {
                new Internship { Id=101, Reference="PSC/PSIP/01/2026", Title="PSIP Internship — Human Resources",     Ministry="Public Service Commission", Location="Nairobi", ClosesOn=DateTime.Today.AddDays(10), IsOpen=true },
                new Internship { Id=102, Reference="MOH/PSIP/02/2026", Title="PSIP Internship — Public Health",        Ministry="Ministry of Health",        Location="Nairobi", ClosesOn=DateTime.Today.AddDays(5),  IsOpen=true },
                new Internship { Id=103, Reference="MOE/PSIP/03/2026", Title="PSIP Internship — Education Management", Ministry="Ministry of Education",     Location="Nakuru",  ClosesOn=DateTime.Today.AddDays(18), IsOpen=true },
                new Internship { Id=104, Reference="TNT/PSIP/04/2026", Title="PSIP Internship — Public Finance",       Ministry="The National Treasury",     Location="Nairobi", ClosesOn=DateTime.Today.AddDays(1),  IsOpen=true },
                new Internship { Id=105, Reference="PSC/PSIP/05/2025", Title="PSIP Internship — ICT (2025 Cohort)",    Ministry="Public Service Commission", Location="Nairobi", ClosesOn=DateTime.Today.AddDays(-30), IsOpen=false }
            };
        }
    }
}