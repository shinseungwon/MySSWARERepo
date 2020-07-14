using Helper;
using System.Collections.Generic;
using System.Data;

namespace SswareApi
{
    public partial class Command
    {
        public sealed class UserManagerClass : XObjectManagerClass
        {
            public UserManagerClass(Command command) : base(command)
            {

            }

            #region Manage Users

            public User AddUser(string name, int type, int title, string loginId , string password
                , Language language, bool isUse = true)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 1, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", name, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@typez", type, SqlDbType.Int);
                Command.DbHelper.AddInput("@title", title, SqlDbType.Int);
                Command.DbHelper.AddInput("@loginId", loginId, SqlDbType.VarChar);
                Command.DbHelper.AddInput("@passwordz", password, SqlDbType.VarBinary, true);
                Command.DbHelper.AddInput("@languagez", (int)language, SqlDbType.Int);
                Command.DbHelper.AddInput("@isUse", isUse, SqlDbType.Bit);
                Command.DbHelper.StoredProcedure("p_UserManager", ds);

                return new User(GetObjectInfo((int)XObjectType.User
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public void UpdateUser(User user)
            {
                Verification(UserType.Employee, XObjectType.User, user.Id);

                Command.DbHelper.AddInput("@operation", 2, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", user.Name, SqlDbType.Int);
                Command.DbHelper.AddInput("@typez", user.UserType, SqlDbType.Int);
                Command.DbHelper.AddInput("@title", user.UserTitle, SqlDbType.Int);
                Command.DbHelper.AddInput("@languagez", (int)user.Language, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_UserManager");
            }

            public void ChangePassword(int id, string oldPassword, string newPassword)
            {
                Verification(UserType.Employee, XObjectType.User, id);

                Command.DbHelper.AddInput("@operation", 3, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.AddInput("@passwordz", oldPassword, SqlDbType.VarBinary, true);
                Command.DbHelper.AddInput("@newPasswordz", newPassword, SqlDbType.VarBinary, true);
                Command.DbHelper.StoredProcedure("p_UserManager");
            }

            public User Login(string loginId, string password)
            {
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 4, SqlDbType.Int);
                Command.DbHelper.AddInput("@loginId", loginId, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@passwordz", password, SqlDbType.VarBinary, true);
                Command.DbHelper.StoredProcedure("p_UserManager", ds);

                return new User(GetObjectInfo((int)XObjectType.User
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public void EnableDisableUser(int id, bool enable)
            {
                Verification(UserType.Employee, XObjectType.User, id);

                EnableDisableObject(1, id, enable);
            }

            public void DeleteUser(int id)
            {
                Verification(UserType.ItAdministrator);

                DeleteObject(1, id);
            }

            public User GetUserById(int id)
            {
                Verification(UserType.Employee, XObjectType.User, id);

                return new User(GetObjectInfo((int)XObjectType.User, id));
            }

            public List<User> GetUserList(string name = null)
            {
                Verification(UserType.ItAdministrator);

                List<Dictionary<string, object>> objectInfoList
                    = GetObjectInfoList(1, name);

                List<User> result = new List<User>();

                foreach (Dictionary<string, object> d in objectInfoList)
                {
                    result.Add(new User(d));
                }

                return result;
            }

            #endregion

            #region Manage Groups

            public Group AddGroup(string name, int positionType = 1, bool isUse = true)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 101, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", name, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@positionType", positionType, SqlDbType.Int);
                Command.DbHelper.AddInput("@isUse", isUse, SqlDbType.Bit);
                Command.DbHelper.StoredProcedure("p_UserManager", ds);

                return new Group(GetObjectInfo((int)XObjectType.Group
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public void UpdateGroup(Group group)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 102, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", group.Id, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", group.Name, SqlDbType.NVarChar);
                Command.DbHelper.StoredProcedure("p_UserManager", ds);
            }

            public void EnableDisableGroup(int id, bool enable)
            {
                Verification(UserType.ItAdministrator);

                EnableDisableObject(2, id, enable);
            }

            public void DeleteGroup(int id)
            {
                Verification(UserType.ItAdministrator);

                DeleteObject(2, id);
            }

            public void BindGroup(int parent, int child)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 103, SqlDbType.Int);
                Command.DbHelper.AddInput("@parent", parent, SqlDbType.Int);
                Command.DbHelper.AddInput("@child", child, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_UserManager", ds);
            }

            public void BindUser(int group, int member, int position = 0)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 104, SqlDbType.Int);
                Command.DbHelper.AddInput("@groupz", group, SqlDbType.Int);
                Command.DbHelper.AddInput("@member", member, SqlDbType.Int);
                Command.DbHelper.AddInput("@position", position, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_UserManager", ds);
            }

            public List<Group> GetChildGroups(int id)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 105, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_UserManager", ds);

                List<Group> result = new List<Group>();

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    result.Add(new Group(ToDictionary(r)));
                }

                return result;
            }

            public List<User> GetMemberUsers(int id)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 106, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_UserManager", ds);

                List<User> result = new List<User>();

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    result.Add(new User(ToDictionary(r)));
                }

                return result;
            }

            #endregion
        }
    }
}
