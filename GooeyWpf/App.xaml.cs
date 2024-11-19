using System.Windows;

namespace GooeyWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Random Random { get; } = new((int)DateTime.Now.Ticks);
    }
}