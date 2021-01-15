using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FloatingClock
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:FloatingClock"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:FloatingClock;assembly=FloatingClock"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:Clock/>
    ///
    /// </summary>
    public class Clock : Canvas
    {
        static Clock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Clock), new FrameworkPropertyMetadata(typeof(Clock)));
        }

        public Clock()
        {
            timer.Elapsed += new ElapsedEventHandler(Handle_Timer);
        }

        public void Start()
        {
            Handle_Timer(null, null);
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public Brush FaceColor = Brushes.White;
        public Brush TickColor = Brushes.DarkGray;
        public Brush HandsColor = Brushes.Black;

        private double size = 0;

        private const double hourWidth = 2;
        private const double minuteWidth = 2;
        private const double secondWidth = 0.5;
        private const double hourScale = 0.3;
        private const double minuteScale = 0.45;
        private const double secondScale = 0.45;
        private const double tailScale = 0.075;

        private UIElement hourHand;
        private UIElement minuteHand;
        private UIElement secondHand;
        private Label date;
        private Label day;

        private readonly Timer timer = new Timer();

        private void Handle_Timer(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateTime();
            });
            timer.Interval = 1000 - DateTime.Now.Millisecond;
        }

        private void UpdateTime() {
            if (hourHand == null || minuteHand == null || secondHand == null)
            {
                return;
            }

            var time = DateTime.Now;
            var hours = Math.Floor(time.TimeOfDay.TotalSeconds) % (12 * 60 * 60) / (60 * 60);
            var minutes = Math.Floor(time.TimeOfDay.TotalSeconds) % (60 * 60) / 60;
            var seconds = Math.Floor(time.TimeOfDay.TotalSeconds) % 60;

            hourHand.RenderTransform = new RotateTransform(hours * 30, hourWidth / 2, size * hourScale);
            minuteHand.RenderTransform = new RotateTransform(minutes * 6, minuteWidth / 2, size * minuteScale);
            secondHand.RenderTransform = new RotateTransform(seconds * 6, secondWidth / 2, size * secondScale);

            date.Content = time;
            day.Content = time;
        }


        private void DrawFace()
        {
            Children.Add(
                new Ellipse
                {
                    Height = size,
                    Width = size,
                    Fill = FaceColor,
                    Stroke = TickColor,
                    StrokeThickness = 1,
                }
            );

            for (double a = 0; a < 360; a += 6)
            {

                var height = 5;
                var width = 1;
                if (a % 30 == 0)
                {
                    height = 10;
                    width = 3;
                }

                var tick = new Rectangle
                {
                    Width = width,
                    Height = height,
                    Fill = TickColor,
                    Stroke = TickColor,
                    RenderTransform = new RotateTransform(a, width / 2, size / 2),
                };

                Children.Add(tick);
                SetTop(tick, 0);
                SetLeft(tick, size / 2 - width / 2);
            }
        }

        private void DrawDate()
        {
            date = new Label
            {
                Content = DateTime.Now,
                ContentStringFormat = "MMM d",
            };

            Children.Add(date);
            date.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            SetLeft(date, (size - date.DesiredSize.Width) / 2);
            SetTop(date, size / 3 - date.DesiredSize.Height / 2);

            day = new Label
            {
                Content = DateTime.Now,
                ContentStringFormat = "dddd",
            };

            Children.Add(day);
            day.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            SetLeft(day, (size - day.DesiredSize.Width) / 2);
            SetTop(day, size / 3 * 2 - day.DesiredSize.Height / 2);
        }

        private void DrawHands()
        {
            hourHand = new Rectangle
            {
                Width = hourWidth,
                Height = size * hourScale + size * tailScale,
                Fill = HandsColor,               
            };
            Canvas.SetTop(hourHand, size / 2 - (size * hourScale));
            Canvas.SetLeft(hourHand, (size - hourWidth) / 2);

            minuteHand = new Rectangle
            {
                Width = minuteWidth,
                Height = size * minuteScale + size * tailScale,
                Fill = HandsColor,
            };
            Canvas.SetTop(minuteHand, size / 2 - (size * minuteScale));
            Canvas.SetLeft(minuteHand, (size - minuteWidth) / 2);


            secondHand = new Rectangle
            {
                Width = secondWidth,
                Height = size * secondScale + size * tailScale,
                Fill = HandsColor,
            };
            Canvas.SetTop(secondHand, size / 2 - (size * secondScale));
            Canvas.SetLeft(secondHand, (size - secondWidth) / 2);

            Children.Add(hourHand);
            Children.Add(minuteHand);
            Children.Add(secondHand);

        }


        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var newSize = Math.Min(arrangeBounds.Width, arrangeBounds.Height);

            if (size != newSize)
            {
                size = newSize;
                Children.Clear();

                DrawFace();
                DrawHands();
                DrawDate();
                UpdateTime();
            }

            return base.ArrangeOverride(arrangeBounds);
        }
    }
}
