using Godot;

namespace Cassebrique.Locators
{
    public interface IAutoLoaderProvider
    {
        void Initialize(Node node);

        HttpRequest GetHttopRequestNode();
    }
}