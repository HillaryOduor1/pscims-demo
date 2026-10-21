<%@ Page Title="My Applications" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ApplicantDashboard.aspx.cs" Inherits="PSCIMS.jobs.ApplicantDashboard" %>
<%@ Import Namespace="PSCIMS.Shared" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">My Applications</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title">Welcome, <asp:Literal ID="litName" runat="server" /></h2>
            <p class="page-sub"><%= ContentProvider.Get("Applicant_Dash_Sub", "Applications you have submitted through PSCIMS.") %></p>
        </div>
        <a href="ActiveJobsAdverts.aspx" class="btn btn--primary"><%= ContentProvider.Get("Applicant_Dash_Btn", "Browse open positions") %></a>
    </div>

    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
        <div class="state">
            <p class="state__mark state__mark--info" aria-hidden="true">&mdash;</p>
            <h3 class="state__title"><%= ContentProvider.Get("Applicant_Dash_EmptyTitle", "No applications yet") %></h3>
            <p class="state__desc"><%= ContentProvider.Get("Applicant_Dash_EmptyText", "Browse open positions and submit your first application.") %></p>
            <div class="state__actions">
                <a href="ActiveJobsAdverts.aspx" class="btn btn--primary"><%= ContentProvider.Get("Applicant_Dash_EmptyBtn", "Browse advertised jobs") %></a>
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phList" runat="server">
        <div class="table-wrap">
            <asp:Repeater ID="rptApplications" runat="server">
                <HeaderTemplate>
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Reference</th>
                                <th>Position</th>
                                <th>Ministry</th>
                                <th>Submitted</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td style="font-family:ui-monospace,monospace;font-size:12.5px;"><%# Server.HtmlEncode(Eval("Reference").ToString()) %></td>
                        <td><strong><%# Server.HtmlEncode(Eval("Title").ToString()) %></strong></td>
                        <td><%# Server.HtmlEncode(Eval("Ministry").ToString()) %></td>
                        <td><%# Eval("SubmittedAt", "{0:dd MMM yyyy}") %></td>
                        <td>
                            <span class='badge <%# StatusClass(Eval("Status")) %>'>
                                <%# Server.HtmlEncode(Eval("Status").ToString()) %>
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