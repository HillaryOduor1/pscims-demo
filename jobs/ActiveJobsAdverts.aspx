<%@ Page Title="Advertised Jobs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActiveJobsAdverts.aspx.cs" Inherits="PSCIMS.jobs.ActiveJobsAdverts" %>
<%@ Import Namespace="PSCIMS.Shared" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Advertised Jobs</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title"><%= ContentProvider.Get("Jobs_Title", "Advertised Jobs") %></h2>
            <p class="page-sub"><%= ContentProvider.Get("Jobs_Sub", "Current vacancies in the Kenya Public Service. All positions close at the dates shown.") %></p>
        </div>
        <a href="RegisterProfile.aspx" class="btn btn--primary" data-nav>
            Register to apply
        </a>
    </div>

    <%-- Filters --%>
    <div class="card mb-6" style="padding:18px 20px;">
        <div class="filter-grid">
            <div>
                <label class="form-label" for="<%= txtSearch.ClientID %>">Search</label>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Job title, reference, ministry…" />
            </div>
            <div>
                <label class="form-label" for="<%= ddlMinistry.ClientID %>">Ministry / Department</label>
                <asp:DropDownList ID="ddlMinistry" runat="server" CssClass="form-control">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="Public Service Commission" Value="PSC" />
                    <asp:ListItem Text="Ministry of Health" Value="MOH" />
                    <asp:ListItem Text="Ministry of Education" Value="MOE" />
                    <asp:ListItem Text="The National Treasury" Value="TNT" />
                    <asp:ListItem Text="Ministry of Interior" Value="MOI" />
                </asp:DropDownList>
            </div>
            <div>
                <label class="form-label" for="<%= ddlSort.ClientID %>">Sort by</label>
                <asp:DropDownList ID="ddlSort" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Newest first" Value="new" />
                    <asp:ListItem Text="Closing soonest" Value="closing" />
                    <asp:ListItem Text="Job title (A–Z)" Value="title" />
                </asp:DropDownList>
            </div>
            <asp:Button ID="btnFilter" runat="server" Text="Apply filters" CssClass="btn btn--primary" OnClick="btnFilter_Click" />
        </div>
    </div>

    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
        <div class="state">
            <p class="state__mark state__mark--info" aria-hidden="true">&mdash;</p>
            <h3 class="state__title">No adverts match your search</h3>
            <p class="state__desc">
                Try removing some filters, or check back later. New positions are posted regularly.
            </p>
            <div class="state__actions">
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear filters" CssClass="btn btn--outline" OnClick="btnClearFilters_Click" />
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phList" runat="server">
        <p style="margin:0 0 16px;font-size:13px;color:var(--text-muted);">
            Showing <strong><asp:Literal ID="litCount" runat="server" Text="0" /></strong> advertised position(s).
        </p>
        <div class="action-grid">
            <asp:Repeater ID="rptJobs" runat="server">
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
                            <a href='<%# "loginPage.aspx?jobId=" + Eval("Id") %>' class="btn btn--primary btn--sm" data-nav>Apply</a>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:PlaceHolder>

</asp:Content>