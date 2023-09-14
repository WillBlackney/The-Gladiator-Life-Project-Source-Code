using WeAreGladiators.Utilities;

namespace WeAreGladiators.VisualEvents
{
    public class VisualEventUpdater : Singleton<VisualEventUpdater>
    {
        private void Update()
        {
            VisualEventManager.PlayNextEventFromQueue();
        }
    }
}
