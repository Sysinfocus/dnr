# DotNetRun
A dotnet run like feature to script your C# code and SQL (Sqlite, SqlServer and Postgres).

## Release Notes
### 0.0.0.4-beta
- Now you can create and run .sql files directly
- Supports Sqlite, Sql Server and Postgres

### 0.0.0.3-beta
- Supports multiple .cs files from the same folder
- Pass arguments

## Setup
Install `DotNetRun` as a dotnet tool
```
dotnet tool install -g DotNetRun --prerelease
```

## Usage

**For C#**

- Create a .cs file in any folder. For eg: `C:\Demo\Test.cs` or use the **samples** folder where you have 3 files to test.
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
- You can now run this file as follow
  ```
  dnr .\Test.cs
  ```

**For SQL**

- Create a .sql file in any folder. For eg: `C:\Demo\Sqlite.sql` or use the **samples** folder where you have 3 files to test.
- Write SQL code in this file as shown in the example below:
  ```
  -- sqlite <type-your-connection-string>

  SELECT * FROM YOURTABLE;  
  ```
  The first line is what database and connection string you want to use. It can be anyone from "sqlite, sqlserver or postgres" followed by a valid connection string.
- You can have any one set of sql statement at a time.
- You can now run this file as follow
  ```
  dnr .\Sqlite.sql
  ```
- That's it.
