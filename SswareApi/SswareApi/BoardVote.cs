using System.Collections.Generic;

namespace SswareApi
{
    public class BoardVote : XObject
    {
        public int Value;
        public int BoardItem;
        public int BoardComment;

        public BoardVote(int id) : base(9, id)
        {

        }

        public BoardVote(Dictionary<string, object> info) : base(9, info)
        {
            Value = (int)Info["Value"];
            BoardItem = (int)Info["BoardItem"];
            BoardComment = (int)Info["BoardComment"];
        }
    }
}
