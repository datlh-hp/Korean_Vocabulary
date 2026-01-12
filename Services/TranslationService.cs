using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Korean_Vocabulary_new.Services
{
    public class TranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<char, string> _romanizationMap;

        public TranslationService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            
            // Initialize Romanization mapping
            _romanizationMap = InitializeRomanizationMap();
        }

        /// <summary>
        /// Dịch từ tiếng Hàn sang tiếng Việt
        /// </summary>
        public async Task<string?> TranslateKoreanToVietnameseAsync(string koreanText)
        {
            if (string.IsNullOrWhiteSpace(koreanText))
                return null;

            try
            {
                // Sử dụng LibreTranslate API (miễn phí, không cần API key)
                // Hoặc có thể dùng MyMemory Translation API
                var url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(koreanText)}&langpair=ko|vi";
                
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<TranslationResponse>(json);
                    
                    if (result?.ResponseData?.TranslatedText != null)
                    {
                        return result.ResponseData.TranslatedText.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Translation error: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Tự động phát hiện loại từ tiếng Hàn
        /// </summary>
        public string? DetectWordType(string koreanWord)
        {
            if (string.IsNullOrWhiteSpace(koreanWord))
                return null;

            koreanWord = koreanWord.Trim();

            // 감탄사 (Thán từ) - các từ cảm thán phổ biến
            if (koreanWord == "안녕하세요" || koreanWord == "안녕" || 
                koreanWord == "네" || koreanWord == "아니요" || 
                koreanWord == "감사합니다" || koreanWord == "죄송합니다" ||
                koreanWord == "예" || koreanWord == "아니")
            {
                return "감탄사";
            }

            // 수사 (Số từ) - số đếm Hàn Quốc
            if (koreanWord == "일" || koreanWord == "이" || koreanWord == "삼" || 
                koreanWord == "사" || koreanWord == "오" || koreanWord == "육" ||
                koreanWord == "칠" || koreanWord == "팔" || koreanWord == "구" || 
                koreanWord == "십" || koreanWord == "백" || koreanWord == "천" ||
                koreanWord == "만")
            {
                return "수사";
            }

            // 형용사 (Tính từ) - kết thúc bằng -적 (nhưng không phải -하다)
            if (koreanWord.EndsWith("적") && !koreanWord.EndsWith("하다"))
            {
                return "형용사";
            }

            // 형용사 (Tính từ) - các từ tính từ phổ biến
            if (koreanWord.EndsWith("있다") || koreanWord.EndsWith("없다") || 
                koreanWord.EndsWith("좋다") || koreanWord.EndsWith("나쁘다") ||
                koreanWord.EndsWith("크다") || koreanWord.EndsWith("작다") ||
                koreanWord.EndsWith("어렵다") || koreanWord.EndsWith("쉽다") ||
                koreanWord.EndsWith("편리하다") || koreanWord.EndsWith("불편하다") ||
                koreanWord.EndsWith("용이하다") || koreanWord.EndsWith("곤란하다") ||
                koreanWord.EndsWith("많다") || koreanWord.EndsWith("적다") ||
                koreanWord.EndsWith("높다") || koreanWord.EndsWith("낮다") ||
                koreanWord.EndsWith("길다") || koreanWord.EndsWith("짧다") ||
                koreanWord.EndsWith("넓다") || koreanWord.EndsWith("좁다") ||
                koreanWord.EndsWith("따뜻하다") || koreanWord.EndsWith("차갑다") ||
                koreanWord.EndsWith("뜨겁다") || koreanWord.EndsWith("시원하다"))
            {
                return "형용사";
            }

            // 동사 (Động từ) - kết thúc bằng -하다, -다, -되다, -하다
            if (koreanWord.EndsWith("하다") || koreanWord.EndsWith("되다") ||
                koreanWord.EndsWith("받다") || koreanWord.EndsWith("주다") ||
                koreanWord.EndsWith("가다") || koreanWord.EndsWith("오다") ||
                koreanWord.EndsWith("보다") || koreanWord.EndsWith("듣다") ||
                koreanWord.EndsWith("읽다") || koreanWord.EndsWith("쓰다") ||
                koreanWord.EndsWith("말하다") || koreanWord.EndsWith("만나다") ||
                koreanWord.EndsWith("사다") || koreanWord.EndsWith("팔다") ||
                koreanWord.EndsWith("먹다") || koreanWord.EndsWith("마시다") ||
                koreanWord.EndsWith("자다") || koreanWord.EndsWith("일어나다") ||
                koreanWord.EndsWith("공부하다") || koreanWord.EndsWith("일하다") ||
                koreanWord.EndsWith("배우다") || koreanWord.EndsWith("가르치다"))
            {
                return "동사";
            }

            // 동사 (Động từ) - các động từ kết thúc bằng -다 (sau khi đã kiểm tra tính từ)
            if (koreanWord.EndsWith("다") && !koreanWord.EndsWith("있다") && 
                !koreanWord.EndsWith("없다") && !koreanWord.EndsWith("좋다") &&
                !koreanWord.EndsWith("나쁘다") && !koreanWord.EndsWith("크다") &&
                !koreanWord.EndsWith("작다") && !koreanWord.EndsWith("어렵다") &&
                !koreanWord.EndsWith("쉽다"))
            {
                return "동사";
            }

            // 부사 (Trạng từ) - các trạng từ phổ biến
            if (koreanWord.EndsWith("게") || koreanWord.EndsWith("히") ||
                koreanWord == "매우" || koreanWord == "아주" || 
                koreanWord == "정말" || koreanWord == "너무" ||
                koreanWord == "잘" || koreanWord == "빨리" ||
                koreanWord == "천천히" || koreanWord == "자주" ||
                koreanWord == "항상" || koreanWord == "가끔")
            {
                return "부사";
            }

            // 조사 (Trợ từ) - các trợ từ phổ biến
            if (koreanWord == "은" || koreanWord == "는" || 
                koreanWord == "이" || koreanWord == "가" ||
                koreanWord == "을" || koreanWord == "를" ||
                koreanWord == "에" || koreanWord == "에서" ||
                koreanWord == "와" || koreanWord == "과" ||
                koreanWord == "의" || koreanWord == "로" ||
                koreanWord == "으로")
            {
                return "조사";
            }

            // 대명사 (Đại từ) - các đại từ phổ biến
            if (koreanWord == "나" || koreanWord == "저" ||
                koreanWord == "너" || koreanWord == "우리" ||
                koreanWord == "그" || koreanWord == "이" ||
                koreanWord == "저것" || koreanWord == "이것" ||
                koreanWord == "그것" || koreanWord == "누구" ||
                koreanWord == "무엇" || koreanWord == "어디")
            {
                return "대명사";
            }

            // 관형사 (Quán từ) - các quán từ phổ biến
            if (koreanWord == "새" || koreanWord == "옛" ||
                koreanWord == "온" || koreanWord == "헌")
            {
                return "관형사";
            }

            // Mặc định là 명사 (Danh từ)
            return "명사";
        }

        /// <summary>
        /// Chuyển đổi Hangeul sang Romanization (phát âm)
        /// </summary>
        public string ConvertToRomanization(string koreanText)
        {
            if (string.IsNullOrWhiteSpace(koreanText))
                return string.Empty;

            var result = new StringBuilder();
            
            for (int i = 0; i < koreanText.Length; i++)
            {
                char c = koreanText[i];
                
                if (IsHangeul(c))
                {
                    string romanized = ConvertHangeulToRomanization(c);
                    if (!string.IsNullOrEmpty(romanized))
                    {
                        result.Append(romanized);
                    }
                    else
                    {
                        // Nếu không chuyển đổi được, giữ nguyên ký tự
                        result.Append(c);
                    }
                }
                else if (char.IsWhiteSpace(c))
                {
                    // Giữ khoảng trắng
                    result.Append(c);
                }
                else
                {
                    // Giữ nguyên các ký tự không phải Hangeul (số, ký tự đặc biệt, v.v.)
                    result.Append(c);
                }
            }

            string finalResult = result.ToString();
            System.Diagnostics.Debug.WriteLine($"ConvertToRomanization: '{koreanText}' -> '{finalResult}'");
            return finalResult;
        }

        private bool IsHangeul(char c)
        {
            // Hangeul Unicode range: AC00-D7AF
            return c >= 0xAC00 && c <= 0xD7AF;
        }

        private string ConvertHangeulToRomanization(char hangeul)
        {
            if (!IsHangeul(hangeul))
                return hangeul.ToString();

            try
            {
                // Hangeul syllable structure: (initial) + (medial) + (final)
                int code = hangeul - 0xAC00;
                int initial = code / (21 * 28);
                int medial = (code % (21 * 28)) / 28;
                int final = code % 28;

                string initialSound = GetInitialSound(initial);
                string medialSound = GetMedialSound(medial);
                string finalSound = GetFinalSound(final);

                // Combine sounds
                string result = initialSound + medialSound;
                if (final > 0 && !string.IsNullOrEmpty(finalSound))
                {
                    result += finalSound;
                }

                System.Diagnostics.Debug.WriteLine($"Hangeul '{hangeul}' (code: {code}) -> initial: {initial}, medial: {medial}, final: {final} -> '{result}'");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting Hangeul '{hangeul}': {ex.Message}");
                return hangeul.ToString();
            }
        }

        private string GetInitialSound(int index)
        {
            // Revised Romanization of Korean
            string[] initials = {
                "g", "kk", "n", "d", "tt", "r", "m", "b", "pp",
                "s", "ss", "", "j", "jj", "ch", "k", "t", "p", "h"
            };
            return index < initials.Length ? initials[index] : "";
        }

        private string GetMedialSound(int index)
        {
            // Revised Romanization of Korean
            string[] medials = {
                "a", "ae", "ya", "yae", "eo", "e", "yeo", "ye",
                "o", "wa", "wae", "oe", "yo", "u", "wo", "we",
                "wi", "yu", "eu", "ui", "i"
            };
            return index < medials.Length ? medials[index] : "";
        }

        private string GetFinalSound(int index)
        {
            if (index == 0) return "";
            
            // Revised Romanization of Korean - batchim (받침)
            // Index mapping: 0=none, 1=ㄱ, 2=ㄲ, 3=ㄳ, 4=ㄴ, 5=ㄵ, 6=ㄶ, 7=ㄷ, 8=ㄹ, 9=ㄺ, 10=ㄻ, 11=ㄼ, 12=ㄽ, 13=ㄾ, 14=ㄿ, 15=ㅀ, 16=ㅁ, 17=ㅂ, 18=ㅄ, 19=ㅅ, 20=ㅆ, 21=ㅇ, 22=ㅈ, 23=ㅊ, 24=ㅋ, 25=ㅌ, 26=ㅍ, 27=ㅎ
            string[] finals = {
                "", "k", "k", "k", "n", "n", "n", "t", "l", "k", "m",
                "p", "l", "l", "l", "m", "p", "l", "p", "p", "t",
                "t", "ng", "t", "t", "k", "t", "p", "t"
            };
            return index < finals.Length ? finals[index] : "";
        }

        private Dictionary<char, string> InitializeRomanizationMap()
        {
            // Basic mapping for common characters
            return new Dictionary<char, string>();
        }

        // Response model for translation API
        private class TranslationResponse
        {
            [JsonPropertyName("responseData")]
            public ResponseData? ResponseData { get; set; }
        }

        private class ResponseData
        {
            [JsonPropertyName("translatedText")]
            public string? TranslatedText { get; set; }
        }
    }
}

