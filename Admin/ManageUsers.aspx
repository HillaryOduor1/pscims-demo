<%@ Page Title="Manage Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="PSCIMS.Admin.ManageUsers" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Manage Users</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title">Manage Users</h2>
            <p class="page-sub">View, edit roles, create users, and reset passwords.</p>
        </div>
        <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn--primary" OnClick="btnAddNew_Click" Text="Create New User" aria-haspopup="dialog" />
    </div>

    <asp:PlaceHolder ID="phMsg" runat="server" Visible="false">
        <div class="alert alert--success" role="alert">
            <asp:Literal ID="litMsg" runat="server" />
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phError" runat="server" Visible="false">
        <div class="alert alert--danger" role="alert">
            <asp:Literal ID="litError" runat="server" />
        </div>
    </asp:PlaceHolder>

    <div class="table-wrap card">
        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false"
            CssClass="table" GridLines="None" OnRowCommand="gvUsers_RowCommand"
            OnRowEditing="gvUsers_RowEditing" OnRowCancelingEdit="gvUsers_RowCancelingEdit"
            OnRowUpdating="gvUsers_RowUpdating" DataKeyNames="Id">

            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="true" />
                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate><%# Eval("FirstName") %> <%# Eval("Surname") %></ItemTemplate>
                    <EditItemTemplate><%# Eval("FirstName") %> <%# Eval("Surname") %></EditItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="IdNumber" HeaderText="ID Number" ReadOnly="true" />
                <asp:BoundField DataField="Email" HeaderText="Email" ReadOnly="true" />

                <asp:TemplateField HeaderText="Role">
                    <ItemTemplate><%# Eval("RoleName") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlRoles" runat="server" CssClass="form-control" aria-label="Role" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="280px">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="Edit" Text="Edit Role" CssClass="btn btn--sm btn--outline" />
                        <asp:LinkButton runat="server" CommandName="SendOtp" CommandArgument='<%# Eval("Id") %>' Text="Send OTP" CssClass="btn btn--sm btn--primary" />
                        <asp:LinkButton runat="server" CommandName="ResetPassword" CommandArgument='<%# Eval("Id") %>' Text="Reset Pass" CssClass="btn btn--sm btn--danger" OnClientClick="return confirm('Are you sure you want to reset this user\'s password? A temporary password will be generated.');" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton runat="server" CommandName="Update" Text="Save" CssClass="btn btn--sm btn--primary" />
                        <asp:LinkButton runat="server" CommandName="Cancel" Text="Cancel" CssClass="btn btn--sm btn--outline" />
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <%-- Create User Modal --%>
    <asp:PlaceHolder ID="phForm" runat="server" Visible="false">
        <div class="modal-overlay">
            <div class="modal" role="dialog" aria-modal="true" aria-labelledby="newUserHeading">
                <h3 class="card-title" id="newUserHeading">Create New User</h3>
                <p class="card-sub">Create a user manually (email verification is bypassed for admin creation).</p>

                <div class="form-group">
                    <label class="form-label" for="<%= txtNewIdNum.ClientID %>">ID / Passport Number</label>
                    <asp:TextBox ID="txtNewIdNum" runat="server" CssClass="form-control" required="required" />
                </div>
                <div class="form-group">
                    <label class="form-label" for="<%= txtNewFirstName.ClientID %>">First Name</label>
                    <asp:TextBox ID="txtNewFirstName" runat="server" CssClass="form-control" required="required" />
                </div>
                <div class="form-group">
                    <label class="form-label" for="<%= txtNewSurname.ClientID %>">Surname</label>
                    <asp:TextBox ID="txtNewSurname" runat="server" CssClass="form-control" required="required" />
                </div>
                <div class="form-group">
                    <label class="form-label" for="<%= txtNewEmail.ClientID %>">Email</label>
                    <asp:TextBox ID="txtNewEmail" runat="server" CssClass="form-control" TextMode="Email" required="required" />
                </div>
                <div class="form-group">
                    <label class="form-label" for="<%= txtNewPassword.ClientID %>">Password</label>
                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" required="required" />
                </div>
                <div class="form-group">
                    <label class="form-label" for="<%= ddlNewRole.ClientID %>">Role</label>
                    <asp:DropDownList ID="ddlNewRole" runat="server" CssClass="form-control" />
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn--outline" OnClick="btnCancel_Click" CausesValidation="false" />
                    <asp:Button ID="btnSave" runat="server" Text="Create User" CssClass="btn btn--primary" OnClick="btnSave_Click" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>