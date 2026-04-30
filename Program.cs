using NLog;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
    var queryC = db.Categories.OrderBy(c => c.CategoryId);
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
        var selectedCategory = db.Categories.FirstOrDefault(c => c.CategoryId == userSelection);
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
      Console.WriteLine("\t3) Discontinued Products");
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
      //discontinued products
      else if (choice == "3")
      {
        Console.Clear();
        var queryP = db.Products.Where(p => p.Discontinued == true).OrderBy(p => p.ProductId);

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

    Console.WriteLine("Would you like to...");
    Console.WriteLine("\t1) Add a new Category");
    Console.WriteLine("\t2) Edit an existing Category");
    Console.Write("?: ");
    choice = Console.ReadLine();

    var configuration = new ConfigurationBuilder()
          .AddJsonFile($"appsettings.json");
    var config = configuration.Build();
    var db = new DataContext();

    if (choice == "1")
    {
      Console.Clear();
      Console.WriteLine("Enter your name for a new Category.\nYour name cannot be longer than 15 letters!");
      var name = "";
      while (true)
      {
        Console.Write(": ");
        name = Console.ReadLine();

        if (name.Length > 15)
        {
          Console.WriteLine("Make sure that it is less than 15 letters!");
        }
        else
        {
          break;
        }
      }

      Console.WriteLine("Enter a new description for your Category");
      Console.Write(": ");
      var description = Console.ReadLine();

      Category category = new Category { CategoryName = name, Description = description };

      if (!db.Categories.Any(c => c.CategoryName == category.CategoryName))
      {
        db.Categories.Add(category);
        db.SaveChanges();
      }


    }
    else if (choice == "2")
    {
      var queryC = db.Categories.OrderBy(c => c.CategoryId);
      List<int> availableIds = new List<int>();

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine($"{queryC.Count()} records returned");

      Console.ForegroundColor = ConsoleColor.Magenta;
      foreach (var item in queryC)
      {
        Console.WriteLine($"\t{item.CategoryId}) - {item.CategoryName}");
        availableIds.Add(item.CategoryId);
      }

      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine("Which would you like to edit?");
      Console.Write("?: ");
      var userTestSelection = Console.ReadLine();

      //checks to see if the user input was an int, then stores the int for later use
      if (int.TryParse(userTestSelection, out int userSelection))
      {
        //checks to see if the user input is in the list of available IDs
        if (availableIds.Contains(userSelection))
        {
          var name = "";
          var description = "";

          Console.Clear();
          //gets the first/default category from the user selected ID. CategoryId is the primary key, so only one should be returned
          var selectedCategory = db.Categories.FirstOrDefault(c => c.CategoryId == userSelection);
          Console.WriteLine($"{selectedCategory.CategoryName} - This is the current name, would you like to change it?\n\t(y/n)");
          Console.Write("?: ");
          if (Console.ReadLine().ToLower() == "y")
          {
            Console.WriteLine("Enter your new name for the Category.\nYour name cannot be longer than 15 letters!");
            while (true)
            {
              Console.Write(": ");
              name = Console.ReadLine();

              if (name.Length > 15)
              {
                Console.WriteLine("Make sure that it is less than 15 letters!");
              }
              else
              {
                break;
              }
            }
          }

          Console.WriteLine($"{selectedCategory.Description}\nThis is the current description, would you like to change it?\n\t(y/n)");
          Console.Write("?: ");
          if (Console.ReadLine().ToLower() == "y")
          {
            Console.WriteLine("Enter a new description for your Category");
            Console.Write(": ");
            description = Console.ReadLine();
          }

          if (name != "")
          {
            selectedCategory.CategoryName = name;
          }


          if (description != "")
          {
            selectedCategory.Description = description;
          }

          db.SaveChanges();
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
    else
    {
      //error handling
      logger.Error("User entered invalid selection.");
      Console.WriteLine("Invalid Selection.\nPress Enter to return to the main menu.");
      Console.ReadLine();
    }
  }
  // Edit / Append to a Product
  else if (choice == "4")
  {
    Console.Clear();

    Console.WriteLine("Would you like to...");
    Console.WriteLine("\t1) Add a new Product");
    Console.WriteLine("\t2) Edit an existing Product");
    Console.Write("?: ");
    choice = Console.ReadLine();

    var configuration = new ConfigurationBuilder()
          .AddJsonFile($"appsettings.json");
    var config = configuration.Build();
    var db = new DataContext();

    if (choice == "1")
    {
      string productName;
      int? supplierId = null;
      int? categoryId = null;
      string? quantityPerUnit;
      decimal? unitPrice;
      short? unitsInStock;
      short? unitsOnOrder;
      short? reorderLevel;
      bool discontinued;

      //set name
      Console.WriteLine("What would you like your new product's name to be?");
      Console.Write("?: ");
      productName = Console.ReadLine();

      //set supplier
      {
        var queryS = db.Suppliers.OrderBy(s => s.SupplierId);
        List<int> availableIds = new List<int>();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{queryS.Count()} records returned");

        Console.ForegroundColor = ConsoleColor.Magenta;
        foreach (var item in queryS)
        {
          Console.WriteLine($"\t{item.SupplierId}) - {item.CompanyName}");
          availableIds.Add(item.SupplierId);
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Who is the supplier?\nIf they are not listed, please type NL.");
        Console.Write("?: ");
        var userTestSelection = Console.ReadLine();

        //checks to see if the user input was an int, then stores the int for later use
        if (int.TryParse(userTestSelection, out int userSelection))
        {
          //checks to see if the user input is in the list of available IDs
          if (availableIds.Contains(userSelection))
          {
            supplierId = userSelection;
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
          if (userTestSelection.ToLower() == "nl")
          {
            //log that the user chose not have any supplier chosen
          }
          else
          {
            //error handling
            logger.Error("User entered invalid selection.");
            Console.WriteLine("Invalid Selection.\nPress Enter to return to the main menu.");
            Console.ReadLine();
          }
        }
      }
      //sets categories
      {
        var queryC = db.Categories.OrderBy(s => s.CategoryId);
        List<int> availableIds = new List<int>();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{queryC.Count()} records returned");

        Console.ForegroundColor = ConsoleColor.Magenta;
        foreach (var item in queryC)
        {
          Console.WriteLine($"\t{item.CategoryId}) - {item.CategoryName}");
          availableIds.Add(item.CategoryId);
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("What is the category?\nIf it is not listed, please type NL.");
        Console.Write("?: ");
        var userTestSelection = Console.ReadLine();

        //checks to see if the user input was an int, then stores the int for later use
        if (int.TryParse(userTestSelection, out int userSelection))
        {
          //checks to see if the user input is in the list of available IDs
          if (availableIds.Contains(userSelection))
          {
            var selectedCategory = db.Categories.FirstOrDefault(c => c.CategoryId == userSelection);
            categoryId = selectedCategory.CategoryId;
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
          if (userTestSelection.ToLower() == "nl")
          {
            //log that the user chose not have any category chosen
          }
          else
          {
            //error handling
            logger.Error("User entered invalid selection.");
            Console.WriteLine("Invalid Selection.\nPress Enter to return to the main menu.");
            Console.ReadLine();
          }
        }
      }
      //sets quantity per unit
      Console.WriteLine($"What is {productName}'s quantity per unit?");
      Console.Write("?: ");
      quantityPerUnit = Console.ReadLine();

      Console.WriteLine($"What is {productName}'s unit price?");
      Console.Write("?: ");
      if (decimal.TryParse(Console.ReadLine(), out decimal decimalHolder))
      {
        unitPrice = decimalHolder;
      }
      else
      {
        unitPrice = null;
      }

      Console.WriteLine($"How many {productName}s are in stock?");
      Console.Write("?: ");
      if (short.TryParse(Console.ReadLine(), out short shortHolder))
      {
        unitsInStock = shortHolder;
      }
      else
      {
        unitsInStock = null;
      }

      Console.WriteLine($"How many {productName}s are on order?");
      Console.Write("?: ");
      if (short.TryParse(Console.ReadLine(), out shortHolder))
      {
        unitsOnOrder = shortHolder;
      }
      else
      {
        unitsOnOrder = null;
      }

      Console.WriteLine($"What is the reorder level of {productName}?");
      Console.Write("?: ");
      if (short.TryParse(Console.ReadLine(), out shortHolder))
      {
        reorderLevel = shortHolder;
      }
      else
      {
        reorderLevel = null;
      }

      Console.WriteLine($"Is {productName} discontinued?\n\t(y/n)");
      while (true)
      {
        Console.Write("?: ");
        var holder = Console.ReadLine();
        if (holder.ToLower() == "y")
        {
          discontinued = true;
          break;
        }
        else if (holder.ToLower() == "n")
        {
          discontinued = false;
          break;
        }
        else
        {
          Console.WriteLine("Please use (y/n)");
        }
      }

      Product product = new Product { ProductName = productName, SupplierId = supplierId, CategoryId = categoryId, QuantityPerUnit = quantityPerUnit, UnitPrice = unitPrice, UnitsInStock = unitsInStock, UnitsOnOrder = unitsOnOrder, ReorderLevel = reorderLevel, Discontinued = discontinued };

      if (!db.Products.Any(c => c.ProductName == product.ProductName))
      {
        db.Products.Add(product);
        db.SaveChanges();
      }
    }
    else if (choice == "2")
    {
      Product productEdited = new Product();

      var queryP = db.Products.OrderBy(p => p.ProductId);
      List<int> availableIds = new List<int>();

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine($"{queryP.Count()} records returned");

      Console.ForegroundColor = ConsoleColor.Magenta;
      foreach (var item in queryP)
      {
        Console.WriteLine($"\t{item.ProductId}) - {item.ProductName}");
        availableIds.Add(item.ProductId);
      }

      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine("Which would you like to edit?");
      Console.Write("?: ");
      var userTestSelection = Console.ReadLine();

      //checks to see if the user input was an int, then stores the int for later use
      if (int.TryParse(userTestSelection, out int userSelection))
      {
        //checks to see if the user input is in the list of available IDs
        if (availableIds.Contains(userSelection))
        {
          productEdited = db.Products.FirstOrDefault(p => p.ProductId == userSelection);
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

      if (productEdited.ProductId != null)
      {
        //edit name
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s name?\n\t(y/n)");
        Console.Write("?: ");
        if (Console.ReadLine().ToLower() == "y")
        {
          Console.WriteLine("What would you like your new product's name to be?");
          Console.Write("?: ");
          productEdited.ProductName = Console.ReadLine();
        }

        //set supplier
        var supplierName = db.Suppliers.Where(s=> s.SupplierId == productEdited.SupplierId).Select(s=> s.CompanyName).FirstOrDefault();
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s supplier?\n{supplierName}\n\t(y/n)");
        Console.Write("?: ");
        if (Console.ReadLine().ToLower() == "y")
        {
          {
            var queryS = db.Suppliers.OrderBy(s => s.SupplierId);
            availableIds = new List<int>();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{queryS.Count()} records returned");

            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in queryS)
            {
              Console.WriteLine($"\t{item.SupplierId}) - {item.CompanyName}");
              availableIds.Add(item.SupplierId);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Who is the supplier?\nIf they are not listed, please type NL.");
            Console.Write("?: ");
            userTestSelection = Console.ReadLine();

            //checks to see if the user input was an int, then stores the int for later use
            if (int.TryParse(userTestSelection, out userSelection))
            {
              //checks to see if the user input is in the list of available IDs
              if (availableIds.Contains(userSelection))
              {
                productEdited.SupplierId = userSelection;
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
              if (userTestSelection.ToLower() == "nl")
              {
                //log that the user chose not have any supplier chosen
              }
              else
              {
                //error handling
                logger.Error("User entered invalid selection.");
                Console.WriteLine("Invalid Selection.\nPress Enter to return to the main menu.");
                Console.ReadLine();
              }
            }
          }
        }
        var categoryName = db.Categories.Where(c => c.CategoryId == productEdited.CategoryId).Select(c => c.CategoryName).FirstOrDefault();
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s category?\n{categoryName}\n\t(y/n)");
        Console.Write("?: ");
        if (Console.ReadLine().ToLower() == "y")
        {
          //sets categories
          {
            var queryC = db.Categories.OrderBy(s => s.CategoryId);
            availableIds = new List<int>();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{queryC.Count()} records returned");

            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in queryC)
            {
              Console.WriteLine($"\t{item.CategoryId}) - {item.CategoryName}");
              availableIds.Add(item.CategoryId);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("What is the category?\nIf it is not listed, please type NL.");
            Console.Write("?: ");
            userTestSelection = Console.ReadLine();

            //checks to see if the user input was an int, then stores the int for later use
            if (int.TryParse(userTestSelection, out userSelection))
            {
              //checks to see if the user input is in the list of available IDs
              if (availableIds.Contains(userSelection))
              {
                var selectedCategory = db.Categories.FirstOrDefault(c => c.CategoryId == userSelection);
                productEdited.CategoryId = selectedCategory.CategoryId;
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
              if (userTestSelection.ToLower() == "nl")
              {
                //log that the user chose not have any category chosen
              }
              else
              {
                //error handling
                logger.Error("User entered invalid selection.");
                Console.WriteLine("Invalid Selection.\nPress Enter to return to the main menu.");
                Console.ReadLine();
              }
            }
          }
        }
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s quantity per unit?\n{productEdited.QuantityPerUnit}\n\t(y/n)");
        Console.Write("?: ");
        if (Console.ReadLine().ToLower() == "y")
        {
          //sets quantity per unit
          Console.WriteLine($"What is {productEdited.ProductName}'s quantity per unit?");
          Console.Write("?: ");
          productEdited.QuantityPerUnit = Console.ReadLine();
        }

        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s unit price?\n{productEdited.UnitPrice}\n\t(y/n)");
        Console.Write("?: ");
        if (Console.ReadLine().ToLower() == "y")
        {
          Console.WriteLine($"What is {productEdited.ProductName}'s unit price?");
          Console.Write("?: ");
          if (decimal.TryParse(Console.ReadLine(), out decimal decimalHolder))
          {
            productEdited.UnitPrice = decimalHolder;
          }
          else
          {
            productEdited.UnitPrice = null;
          }
        }

        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s units in stock?\n{productEdited.UnitsInStock}\n\t(y/n)");
        Console.Write("?: ");
        if (Console.ReadLine().ToLower() == "y")
        {
          Console.WriteLine($"How many {productEdited.ProductName}s are in stock?");
          Console.Write("?: ");
          if (short.TryParse(Console.ReadLine(), out short shortHolder))
          {
            productEdited.UnitsInStock = shortHolder;
          }
          else
          {
            productEdited.UnitsInStock = null;
          }
        }

        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s units on order?\n{productEdited.UnitsOnOrder}\n\t(y/n)");
        Console.Write("?: ");
        if (Console.ReadLine().ToLower() == "y")
        {
          Console.WriteLine($"How many {productEdited.ProductName}s are on order?");
          Console.Write("?: ");
          if (short.TryParse(Console.ReadLine(), out short shortHolder))
          {
            productEdited.UnitsOnOrder = shortHolder;
          }
          else
          {
            productEdited.UnitsOnOrder = null;
          }
        }

        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s reorder level?\n{productEdited.ReorderLevel}\n\t(y/n)");
        Console.Write("?: ");
        if (Console.ReadLine().ToLower() == "y")
        {
          Console.WriteLine($"What is the reorder level of {productEdited.ProductName}?");
          Console.Write("?: ");
          if (short.TryParse(Console.ReadLine(), out short shortHolder))
          {
            productEdited.ReorderLevel = shortHolder;
          }
          else
          {
            productEdited.ReorderLevel = null;
          }
        }

        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s disconinued status?\n{productEdited.Discontinued}\n\t(y/n)");
        Console.Write("?: ");
        if (Console.ReadLine().ToLower() == "y")
        {
          Console.WriteLine($"Is {productEdited.ProductName} discontinued?\n\t(y/n)");
          while (true)
          {
            Console.Write("?: ");
            var holder = Console.ReadLine();
            if (holder.ToLower() == "y")
            {
              productEdited.Discontinued = true;
              break;
            }
            else if (holder.ToLower() == "n")
            {
              productEdited.Discontinued = false;
              break;
            }
            else
            {
              Console.WriteLine("Please use (y/n)");
            }
          }
        }

        db.SaveChanges();
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
