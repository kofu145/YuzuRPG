namespace YuzuRPG.Core;

public enum TransitionType {
    TOPDOWN = 0,
    STRIPEDSIDEBYSIDE = 1
}

public class Transition
{
    private int speed;
    public TransitionType transitionType;
    public Transition(int speed, TransitionType transitionType)
    {
        this.speed = speed;
        this.transitionType = transitionType;
    }

    public void Render()
    {
        switch (transitionType)
        {
            case TransitionType.TOPDOWN:
                // basic falling down transition
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < Console.WindowHeight; i++)
                {
                    Console.WriteLine(new string(' ', Console.WindowWidth - 1));
                    Thread.Sleep(speed);
                }
                Console.Clear();
                break;
            case TransitionType.STRIPEDSIDEBYSIDE:
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    var k = 0;
                    for (int j = 0; j < Console.WindowHeight; j++)
                    {
                        k = Console.WindowWidth - i;
                        if (j % 2 == 0)
                        {
                            Console.SetCursorPosition(0, j);
                            Console.Write(new string(' ', i));
                        }
                        else
                        {
                            Console.SetCursorPosition(k - 1, j);
                            Console.Write(new string(' ', i));
                        }
                        if (j != Console.WindowHeight - 1)
                            Console.WriteLine();
                    }
                    Thread.Sleep(new TimeSpan(7000));
                }
                Console.ResetColor();
                Console.Clear();
                break;
            default:
                // basic falling down transition
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < Console.WindowHeight; i++)
                {
                    Console.WriteLine(new string(' ', Console.WindowWidth - 1));
                    Thread.Sleep(speed);
                }
                Console.Clear();
                break;
        }
    }
}