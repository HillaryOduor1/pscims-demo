using System;
using System.Collections.Generic;
using System.Linq;

namespace PSCIMS.jobs
{
    public partial class ActiveJobsAdverts : System.Web.UI.Page
    {
        // Simple in-memory sample data. Replace with DAL call when wiring to DB.
        // Example: List<Advert> jobs = AdvertDal.GetActiveAdverts();
        private class Advert
        {
            public int Id { get; set; }
            public string Reference { get; set; }
            public string Title { get; set; }
            public string Ministry { get; set; }
            public string Location { get; set; }
            public DateTime ClosesOn { get; set; }
            public bool IsOpen { get; set; }
        }

        private List<Advert> _all;

        protected void Page_Load(object sender, EventArgs e)
        {
            _all = SampleData();

            if (!IsPostBack)
            {
                Bind();
            }
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

            var query = _all.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(j =>
                    (j.Title ?? "").ToLowerInvariant().Contains(search) ||
                    (j.Reference ?? "").ToLowerInvariant().Contains(search) ||
                    (j.Ministry ?? "").ToLowerInvariant().Contains(search));
            }

            if (!string.IsNullOrEmpty(ministry))
            {
                query = query.Where(j => string.Equals(j.Ministry, ministry, StringComparison.OrdinalIgnoreCase)
                                      || (j.Ministry ?? "").IndexOf(ministry, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            switch (sort)
            {
                case "closing":
                    query = query.OrderBy(j => j.ClosesOn);
                    break;
                case "title":
                    query = query.OrderBy(j => j.Title);
                    break;
                default:
                    query = query.OrderByDescending(j => j.ClosesOn);
                    break;
            }

            var list = query.ToList();

            phEmpty.Visible = list.Count == 0;
            phList.Visible = list.Count > 0;
            litCount.Text = list.Count.ToString();
            rptJobs.DataSource = list;
            rptJobs.DataBind();
        }

        private List<Advert> SampleData()
        {
            return new List<Advert>
            {
                new Advert { Id=1, Reference="PSC/HRM/01/2026", Title="Senior Human Resource Management Officer", Ministry="Public Service Commission", Location="Nairobi", ClosesOn=DateTime.Today.AddDays(14), IsOpen=true },
                new Advert { Id=2, Reference="MOH/CS/02/2026",   Title="Chief Pharmacist",                        Ministry="Ministry of Health",          Location="Nairobi", ClosesOn=DateTime.Today.AddDays(7),  IsOpen=true },
                new Advert { Id=3, Reference="MOE/TCH/03/2026",  Title="Senior Teacher — Mathematics",            Ministry="Ministry of Education",       Location="Kisumu",  ClosesOn=DateTime.Today.AddDays(21), IsOpen=true },
                new Advert { Id=4, Reference="TNT/ACC/04/2026",  Title="Principal Accountant",                    Ministry="The National Treasury",       Location="Nairobi", ClosesOn=DateTime.Today.AddDays(3),  IsOpen=true },
                new Advert { Id=5, Reference="MOI/ICT/05/2026",  Title="Information Communication Technology Officer", Ministry="Ministry of Interior", Location="Mombasa", ClosesOn=DateTime.Today.AddDays(10), IsOpen=true },
                new Advert { Id=6, Reference="PSC/ICT/06/2026",  Title="ICT Officer I",                           Ministry="Public Service Commission",   Location="Nairobi", ClosesOn=DateTime.Today.AddDays(-2), IsOpen=false }
            };
        }
    }
}