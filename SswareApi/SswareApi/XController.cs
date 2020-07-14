using System.Collections.Generic;

namespace SswareApi
{
    public class XController : XObject
    {
        public string Title;
        public int Parent;

        public XController(int id) : base(3, id)
        {

        }

        public XController(Dictionary<string, object> info) : base(3, info)
        {
            Title = (string)info["Title"];
            Parent = (int)info["Parent"];
        }
    }
}
