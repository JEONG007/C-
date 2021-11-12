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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Slider
{
    /// <summary>
    /// NewSlider.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class NewSlider : UserControl, INotifyPropertyChanged
    {
        private bool _isMove = false;
        private String _sValue = "0";
        private int _width = 0;
        private int _max = 100;
        private Point ptStartMove;
        private Thickness mgnStartMove;

        public event PropertyChangedEventHandler PropertyChanged; 

        public System.Windows.Media.Brush Fill { //슬라이더 버튼 색상
            set
            {
                ybar.Fill = value;
            }
        }

        public String Value //슬라이더 값
        { 
            get => _sValue;
            set
            {
                _sValue = value;
                OnPropertyChanged();
            }
        }

        public int Max { get => _max; set => _max = value; }

        public NewSlider()
        {
            InitializeComponent();
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private double GetPercentage(double value)
        {
            return System.Math.Round(value * 100 / Max, 100);
        }

        private void ybar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMove) return;

            Point pt = Mouse.GetPosition(this);
            int moveX = (int)(mgnStartMove.Left + (pt.X - ptStartMove.X));
            if(moveX >= 0 && moveX <= _width)
            {
                ybar.Margin = new Thickness(moveX, 0, 0, 0);
                //text.Text = ((int)moveX).ToString();
                Value = moveX.ToString();
            }
        }

        private void ybar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMove = true;
            _width = (int)xbar.ActualWidth;
            ptStartMove = e.GetPosition(this);
            mgnStartMove = ybar.Margin;
            ybar.CaptureMouse(); //영영밖에서도 스크롤 유지
        }

        private void ybar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMove = false;
            ybar.ReleaseMouseCapture();
        }

        //private void ybar_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    _isMove = false;
        //}
    }
}
