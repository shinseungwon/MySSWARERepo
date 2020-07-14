using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using CORE;

namespace SSWARE
{
    public class Base : Page
    {
        protected DbHelper dbHelper;
        protected Logger logger;
        protected MailHelper mailHelper;
        protected FileHelper fileHelper;

        protected Dictionary<string, string> localize;

        protected XmlDocument doc;

        protected string calledPageUrl;
        protected string partPageUrl;
        protected string commonPageUrl;

        protected string smtpDomain;
        protected int smtpPort;
        protected string smtpId;
        protected string smtpPassword;

        protected string ftpAddress;
        protected string ftpUserId;
        protected string ftpPassword;
        protected string ftpIv;

        protected int userKey;
        protected string userLoginId;
        protected string name;
        protected string userType;
        protected string userDept;
        protected string userLevel;
        protected string userLang;

        public Base()
        {            
            calledPageUrl = HttpContext.Current.Request.ServerVariables["URL"];
            string[] splits = calledPageUrl.Split('/');
            
            partPageUrl = "";
            for(int i = 1;i < splits.Length; i++)
            {
                if (i == splits.Length - 1)
                {
                    partPageUrl += "/Ajax.aspx";                    
                }
                else
                {
                    partPageUrl += "/" + splits[i];
                }
            }

            commonPageUrl = "/Form/Common/Ajax.aspx";

            dbHelper = new DbHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SSWARE"].ConnectionString);
            logger = new Logger(HttpRuntime.AppDomainAppPath + "/Logs/");
            dbHelper.SetLogger(logger);            

            smtpDomain = System.Configuration.ConfigurationManager.AppSettings["SmtpDomain"];
            smtpPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["smtpPort"]);
            smtpId = System.Configuration.ConfigurationManager.AppSettings["smtpId"];
            smtpPassword = System.Configuration.ConfigurationManager.AppSettings["smtpPassword"];
            mailHelper = new MailHelper(smtpDomain, smtpPort, smtpId, smtpPassword);

            ftpAddress = System.Configuration.ConfigurationManager.AppSettings["FtpAddress"];
            ftpUserId = System.Configuration.ConfigurationManager.AppSettings["FtpId"];
            ftpPassword = System.Configuration.ConfigurationManager.AppSettings["FtpPassword"];
            ftpIv = System.Configuration.ConfigurationManager.AppSettings["FtpEncryptIV"];
            fileHelper = new FileHelper(ftpAddress, ftpUserId, ftpPassword, ftpIv, dbHelper, logger);

            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket;

            if (cookie != null)
            {
                ticket = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value);                
                dbHelper.AddInput("LoginId", ticket.Name, SqlDbType.NVarChar);
                DataSet ds = new DataSet();
                dbHelper.CallSP("P_GetUserInfo", ref ds);
                DataRow row = ds.Tables[0].Rows[0];
                                
                userKey = int.Parse(row["Key"].ToString());
                userLoginId = row["LoginId"].ToString();
                name = row["Name"].ToString();
                userType = row["Type"].ToString();
                userDept = row["Dept"].ToString();
                userLevel = row["Level"].ToString();
                userLang = row["Language"].ToString();
            }
            
            //스크립트 설정
            string script = "var calledPageUrl = \"" + calledPageUrl + "\"; "
                + "var partPageUrl = \"" + partPageUrl + "\"; "
                + "var commonPageUrl = \"" + commonPageUrl + "\"; ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetUrl", script, true);

            localize = GetDocumentLocalize();
        }

        protected string SetControls(ref HtmlGenericControl control)
        {            
            string[] urlSplit = calledPageUrl.Split('/');
            string topMenu = urlSplit[urlSplit.Length - 2];
            string page = urlSplit[urlSplit.Length - 1].Split('.')[0];

            dbHelper.AddInput("@LoginId", FormsAuthentication.Decrypt
                (HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name, SqlDbType.NVarChar);
            dbHelper.AddInput("@TopMenu", topMenu, SqlDbType.VarChar);
            dbHelper.AddInput("@Page", page, SqlDbType.VarChar);
            dbHelper.AddOutput("@PageTitle", SqlDbType.VarChar, 200);
            dbHelper.AddOutput("@PageAuth", SqlDbType.VarChar, 10);

            DataSet ds = new DataSet();
            string[] sa = new string[2];
            dbHelper.CallSP("P_GetPageControls", ref ds, ref sa);
            control.Visible = sa[1] == "true";

            Dictionary<string, DataRow> dic = new Dictionary<string, DataRow>();
            
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                dic.Add(row["ObjectId"].ToString(), row);
            }

            System.Reflection.PropertyInfo pi;

            foreach (Control c in control.Controls)
            {
                if (c.ID != null)
                {                    
                    c.Visible = dic[c.ID]["AccessFlag"].ToString() == "true";
                    pi = c.GetType().GetProperty("Text");                    
                    pi.SetValue(c, dic[c.ID]["Tag"].ToString());                    
                }
            }
            
            return sa[0];
        }

        protected void SetPagetoDb(HtmlGenericControl control, string pageTitle)
        {
            string[] urlSplit = calledPageUrl.Split('/');
            string topMenu = urlSplit[urlSplit.Length - 2];
            string page = urlSplit[urlSplit.Length - 1].Split('.')[0];

            StringBuilder b = new StringBuilder();
            string[] type;

            b.Append("<body>");
            foreach (Control c in control.Controls)
            {
                if (c.ID == null) continue;

                type = c.GetType().ToString().Split('.');
                b.Append("<row id = \"" + c.ID + "\" />");
            }
            b.Append("</body>");
                        
            dbHelper.AddInput("@TopMenu", topMenu, SqlDbType.VarChar);            
            dbHelper.AddInput("@Page", page, SqlDbType.VarChar);
            dbHelper.AddInput("@PageTitle", pageTitle, SqlDbType.VarChar);
            dbHelper.AddInput("@Xml", b.ToString() , SqlDbType.Xml);
            dbHelper.CallSP("p_RegisterPage");
        }

        protected void DownLoadFile(string title, byte[] file)
        {
            Response.AddHeader("Content-Disposition", "attachement;filename="
                + HttpUtility.UrlEncode(title));
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.OutputStream.Write(file, 0, file.Length);
            Response.BinaryWrite(file);
        }

        protected Dictionary<string, string> GetDocumentLocalize()
        {
            DataTable datatable = dbHelper.CallFT("ft_GetLocalizeDocumentTable('"
                + calledPageUrl + "', " + userKey + ")");

            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach(DataRow row in datatable.Rows)
            {
                dic.Add(row["TagName"].ToString(), row["Tag"].ToString());
            }

            return dic;
        }

        protected string GetLocalize(string tagName)
        {
            string result;

            try
            {
                result = localize[tagName];
            }
            catch (Exception e)
            {
                logger.WriteText(e.ToString());
                result = tagName;
            }

            return result;
        }
        
        protected void SetXmlLine(XmlNode node, string line)
        {
            XmlDocumentFragment frag = doc.CreateDocumentFragment();
            frag.InnerXml = line;
            node.AppendChild(frag);            
        }
    }
}