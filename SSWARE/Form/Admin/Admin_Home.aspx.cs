using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SSWARE.Form.Main
{
    public partial class HomeAdmin : Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {                
                //SetPagetoDb(Contents, "관리자 홈");                
            }
        }
    }
}