public static class UserStore
{
    private static readonly List<UserProfile> _users = new List<UserProfile>();

    public static void AddUser(UserProfile user)
    {
        if (!_users.Any(u => u.Email == user.Email && u.Provider == user.Provider))
        {
            _users.Add(user);
        }
    }

    public static void RemoveUser(string email, string provider)
    {
        var user = _users.FirstOrDefault(u => u.Email == email && u.Provider == provider);
        if (user != null)
        {
            _users.Remove(user);
        }
    }

    public static List<UserProfile> GetAllUsers()
    {
        return _users;
    }
}
