using System;
using System.IO;
using System.Threading;

class Program
{
    private static readonly string filePath = "/junk/log/out.txt";
    private static int lineCount = 0; // Shared counter across threads
    private static readonly object fileLock = new object(); // Lock for synchronizing file access

    static void Main()
    {
        // Ensure log directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        // Initialize the file with the first line
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine($"0, 0, {DateTime.Now:HH:mm:ss.fff}");
        }

        // Launch 10 threads
        Thread[] threads = new Thread[10];
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(AppendLines);
            threads[i].Name = (i).ToString();
            threads[i].Start();
        }

        // Wait for all threads to complete
        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        // Wait for a character press before exiting
        Console.WriteLine("All threads have terminated. Press any key to exit...");
        Console.ReadKey();
    }

    static void AppendLines()
    {
        for (int i = 0; i < 10; i++)
        {
            lock (fileLock)
            {
                try
                {
                    string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff");
                    int currentLineCount = Interlocked.Increment(ref lineCount); // Increment line count safely

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine($"{currentLineCount}, {Thread.CurrentThread.Name}, {timeStamp}");
                    }
                }
                catch (Exception)
                {
                    //Do nothing, just keep trying next time.
                }
            }
        }
    }
}