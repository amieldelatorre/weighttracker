namespace WeightTracker.Models
{
    public abstract class ICreate
    {
        protected internal Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();

        protected internal void AddToErrors(string key, string message)
        {
            if (Errors.ContainsKey(key))
                Errors[key].Add(message);
            else
                Errors[key] = new List<string>() { message };
        }

        public Dictionary<string, List<string>> GetErrors() { return Errors; }
    }
}
