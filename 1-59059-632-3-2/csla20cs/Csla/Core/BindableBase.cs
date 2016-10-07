using System;
using System.ComponentModel;
using System.Reflection;

namespace Csla.Core
{
  /// <summary>
  /// This class implements INotifyPropertyChanged
  /// in a serialization-safe manner.
  /// </summary>
  [Serializable()]
  public abstract class BindableBase : System.ComponentModel.INotifyPropertyChanged
  {
    [NonSerialized()]
    private PropertyChangedEventHandler _nonSerializableHandlers;
    private PropertyChangedEventHandler _serializableHandlers;

    protected BindableBase()
    {

    }

    /// <summary>
    /// Implements a serialization-safe PropertyChanged event.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1062:ValidateArgumentsOfPublicMethods")]
    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
        if (value.Method.IsPublic && 
           (value.Method.DeclaringType.IsSerializable || 
            value.Method.IsStatic))
          _serializableHandlers = (PropertyChangedEventHandler)
            System.Delegate.Combine(_serializableHandlers, value);
        else
          _nonSerializableHandlers = (PropertyChangedEventHandler)
            System.Delegate.Combine(_nonSerializableHandlers, value);
      }
      remove
      {
        if (value.Method.IsPublic && 
           (value.Method.DeclaringType.IsSerializable || 
            value.Method.IsStatic))
          _serializableHandlers = (PropertyChangedEventHandler)
            System.Delegate.Remove(_serializableHandlers, value);
        else
          _nonSerializableHandlers = (PropertyChangedEventHandler)
            System.Delegate.Remove(_nonSerializableHandlers, value);
      }
    }

    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for all object properties.
    /// </summary>
    /// <remarks>
    /// This method is for backward compatibility with
    /// CSLA .NET 1.x.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnIsDirtyChanged()
    {
      OnUnknownPropertyChanged();
    }

    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for all object properties.
    /// </summary>
    /// <remarks>
    /// This method is automatically called by MarkDirty.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnUnknownPropertyChanged()
    {
      PropertyInfo [] properties = 
        this.GetType().GetProperties(
        BindingFlags.Public | BindingFlags.Instance);
      foreach (PropertyInfo item in properties)
        OnPropertyChanged(item.Name);
    }

    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for a specific property.
    /// </summary>
    /// <remarks>
    /// This method may be called by properties in the business
    /// class to indicate the change in a specific property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler nonSerializableHandlers =
        _nonSerializableHandlers;
      if (nonSerializableHandlers != null)
        nonSerializableHandlers.Invoke(this,
          new PropertyChangedEventArgs(propertyName));
      PropertyChangedEventHandler serializableHandlers =
        _serializableHandlers;
      if (serializableHandlers != null)
        serializableHandlers.Invoke(this,
          new PropertyChangedEventArgs(propertyName));
    }
  }
}