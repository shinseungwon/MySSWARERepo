using System;

namespace SSWARE
{
    public partial class Default : Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                logger.WriteText(userType);
                if(userType == "ADMIN")
                    Response.Redirect("~/Form/Admin/Admin_Home.aspx");
                else
                    Response.Redirect("~/Form/Main/Home.aspx");
            }
        }
    }
}