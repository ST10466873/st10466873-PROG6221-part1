using System;
using System.Collections.Generic;
using System.Text;

namespace st10466873_PROG6221_poE
{
    internal class UserSession
    {
        public string UserName { get; set; } = "User";
        public string FavoriteTopic { get; set; } = "";

        public string LastTopic { get; set; } = "";

        public Dictionary<string, string> Preferences { get; set; } = new Dictionary<string, string>();
    }
}
