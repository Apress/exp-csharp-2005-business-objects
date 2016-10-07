using System;
using System.Web.UI;
using System.Web.UI.Design;
using System.ComponentModel;

namespace Csla.Web.Design
{

  /// <summary>
  /// Implements designer support for CslaDataSource.
  /// </summary>
  public class CslaDataSourceDesigner : DataSourceDesigner
  {

    private CslaDataSource _control = null;
    private CslaDesignerDataSourceView _view = null;

    /// <summary>
    /// Initialize the designer component.
    /// </summary>
    /// <param name="component">The CslaDataSource control to 
    /// be designed.</param>
    public override void Initialize(IComponent component)
    {
      base.Initialize(component);
      _control = (CslaDataSource)component;
    }

    /// <summary>
    /// Returns the default view for this designer.
    /// </summary>
    /// <param name="viewName">Ignored</param>
    /// <returns></returns>
    /// <remarks>
    /// This designer supports only a "Default" view.
    /// </remarks>
    public override DesignerDataSourceView GetView(string viewName)
    {
      if (_view == null)
        _view = new CslaDesignerDataSourceView(this, "Default");
      return _view;
    }

    /// <summary>
    /// Return a list of available views.
    /// </summary>
    /// <remarks>
    /// This designer supports only a "Default" view.
    /// </remarks>
    public override string[] GetViewNames()
    {
      return new string[] { "Default" };
    }

    /// <summary>
    /// Refreshes the schema for the data.
    /// </summary>
    /// <param name="preferSilent"></param>
    /// <remarks></remarks>
    public override void RefreshSchema(bool preferSilent)
    {
      this.OnSchemaRefreshed(EventArgs.Empty);
    }

    /// <summary>
    /// Get a value indicating whether the control can
    /// refresh its schema.
    /// </summary>
    public override bool CanRefreshSchema
    {
      get { return true; }
    }

    /// <summary>
    /// Get a value indicating whether the control can
    /// be resized.
    /// </summary>
    public override bool AllowResize
    {
      get { return false; }
    }

    /// <summary>
    /// Get a reference to the CslaDataSource control being
    /// designed.
    /// </summary>
    internal CslaDataSource DataSourceControl
    {
      get { return _control; }
    }

  }
}
