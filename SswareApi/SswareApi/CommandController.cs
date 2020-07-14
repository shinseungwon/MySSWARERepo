using Helper;
using System.Data;

namespace SswareApi
{
    public partial class Command
    {
        public sealed class ControllerManagerClass : XObjectManagerClass
        {
            public ControllerManagerClass(Command command) : base(command)
            {

            }

            #region Manage Controller

            public XController RegisterController(string name, string title, int parent = 0)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 1, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", name, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@title", title, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@parent", parent, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_ControllerManager", ds);

                return new XController(GetObjectInfo((int)XObjectType.Controller
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public void UpdateController(XController xController)
            {
                Verification(UserType.ItAdministrator);

                Command.DbHelper.AddInput("@operation", 2, SqlDbType.Int);
                Command.DbHelper.AddInput("@title", xController.Title, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@parent", xController.Parent, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_ControllerManager");
            }

            public void EnableDisableController(int id, bool enable)
            {
                Verification(UserType.ItAdministrator);

                EnableDisableObject(3, id, enable);
            }

            public void UnregisterController(int id)
            {
                Verification(UserType.ItAdministrator);

                DeleteObject(3, id);
            }

            #endregion

            #region Manage Action

            public XAction RegisterAction(string name, string header, int controller, int parent = 0)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 101, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", name, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@header", header, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@controller", controller, SqlDbType.Int);
                Command.DbHelper.AddInput("@parent", parent, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_ControllerManager", ds);

                return new XAction(GetObjectInfo((int)XObjectType.Action
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public void UpdateAction(XAction xAction)
            {
                Verification(UserType.ItAdministrator);

                Command.DbHelper.AddInput("@operation", 102, SqlDbType.Int);
                Command.DbHelper.AddInput("@header", xAction.Header, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@parent", xAction.Parent, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_ControllerManager");
            }

            public void EnableDisableAction(int id, bool enable)
            {
                Verification(UserType.ItAdministrator);

                EnableDisableObject(4, id, enable);
            }

            public void UnregisterAction(int id)
            {
                Verification(UserType.ItAdministrator);

                DeleteObject(4, id);
            }

            #endregion

            #region Manage Element

            public Element RegisterElement(string name, string domId, int action, int parent = 0)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 201, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", name, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@header", domId, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@parent", action, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_ControllerManager", ds);

                return new Element(GetObjectInfo((int)XObjectType.Element
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public void UpdateElement(Element element)
            {
                Verification(UserType.ItAdministrator);

                Command.DbHelper.AddInput("@operation", 202, SqlDbType.Int);
                Command.DbHelper.AddInput("@domId", element.DomId, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@parent", element.Parent, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_ControllerManager");
            }

            public void EnableDisableElement(int id, bool enable)
            {
                Verification(UserType.ItAdministrator);

                EnableDisableObject(5, id, enable);
            }

            public void UnregisterElement(int id)
            {
                Verification(UserType.ItAdministrator);

                DeleteObject(5, id);
            }

            #endregion
        }
    }
}
