namespace SswareApi
{
    public sealed class Localize
    {
        public readonly int Id;
        public readonly int XObjectType;
        public readonly int Key;
        public string Item;
        public string Memo;
        public string English;
        public string Korean;        

        public Localize(int id, int xObjectType, int key, string item
            , string memo, string english, string korean)
        {
            Id = id;
            XObjectType = xObjectType;
            Key = key;
            Item = item;
            Memo = memo;
            English = english;
            Korean = korean;            
        }
    }
}