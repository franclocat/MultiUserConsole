using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UI;

internal class MenuOption
{
    public ConsoleKey Key { get; internal set; }
    public string Description { get; internal set; }
    public Action Action { get; internal set; }

    public string DisplayInfo()
    {
        return $"{(char)Key} - {Description}";
    }

    public override string ToString()
    {
        return Description;
    }
}
