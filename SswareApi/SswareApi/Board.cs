using System.Collections.Generic;

namespace SswareApi
{
    public class Board : XObject
    {
        public Board(int id) : base(6, id)
        {

        }

        public Board(Dictionary<string, object> info) : base(6, info)
        {

        }
    }
}
