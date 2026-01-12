using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Korean_Vocabulary_new.Models
{
    [Table("VocabularyWords")]
    [Preserve(AllMembers = true)]
    public class VocabularyWord : INotifyPropertyChanged
    {
        private bool _isFavorite;
        private bool _isSelected;
        private string _koreanWord = string.Empty;
        private string _vietnameseMeaning = string.Empty;
        private string? _pronunciation;
        private string? _exampleSentence;
        private string? _exampleTranslation;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(200)]
        public string KoreanWord
        {
            get => _koreanWord;
            set
            {
                if (_koreanWord != value)
                {
                    _koreanWord = value;
                    OnPropertyChanged();
                }
            }
        }

        [MaxLength(200)]
        public string VietnameseMeaning
        {
            get => _vietnameseMeaning;
            set
            {
                if (_vietnameseMeaning != value)
                {
                    _vietnameseMeaning = value;
                    OnPropertyChanged();
                }
            }
        }

        [MaxLength(200)]
        public string? Pronunciation
        {
            get => _pronunciation;
            set
            {
                if (_pronunciation != value)
                {
                    _pronunciation = value;
                    OnPropertyChanged();
                }
            }
        }

        [MaxLength(500)]
        public string? ExampleSentence
        {
            get => _exampleSentence;
            set
            {
                if (_exampleSentence != value)
                {
                    _exampleSentence = value;
                    OnPropertyChanged();
                }
            }
        }

        [MaxLength(500)]
        public string? ExampleTranslation
        {
            get => _exampleTranslation;
            set
            {
                if (_exampleTranslation != value)
                {
                    _exampleTranslation = value;
                    OnPropertyChanged();
                }
            }
        }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(50)]
        public string? WordType { get; set; } // Loại từ: 명사, 동사, 형용사, etc.

        public int DifficultyLevel { get; set; } = 1; // 1-5

        public int StudyCount { get; set; } = 0;

        public int CorrectCount { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime LastStudiedDate { get; set; } = DateTime.MinValue;

        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (_isFavorite != value)
                {
                    _isFavorite = value;
                    OnPropertyChanged();
                }
            }
        }

        [Ignore]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

