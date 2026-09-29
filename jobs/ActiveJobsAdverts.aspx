<%@ Page Title="Advertised Jobs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActiveJobsAdverts.aspx.cs" Inherits="PSCIMS.jobs.ActiveJobsAdverts" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Advertised Jobs</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title">Advertised Jobs</h2>
            <p class="page-sub">Current vacancies in the Kenya Public Service. All positions close at the dates shown.</p>
        </div>
        <a href="RegisterProfile.aspx" class="btn btn--primary" data-nav>
            Register to apply
        </a>
    </div>

    <!--<div class="alert alert--info">
        <div class="alert__title">Before you apply</div>
        You must <a href="RegisterProfile.aspx" data-nav>register</a> and
        <a href="loginPage.aspx" data-nav>sign in</a> to submit an application.
        Ensure all qualifications, experience, and referee details are complete
        before the advert closes.
    </div>-->

    <%-- Filters --%>
    <div class="card mb-6" style="padding:18px 20px;">
        <div style="display:grid;grid-template-columns:2fr 1fr 1fr auto;gap:12px;align-items:end;">
            <div>
                <label class="form-label" for="<%= txtSearch.ClientID %>">Search</label>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                             placeholder="Job title, reference, ministry…" />
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
            <asp:Button ID="btnFilter" runat="server" Text="Apply filters"
                        CssClass="btn btn--primary"
                        OnClick="btnFilter_Click" />
        </div>
    </div>

    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
        <div class="state">
            <p class="state__mark state__mark--info" aria-hidden="true">&mdash;</p>
            <h3 class="state__title">No adverts match your search</h3>
            <p class="state__desc">
                Try removing some filters, or check back later — new positions are posted regularly.
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
            Showing <strong><asp:Literal ID="litCount" runat="server" Text="0" /></strong> advertised position(s).
        </p>

        <div class="action-grid">
            <asp:Repeater ID="rptJobs" runat="server">
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
                            <a href='<%# "loginPage.aspx?jobId=" + Eval("Id") %>'
                               class="btn btn--primary btn--sm"
                               data-nav>Apply</a>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

    </asp:PlaceHolder>

</asp:Content>