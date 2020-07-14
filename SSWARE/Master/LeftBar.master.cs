using CORE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SSWARE.Master
{
    public partial class LeftBar : System.Web.UI.MasterPage
    {
        string calledPageUrl;
        DbHelper dbHelper;
        Logger logger;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                calledPageUrl = HttpContext.Current.Request.ServerVariables["URL"];
                dbHelper = new DbHelper(System.Configuration.ConfigurationManager.ConnectionStrings["SSWARE"].ConnectionString);
                logger = new Logger(HttpRuntime.AppDomainAppPath + "/Logs/");
                dbHelper.SetLogger(logger);                

                string TopBar = Request["TopBar"] ?? "1";

                StringBuilder sb = new StringBuilder();
                DataSet ds = new DataSet();
                dbHelper.CallQuery("select * from LeftMenu where TopMenu = " + TopBar + " order by sort, Parent", ref ds);                

                GetLi(ds.Tables[0], ref sb, 0);
                
                sb.Append("</ul>");
                leftnav.InnerHtml = sb.ToString();                
            }
        }

        private void GetLi(DataTable t, ref StringBuilder b, int depth, int parent = 0)
        {
            int i;

            for (i = 0; i < t.Rows.Count; i++)
            {
                if (int.Parse(t.Rows[i]["parent"].ToString()) != parent) continue;
                else if (GetChildCount(t, t.Rows[i]["id"].ToString()) == 0)
                {
                    b.Append("<li class=\"Depth" + depth + "\" onClick=\"location.href='" + t.Rows[i]["Url"] + ".aspx'\">" + t.Rows[i]["Name"] + "</li>");                    
                }
                else
                {                    
                    b.Append("<li class=\"ChildDepth" + depth + "\" ><span class=\"Expand\">" + t.Rows[i]["Name"] + "</span><ul class=\"Nested\">");
                    GetLi(t, ref b, depth + 1, int.Parse(t.Rows[i]["id"].ToString()));
                    b.Append("</ul></li>");
                }
            }
        }

        private int GetChildCount(DataTable t, string id)
        {
            int count = 0;

            foreach(DataRow row in t.Rows)
            {
                if (row["Parent"].ToString() == id) count++;
            }

            return count;
        }
    }
}