using CORE;
using Microsoft.Extensions.Configuration;

namespace SSWARE_CORE_PROTOTYPE.Models
{
    public class ModelBase
    {        
        private readonly IConfiguration configuration;
        public DbHelper dbHelper;
        public Logger logger;

        public ModelBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            string connectionString = configuration.GetSection("ConnectionStrings").GetSection("MainDatabase").Value;
            string logFilePath = configuration.GetSection("LogFilePath").Value;

            dbHelper = new DbHelper(connectionString);
            logger = new Logger(logFilePath);
            dbHelper.SetLogger(logger);
            dbHelper.throwError = true;            
        }
    }
}
