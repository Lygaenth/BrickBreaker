namespace Cassebrique.Scenes.UI.MainMenu
{
    public class MenuAction
    {
        private string _action;
        public string Action { get => _action; }

        public readonly static MenuAction Start = new MenuAction("Start"); //, SignalName.RequestStart);
 
        public readonly static MenuAction Continue = new MenuAction("Continue");

        public readonly static MenuAction HighScore = new MenuAction("HighScore");

        public readonly static MenuAction Controls = new MenuAction("Controls");

        public readonly static MenuAction Quit = new MenuAction("Controls");

        public readonly static MenuAction Settings = new MenuAction("Settings");

        public readonly static MenuAction Audio = new MenuAction("Audio");

        public readonly static MenuAction Reset = new MenuAction("Reset");

        private MenuAction(string action)
        {
            _action = action;
        }

        public override bool Equals(object obj)
        {
            if (obj is MenuAction menuAction)
                return Action == menuAction.Action;

            return false;
        }
    }
}
