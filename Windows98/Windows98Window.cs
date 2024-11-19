using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Vanara.PInvoke;

namespace Windows98
{
    public class Windows98Window : Window
    {
        private Button? CloseButton { get; set; }
        private Button? MaximizeButton { get; set; }
        private Button? MinimizeButton { get; set; }
        private Border? TitleBar { get; set; }

        static Windows98Window()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Windows98Window),
                new FrameworkPropertyMetadata(typeof(Windows98Window)));

            AppContext.SetSwitch("Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering", false);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            CloseButton = GetTemplateChild("PART_CloseButton") as Button;
            MaximizeButton = GetTemplateChild("PART_MaximizeButton") as Button;
            MinimizeButton = GetTemplateChild("PART_MinimizeButton") as Button;
            TitleBar = GetTemplateChild("PART_TitleBar") as Border;

            if (CloseButton is not null) CloseButton.Click += CloseButton_Click;
            if (MaximizeButton is not null) MaximizeButton.Click += MaximizeButton_Click;
            if (MinimizeButton is not null) MinimizeButton.Click += MinimizeButton_Click;
            //if (TitleBar is not null) TitleBar.MouseDown += TitleBar_MouseDown;

            WindowInteropHelper helper = new(this);
            DwmApi.DwmSetWindowAttribute(
                helper.EnsureHandle(),
                DwmApi.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                DwmApi.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND);
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
