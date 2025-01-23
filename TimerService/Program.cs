using System.Timers;
using Microsoft.Toolkit.Uwp.Notifications;

namespace TimerService
{
    internal class NotificationTimerWrapper
    {
        private static int _totalTimerCount;

        private readonly System.Timers.Timer _timer;
        private readonly string? _notificationMessage;
        private int _notificationCount;
        public readonly string Identifier;
        public readonly int Id;

        public NotificationTimerWrapper(TimeSpan timerTime, string? notificationMessage, string identifier)
        {
            _totalTimerCount++;
            Id = _totalTimerCount;
            Identifier = identifier;
            _notificationCount = 0;
            _notificationMessage = notificationMessage;
            _timer = new System.Timers.Timer(timerTime);
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            ShowNotification();
        }

        private void ShowNotification()
        {
            try
            {
                _notificationCount++;
                new ToastContentBuilder()
                    .AddText(_notificationMessage)
                    .AddText($"This is your #{_notificationCount} scheduled notification!").Show();
            }
            catch
            {
                // discard
            }
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        public void WriteIdentifier()
        {
            Console.WriteLine($"{Id}. {Identifier}");
        }
    }

    internal class ProgramPathOption(int id, string text, Action programPath)
    {
        public int Id = id;

        public void WriteMessage()
        {
            Console.WriteLine(text);
        }

        public void ExecutePath()
        {
            programPath.Invoke();
        }
    }

    internal class ProgramPathOptionsCollection(List<ProgramPathOption> programPathOptions)
    {
        public ProgramPathOptionsCollection() : this([])
        {
        }

        public void AddProgramPathOption(ProgramPathOption programPathOption)
        {
            programPathOptions.Add(programPathOption);
        }

        public bool OptionExists(int id)
        {
            return programPathOptions.Any(p => p.Id == id);
        }

        public void WriteAllMessages()
        {
            programPathOptions.ForEach(p => p.WriteMessage());
        }

        public void RunSelectedOption()
        {
            Console.Write("Your pick: ");
            var selectedOption = Helpers.GetNumber();

            if (OptionExists(selectedOption))
            {
                var selectedProgramPathOption = programPathOptions.First(p => p.Id == selectedOption);
                selectedProgramPathOption.ExecutePath();
            }
            else
            {
                Console.WriteLine("Not a valid option.");
            }
        }
    }

    internal class Helpers
    {
        public static int GetNumber()
        {
            var flag = true;
            var result = 0;

            while (flag)
            {
                try
                {
                    result = Convert.ToInt32(Console.ReadLine());
                    flag = false;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Not a number!");
                }
            }

            return result;
        }
    }

    internal class Program
    {
        private static bool _running = true;

        private static readonly List<NotificationTimerWrapper> Timers = [];

        private static void Main(string[] args)
        {
            Console.WriteLine("Little timer service to help with afk nmz, press Ctrl+C to stop.");
            Console.WriteLine("It will constantly run a timer based on a given amount of seconds and send windows notifications based on your provided message.");

            var programPathOptions = new ProgramPathOptionsCollection();

            programPathOptions.AddProgramPathOption(new ProgramPathOption(1, "1. Setup new timer?", SetupNewTimer));
            programPathOptions.AddProgramPathOption(new ProgramPathOption(2, "2. Stop a timer?", StopATimer));
            programPathOptions.AddProgramPathOption(new ProgramPathOption(3, "3. Stop application?", StopApplication));

            while (_running)
            {
                programPathOptions.WriteAllMessages();
                programPathOptions.RunSelectedOption();
            }
        }

        private static void StopApplication()
        {
            Console.WriteLine("Closing app.");
            Timers.ForEach(t => t.StopTimer());
            Timers.Clear();
            Thread.Sleep(500);
            _running = false;
        }

        private static void StopATimer()
        {
            Console.WriteLine("Stop a timer selected.");
            if (Timers.Count == 0)
            {
                Console.WriteLine("No timers to stop.");
                return;
            }

            Timers.ForEach(t => t.WriteIdentifier());
            Console.WriteLine("Which timer would you like to stop?");
            Console.Write("Your pick: ");

            var selectedTimer = Helpers.GetNumber();

            var timer = Timers.FirstOrDefault(t => t.Id == selectedTimer);

            if (timer is null)
            {
                Console.WriteLine("No timer with that id found.");
                return;
            }

            timer.StopTimer();
            Timers.Remove(timer);
        }

        private static void SetupNewTimer()
        {
            Console.WriteLine("Setup new timer selected.");

            Console.WriteLine("How many second should the timer be?");
            Console.Write("Seconds: ");

            var seconds = Helpers.GetNumber();

            if (seconds <= 0)
            {
                Console.WriteLine("Timespan can not be less than or equal to zero seconds.");
                return;
            }

            Console.WriteLine("What would you like to name the timer? Should be unique.");
            Console.Write("Name: ");

            var givenName = Console.ReadLine();

            if (Timers.Any(t => t.Identifier == givenName))
            {
                Console.WriteLine("Name is not unique.");
                return;
            }

            if (string.IsNullOrWhiteSpace(givenName))
            {
                Console.WriteLine("An empty name is not valid.");
                return;
            }

            Console.WriteLine("What message should the windows notification show?");
            Console.Write("Message: ");
            var givenMessage = Console.ReadLine();

            Timers.Add(new NotificationTimerWrapper(TimeSpan.FromSeconds(seconds), givenMessage, givenName));
        }
    }
}