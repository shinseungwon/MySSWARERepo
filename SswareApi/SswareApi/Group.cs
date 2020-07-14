using System.Collections.Generic;

namespace SswareApi
{
    public sealed class Group : XObject
    {
        public int PositionType;

        public Group(int id) : base(2, id)
        {

        }

        public Group(Dictionary<string, object> info) : base(2, info)
        {

        }
    }
}