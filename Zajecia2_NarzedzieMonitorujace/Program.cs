using System;
using System.Diagnostics;
using System.Threading;

namespace SystemMonitor
{
    class Program
    {
        private static PerformanceCounter cpuCounter;
        private static PerformanceCounter ramCounter;
        private static EventLog eventLog;

        static void Main(string[] args)
        {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                ramCounter = new PerformanceCounter("Memory", "Available MBytes");

                InitializeEventLog();

                Console.WriteLine("Rozpoczęcie monitorowania:");

                // Start monitorowania
                Thread monitoringThread = new Thread(new ThreadStart(MonitorSystem));
                monitoringThread.IsBackground = true;
                monitoringThread.Start();

                Console.ReadLine();
        }

        static void InitializeEventLog()
        {
                if (!EventLog.SourceExists("SystemMonitor"))
                {
                    EventLog.CreateEventSource("SystemMonitor", "Application");
                }
                eventLog = new EventLog
                {
                    Source = "SystemMonitor",
                    Log = "Application"
                };
        }

        static void MonitorSystem()
        {
            while (true)
            {
                float cpuUsage = cpuCounter.NextValue();
                float availableMemory = ramCounter.NextValue();

                Console.WriteLine($"Uzycie procesora: {cpuUsage}%");
                Console.WriteLine($"Uzycie pamieci: {availableMemory}MB");

                if (cpuUsage > 80)
                {
                    LogEvent("Wysokie uzycie procesora: " + cpuUsage + "%", EventLogEntryType.Warning);
                }
                if (availableMemory < 200)
                {
                    LogEvent("Mało dostępnej pamięci: " + availableMemory + "MB", EventLogEntryType.Warning);
                }

                Thread.Sleep(1000);
            }
        }

        static void LogEvent(string message, EventLogEntryType type)
        {
            eventLog.WriteEntry(message, type);
        }
    }
}
