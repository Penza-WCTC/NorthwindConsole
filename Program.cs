using NLog;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

do
{
  //Menu Display
  Console.Clear();
  {
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Would you like to ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("View ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("records from:");

    Console.WriteLine("\t1) Categories");
    Console.WriteLine("\t2) Products");

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Or would you like to ");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("Edit / Append ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("to:");

    Console.WriteLine("\t3) Categories");
    Console.WriteLine("\t4) Products");

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Or would you like to ");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write("Delete ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("from:");

    Console.WriteLine("\t5) Categories");
    Console.WriteLine("\t6) Products");

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Or would you like to ");
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write("Seach ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("for a specific:");

    Console.WriteLine("\t7) Category");
    Console.WriteLine("\t8) Product");

    Console.Write("?: ");
  }

  string? choice = Console.ReadLine();
  logger.Info("Option {choice} selected", choice);

  // View Specific Category Information
  if (choice == "1")
  {
    Console.Clear();
    // display categories
    var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");
    var config = configuration.Build();

    //pull query for categories
    var db = new DataContext();
    var queryC = db.Categories.OrderBy(c => c.CategoryName);
    List<int> availableIds = new List<int>();

    //display the categories
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{queryC.Count()} records returned");
    Console.ForegroundColor = ConsoleColor.Magenta;
    foreach (var item in queryC)
    {
      Console.WriteLine($"\t{item.CategoryId}) - {item.CategoryName}");
      availableIds.Add(item.CategoryId);
    }

    //allow user to pick what category they want more info on
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Which would you like more information on?");
    Console.Write("?: ");
    string? userTestSelection = Console.ReadLine();

    //checks to see if the user input was an int, then stores the int for later use
    if (int.TryParse(userTestSelection, out int userSelection))
    {
      //checks to see if the user input is in the list of available IDs
      if (availableIds.Contains(userSelection))
      {

        Console.Clear();
        //gets the first/default category from the user selected ID. CategoryId is the primary key, so only one should be returned
        var selectedCategory = queryC.FirstOrDefault(c => c.CategoryId == userSelection);
        //get a query on products that are not discontinued and a part of the current category
        var queryP = db.Products.Where(p => p.CategoryId == userSelection && p.Discontinued == false);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{queryP.Count()} records returned");
        Console.WriteLine($"Products from {selectedCategory.CategoryName}, which stores products catagorized as {selectedCategory.Description}:");

        //displays the information to the user
        Console.ForegroundColor = ConsoleColor.Magenta;
        foreach (var item in queryP)
        {
          Console.WriteLine($"\t{item.ProductId}) {item.ProductName}: ${item.UnitPrice} ");
        }

        //allow user to leave
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Press enter to return to the main menu.");
        Console.ReadLine();
      }
      else
      {
        //error handling
        logger.Error("User entered an ID out of range");
        Console.WriteLine("That is not an available ID to select.\nPress Enter to return to the main menu.");
        Console.ReadLine();
      }
    }
    else
    {
      //error handling
      logger.Error("User entered invalid selection.");
      Console.WriteLine("Invalid Selection.\nPress Enter to return to the main menu.");
      Console.ReadLine();
    }
  }
  // View Specific Product Information
  else if (choice == "2")
  {
    while (true)
    {
      Console.Clear();

      Console.WriteLine("Would you like to view...");
      Console.WriteLine("\t1) All Products");
      Console.WriteLine("\t2) Active Products");
      Console.WriteLine("\t3) Discounted Products");
      Console.Write("\tPress Enter to ");
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("leave");
      Console.ForegroundColor = ConsoleColor.White;
      choice = Console.ReadLine();

      var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");
      var config = configuration.Build();
      var db = new DataContext();

      //all products
      if (choice == "1")
      {
        Console.Clear();
        var queryP = db.Products.OrderBy(p => p.ProductId);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{queryP.Count()} records returned");
        Console.ForegroundColor = ConsoleColor.Magenta;
        foreach (var item in queryP)
        {
          Console.WriteLine($"\t{item.ProductId}) {item.ProductName}, ${item.UnitPrice}");
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Press Enter to filter again");
        Console.ReadLine();
      }
      //active products
      else if (choice == "2")
      {
        Console.Clear();
        var queryP = db.Products.Where(p => p.Discontinued == false).OrderBy(p => p.ProductId);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{queryP.Count()} records returned");
        Console.ForegroundColor = ConsoleColor.Magenta;
        foreach (var item in queryP)
        {
          Console.WriteLine($"\t{item.ProductId}) {item.ProductName}, ${item.UnitPrice}");
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Press Enter to filter again");
        Console.ReadLine();
      }
      //discounted products
      else if (choice == "3")
      {

      }
      else
      {
        Console.Clear();
        Console.WriteLine("You selected an option that was not on the menu.\nWould you like to return to menu?\n\t(y/n)");
        Console.Write("?: ");
        if (Console.ReadLine().ToLower() == "y")
        {
          logger.Info("Exited Product Info");
          break;
        }
      }

    }
  }
  // Edit / Append to a Category
  else if (choice == "3")
  {
    Console.Clear();
  }
  // Edit / Append to a Product
  else if (choice == "4")
  {
    Console.Clear();
  }
  // Delete a Category
  else if (choice == "5")
  {
    Console.Clear();
  }
  // Delete a Product
  else if (choice == "6")
  {
    Console.Clear();
  }
  // Search for a Category
  else if (choice == "7")
  {
    Console.Clear();
  }
  // Search for a Product
  else if (choice == "8")
  {
    Console.Clear();
  }
  else
  {
    Console.Clear();
    Console.WriteLine("You selected an option that was not on the menu.\nWould you like to exit the program?\n\t(y/n)");
    Console.Write("?: ");
    if (Console.ReadLine().ToLower() == "y")
    {
      break;
    }
  }
} while (true);

logger.Info("Program ended");


/*
          if (choice == "1")
          {
            // display categories
            var configuration = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json");

            var config = configuration.Build();

            var db = new DataContext();
            var query = db.Categories.OrderBy(p => p.CategoryName);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
              Console.WriteLine($"{item.CategoryName} - {item.Description}");
            }
            Console.ForegroundColor = ConsoleColor.White;
          }
          else if (choice == "2")
          {
            // Add category
            Category category = new();
            Console.WriteLine("Enter Category Name:");
            category.CategoryName = Console.ReadLine()!;
            Console.WriteLine("Enter the Category Description:");
            category.Description = Console.ReadLine();
            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
              var db = new DataContext();
              // check for unique name
              if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
              {
                // generate validation error
                isValid = false;
                results.Add(new ValidationResult("Name exists", ["CategoryName"]));
              }
              else
              {
                logger.Info("Validation passed");
                // TODO: save category to db
              }
            }
            if (!isValid)
            {
              foreach (var result in results)
              {
                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
              }
            }
          }
          else if (choice == "3")
          {
            var db = new DataContext();
            var query = db.Categories.OrderBy(p => p.CategoryId);

            Console.WriteLine("Select the category whose products you want to display:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var item in query)
            {
              Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            int id = int.Parse(Console.ReadLine()!);
            Console.Clear();
            logger.Info($"CategoryId {id} selected");
            Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id)!;
            Console.WriteLine($"{category.CategoryName} - {category.Description}");
            foreach (Product p in category.Products)
            {
              Console.WriteLine($"\t{p.ProductName}");
            }
          }
          else if (choice == "4")
          {
            var db = new DataContext();
            var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
            foreach (var item in query)
            {
              Console.WriteLine($"{item.CategoryName}");
              foreach (Product p in item.Products)
              {
                Console.WriteLine($"\t{p.ProductName}");
              }
            }
          }
          else if (String.IsNullOrEmpty(choice))
          {
            break;
          }
          Console.WriteLine();
  */
