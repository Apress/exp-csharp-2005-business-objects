using System;

public class ProjectResourceData
{
  private int _resourceId;
  private string _firstName;
  private string _lastName;
  private string _assigned;
  private int _role;

  public int ResourceId
  {
    get { return _resourceId; }
    set { _resourceId = value; }
  }

  public string FirstName
  {
    get { return _firstName; }
    set { _firstName = value; }
  }

  public string LastName
  {
    get { return _lastName; }
    set { _lastName = value; }
  }

  public string Assigned
  {
    get { return _assigned; }
    set { _assigned = value; }
  }

  public int Role
  {
    get { return _role; }
    set { _role = value; }
  }	
}
