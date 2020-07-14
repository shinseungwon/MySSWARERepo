using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SSWARE.Form.Common
{
    public partial class Ajax : Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["Target"] == null)
                {
                    throw new Exception("No Ajax Target");
                }
                else
                {
                    switch (Request["Target"])
                    {
                        case "":
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}