
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using SswareApi;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Command command = new Command("192.168.0.27", "SSWARE", "sa", "sungwon@530");
            //command.LogIn("ssw900528@gmail.com", "sungwon530");

            CommandTest(command);
            CommandUserTest(command);
            CommandFileTest(command);
            CommandControllerTest(command);
            CommandBoardTest(command);
            CommandEtcTest(command);
        }

        public static void CommandTest(Command command)
        {
            command.LogIn("ssw900528@gmail.com", "sungwon530");
            List<User> users = command.UserManager.GetUserList();
            foreach(User u in users)
            {
                Console.WriteLine(u.LoginId + "/" + u.Name);
            }
        }

        public static void CommandUserTest(Command command)
        {
            //command.UserManager.AddUser("신승원", 0, 0, null, "sungwon530", Language.English);
        }

        public static void CommandFileTest(Command command)
        {

        }

        public static void CommandControllerTest(Command command)
        {

        }

        public static void CommandBoardTest(Command command)
        {

        }

        public static void CommandEtcTest(Command command)
        {

        }
    }
}