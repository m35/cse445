<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TryIt22._Default" ValidateRequest="false" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Assignment 3, part 2.2 TryIt test page</h1>
    <p>Michael Sabin (team "League of extraordinary software developers")</p>

    <h3>Service URL</h3>
    <p>URL <a href="http://localhost:50948/Service1.svc?wsdl">http://localhost:50948/Service1.svc?wsdl</a> </p>

    <h3>SolarIntensity (required service)</h3>
    <h4>Description</h4>
    <pre>decimal SolarIntensity(decimal latitude, decimal longitude)</pre>
    <p>A service that returns the annual average sunshine index of a given position (latitude, longitude). This service can be used for deciding if installing solar energy device is effective at the location.</p>
    <h4>TryIt</h4>
    <div>
    <table border="1">
        <caption>Example cities</caption>
        <tr><th>City</th><th>Latitude</th><th>Longitude</th></tr>
        <tr><td>Phoenix, AZ</td><td>33.4484</td><td>-112.0740</td></tr>
        <tr><td>Seattle, WA</td><td>47.6062</td><td>-122.3321</td></tr>
        <tr><td>Austin, TX</td><td>30.2672</td><td>-97.7431</td></tr>
    </table>
    </div>
    <p></p>
    <p>Latitude (in decimal, -90 to 90): 
        <asp:TextBox ID="txtLat" runat="server"></asp:TextBox>
    </p>
    <p>Longitude (in decimal, -180 to 180): 
        <asp:TextBox ID="txtLon" runat="server"></asp:TextBox>
    </p>
    <p><asp:Button ID="btnLookupSolar" runat="server" Text="Lookup solar intensity" OnClick="btnLookupSolar_Click" />
    </p>
    <p><b><asp:Label ID="lblSolarIndex" runat="server">(will return decimal solar index value)</asp:Label></b>
       </p>

    <hr />

    <h3>WordFilter (required service)</h3>
    <h4>Description</h4>
    <pre>string WordFilter(string text)</pre>
    <p>Analyze a string of words and filter out the function words (stop words) such as "a", "an", "in", "on", "the", "is", "are", "am", as well as the element tag names and attribute names quoted in angle brackets &lt;...&gt;, if the string represents an XML page or HTML source page.</p>
    <h4>TryIt</h4>
    <p>Input text (string):</p>
    <asp:TextBox ID="txtText" runat="server" Rows="5" TextMode="MultiLine" Width="311px"></asp:TextBox>
    <p><asp:Button ID="btnFilter" runat="server" Text="Filter text" OnClick="btnFilter_Click" /></p>
    <p>Returned filtered text (string):</p>
    <asp:TextBox ID="txtFiltered" runat="server" Rows="5" TextMode="MultiLine" Width="318px"></asp:TextBox>

</asp:Content>
