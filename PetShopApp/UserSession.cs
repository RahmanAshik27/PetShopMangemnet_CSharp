using System;

namespace PetShopApp
{
    public static class UserSession
    {
        // Eikhane login kora user-er shob info save thakbe
        public static string CurrentUsername { get; set; }
        public static string CurrentFullName { get; set; }
        public static int CurrentUserID { get; internal set; }
    }
}