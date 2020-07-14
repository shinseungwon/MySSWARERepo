using CORE;
using System;
using System.Data;
using System.Text;
using System.Web;

namespace SSWARE.Form.Admin
{
    public partial class Code : Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //SetPagetoDb(Contents, "코드 관리");
                //Title = SetControls(ref Contents);

                if (Request["type"] == "data")
                {
                    DataSet ds = new DataSet();
                    dbHelper.CallQuery("select id, Name, Memo from code where parent = 0", ref ds);
                    Response.ContentType = "application/json";
                    Response.Write(GridHelper.GetJson(ds.Tables[0]));
                    Response.End();
                }
                else if (Request["type"] == "upload")
                {
                    logger.WriteText(Server.UrlDecode(Request["changes"]));
                    dbHelper.AddInput("@JSON", Server.UrlDecode(Request["changes"]), SqlDbType.NVarChar);
                    dbHelper.CallSP("p_EditCode");                    
                    Response.End();
                }
                else
                {
                    if (Request["type"] == "tdata")
                    {
                        DataSet ds = new DataSet();
                        dbHelper.CallQuery("select * from dbo.ft_Organization()", ref ds);
                        Response.ContentType = "application/json";
                        Response.Write(GridHelper.GetTreeJson(ds.Tables[0]));
                        //logger.WriteText(GridHelper.GetTreeJson(ds.Tables[0]));
                        Response.End();                        
                    }
                    else if (Request["type"] == "tupload")
                    {
                        logger.WriteText(Server.UrlDecode(Request["changes"]));
                        //dbHelper.AddInput("@JSON", Server.UrlDecode(Request["changes"]), SqlDbType.NVarChar);
                        //dbHelper.CallSP("p_EditCode");
                        Response.End();
                    }
                }
            }
        }
    }
}