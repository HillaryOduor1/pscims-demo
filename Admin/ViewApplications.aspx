<%@ Page Title="View Applications" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewApplications.aspx.cs" Inherits="PSCIMS.Admin.ViewApplications" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">View Applications</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title">Submitted Applications</h2>
            <p class="page-sub">Review candidate submissions and download CVs.</p>
        </div>
    </div>

    <div class="table-wrap card">
        <asp:GridView ID="gvApps" runat="server" AutoGenerateColumns="false" CssClass="table" GridLines="None">
            <Columns>
                <asp:BoundField DataField="ApplicationId" HeaderText="App ID" />
                <asp:BoundField DataField="AdvertTitle" HeaderText="Position Applied For" />
                <asp:BoundField DataField="ApplicantName" HeaderText="Candidate Name" />
                <asp:BoundField DataField="ApplicantEmail" HeaderText="Email" />
                <asp:BoundField DataField="SubmissionDate" HeaderText="Date Applied" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:TemplateField HeaderText="CV / Documents" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <%-- Assuming you have a generic handler for downloading files --%>
                        <a href='<%# ResolveUrl("~/DownloadFile.ashx?appId=" + Eval("ApplicationId")) %>' class="btn btn--sm btn--outline" target="_blank">Download CV</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>