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

    /// <summary>Initializes a new instance of the <see cref="Menu" /> class.</summary>
    public Menu()
    {
        _menuOptions = new List<MenuOption>()
        {
            new MenuOption { Key = ConsoleKey.E, Description = "Exit", Action = Exit }
        };
    }

    /// <summary>Shows this instance.</summary>
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
    }
}
