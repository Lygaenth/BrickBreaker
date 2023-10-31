using Godot;

namespace Cassebrique.Locators
{
    public class AutoLoaderProvider : IAutoLoaderProvider
    {

        private Node _mainNode;

        public void Initialize(Node mainNode)
        {
            _mainNode = mainNode;
        }

        public HttpRequest GetHttopRequestNode()
        {
            return _mainNode.GetNode<HttpRequest>("/root/HighScoreHttpRequest");
        }

        public SoundService GetSoundService()
        {
            return _mainNode.GetNode<SoundService>("/root/SoundService");
        }
    }
}
