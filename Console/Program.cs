// See https://aka.ms/new-console-template for more information

using Console;
var file = "/log/out.txt";
try
{
    if (!Directory.Exists("/log"))
        Directory.CreateDirectory("/log");

    File.WriteAllText(file, $"0, 0, {DateTime.Now:hh:mm:ss:fff}{Environment.NewLine}");
}
catch (Exception e)
{
    System.Console.WriteLine("Exiting: unable to create log file at destination.");
    if (e is UnauthorizedAccessException)
        System.Console.WriteLine("Please check that the user has file permissions from the root path");
    
    return;
}
try
{
    new Start(file).Run();
}
catch
{
    System.Console.WriteLine("An exception occurred, see log file at /log/error.txt");
}
