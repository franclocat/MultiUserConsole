using Client.Services;
using Client.Services.Interfaces;
using Shared.DTO;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UI;

internal class Menu
{
    private List<MenuOption> _menuOptions;
    private bool _isClosing = false;
    private IAuthService _authService;

    public Menu(IAuthService authService)
    {
        _menuOptions = new List<MenuOption>()
        {
            new MenuOption { Key = ConsoleKey.E, Description = "Exit", Action = Exit },
            new MenuOption { Key = ConsoleKey.L, Description = "Login", Action = Login },
            new MenuOption { Key = ConsoleKey.R, Description = "Register", Action = Register }
        };
        _authService = authService;
    }

    public void Show()
    {
        while (!_isClosing)
        {
            Console.Clear();
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

        Console.WriteLine("Register");
        Console.WriteLine("Enter Username");
        user.Username = Console.ReadLine();
        Console.WriteLine("Enter Password");
        user.Password = Console.ReadLine();

        ServiceResult result = _authService.Register(user).Result;

        if (result.IsSuccessful)
        {
            Console.WriteLine("Register successful.");
        }
        else
        {
            Console.WriteLine(result.ErrorMessage);
        }
    }

    private void Login()
    {
        UserDTO user = new UserDTO();

        AnsiConsole.Write(new Markup("[blue]Login:[/]"));
        Console.WriteLine();
        Console.WriteLine("Enter Username");
        user.Username = Console.ReadLine();
        Console.WriteLine("Enter Password");
        user.Password = Console.ReadLine();

        ServiceResult result = _authService.Login(user).Result;

        if (result.IsSuccessful)
        {
            AnsiConsole.Write(new Markup("[green]Login successful.[/]"));
        }
        else
        {
            AnsiConsole.Write(new Markup($"[red]{result.ErrorMessage}[/]"));
        }
    }
}
