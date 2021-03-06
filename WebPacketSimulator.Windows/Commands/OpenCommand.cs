﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WebPacketSimulator.Wpf
{
    public class OpenCommand : ICommand
    {
        public static OpenCommand Instance = new OpenCommand();
        public event EventHandler CanExecuteChanged = delegate { };
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            //Saving current work
            switch (VisualHelpers.SaveCurrentWorkQuery())
            {
                case MessageBoxResult.Cancel:
                    return;
                case MessageBoxResult.Yes:
                    SaveCommand.Instance.Execute(null);
                    break;
            }

            //Opening the new file
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = FileHandler.FileDialogFilter;
            dialog.ShowDialog();
            if (string.IsNullOrEmpty(dialog.FileName))
            {
                return;
            }

            FileHandler.LoadFile(dialog.FileName, true);
        }
    }
}
