using Rocket.API;

namespace Rocket.Core.Tests
{
    public class TestingPlayer : IRocketPlayer
    {
        public string DisplayName => "TestingPlayer";
        public TestingPlayer(string id = "1", bool admin = false)
        {
            IsAdmin = admin;
            Id = id;
        }

        private bool isAdmin;
        public bool IsAdmin
        {
            get => isAdmin;
            set => isAdmin = value;
        }

        private string id;

        public string Id
        {
            get => id;

            set => id = value;
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(((IRocketPlayer)obj).Id);
        }
    }
}
