using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Templates
{
  [Serializable()]
  class CommandObject : CommandBase
  {
    #region Authorization Methods

    public static bool CanExecuteCommand()
    {
      // TODO: customize to check user role
      //return ApplicationContext.User.IsInRole("");
      return true;
    }

    #endregion

    #region Client-side Code

    // TODO: add your own fields and properties
    bool _result;

    public bool Result
    {
      get { return _result; }
    }

    private void BeforeServer()
    {
      // TODO: implement code to run on client
      // before server is called
    }

    private void AfterServer()
    {
      // TODO: implement code to run on client
      // after server is called
    }

    #endregion

    #region Factory Methods

    public static bool Execute()
    {
      CommandObject cmd = new CommandObject();
      cmd.BeforeServer();
      cmd = DataPortal.Execute<CommandObject>(cmd);
      cmd.AfterServer();
      return cmd.Result;
    }

    private CommandObject()
    { /* require use of factory methods */ }

    #endregion

    #region Server-side Code

    protected override void DataPortal_Execute()
    {
      // TODO: implement code to run on server
      // and set result value(s)
      _result = true;
    }

    #endregion
  }
}
