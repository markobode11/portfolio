using System;

namespace MenuSystem
{
    public sealed class MenuItem
    {
        private string Label { get; }
        public string UserChoice { get; }

        public Func<string> MethodToExecute { get; } = null!;

        public Action MethodDependingOnMenuLevel { get; } = null!;


        public MenuItem(string label, string userChoice, Func<string> methodToExecute)
        {
            Label = label.Trim();
            UserChoice = userChoice.Trim();
            MethodToExecute = methodToExecute;
        }

        public MenuItem(string label, string userChoice, Action methodToExecute)
        {
            Label = label.Trim();
            UserChoice = userChoice.Trim();
            MethodDependingOnMenuLevel = methodToExecute;
        }

        public MenuItem(string label, string userChoice, Action methodToExecute, Func<string> secondMethodToExecute)
        {
            Label = label.Trim();
            UserChoice = userChoice.Trim();
            MethodDependingOnMenuLevel = methodToExecute;
            MethodToExecute = secondMethodToExecute;
        }

        public override string ToString()
        {
            return UserChoice + ") " + Label;
        }
    }
}