namespace WeightTracker.Exceptions
{
    [Serializable]
    public class UsernameAlreadyExistsException : Exception
    {
        public string Username { get; }
        public UsernameAlreadyExistsException() { }
        public UsernameAlreadyExistsException(string message) : base(message) { }
        public UsernameAlreadyExistsException(string message, Exception exception) : base(message, exception) { }
        public UsernameAlreadyExistsException(string message, string username) : this(message)
        {
            Username = username;
        }
    }
}
