namespace SpiroWeb.Managers
{
    public static class UserPermissionsManager
    {
        //static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        static public bool IsUserModerator(string userId)
        {
            if (userId.Equals("d3d48305-4527-49ac-a930-49e4a511af14") || userId.Equals("9ff8224f-17cf-49fb-b555-05779a13eb40"))
            {
                return true;

            }
            return false;
        }
    }
}