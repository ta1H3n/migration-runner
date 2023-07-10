// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using MigrationRunner;


var text = File.ReadAllText("c_strings.txt");
var Dbs = JsonSerializer.Deserialize<List<DatabaseInfo>>(text);
var SelectedDbs = new List<DatabaseInfo>();

try
{
    while (true)
    {
        var db = SelectDb(Dbs);
        if (db != null)
        {
            if (SelectedDbs.Contains(db))
            {
                Console.WriteLine("Already added...");
                continue;
            }

            SelectedDbs.Add(db);
            SelectedDbs = SelectedDbs.OrderBy(x => Dbs.FindIndex(y => y.label == x.label)).ToList();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            ListDbs(SelectedDbs, "Selected dbs:");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            break;
        }
    }

    ListDbs(SelectedDbs, "Confirm running migration on the following dbs:");
    Console.WriteLine("Say ok");
    if (ReadLine() == "ok")
    {
        foreach (var db in SelectedDbs)
        {
            Console.WriteLine($"Executing migrations on {db.label}...");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(Execute(db.connectionString));
            Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Done for {db.label}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }
    }

    Console.WriteLine("Finished...");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Wtf?: {ex}");
    Console.ForegroundColor = ConsoleColor.White;
}
finally
{
    Console.ReadLine();
}

string Execute(string connectionString)
{
    Process p = new Process();
    // Redirect the output stream of the child process.
    p.StartInfo.UseShellExecute = false;
    p.StartInfo.RedirectStandardOutput = true;
    p.StartInfo.FileName = "cmd.exe";
    p.StartInfo.Arguments = $"/C dotnet ef database update --connection \"{connectionString}\"";
    p.Start();
    
    // Read the output stream first and then wait.
    var output = p.StandardOutput.ReadToEnd();
    p.WaitForExit();

    return output;
}

void ListDbs(List<DatabaseInfo> dbs, string msg)
{
    Console.WriteLine(msg);
    int i = 0;
    foreach (var db in dbs)
    {
        i++;
        Console.WriteLine($"{i}. {db.label}");
    }
    Console.WriteLine();
}

DatabaseInfo SelectDb(List<DatabaseInfo> dbs)
{
    ListDbs(dbs, "Available dbs:");
    Console.WriteLine("Select additional db or 'ok'");
    
    while (true)
    {
        try
        {
            var read = ReadLine();
            if (read == "ok")
            {
                return null;
            }

            var i = int.Parse(read) - 1;
            var db = dbs[i];
            if (db != null)
            {
                return db;
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Wtf?: {ex}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}

string ReadLine()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write(">>> ");
    var response = Console.ReadLine();
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine();
    return response;
}