<%@ Page Title="Internships" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActiveAdvertsInternsInternshipExt.aspx.cs" Inherits="PSCIMS.jobs.ActiveAdvertsInternsInternshipExt" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Internships</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title">Public Service Internship Programme</h2>
            <p class="page-sub">Current internship opportunities under the PSIP programme. Open to Kenyan graduates.</p>
        </div>
        <a href="RegisterProfile.aspx" class="btn btn--primary" data-nav>
            Register to apply
        </a>
    </div>

    <!--<div class="alert alert--info">
        <div class="alert__title">About the PSIP</div>
        The Public Service Internship Programme (PSIP) is a 12-month structured programme
        that provides graduates with hands-on experience in the public service. Successful
        interns receive a monthly stipend and a Certificate of Completion.
    </div>-->

    <%-- Filter --%>
    <div class="card mb-6" style="padding:18px 20px;">
        <div style="display:grid;grid-template-columns:2fr 1fr 1fr auto;gap:12px;align-items:end;">
            <div>
                <label class="form-label" for="<%= txtSearch.ClientID %>">Search</label>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                             placeholder="Internship title, reference, ministry…" />
            </div>
            <div>
                <label class="form-label" for="<%= ddlMinistry.ClientID %>">Ministry</label>
                <asp:DropDownList ID="ddlMinistry" runat="server" CssClass="form-control">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="Public Service Commission" Value="PSC" />
                    <asp:ListItem Text="Ministry of Health" Value="MOH" />
                    <asp:ListItem Text="Ministry of Education" Value="MOE" />
                    <asp:ListItem Text="The National Treasury" Value="TNT" />
                </asp:DropDownList>
            </div>
            <div>
                <label class="form-label" for="<%= ddlSort.ClientID %>">Sort by</label>
                <asp:DropDownList ID="ddlSort" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Newest first" Value="new" />
                    <asp:ListItem Text="Closing soonest" Value="closing" />
                    <asp:ListItem Text="Title (A–Z)" Value="title" />
                </asp:DropDownList>
            </div>
            <asp:Button ID="btnFilter" runat="server" Text="Apply filters"
                        CssClass="btn btn--primary"
                        OnClick="btnFilter_Click" />
        </div>
    </div>

    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
        <div class="state">
            <p class="state__mark state__mark--info" aria-hidden="true">&mdash;</p>
            <h3 class="state__title">No internships match your search</h3>
            <p class="state__desc">
                Try removing filters or check back later. New internship rounds are advertised
                periodically through the year.
            </p>
            <div class="state__actions">
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear filters"
                            CssClass="btn btn--outline"
                            OnClick="btnClearFilters_Click" />
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phList" runat="server">

        <p style="margin:0 0 16px;font-size:13px;color:var(--text-muted);">
            Showing <strong><asp:Literal ID="litCount" runat="server" Text="0" /></strong> internship(s).
        </p>

        <div class="action-grid">
            <asp:Repeater ID="rptInternships" runat="server">
                <ItemTemplate>
                    <div class="action-card" style="cursor:default;">
                        <div style="display:flex;justify-content:space-between;align-items:flex-start;gap:12px;margin-bottom:8px;">
                            <span style="font-size:11px;font-weight:800;letter-spacing:1px;text-transform:uppercase;color:var(--text-muted);">
                                <%# Eval("Reference") %>
                            </span>
                            <span class='badge <%# (bool)Eval("IsOpen") ? "badge--open" : "badge--closed" %>'>
                                <%# (bool)Eval("IsOpen") ? "Open" : "Closed" %>
                            </span>
                        </div>
                        <p class="action-card__title"><%# Eval("Title") %></p>
                        <p class="action-card__desc" style="margin-bottom:12px;">
                            <%# Eval("Ministry") %> &nbsp;·&nbsp; <%# Eval("Location") %>
                        </p>
                        <div style="display:flex;justify-content:space-between;align-items:center;font-size:12px;">
                            <span style="color:var(--text-muted);">
                                Closes <strong style="color:var(--text);"><%# Eval("ClosesOn", "{0:dd MMM yyyy}") %></strong>
                            </span>
                            <a href='<%# "loginPage.aspx?internshipId=" + Eval("Id") %>'
                               class="btn btn--primary btn--sm"
                               data-nav>Apply</a>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

    </asp:PlaceHolder>

    <div class="card mb-6" style="margin-top:32px;">
        <h3 class="card-title">Eligibility</h3>
        <ul style="line-height:1.85;padding-left:20px;margin:0 0 0 0;color:var(--text);">
            <li>Kenyan citizen with a valid national ID.</li>
            <li>Holder of a Bachelor's degree from a recognised university, obtained within the last three (3) years.</li>
            <li>Must not have previously participated in a PSIP cohort.</li>
            <li>Aged 35 years or below at the time of application.</li>
        </ul>
    </div>

    <div class="card mb-6">
        <h3 class="card-title">Application requirements</h3>
        <p class="card-sub">Prepare the following before you begin.</p>
        <ul style="line-height:1.85;padding-left:20px;margin:0;color:var(--text);">
            <li>National ID or Passport</li>
            <li>KCSE Certificate</li>
            <li>University Degree Certificate and Transcripts</li>
            <li>Curriculum Vitae</li>
            <li>Two referees with contact details</li>
        </ul>
    </div>

    <div class="alert alert--warning">
        <div class="alert__title">Disclaimer</div>
        Section 100(4) of the Public Service Commission Act 2017 provides that a person who
        gives false or misleading information to the Commission is, on conviction, liable to
        a fine not exceeding Kshs. 200,000 or to imprisonment for a term not exceeding two
        years, or both.
    </div>

</asp:Content>