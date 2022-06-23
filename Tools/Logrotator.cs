using System;
using System.IO;
using System.IO.Compression;
using System.Timers;
using Serilog;

namespace Tools;

public class Logrotator : IDisposable
{
    private const string logsDir = $"./Logs";
    private const string zipDir = $"./LogBackup";

    private string zipFilePath
    {
        get
        {
            var d = DateTime.Now.AddMonths(-1);
            return $"./{zipDir}/{d.Year}-{d.Month}.zip";
        }
    }

    private Timer timer;
    private int intervalHours = 1;

    public Logrotator()
    {
        timer = new Timer(intervalHours * 60 * 60 * 1000);
        timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
    }

    private void OnTimedEvent(object obj, ElapsedEventArgs e)
    {
        if (!File.Exists(zipFilePath))
        {
            Directory.CreateDirectory(zipDir);

            Log.Information("LogsBackup : 開始壓縮Logs...");
            ZipFile.CreateFromDirectory(logsDir, zipFilePath, CompressionLevel.Optimal, false);
            ClearLogsFolder();
            Log.Information("LogsBackup : Done");
        }
    }

    private void ClearLogsFolder()
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(logsDir);
            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }
        }
        catch (System.Exception ex)
        {
            Log.Warning(ex, "清空Logs失敗");
        }
    }

    public void Dispose()
    {
        timer.Stop();
        timer.Dispose();
    }
}