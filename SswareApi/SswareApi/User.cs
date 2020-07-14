using System.Collections.Generic;

namespace SswareApi
{
    public sealed class User : XObject
    {
        public UserType UserType;
        public int UserTitle { get; }
        public string LoginId { get; }
        public Language Language;
        public bool IsUse;

        public int position;

        public User(int id) : base(1, id)
        {

        }

        public User(Dictionary<string, object> info) : base(1, info)
        {
            UserType = (UserType)(int)Info["Typez"];
            UserTitle = (int)Info["Title"];
            LoginId = (string)Info["LoginId"];
            IsUse = (bool)Info["IsUse"];
            Language = (Language)(int)Info["Languagez"];
        }
    }
}