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
using System.Windows.Threading;

namespace Slider.Custom
{
    /// <summary>
    /// ReNewSlider.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReNewSlider : UserControl
    {
        //Max Value. default는 100
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(float), typeof(ReNewSlider), new PropertyMetadata(100f));
        //Min Value. default는 0
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(float), typeof(ReNewSlider), new PropertyMetadata(0f));
        //슬라이더 값
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(float), typeof(ReNewSlider), new PropertyMetadata(0f));
        //버튼 x축 좌표
        public static readonly DependencyProperty MoveXProperty =
            DependencyProperty.Register("MoveX", typeof(float), typeof(ReNewSlider), new PropertyMetadata(0f));

        //마우스 조작 이동 크기. default는 1
        public static readonly DependencyProperty MouseStepSizeProperty =
            DependencyProperty.Register("MouseStepSize", typeof(float), typeof(ReNewSlider), new PropertyMetadata(1f));
        //키보드 조작 이동 크기. default는 0.1
        public static readonly DependencyProperty KeyboardStepSizeProperty =
            DependencyProperty.Register("KeyboardStepSize", typeof(float), typeof(ReNewSlider), new PropertyMetadata(0.1f));

        public float Maximum //Max Value
        {
            get => (float)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        public float Minimum //Min Value
        {
            get => (float)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }
        public float Value //슬라이더 값
        {
            get => (float)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public float MoveX //버튼 x축 좌표
        {
            get => (float)GetValue(MoveXProperty);
            set => SetValue(MoveXProperty, value);
        }

        public float MouseStepSize //영역내(버튼제외) 클릭 시 MouseStepSize 만큼 이동
        {
            get => (float)GetValue(MouseStepSizeProperty);
            set => SetValue(MouseStepSizeProperty, value);
        }
        public float KeyboardStepSize //포커스온 상태에서 방향키 조작 시 KeyboardStepSize 만큼 이동
        {
            get => (float)GetValue(KeyboardStepSizeProperty);
            set => SetValue(KeyboardStepSizeProperty, value);
        }

        public Brush Fill
        { //슬라이더 버튼 색상
            set => ybar.Fill = value;
        }

        private bool _isMove = false;
        private float _width = 0;
        private float _stepWidth = 0;

        private Point ptStartMove;
        private Thickness mgnStartMove;

        public ReNewSlider()
        {
            InitializeComponent();

            Loaded += delegate
            {
                this.KeyDown += CustomSlider_KeyDown;
                this.Focusable = true;
            };
        }

        private void ybar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMove) return;

            Point pt = e.GetPosition(this);
            MoveX = (float)(mgnStartMove.Left + (pt.X - ptStartMove.X));

            if (MoveX < 0f)
            {
                //ybar.Margin = new Thickness(0, 0, 0, 0);
                MoveX = 0;
                Value = Minimum;
            }
            else if (MoveX > _width)
            {
                //ybar.Margin = new Thickness(_width, 0, 0, 0);
                MoveX = _width;
                Value = Maximum;
            }
            else //moveX >= 0 && moveX <= _width
            {
                //ybar.Margin = new Thickness(moveX, 0, 0, 0);
                Value = (MoveX / _width * (Maximum - Minimum)) + Minimum;
            }
        }

        private void ybar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMove = true;
            ptStartMove = e.GetPosition(this);
            mgnStartMove = ybar.Margin;
            ybar.CaptureMouse(); //영역 밖에서도 스크롤 유지
        }

        private void ybar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMove = false;
            ybar.ReleaseMouseCapture(); //영역 밖에서도 스크롤 유지 해제
        }
        /*StepSize만큼 버튼 이동*/
        private void MoveButton(bool isLeft, float StepSize)
        {
            if (isLeft) //왼쪽 이동
            {
                if (Value - StepSize < Minimum)//끝값 예외처리
                {
                    MoveX = 0;
                    Value = Minimum;
                }
                else
                {
                    Value -= StepSize;
                    MoveX -= StepSize * _stepWidth;
                }
            }
            else //오른쪽 이동
            {
                if (Value + StepSize > Maximum) //끝값 예외처리
                {
                    MoveX = _width;
                    Value = Maximum;
                }
                else
                {
                    Value += StepSize;
                    MoveX += StepSize * _stepWidth;
                }
            }
        }

        private void SliderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus(); //키보드 포커스

            if (_isMove) return;

            Point pt = e.GetPosition(this);
            mgnStartMove = ybar.Margin;
           
            MoveButton(pt.X < mgnStartMove.Left + 15, MouseStepSize);
        }

        private void CustomSlider_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Down:
                    MoveButton(true, KeyboardStepSize);
                    break;
                case Key.Right:
                case Key.Up:
                    MoveButton(false, KeyboardStepSize);
                    break;
            }
            e.Handled = true;
        }

        //창 크기 변화할 때 실행
        private void SliderGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _width = (float)xbar.ActualWidth;
            _stepWidth = _width / (Maximum - Minimum);

            //전체 길이가 변할 때 버튼 위치도 변화
            MoveX = (Value - Minimum) * _stepWidth;
        }
    }
}
