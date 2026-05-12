using NLog;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using NLog.LayoutRenderers;
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
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write("Leave ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("the program: ");
    Console.WriteLine("\tEnter a non-menu input.");
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
    string? userTestSelection = GetLegitUserInput();

    int userSelection = UserChoiceWithinList(userTestSelection, availableIds);

    if (userSelection != 0)
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
      Console.WriteLine("\t4) All a Product's Data");
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
      else if (choice == "4")
      {
        //pull query for products
        var queryP = db.Products.OrderBy(p => p.ProductId);
        List<int> availableIds = new List<int>();

        //display the products
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{queryP.Count()} records returned");
        Console.ForegroundColor = ConsoleColor.Magenta;
        foreach (var item in queryP)
        {
          Console.WriteLine($"\t{item.ProductId}) - {item.ProductName}");
          availableIds.Add(item.ProductId);
        }

        //allow user to pick what products they want to see fully
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Which would you like to see all data for?");
        Console.Write("?: ");
        string? userTestSelection = GetLegitUserInput();

        int userSelection = UserChoiceWithinList(userTestSelection, availableIds);

        if (userSelection != 0)
        {
          Console.Clear();
          //gets the first/default Product from the user selected ID. productId is the primary key, so only one should be returned
          var selectedProduct = db.Products.FirstOrDefault(p => p.ProductId == userSelection);

          Console.ForegroundColor = ConsoleColor.White;
          Console.WriteLine($"Name: {selectedProduct.ProductName} | Id: {selectedProduct.ProductId} | Category Id: {selectedProduct.CategoryId} | Supplier Id: {selectedProduct.SupplierId}");
          Console.WriteLine($"Quantity per Unit: {selectedProduct.QuantityPerUnit} | Unit Price: {selectedProduct.UnitPrice}");
          Console.WriteLine($"Units in Stock: {selectedProduct.UnitsInStock} | Units on Order: {selectedProduct.UnitsOnOrder} | Reorder Level: {selectedProduct.ReorderLevel}");
          if (selectedProduct.Discontinued)
          {
            Console.WriteLine($"{selectedProduct.ProductName} is discontinued");
          }
          else
          {
            Console.WriteLine($"{selectedProduct.ProductName} is not discontinued");
          }

          //allow user to read then leave
          Console.ForegroundColor = ConsoleColor.White;
          Console.WriteLine("Press 'Enter' to leave.");
          Console.ReadLine();
        }
      }
      else
      {
        Console.Clear();
        Console.WriteLine("You selected an option that was not on the menu.\nWould you like to return to menu?\n\t(y/n)");
        Console.Write("?: ");
        if (GetLegitUserInput().ToLower() == "y")
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
        name = GetLegitUserInput();

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
      var userTestSelection = GetLegitUserInput();

      int userSelection = UserChoiceWithinList(userTestSelection, availableIds);
      if (userSelection != 0)
      {
        var name = "";
        var description = "";

        Console.Clear();
        //gets the first/default category from the user selected ID. CategoryId is the primary key, so only one should be returned
        var selectedCategory = db.Categories.FirstOrDefault(c => c.CategoryId == userSelection);
        Console.WriteLine($"{selectedCategory.CategoryName} - This is the current name, would you like to change it?\n\t(y/n)");
        Console.Write("?: ");
        if (GetLegitUserInput().ToLower() == "y")
        {
          Console.WriteLine("Enter your new name for the Category.\nYour name cannot be longer than 15 letters!");
          while (true)
          {
            Console.Write(": ");
            name = GetLegitUserInput();

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
        if (GetLegitUserInput().ToLower() == "y")
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
      productName = GetLegitUserInput();

      //set supplier. This is a special situation so we don't use UserChoiceWithinList()
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
        var userTestSelection = GetLegitUserInput();

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
      //set categories. This is a special situation so we don't use UserChoiceWithinList()
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
        var userTestSelection = GetLegitUserInput();

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

      //sets unit price
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

      //sets units in stock
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

      //sets units on order
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

      //sets reorder level
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

      //sets discontinued
      Console.WriteLine($"Is {productName} discontinued?\n\t(y/n)");
      while (true)
      {
        Console.Write("?: ");
        var holder = GetLegitUserInput();
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

      //creates the new product
      Product product = new Product { ProductName = productName, SupplierId = supplierId, CategoryId = categoryId, QuantityPerUnit = quantityPerUnit, UnitPrice = unitPrice, UnitsInStock = unitsInStock, UnitsOnOrder = unitsOnOrder, ReorderLevel = reorderLevel, Discontinued = discontinued };

      //if that product name is not already there, then add it.
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
      var userTestSelection = GetLegitUserInput();

      int userSelection = UserChoiceWithinList(userTestSelection, availableIds);

      if (userSelection != 0)
      {
        productEdited = db.Products.FirstOrDefault(p => p.ProductId == userSelection);
      }


      if (productEdited.ProductId != null)
      {
        //edit name
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s name?\n\t(y/n)");
        Console.Write("?: ");
        if (GetLegitUserInput().ToLower() == "y")
        {
          Console.WriteLine("What would you like your new product's name to be?");
          Console.Write("?: ");
          productEdited.ProductName = GetLegitUserInput();
        }

        //set supplier. Special situation so we don't use UserChoiceWithinList()
        var supplierName = db.Suppliers.Where(s => s.SupplierId == productEdited.SupplierId).Select(s => s.CompanyName).FirstOrDefault();
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s supplier?\n{supplierName}\n\t(y/n)");
        Console.Write("?: ");
        if (GetLegitUserInput().ToLower() == "y")
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
            userTestSelection = GetLegitUserInput();

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

        //edits category. Special situation so we don't use UserChoiceWithinList().
        var categoryName = db.Categories.Where(c => c.CategoryId == productEdited.CategoryId).Select(c => c.CategoryName).FirstOrDefault();
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s category?\n{categoryName}\n\t(y/n)");
        Console.Write("?: ");
        if (GetLegitUserInput().ToLower() == "y")
        {
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
            userTestSelection = GetLegitUserInput();

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

        //edits quantity per unit.
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s quantity per unit?\n{productEdited.QuantityPerUnit}\n\t(y/n)");
        Console.Write("?: ");
        if (GetLegitUserInput().ToLower() == "y")
        {
          //sets quantity per unit
          Console.WriteLine($"What is {productEdited.ProductName}'s quantity per unit?");
          Console.Write("?: ");
          productEdited.QuantityPerUnit = Console.ReadLine();
        }

        //edits unit price.
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s unit price?\n{productEdited.UnitPrice}\n\t(y/n)");
        Console.Write("?: ");
        if (GetLegitUserInput().ToLower() == "y")
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

        //edits units in stock.
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s units in stock?\n{productEdited.UnitsInStock}\n\t(y/n)");
        Console.Write("?: ");
        if (GetLegitUserInput().ToLower() == "y")
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

        //edits units on order
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s units on order?\n{productEdited.UnitsOnOrder}\n\t(y/n)");
        Console.Write("?: ");
        if (GetLegitUserInput().ToLower() == "y")
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

        //edits reorder level
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s reorder level?\n{productEdited.ReorderLevel}\n\t(y/n)");
        Console.Write("?: ");
        if (GetLegitUserInput().ToLower() == "y")
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

        //edits discontinued status
        Console.WriteLine($"Would you like to edit {productEdited.ProductName}'s disconinued status?\n{productEdited.Discontinued}\n\t(y/n)");
        Console.Write("?: ");
        if (GetLegitUserInput().ToLower() == "y")
        {
          Console.WriteLine($"Is {productEdited.ProductName} discontinued?\n\t(y/n)");
          while (true)
          {
            Console.Write("?: ");
            var holder = GetLegitUserInput();
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
  }
  // Delete a Category
  else if (choice == "5")
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
    Console.WriteLine("Which would you like to delete?");
    Console.Write("?: ");
    string? userTestSelection = GetLegitUserInput();

    int userSelection = UserChoiceWithinList(userTestSelection, availableIds);

    //checks to see if the user input was an int, then stores the int for later use
    if (userSelection != 0)
    {
      Console.Clear();
      //gets the first/default category from the user selected ID. CategoryId is the primary key, so only one should be returned
      var selectedCategory = db.Categories.FirstOrDefault(c => c.CategoryId == userSelection);
      //get a query on products that are not discontinued and a part of the current category
      var queryP = db.Products.Where(p => p.CategoryId == userSelection && p.Discontinued == false);

      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"You have selected: {selectedCategory.CategoryName}, which stores {queryP.Count()} products.");

      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine("Would you like to delete this category?\n\t(y/n)");
      if (GetLegitUserInput().ToLower() == "y")
      {
        foreach (var item in queryP)
        {
          item.Category = null;
          item.CategoryId = null;
        }

        db.Categories.Remove(selectedCategory);
        db.SaveChanges();

        Console.WriteLine("This Category has been Deleted.\nAll related Products no longer have a Category attatched.\nPlease press 'Enter' to return to the main menu.");
        Console.ReadLine();
      }
    }
  }
  // Delete a Product
  else if (choice == "6")
  {
    Console.Clear();
    // display Products
    var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");
    var config = configuration.Build();

    //pull query for products
    var db = new DataContext();
    var queryP = db.Products.OrderBy(p => p.ProductId);
    List<int> availableIds = new List<int>();

    //display the products
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{queryP.Count()} records returned");
    Console.ForegroundColor = ConsoleColor.Magenta;
    foreach (var item in queryP)
    {
      Console.WriteLine($"\t{item.ProductId}) - {item.ProductName}");
      availableIds.Add(item.ProductId);
    }

    //allow user to pick what products they want to delete
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Which would you like to delete?");
    Console.Write("?: ");
    string? userTestSelection = GetLegitUserInput();

    int userSelection = UserChoiceWithinList(userTestSelection, availableIds);

    //checks to see if the user input was an int, then stores the int for later use
    if (userSelection != 0)
    {
      Console.Clear();
      //gets the first/default Product from the user selected ID. productId is the primary key, so only one should be returned
      var selectedProduct = db.Products.FirstOrDefault(p => p.ProductId == userSelection);

      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"{selectedProduct.ProductName} has been selected.");

      //allow user to chose
      Console.ForegroundColor = ConsoleColor.White;
      if (selectedProduct.Discontinued == false)
      {
        Console.WriteLine("Would you like to discontinue this product?\n\t(y/n).");
        if (GetLegitUserInput().ToLower() == "y")
        {
          selectedProduct.Discontinued = true;
          db.SaveChanges();
        }
      }
      else
      {
        Console.WriteLine("This product is listed as discontinued.\nPlease press 'Enter' to return to the main menu.");
        Console.ReadLine();
      }

    }
  }
  else
  {
    Console.Clear();
    Console.WriteLine("You selected an option that was not on the menu.\nWould you like to exit the program?\n\t(y/n)");
    Console.Write("?: ");
    if (GetLegitUserInput().ToLower() == "y")
    {
      break;
    }
  }
} while (true);

logger.Info("Program ended");

static int UserChoiceWithinList(string userTestSelection, List<int> availableIds)
{
  //checks to see if the user input was an int, then stores the int for later use
  if (int.TryParse(userTestSelection, out int userSelection))
  {
    //checks to see if the user input is in the list of available IDs
    if (availableIds.Contains(userSelection))
    {
      return userSelection;
    }
    else
    {
      Console.WriteLine("That is not an available ID to select.\nPress Enter to return to the main menu.");
      Console.ReadLine();
      return 0;
    }
  }
  else
  {
    Console.WriteLine("That is not an ID. Please enter an ID.\nPress Enter to return to the main menu.");
    Console.ReadLine();
    return 0;
  }
}

static String GetLegitUserInput()
{
  String? userInput;
  while (true)
  {
    userInput = Console.ReadLine();

    if(userInput != null && userInput != "")
    {
      return userInput;
    }
    else
    {
      Console.WriteLine("Please enter a value.\n?: ");
    }
  }
}
