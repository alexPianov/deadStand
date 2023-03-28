
namespace Playstel
{
    public class UiTransparencyOnStart : UiTransparency
    {
        public bool transparency;
        private void Start()
        {
            Transparency(transparency);
        }
    }
}