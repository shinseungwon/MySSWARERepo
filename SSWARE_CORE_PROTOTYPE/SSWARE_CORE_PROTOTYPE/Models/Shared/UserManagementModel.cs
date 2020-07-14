using Microsoft.Extensions.Configuration;

namespace SSWARE_CORE_PROTOTYPE.Models.Shared
{
    public class UserManagementModel : ModelBase
    {
        public UserManagementModel(IConfiguration cofiguration) : base(cofiguration)
        {
            
        }

        public int SignIn(string id, string password, ref string message)
        {           
            string[] result = new string[2];

            dbHelper.AddInput("@Id", id, System.Data.SqlDbType.NVarChar);
            dbHelper.AddInput("@Password", password, System.Data.SqlDbType.VarBinary, true);
            dbHelper.AddOutput("@Result", System.Data.SqlDbType.Int);
            dbHelper.AddOutput("@Message", System.Data.SqlDbType.NVarChar, 100);
            dbHelper.CallSP("p_Authorize", ref result);
            message = result[1];
            logger.WriteDb(dbHelper, "Logs", "SignIn", message + ", id : " + id);

            return int.Parse(result[0]);
        }
    }
}