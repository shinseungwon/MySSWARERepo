using System.Collections.Generic;

namespace SswareApi
{
    public class BoardItem : XObject
    {        
        public string Body;
        public int Board;

        public BoardItem(int id) : base(7, id)
        {

        }

        public BoardItem(Dictionary<string, object> info) : base(7, info)
        {
            Body = (string)Info["Body"];
            Board = (int)Info["Board"];
        }
    }
}
