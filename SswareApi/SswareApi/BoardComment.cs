using System.Collections.Generic;

namespace SswareApi
{
    public class BoardComment : XObject
    {
        public string Body;
        public int BoardItem;
        public int Parent;

        public BoardComment(int id) : base(8, id)
        {

        }

        public BoardComment(Dictionary<string, object> info) : base(8, info)
        {
            Body = (string)Info["Body"];
            BoardItem = (int)Info["BoardItem"];
            Parent = (int)Info["Parent"];
        }
    }
}
