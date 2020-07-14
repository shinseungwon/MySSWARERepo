using CORE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SSWARE.Master
{
    public partial class TopBar : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string calledPageUrl = HttpContext.Current.Request.ServerVariables["URL"];
                DbHelper dbHelper = new DbHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SSWARE"].ConnectionString);
                Logger logger = new Logger(HttpRuntime.AppDomainAppPath + "/Logs/");
                dbHelper.SetLogger(logger);

                HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value);                
                string query = "select * from TopMenu ";

                if (cookie != null)
                {
                    query += "select '/Form/' + TopMenu.Url + '/' + LeftMenu.Url + '.aspx' as Url, LeftMenu.Name as Name from RightMenu " +
                        "left join Users on Users.id = RightMenu.Users " +
                        "left join LeftMenu on LeftMenu.id = RightMenu.LeftMenu " +
                        "left join TopMenu on TopMenu.id = LeftMenu.TopMenu " +
                        "where Users.LoginId = '" + ticket.Name + "'";
                }

                StringBuilder sb; 
                DataSet ds = new DataSet();
                
                dbHelper.CallQuery(query, ref ds);

                if(ds.Tables.Count > 0)
                {
                    sb = new StringBuilder();
                    sb.Append("<ul id=\"TopBarList\">");
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        sb.Append("<li onClick=\"location.href='/Form/" + row["Url"] + "/home.aspx'\">" + row["Name"] + "</li>");
                    }
                    sb.Append("</ul>");
                    topbar.InnerHtml = sb.ToString();
                }
                
                if(ds.Tables.Count > 1)
                {
                    sb = new StringBuilder();
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        sb.Append("<a href=\"" + row["Url"] + "\">" + row["Name"] + "</a>");                        
                    }
                    rightbar.InnerHtml = rightbar.InnerHtml + sb.ToString();
                }                
            }
        }

        protected void LogOut_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }
    }
}