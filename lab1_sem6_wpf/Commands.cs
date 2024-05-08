using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace lab1_sem6_wpf
{
    public static class Commands
    {
        public static RoutedCommand CheckControlsCommand = new RoutedCommand("Check controls command", typeof(Commands));
    }
}
