using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utility.WPF.Colours;

namespace Utility.WPF.Demo.Colours
{
    /// <summary>
    /// Interaction logic for RandomRectangles.xaml
    /// </summary>
    public partial class RandomRectanglesView : UserControl
    {
        private Random rand = new Random();
        private ulong count = 0;
        private WriteableBitmap bitmap;
        private FieldInfo[] fields;

        public RandomRectanglesView()
        {
            InitializeComponent();
            CompositionTarget.Rendering += OnRendering;
            this.SizeChanged += MainView_SizeChanged;
            fields = typeof(NiceNativeColours.Light).GetFields();
        }

        private void MainView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            bitmap = BitmapFactory.New((int)e.NewSize.Width, (int)e.NewSize.Height);
            img.Source = bitmap;
        }

        private void OnRendering(object sender, EventArgs args)
        {
            //Button_Click();
            sdf();
        }

        private void sdf()
        {
            if (bitmap == null)
                return;

            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;

            int x1 = rand.Next(width);
            int x2 = rand.Next(width);
            int y1 = rand.Next(height);
            int y2 = rand.Next(height);
            var clr = (Color)fields[rand.Next(1, fields.Length - 1)].GetValue(default);

            using (bitmap.GetBitmapContext())
            {
                DrawRectangleFilled(x1, x2, y1, y2, clr);
            }
        }

        private void DrawRectangleFilled(int x1, int y1, int x2, int y2, Color clr)
        {
            if (x2 > x1)
            {
                for (int i = x1; i < x2 - x1; i++)
                {
                    bitmap.DrawLine(i, y1, i, y2, clr);
                }
            }
            else
            {
                for (int i = x2; i < x1 - x2; i++)
                {
                    bitmap.DrawLine(i, y1, i, y2, clr);
                }
            }
        }
    }
}