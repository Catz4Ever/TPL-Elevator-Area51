namespace Area_51
{
    internal class Elevator
    {
        private object elevatorLock = new object();
        private Queue<Floor> floorQueue = new Queue<Floor>();
        private Floor currentFloor = Floor.G;
        private bool isMoving = false;
        private Semaphore semaphore = new Semaphore(1, 1);
        private ManualResetEvent elevatorArrived = new ManualResetEvent(false);
        public List<Agent> agentsInElevator = new List<Agent>();
        private bool buttonPressed = false;

        public void CallElevator(Floor floor, string agentName)
        {
            lock (elevatorLock)
            {
                if (!buttonPressed)
                {
                    buttonPressed = true;
                    floorQueue.Enqueue(floor);
                    if (!isMoving)
                    {
                        isMoving = true;
                        Thread elevatorThread = new Thread(MoveElevator);
                        elevatorThread.Start();
                    }
                    Console.WriteLine($"{agentName} pressed the elevator button on floor {floor}");
                }
            }
        }

        private void MoveElevator()
        {
            while (floorQueue.Count > 0)
            {
                Floor destinationFloor = floorQueue.Dequeue();
                MoveToFloor(destinationFloor);
                OpenDoor(destinationFloor);
            }
            isMoving = false;
        }

        private void MoveToFloor(Floor destinationFloor)
        {
            int direction = Math.Sign((int)destinationFloor - (int)currentFloor);
            while (currentFloor != destinationFloor)
            {
                Thread.Sleep(1000);
                currentFloor = (Floor)((int)currentFloor + direction);
                Console.WriteLine($"Elevator is moving to floor {currentFloor}");
            }
            elevatorArrived.Set();
        }

        private void OpenDoor(Floor floor)
        {
            Console.WriteLine($"Elevator arrived at floor {floor}");
            semaphore.WaitOne();

            // Check if any agent inside the elevator needs to exit at the current floor
            Agent exitingAgent = agentsInElevator.Find(agent => agent.DestinationFloor == floor);

            if (exitingAgent != null)
            {
                SecurityLevel agentSecurityLevel = GetAgentSecurityLevel(Thread.CurrentThread as Thread);

                if (CheckSecurityLevel(floor, agentSecurityLevel))
                {
                    Console.WriteLine("Security check complete. Door opening...");
                    Thread.Sleep(1000);
                    Console.WriteLine("Door opened.");

                    Console.WriteLine($"{exitingAgent.Name} exited the elevator.");
                    ExitElevator(exitingAgent);
                }
                else
                {
                    Console.WriteLine("Security check failed. Agent does not have access to this floor.");
                }
            }
            else
            {
                // Continue with the security check for agents entering the elevator
                SecurityLevel agentSecurityLevel = GetAgentSecurityLevel(Thread.CurrentThread as Thread);

                if (CheckSecurityLevel(floor, agentSecurityLevel))
                {
                    Console.WriteLine("Security check complete. Door opening...");
                    Thread.Sleep(1000);
                    Console.WriteLine("Door opened.");

                    // Check if the current agent can enter the elevator
                    Agent currentAgent = agentsInElevator.Find(agent => agent.ThreadId == Thread.CurrentThread.ManagedThreadId);
                    if (currentAgent != null && currentAgent.DestinationFloor == floor)
                    {
                        Console.WriteLine($"{currentAgent.Name} entered the elevator.");
                    }
                }
                else
                {
                    Console.WriteLine("Security check failed. Agent does not have access to this floor.");
                }
            }

            semaphore.Release();
            Console.WriteLine($"{agentsInElevator.Count} agents");

        }

        private bool CheckSecurityLevel(Floor floor, SecurityLevel agentSecurityLevel)
        {
            switch (floor)
            {
                case Floor.G:
                    return agentSecurityLevel >= SecurityLevel.Confidential;
                case Floor.S:
                    return agentSecurityLevel >= SecurityLevel.Secret;
                case Floor.T1:
                case Floor.T2:
                    return agentSecurityLevel == SecurityLevel.TopSecret;
                default:
                    return false;
            }
        }

        private SecurityLevel GetAgentSecurityLevel(Thread agentThread)
        {
            if (agentThread.Name != null && agentThread.Name.Contains("TopSecret"))
            {
                return SecurityLevel.TopSecret;
            }
            else if (agentThread.Name != null && agentThread.Name.Contains("Secret"))
            {
                return SecurityLevel.Secret;
            }
            else
            {
                return SecurityLevel.Confidential;
            }
        }

        public void WaitForElevator()
        {
            elevatorArrived.WaitOne();
            elevatorArrived.Reset();
        }

        public void EnterElevator(Agent agent)
        {
            agentsInElevator.Add(agent);
        }
        public void ExitElevator(Agent agent)
        {
            lock (elevatorLock)
            {
                agentsInElevator.Remove(agent);
                Console.WriteLine($"{agent.Name} exited.");
                elevatorArrived.Reset();
            }
        }
        public int AgentsInElevatorCount
        {
            get { return agentsInElevator.Count; }
        }
    }
}