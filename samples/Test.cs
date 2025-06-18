#:import *.cs
Console.WriteLine("Welcome again!");
Example.Test();
int a = 2;
int b = 3;
if (args.Length == 1)
{
    a = int.Parse(args[0]);
}
else if (args.Length == 2)
{
    a = int.Parse(args[0]);
    b = int.Parse(args[1]);
}
Calculator.Add(a,b);
