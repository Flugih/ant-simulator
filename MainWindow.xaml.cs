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

namespace ant_simulator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    bool[,] cells = new bool[0, 0];
    Rectangle[,] rectangles = new Rectangle[0, 0];
    int fieldWidth;
    int fieldHeight;
    int antX;
    int antY;
    int antDirection;
    int steps;
    int stepsPerTick = 5;
    DispatcherTimer timer = new DispatcherTimer();

    public MainWindow()
    {
        InitializeComponent();
        timer.Interval = TimeSpan.FromMilliseconds(10);
        timer.Tick += Timer_Tick;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        GenerateField();
    }

    private void buttonGenerate_Click(object sender, RoutedEventArgs e)
    {
        GenerateField();
    }

    private void buttonStart_Click(object sender, RoutedEventArgs e)
    {
        timer.Start();
        buttonStart.IsEnabled = false;
        buttonPause.IsEnabled = true;
    }

    private void buttonPause_Click(object sender, RoutedEventArgs e)
    {
        timer.Stop();
        buttonStart.IsEnabled = true;
        buttonPause.IsEnabled = false;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        for (int i = 0; i < stepsPerTick; i++)
        {
            MakeStep();
        }

        textBlockSteps.Text = "Шагов: " + steps;
    }

    private void GenerateField()
    {
        int newWidth;
        int newHeight;

        if (int.TryParse(textBoxWidth.Text, out newWidth) == false || newWidth < 10 || newWidth > 100)
        {
            MessageBox.Show("Ширина должна быть числом от 10 до 100");
            return;
        }
        if (int.TryParse(textBoxHeight.Text, out newHeight) == false || newHeight < 10 || newHeight > 100)
        {
            MessageBox.Show("Высота должна быть числом от 10 до 100");
            return;
        }

        timer.Stop();

        fieldWidth = newWidth;
        fieldHeight = newHeight;
        cells = new bool[fieldWidth, fieldHeight];

        antX = fieldWidth / 2;
        antY = fieldHeight / 2;
        antDirection = 0;
        steps = 0;

        gridField.Children.Clear();
        gridField.Columns = fieldWidth;
        gridField.Rows = fieldHeight;
        rectangles = new Rectangle[fieldWidth, fieldHeight];

        for (int y = 0; y < fieldHeight; y++)
        {
            for (int x = 0; x < fieldWidth; x++)
            {
                Rectangle rectangle = new Rectangle();
                rectangle.Width = 10;
                rectangle.Height = 10;
                rectangle.Stroke = Brushes.LightGray;
                rectangle.StrokeThickness = 0.5;

                rectangles[x, y] = rectangle;
                gridField.Children.Add(rectangle);
                PaintCell(x, y);
            }
        }

        textBlockSteps.Text = "Шагов: 0";
        buttonStart.IsEnabled = true;
        buttonPause.IsEnabled = false;
    }

    private void MakeStep()
    {
        string rule;
        bool recolor;

        if (cells[antX, antY] == false)
        {
            rule = comboBoxWhite.Text;
            recolor = checkBoxWhiteRecolor.IsChecked == true;
        }
        else
        {
            rule = comboBoxBlack.Text;
            recolor = checkBoxBlackRecolor.IsChecked == true;
        }

        if (rule == "Повернуть направо")
        {
            TurnRight();
        }
        if (rule == "Повернуть налево")
        {
            TurnLeft();
        }

        if (recolor)
        {
            cells[antX, antY] = !cells[antX, antY];
        }

        int oldX = antX;
        int oldY = antY;

        MoveForward();
        steps++;

        PaintCell(oldX, oldY);
        PaintCell(antX, antY);
    }

    private void TurnRight()
    {
        antDirection = antDirection + 1;
        if (antDirection > 3)
        {
            antDirection = 0;
        }
    }

    private void TurnLeft()
    {
        antDirection = antDirection - 1;
        if (antDirection < 0)
        {
            antDirection = 3;
        }
    }

    private void MoveForward()
    {
        if (antDirection == 0)
        {
            antY = antY - 1;
        }
        else if (antDirection == 1)
        {
            antX = antX + 1;
        }
        else if (antDirection == 2)
        {
            antY = antY + 1;
        }
        else
        {
            antX = antX - 1;
        }

        if (antX < 0)
        {
            antX = fieldWidth - 1;
        }
        if (antX >= fieldWidth)
        {
            antX = 0;
        }
        if (antY < 0)
        {
            antY = fieldHeight - 1;
        }
        if (antY >= fieldHeight)
        {
            antY = 0;
        }
    }

    private void PaintCell(int x, int y)
    {
        if (x == antX && y == antY)
        {
            rectangles[x, y].Fill = Brushes.Red;
        }
        else if (cells[x, y])
        {
            rectangles[x, y].Fill = Brushes.Black;
        }
        else
        {
            rectangles[x, y].Fill = Brushes.White;
        }
    }
}
