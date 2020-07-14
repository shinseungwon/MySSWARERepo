using Helper;
using System.Collections.Generic;

namespace SswareApi
{
    public sealed class XFile : XObject
    {
        public XFile(int id) : base(10, id)
        {

        }

        public XFile(Dictionary<string, object> info) : base(10, info)
        {

        }
    }
}
