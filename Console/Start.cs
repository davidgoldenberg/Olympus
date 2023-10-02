namespace Console;

public class Start
{
    private readonly string _filePath;
    public Start(string filePath)
    {
        _filePath = filePath;
    }
    public void Run()
    {
        var flipped = false;
        var are = new AutoResetEvent(flipped);
        var i = 0;
        while (i++ < 10)
        {
            new Thread(new LogWriter(are, _filePath).Write).Start();
            if (!flipped) // not waiting for the entire set of threads to be created before beginning work
            {
                flipped = true;
                are.Set();
            }    
        }
        System.Console.Read();
    }
}