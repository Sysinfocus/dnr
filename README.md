# DotNetRun
A dotnet run like feature to script your C# code.

## Release Notes
### 0.0.0.3-beta
- Supports multiple .cs files from the same folder
- Pass arguments

## Setup
Install `DotNetRun` as a dotnet tool
```
dotnet tool install -g DotNetRun --prerelease
```

## Usage
- Create a .cs file in any folder. For eg: `C:\Demo\Test.cs`
- Write C# code in this file as shown in the example below:
  ```
  #:package Humanizer@2.*
  #:import *.cs
  
  using Humanizer;
  
  var day = DateTime.Now - new DateTime(2000,1,1);
  
  Console.WriteLine($"Hello world to DNR! Today is {day.Humanize()} of this century.");
  Console.WriteLine();
  Console.Write("I can print multiplication table for you. Give me a number: ");
  string? number = Console.ReadLine();
  if (int.TryParse(number, out int num))
  {
      Console.WriteLine();
      for (int i = 1; i <= 10; i++)
      {
          var n = i.ToString().PadLeft(2, ' ');
          var r = (num * i).ToString().PadLeft((num * 10).ToString().Length, ' ');
          Console.WriteLine($"{num} x {n} = {r}");
      }
  }
  
  Console.WriteLine();
  ```
  `#:import` you can pass a single file like `#:import Example.cs` or all files in the folder with `#:import *.cs`
- You run now run this file as follow
  ```
  dnr .\Test.cs
  ```
- That's it.

