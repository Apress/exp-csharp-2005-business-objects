using System;
using Csla.Properties;

namespace Csla
{

  /// <summary>
  /// This is the base class from which most business objects
  /// will be derived.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class is the core of the CSLA .NET framework. To create
  /// a business object, inherit from this class.
  /// </para><para>
  /// Please refer to 'Expert C# 2005 Business Objects' for
  /// full details on the use of this base class to create business
  /// objects.
  /// </para>
  /// </remarks>
  /// <typeparam name="T">Type of the business object being defined.</typeparam>
  [Serializable()]
  public abstract class BusinessBase<T> : 
    Core.BusinessBase where T : BusinessBase<T>
  {

    #region Object ID Value

    /// <summary>
    /// Override this method to return a unique identifying
    /// value for this object.
    /// </summary>
    /// <remarks>
    /// If you can not provide a unique identifying value, it
    /// is best if you can generate such a unique value (even
    /// temporarily). If you can not do that, then return 
    /// <see langword="Nothing"/> and then manually override the
    /// <see cref="Equals"/>, <see cref="GetHashCode"/> and
    /// <see cref="ToString"/> methods in your business object.
    /// </remarks>
    protected abstract object GetIdValue();

    #endregion

    #region System.Object Overrides

    /// <summary>
    /// Compares this object for equality with another object, using
    /// the results of <see cref="GetIdValue"/> to determine
    /// equality.
    /// </summary>
    /// <param name="obj">The object to be compared.</param>
    public override bool Equals(object obj)
    {
      if (obj is T)
      {
        object id = GetIdValue();
        if (id == null)
          throw new ArgumentException(Resources.GetIdValueCantBeNull);
        return ((T)obj).GetIdValue().Equals(id);
      }
      else
        return false;
    }

    /// <summary>
    /// Returns a hash code value for this object, based on
    /// the results of <see cref="GetIdValue"/>.
    /// </summary>
    public override int GetHashCode()
    {
      object id = GetIdValue();
      if (id == null)
        throw new ArgumentException(Resources.GetIdValueCantBeNull);
      return id.GetHashCode();
    }

    /// <summary>
    /// Returns a text representation of this object by
    /// returning the <see cref="GetIdValue"/> value
    /// in text form.
    /// </summary>
    public override string ToString()
    {
      object id = GetIdValue();
      if (id == null)
        throw new ArgumentException(Resources.GetIdValueCantBeNull);
      return id.ToString();
    }

    #endregion

    #region Clone

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>
    /// A new object containing the exact data of the original object.
    /// </returns>
    public virtual T Clone()
    {
      return (T)GetClone();
    }

    #endregion

    #region Data Access

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Calling this method starts the save operation, causing the object
    /// to be inserted, updated or deleted within the database based on the
    /// object's current state.
    /// </para><para>
    /// If <see cref="Core.BusinessBase.IsDeleted" /> is <see langword="true"/>
    /// the object will be deleted. Otherwise, if <see cref="Core.BusinessBase.IsNew" /> 
    /// is <see langword="true"/> the object will be inserted. 
    /// Otherwise the object's data will be updated in the database.
    /// </para><para>
    /// All this is contingent on <see cref="Core.BusinessBase.IsDirty" />. If
    /// this value is <see langword="false"/>, no data operation occurs. 
    /// It is also contingent on <see cref="Core.BusinessBase.IsValid" />. 
    /// If this value is <see langword="false"/> an
    /// exception will be thrown to indicate that the UI attempted to save an
    /// invalid object.
    /// </para><para>
    /// It is important to note that this method returns a new version of the
    /// business object that contains any data updated during the save operation.
    /// You MUST update all object references to use this new version of the
    /// business object in order to have access to the correct object data.
    /// </para><para>
    /// You can override this method to add your own custom behaviors to the save
    /// operation. For instance, you may add some security checks to make sure
    /// the user can save the object. If all security checks pass, you would then
    /// invoke the base Save method via <c>base.Save()</c>.
    /// </para>
    /// </remarks>
    /// <returns>A new object containing the saved values.</returns>
    public virtual T Save()
    {
      if (this.IsChild)
        throw new NotSupportedException(Resources.NoSaveChildException);
      if (EditLevel > 0)
        throw new Validation.ValidationException(Resources.NoSaveEditingException);
      if (!IsValid)
        throw new Validation.ValidationException(Resources.NoSaveInvalidException);
      if (IsDirty)
        return (T)DataPortal.Update(this);
      else
        return (T)this;
    }

    /// <summary>
    /// Saves the object to the database, forcing
    /// IsNew to <see langword="false"/> and IsDirty to True.
    /// </summary>
    /// <param name="forceUpdate">
    /// If <see langword="true"/>, triggers overriding IsNew and IsDirty. 
    /// If <see langword="false"/> then it is the same as calling Save().
    /// </param>
    /// <returns>A new object containing the saved values.</returns>
    /// <remarks>
    /// This overload is designed for use in web applications
    /// when implementing the Update method in your 
    /// data wrapper object.
    /// </remarks>
    public T Save(bool forceUpdate)
    {
      if (forceUpdate && IsNew)
      {
        // mark the object as old - which makes it
        // not dirty
        MarkOld();
        // now mark the object as dirty so it can save
        MarkDirty(true);
      }
      return this.Save();
    }

    #endregion

  }
}
