using System.Timers;
using Microsoft.Toolkit.Uwp.Notifications;

namespace TimerService
{
    internal class Program
    {
        private static System.Timers.Timer notificationTimer;

        private static int notificationCount = 0;

        private static DateTime startTime;
        private static int startExp;

        private static void Main(string[] args)
        {
            Console.WriteLine("Little timer service to help with afk nmz, press Ctrl+C to stop.");

            Console.WriteLine("How many seconds do you want the interval to be?");

            var intervalSeconds = GetNumber();
            var intervalMs = intervalSeconds * 1000;

            Console.WriteLine($"Windows Notifications every {intervalSeconds} seconds. Enter start exp to start.");

            startExp = Convert.ToInt32(Console.ReadLine());

            startTime = DateTime.Now;

            notificationTimer = new System.Timers.Timer(intervalMs);
            notificationTimer.Elapsed += OnTimedEvent;
            notificationTimer.AutoReset = true;
            notificationTimer.Enabled = true;

            while (true)
            {
                Console.WriteLine("Enter your new XP:");
                LogExperience(GetNumber());
            }
        }

        private static int GetNumber()
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

        private static void LogExperience(int newExp)
        {
            var delta = newExp - startExp;
            var elapsedTime = DateTime.Now - startTime;

            double hoursElapsed = elapsedTime.TotalHours;

            double expPerHour = hoursElapsed > 0 ? delta / hoursElapsed : 0;

            Console.WriteLine($"XP Per Hour: {expPerHour:F2}");
        }

        private static void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            ShowNotification();
        }

        private static void ShowNotification()
        {
            try
            {
                notificationCount++;
                new ToastContentBuilder()
                    .AddText("Reminder")
                    .AddText($"This is your #{notificationCount} scheduled notification!").Show();
            }
            catch
            {
                // discard
            }
        }
    }
}