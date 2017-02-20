using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModernMewgibot.Models
{
    class TriviaQuestion : INotifyPropertyChanged
    {
        private string _question;
        public string Question
        {
            get { return _question; }
            set
            {
                if (value == _question)
                    return;

                _question = value;
                OnPropertyChanged();
            }
        }

        private string _answer;
        public string Answer
        {
            get { return _answer; }
            set
            {
                if (value == _answer)
                    return;

                _answer = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}