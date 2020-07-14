using CORE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SSWARE_CORE_PROTOTYPE.Models.Shared;
using System;

namespace SSWARE_CORE_PROTOTYPE.Controllers
{
    public class ControllerBase : Controller
    {
        protected readonly IConfiguration configuration;
        protected DbHelper dbHelper;
        protected Logger logger;

        public ControllerBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            string connectionString = configuration.GetSection("ConnectionStrings").GetSection("MainDatabase").Value;
            string logFilePath = configuration.GetSection("LogFilePath").Value;

            dbHelper = new DbHelper(connectionString);
            logger = new Logger(logFilePath);
            dbHelper.SetLogger(logger);
            dbHelper.throwError = true;
        }

        protected bool CheckSigned(ref string message)
        {            
            UserManagementModel model = new UserManagementModel(configuration);

            string idBase64 = Request.Cookies["ID"];
            if (!string.IsNullOrEmpty(idBase64))
            {
                string id = System.Text.Encoding.Default.GetString(Convert.FromBase64CharArray(
                    idBase64.ToCharArray(), 0, idBase64.Length));
                    
                if (!string.IsNullOrEmpty(id))
                {
                    int result = model.SignIn(id, "", ref message);
                    return result == 1 ? false : true;
                }
            }

            return false;   
        }
    }
}
