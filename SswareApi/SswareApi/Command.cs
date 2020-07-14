using Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SswareApi
{
    public partial class Command
    {
        public readonly DbHelper DbHelper;
        public readonly LogHelper LogHelper;

        private User CurrentUser = null;

        public readonly CodeManagerClass CodeManager;
        public readonly LocalizeManagerClass LocalizeManager;
        public readonly AuthorityManagerClass AuthorityManager;

        public readonly UserManagerClass UserManager;
        public readonly FileManagerClass FileManager;
        public readonly ControllerManagerClass ControllerManager;
        public readonly BoardManagerClass BoardManager;        

        public Command(string ip, string catalog, string id, string password)
        {
            LogHelper = new LogHelper(@"C:\" + catalog + @"\");

            string connectionString = "Data Source=" + ip + ",1433; Initial Catalog=" + catalog
                + "; User id=" + id + "; Password=" + password + ";";

            DbHelper = new DbHelper(connectionString, LogHelper);

            CodeManager = new CodeManagerClass(this);
            LocalizeManager = new LocalizeManagerClass(this);
            AuthorityManager = new AuthorityManagerClass(this);

            UserManager = new UserManagerClass(this);
            FileManager = new FileManagerClass(this);
            ControllerManager = new ControllerManagerClass(this);
            BoardManager = new BoardManagerClass(this);
        }

        public void LogIn(string loginId, string password)
        {
            CurrentUser = UserManager.Login(loginId, password);
        }

        public abstract class ManagerClass
        {
            protected readonly Command Command;

            public ManagerClass(Command command)
            {
                Command = command;
            }

            protected Dictionary<string, object> ToDictionary(DataRow dataRow)
            {
                return dataRow.Table.Columns.Cast<DataColumn>()
                        .ToDictionary(c => c.ColumnName, c => dataRow[c]);
            }

            protected AuthoritySet Verification(UserType userType
                , XObjectType targetType = XObjectType.None, int targetId = 0)
            {
                if (Command.CurrentUser == null)
                {
                    throw new Exception("Need Login.");
                }
                else if ((int)userType > (int)Command.CurrentUser.UserType)
                {
                    throw new Exception("No Authority User Type.");
                }
                else if (Command.CurrentUser.UserType != UserType.ItAdministrator
                    && targetType != 0 && targetId != 0)
                {
                    XObject target = new XObject((int)targetType, targetId);
                    return Command.AuthorityManager
                        .GetAuthorityInfo(Command.CurrentUser, target);
                }
                else
                {
                    return new AuthoritySet
                    {
                        AuthorityType = AuthorityType.None,
                        AuthorityValue = AuthorityValue.NotSet
                    };
                }
            }
        }

        public abstract class XObjectManagerClass : ManagerClass
        {
            public XObjectManagerClass(Command command) : base(command)
            {

            }

            protected Dictionary<string, object> GetObjectInfo(int xObjectType, int id)
            {
                Dictionary<string, object> result = null;
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 1, SqlDbType.Int);
                Command.DbHelper.AddInput("@ObjectzType", xObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_ObjectManager", ds);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    result = ToDictionary(ds.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Not found");
                }

                return result;
            }

            protected List<Dictionary<string, object>> GetObjectInfoList(int xObjectType, string name = null)
            {
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 2, SqlDbType.Int);
                Command.DbHelper.AddInput("@objectzType", xObjectType, SqlDbType.Int);

                if (name != null)
                {
                    Command.DbHelper.AddInput("@name", name, SqlDbType.NVarChar);
                }

                Command.DbHelper.StoredProcedure("p_ObjectManager", ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Dictionary<string, object> info = ToDictionary(r);
                    result.Add(info);
                }

                return result;
            }

            protected void EnableDisableObject(int xObjectType, int id, bool enable)
            {
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 3, SqlDbType.Int);
                Command.DbHelper.AddInput("@objectzType", xObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.AddInput("@enable", enable, SqlDbType.Bit);
                Command.DbHelper.StoredProcedure("p_ObjectManager", ds);

                if ((int)ds.Tables[0].Rows[0][0] != 1)
                {
                    throw new Exception("Not found");
                }
            }

            protected void DeleteObject(int xObjectType, int id)
            {
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 4, SqlDbType.Int);
                Command.DbHelper.AddInput("@objectzType", xObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_ObjectManager", ds);

                if ((int)ds.Tables[0].Rows[0][0] != 1)
                {
                    throw new Exception("Not found");
                }
            }
        }

        public sealed class CodeManagerClass : ManagerClass
        {
            public CodeManagerClass(Command command) : base(command)
            {

            }

            public Code GetCode(int id)
            {
                Code code = null;
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 1, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_CodeManager", ds);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow r = ds.Tables[0].Rows[0];
                    Localize localize = null;

                    if ((int)r["Lid"] != 0)
                    {
                        localize = new Localize(
                            (int)r["Lid"],
                            (int)XObjectType.None,
                            (int)r["LKeyz"],
                            (string)r["LItem"],
                            (string)r["LMemo"],
                            (string)r["LEnglish"],
                            (string)r["LKorean"]);
                    }

                    code = new Code(
                        (int)r["id"],
                        (string)r["Name"],
                        (int)r["Parent"],
                        (int)r["SettingValue"],
                        (int)r["Value"],
                        (int)r["Sort"],
                        (string)r["Memo"],
                        (bool)r["IsSystem"],
                        localize);
                }

                return code;
            }

            public List<Code> GetCodeList(int parent)
            {
                List<Code> codes = new List<Code>();
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 2, SqlDbType.Int);

                if (parent != 0)
                {
                    Command.DbHelper.AddInput("@id", parent, SqlDbType.Int);
                }

                Command.DbHelper.StoredProcedure("p_CodeManager", ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Localize localize = null;

                    if ((int)r["Lid"] != 0)
                    {
                        localize = new Localize(
                            (int)r["Lid"],
                            (int)XObjectType.None,
                            (int)r["LKeyz"],
                            (string)r["LItem"],
                            (string)r["LMemo"],
                            (string)r["LEnglish"],
                            (string)r["LKorean"]);
                    }

                    codes.Add(new Code(
                        (int)r["id"],
                        (string)r["Namez"],
                        (int)r["Parent"],
                        (int)r["SettingValuez"],
                        (int)r["Valuez"],
                        (int)r["Sort"],
                        (string)r["Memo"],
                        (bool)r["IsSystem"],
                        localize));
                }

                return codes;
            }

            public void UpdateCode(Code code)
            {
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 3, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", code.Id, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", code.Name, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@parent", code.Parent, SqlDbType.Int);
                Command.DbHelper.AddInput("@valuez", code.Value, SqlDbType.Int);
                Command.DbHelper.AddInput("@settingValuez", code.SettingValue, SqlDbType.Int);
                Command.DbHelper.AddInput("@sort", code.Sort, SqlDbType.Int);
                Command.DbHelper.AddInput("@memo", code.Memo, SqlDbType.NVarChar);

                if (code.Localize != null)
                {
                    if (code.Localize.Id > 0)
                    {
                        Command.DbHelper.AddInput("@localize", code.Localize.Id, SqlDbType.Int);
                    }
                }

                Command.DbHelper.StoredProcedure("p_CodeManager", ds);
            }

            public void DeleteCode(int id)
            {
                Command.DbHelper.AddInput("@operation", 4, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_CodeManager");
            }

            public Code AddCode(string name, int parent = 0, int settingValue = 0
                , int value = 0, int sort = 0, string memo = "", Localize localize = null)
            {
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 5, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", name, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@parent", parent, SqlDbType.Int);
                Command.DbHelper.AddInput("@valuez", value, SqlDbType.Int);
                Command.DbHelper.AddInput("@settingValuez", settingValue, SqlDbType.Int);
                Command.DbHelper.AddInput("@sort", sort, SqlDbType.Int);
                Command.DbHelper.AddInput("@memo", memo, SqlDbType.NVarChar);

                if (localize != null)
                {
                    if (localize.Id > 0)
                    {
                        Command.DbHelper.AddInput("@localize", localize.Id, SqlDbType.Int);
                    }
                }

                Command.DbHelper.StoredProcedure("p_CodeManager", ds);

                int identity = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

                return new Code(identity, name, parent, settingValue
                    , value, sort, memo, false, localize);
            }
        }

        public sealed class LocalizeManagerClass : ManagerClass
        {
            public LocalizeManagerClass(Command command) : base(command)
            {

            }

            public Localize GetLocalize(int id)
            {
                Localize localize = null;
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 1, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_LocalizeManager", ds);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow r = ds.Tables[0].Rows[0];

                    localize = new Localize(
                        (int)r["id"],
                        (int)r["XObjectType"],
                        (int)r["Keyz"],
                        (string)r["LItem"],
                        (string)r["Memo"],
                        (string)r["Korean"],
                        (string)r["English"]);
                }

                return localize;
            }

            public Localize GetLocalize(int xObjectType, int key)
            {
                Localize localize = null;
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 2, SqlDbType.Int);
                Command.DbHelper.AddInput("@objectzType", xObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@key", key, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_LocalizeManager", ds);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow r = ds.Tables[0].Rows[0];

                    localize = new Localize(
                        (int)r["Lid"],
                        (int)r["LObjectzType"],
                        (int)r["LKeyz"],
                        (string)r["LItem"],
                        (string)r["LMemo"],
                        (string)r["LKorean"],
                        (string)r["LEnglish"]);
                }

                return localize;
            }

            public void UpdateLocalize(Localize localize)
            {
                Command.DbHelper.AddInput("@operation", 4, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", localize.Id, SqlDbType.Int);
                Command.DbHelper.AddInput("@memo", localize.Memo, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@english", localize.English, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@korean", localize.Korean, SqlDbType.NVarChar);
                Command.DbHelper.StoredProcedure("p_LocalizeManager");
            }

            public void DeleteLocalize(int id)
            {
                Command.DbHelper.AddInput("@operation", 5, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_LocalizeManager");
            }

            public Localize AddLocalize(int xObjectType = 0, int key = 0, string item = ""
                , string memo = "", string english = "", string korean = "")
            {
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 6, SqlDbType.Int);
                Command.DbHelper.AddInput("@objectzType", xObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@key", key, SqlDbType.Int);
                Command.DbHelper.AddInput("@item", item, SqlDbType.VarChar);
                Command.DbHelper.AddInput("@memo", memo, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@english", english, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@korean", korean, SqlDbType.NVarChar);
                Command.DbHelper.StoredProcedure("p_LocalizeManager", ds);

                int identity = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

                return new Localize(identity, xObjectType, key, item, memo, english, korean);
            }
        }

        public sealed class AuthorityManagerClass : ManagerClass
        {
            public AuthorityManagerClass(Command command) : base(command)
            {

            }

            public void AddAuthority(XObject subject, XObject target
                , AuthorityType authorityType, AuthorityValue authorityValue)
            {
                Command.DbHelper.AddInput("@operation", 1, SqlDbType.Int);
                Command.DbHelper.AddInput("@subjectzType", subject?.XObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@subjectz", subject?.Id, SqlDbType.Int);
                Command.DbHelper.AddInput("@targetzType", target?.XObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@targetz", target?.Id, SqlDbType.Int);
                Command.DbHelper.AddInput("@authorityType", (int)authorityType, SqlDbType.Int);
                Command.DbHelper.AddInput("@authorityValue", (int)authorityValue, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_AuthorityManager");
            }

            public void RemoveAuthority(XObject subject, XObject target)
            {
                Command.DbHelper.AddInput("@operation", 2, SqlDbType.Int);
                Command.DbHelper.AddInput("@subjectzType", subject.XObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@subjectz", subject.Id, SqlDbType.Int);
                Command.DbHelper.AddInput("@targetzType", target.XObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@targetz", target.Id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_AuthorityManager");
            }

            public AuthoritySet GetAuthorityInfo(XObject subject, XObject target)
            {
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 3, SqlDbType.Int);
                Command.DbHelper.AddInput("@subjectzType", subject.XObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@subjectz", subject.Id, SqlDbType.Int);
                Command.DbHelper.AddInput("@targetzType", target.XObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@targetz", target.Id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_AuthorityManager", ds);

                AuthoritySet authoritySet;

                if (ds.Tables[0].Rows.Count == 0)
                {
                    authoritySet = new AuthoritySet
                    {
                        AuthorityType = AuthorityType.None,
                        AuthorityValue = AuthorityValue.NotSet
                    };
                }
                else
                {
                    authoritySet = new AuthoritySet
                    {
                        AuthorityType = (AuthorityType)ds.Tables[0].Rows[0][0],
                        AuthorityValue = (AuthorityValue)ds.Tables[0].Rows[0][1]
                    };
                }

                return authoritySet;
            }

            public List<XObject> GetAuthorizedObjectList(XObject subject, int targetType
                , AuthorityType authorityType, AuthorityValue authorityValue)
            {
                List<XObject> targets = new List<XObject>();
                DataSet ds = new DataSet();

                Command.DbHelper.AddInput("@operation", 4, SqlDbType.Int);
                Command.DbHelper.AddInput("@subjectzType", subject.XObjectType, SqlDbType.Int);
                Command.DbHelper.AddInput("@subjectz", subject.Id, SqlDbType.Int);
                Command.DbHelper.AddInput("@targetzType", targetType, SqlDbType.Int);
                Command.DbHelper.AddInput("@authorityType", (int)authorityType, SqlDbType.Int);
                Command.DbHelper.AddInput("@authorityValue", (int)authorityValue, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_AuthorityManager", ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Dictionary<string, object> dictionary
                        = ToDictionary(ds.Tables[0].Rows[0]);

                    XObject obj = new XObject(targetType, dictionary);
                    targets.Add(obj);
                }

                return targets;
            }
        }
    }
}