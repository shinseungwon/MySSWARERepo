using CORE;
using System;
using System.Data;

namespace SSWARE.Form.Common
{
    public partial class AjaxAuto : Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if(Request["Target"] == null)
                {                    
                    throw new Exception("No AjaxAuto Target");
                }
                else
                {
                    if (Request["Target"].StartsWith("p_"))
                    {
                        int retVal;
                        int outputCount = 0;
                        for (int i = 0; i < Request.Form.Count; i++)
                        {
                            if (Request.Form.GetKey(i) == "Target") continue;
                            else
                            {
                                if (Request.Form.GetKey(i).StartsWith("o_")) 
                                {
                                    dbHelper.AddOutput(Request.Form.GetKey(i).Replace("o_", ""), SqlDbType.NVarChar);
                                    outputCount++;
                                }
                                else
                                {
                                    dbHelper.AddInput(Request.Form.GetKey(i), Request.Form[i], SqlDbType.NVarChar);
                                }
                            }
                        }
                        DataSet ds = new DataSet();
                        string[] retValArray = new string[outputCount];
                        retVal = dbHelper.CallSP(Request["Target"], ref ds, ref retValArray);

                        SPBag bag = new SPBag
                        {
                            returnValue = retVal,
                            returnValueArray = retValArray,
                            dataSet = ds
                        };

                        Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(bag));
                    }
                    else
                    {
                        if (Request["Target"].StartsWith("fs_"))
                        {
                            string retVal = dbHelper.CallFS(Request["Target"]);
                            Response.Write(retVal);
                        }
                        else if (Request["Target"].StartsWith("ft_"))
                        {
                            DataTable t = dbHelper.CallFT(Request["Target"]);
                            Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(t));
                        }
                    }
                }
            }
        }
    }
}