using System.Collections.Generic;

namespace SswareApi
{
    public class XAction : XObject
    {
        public string Header;
        public int Parent;

        public XAction(int id) : base(4, id)
        {

        }

        public XAction(Dictionary<string, object> info) : base(4, info)
        {
            Header = (string)Info["Header"];
            Parent = (int)Info["Parent"];
        }
    }
}
