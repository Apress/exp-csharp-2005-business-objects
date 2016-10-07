<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ResourceEdit.aspx.cs" Inherits="ResourceEdit" title="Resource Information" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <div>
        <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
          <asp:View ID="MainView" runat="server">
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False"
          DataSourceID="ResourceDataSource" Height="50px"
          Width="440px" DataKeyNames="Id" OnItemDeleted="DetailsView1_ItemDeleted" OnItemInserted="DetailsView1_ItemInserted" OnItemUpdated="DetailsView1_ItemUpdated">
          <Fields>
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False" >
              <ControlStyle Width="95%" />
            </asp:BoundField>
            <asp:BoundField DataField="LastName" HeaderText="LastName" SortExpression="LastName" >
              <ControlStyle Width="95%" />
            </asp:BoundField>
            <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" >
              <ControlStyle Width="95%" />
            </asp:BoundField>
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ShowInsertButton="True" />
          </Fields>
        </asp:DetailsView>
        <br />
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
          DataSourceID="AssignmentsDataSource" DataKeyNames="ProjectId">
          <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
          <Columns>
            <asp:BoundField DataField="ProjectID" HeaderText="ProjectID" SortExpression="ProjectID"
              Visible="False" />
            <asp:HyperLinkField DataNavigateUrlFields="ProjectId" DataNavigateUrlFormatString="ProjectEdit.aspx?id={0}"
              DataTextField="ProjectName" HeaderText="Project Name" />
            <asp:BoundField DataField="Assigned" HeaderText="Assigned" SortExpression="Assigned" ReadOnly="True" />
            <asp:TemplateField HeaderText="Role" SortExpression="Role">
              <EditItemTemplate><asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="RoleListDataSource" DataTextField="Value" DataValueField="Key" SelectedValue='<%# Bind("Role") %>' Width="184px">
              </asp:DropDownList>
              </EditItemTemplate>
              <ItemTemplate>
                <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="RoleListDataSource" DataTextField="Value" DataValueField="Key" Enabled="False" SelectedValue='<%# Bind("Role") %>' Width="184px">
                </asp:DropDownList>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
          </Columns>
        </asp:GridView>
            <asp:LinkButton ID="AssignProjectButton" runat="server" OnClick="AssignProjectButton_Click">Assign to project</asp:LinkButton></asp:View>
          <asp:View ID="AssignView" runat="server">
            <asp:GridView ID="GridView2" runat="server" AllowPaging="True" AutoGenerateColumns="False"
              DataSourceID="ProjectListDataSource" DataKeyNames="Id" OnSelectedIndexChanged="GridView2_SelectedIndexChanged">
              <Columns>
                <asp:BoundField DataField="Name" HeaderText="Name" ReadOnly="True" SortExpression="Name" />
                <asp:CommandField ShowSelectButton="True" />
              </Columns>
            </asp:GridView>
            <asp:LinkButton ID="CancelAssignButton" runat="server" OnClick="CancelAssignButton_Click">Cancel</asp:LinkButton></asp:View>
        </asp:MultiView>
        <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red"></asp:Label><br />
      <csla:CslaDataSource ID="ResourceDataSource" runat="server"
        TypeName="ProjectTracker.Library.Resource" TypeAssemblyName="ProjectTracker.Library" OnDeleteObject="ResourceDataSource_DeleteObject" OnInsertObject="ResourceDataSource_InsertObject" OnSelectObject="ResourceDataSource_SelectObject" OnUpdateObject="ResourceDataSource_UpdateObject"></csla:CslaDataSource>
      <csla:CslaDataSource ID="AssignmentsDataSource" runat="server"
        TypeName="ProjectTracker.Library.ResourceAssignments" TypeAssemblyName="ProjectTracker.Library" OnDeleteObject="AssignmentsDataSource_DeleteObject" OnSelectObject="AssignmentsDataSource_SelectObject" OnUpdateObject="AssignmentsDataSource_UpdateObject"></csla:CslaDataSource>
      <csla:CslaDataSource ID="RoleListDataSource" runat="server"
        TypeName="ProjectTracker.Library.RoleList" TypeAssemblyName="ProjectTracker.Library" OnSelectObject="RoleListDataSource_SelectObject"></csla:CslaDataSource>
        <csla:CslaDataSource ID="ProjectListDataSource" runat="server" TypeAssemblyName="ProjectTracker.Library"
          TypeName="ProjectTracker.Library.ProjectList" OnSelectObject="ProjectListDataSource_SelectObject">
        </csla:CslaDataSource>
       </div>
</asp:Content>