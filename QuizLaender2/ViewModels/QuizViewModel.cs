using QuizLaender2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

namespace QuizLaender2.ViewModels
{
    internal class QuizViewModel : INotifyPropertyChanged
    {
        private List<Country> _allCountries;
        private Country _currentCountry;
        private int _score;
        private int _error = 3;
        private int _questionIndex;
        public string CorrectAnswer => _currentCountry?.Name;

        private string _selectedAnswer;
        public string SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                _selectedAnswer = value;
                OnPropertyChanged();
            }
        }
        private DispatcherTimer _timer;
        private int _timeLeft = 10;

        public int TimeLeft
        {
            get => _timeLeft;
            set
            {
                _timeLeft = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimerColor));
            }
        }
        public string TimerColor
        {
            get
            {
                if (TimeLeft <= 2) return "Red";
                if (TimeLeft <= 5) return "Orange";
                return "Green";
            }
        }
        public int Lifes
        {
            get => _error;
            set
            {
                _error = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> AnswerOptions { get; set; }

        public string CurrentFlag => _currentCountry?.FlagPath;

        public int Score
        {
            get => _score;
            set { _score = value; OnPropertyChanged(); }
        }

        public QuizViewModel()
        {
            LoadCountries();
            AnswerOptions = new ObservableCollection<string>();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            
            
            NextQuestion();
            _timer.Start();
            
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeLeft--;
            if (TimeLeft <= 0)
            {
                _timer.Stop();
                MessageBox.Show($"Zeit abgelaufen!\nPunkte: {Score}");
                Lifes--;
                if (Lifes <= 0)
                {
                    MessageBox.Show($"Game Over!\nPunkte: {Score}");
                    Score = 0;
                    Lifes = 3;
                }
                NextQuestion();
            }

        }
        private void LoadCountries()
        {
            var json = File.ReadAllText("Data/Countries.json");
            _allCountries = JsonSerializer.Deserialize<List<Country>>(json);
        }

        public void NextQuestion()
        {
            _timer.Stop();
            TimeLeft = 10;
           
            var random = new Random();

            _currentCountry = _allCountries[random.Next(_allCountries.Count)];

            var options = _allCountries
                .OrderBy(x => random.Next())
                .Take(4)
                .Select(x => x.Name)
                .ToList();

            if (!options.Contains(_currentCountry.Name))
                options[0] = _currentCountry.Name;

            options = options.OrderBy(x => random.Next()).ToList();

            AnswerOptions.Clear();
            foreach (var option in options)
                AnswerOptions.Add(option);

            OnPropertyChanged(nameof(CurrentFlag));
            _timer.Start();

        }

        public bool CheckAnswer(string selected)
        {
            _timer.Stop();
            if (selected == _currentCountry.Name)
            {
                Score++;
            }
            else
            {
                Lifes--;
                //MessageBox.Show("Falsche Antwort :( Die richtige Antwort ist:" + _currentCountry.Name);
               
            }
            return Lifes > 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
