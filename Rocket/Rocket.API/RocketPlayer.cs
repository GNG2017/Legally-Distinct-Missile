namespace Rocket.API
{
    public class RocketPlayer : IRocketPlayer
    {
        private readonly string id;
        public string Id => id;

        private readonly string displayName;
        public string DisplayName => displayName;

        private readonly bool isAdmin;
        public bool IsAdmin => isAdmin;

        public RocketPlayer(string Id, string DisplayName = null, bool IsAdmin = false)
        {
            id = Id;
            if (DisplayName != null)
                displayName = DisplayName;

            isAdmin = IsAdmin;
        }

        public int CompareTo(object obj) => Id.CompareTo(((IRocketPlayer)obj).Id);
    }
}
