using PaintShopIMS.Models;

namespace PaintShopIMS.Helpers
{
    /// <summary>
    /// Global state maintaining the currently authenticated User session.
    /// </summary>
    public static class CurrentSession
    {
        public static User? CurrentUser { get; private set; }

        public static bool IsAdmin => CurrentUser?.IsAdmin ?? false;

        public static void SetUser(User u)
        {
            CurrentUser = u;
        }

        public static void Clear()
        {
            CurrentUser = null;
        }
    }
}
