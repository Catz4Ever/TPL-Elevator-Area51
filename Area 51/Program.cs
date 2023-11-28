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
            Agent agent1 = new Agent("AAAA", SecurityLevel.Confidential, elevator);
            Agent agent2 = new Agent("BBBB", SecurityLevel.Secret, elevator);
            Agent agent3 = new Agent("CCCC", SecurityLevel.TopSecret, elevator);
            Agent agent4 = new Agent("DDDD", SecurityLevel.Secret, elevator);
            Agent agent5 = new Agent("EEEE", SecurityLevel.TopSecret, elevator);
            Agent agent6 = new Agent("FFFF", SecurityLevel.Confidential, elevator);
            Agent agent7 = new Agent("GGGG", SecurityLevel.TopSecret, elevator);

            Thread agent1Thread = new Thread(agent1.UseElevator);
            Thread agent2Thread = new Thread(agent2.UseElevator);
            Thread agent3Thread = new Thread(agent3.UseElevator);
            Thread agent4Thread = new Thread(agent4.UseElevator);
            Thread agent5Thread = new Thread(agent5.UseElevator);
            Thread agent6Thread = new Thread(agent6.UseElevator);
            Thread agent7Thread = new Thread(agent7.UseElevator);

            agent1Thread.Start();
            agent2Thread.Start();
            agent3Thread.Start();
            agent4Thread.Start();
            agent5Thread.Start();
            agent6Thread.Start();
            agent7Thread.Start();

            agent1Thread.Join();
            agent2Thread.Join();
            agent3Thread.Join();
            agent4Thread.Join();
            agent5Thread.Join();
            agent6Thread.Join();
            agent7Thread.Join();

            while (elevator.AgentsInElevatorCount > 0)
            {
                Thread.Sleep(1000);
            }
        }
    }
}