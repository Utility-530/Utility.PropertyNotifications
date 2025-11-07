namespace Utility.Pipes
{
    public abstract record QueueItem
    {
        public QueueItem()
        {
            DateTime = DateTime.Now;
        }

        public DateTime DateTime { get; }

        public abstract void Invoke();
    }
}