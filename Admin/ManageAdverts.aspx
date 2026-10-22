<%@ Page Title="Manage Adverts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageAdverts.aspx.cs" Inherits="PSCIMS.Admin.ManageAdverts" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Manage Adverts</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title">Manage Job Adverts</h2>
            <p class="page-sub">Create, edit, or close job postings.</p>
        </div>
        <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn--primary" OnClick="btnAddNew_Click" Text="Add New Advert" aria-haspopup="dialog" />
    </div>

    <asp:PlaceHolder ID="phMsg" runat="server" Visible="false">
        <div class="alert alert--success" role="alert">
            <asp:Literal ID="litMsg" runat="server" />
        </div>
    </asp:PlaceHolder>

    <div class="table-wrap card">
        <asp:GridView ID="gvAdverts" runat="server" AutoGenerateColumns="false" CssClass="table" GridLines="None"
            OnRowCommand="gvAdverts_RowCommand" DataKeyNames="Id">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" />
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="ClosesOn" HeaderText="Closes" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <%# Convert.ToBoolean(Eval("IsActive")) ? "<span class='badge badge--open'>Active</span>" : "<span class='badge badge--closed'>Closed</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="300px">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="EditAdvert" CommandArgument='<%# Eval("Id") %>' Text="Edit" CssClass="btn btn--sm btn--outline" />
                        <asp:LinkButton runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("Id") %>' Text="Toggle Active" CssClass="btn btn--sm btn--primary" />
                        <asp:LinkButton runat="server" CommandName="DeleteAdvert" CommandArgument='<%# Eval("Id") %>' Text="Delete" CssClass="btn btn--sm btn--outline" OnClientClick="return confirm('Are you sure you want to delete this advert?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <%-- Add/Edit Modal --%>
    <asp:PlaceHolder ID="phForm" runat="server" Visible="false">
        <div class="modal-overlay">
            <div class="modal" role="dialog" aria-modal="true" aria-labelledby="advertModalHeading">
                <h3 class="card-title" id="advertModalHeading">Create / Edit Advert</h3>
                <asp:HiddenField ID="hfAdvertId" runat="server" />

                <div class="form-group">
                    <label class="form-label" for="<%= txtTitle.ClientID %>">Title</label>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" required="required" />
                </div>
                <div class="form-group">
                    <label class="form-label" for="<%= txtDesc.ClientID %>">Description</label>
                    <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" required="required" />
                </div>
                <div class="form-group">
                    <label class="form-label" for="<%= txtClosingDate.ClientID %>">Closing Date</label>
                    <asp:TextBox ID="txtClosingDate" runat="server" CssClass="form-control" TextMode="Date" required="required" />
                </div>
                <div class="form-actions">
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn--outline" OnClick="btnCancel_Click" CausesValidation="false" />
                    <asp:Button ID="btnSave" runat="server" Text="Save Advert" CssClass="btn btn--primary" OnClick="btnSave_Click" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>