# Ứng dụng Học Từ vựng Tiếng Hàn

Ứng dụng Android được xây dựng bằng C# MAUI để ghi chú và học từ vựng tiếng Hàn. Dữ liệu được lưu trữ cục bộ trên thiết bị.

## Tính năng

- 📝 **Quản lý từ vựng**: Thêm, sửa, xóa từ vựng tiếng Hàn với đầy đủ thông tin
  - Kiểm tra từ vựng trùng khi thêm mới
  - Cảnh báo nếu từ đã tồn tại với tùy chọn tiếp tục hoặc hủy
- 🔍 **Tìm kiếm**: Tìm kiếm từ vựng theo từ khóa (tiếng Hàn, tiếng Việt, phát âm)
  - Tìm kiếm real-time khi gõ
- 📚 **Quản lý danh mục**: 
  - Thêm, sửa, xóa danh mục
  - Chọn màu sắc tự do bằng RGB sliders (Red, Green, Blue)
  - Sắp xếp lại thứ tự danh mục (lên/xuống)
  - Sidebar danh mục có thể ẩn/hiện
  - Phân loại từ vựng theo danh mục (TOPIK 1-4, Yêu thích, v.v.)
- 🏷️ **Loại từ**: Phân loại theo loại từ (Danh từ, Động từ, Tính từ, v.v.)
- ⭐ **Yêu thích**: Đánh dấu từ vựng yêu thích (ngôi sao vàng khi yêu thích, xám khi chưa)
- 📖 **Học tập thông minh**: 
  - Chọn số lượng từ không giới hạn
  - 2 chế độ học: Tự động, Ngẫu nhiên
  - Lọc theo danh mục và loại từ
  - Kiểm tra đáp án và theo dõi tiến độ
  - Hiển thị tiến độ học tập (số từ đúng/tổng số từ)
- 📥 **Import TOPIK**: Nhập sẵn hơn 200 từ vựng TOPIK 1-4
- 💾 **Lưu trữ local**: Dữ liệu được lưu trong SQLite database trên thiết bị
- 📊 **Thống kê học tập**: Theo dõi số lần học, số lần đúng, ngày học cuối

## Cấu trúc dự án

```
Korean_Vocabulary_new/
├── Models/              # Models cho VocabularyWord, Category, WordTypeHelper
├── Services/            # DatabaseService, TopikDataService để quản lý SQLite
├── ViewModels/          # ViewModels theo pattern MVVM
│   ├── VocabularyListViewModel
│   ├── AddEditViewModel
│   ├── StudyViewModel
│   ├── StudySettingsViewModel
│   ├── CategoryListViewModel
│   └── AddEditCategoryViewModel
├── Pages/               # Các trang XAML
│   ├── VocabularyListPage
│   ├── AddEditPage
│   ├── StudyPage
│   ├── StudySettingsPage
│   ├── CategoryListPage
│   └── AddEditCategoryPage
├── Converters/          # Value converters cho XAML
└── Resources/           # Tài nguyên (hình ảnh, fonts, styles)
    ├── Icon/            # Icon PNG cho các nút chức năng
    ├── AppIcon/         # Icon ứng dụng
    └── Splash/          # Splash screen
```

## Yêu cầu

- .NET 8.0
- Visual Studio 2022 hoặc Visual Studio Code với MAUI extension
- Android SDK (API 21 trở lên)

## Cài đặt

1. Mở solution file `Korean_Vocabulary_new.sln`
2. Restore NuGet packages
3. Chọn target platform là Android
4. Build và chạy ứng dụng

## Sử dụng

### Thêm từ vựng mới
1. Nhấn nút "➕ Thêm từ" ở màn hình danh sách
2. Nhập từ tiếng Hàn và nghĩa tiếng Việt (bắt buộc)
3. Có thể thêm:
   - Phát âm (Romanization)
   - Loại từ (Danh từ, Động từ, Tính từ, v.v.)
   - Danh mục
   - Câu ví dụ và bản dịch
   - Độ khó (1-5)
   - Đánh dấu yêu thích
4. Nhấn "Lưu" để lưu từ vựng
5. **Kiểm tra trùng**: Hệ thống tự động kiểm tra nếu từ vựng đã tồn tại
   - Nếu trùng, hiển thị cảnh báo với nghĩa hiện tại
   - Chọn "Tiếp tục" để lưu hoặc "Hủy" để quay lại

### Import dữ liệu TOPIK
1. Nhấn nút "📥 TOPIK" ở màn hình danh sách
2. Xác nhận import
3. Hệ thống sẽ tự động thêm hơn 200 từ vựng TOPIK 1-4 vào database
4. Các từ vựng được phân loại theo TOPIK level và loại từ tự động

### Học tập
1. Nhấn nút "📚 Học tập" ở màn hình danh sách
2. **Cài đặt học tập**:
   - **Số lượng từ**: Nhập số từ muốn học (không giới hạn)
   - **Chế độ học**:
     - **Tự động**: Hệ thống tự chọn từ cần ôn lại (ưu tiên từ có tỷ lệ đúng thấp)
     - **Ngẫu nhiên**: Chọn từ ngẫu nhiên từ database
   - **Lọc** (tùy chọn): Lọc theo danh mục hoặc loại từ
3. Nhấn "Bắt đầu học"
4. **Quá trình học**:
   - Xem từ tiếng Hàn, phát âm, loại từ
   - Nhập nghĩa tiếng Việt
   - Nhấn "Kiểm tra" để xem kết quả
   - Nhấn "Hiển thị đáp án" để xem đáp án ngay
   - Nhấn "Từ tiếp theo" để chuyển sang từ tiếp theo
