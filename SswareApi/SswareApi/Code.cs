namespace SswareApi
{
    public sealed class Code
    {
        public readonly int Id;
        public readonly bool IsSystem;
        public string Name;
        public int Parent;
        public int SettingValue;
        public int Value;
        public int Sort;
        public string Memo;        
        public Localize Localize;

        public Code(int id, string name, int parent, int settingValue
            , int value, int sort, string memo, bool isSystem, Localize localize)
        {
            Id = id;
            Name = name;
            Parent = parent;
            SettingValue = settingValue;
            Value = value;
            Sort = sort;
            Memo = memo;
            IsSystem = isSystem;

            Localize = localize;
        }      
    }
}