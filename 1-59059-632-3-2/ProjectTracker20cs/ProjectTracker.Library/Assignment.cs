using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Validation;
using Csla.Data;
using System.Reflection;

namespace ProjectTracker.Library
{
  internal interface IHoldRoles
  {
    int Role { get; set;}
  }

  internal static class Assignment
  {

    #region Business Methods

    public static DateTime GetDefaultAssignedDate()
    {
      return DateTime.Today;
    }

    #endregion

    #region Validation Rules

    /// <summary>
    /// Ensure the Role property value exists
    /// in RoleList
    /// </summary>
    public static bool ValidRole(object target, RuleArgs e)
    {
      int role = ((IHoldRoles)target).Role;

      if (RoleList.GetList().ContainsKey(role))
        return true;
      else
      {
        e.Description = "Role must be in RoleList";
        return false;
      }
    }

    #endregion

    #region Data Access

    public static byte[] AddAssignment(
      SqlConnection cn, Guid projectId, int resourceId, 
      SmartDate assigned, int role)
    {
      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandText = "addAssignment";
        return DoAddUpdate(
          cm, projectId, resourceId, assigned, role);
      }
    }

    public static byte[] UpdateAssignment(SqlConnection cn,
      Guid projectId, int resourceId, SmartDate assigned, 
      int newRole, byte[] timestamp)
    {
      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandText = "updateAssignment";
        cm.Parameters.AddWithValue("@lastChanged", timestamp);
        return DoAddUpdate(
          cm, projectId, resourceId, assigned, newRole);
      }
    }

    private static byte[] DoAddUpdate(SqlCommand cm,
      Guid projectId, int resourceId, SmartDate assigned,
      int newRole)
    {
      cm.CommandType = CommandType.StoredProcedure;
      cm.Parameters.AddWithValue("@projectId", projectId);
      cm.Parameters.AddWithValue("@resourceId", resourceId);
      cm.Parameters.AddWithValue("@assigned", assigned.DBValue);
      cm.Parameters.AddWithValue("@role", newRole);
      SqlParameter param =
        new SqlParameter("@newLastChanged", SqlDbType.Timestamp);
      param.Direction = ParameterDirection.Output;
      cm.Parameters.Add(param);

      cm.ExecuteNonQuery();

      return (byte[])cm.Parameters["@newLastChanged"].Value;
    }

    public static void RemoveAssignment(
      SqlConnection cn, Guid projectId, int resourceId)
    {
      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "deleteAssignment";
        cm.Parameters.AddWithValue("@projectId", projectId);
        cm.Parameters.AddWithValue("@resourceId", resourceId);

        cm.ExecuteNonQuery();
      }
    }

    #endregion

  }
}
