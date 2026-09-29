<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PSCIMS.jobs.Default" %>
<%@ Register Src="~/JobsContent/DefaultContent.ascx" TagPrefix="psc" TagName="DefaultContent" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Dashboard</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <psc:DefaultContent runat="server" />
</asp:Content>