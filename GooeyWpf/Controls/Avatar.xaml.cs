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

namespace GooeyWpf.Controls
{
    /// <summary>
    /// Interaction logic for Avatar.xaml
    /// </summary>
    public partial class Avatar : UserControl
    {
        public Avatar()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FaceImageProperty = DependencyProperty.Register(
            "FaceImage", typeof(ImageSource), typeof(Avatar),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public ImageSource FaceImage
        {
            get => (ImageSource)GetValue(FaceImageProperty);
            set => SetValue(FaceImageProperty, value);
        }

        public static readonly DependencyProperty EyeImageProperty = DependencyProperty.Register(
            "EyeImage", typeof(ImageSource), typeof(Avatar),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public ImageSource EyeImage
        {
            get => (ImageSource)GetValue(EyeImageProperty);
            set => SetValue(EyeImageProperty, value);
        }

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
            "Stretch", typeof(Stretch), typeof(Avatar),
            new FrameworkPropertyMetadata(Stretch.Fill, FrameworkPropertyMetadataOptions.AffectsRender));

        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        public static readonly DependencyProperty EyeImageVisibilityProperty = DependencyProperty.Register(
            "EyeImageVisibility", typeof(Visibility), typeof(Avatar),
            new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender));

        public Visibility EyeImageVisibility
        {
            get => (Visibility)GetValue(EyeImageVisibilityProperty);
            set => SetValue(EyeImageVisibilityProperty, value);
        }
    }
}
