using System.Xml.Linq;

namespace Area_51
{
    internal class Agent
    {
        public string Name;
        private SecurityLevel securityLevel;
        private Elevator elevator;
        public Floor DestinationFloor { get; private set; }
        public int ThreadId { get; private set; }
        public Agent(string name, SecurityLevel securityLevel, Elevator elevator)
        {
            this.Name = name;
            this.securityLevel = securityLevel;
            this.elevator = elevator;
        }

        public void UseElevator()
        {
            Random random = new Random();
            Floor destinationFloor = (Floor)random.Next(Enum.GetValues(typeof(Floor)).Length);
            Console.WriteLine($"{Name} is calling the elevator to floor {destinationFloor}");
            elevator.CallElevator(destinationFloor, Name);
            Console.WriteLine($"{Name} is waiting for the elevator");
            elevator.WaitForElevator();
            elevator.EnterElevator(this);
            Console.WriteLine($"{Name} is inside the elevator");
        }
    }
}