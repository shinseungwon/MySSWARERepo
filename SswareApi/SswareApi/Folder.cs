using System.Collections.Generic;

namespace SswareApi
{
    public sealed class Folder : XObject
    {
        public Folder(int id) : base(11, id)
        {

        }

        public Folder(Dictionary<string, object> info) : base(11, info)
        {

        }
    }
}
