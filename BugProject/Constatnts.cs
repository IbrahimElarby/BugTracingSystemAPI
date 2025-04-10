namespace BugProject
{
    public static class Constatnts
    {
        public static class Policies
        {
            public const string ForAdminOnly = "ForAdminOnly";
            public const string ForDev = "ForDev";
        }
    }

    enum BugRoles
    {
        Manager,
        Developer,
        Tester,
        User
    }
}