5. Xem kết quả cuối cùng: Số từ đúng/tổng số từ

### Tìm kiếm
- Sử dụng thanh tìm kiếm ở đầu màn hình để tìm từ vựng
- Tìm kiếm theo: từ tiếng Hàn, nghĩa tiếng Việt, hoặc phát âm

### Quản lý danh mục
- **Xem danh mục**: Nhấn nút menu (☰) ở header để hiện/ẩn sidebar danh mục
- **Chọn danh mục**: Tap vào danh mục trong sidebar để lọc từ vựng
- **Thêm danh mục mới**:
  1. Nhấn nút Settings (⚙️) trong sidebar danh mục
  2. Nhấn nút "➕ Thêm" ở màn hình quản lý danh mục
  3. Nhập tên danh mục
  4. Chọn màu bằng cách điều chỉnh RGB sliders (Red, Green, Blue)
  5. Xem preview màu real-time
  6. Nhấn "Lưu"
- **Sửa danh mục**: 
  1. Vào màn hình quản lý danh mục
  2. Tap vào danh mục hoặc nhấn nút Edit
  3. Sửa tên và màu sắc
  4. Nhấn "Lưu"
- **Xóa danh mục**: 
  1. Vào màn hình quản lý danh mục
  2. Nhấn nút Delete bên cạnh danh mục
  3. Xác nhận xóa
  4. Các từ vựng trong danh mục sẽ được chuyển về "Tất cả"
- **Sắp xếp lại danh mục**:
  1. Vào màn hình quản lý danh mục
  2. Nhấn nút ↑ để di chuyển lên
  3. Nhấn nút ↓ để di chuyển xuống
- **Danh mục mặc định**: "Tất cả", "Yêu thích", "Mới học", "Cần ôn lại" (không thể xóa)
- **Danh mục TOPIK**: "TOPIK 1", "TOPIK 2", "TOPIK 3", "TOPIK 4" (sau khi import)

### Yêu thích
- Nhấn ngôi sao ⭐ bên cạnh từ vựng để đánh dấu yêu thích
- Ngôi sao xám: Chưa yêu thích
- Ngôi sao vàng: Đã yêu thích
- Xem từ yêu thích: Chọn danh mục "Yêu thích"
- Trạng thái yêu thích được lưu và cập nhật real-time

### Xóa từ vựng
- Nhấn nút Delete (🗑️) bên cạnh từ vựng
- Xác nhận xóa
- Từ vựng sẽ bị xóa vĩnh viễn khỏi database

## Công nghệ sử dụng

- **.NET MAUI**: Framework đa nền tảng
- **SQLite**: Database local để lưu trữ dữ liệu
- **MVVM Pattern**: Kiến trúc ứng dụng
- **XAML**: Giao diện người dùng

## Ghi chú

- **Dữ liệu**: Được lưu trong file `korean_vocabulary.db` trong thư mục AppData của ứng dụng
- **Danh mục mặc định**: Ứng dụng tự động tạo các danh mục mặc định khi lần đầu chạy
  - Các danh mục mặc định không thể xóa
  - Có thể sắp xếp lại thứ tự danh mục
- **Màu sắc danh mục**: 
  - Chọn màu tự do bằng RGB sliders (0-255 cho mỗi màu)
  - Preview màu real-time khi điều chỉnh
  - Màu được lưu dưới dạng hex code (#RRGGBB)
- **Thống kê học tập**: 
  - Số lần học (StudyCount)
  - Số lần đúng (CorrectCount)
  - Ngày học cuối (LastStudiedDate)
  - Tự động cập nhật khi học tập
- **Loại từ**: Hỗ trợ 9 loại từ tiếng Hàn (명사, 동사, 형용사, 부사, 대명사, 조사, 감탄사, 수사, 관형사)
- **TOPIK Data**: Bao gồm hơn 200 từ vựng từ TOPIK 1-4, được phân loại và gán loại từ tự động
- **Chế độ học**: 
  - Tự động: Ưu tiên từ chưa học hoặc có tỷ lệ đúng < 70%
  - Ngẫu nhiên: Chọn ngẫu nhiên từ database
- **Kiểm tra trùng**: 
  - Tự động kiểm tra khi thêm từ vựng mới
  - Kiểm tra khi sửa từ vựng (trừ chính từ đang sửa)
  - Hiển thị cảnh báo với nghĩa hiện tại nếu trùng
- **Giao diện**:
  - Sidebar danh mục có thể ẩn/hiện bằng nút menu (☰)
  - Sử dụng icon PNG thay vì emoji
  - Layout danh mục dọc với border và background

## Tính năng nổi bật

✨ **Học tập thông minh**: Hệ thống tự động đề xuất từ vựng cần ôn lại dựa trên thống kê học tập

🎯 **Linh hoạt**: Chọn số lượng từ không giới hạn, học theo cách bạn muốn

📚 **Dữ liệu phong phú**: Import sẵn từ vựng TOPIK 1-4, hoặc tự thêm từ vựng của riêng bạn

🎨 **Giao diện thân thiện**: UI đơn giản, dễ sử dụng, hỗ trợ tiếng Việt hoàn toàn

🌈 **Tùy biến danh mục**: Chọn màu sắc tự do cho danh mục bằng RGB sliders, sắp xếp lại thứ tự theo ý muốn

🔍 **Kiểm tra trùng**: Tự động phát hiện và cảnh báo từ vựng trùng khi thêm mới

📱 **Sidebar linh hoạt**: Danh mục hiển thị trong sidebar có thể ẩn/hiện để tối ưu không gian màn hình

