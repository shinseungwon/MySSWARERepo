using System;
using System.Collections.Generic;

namespace SswareApi
{
    public class XObject
    {
        protected readonly Dictionary<string, object> Info;

        public readonly int XObjectType;
        public readonly int Id;

        public readonly string Name;
        public List<Localize> Localizes;

        public XObject(int xObjectType, int id)
        {
            XObjectType = xObjectType;
            Id = id;
        }

        public XObject(int xObjectType, Dictionary<string, object> info)
        {
            XObjectType = xObjectType;
            Info = info;
            Id = (int)info["id"];

            if (Info.ContainsKey("Namez"))
            {
                if (Info["Namez"].GetType() == typeof(string))
                {
                    Name = (string)Info["Namez"];
                }
            }
        }
    }
}