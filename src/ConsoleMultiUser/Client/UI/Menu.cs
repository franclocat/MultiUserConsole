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
    private string _authToken = string.Empty;
    private UserDTO? _currentUser = null;

    public Menu(IAuthService authService)
    {
        _menuOptions = new List<MenuOption>()
        {
            new MenuOption { Key = ConsoleKey.E, Description = "Exit", Action = Exit },
            new MenuOption { Key = ConsoleKey.L, Description = "Login", Action = Login },
            new MenuOption { Key = ConsoleKey.R, Description = "Register", Action = Register },
            new MenuOption { Key = ConsoleKey.C, Description = "Check Authorization", Action = CheckAuthorization },
            new MenuOption { Key = ConsoleKey.O, Description = "Logout", Action = Logout },
        };
        _authService = authService;
    }

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
            Console.WriteLine();

            //display options
            _menuOptions.ForEach(option => Console.WriteLine(option.DisplayInfo()));
            ConsoleKey key = Console.ReadKey(true).Key;
            MenuOption? selectedOption = _menuOptions.FirstOrDefault(option => option.Key == key);

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

        GenericServiceResult<UserDTO> result = _authService.Register(user).Result;

        if (result.IsSuccessful)
        {
            AnsiConsole.Write(new Markup("[green]Register successful.[/]"));
            _authToken = result.Value.TokenDto.Token;
            _currentUser = result.Value;
            _authService.SetAuthToken(_authToken);
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
        AnsiConsole.Write(new Markup("[gray]Enter Username: [/]"));
        user.Username = Console.ReadLine();
        AnsiConsole.Write(new Markup("[gray]Enter Password: [/]"));
        user.Password = Console.ReadLine();
        Console.WriteLine();

        GenericServiceResult<UserDTO> result = _authService.Login(user).Result;

        if (result.IsSuccessful)
        {
            Console.WriteLine();
            AnsiConsole.Write(new Markup("[green]Login successful.[/]"));
            _authToken = result.Value.TokenDto.Token;
            _currentUser = result.Value;
            _authService.SetAuthToken(_authToken);
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
        _authToken = string.Empty;
        _currentUser = null;
        _authService.SetAuthToken(_authToken);

        ServiceResult result = _authService.CheckAuthorization().Result;
        if (!result.IsSuccessful)
        {
            AnsiConsole.Write(new Markup($"[blue]Logged out.[/]"));
        }
        else
        {
            AnsiConsole.Write(new Markup($"[red]{result.ErrorMessage}[/]"));
        }
    }
}
