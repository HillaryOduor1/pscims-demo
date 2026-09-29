<%@ Page Title="Public Universities & Independent Offices" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PSCIMS.puio.Default" %>
<%@ Register Src="~/PuioContent/DefaultContent.ascx" TagPrefix="psc" TagName="PuioContent" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Public Universities &amp; Independent Offices</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <psc:PuioContent runat="server" />
</asp:Content>