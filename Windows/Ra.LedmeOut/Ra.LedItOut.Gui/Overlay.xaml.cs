using LedItOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ra.LedItOut.Gui
{
    /// <summary>
    /// Interaktionslogik für Overlay.xaml
    /// </summary>
    public partial class Overlay : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GwlExstyle = -20;
        private const int WsExTransparent = 0x20;

        public Overlay()
        {


            InitializeComponent();
            Loaded += Overlay_Loaded;



        }

        private void Overlay_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            int exstyle = GetWindowLong(handle, GwlExstyle);
            exstyle |= WsExTransparent;
            SetWindowLong(handle, GwlExstyle, exstyle);
            ReDraw();
        }

        private void ReDraw()
        {
            //AdjustableArrowCap bigArrow = new AdjustableArrowCap(7, 7);

            //Pen penArrow = new Pen(Brushes.Black, 7);
            ////penArrow.Alignment = PenAlignment.Inset;
            ////penArrow.DashStyle.
            //penArrow. = PenLineCap.
            //Font font = new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold);


            try
            {
                this.drawFrame.Children.Clear();

                //UserSettings settings = UserSettings.Instance.LoadFromProperty();
                //SpotSet.Refresh();
                Program.InitDefault();

                lock (SpotSet.Lock)
                {
                    foreach (Spot spot in SpotSet.Spots)
                    {
                        Rectangle r = new Rectangle()
                        {
                            Width = spot.RectangleOverlayBorder.Width,
                            Height = spot.RectangleOverlayBorder.Height,                             
                            Fill = Brushes.Red
                        };
                        Canvas.SetLeft(r, spot.RectangleOverlayBorder.Left);
                        Canvas.SetTop(r, spot.RectangleOverlayBorder.Top);
                        
                        this.drawFrame.Children.Add(r);
                        if (spot == SpotSet.Spots[0])
                        {
                            //_mGraphics.DrawString("1.", font, solidBrushBlack, spot.Rectangle.Right + 3, spot.Rectangle.Bottom + 3);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

        }
    }

   
}
