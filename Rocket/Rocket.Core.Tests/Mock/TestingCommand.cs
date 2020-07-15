using Rocket.API;
using System;
using System.Collections.Generic;

namespace Rocket.Core.Tests
{
    public class TestingCommand : IRocketCommand
    {
        public TestingCommand(string name)
        {
            Name = name;
        }

        private List<string> aliases = new List<string>();

        private AllowedCaller allowedCaller = AllowedCaller.Both;

        private string help = "Test Help";

        private string name = "test";

        private List<string> permissions = new List<string>() { "test" };

        private string syntax = "";

        public List<string> Aliases
        {
            get => aliases;

            set => aliases = value;
        }

        public AllowedCaller AllowedCaller
        {
            get => allowedCaller;

            set => allowedCaller = value;
        }

        public string Help
        {
            get => help;

            set => help = value;
        }

        public string Name
        {
            get => name;

            set => name = value;
        }

        public List<string> Permissions
        {
            get => permissions;

            set => permissions = value;
        }

        public string Syntax
        {
            get => syntax;

            set => syntax = value;
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            throw new NotImplementedException();
        }
    }
}
