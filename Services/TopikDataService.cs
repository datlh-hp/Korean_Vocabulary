using Korean_Vocabulary_new.Models;
using Korean_Vocabulary_new.Services;

namespace Korean_Vocabulary_new.Services
{
    public class TopikDataService
    {
        private readonly DatabaseService _databaseService;

        public TopikDataService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task SeedTopikVocabularyAsync()
        {
            
            var words = GetTopikVocabulary();
            int addedCount = 0;
            int updatedCount = 0;
            int skippedCount = 0;
            
            foreach (var word in words)
            {
                // Auto-detect word type if not set
                if (string.IsNullOrEmpty(word.WordType))
                {
                    word.WordType = DetectWordType(word.KoreanWord);
                }

                // Check if word already exists
                var existing = await _databaseService.SearchWordsAsync(word.KoreanWord);
                var existingWord = existing.FirstOrDefault(w => w.KoreanWord == word.KoreanWord);
                
                if (existingWord == null)
                {
                    await _databaseService.SaveWordAsync(word);
                    addedCount++;
                }
                else
                {
                    // Update existing word with WordType if it's missing
                    if (string.IsNullOrEmpty(existingWord.WordType) && !string.IsNullOrEmpty(word.WordType))
                    {
                        existingWord.WordType = word.WordType;
                        await _databaseService.SaveWordAsync(existingWord);
                        updatedCount++;
                    }
                    else
                    {
                        skippedCount++;
                    }
                }
            }
        }

        private string? DetectWordType(string koreanWord)
        {
            if (string.IsNullOrEmpty(koreanWord)) return null;

            // Tính từ: kết thúc bằng -적 (nhưng không phải -하다)
            if (koreanWord.EndsWith("적") && !koreanWord.EndsWith("하다"))
            {
                return "형용사";
            }

            // Tính từ: một số từ đặc biệt
            if (koreanWord.EndsWith("있다") || koreanWord.EndsWith("없다") || 
                koreanWord.EndsWith("좋다") || koreanWord.EndsWith("나쁘다") ||
                koreanWord.EndsWith("크다") || koreanWord.EndsWith("작다") ||
                koreanWord.EndsWith("어렵다") || koreanWord.EndsWith("쉽다") ||
                koreanWord.EndsWith("편리하다") || koreanWord.EndsWith("불편하다") ||
                koreanWord.EndsWith("용이하다") || koreanWord.EndsWith("곤란하다"))
            {
                return "형용사";
            }

            // Động từ: kết thúc bằng -하다, -다 (sau khi đã kiểm tra tính từ)
            if (koreanWord.EndsWith("하다") || koreanWord.EndsWith("다"))
            {
                return "동사";
            }

            // Số từ
            if (koreanWord == "일" || koreanWord == "이" || koreanWord == "삼" || 
                koreanWord == "사" || koreanWord == "오" || koreanWord == "육" ||
                koreanWord == "칠" || koreanWord == "팔" || koreanWord == "구" || koreanWord == "십")
            {
                return "수사";
            }

            // Mặc định là danh từ
            return "명사";
        }


