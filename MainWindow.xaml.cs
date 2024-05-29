using System.Text;
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

namespace Game_Snake_Wpf
{
    public partial class MainWindow : Window
    {
        private const int SnakeSquareSize = 30;
        private readonly SolidColorBrush _snakeColor = Brushes.Green;

        private Rectangle? _snakeHead;
        private Point _foodPosition;

        private static readonly Random randomPositionFood = new Random();

        private enum Direction {Left, Right, Top, Bottom}
        private Direction _direction = Direction.Right;
        
        //Настройки под таймер
        private const int TimerInterval = 200;
        private DispatcherTimer? _timer;

        public MainWindow()
        {
            InitializeComponent();
            GameCanvas.Loaded += GameCanvas_Loaded; //подписывемся на событие

        }

        private void GameCanvas_Loaded(object sender, RoutedEventArgs e) //Событие загрузки холста
        {
            _snakeHead = CreateSnakeSegment(new Point(5, 5));
            GameCanvas.Children.Add(_snakeHead);
            PlaceFood();

            //Настраиваем таймер
            _timer = new DispatcherTimer(); //Выделяем память
            _timer.Tick += Timer_Tick; //Привязываем метод к событию
            _timer.Interval = TimeSpan.FromMilliseconds(TimerInterval); //Задаем интервал в мс
            _timer.Start(); // запускаем
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            Point newHeadPosition = CalculateNewHeadPosition();
            Canvas.SetLeft(_snakeHead, newHeadPosition.X * SnakeSquareSize);
            Canvas.SetTop(_snakeHead, newHeadPosition.Y * SnakeSquareSize);

        }

        private Point CalculateNewHeadPosition()
        {
            double left = Canvas.GetLeft(_snakeHead) / SnakeSquareSize;
            double top = Canvas.GetTop(_snakeHead) / SnakeSquareSize;
            Point headCurrentPosition = new Point(left, top);

            Point newHeadPosition = new Point();

            switch(_direction)
            {
                case Direction.Left:
                    newHeadPosition = new Point(headCurrentPosition.X - 1, headCurrentPosition.Y);
                    break;
                case Direction.Right:
                    newHeadPosition = new Point(headCurrentPosition.X + 1, headCurrentPosition.Y);
                    break;
                case Direction.Top:
                    newHeadPosition = new Point(headCurrentPosition.X, headCurrentPosition.Y - 1);
                    break;
                case Direction.Bottom:
                    newHeadPosition = new Point(headCurrentPosition.X, headCurrentPosition.Y + 1);
                    break;
            }
            return newHeadPosition;
        }

        private void PlaceFood() //размещаем еду на случайной позиции
        {
            int maxX = (int)(GameCanvas.ActualWidth/ SnakeSquareSize);
            int maxY = (int)(GameCanvas.ActualHeight / SnakeSquareSize);
            int foodX = randomPositionFood.Next(0, maxX);
            int foodY = randomPositionFood.Next(0, maxY);

            _foodPosition = new Point(foodX, foodY);

            Image foodImage = new Image
            {
                Width = SnakeSquareSize, Height = SnakeSquareSize,
                Source = new BitmapImage(new Uri("/Images/Food.png", UriKind.Relative))
            };

            Canvas.SetLeft(foodImage, foodX * SnakeSquareSize);
            Canvas.SetTop(foodImage, foodY * SnakeSquareSize);

            GameCanvas.Children.Add(foodImage);

        }

        private Rectangle CreateSnakeSegment(Point position) 
        {
            Rectangle rectangle = new Rectangle
            {
                Width = SnakeSquareSize, Height = SnakeSquareSize,
                Fill = _snakeColor
            };
            Canvas.SetLeft(rectangle, position.X * SnakeSquareSize);
            Canvas.SetTop(rectangle, position.Y * SnakeSquareSize);
            return rectangle;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Up:
                    if (_direction != Direction.Bottom)
                        _direction = Direction.Top;
                    break;
                case Key.Down:
                    if (_direction != Direction.Top)
                        _direction = Direction.Bottom;
                    break;
                case Key.Left:
                    if (_direction != Direction.Right)
                        _direction = Direction.Left;
                    break;
                case Key.Right:
                    if (_direction != Direction.Left)
                        _direction = Direction.Right;
                    break;
            }
        }
    }
}