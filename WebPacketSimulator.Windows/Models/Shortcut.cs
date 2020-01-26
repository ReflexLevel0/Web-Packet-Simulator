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
                ShortcutDescription = "Open a file" 
            },
            new Shortcut()
            { 
                ShortcutKeyCombination = "Ctrl + S", 
                ShortcutDescription = "Save the current file" 
            },
            new Shortcut()
            {
                ShortcutKeyCombination = "Ctrl + H",
                ShortcutDescription = "Open up help window"
            },
            new Shortcut()
            {
                ShortcutKeyCombination = "Ctrl + ->", 
                ShortcutDescription = "Make simulation faster"
            },
            new Shortcut()
            {
                ShortcutKeyCombination = "Ctrl + <-", 
                ShortcutDescription = "Make simulation slower"
            },
            new Shortcut()
            { 
                ShortcutKeyCombination = "Esc", 
                ShortcutDescription = "Unhighlight all selected objects" 
            },
            new Shortcut()
            {
                ShortcutKeyCombination = "Del",
                ShortcutDescription = "Delete all highlighted objects"
            },
            new Shortcut()
            {
                ShortcutKeyCombination = "Ctrl + N",
                ShortcutDescription = "Create a new project"
            }
        }.OrderBy(s => s.ShortcutDescription).ToList();
    }
}