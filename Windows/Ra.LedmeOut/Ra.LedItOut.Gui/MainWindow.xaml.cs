using LedItOut;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
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

namespace Ra.LedItOut.Gui
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Arrow Points
        private Point _mTopEnd;
        private Point _mTopHead;
        private Point _mRightEnd;
        private Point _mRightHead;
        private Point _mBottomEnd;
        private Point _mBottomHead;
        private Point _mLeftEnd;
        private Point _mLeftHead;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitArrowPoints()
        {
            var screenHeight = System.Windows.SystemParameters.WorkArea.Height;
            var screenWidth = System.Windows.SystemParameters.WorkArea.Height;

            _mTopEnd = new Point(1 * (screenWidth / 4), 1 * (screenHeight / 4));
            _mTopHead = new Point(3 * (screenWidth / 4), 1 * (screenHeight / 4));

            _mRightEnd = new Point(4 * (screenWidth / 5), 1 * (screenHeight / 4));
            _mRightHead = new Point(4 * (screenWidth / 5), 3 * (screenHeight / 4));

            _mBottomEnd = new Point(3 * (screenWidth / 4), 3 * (screenHeight / 4));
            _mBottomHead = new Point(1 * (screenWidth / 4), 3 * (screenHeight / 4));

            _mLeftEnd = new Point(1 * (screenWidth / 5), 3 * (screenHeight / 4));
            _mLeftHead = new Point(1 * (screenWidth / 5), 1 * (screenHeight / 4));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new Overlay().Show();
        }
    }
}
