using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebPacketSimulator.Common;

namespace WebPacketSimulator.Wpf
{
    /// <summary>
    /// Interaction logic for PacketConsoleUserControl.xaml
    /// </summary>
    public partial class PacketConsole : UserControl
    {
        public static PacketConsole Instance;

        public PacketConsole()
        {
            Instance = this;
            InitializeComponent();
        }

        /// <summary>
        /// This function updates the packet console
        /// </summary>
        /// <param name="destinationRouter"> Router to which the packet is going to </param>
        /// <param name="sourceRouter"> Router from which the packet is going to </param>
        /// <param name="firstAnimation"> If true, new linw will be appended before new text </param>
        public static void UpdatePacketConsole(Router sourceRouter, Router destinationRouter, bool firstAnimation)
        {
            var textBlock = Instance.PacketConsoleTextBlock;
            var scroll = Instance.PacketConsoleScrollViewer;
            bool automaticScroll = Math.Abs(scroll.ActualHeight + scroll.VerticalOffset - scroll.ExtentHeight) < 1;

            //Making and appending the message
            StringBuilder textToAppend = new StringBuilder(128);
            if (string.IsNullOrEmpty(textBlock.Text) == false)
            {
                textToAppend.AppendLine();
                if (firstAnimation)
                {
                    textToAppend.AppendLine();
                }
            }
            textToAppend.Append("Packet sent");
            for (int i = 0; i < 2; i++)
            {
                textToAppend.Append((i == 0) ? " from " : " to ");
                var currentRouter = (i == 0) ? sourceRouter : destinationRouter;
                var emptyAddress = string.IsNullOrEmpty(currentRouter.Address);
                var emptyName = string.IsNullOrEmpty(currentRouter.Name);
                if (emptyAddress && emptyName)
                {
                    textToAppend.Append("[unknown]");
                }
                else if (emptyAddress && !emptyName)
                {
                    textToAppend.Append(currentRouter.Name);
                }
                else if (!emptyAddress && emptyName)
                {
                    textToAppend.Append(currentRouter.Address);
                }
                else
                {
                    textToAppend.Append(string.Format("{0} ({1})", currentRouter.Address, currentRouter.Name));
                }
            }
            textBlock.Text += textToAppend.ToString();
            if (automaticScroll)
            {
                Instance.PacketConsoleScrollViewer.ScrollToEnd();
            }

            //Removing lines from the console if there are too many lines
            int count = textBlock.Text.Count(c => c == '\n');
            while (count > 50)
            {
                textBlock.Text = textBlock.Text.Remove(0, textBlock.Text.IndexOf('\n') + 1);
                count--;
            }
        }

        /// <summary>
        /// This function send a message animation termination message to the console
        /// </summary>
        public static void UpdatePacketConsole()
        {
            Instance.PacketConsoleTextBlock.Text += "\nMessage animation canceled!\n";
        }

        /// <summary>
        /// This function updates the console to show data summary after packet animation has ended
        /// </summary>
        /// <param name="path"> Packet's path </param>
        public static void UpdatePacketConsole(List<Router> path)
        {
            StringBuilder text = new StringBuilder(128);
            text.AppendLine();
            text.Append(string.Format("Path length: {0}", path.Count));
            Instance.PacketConsoleTextBlock.Text += text.ToString();
        }
    }
}