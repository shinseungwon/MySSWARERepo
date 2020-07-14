namespace SswareApi
{
    public enum CodeType
    {
        Message = 1,
        XObjectType = 7,
        Language = 20,
        UserTitle = 23,
        UserType = 36,
        PositionType = 40
    }

    public enum XObjectType
    {        
        None = 0,
        User = 1,
        Group = 2,        
        Controller = 3,
        Action = 4,
        Element = 5,
        Board = 6,
        BoardItem = 7,
        BoardComment = 8,
        BoardVote = 9,
        File = 10,
        Folder = 11
    };

    public enum Language
    {
        English = 0,
        Korean = 1
    }

    public enum AuthorityType
    {
        None = 0,
        Read = 1,
        Modify = 2
    };

    public enum AuthorityValue
    {
        Deny = -1,
        NotSet = 0,
        Allow = 1
    };

    public enum DictionaryMode
    {
        Id = 0,
        Name = 1
    };

    public enum UserType
    {
        ItAdministrator = 1,
        Employee = 2,
        Guest = 3
    };

    public struct AuthoritySet
    {
        public AuthorityType AuthorityType;
        public AuthorityValue AuthorityValue;
    }
}