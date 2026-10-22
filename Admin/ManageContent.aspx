<%@ Page Title="Manage Content" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageContent.aspx.cs" Inherits="PSCIMS.Admin.ManageContent" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Manage Content</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title">Manage Site Content</h2>
            <p class="page-sub">Select a page to edit its content.</p>
        </div>
    </div>

    <asp:PlaceHolder ID="phMsg" runat="server" Visible="false">
        <div class="alert alert--success" role="alert">
            <asp:Literal ID="litMsg" runat="server" />
        </div>
    </asp:PlaceHolder>

    <div class="card mb-6" style="padding:18px 20px;">
        <div class="form-group" style="margin-bottom:0;">
            <label class="form-label" for="<%= ddlPages.ClientID %>">Select Page</label>
            <asp:DropDownList ID="ddlPages" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlPages_SelectedIndexChanged">
                <asp:ListItem Text="Home / Dashboard (Default.aspx)" Value="Home_" />
                <asp:ListItem Text="Internships (ActiveAdvertsInternsInternshipExt.aspx)" Value="Int_" />
                <asp:ListItem Text="Emerging Leaders (ELPEXT.aspx)" Value="ELP_" />
                <asp:ListItem Text="Advertised Jobs (ActiveJobsAdverts.aspx)" Value="Jobs_" />
                <asp:ListItem Text="Application Status (AdvertStatusGlobal.aspx)" Value="Status_" />
                <asp:ListItem Text="Applicant Dashboard (ApplicantDashboard.aspx)" Value="Applicant_Dash_" />
                <asp:ListItem Text="Global Messages (Errors, Not Found)" Value="Global_" />
            </asp:DropDownList>
        </div>
    </div>

    <div class="table-wrap card">
        <asp:GridView ID="gvContent" runat="server" AutoGenerateColumns="false"
            CssClass="table" GridLines="None" OnRowCommand="gvContent_RowCommand" DataKeyNames="ContentKey">

            <Columns>
                <asp:BoundField DataField="ContentKey" HeaderText="Key Name" />
                <asp:TemplateField HeaderText="Content Value">
                    <ItemTemplate>
                        <span class="table__truncate"><%# Eval("ContentValue") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="LastUpdated" HeaderText="Last Updated" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="EditContent" CommandArgument='<%# Eval("ContentKey") %>' Text="Edit" CssClass="btn btn--sm btn--outline" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <%-- Edit Modal --%>
    <asp:PlaceHolder ID="phForm" runat="server" Visible="false">
        <div class="modal-overlay">
            <div class="modal" role="dialog" aria-modal="true" aria-labelledby="contentModalHeading">
                <h3 class="card-title" id="contentModalHeading">Edit Content</h3>
                <asp:HiddenField ID="hfContentKey" runat="server" />

                <div class="form-group">
                    <label class="form-label" for="<%= txtKeyName.ClientID %>">Key Name</label>
                    <asp:TextBox ID="txtKeyName" runat="server" CssClass="form-control" ReadOnly="true" BackColor="#f0f0f0" />
                </div>
                <div class="form-group">
                    <label class="form-label" for="<%= txtContentValue.ClientID %>">Content Value</label>
                    <asp:TextBox ID="txtContentValue" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6" />
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn--outline" OnClick="btnCancel_Click" CausesValidation="false" />
                    <asp:Button ID="btnSave" runat="server" Text="Save Content" CssClass="btn btn--primary" OnClick="btnSave_Click" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>