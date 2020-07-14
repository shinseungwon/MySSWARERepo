using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices;
using CORE;

namespace SSWARE.Form.Main
{
    public partial class Logon : System.Web.UI.Page
    {
        protected DbHelper dbHelper;
        protected Logger logger;

        protected void Page_Load(object sender, EventArgs e)
        {
            dbHelper = new DbHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SSWARE"].ConnectionString);
            logger = new Logger(HttpRuntime.AppDomainAppPath + "/Logs/");
            
        }

        protected void Logon_Click(object sender, EventArgs e)
        {
            if(System.Configuration.ConfigurationManager.AppSettings["IsUseActiveDirectory"] == "Y")
            {
                DirectoryEntry entry = new DirectoryEntry(System.Configuration.ConfigurationManager
                    .AppSettings["ActiveDirectoryAddress"], UserEmail.Text, UserPass.Text);

                DirectorySearcher search;
                SearchResult searchresult;
                try
                {
                     search = new DirectorySearcher(entry);
                     searchresult = search.FindOne();
                }catch (Exception ex)
                {
                    Msg.Text = "AD인증 실패";
                    logger.WriteDb(dbHelper, "Logs", "Logon", "AD인증 실패 : ID = " +UserEmail.Text+ ", 상세 : " + ex.ToString());                    
                    return;
                }

                if(searchresult == null)
                {
                    Msg.Text = "AD인증 실패";
                    logger.WriteDb(dbHelper, "Logs", "Logon", "AD인증 실패 : ID = " + UserEmail.Text + ", 상세 : AD 계정 없음");                    
                    return;
                }
                else
                {
                    int LoginId = int.Parse(dbHelper.CallFS("dbo.fs_GetLoginIdFromAd(" + UserEmail.Text + ")"));
                    if (LoginId == 0)
                    {
                        Msg.Text = "AD 정보없음";
                        logger.WriteDb(dbHelper, "Logs", "Logon", "조직도 인증 실패 : ID = " + UserEmail.Text + ", 상세 : Users에 해당 AD정보 없음");                        
                    }
                    else
                    {
                        FormsAuthentication.RedirectFromLoginPage(UserEmail.Text, Persist.Checked);
                        logger.WriteDb(dbHelper, "Logs", "Logon", "AD 로그인 성공 : ID = " + UserEmail.Text);
                    }
                }
            }
            else
            {
                string[] result = new string[2];

                dbHelper.AddInput("@Id", UserEmail.Text, System.Data.SqlDbType.NVarChar);
                dbHelper.AddInput("@Password", UserPass.Text, System.Data.SqlDbType.VarBinary, true);
                dbHelper.AddOutput("@Result", System.Data.SqlDbType.Int);
                dbHelper.AddOutput("@Message", System.Data.SqlDbType.NVarChar, 100);
                dbHelper.CallSP("p_Authorize", ref result);

                if (result[0] == "0")
                {
                    logger.WriteDb(dbHelper, "Logs", "Logon", "조직도 로그인 성공 : ID = " + UserEmail.Text);
                    FormsAuthentication.RedirectFromLoginPage(UserEmail.Text, Persist.Checked);
                }
                else
                {
                    Msg.Text = result[1];
                }
            }
        }

        private string GetIP()
        {
            String ip =
                HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }
    }
}