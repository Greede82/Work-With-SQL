using System;

namespace WpfApp1.Models
{
    public static class UserSession
    {
        public static User CurrentUser { get; private set; }
        public static bool IsLoggedIn => CurrentUser != null;

        public static event EventHandler UserLoggedIn;
        public static event EventHandler UserLoggedOut;

        public static void Login(User user)
        {
            CurrentUser = user;
            UserLoggedIn?.Invoke(null, EventArgs.Empty);
        }

        public static void Logout()
        {
            CurrentUser = null;
            UserLoggedOut?.Invoke(null, EventArgs.Empty);
        }

        public static bool HasPermission(UserRole minimumRole)
        {
            if (!IsLoggedIn) return false;
            return CurrentUser.Role <= minimumRole;
        }
    }
}
