<%@ Page Title="Manage Adverts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageAdverts.aspx.cs" Inherits="PSCIMS.Admin.ManageAdverts" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Manage Adverts</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- Responsive Page Header -->
    <div class="flex-between mb-6" style="display:flex; flex-wrap:wrap; gap:16px; align-items:flex-start;">
        <div>
            <h2 class="page-title">Manage Job Adverts</h2>
            <p class="page-sub">Create, edit, or close job postings.</p>
        </div>
        <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn--primary" OnClick="btnAddNew_Click" Text="Add New Advert" style="flex-shrink:0;" />
    </div>

    <asp:PlaceHolder ID="phMsg" runat="server" Visible="false">
        <div class="alert alert--success" role="alert">
            <asp:Literal ID="litMsg" runat="server" />
        </div>
    </asp:PlaceHolder>

    <div class="table-wrap card" style="padding:0;">
        <asp:GridView ID="gvAdverts" runat="server" AutoGenerateColumns="false" CssClass="table js-stack" GridLines="None"
            OnRowCommand="gvAdverts_RowCommand" DataKeyNames="Id" style="margin:0; border:none;">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="hide-on-mobile" />
                <asp:TemplateField HeaderText="Title">
                    <HeaderStyle CssClass="hide-on-mobile" />
                    <ItemTemplate>
                        <span data-label="Title" style="font-weight:600; display:block;"><%# Eval("Title") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ClosesOn" HeaderText="Closes" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:TemplateField HeaderText="Status">
                    <HeaderStyle CssClass="hide-on-mobile" />
                    <ItemTemplate>
                        <%# Convert.ToBoolean(Eval("IsActive")) ? "<span class='badge badge--open'>Active</span>" : "<span class='badge badge--closed'>Closed</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="actions-cell">
                    <HeaderStyle CssClass="hide-on-mobile" />
                    <ItemTemplate>
                        <div style="display:flex; gap:8px; flex-wrap:wrap; justify-content:flex-start; width:100%;">
                            <asp:LinkButton runat="server" CommandName="EditAdvert" CommandArgument='<%# Eval("Id") %>' Text="Edit" CssClass="btn btn--sm btn--outline" style="flex:1;" />
                            <asp:LinkButton runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("Id") %>' Text="Toggle" CssClass="btn btn--sm btn--primary" style="flex:1;" />
                            <asp:LinkButton runat="server" CommandName="DeleteAdvert" CommandArgument='<%# Eval("Id") %>' Text="Delete" CssClass="btn btn--sm btn--outline" OnClientClick="return confirm('Are you sure you want to delete this advert?');" style="flex:1;" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <%-- Add/Edit Modal --%>
    <asp:PlaceHolder ID="phForm" runat="server" Visible="false">
        <div id="advertModalOverlay" class="modal-overlay" onclick="closeAdvertModal(event);">
            <div class="modal" role="dialog" aria-modal="true" aria-labelledby="advertModalHeading" 
                 style="position:relative; display:flex; flex-direction:column; max-height:90vh;" onclick="event.stopPropagation();">
                
                <div style="flex-shrink:0; padding-bottom:16px; border-bottom:1px solid var(--border); margin-bottom:16px;">
                    <button type="button" aria-label="Close" 
                            onclick="closeAdvertModal(event);" 
                            style="position:absolute;top:16px;right:16px;background:transparent;border:none;color:#9ca3af;font-size:28px;line-height:1;cursor:pointer;padding:0 8px;z-index:100;">
                        &times;
                    </button>
                    <h3 class="card-title" id="advertModalHeading" style="margin:0;">Create / Edit Advert</h3>
                </div>

                <div style="flex:1; overflow-y:auto; padding-right:4px;">
                    <asp:HiddenField ID="hfAdvertId" runat="server" />

                    <div class="form-group">
                        <label class="form-label" for="<%= txtTitle.ClientID %>">Title</label>
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" required="required" />
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="<%= txtDesc.ClientID %>">Description</label>
                        <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" required="required" />
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="<%= txtClosingDate.ClientID %>">Closing Date</label>
                        <asp:TextBox ID="txtClosingDate" runat="server" CssClass="form-control" TextMode="Date" required="required" />
                    </div>
                </div>

                <div style="flex-shrink:0; padding-top:16px; border-top:1px solid var(--border); display:flex; gap:12px; margin-top:16px;">
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn--outline" OnClick="btnCancel_Click" CausesValidation="false" formnovalidate="formnovalidate" style="flex:1;" />
                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn--primary" OnClick="btnSave_Click" style="flex:1;" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <script>
        function closeAdvertModal(e) {
            if (e) e.preventDefault(); 
            var overlay = document.getElementById('advertModalOverlay');
            if (overlay) overlay.style.display = 'none'; 
            var cancelBtn = document.getElementById('<%= btnCancel.ClientID %>');
            if (cancelBtn) cancelBtn.click();
        }
    </script>
</asp:Content>