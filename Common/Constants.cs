using System.Net;

namespace Common
{
    public class Constants
    {
        public const int ClientCount = 100;
        public static readonly IPAddress IpAddress = IPAddress.Parse("127.0.0.1");
        public const int UdpPort = 40001;
        public const int TcpPort = 40002;
        public const string ServerToClient = @"This is Ground Control to Major Tom
You've really made the grade
And the papers want to know whose shirts you wear
Now it's time to leave the capsule if you dare. OVER";
        public const string ClientToServer = @"This is Major Tom to Ground Control
I'm stepping through the door
And I'm floating in the most peculiar way
And the stars look very different today
For here am I sitting in my tin can
Far above the world
Planet Earth is blue
And there's nothing I can do

Though I'm past one hundred thousand miles
I'm feeling very still
And I think my spaceship knows which way to go
Tell my wife I love her very much, she knows. OVER";
    }
}
