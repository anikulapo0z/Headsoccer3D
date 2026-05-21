using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameLogs
{
    static string logFolder = Path.Combine(Application.persistentDataPath, "Logs");
    static string timeFormat = "HH:mm:ss";
    static string dateFormat = "yyyy-MM-dd";

    static string dailyFile = Path.Combine(logFolder, $"log_{DateTime.Now.ToString(dateFormat)}.txt");
    static string timestamp = DateTime.Now.ToString(timeFormat);

    static Dictionary<int, (string message, DateTime time)> activeTimers = new();



    public static void WriteMessage(string message)
    {
        WriteLine($"[{timestamp}] [WriteMessage] {message}");
    }

    public static void StartTimer(int id, string message)
    {
        if (activeTimers.ContainsKey(id))
        {
            WriteLine($"[{timestamp}] [StartTimer] ID = {id} | Timer already exists: ({activeTimers[id].message}) | Starting new timer");
        }
        activeTimers[id] = (message, DateTime.Now);
        WriteLine($"[{timestamp}] [StartTimer] Starting timer: ({activeTimers[id].message}) | ID = {id}");
    }
    public static void EndTimer(int id)
    {
        if (!activeTimers.ContainsKey(id))
        {
            WriteLine($"[{timestamp}] [EndTimerError] Timer ID ({id}) does not exist, no timer to end");
            return;
        }
        TimeSpan elapsed = DateTime.Now - activeTimers[id].time;

        WriteLine($"[{timestamp}] [EndTimer] Ending timer ID: ({id}) | Message: ({activeTimers[id].message})");
        WriteLine($"[{timestamp}] [TimerDuration] Timer duration: ({elapsed})");
        activeTimers.Remove(id);
    }


    private static void WriteLine(string line)
    {
        try
        {
            TryCreateFolder();
            File.AppendAllText(dailyFile, line + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[GameLogs] Could not write log: {ex.Message}");
        }
    }

    private static void TryCreateFolder()
    {
        if (!Directory.Exists(logFolder))
            Directory.CreateDirectory(logFolder);
    }


}
