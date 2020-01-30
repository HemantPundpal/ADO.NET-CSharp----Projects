<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="DQLCommandBuilderinCSharp.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <asp:Label ID="LblStudentId" runat="server" Text="Enter Student ID"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="TbxStudentId" runat="server"></asp:TextBox>
        <asp:Button ID="BtnLoadStudent" runat="server" Text="Load Student" OnClick="BtnLoadStudent_Click" />
        <asp:Label ID="LblMessage" runat="server"></asp:Label>
        <br />
        <asp:Label ID="LblStudentFirstName" runat="server" Text="Student First Name"></asp:Label>
&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="TbxStudentFirstName" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="LblStudentLastName0" runat="server" Text="Student Last Name"></asp:Label>
&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="TbxStudentLastName" runat="server"></asp:TextBox>
        <asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_Click" Text="Update" />
        <br />
        <br />
        <asp:Label ID="LblInsert" runat="server"></asp:Label>
        <br />
        <asp:Label ID="LblUpdate" runat="server"></asp:Label>
        <br />
        <asp:Label ID="LblDelete" runat="server"></asp:Label>
    </form>
</body>
</html>
