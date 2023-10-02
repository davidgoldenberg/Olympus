using System.Text;

namespace Console;

/// <summary>
/// LogWriter is intended to be run using <see cref="System.Threading.Thread"/>
/// </summary>
public class LogWriter
{
    private readonly AutoResetEvent _are;
    private readonly string _path;  

    /// <summary>
    /// Uses the given <see cref="AutoResetEvent"/> to synchronize access to file at the given path
    /// </summary>
    /// <param name="are">The synchronizing <see cref="AutoResetEvent"/></param>
    /// <param name="path">The file's path</param>
    public LogWriter(AutoResetEvent are, string path)
    {
        _are = are;
        _path = path;
    }

    /// <summary>
    /// Writes log lines 10 times
    /// </summary>
    public void Write()
    {
        var i = 0;
        try
        {
            while (i++ < 10)
            {
                _are.WaitOne(); // using the AutoResetEvent as the safe locking mechanism for file access
                var lineNumber = LastLineNumber();
                WriteOnce(lineNumber);
                _are.Set();
            }
        }
        catch (Exception e)
        {
            try
            {
                using var stream = new FileStream("/log/error.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                using var errorWriter = new StreamWriter(stream);
                errorWriter.Write(e.ToString());
                throw;
            }
            catch (Exception exception)
            {
                System.Console.WriteLine("Error writing to file:");
                System.Console.WriteLine(e.ToString());
                System.Console.WriteLine(exception);
                throw;
            }
        }
    }

    // reads the last line of the file in order to get the last written line number
    private int LastLineNumber()
    {
        var stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        if (stream.Length > 50)
            stream.Seek(-50, SeekOrigin.End);

        var chunk = reader.ReadToEnd();
        var lineNumber = chunk.Split(Environment.NewLine)
            .Last(s => !string.IsNullOrEmpty(s) && s.Length > 2)
            .Split(',')
            .First()
            .Trim();
        
        return int.Parse(lineNumber) + 1;
    }
    
    /// <summary>
    /// Writes the line number, thread id, and current time stamp
    /// </summary>
    /// <param name="lineNumber">The line number for this line</param>
    private void WriteOnce(int lineNumber)
    {
        using var stream = new FileStream(_path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        var line = $"{lineNumber}, {Thread.CurrentThread.ManagedThreadId}, {DateTime.Now:hh:mm:ss:fff}{Environment.NewLine}";
        var bytes = Encoding.UTF8.GetBytes(line);
        stream.Write(bytes);
    }
}