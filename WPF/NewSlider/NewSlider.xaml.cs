using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace NewSlider
{
    /// <summary>
    /// NewSlider.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NewSlider : UserControl, INotifyPropertyChanged
    {
        private bool _isMove = false;
        private String _sValue = "0";
        private Point ptStartMove;
        private Thickness mgnStartMove;

        public event PropertyChangedEventHandler PropertyChanged;

        public NewSlider()
        {
            InitializeComponent();
        }
        public System.Windows.Media.Brush Fill
        {
            set
            {
                ybar.Fill = value;
            }
        }
        public String Value
        {
            get => _sValue;
            set
            {
                _sValue = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ybar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMove) return;

            Point pt = Mouse.GetPosition(this);
            int moveX = (int)(mgnStartMove.Left + (pt.X - ptStartMove.X));
            if (moveX >= 0 && moveX <= 300)
            {
                ybar.Margin = new Thickness(moveX, 0, 0, 0);
                //text.Text = ((int)moveX).ToString();
                Value = moveX.ToString();
            }
        }
        private void ybar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMove = true;
            ptStartMove = e.GetPosition(this);
            mgnStartMove = ybar.Margin;
        }

        private void ybar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMove = false;
        }

        private void ybar_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMove = false;
        }
    }
}
