<%@ Page Title="Application Status" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdvertStatusGlobal.aspx.cs" Inherits="PSCIMS.jobs.AdvertStatusGlobal" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Application Status</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title">Status of Advertised Posts</h2>
            <p class="page-sub">Check whether an advertised position is still open or has closed.</p>
        </div>
        <a href="loginPage.aspx" class="btn btn--primary" data-nav>
            Sign in to see your applications
        </a>
    </div>

    <!--<div class="alert alert--info">
        <div class="alert__title">What this page shows</div>
        Public status of every position advertised through PSCIMS. To see the status
        of <em>your own</em> applications, <a href="loginPage.aspx" data-nav>sign in</a>.
    </div>-->

    <div class="card mb-6" style="padding:18px 20px;">
        <div style="display:grid;grid-template-columns:2fr 1fr 1fr auto;gap:12px;align-items:end;">
            <div>
                <label class="form-label" for="<%= txtSearch.ClientID %>">Search</label>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                             placeholder="Job title, reference, ministry…" />
            </div>
            <div>
                <label class="form-label" for="<%= ddlStatus.ClientID %>">Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="Open" Value="open" />
                    <asp:ListItem Text="Closed" Value="closed" />
                </asp:DropDownList>
            </div>
            <div>
                <label class="form-label" for="<%= ddlSort.ClientID %>">Sort by</label>
                <asp:DropDownList ID="ddlSort" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Closing soonest" Value="closing" />
                    <asp:ListItem Text="Newest first" Value="new" />
                    <asp:ListItem Text="Reference (A–Z)" Value="ref" />
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
            <p class="state__desc">Try adjusting the filters or clear them to see all adverts.</p>
            <div class="state__actions">
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear filters"
                            CssClass="btn btn--outline"
                            OnClick="btnClearFilters_Click" />
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phList" runat="server">
        <div class="table-wrap">
            <asp:Repeater ID="rptStatus" runat="server">
                <HeaderTemplate>
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Reference</th>
                                <th>Position</th>
                                <th>Ministry</th>
                                <th>Closes</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td style="font-family:ui-monospace,monospace;font-size:12.5px;"><%# Eval("Reference") %></td>
                        <td><strong><%# Eval("Title") %></strong></td>
                        <td><%# Eval("Ministry") %></td>
                        <td><%# Eval("ClosesOn", "{0:dd MMM yyyy}") %></td>
                        <td>
                            <span class='badge <%# (bool)Eval("IsOpen") ? "badge--open" : "badge--closed" %>'>
                                <%# (bool)Eval("IsOpen") ? "Open" : "Closed" %>
                            </span>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </asp:PlaceHolder>

</asp:Content>