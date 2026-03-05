using QuizLaender2.ViewModels;
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

namespace QuizLaender2
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QuizViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            _vm = new QuizViewModel();
            DataContext = _vm;
        }
        public void gameOver()
        {
            MessageBox.Show(
            $"Game Over!\n\nDeine Punkte: {_vm.Score}",
            "Game Over",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

            // Spiel neu starten
            _vm = new QuizViewModel();
            DataContext = _vm;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string selected = button.Content.ToString();

            bool correct = _vm.CheckAnswer(selected);

            if (selected == _vm.CorrectAnswer) 
            {
                button.Background = Brushes.Green;
                await Task.Delay(2000);
                button.ClearValue(Button.BackgroundProperty);
                
            }
            else
            {
                button.Background = Brushes.Red;
                await Task.Delay(2000);
                button.ClearValue(Button.BackgroundProperty);
                
                
            }
            if (!correct)
            {
                gameOver();
                return;
            }
            _vm.NextQuestion();
        }
    }
    }
