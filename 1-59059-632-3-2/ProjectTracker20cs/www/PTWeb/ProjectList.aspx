<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
  CodeFile="ProjectList.aspx.cs" Inherits="ProjectList" Title="Project List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div>
    <strong>Projects:<br />
    </strong>
    <asp:GridView ID="GridView1" runat="server" 
      AllowPaging="True" AutoGenerateColumns="False"
      DataSourceID="ProjectListDataSource" PageSize="4" 
      DataKeyNames="Id" OnRowDeleted="GridView1_RowDeleted">
      <Columns>
        <asp:BoundField DataField="Id" HeaderText="Id" 
          SortExpression="Id" Visible="False" />
        <asp:HyperLinkField DataNavigateUrlFields="Id" 
          DataNavigateUrlFormatString="ProjectEdit.aspx?id={0}"
          DataTextField="Name" HeaderText="Name" />
        <asp:CommandField ShowDeleteButton="True" 
          SelectText="Edit" />
      </Columns>
    </asp:GridView>
    <asp:LinkButton ID="NewProjectButton" runat="server" 
      OnClick="NewProjectButton_Click">New project</asp:LinkButton>
    <br />
    <br />
    <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red"></asp:Label>
    <csla:CslaDataSource ID="ProjectListDataSource" runat="server" 
      TypeName="ProjectTracker.Library.ProjectList"
      TypeAssemblyName="ProjectTracker.Library" 
      OnDeleteObject="ProjectListDataSource_DeleteObject"
      OnSelectObject="ProjectListDataSource_SelectObject">
    </csla:CslaDataSource>
    <br />
  </div>
</asp:Content>