        private List<VocabularyWord> GetTopikVocabulary()
        {
            return new List<VocabularyWord>
            {
                // TOPIK 1 - Cơ bản
                new VocabularyWord { KoreanWord = "안녕하세요", VietnameseMeaning = "Xin chào", Pronunciation = "annyeonghaseyo", Category = "", DifficultyLevel = 1, WordType = "감탄사" },
                new VocabularyWord { KoreanWord = "감사합니다", VietnameseMeaning = "Cảm ơn", Pronunciation = "gamsahamnida", Category = "", DifficultyLevel = 1, WordType = "동사" },
                new VocabularyWord { KoreanWord = "죄송합니다", VietnameseMeaning = "Xin lỗi", Pronunciation = "joesonghamnida", Category = "", DifficultyLevel = 1, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "네", VietnameseMeaning = "Vâng, có", Pronunciation = "ne", Category = "", DifficultyLevel = 1, WordType = "감탄사" },
                new VocabularyWord { KoreanWord = "아니요", VietnameseMeaning = "Không", Pronunciation = "aniyo", Category = "", DifficultyLevel = 1, WordType = "감탄사" },
                new VocabularyWord { KoreanWord = "물", VietnameseMeaning = "Nước", Pronunciation = "mul", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "밥", VietnameseMeaning = "Cơm", Pronunciation = "bap", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "사과", VietnameseMeaning = "Táo", Pronunciation = "sagwa", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "학교", VietnameseMeaning = "Trường học", Pronunciation = "hakgyo", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "학생", VietnameseMeaning = "Học sinh", Pronunciation = "haksaeng", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "선생님", VietnameseMeaning = "Giáo viên", Pronunciation = "seonsaengnim", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "친구", VietnameseMeaning = "Bạn bè", Pronunciation = "chingu", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "가족", VietnameseMeaning = "Gia đình", Pronunciation = "gajok", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "아버지", VietnameseMeaning = "Bố", Pronunciation = "abeoji", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "어머니", VietnameseMeaning = "Mẹ", Pronunciation = "eomeoni", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "오늘", VietnameseMeaning = "Hôm nay", Pronunciation = "oneul", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "내일", VietnameseMeaning = "Ngày mai", Pronunciation = "naeil", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "어제", VietnameseMeaning = "Hôm qua", Pronunciation = "eoje", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "시간", VietnameseMeaning = "Thời gian", Pronunciation = "sigan", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "일", VietnameseMeaning = "Một, ngày", Pronunciation = "il", Category = "", DifficultyLevel = 1, WordType = "수사" },
                new VocabularyWord { KoreanWord = "이", VietnameseMeaning = "Hai", Pronunciation = "i", Category = "", DifficultyLevel = 1, WordType = "수사" },
                new VocabularyWord { KoreanWord = "삼", VietnameseMeaning = "Ba", Pronunciation = "sam", Category = "", DifficultyLevel = 1, WordType = "수사" },
                new VocabularyWord { KoreanWord = "사람", VietnameseMeaning = "Người", Pronunciation = "saram", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "한국", VietnameseMeaning = "Hàn Quốc", Pronunciation = "hanguk", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "한국어", VietnameseMeaning = "Tiếng Hàn", Pronunciation = "hangugeo", Category = "", DifficultyLevel = 1, WordType = "명사" },
                new VocabularyWord { KoreanWord = "좋아하다", VietnameseMeaning = "Thích", Pronunciation = "joahada", Category = "", DifficultyLevel = 1, WordType = "동사" },
                new VocabularyWord { KoreanWord = "먹다", VietnameseMeaning = "Ăn", Pronunciation = "meokda", Category = "", DifficultyLevel = 1, WordType = "동사" },
                new VocabularyWord { KoreanWord = "마시다", VietnameseMeaning = "Uống", Pronunciation = "masida", Category = "", DifficultyLevel = 1, WordType = "동사" },
                new VocabularyWord { KoreanWord = "가다", VietnameseMeaning = "Đi", Pronunciation = "gada", Category = "", DifficultyLevel = 1, WordType = "동사" },
                new VocabularyWord { KoreanWord = "오다", VietnameseMeaning = "Đến", Pronunciation = "oda", Category = "", DifficultyLevel = 1, WordType = "동사" },

                // TOPIK 2 - Sơ cấp (WordType sẽ được tự động phát hiện)
                new VocabularyWord { KoreanWord = "공부하다", VietnameseMeaning = "Học tập", Pronunciation = "gongbuhada", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "일하다", VietnameseMeaning = "Làm việc", Pronunciation = "ilhada", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "만나다", VietnameseMeaning = "Gặp gỡ", Pronunciation = "mannada", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "사다", VietnameseMeaning = "Mua", Pronunciation = "sada", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "팔다", VietnameseMeaning = "Bán", Pronunciation = "palda", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "읽다", VietnameseMeaning = "Đọc", Pronunciation = "ikda", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "쓰다", VietnameseMeaning = "Viết", Pronunciation = "sseuda", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "듣다", VietnameseMeaning = "Nghe", Pronunciation = "deutda", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "말하다", VietnameseMeaning = "Nói", Pronunciation = "malhada", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "보다", VietnameseMeaning = "Xem", Pronunciation = "boda", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "만들다", VietnameseMeaning = "Làm, tạo", Pronunciation = "mandeulda", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "주다", VietnameseMeaning = "Cho, tặng", Pronunciation = "juda", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "받다", VietnameseMeaning = "Nhận", Pronunciation = "batda", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "도와주다", VietnameseMeaning = "Giúp đỡ", Pronunciation = "dowajuda", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "알다", VietnameseMeaning = "Biết", Pronunciation = "alda", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "모르다", VietnameseMeaning = "Không biết", Pronunciation = "moreuda", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "시작하다", VietnameseMeaning = "Bắt đầu", Pronunciation = "sijakhada", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "끝나다", VietnameseMeaning = "Kết thúc", Pronunciation = "kkeunnada", Category = "", DifficultyLevel = 2 },
                new VocabularyWord { KoreanWord = "시작", VietnameseMeaning = "Sự bắt đầu", Pronunciation = "sijak", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "끝", VietnameseMeaning = "Kết thúc", Pronunciation = "kkeut", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "아침", VietnameseMeaning = "Buổi sáng", Pronunciation = "achim", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "점심", VietnameseMeaning = "Buổi trưa", Pronunciation = "jeomsim", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "저녁", VietnameseMeaning = "Buổi tối", Pronunciation = "jeonyeok", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "밤", VietnameseMeaning = "Đêm", Pronunciation = "bam", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "월요일", VietnameseMeaning = "Thứ hai", Pronunciation = "woryoil", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "화요일", VietnameseMeaning = "Thứ ba", Pronunciation = "hwayoil", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "수요일", VietnameseMeaning = "Thứ tư", Pronunciation = "suyoil", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "목요일", VietnameseMeaning = "Thứ năm", Pronunciation = "mogyoil", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "금요일", VietnameseMeaning = "Thứ sáu", Pronunciation = "geumyoil", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "토요일", VietnameseMeaning = "Thứ bảy", Pronunciation = "toyoil", Category = "", DifficultyLevel = 2, WordType = "명사" },
                new VocabularyWord { KoreanWord = "일요일", VietnameseMeaning = "Chủ nhật", Pronunciation = "iryoil", Category = "", DifficultyLevel = 2, WordType = "명사" },

                // TOPIK 3 - Trung cấp
                new VocabularyWord { KoreanWord = "경험", VietnameseMeaning = "Kinh nghiệm", Pronunciation = "gyeongheom", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "기회", VietnameseMeaning = "Cơ hội", Pronunciation = "gihoe", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "목표", VietnameseMeaning = "Mục tiêu", Pronunciation = "mokpyo", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "계획", VietnameseMeaning = "Kế hoạch", Pronunciation = "gyehoek", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "준비", VietnameseMeaning = "Chuẩn bị", Pronunciation = "junbi", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "준비하다", VietnameseMeaning = "Chuẩn bị", Pronunciation = "junbihada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "결정하다", VietnameseMeaning = "Quyết định", Pronunciation = "gyeoljeonghada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "선택하다", VietnameseMeaning = "Lựa chọn", Pronunciation = "seontaekhada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "이해하다", VietnameseMeaning = "Hiểu", Pronunciation = "ihaehada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "설명하다", VietnameseMeaning = "Giải thích", Pronunciation = "seolmyeonghada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "질문", VietnameseMeaning = "Câu hỏi", Pronunciation = "jilmun", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "답변", VietnameseMeaning = "Câu trả lời", Pronunciation = "dapbyeon", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "문제", VietnameseMeaning = "Vấn đề, bài tập", Pronunciation = "munje", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "해결하다", VietnameseMeaning = "Giải quyết", Pronunciation = "haegyeolhada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "노력하다", VietnameseMeaning = "Nỗ lực", Pronunciation = "noryeokhada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "성공하다", VietnameseMeaning = "Thành công", Pronunciation = "seonggonghada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "실패하다", VietnameseMeaning = "Thất bại", Pronunciation = "silpaehada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "도전하다", VietnameseMeaning = "Thử thách", Pronunciation = "dojeonhada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "배우다", VietnameseMeaning = "Học", Pronunciation = "baeuda", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "가르치다", VietnameseMeaning = "Dạy", Pronunciation = "gareuchida", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "기억하다", VietnameseMeaning = "Nhớ", Pronunciation = "gieokhada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "잊어버리다", VietnameseMeaning = "Quên", Pronunciation = "ijeobeorida", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "생각하다", VietnameseMeaning = "Suy nghĩ", Pronunciation = "saenggakhada", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "느끼다", VietnameseMeaning = "Cảm nhận", Pronunciation = "neukkida", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "느낌", VietnameseMeaning = "Cảm giác", Pronunciation = "neukkim", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "기분", VietnameseMeaning = "Tâm trạng", Pronunciation = "gibun", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "감정", VietnameseMeaning = "Cảm xúc", Pronunciation = "gamjeong", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "관심", VietnameseMeaning = "Quan tâm", Pronunciation = "gwansim", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "관심있다", VietnameseMeaning = "Quan tâm", Pronunciation = "gwansimitda", Category = "", DifficultyLevel = 3, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "취미", VietnameseMeaning = "Sở thích", Pronunciation = "chwimi", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "여행", VietnameseMeaning = "Du lịch", Pronunciation = "yeohaeng", Category = "", DifficultyLevel = 3 },
                new VocabularyWord { KoreanWord = "여행하다", VietnameseMeaning = "Đi du lịch", Pronunciation = "yeohaenghada", Category = "", DifficultyLevel = 3 },

                // TOPIK 4 - Trung cấp nâng cao
                new VocabularyWord { KoreanWord = "능력", VietnameseMeaning = "Năng lực", Pronunciation = "neungnyeok", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "실력", VietnameseMeaning = "Thực lực", Pronunciation = "sillyeok", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "실천하다", VietnameseMeaning = "Thực hành", Pronunciation = "silcheonhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "연습하다", VietnameseMeaning = "Luyện tập", Pronunciation = "yeonseuphada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "개발하다", VietnameseMeaning = "Phát triển", Pronunciation = "gaebalhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "발전하다", VietnameseMeaning = "Phát triển", Pronunciation = "baljeonhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "향상하다", VietnameseMeaning = "Cải thiện", Pronunciation = "hyangsanghada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "개선하다", VietnameseMeaning = "Cải thiện", Pronunciation = "gaeseonhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "변화", VietnameseMeaning = "Thay đổi", Pronunciation = "byeonhwa", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "변화하다", VietnameseMeaning = "Thay đổi", Pronunciation = "byeonhwahada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "영향", VietnameseMeaning = "Ảnh hưởng", Pronunciation = "yeonghyang", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "영향을 주다", VietnameseMeaning = "Gây ảnh hưởng", Pronunciation = "yeonghyangeul juda", Category = "", DifficultyLevel = 4, WordType = "동사" },
                new VocabularyWord { KoreanWord = "의견", VietnameseMeaning = "Ý kiến", Pronunciation = "uigyeon", Category = "", DifficultyLevel = 4, WordType = "명사" },
                new VocabularyWord { KoreanWord = "의견을 말하다", VietnameseMeaning = "Nói ý kiến", Pronunciation = "uigyeoneul malhada", Category = "", DifficultyLevel = 4, WordType = "동사" },
                new VocabularyWord { KoreanWord = "토론하다", VietnameseMeaning = "Thảo luận", Pronunciation = "toronhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "논의하다", VietnameseMeaning = "Thảo luận", Pronunciation = "nonuihada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "협력하다", VietnameseMeaning = "Hợp tác", Pronunciation = "hyeomnyeokhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "협조하다", VietnameseMeaning = "Hợp tác", Pronunciation = "hyeopjohada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "소통하다", VietnameseMeaning = "Giao tiếp", Pronunciation = "sotonghada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "소통", VietnameseMeaning = "Giao tiếp", Pronunciation = "sotong", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "의사소통", VietnameseMeaning = "Giao tiếp", Pronunciation = "uisasotong", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "전문가", VietnameseMeaning = "Chuyên gia", Pronunciation = "jeonmunga", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "전문", VietnameseMeaning = "Chuyên môn", Pronunciation = "jeonmun", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "전문적", VietnameseMeaning = "Chuyên nghiệp", Pronunciation = "jeonmunjeok", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "전문성", VietnameseMeaning = "Tính chuyên nghiệp", Pronunciation = "jeonmunseong", Category = "", DifficultyLevel = 4, WordType = "명사" },
                new VocabularyWord { KoreanWord = "자격", VietnameseMeaning = "Tư cách, bằng cấp", Pronunciation = "jagyeok", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "자격증", VietnameseMeaning = "Chứng chỉ", Pronunciation = "jagyeokjeung", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "경력", VietnameseMeaning = "Kinh nghiệm làm việc, sự nghiệp", Pronunciation = "gyeongnyeok", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "직업", VietnameseMeaning = "Nghề nghiệp", Pronunciation = "jigeop", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "직장", VietnameseMeaning = "Nơi làm việc", Pronunciation = "jikjang", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "회사", VietnameseMeaning = "Công ty", Pronunciation = "hoesa", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "사업", VietnameseMeaning = "Kinh doanh", Pronunciation = "saeop", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "사업하다", VietnameseMeaning = "Kinh doanh", Pronunciation = "saeophada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "경영", VietnameseMeaning = "Quản lý", Pronunciation = "gyeongyeong", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "경영하다", VietnameseMeaning = "Quản lý", Pronunciation = "gyeongyeonghada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "관리하다", VietnameseMeaning = "Quản lý", Pronunciation = "gwallihada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "조직", VietnameseMeaning = "Tổ chức", Pronunciation = "jojik", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "조직하다", VietnameseMeaning = "Tổ chức", Pronunciation = "jojikhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "시스템", VietnameseMeaning = "Hệ thống", Pronunciation = "siseutem", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "체계", VietnameseMeaning = "Hệ thống", Pronunciation = "chegye", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "과정", VietnameseMeaning = "Quá trình", Pronunciation = "gwajeong", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "절차", VietnameseMeaning = "Thủ tục", Pronunciation = "jeolcha", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "방법", VietnameseMeaning = "Phương pháp", Pronunciation = "bangbeop", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "방식", VietnameseMeaning = "Cách thức", Pronunciation = "bangsik", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "전략", VietnameseMeaning = "Chiến lược", Pronunciation = "jeollyak", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "전략적", VietnameseMeaning = "Mang tính chiến lược", Pronunciation = "jeollyakjeok", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "효과", VietnameseMeaning = "Hiệu quả", Pronunciation = "hyogwa", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "효과적", VietnameseMeaning = "Hiệu quả", Pronunciation = "hyogwajeok", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "효율", VietnameseMeaning = "Hiệu suất", Pronunciation = "hyoyul", Category = "", DifficultyLevel = 4, WordType = "명사" },
                new VocabularyWord { KoreanWord = "효율적", VietnameseMeaning = "Hiệu quả", Pronunciation = "hyoyuljeok", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "생산성", VietnameseMeaning = "Năng suất", Pronunciation = "saengsanseong", Category = "", DifficultyLevel = 4, WordType = "명사" },
                new VocabularyWord { KoreanWord = "생산하다", VietnameseMeaning = "Sản xuất", Pronunciation = "saengsanhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "제품", VietnameseMeaning = "Sản phẩm", Pronunciation = "jepum", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "서비스", VietnameseMeaning = "Dịch vụ", Pronunciation = "seobiseu", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "고객", VietnameseMeaning = "Khách hàng", Pronunciation = "gogaek", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "고객서비스", VietnameseMeaning = "Dịch vụ khách hàng", Pronunciation = "gogaekseobiseu", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "만족", VietnameseMeaning = "Hài lòng", Pronunciation = "manjok", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "만족하다", VietnameseMeaning = "Hài lòng", Pronunciation = "manjokhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "만족도", VietnameseMeaning = "Mức độ hài lòng", Pronunciation = "manjokdo", Category = "", DifficultyLevel = 4, WordType = "명사" },
                new VocabularyWord { KoreanWord = "품질", VietnameseMeaning = "Chất lượng", Pronunciation = "pumjil", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "품질관리", VietnameseMeaning = "Quản lý chất lượng", Pronunciation = "pumjilgwalli", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "표준", VietnameseMeaning = "Tiêu chuẩn", Pronunciation = "pyojun", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "기준", VietnameseMeaning = "Tiêu chuẩn", Pronunciation = "gijun", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "평가", VietnameseMeaning = "Đánh giá", Pronunciation = "pyeongga", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "평가하다", VietnameseMeaning = "Đánh giá", Pronunciation = "pyeonggahada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "검토하다", VietnameseMeaning = "Xem xét", Pronunciation = "geomtohada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "검토", VietnameseMeaning = "Sự xem xét", Pronunciation = "geomto", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "분석하다", VietnameseMeaning = "Phân tích", Pronunciation = "bunseokhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "분석", VietnameseMeaning = "Phân tích", Pronunciation = "bunseok", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "연구", VietnameseMeaning = "Nghiên cứu", Pronunciation = "yeongu", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "연구하다", VietnameseMeaning = "Nghiên cứu", Pronunciation = "yeonguhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "조사", VietnameseMeaning = "Điều tra", Pronunciation = "josa", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "조사하다", VietnameseMeaning = "Điều tra", Pronunciation = "josahada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "실험", VietnameseMeaning = "Thí nghiệm", Pronunciation = "silheom", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "실험하다", VietnameseMeaning = "Thí nghiệm", Pronunciation = "silheomhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "테스트", VietnameseMeaning = "Kiểm tra", Pronunciation = "teseuteu", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "시험", VietnameseMeaning = "Kỳ thi", Pronunciation = "siheom", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "시험을 보다", VietnameseMeaning = "Thi", Pronunciation = "siheomeul boda", Category = "", DifficultyLevel = 4, WordType = "동사" },
                new VocabularyWord { KoreanWord = "합격하다", VietnameseMeaning = "Đỗ, đậu", Pronunciation = "hapgyeokhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "불합격하다", VietnameseMeaning = "Trượt", Pronunciation = "bulhapgyeokhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "합격", VietnameseMeaning = "Sự đỗ", Pronunciation = "hapgyeok", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "불합격", VietnameseMeaning = "Sự trượt", Pronunciation = "bulhapgyeok", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "점수", VietnameseMeaning = "Điểm số", Pronunciation = "jeomsu", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "성적", VietnameseMeaning = "Thành tích", Pronunciation = "seongjeok", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "결과", VietnameseMeaning = "Kết quả", Pronunciation = "gyeolgwa", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "성과", VietnameseMeaning = "Thành quả", Pronunciation = "seonggwa", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "성취", VietnameseMeaning = "Thành tựu", Pronunciation = "seongchwi", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "성취하다", VietnameseMeaning = "Đạt được", Pronunciation = "seongchwihada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "달성하다", VietnameseMeaning = "Đạt được", Pronunciation = "dalseonghada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "달성", VietnameseMeaning = "Sự đạt được", Pronunciation = "dalseong", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "목표를 달성하다", VietnameseMeaning = "Đạt mục tiêu", Pronunciation = "mokpyoreul dalseonghada", Category = "", DifficultyLevel = 4, WordType = "동사" },
                new VocabularyWord { KoreanWord = "도달하다", VietnameseMeaning = "Đạt đến", Pronunciation = "dodalhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "도달", VietnameseMeaning = "Sự đạt đến", Pronunciation = "dodal", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "도전", VietnameseMeaning = "Thử thách", Pronunciation = "dojeon", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "도전하다", VietnameseMeaning = "Thử thách", Pronunciation = "dojeonhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "장애물", VietnameseMeaning = "Vật cản", Pronunciation = "jang-aemul", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "장애", VietnameseMeaning = "Trở ngại", Pronunciation = "jang-ae", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "어려움", VietnameseMeaning = "Khó khăn", Pronunciation = "eoryeoum", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "곤란", VietnameseMeaning = "Khó khăn", Pronunciation = "gollan", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "곤란하다", VietnameseMeaning = "Gặp khó khăn", Pronunciation = "gollanhada", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "어렵다", VietnameseMeaning = "Khó", Pronunciation = "eoryeopda", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "쉽다", VietnameseMeaning = "Dễ", Pronunciation = "swipda", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "용이하다", VietnameseMeaning = "Dễ dàng", Pronunciation = "yongihada", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "편리하다", VietnameseMeaning = "Tiện lợi", Pronunciation = "pyeollihada", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "불편하다", VietnameseMeaning = "Bất tiện", Pronunciation = "bulpyeonhada", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "편리함", VietnameseMeaning = "Sự tiện lợi", Pronunciation = "pyeolliham", Category = "", DifficultyLevel = 4, WordType = "명사" },
                new VocabularyWord { KoreanWord = "불편함", VietnameseMeaning = "Sự bất tiện", Pronunciation = "bulpyeonham", Category = "", DifficultyLevel = 4, WordType = "명사" },
                new VocabularyWord { KoreanWord = "편의", VietnameseMeaning = "Tiện lợi", Pronunciation = "pyeonui", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "불편", VietnameseMeaning = "Bất tiện", Pronunciation = "bulpyeon", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "편의시설", VietnameseMeaning = "Tiện ích", Pronunciation = "pyeonuiseol", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "시설", VietnameseMeaning = "Cơ sở vật chất", Pronunciation = "siseol", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "시설물", VietnameseMeaning = "Cơ sở vật chất", Pronunciation = "siseolmul", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "건물", VietnameseMeaning = "Tòa nhà", Pronunciation = "geonmul", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "건축", VietnameseMeaning = "Kiến trúc", Pronunciation = "geonchuk", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "건축하다", VietnameseMeaning = "Xây dựng", Pronunciation = "geonchukhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "건설", VietnameseMeaning = "Xây dựng", Pronunciation = "geonseol", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "건설하다", VietnameseMeaning = "Xây dựng", Pronunciation = "geonseolhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "설계", VietnameseMeaning = "Thiết kế", Pronunciation = "seolgye", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "설계하다", VietnameseMeaning = "Thiết kế", Pronunciation = "seolgyehada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "디자인", VietnameseMeaning = "Thiết kế", Pronunciation = "dijain", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "디자인하다", VietnameseMeaning = "Thiết kế", Pronunciation = "dijainhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "예술", VietnameseMeaning = "Nghệ thuật", Pronunciation = "yesul", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "예술가", VietnameseMeaning = "Nghệ sĩ", Pronunciation = "yesulga", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "작품", VietnameseMeaning = "Tác phẩm", Pronunciation = "jakpum", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "창작", VietnameseMeaning = "Sáng tạo", Pronunciation = "changjak", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "창작하다", VietnameseMeaning = "Sáng tạo", Pronunciation = "changjakhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "창의력", VietnameseMeaning = "Khả năng sáng tạo", Pronunciation = "changuiryeok", Category = "", DifficultyLevel = 4, WordType = "명사" },
                new VocabularyWord { KoreanWord = "창의적", VietnameseMeaning = "Sáng tạo", Pronunciation = "changuijeok", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "창의성", VietnameseMeaning = "Tính sáng tạo", Pronunciation = "changuiseong", Category = "", DifficultyLevel = 4, WordType = "명사" },
                new VocabularyWord { KoreanWord = "혁신", VietnameseMeaning = "Đổi mới", Pronunciation = "hyeoksin", Category = "", DifficultyLevel = 4, WordType = "명사" },
                new VocabularyWord { KoreanWord = "혁신하다", VietnameseMeaning = "Đổi mới", Pronunciation = "hyeoksinhada", Category = "", DifficultyLevel = 4, WordType = "동사" },
                new VocabularyWord { KoreanWord = "혁신적", VietnameseMeaning = "Mang tính đổi mới", Pronunciation = "hyeoksinjeok", Category = "", DifficultyLevel = 4, WordType = "형용사" },
                new VocabularyWord { KoreanWord = "혁신성", VietnameseMeaning = "Tính đổi mới", Pronunciation = "hyeoksinseong", Category = "", DifficultyLevel = 4, WordType = "명사" },
                new VocabularyWord { KoreanWord = "개선", VietnameseMeaning = "Cải thiện", Pronunciation = "gaeseon", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "개선하다", VietnameseMeaning = "Cải thiện", Pronunciation = "gaeseonhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "향상", VietnameseMeaning = "Cải thiện, sự cải thiện", Pronunciation = "hyangsang", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "향상하다", VietnameseMeaning = "Cải thiện", Pronunciation = "hyangsanghada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "증가", VietnameseMeaning = "Tăng", Pronunciation = "jeungga", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "증가하다", VietnameseMeaning = "Tăng", Pronunciation = "jeunggahada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "감소", VietnameseMeaning = "Giảm", Pronunciation = "gamso", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "감소하다", VietnameseMeaning = "Giảm", Pronunciation = "gamsohada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "증대", VietnameseMeaning = "Mở rộng", Pronunciation = "jeungdae", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "증대하다", VietnameseMeaning = "Mở rộng", Pronunciation = "jeungdaehada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "확대", VietnameseMeaning = "Mở rộng", Pronunciation = "hwakdae", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "확대하다", VietnameseMeaning = "Mở rộng", Pronunciation = "hwakdaehada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "축소", VietnameseMeaning = "Thu nhỏ, thu hẹp", Pronunciation = "chukso", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "축소하다", VietnameseMeaning = "Thu nhỏ, thu hẹp", Pronunciation = "chuksohada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "확장", VietnameseMeaning = "Mở rộng", Pronunciation = "hwakjang", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "확장하다", VietnameseMeaning = "Mở rộng", Pronunciation = "hwakjanghada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "확산", VietnameseMeaning = "Lan rộng", Pronunciation = "hwaksan", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "확산하다", VietnameseMeaning = "Lan rộng", Pronunciation = "hwaksanhada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "보급", VietnameseMeaning = "Phổ biến", Pronunciation = "bogeup", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "보급하다", VietnameseMeaning = "Phổ biến", Pronunciation = "bogeupada", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "보급률", VietnameseMeaning = "Tỷ lệ phổ biến", Pronunciation = "bogeupnyul", Category = "", DifficultyLevel = 4 },
                new VocabularyWord { KoreanWord = "보급도", VietnameseMeaning = "Mức độ phổ biến", Pronunciation = "bogeupdo", Category = "", DifficultyLevel = 4 },
            };
        }
    }
}

