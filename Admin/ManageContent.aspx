<%@ Page Title="Manage Content" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageContent.aspx.cs" Inherits="PSCIMS.Admin.ManageContent" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Manage Content</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="mb-6">
        <h2 class="page-title">Manage Site Content</h2>
        <p class="page-sub">Select a page to edit its content.</p>
    </div>

    <asp:PlaceHolder ID="phMsg" runat="server" Visible="false">
        <div class="alert alert--success" role="alert">
            <asp:Literal ID="litMsg" runat="server" />
        </div>
    </asp:PlaceHolder>

    <!-- Full width dropdown card -->
    <div class="card mb-6" style="padding:18px 20px;">
        <div class="form-group" style="margin-bottom:0;">
            <label class="form-label" for="<%= ddlPages.ClientID %>">Select Page</label>
            <asp:DropDownList ID="ddlPages" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlPages_SelectedIndexChanged" style="width:100%;">
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

    <div class="table-wrap card" style="padding:0;">
        <asp:GridView ID="gvContent" runat="server" AutoGenerateColumns="false" 
            CssClass="table js-stack" GridLines="None" OnRowCommand="gvContent_RowCommand" DataKeyNames="ContentKey" style="margin:0; border:none;">
            
            <Columns>
                <asp:BoundField DataField="ContentKey" HeaderText="Key Name" ItemStyle-CssClass="key-name-cell" />
                <asp:TemplateField HeaderText="Content Value">
                    <HeaderStyle CssClass="hide-on-mobile" />
                    <ItemTemplate>
                        <span class="table__truncate" data-label="Value" style="display:block;"><%# Eval("ContentValue") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="LastUpdated" HeaderText="Updated" DataFormatString="{0:yyyy-MM-dd HH:mm}" ItemStyle-CssClass="hide-on-mobile" />
                <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="actions-cell">
                    <HeaderStyle CssClass="hide-on-mobile" />
                    <ItemTemplate>
                        <div style="width:100%;">
                            <asp:LinkButton runat="server" CommandName="EditContent" CommandArgument='<%# Eval("ContentKey") %>' Text="Edit Content" CssClass="btn btn--sm btn--outline" style="width:100%;" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <%-- Edit Modal --%>
    <asp:PlaceHolder ID="phForm" runat="server" Visible="false">
        <div id="contentModalOverlay" class="modal-overlay" onclick="closeContentModal(event);">
            <div class="modal" role="dialog" aria-modal="true" aria-labelledby="contentModalHeading" 
                 style="position:relative; display:flex; flex-direction:column; max-height:90vh;" onclick="event.stopPropagation();">
                
                <div style="flex-shrink:0; padding-bottom:16px; border-bottom:1px solid var(--border); margin-bottom:16px;">
                    <button type="button" aria-label="Close" 
                            onclick="closeContentModal(event);" 
                            style="position:absolute;top:16px;right:16px;background:transparent;border:none;color:#9ca3af;font-size:28px;line-height:1;cursor:pointer;padding:0 8px;z-index:100;">
                        &times;
                    </button>
                    <h3 class="card-title" id="contentModalHeading" style="margin:0;">Edit Content</h3>
                </div>

                <div style="flex:1; overflow-y:auto; padding-right:4px;">
                    <asp:HiddenField ID="hfContentKey" runat="server" />

                    <div class="form-group">
                        <label class="form-label" for="<%= txtKeyName.ClientID %>">Key Name</label>
                        <asp:TextBox ID="txtKeyName" runat="server" CssClass="form-control" ReadOnly="true" BackColor="#f0f0f0" />
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="<%= txtContentValue.ClientID %>">Content Value</label>
                        <asp:TextBox ID="txtContentValue" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6" />
                    </div>
                </div>

                <div style="flex-shrink:0; padding-top:16px; border-top:1px solid var(--border); display:flex; gap:12px; margin-top:16px;">
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn--outline" OnClick="btnCancel_Click" CausesValidation="false" formnovalidate="formnovalidate" style="flex:1;" />
                    <asp:Button ID="btnSave" runat="server" Text="Save Content" CssClass="btn btn--primary" OnClick="btnSave_Click" style="flex:1;" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <script>
        function closeContentModal(e) {
            if (e) e.preventDefault(); 
            var overlay = document.getElementById('contentModalOverlay');
            if (overlay) overlay.style.display = 'none'; 
            var cancelBtn = document.getElementById('<%= btnCancel.ClientID %>');
            if (cancelBtn) cancelBtn.click();
        }
    </script>
</asp:Content>