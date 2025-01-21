using Client.Services;
using Client.Services.Interfaces;
using Shared.DTO;
using Spectre.Console;

namespace Client.UI;

internal class Menu
{
    private List<MenuOption> _menuOptions;
    private bool _isClosing = false;
    private IAuthService _authService;
    private IProductService _productService;
    private UserDTO? _currentUser = null;

    public Menu(IAuthService authService, IProductService productService)
    {
        _menuOptions = new List<MenuOption>()
        {
            new MenuOption { Key = ConsoleKey.E, Description = "Exit", Action = Exit },
            new MenuOption { Key = ConsoleKey.L, Description = "Login", Action = Login },
            new MenuOption { Key = ConsoleKey.R, Description = "Register", Action = Register },
            new MenuOption { Key = ConsoleKey.C, Description = "Check Authorization", Action = CheckAuthorization },
            new MenuOption { Key = ConsoleKey.A, Description = "Add Product", Action = AddProduct },
            new MenuOption { Key = ConsoleKey.U, Description = "Edit Product", Action = EditProduct },
            new MenuOption { Key = ConsoleKey.U, Description = "Show Products", Action = ShowProducts },
            new MenuOption { Key = ConsoleKey.O, Description = "Logout", Action = Logout },
        };
        _authService = authService;
        _productService = productService;
    }

    //public void Show()
    //{
    //    while (!_isClosing)
    //    {
    //        Console.Clear();

    //        //display username
    //        if (_currentUser != null)
    //        {
    //            Console.WriteLine($"Logged in: {_currentUser.Username}");
    //            Console.WriteLine($"Token expiry date: {_currentUser.TokenDto.ExpiryDate}");
    //        }
    //        else
    //        {
    //            Console.WriteLine("Log in to display username.");
    //        }

    //        //display options
    //        _menuOptions.ForEach(option => Console.WriteLine(option.DisplayInfo()));
    //        ConsoleKey key = Console.ReadKey(true).Key;
    //        MenuOption? selectedOption = _menuOptions.FirstOrDefault(option => option.Key == key);

    //        if (selectedOption != null)
    //        {
    //            Console.WriteLine();
    //            selectedOption.Action.Invoke();
    //            Console.ReadKey();
    //        }
    //    }
    //}

