using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPacketSimulator.Wpf
{
    public class Shortcut
    {
        public string ShortcutKeyCombination { get; set; }
        public string ShortcutDescription { get; set; }
        public static List<Shortcut> Shortcuts = new List<Shortcut>()
        {
            new Shortcut()
            { 
                ShortcutKeyCombination = "Ctrl + O", 
                ShortcutDescription = "open a file" 
            },
            new Shortcut()
            { 
                ShortcutKeyCombination = "Ctrl + S", 
                ShortcutDescription = "save the current file" 
            },
            new Shortcut()
            {
                ShortcutKeyCombination = "Ctrl + ->", 
                ShortcutDescription = "make simulation faster"
            },
            new Shortcut()
            {
                ShortcutKeyCombination = "Ctrl + <-", 
                ShortcutDescription = "make simulation slower"
            },
            new Shortcut()
            { 
                ShortcutKeyCombination = "Esc", 
                ShortcutDescription = "unhighlight all selected objects" 
            }
        };
    }
}