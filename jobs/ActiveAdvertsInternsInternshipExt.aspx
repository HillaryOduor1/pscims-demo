<%@ Page Title="Internships" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActiveAdvertsInternsInternshipExt.aspx.cs" Inherits="PSCIMS.jobs.ActiveAdvertsInternsInternshipExt" %>
<%@ Import Namespace="PSCIMS.Shared" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Internships</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title"><%= ContentProvider.Get("Int_Title", "Public Service Internship Programme") %></h2>
            <p class="page-sub"><%= ContentProvider.Get("Int_Sub", "Current internship opportunities under the PSIP programme. Open to Kenyan graduates.") %></p>
        </div>
        <a href="RegisterProfile.aspx" class="btn btn--primary" data-nav>
            Register to apply
        </a>
    </div>

    <%-- Filter --%>
    <div class="card mb-6" style="padding:18px 20px;">
        <div class="filter-grid">
            <div>
                <label class="form-label" for="<%= txtSearch.ClientID %>">Search</label>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Internship title, reference, ministry…" />
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
            <asp:Button ID="btnFilter" runat="server" Text="Apply filters" CssClass="btn btn--primary" OnClick="btnFilter_Click" />
        </div>
    </div>

    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
        <div class="state">
            <p class="state__mark state__mark--info" aria-hidden="true">&mdash;</p>
            <h3 class="state__title">No internships match your search</h3>
            <p class="state__desc">Try removing filters or check back later.</p>
            <div class="state__actions">
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear filters" CssClass="btn btn--outline" OnClick="btnClearFilters_Click" />
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
                        <div>
                            <div class="job-card__header">
                                <span class="job-card__ref"><%# Eval("Reference") %></span>
                                <span class='badge <%# (bool)Eval("IsOpen") ? "badge--open" : "badge--closed" %>'><%# (bool)Eval("IsOpen") ? "Open" : "Closed" %></span>
                            </div>
                            <p class="job-card__title"><%# Eval("Title").ToString().Replace(" — ", " ") %></p>
                            <p class="job-card__desc"><%# Eval("Ministry") %> &middot; <%# Eval("Location") %></p>
                        </div>
                        <div class="job-card__footer">
                            <span class="job-card__date">Closes <strong><%# Eval("ClosesOn", "{0:dd MMM yyyy}") %></strong></span>
                            <a href='<%# "loginPage.aspx?internshipId=" + Eval("Id") %>' class="btn btn--primary btn--sm" data-nav>Apply</a>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:PlaceHolder>

    <div class="card mb-6" style="margin-top:32px;">
        <h3 class="card-title"><%= ContentProvider.Get("Int_Eligibility_Title", "Eligibility") %></h3>
        <ul class="prose-list">
            <li><%= ContentProvider.Get("Int_Eligibility_1", "Kenyan citizen with a valid national ID.") %></li>
            <li><%= ContentProvider.Get("Int_Eligibility_2", "Holder of a Bachelor's degree from a recognised university, obtained within the last three (3) years.") %></li>
            <li><%= ContentProvider.Get("Int_Eligibility_3", "Must not have previously participated in a PSIP cohort.") %></li>
            <li><%= ContentProvider.Get("Int_Eligibility_4", "Aged 35 years or below at the time of application.") %></li>
        </ul>
    </div>

    <div class="card mb-6">
        <h3 class="card-title"><%= ContentProvider.Get("Int_Req_Title", "Application requirements") %></h3>
        <p class="card-sub"><%= ContentProvider.Get("Int_Req_Sub", "Prepare the following before you begin.") %></p>
        <ul class="prose-list">
            <li><%= ContentProvider.Get("Int_Req_1", "National ID or Passport") %></li>
            <li><%= ContentProvider.Get("Int_Req_2", "KCSE Certificate") %></li>
            <li><%= ContentProvider.Get("Int_Req_3", "University Degree Certificate and Transcripts") %></li>
            <li><%= ContentProvider.Get("Int_Req_4", "Curriculum Vitae") %></li>
            <li><%= ContentProvider.Get("Int_Req_5", "Two referees with contact details") %></li>
        </ul>
    </div>

    <div class="alert alert--warning">
        <div class="alert__title"><%= ContentProvider.Get("Home_Disclaimer_Title", "Disclaimer") %></div>
        <%= ContentProvider.Get("Home_Disclaimer_Text", "Section 100(4) of the Public Service Commission Act 2017 provides that a person who gives false or misleading information to the Commission is, on conviction, liable to a fine not exceeding Kshs. 200,000 or to imprisonment for a term not exceeding two years, or both.") %>
    </div>

</asp:Content>