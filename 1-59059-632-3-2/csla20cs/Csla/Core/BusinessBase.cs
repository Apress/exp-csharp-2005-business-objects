using System;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.Serialization;
using Csla.Properties;

namespace Csla.Core
{

  /// <summary>
  /// This is the non-generic base class from which most
  /// business objects will be derived.
  /// </summary>
  /// <remarks>
  /// See Chapter 3 for details.
  /// </remarks>
  [Serializable()]
  public abstract class BusinessBase : Csla.Core.UndoableBase,
    System.ComponentModel.IEditableObject, System.ComponentModel.IDataErrorInfo, 
    ICloneable
  {

    #region Constructors

    protected BusinessBase()
    {
      AddBusinessRules();
      AddAuthorizationRules();
    }

    #endregion

    #region IsNew, IsDeleted, IsDirty, IsSavable

    // keep track of whether we are new, deleted or dirty
    private bool _isNew = true;
    private bool _isDeleted;
    private bool _isDirty = true;

    /// <summary>
    /// Returns <see langword="true" /> if this is a new object, 
    /// <see langword="false" /> if it is a pre-existing object.
    /// </summary>
    /// <remarks>
    /// An object is considered to be new if its primary identifying (key) value 
    /// doesn't correspond to data in the database. In other words, 
    /// if the data values in this particular
    /// object have not yet been saved to the database the object is considered to
    /// be new. Likewise, if the object's data has been deleted from the database
    /// then the object is considered to be new.
    /// </remarks>
    /// <returns>A value indicating if this object is new.</returns>
    [Browsable(false)]
    public bool IsNew
    {
      get { return _isNew; }
    }

    /// <summary>
    /// Returns <see langword="true" /> if this object is marked for deletion.
    /// </summary>
    /// <remarks>
    /// CSLA .NET supports both immediate and deferred deletion of objects. This
    /// property is part of the support for deferred deletion, where an object
    /// can be marked for deletion, but isn't actually deleted until the object
    /// is saved to the database. This property indicates whether or not the
    /// current object has been marked for deletion. If it is <see langword="true" />
    /// , the object will
    /// be deleted when it is saved to the database, otherwise it will be inserted
    /// or updated by the save operation.
    /// </remarks>
    /// <returns>A value indicating if this object is marked for deletion.</returns>
    [Browsable(false)]
    public bool IsDeleted
    {
      get { return _isDeleted; }
    }

    /// <summary>
    /// Returns <see langword="true" /> if this object's data has been changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an object's data is changed, CSLA .NET makes note of that change
    /// and considers the object to be 'dirty' or changed. This value is used to
    /// optimize data updates, since an unchanged object does not need to be
    /// updated into the database. All new objects are considered dirty. All objects
    /// marked for deletion are considered dirty.
    /// </para><para>
    /// Once an object's data has been saved to the database (inserted or updated)
    /// the dirty flag is cleared and the object is considered unchanged. Objects
    /// newly loaded from the database are also considered unchanged.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if this object's data has been changed.</returns>
    [Browsable(false)]
    public virtual bool IsDirty
    {
      get { return _isDirty; }
    }

    /// <summary>
    /// Marks the object as being a new object. This also marks the object
    /// as being dirty and ensures that it is not marked for deletion.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Newly created objects are marked new by default. You should call
    /// this method in the implementation of DataPortal_Update when the
    /// object is deleted (due to being marked for deletion) to indicate
    /// that the object no longer reflects data in the database.
    /// </para><para>
    /// If you override this method, make sure to call the base
    /// implementation after executing your new code.
    /// </para>
    /// </remarks>
    protected virtual void MarkNew()
    {
      _isNew = true;
      _isDeleted = false;
      MarkDirty();
    }

    /// <summary>
    /// Marks the object as being an old (not new) object. This also
    /// marks the object as being unchanged (not dirty).
    /// </summary>
    /// <remarks>
    /// <para>
    /// You should call this method in the implementation of
    /// DataPortal_Fetch to indicate that an existing object has been
    /// successfully retrieved from the database.
    /// </para><para>
    /// You should call this method in the implementation of 
    /// DataPortal_Update to indicate that a new object has been successfully
    /// inserted into the database.
    /// </para><para>
    /// If you override this method, make sure to call the base
    /// implementation after executing your new code.
    /// </para>
    /// </remarks>
    protected virtual void MarkOld()
    {
      _isNew = false;
      MarkClean();
    }

    /// <summary>
    /// Marks an object for deletion. This also marks the object
    /// as being dirty.
    /// </summary>
    /// <remarks>
    /// You should call this method in your business logic in the
    /// case that you want to have the object deleted when it is
    /// saved to the database.
    /// </remarks>
    protected void MarkDeleted()
    {
      _isDeleted = true;
      MarkDirty();
    }

    /// <summary>
    /// Marks an object as being dirty, or changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You should call this method in your business logic any time
    /// the object's internal data changes. Any time any instance
    /// variable changes within the object, this method should be called
    /// to tell CSLA .NET that the object's data has been changed.
    /// </para><para>
    /// Marking an object as dirty does two things. First it ensures
    /// that CSLA .NET will properly save the object as appropriate. Second,
    /// it causes CSLA .NET to tell Windows Forms data binding that the
    /// object's data has changed so any bound controls will update to
    /// reflect the new values.
    /// </para>
    /// </remarks>
    protected void MarkDirty()
    {
      MarkDirty(false);
    }

    /// <summary>
    /// Marks an object as being dirty, or changed.
    /// </summary>
    /// <param name="supressEvent">
    /// <see langword="true" /> to supress the PropertyChanged event that is otherwise
    /// raised to indicate that the object's state has changed.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void MarkDirty(bool suppressEvent)
    {
      _isDirty = true;
      if (!suppressEvent)
        OnUnknownPropertyChanged();
    }

    /// <summary>
    /// Performs processing required when the current
    /// property has changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method calls CheckRules(propertyName), MarkDirty and
    /// OnPropertyChanged(propertyName). MarkDirty is called such
    /// that no event is raised for IsDirty, so only the specific
    /// property changed event for the current property is raised.
    /// </para><para>
    /// This implementation uses System.Diagnostics.StackTrace to
    /// determine the name of the current property, and so must be called
    /// directly from the property to be checked.
    /// </para>
    /// </remarks>
    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    protected void PropertyHasChanged()
    {
      string propertyName = 
        new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
      PropertyHasChanged(propertyName);
    }

    /// <summary>
    /// Performs processing required when a property
    /// has changed.
    /// </summary>
    /// <remarks>
    /// This method calls CheckRules(propertyName), MarkDirty and
    /// OnPropertyChanged(propertyName). MarkDirty is called such
    /// that no event is raised for IsDirty, so only the specific
    /// property changed event for the current property is raised.
    /// </remarks>
    protected virtual void PropertyHasChanged(string propertyName)
    {
      ValidationRules.CheckRules(propertyName);
      MarkDirty(true);
      OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// Forces the object's IsDirty flag to <see langword="false" />.
    /// </summary>
    /// <remarks>
    /// This method is normally called automatically and is
    /// not intended to be called manually.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void MarkClean()
    {
      _isDirty = false;
      OnUnknownPropertyChanged();
    }

    /// <summary>
    /// Returns <see langword="true" /> if this object is both dirty and valid.
    /// </summary>
    /// <remarks>
    /// An object is considered dirty (changed) if 
    /// <see cref="P:Csla.BusinessBase.IsDirty" /> returns <see langword="true" />. It is
    /// considered valid if IsValid
    /// returns <see langword="true" />. The IsSavable property is
    /// a combination of these two properties. 
    /// </remarks>
    /// <returns>A value indicating if this object is both dirty and valid.</returns>
    [Browsable(false)]
    public virtual bool IsSavable
    {
      get { return (IsDirty && IsValid); }
    }

    #endregion

    #region Authorization

    [NotUndoable()]
    private Security.AuthorizationRules _authorizationRules;

    /// <summary>
    /// Override this method to add authorization
    /// rules for your object's properties.
    /// </summary>
    protected virtual void AddAuthorizationRules()
    {

    }

    /// <summary>
    /// Provides access to the AuthorizationRules object for this
    /// object.
    /// </summary>
    /// <remarks>
    /// Use this object to add a list of allowed and denied roles for
    /// reading and writing properties of the object. Typically these
    /// values are added once when the business object is instantiated.
    /// </remarks>
    protected Security.AuthorizationRules AuthorizationRules
    {
      get 
      { 
        if (_authorizationRules == null)
          _authorizationRules = new Security.AuthorizationRules();
        return _authorizationRules; 
      }
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if read is allowed.</returns>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public bool CanReadProperty(bool throwOnFalse)
    {
      string propertyName = 
        new System.Diagnostics.StackTrace().
        GetFrame(1).GetMethod().Name.Substring(4);
      bool result = CanReadProperty(propertyName);
      if (throwOnFalse && result == false)
        throw new System.Security.SecurityException(
          String.Format("{0} ({1})", 
          Resources.PropertyGetNotAllowed, propertyName));
      return result;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if read is allowed.</returns>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    public bool CanReadProperty(string propertyName, bool throwOnFalse)
    {
      bool result = CanReadProperty(propertyName);
      if (throwOnFalse && result == false)
        throw new System.Security.SecurityException(
          String.Format("{0} ({1})",
          Resources.PropertyGetNotAllowed, propertyName));
      return result;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if read is allowed.</returns>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public bool CanReadProperty()
    {
      string propertyName = 
        new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
      return CanReadProperty(propertyName);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to read the
    /// specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns><see langword="true" /> if read is allowed.</returns>
    /// <remarks>
    /// <para>
    /// If a list of allowed roles is provided then only users in those
    /// roles can read. If no list of allowed roles is provided then
    /// the list of denied roles is checked.
    /// </para><para>
    /// If a list of denied roles is provided then users in the denied
    /// roles are denied read access. All other users are allowed.
    /// </para><para>
    /// If neither a list of allowed nor denied roles is provided then
    /// all users will have read access.
    /// </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanReadProperty(string propertyName)
    {
      bool result = true;
      if (AuthorizationRules.HasReadAllowedRoles(propertyName))
      {
        // some users are explicitly granted read access
        // in which case all other users are denied.
        if (!AuthorizationRules.IsReadAllowed(propertyName))
          result = false;
      }
      else if (AuthorizationRules.HasReadDeniedRoles(propertyName))
      {
        // some users are explicitly denied read access.
        if (AuthorizationRules.IsReadDenied(propertyName))
          result = false;
      }
      return result;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to write the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if write is allowed.</returns>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public bool CanWriteProperty(bool throwOnFalse)
    {
      string propertyName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
      bool result = CanWriteProperty(propertyName);
      if (throwOnFalse && result == false)
        throw new System.Security.SecurityException(
          String.Format("{0} ({1})", Resources.PropertySetNotAllowed, propertyName));
      return result;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to write the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if write is allowed.</returns>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>
    public bool CanWriteProperty(string propertyName, bool throwOnFalse)
    {
      bool result = CanWriteProperty(propertyName);
      if (throwOnFalse && result == false)
        throw new System.Security.SecurityException(
          String.Format("{0} ({1})", Resources.PropertySetNotAllowed, propertyName));
      return result;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to write the
    /// calling property.
    /// </summary>
    /// <returns><see langword="true" /> if write is allowed.</returns>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public bool CanWriteProperty()
    {
      string propertyName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
      return CanWriteProperty(propertyName);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to write the
    /// specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <returns><see langword="true" /> if write is allowed.</returns>
    /// <remarks>
    /// <para>
    /// If a list of allowed roles is provided then only users in those
    /// roles can write. If no list of allowed roles is provided then
    /// the list of denied roles is checked.
    /// </para><para>
    /// If a list of denied roles is provided then users in the denied
    /// roles are denied write access. All other users are allowed.
    /// </para><para>
    /// If neither a list of allowed nor denied roles is provided then
    /// all users will have write access.
    /// </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanWriteProperty(string propertyName)
    {
      bool result = true;
      if (AuthorizationRules.GetRolesForProperty(
        propertyName, Csla.Security.AccessType.WriteAllowed).Length > 0)
      {
        // some users are explicitly granted write access
        // in which case all other users are denied
        if (!AuthorizationRules.IsWriteAllowed(propertyName))
          result = false;
      }
      else if (AuthorizationRules.GetRolesForProperty(
        propertyName, Csla.Security.AccessType.WriteDenied).Length > 0)
      {
        // some users are explicitly denied write access
        if (AuthorizationRules.IsWriteDenied(propertyName))
          result = false;
      }
      return result;
    }

    #endregion

    #region Parent/Child link

    [NotUndoable()]
    [NonSerialized()]
    private Core.IEditableCollection _parent;

    /// <summary>
    /// Provide access to the parent reference for use
    /// in child object code.
    /// </summary>
    /// <remarks>
    /// This value will be Nothing for root objects.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected Core.IEditableCollection Parent
    {
      get { return _parent; }
    }

    /// <summary>
    /// Used by BusinessListBase as a child object is 
    /// created to tell the child object about its
    /// parent.
    /// </summary>
    /// <param name="parent">A reference to the parent collection object.</param>
    internal void SetParent(Core.IEditableCollection parent)
    {
      if (!IsChild)
        throw new InvalidOperationException(Resources.ParentSetException);
      _parent = parent;
    }

    #endregion

    #region System.ComponentModel.IEditableObject

    [NotUndoable()]
    private bool _bindingEdit;
    private bool _neverCommitted = true;

    /// <summary>
    /// Allow data binding to start a nested edit on the object.
    /// </summary>
    /// <remarks>
    /// Data binding may call this method many times. Only the first
    /// call should be honored, so we have extra code to detect this
    /// and do nothing for subsquent calls.
    /// </remarks>
    void System.ComponentModel.IEditableObject.BeginEdit()
    {
      if (!_bindingEdit)
        BeginEdit();
    }

    /// <summary>
    /// Allow data binding to cancel the current edit.
    /// </summary>
    /// <remarks>
    /// Data binding may call this method many times. Only the first
    /// call to either IEditableObject.CancelEdit or 
    /// IEditableObject.EndEdit
    /// should be honored. We include extra code to detect this and do
    /// nothing for subsequent calls.
    /// </remarks>
    void System.ComponentModel.IEditableObject.CancelEdit()
    {
      if (_bindingEdit)
      {
        CancelEdit();
        if (IsNew && _neverCommitted && EditLevel <= EditLevelAdded)
        {
          // we're new and no EndEdit or ApplyEdit has ever been
          // called on us, and now we've been cancelled back to
          // where we were added so we should have ourselves
          // removed from the parent collection
          if (Parent != null)
            Parent.RemoveChild(this);
        }
      }
    }

    /// <summary>
    /// Allow data binding to apply the current edit.
    /// </summary>
    /// <remarks>
    /// Data binding may call this method many times. Only the first
    /// call to either IEditableObject.EndEdit or 
    /// IEditableObject.CancelEdit
    /// should be honored. We include extra code to detect this and do
    /// nothing for subsequent calls.
    /// </remarks>
    void System.ComponentModel.IEditableObject.EndEdit()
    {
      if (_bindingEdit)
        ApplyEdit();
    }

    #endregion

    #region Begin/Cancel/ApplyEdit

    /// <summary>
    /// Starts a nested edit on the object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When this method is called the object takes a snapshot of
    /// its current state (the values of its variables). This snapshot
    /// can be restored by calling CancelEdit
    /// or committed by calling ApplyEdit.
    /// </para><para>
    /// This is a nested operation. Each call to BeginEdit adds a new
    /// snapshot of the object's state to a stack. You should ensure that 
    /// for each call to BeginEdit there is a corresponding call to either 
    /// CancelEdit or ApplyEdit to remove that snapshot from the stack.
    /// </para><para>
    /// See Chapters 2 and 3 for details on n-level undo and state stacking.
    /// </para>
    /// </remarks>
    public void BeginEdit()
    {
      _bindingEdit = true;
      CopyState();
    }

    /// <summary>
    /// Cancels the current edit process, restoring the object's state to
    /// its previous values.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be restored. This resets the object's values
    /// to the point of the last BeginEdit call.
    /// </remarks>
    public void CancelEdit()
    {
      UndoChanges();
    }

    /// <summary>
    /// Called when an undo operation has completed.
    /// </summary>
    /// <remarks>
    /// This method resets the object as a result of
    /// deserialization and raises PropertyChanged events
    /// to notify data binding that the object has changed.
    /// </remarks>
    protected override void UndoChangesComplete()
    {
      _bindingEdit = false;
      ValidationRules.SetTarget(this);
      AddBusinessRules();
      OnUnknownPropertyChanged();
      base.UndoChangesComplete();
    }

    /// <summary>
    /// Commits the current edit process.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be discarded, thus committing any changes made
    /// to the object's state since the last BeginEdit call.
    /// </remarks>
    public void ApplyEdit()
    {
      _bindingEdit = false;
      _neverCommitted = false;
      AcceptChanges();
    }

    #endregion

    #region IsChild

    [NotUndoable()]
    private bool _isChild;

    /// <summary>
    /// Returns <see langword="true" /> if this is a child (non-root) object.
    /// </summary>
    protected internal bool IsChild
    {
      get { return _isChild; }
    }

    /// <summary>
    /// Marks the object as being a child object.
    /// </summary>
    protected void MarkAsChild()
    {
      _isChild = true;
    }

    #endregion

    #region Delete

    /// <summary>
    /// Marks the object for deletion. The object will be deleted as part of the
    /// next save operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// CSLA .NET supports both immediate and deferred deletion of objects. This
    /// method is part of the support for deferred deletion, where an object
    /// can be marked for deletion, but isn't actually deleted until the object
    /// is saved to the database. This method is called by the UI developer to
    /// mark the object for deletion.
    /// </para><para>
    /// To 'undelete' an object, use n-level undo as discussed in Chapters 2 and 3.
    /// </para>
    /// </remarks>
    public void Delete()
    {
      if (this.IsChild)
        throw new NotSupportedException(Resources.ChildDeleteException);

      MarkDeleted();
    }

    /// <summary>
    /// Called by a parent object to mark the child
    /// for deferred deletion.
    /// </summary>
    internal void DeleteChild()
    {
      if (!this.IsChild)
        throw new NotSupportedException(Resources.NoDeleteRootException);

      MarkDeleted();
    }

    #endregion

    #region Edit Level Tracking (child only)

    // we need to keep track of the edit
    // level when we weere added so if the user
    // cancels below that level we can be destroyed
    private int _editLevelAdded;

    /// <summary>
    /// Gets or sets the current edit level of the
    /// object.
    /// </summary>
    /// <remarks>
    /// Allow the collection object to use the
    /// edit level as needed.
    /// </remarks>
    internal int EditLevelAdded
    {
      get { return _editLevelAdded; }
      set { _editLevelAdded = value; }
    }

    #endregion

    #region ICloneable

    object ICloneable.Clone()
    {
      return GetClone();
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>
    /// A new object containing the exact data of the original object.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual object GetClone()
    {
      return ObjectCloner.Clone(this);
    }

    #endregion

    #region ValidationRules, IsValid

    private Validation.ValidationRules _validationRules;

    /// <summary>
    /// Provides access to the broken rules functionality.
    /// </summary>
    /// <remarks>
    /// This property is used within your business logic so you can
    /// easily call the AddRule() method to associate validation
    /// rules with your object's properties.
    /// </remarks>
    protected Validation.ValidationRules ValidationRules
    {
      get
      {
        if (_validationRules == null)
          _validationRules = new Csla.Validation.ValidationRules(this);
        return _validationRules;
      }
    }

    /// <summary>
    /// Override this method in your business class to
    /// be notified when you need to set up business
    /// rules.
    /// </summary>
    /// <remarks>
    /// AddBusinessRules is automatically called by CSLA .NET
    /// when your object should associate validation rules
    /// with its properties.
    /// </remarks>
    protected virtual void AddBusinessRules()
    {

    }

    /// <summary>
    /// Returns <see langword="true" /> if the object is currently valid, <see langword="false" /> if the
    /// object has broken rules or is otherwise invalid.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this property relies on the underling ValidationRules
    /// object to track whether any business rules are currently broken for this object.
    /// </para><para>
    /// You can override this property to provide more sophisticated
    /// implementations of the behavior. For instance, you should always override
    /// this method if your object has child objects, since the validity of this object
    /// is affected by the validity of all child objects.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if the object is currently valid.</returns>
    [Browsable(false)]
    public virtual bool IsValid
    {
      get { return ValidationRules.IsValid; }
    }


    /// <summary>
    /// Provides access to the readonly collection of broken business rules
    /// for this object.
    /// </summary>
    /// <returns>A Csla.Validation.RulesCollection object.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual Validation.BrokenRulesCollection BrokenRulesCollection
    {
      get { return ValidationRules.GetBrokenRules(); }
    }

    #endregion

    #region Data Access

    /// <summary>
    /// Override this method to load a new business object with default
    /// values from the database.
    /// </summary>
    /// <remarks>
    /// Normally you will overload this method to accept a strongly-typed
    /// criteria parameter, rather than overriding the method with a
    /// loosely-typed criteria parameter.
    /// </remarks>
    /// <param name="Criteria">An object containing criteria values.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException(Resources.CreateNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <remarks>
    /// Normally you will overload this method to accept a strongly-typed
    /// criteria parameter, rather than overriding the method with a
    /// loosely-typed criteria parameter.
    /// </remarks>
    /// <param name="Criteria">An object containing criteria values to identify the object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException(Resources.FetchNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow insertion of a business
    /// object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Insert()
    {
      throw new NotSupportedException(Resources.InsertNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow update of a business
    /// object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Update()
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow deferred deletion of a business object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_DeleteSelf()
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow immediate deletion of a business object.
    /// </summary>
    /// <param name="Criteria">An object containing criteria values to identify the object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    #endregion

    #region IDataErrorInfo

    string IDataErrorInfo.Error
    {
      get
      {
        if (!IsValid)
          return ValidationRules.GetBrokenRules().ToString();
        else
          return String.Empty;
      }
    }

    string IDataErrorInfo.this[string columnName]
    {
      get
      {
        string result = string.Empty;
        if (!IsValid)
        {
          Validation.BrokenRule rule = 
            ValidationRules.GetBrokenRules().GetFirstBrokenRule(columnName);
          if (rule != null)
            result = rule.Description;
        }
        return result;
      }
    }

    #endregion

    #region Serialization Notification

    [OnDeserialized()]
    private void OnDeserializedHandler(StreamingContext context)
    {
      ValidationRules.SetTarget(this);
      AddBusinessRules();
      OnDeserialized(context);
    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDeserialized(StreamingContext context)
    {
      // do nothing - this is here so a subclass
      // could override if needed
    }

    #endregion
  }
}