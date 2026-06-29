using System.Media;

namespace PaintShopIMS.Helpers
{
    public static class SoundHelper
    {
        public static void PlaySuccess()
        {
            SystemSounds.Beep.Play();
        }

        public static void PlayError()
        {
            SystemSounds.Hand.Play();
        }
    }
}
