namespace RadheDevelopers.RealEstate
{
    public static class SessionManager
    {
        public static int LoggedInUserId { get; set; }
        public static string LoggedInUserRole { get; set; }
        public static string LoggedInUserName { get; set; }

        public static void ClearSession()
        {
            LoggedInUserId = 0;
            LoggedInUserRole = null;
            LoggedInUserName = null;
        }
    }
}
