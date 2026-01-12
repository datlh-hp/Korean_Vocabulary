using Microsoft.Maui.Media;

namespace Korean_Vocabulary_new.Services
{
    public class AudioService
    {
        /// <summary>
        /// Phát âm từ tiếng Hàn sử dụng Text-to-Speech
        /// </summary>
        public async Task SpeakKoreanAsync(string koreanText, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(koreanText))
                return;

            try
            {
                // Sử dụng locale tiếng Hàn (ko-KR) để phát âm chính xác
                var locales = await TextToSpeech.GetLocalesAsync();
                var koreanLocale = locales.FirstOrDefault(l => l.Language.StartsWith("ko", StringComparison.OrdinalIgnoreCase));

                var options = new SpeechOptions
                {
                    Locale = koreanLocale,
                    Pitch = 1.0f,      // Cao độ bình thường
                    Volume = 1.0f,     // Âm lượng tối đa
                };

                await TextToSpeech.SpeakAsync(koreanText, options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AudioService SpeakKoreanAsync error: {ex.Message}");
                // Không throw exception để không làm crash app
            }
        }

        /// <summary>
        /// Phát âm từ tiếng Việt
        /// </summary>
        public async Task SpeakVietnameseAsync(string vietnameseText, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(vietnameseText))
                return;

            try
            {
                // Sử dụng locale tiếng Việt (vi-VN) nếu có
                var locales = await TextToSpeech.GetLocalesAsync();
                var vietnameseLocale = locales.FirstOrDefault(l => l.Language.StartsWith("vi", StringComparison.OrdinalIgnoreCase));

                var options = new SpeechOptions
                {
                    Locale = vietnameseLocale,
                    Pitch = 1.0f,
                    Volume = 1.0f,
                };

                await TextToSpeech.SpeakAsync(vietnameseText, options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AudioService SpeakVietnameseAsync error: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra xem Text-to-Speech có sẵn không
        /// </summary>
        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var locales = await TextToSpeech.GetLocalesAsync();
                return locales != null && locales.Any();
            }
            catch
            {
                return false;
            }
        }
    }
}

