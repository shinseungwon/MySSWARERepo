using System.Collections.Generic;

namespace SswareApi
{
    public class Element : XObject
    {
        public string DomId;
        public int Parent;

        public Element(int id) : base(5, id)
        {

        }

        public Element(Dictionary<string, object> info) : base(5, info)
        {
            DomId = (string)Info["DomId"];
            Parent = (int)Info["Parent"];
        }
    }
}
