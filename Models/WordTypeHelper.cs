namespace Korean_Vocabulary_new.Models
{
    public static class WordTypeHelper
    {
        public static List<string> GetWordTypes()
        {
            return new List<string>
            {
                "명사",      // Danh từ
                "동사",      // Động từ
                "형용사",    // Tính từ
                "부사",      // Trạng từ
                "대명사",    // Đại từ
                "조사",      // Trợ từ
                "감탄사",    // Thán từ
                "수사",      // Số từ
                "관형사"     // Quán từ
            };
        }

        public static string GetVietnameseName(string koreanWordType)
        {
            return koreanWordType switch
            {
                "명사" => "Danh từ",
                "동사" => "Động từ",
                "형용사" => "Tính từ",
                "부사" => "Trạng từ",
                "대명사" => "Đại từ",
                "조사" => "Trợ từ",
                "감탄사" => "Thán từ",
                "수사" => "Số từ",
                "관형사" => "Quán từ",
                _ => koreanWordType
            };
        }
    }
}


