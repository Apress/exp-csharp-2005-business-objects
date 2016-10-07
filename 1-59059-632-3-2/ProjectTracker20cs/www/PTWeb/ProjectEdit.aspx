<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
  CodeFile="ProjectEdit.aspx.cs" Inherits="ProjectEdit" Title="Project Information" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div>
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
      <asp:View ID="MainView" runat="server">
        <asp:DetailsView ID="DetailsView1" runat="server" Height="50px" Width="536px" AutoGenerateRows="False"
          DataSourceID="ProjectDataSource" DataKeyNames="Id" OnItemDeleted="DetailsView1_ItemDeleted"
          OnItemInserted="DetailsView1_ItemInserted" OnItemUpdated="DetailsView1_ItemUpdated">
          <Fields>
            <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id"
              InsertVisible="False" >
              <ItemStyle Width="100%" />
            </asp:BoundField>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" >
              <ControlStyle Width="95%" />
            </asp:BoundField>
            <asp:BoundField DataField="Started" HeaderText="Started" SortExpression="Started" >
              <ControlStyle Width="95%" />
            </asp:BoundField>
            <asp:BoundField DataField="Ended" HeaderText="Ended" SortExpression="Ended" >
              <ControlStyle Width="95%" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Description" SortExpression="Description">
              <EditItemTemplate>
                <asp:TextBox ID="TextBox1" TextMode="MultiLine" Width="95%" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
              </EditItemTemplate>
              <InsertItemTemplate>
                <asp:TextBox ID="TextBox1" TextMode="MultiLine" Width="95%" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
              </InsertItemTemplate>
              <ItemTemplate>
                <asp:TextBox ID="TextBox1" TextMode="MultiLine" ReadOnly="true" Width="95%" runat="server"
                  Text='<%# Bind("Description") %>'></asp:TextBox>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ShowInsertButton="True" />
          </Fields>
        </asp:DetailsView>
        &nbsp;&nbsp;&nbsp;<br />
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
          DataSourceID="ResourcesDataSource" Width="432px" DataKeyNames="ResourceId">
          <Columns>
            <asp:BoundField DataField="ResourceId" HeaderText="ResourceId" ReadOnly="True" SortExpression="ResourceId"
              Visible="False" />
            <asp:HyperLinkField DataNavigateUrlFields="ResourceId" DataNavigateUrlFormatString="ResourceEdit.aspx?id={0}"
              DataTextField="FullName" HeaderText="Name" />
            <asp:BoundField DataField="Assigned" HeaderText="Assigned" ReadOnly="True" SortExpression="Assigned" />
  <asp:TemplateField HeaderText="Role" SortExpression="Role">
    <EditItemTemplate>
      <asp:DropDownList ID="DropDownList1" runat="server" 
        DataSourceID="RoleListDataSource"
        DataTextField="Value" DataValueField="Key" 
        SelectedValue='<%# Bind("Role") %>'>
      </asp:DropDownList>
    </EditItemTemplate>
    <ItemTemplate>
      <asp:DropDownList ID="DropDownList2" runat="server" 
        DataSourceID="RoleListDataSource"
        DataTextField="Value" DataValueField="Key" 
        Enabled="False" SelectedValue='<%# Bind("Role") %>'>
      </asp:DropDownList>
    </ItemTemplate>
  </asp:TemplateField>
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
          </Columns>
        </asp:GridView>
        <asp:LinkButton ID="AddResourceButton" runat="server" OnClick="AddResourceButton_Click">Add resource</asp:LinkButton><br />
        <br />
        &nbsp;
      </asp:View>
      <asp:View ID="AssignView" runat="server">
        <strong>Pick resource:</strong><br />
        <asp:GridView ID="GridView2" runat="server" AllowPaging="True" AutoGenerateColumns="False"
          DataKeyNames="Id" DataSourceID="ResourceListDataSource" OnSelectedIndexChanged="GridView2_SelectedIndexChanged">
          <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" ReadOnly="True" SortExpression="Name" />
            <asp:CommandField ShowSelectButton="True" />
          </Columns>
        </asp:GridView>
        <asp:LinkButton ID="CancelAssignButton" runat="server" OnClick="CancelAssignButton_Click">Cancel</asp:LinkButton><br />
        <csla:CslaDataSource ID="ResourceListDataSource" runat="server" TypeAssemblyName="ProjectTracker.Library"
          TypeName="ProjectTracker.Library.ResourceList" OnSelectObject="ResourceListDataSource_SelectObject">
        </csla:CslaDataSource>
      </asp:View>
    </asp:MultiView>
    <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red"></asp:Label><br />
    &nbsp;<csla:CslaDataSource ID="ProjectDataSource" runat="server" TypeName="ProjectTracker.Library.Project"
      TypeAssemblyName="ProjectTracker.Library" OnDeleteObject="ProjectDataSource_DeleteObject"
      OnInsertObject="ProjectDataSource_InsertObject" OnSelectObject="ProjectDataSource_SelectObject"
      OnUpdateObject="ProjectDataSource_UpdateObject">
    </csla:CslaDataSource>
    <csla:CslaDataSource ID="ResourcesDataSource" runat="server" TypeName="ProjectTracker.Library.ProjectResources"
      TypeAssemblyName="ProjectTracker.Library" OnDeleteObject="ResourcesDataSource_DeleteObject"
      OnSelectObject="ResourcesDataSource_SelectObject" OnUpdateObject="ResourcesDataSource_UpdateObject">
    </csla:CslaDataSource>
    <csla:CslaDataSource ID="RoleListDataSource" runat="server" TypeName="ProjectTracker.Library.RoleList"
      TypeAssemblyName="ProjectTracker.Library" OnSelectObject="RoleListDataSource_SelectObject">
    </csla:CslaDataSource>
    <br />
  </div>
  <br />
  &nbsp;
</asp:Content>
