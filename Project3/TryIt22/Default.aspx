<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TryIt22._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Assignment 3, part 2.2 TryIt test page</h1>
    <p>Michael Sabin</p>

    <h2>Service URL</h2>
    <p>URL http://localhost/ </p>

    <h2>SolarIntensity</h2>
    <h3>Description</h3>
    <p>A service that returns the annual average sunshine index of a given position (latitude, longitude). This service can be used for deciding if installing solar energy device is effective at the location.</p>
    <h3>TryIt</h3>
    <p>Latitude: 
        <asp:TextBox ID="txtLat" runat="server"></asp:TextBox>
    </p>
    <p>Longitude: 
        <asp:TextBox ID="txtLon" runat="server"></asp:TextBox>
    </p>
    <p><asp:Button ID="btnLookupSolar" runat="server" Text="Lookup solar intensity" OnClick="btnLookupSolar_Click" />
    </p>
    <p><asp:Label ID="lblSolarIndex" runat="server"></asp:Label>
       </p>

    <hr />

    <h2>WordFilter</h2>
    <h3>Description</h3>
    <p>Analyze a string of words and filter out the function words (stop words) such as "a", "an", "in", "on", "the", "is", "are", "am", as well as the element tag names and attribute names quoted in angle brackets &lt;...&gt;, if the string represents an XML page or HTML source page.</p>
    <h3>TryIt</h3>
    <p>Text:</p>
    <asp:TextBox ID="txtText" runat="server" Rows="5" TextMode="MultiLine" Width="311px"></asp:TextBox>
    <p><asp:Button ID="btnFilter" runat="server" Text="Filter text" OnClick="btnFilter_Click" /></p>
    <p>Filtered text:</p>
    <asp:TextBox ID="txtFiltered" runat="server" Rows="5" TextMode="MultiLine" Width="318px"></asp:TextBox>

</asp:Content>
