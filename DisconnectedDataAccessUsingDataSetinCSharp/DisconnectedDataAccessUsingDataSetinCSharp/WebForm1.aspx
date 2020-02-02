<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="DisconnectedDataAccessUsingDataSetinCSharp.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <asp:Button ID="BtnGetDataFromDB" runat="server" Text="Get Data From DB" OnClick="BtnGetDataFromDB_Click" />
        <p>
            <asp:GridView ID="GrdvStudents" runat="server" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2" AutoGenerateColumns="False" DataKeyNames="Id" OnRowCancelingEdit="GrdvStudents_RowCancelingEdit" OnRowDeleting="GrdvStudents_RowDeleting" OnRowEditing="GrdvStudents_RowEditing" OnRowUpdating="GrdvStudents_RowUpdating">
                <Columns>
                    <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
                    <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                    <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
                    <asp:BoundField DataField="LastName" HeaderText="LastName" SortExpression="LastName" />
                </Columns>
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#FFF1D4" />
                <SortedAscendingHeaderStyle BackColor="#B95C30" />
                <SortedDescendingCellStyle BackColor="#F1E5CE" />
                <SortedDescendingHeaderStyle BackColor="#93451F" />
            </asp:GridView>
            <asp:TextBox ID="TbxStudentsTableStatus" runat="server" Height="128px" TextMode="MultiLine" Visible="False" Width="305px"></asp:TextBox>
            <asp:Button ID="BtnHideStudentsTableStatus" runat="server" Font-Size="XX-Small" Height="20px" OnClick="BtnHideStudentsTableStatus_Click" Text="Hide" Visible="False" Width="33px" />
        </p>
        <p>
            <asp:GridView ID="GrdvStudentEmails" runat="server" AutoGenerateColumns="False" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2" DataKeyNames="Id" OnRowCancelingEdit="GrdvStudentEmails_RowCancelingEdit" OnRowDeleting="GrdvStudentEmails_RowDeleting" OnRowEditing="GrdvStudentEmails_RowEditing" OnRowUpdating="GrdvStudentEmails_RowUpdating">
                <Columns>
                    <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
                    <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                    <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                    <asp:BoundField DataField="StudentId" HeaderText="StudentId" SortExpression="StudentId" />
                </Columns>
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#FFF1D4" />
                <SortedAscendingHeaderStyle BackColor="#B95C30" />
                <SortedDescendingCellStyle BackColor="#F1E5CE" />
                <SortedDescendingHeaderStyle BackColor="#93451F" />
            </asp:GridView>
            <asp:TextBox ID="TbxStudentEmailTableStatus" runat="server" Height="128px" TextMode="MultiLine" Visible="False" Width="305px"></asp:TextBox>
            <asp:Button ID="BtnHideStudentEmailStatus" runat="server" Font-Size="XX-Small" Height="20px" OnClick="BtnHideStudentEmailStatus_Click" style="margin-right: 0px" Text="Hide" Visible="False" Width="34px" />
        </p>
        <p>
            <asp:Button ID="BtnAcceptChanges" runat="server" Font-Bold="True" ForeColor="#00CC00" OnClick="BtnAcceptChanges_Click" Text="Accept Changes" Visible="False" />
            <asp:Button ID="BtnRejectChanges" runat="server" Font-Bold="True" ForeColor="Red" OnClick="BtnRejectChanges_Click" Text="Reject Changes" Visible="False" />
        </p>
        <p>
            <asp:Label ID="LblMessage" runat="server" Font-Bold="True" ForeColor="Blue"></asp:Label>
        </p>
        <p>
            <asp:Button ID="BtnUpdateDB" runat="server" Text="Update DB" OnClick="BtnUpdateDB_Click" />
            <asp:Button ID="BtnGetTablesStatus" runat="server" OnClick="BtnGetTablesStatus_Click" Text="Get Tables Status" />
            </p>
        <p>
            <asp:Label ID="LblFirstName" runat="server" Text="First Name"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="TbxFirstName" runat="server"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="LblLastName" runat="server" Text="Last Name"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="TbxLastName" runat="server"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="LblEmail" runat="server" Text="Email"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="TbxEmail" runat="server"></asp:TextBox>
        </p>
        <p>
            <asp:Button ID="BtnAddStudent" runat="server" OnClick="BtnAddStudent_Click" Text="Add Student" />
        </p>
        <p>
            <asp:Label ID="LblStudentId" runat="server" Text="Student ID"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="TbxStudentId" runat="server"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="LblAdditionalEmail" runat="server" Text="Additional Email"></asp:Label>
&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="TbxAdditionalEmail" runat="server"></asp:TextBox>
        </p>
        <p>
            <asp:Button ID="BtnAddAdditionalEmail" runat="server" OnClick="BtnAddAdditionalEmail_Click" Text="Add Additional Email" />
        </p>
    </form>
</body>
</html>
