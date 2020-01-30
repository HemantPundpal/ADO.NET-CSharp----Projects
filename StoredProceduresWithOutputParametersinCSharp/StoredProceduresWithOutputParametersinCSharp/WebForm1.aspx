<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="StoredProceduresWithOutputParametersinCSharp.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="LFirstName" runat="server" Text="First Name"></asp:Label>
&nbsp;&nbsp;
            <asp:TextBox ID="TbxFirstName" runat="server" Width="160px"></asp:TextBox>
            <br />
            <asp:Label ID="LLastName" runat="server" Text="Last Name"></asp:Label>
&nbsp;&nbsp;
            <asp:TextBox ID="TbxLastName" runat="server" Width="162px"></asp:TextBox>
            <br />
            <asp:Label ID="LEmail" runat="server" Text="Email"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="TbxEmail" runat="server" Width="163px"></asp:TextBox>
            <br />
            <asp:Button ID="BtnSubmitStudent" runat="server" OnClick="BtnSubmitStudent_Click" Text="Submit Student" />
            <br />
            <asp:Label ID="LMessage" runat="server" Text="Message"></asp:Label>
        </div>
    </form>
</body>
</html>
