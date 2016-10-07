using System;
using System.Reflection;
using System.Security.Principal;
using System.Collections.Specialized;
using Csla.Properties;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side DataPortal 
  /// message router as discussed
  /// in Chapter 4.
  /// </summary>
  public class DataPortal : IDataPortalServer
  {

    #region Data Access

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Create(
      Type objectType, object criteria, DataPortalContext context)
    {
      try
      {
        SetContext(context);

        DataPortalResult result;

        MethodInfo method = MethodCaller.GetMethod(
          objectType, "DataPortal_Create", criteria);

        IDataPortalServer portal;
        switch (TransactionalType(method))
        {
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = portal.Create(objectType, criteria, context);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }

            break;
          case TransactionalTypes.TransactionScope:

            portal = new TransactionalDataPortal();
            result = portal.Create(objectType, criteria, context);

            break;
          default:
            portal = new SimpleDataPortal();
            result = portal.Create(objectType, criteria, context);
            break;
        }
        return result;
      }
      finally
      {
        ClearContext(context);
      }
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Fetch(object criteria, DataPortalContext context)
    {
      try
      {
        SetContext(context);

        DataPortalResult result;

        MethodInfo method = MethodCaller.GetMethod(
          MethodCaller.GetObjectType(criteria), "DataPortal_Fetch", criteria);

        IDataPortalServer portal;
        switch (TransactionalType(method))
        {
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = portal.Fetch(criteria, context);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }
            break;
          case TransactionalTypes.TransactionScope:
            portal = new TransactionalDataPortal();
            result = portal.Fetch(criteria, context);
            break;
          default:
            portal = new SimpleDataPortal();
            result = portal.Fetch(criteria, context);
            break;
        }
        return result;
      }
      finally
      {
        ClearContext(context);
      }
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      try
      {
        SetContext(context);

        DataPortalResult result;

        MethodInfo method;
        string methodName;
        if (obj is CommandBase)
          methodName = "DataPortal_Execute";
        else if (obj is Core.BusinessBase)
        {
          Core.BusinessBase tmp = (Core.BusinessBase)obj;
          if (tmp.IsDeleted)
            methodName = "DataPortal_DeleteSelf";
          else
            if (tmp.IsNew)
              methodName = "DataPortal_Insert";
            else
              methodName = "DataPortal_Update";
        }
        else
          methodName = "DataPortal_Update";

        method = MethodCaller.GetMethod(obj.GetType(), methodName);

        IDataPortalServer portal;
        switch (TransactionalType(method))
        {
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = portal.Update(obj, context);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }
            break;
          case TransactionalTypes.TransactionScope:
            portal = new TransactionalDataPortal();
            result = portal.Update(obj, context);
            break;
          default:
            portal = new SimpleDataPortal();
            result = portal.Update(obj, context);
            break;
        }
        return result;
      }
      finally
      {
        ClearContext(context);
      }
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Delete(object criteria, DataPortalContext context)
    {
      try
      {
        SetContext(context);

        DataPortalResult result;

        MethodInfo method = MethodCaller.GetMethod(
          MethodCaller.GetObjectType(criteria), "DataPortal_Delete", criteria);

        IDataPortalServer portal;
        switch (TransactionalType(method))
        {
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = portal.Delete(criteria, context);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }
            break;
          case TransactionalTypes.TransactionScope:
            portal = new TransactionalDataPortal();
            result = portal.Delete(criteria, context);
            break;
          default:
            portal = new SimpleDataPortal();
            result = portal.Delete(criteria, context);
            break;
        }
        return result;
      }
      finally
      {
        ClearContext(context);
      }
    }

    #endregion

    #region Context

    private static void SetContext(DataPortalContext context)
    {
      // if the dataportal is not remote then
      // do nothing
      if (!context.IsRemotePortal) return;

      // set the app context to the value we got from the
      // client
      ApplicationContext.SetContext(context.ClientContext, context.GlobalContext);

      // set the context value so everyone knows the
      // code is running on the server
      ApplicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);

      // set the thread's culture to match the client
      System.Threading.Thread.CurrentThread.CurrentCulture = 
        new System.Globalization.CultureInfo(context.ClientCulture);
      System.Threading.Thread.CurrentThread.CurrentUICulture = 
        new System.Globalization.CultureInfo(context.ClientUICulture);

      if (ApplicationContext.AuthenticationType == "Windows")
      {
        // When using integrated security, Principal must be null
        if (context.Principal == null)
        {
          // Set .NET to use integrated security
          AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
          return;
        }
        else
        {
          throw new System.Security.SecurityException(Resources.NoPrincipalAllowedException);
        }
      }
      // We expect the Principal to be of the type BusinesPrincipal
      if (context.Principal != null)
      {
        if (context.Principal is Security.BusinessPrincipalBase)
          ApplicationContext.User = context.Principal;
        else
          throw new System.Security.SecurityException(
            Resources.BusinessPrincipalException + " " + 
            ((object)context.Principal).ToString());
      }
      else
        throw new System.Security.SecurityException(
          Resources.BusinessPrincipalException + " Nothing");
    }

    private static void ClearContext(DataPortalContext context)
    {
      // if the dataportal is not remote then
      // do nothing
      if (!context.IsRemotePortal) return;
      ApplicationContext.Clear();
      if (ApplicationContext.AuthenticationType != "Windows")
        ApplicationContext.User = null;
    }

    #endregion

    #region Helper methods

    private static bool IsTransactionalMethod(MethodInfo method)
    {
      return Attribute.IsDefined(method, typeof(TransactionalAttribute));
    }

    private static TransactionalTypes TransactionalType(MethodInfo method)
    {
      TransactionalTypes result;
      if (IsTransactionalMethod(method))
      {
        TransactionalAttribute attrib =
          (TransactionalAttribute)Attribute.GetCustomAttribute(
          method, typeof(TransactionalAttribute));
        result = attrib.TransactionType;
      }
      else
        result = TransactionalTypes.Manual;
      return result;
    }

    #endregion

  }
}