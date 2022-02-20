namespace WeightTracker.Exceptions
{
    [Serializable]
    public class UsernameNotFoundException : Exception
    {
        public string Username { get; }
        public UsernameNotFoundException() { }
        public UsernameNotFoundException(string message) : base(message) { }
        public UsernameNotFoundException(string message, Exception exception) : base(message, exception) { }
        public UsernameNotFoundException(string message, string username) : this(message)
        {
            Username = username;
        }
    }
}
