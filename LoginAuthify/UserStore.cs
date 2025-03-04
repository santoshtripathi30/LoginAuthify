public static class UserStore
{
    private static readonly List<UserProfile> _users = new List<UserProfile>();

    public static void AddUser(UserProfile user)
    {
        if (!_users.Any(u => u.Email == user.Email))
        {
            _users.Add(user);
        }
    }

    public static List<UserProfile> GetAllUsers()
    {
        return _users;
    }
}