    public void Show()
    {
        while (!_isClosing)
        {
            Console.Clear();

            //display username
            if (_currentUser != null)
            {
                AnsiConsole.Write(new Rows(
                    new Markup($"[blue]Logged in:[/] {_currentUser.Username}"),
                    new Markup($"[blue]Token expiry date:[/] {_currentUser.TokenDto.ExpiryDate}")
                    ));
            }
            else
            {
                AnsiConsole.Write(new Rows(
                   new Markup($"[blue]Log in to display username.[/]")));
            }

            MenuOption selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<MenuOption>()
                    .Title("[gray]Select an option from the menu[/]")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                    .AddChoices(_menuOptions)
                    );

            if (selectedOption != null)
            {
                Console.WriteLine();
                selectedOption.Action.Invoke();
                Console.ReadKey();
            }
        }
    }



    private void Exit()
    {
        _isClosing = true;
        AnsiConsole.Write(new Markup("[red]Good Bye![/]"));
        Console.WriteLine();
    }

    private void Register()
    {
        UserDTO user = new UserDTO();

        AnsiConsole.Write(new Markup("[blue]Register[/]"));
        Console.WriteLine();
        AnsiConsole.Write(new Markup("[gray]Enter Username: [/]"));
        user.Username = Console.ReadLine();
        AnsiConsole.Write(new Markup("[gray]Enter Password: [/]"));
        user.Password = Console.ReadLine();
        Console.WriteLine();

        GenericServiceResult<UserDTO?> result = _authService.Register(user).Result;

        if (result.IsSuccessful)
        {
            AnsiConsole.Write(new Markup("[green]Register successful.[/]"));
            _currentUser = result.Value;
            _authService.SetAuthToken(result.Value.TokenDto.Token);
        }
        else
        {
            AnsiConsole.Write(new Markup($"[red]{result.ErrorMessage}[/]"));
        }
    }

    private void Login()
    {
        UserDTO user = new UserDTO();

        AnsiConsole.Write(new Markup("[blue]Login:[/]"));
        Console.WriteLine();
        //user.Username = Console.ReadLine();
        user.Username = AnsiConsole.Prompt(new TextPrompt<string>("[gray]Enter Username: [/]"));
        //user.Password = Console.ReadLine();
        user.Password = AnsiConsole.Prompt(new TextPrompt<string>("[gray]Enter Password: [/]").Secret());
        Console.WriteLine();

        GenericServiceResult<UserDTO?> result = _authService.Login(user).Result;

        if (result.IsSuccessful)
        {
            Console.WriteLine();
            AnsiConsole.Write(new Markup("[green]Login successful.[/]"));
            _currentUser = result.Value;
            _authService.SetAuthToken(result.Value.TokenDto.Token);
            _productService.SetAuthToken(result.Value.TokenDto.Token);
        }
        else
        {
            AnsiConsole.Write(new Markup($"[red]{result.ErrorMessage}[/]"));
        }
    }

    private void CheckAuthorization()
    {
        if (_currentUser == null)
        {
            AnsiConsole.Write(new Markup($"[red]Not logged in![/]"));
            return;
        }

        ServiceResult result = _authService.CheckAuthorization().Result;
        if (result.IsSuccessful)
        {
            AnsiConsole.Write(new Markup("[green]Auth successfully checked.[/]"));
        }
        else
        {
            AnsiConsole.Write(new Markup($"[red]{result.ErrorMessage}[/]"));
        }
    }

    private void Logout()
    {
        if (_currentUser == null) 
        {
            AnsiConsole.Write(new Markup($"[red]You are not logged in.[/]"));
            return;
        }
        _currentUser = null;
        _authService.SetAuthToken(string.Empty);

        ServiceResult result = _authService.CheckAuthorization().Result;
        if (!result.IsSuccessful)
        {
            AnsiConsole.Write(new Markup($"[red]Logged out.[/]"));
        }
        else
        {
            AnsiConsole.Write(new Markup($"[red]{result.ErrorMessage}[/]"));
        }
    }

    private void AddProduct()
    {
        ProductDTO product = new ProductDTO();
        product.Title = AnsiConsole.Prompt(new TextPrompt<string>("[gray]Enter Product Title: [/]"));
        product.Description = AnsiConsole.Prompt(new TextPrompt<string>("[gray]Enter Product Description: [/]"));

        ServiceResult result = _productService.Add(product).Result;
        if (result.IsSuccessful)
        {
            AnsiConsole.Write(new Markup($"[green]Product added.[/]"));
        }
        else
        {
            AnsiConsole.Write(new Markup($"[red]{result.ErrorMessage}[/]"));
        }
    }

    private void EditProduct()
    {
        GenericServiceResult<IEnumerable<ProductDTO>> resultGetAll = _productService.GetAll().Result;
        if (resultGetAll.IsSuccessful && resultGetAll.Value != null)
        {
            ProductDTO selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<ProductDTO>()
                    .Title("[gray]Select an option from the menu[/]")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                    .AddChoices(resultGetAll.Value)
                    );

            AnsiConsole.Write(new Markup($"[gray]{selectedOption}[/]"));
            Console.WriteLine();

            selectedOption.Title = AnsiConsole.Prompt(new TextPrompt<string>("[gray]Enter Product Title: [/]"));
            selectedOption.Description = AnsiConsole.Prompt(new TextPrompt<string>("[gray]Enter Product Description: [/]"));

            GenericServiceResult<ProductDTO> resultUpdate = _productService.Update(selectedOption).Result;
            if (resultUpdate.IsSuccessful)
            {
                AnsiConsole.Write(new Markup($"[green]Product successfully updated.[/]"));
            }
            else
            {
                AnsiConsole.Write(new Markup($"[red]{resultUpdate.ErrorMessage}[/]"));
            }
        } else
        {
            AnsiConsole.Write(new Markup($"[red]{resultGetAll.ErrorMessage}[/]"));
        }
    }

    private async void ShowProducts()
    {
        GenericServiceResult<IEnumerable<ProductDTO>> result = _productService.GetAll().Result;
        if (result.IsSuccessful && result.Value != null)
        {
            var table = new Table();
            table.AddColumn("Id");
            table.AddColumn("Title");
            table.AddColumn("Description");

            foreach (ProductDTO product in result.Value)
            {
                table.AddRow($"{product.Id}", $"{product.Title}", $"{product.Description}");
            }

            AnsiConsole.Write(table);
        }
        else
        {
            AnsiConsole.Write(new Markup($"[red]{result.ErrorMessage}[/]"));
        }
    }
}
