namespace Area_51
{
    public enum SecurityLevel
    {
        Confidential,
        Secret,
        TopSecret
    }

    public enum Floor
    {
        G,
        S,
        T1,
        T2
    }
    class Program
    {
        static void Main()
        {
            Elevator elevator = new Elevator();

            Random random = new Random();
            Array values = Enum.GetValues(typeof(SecurityLevel));
            List<Thread> agentThreads = new List<Thread>(7);

            for (int i = 0; i < 7; i++)
            {
                Agent agent = new Agent(i.ToString(), (SecurityLevel)values.GetValue(random.Next(values.Length)), elevator);
                Thread t = new Thread(() => { agent.UseElevator(); });
                t.Start();
                agentThreads.Add(t);
            }

            foreach (var t in agentThreads) t.Join();
        }
    }
}