namespace Cassebrique.Scenes.UI
{
    public class TimedMessage
    {
        public string Message { get; }
        public float Time { get; }

        public TimedMessage(string message, float time)
        {
            Message = message;
            Time = time;
        }
    }
}
