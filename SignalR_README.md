# SignalR Real-time Implementation

## Tổng quan
Dự án đã được tích hợp SignalR để cung cấp tính năng real-time cho tất cả các CRUD operations và hiển thị dữ liệu.

## Các tính năng Real-time đã implement

### 1. News Articles
- **Create**: Thông báo real-time khi có bài viết mới được tạo
- **Update**: Cập nhật real-time khi bài viết được chỉnh sửa
- **Delete**: Thông báo và xóa khỏi danh sách real-time
- **Status Change**: Cập nhật trạng thái Published/Draft real-time

### 2. Categories
- **Create**: Thông báo real-time khi có category mới
- **Update**: Cập nhật thông tin category real-time
- **Delete**: Xóa category khỏi danh sách real-time
- **Navigation Update**: Cập nhật menu navigation khi có thay đổi category

### 3. System Accounts
- **Create**: Thông báo khi có tài khoản mới được tạo
- **Update**: Cập nhật thông tin tài khoản real-time
- **Delete**: Thông báo xóa tài khoản và force logout user bị xóa
- **Login/Logout**: Hiển thị trạng thái online/offline của users

## Cấu trúc SignalR

### Hubs
- `NewsArticleHub`: Xử lý real-time cho News Articles
- `CategoryHub`: Xử lý real-time cho Categories
- `SystemAccountHub`: Xử lý real-time cho System Accounts
- `BaseHub`: Base class chung với authentication logic

### DTOs
- `NewsArticleDto`: Data transfer object cho News Articles
- `CategoryDto`: Data transfer object cho Categories
- `SystemAccountDto`: Data transfer object cho System Accounts

### Services
- `SignalRNotificationService`: Service tập trung để gửi SignalR notifications
- Các Service đã được tích hợp SignalR: `NewsArticleService`, `CategoryService`, `SystemAccountService`

### JavaScript Client
- `signalr-client.js`: Base SignalR connection management
- `newsarticle-realtime.js`: Real-time logic cho News Articles
- `category-realtime.js`: Real-time logic cho Categories
- `account-realtime.js`: Real-time logic cho System Accounts

## Cách sử dụng

### 1. Test SignalR
- Đăng nhập với tài khoản Admin
- Truy cập `/Test/SignalRTest` để test các tính năng real-time
- Mở nhiều tab browser để thấy real-time updates

### 2. Real-time trong các trang chính
- **News Articles Index**: Tự động cập nhật danh sách khi có CRUD operations
- **Categories Index**: Tự động cập nhật danh sách categories
- **Accounts Index**: Hiển thị trạng thái online/offline của users

### 3. Notifications
- Tất cả real-time updates đều có notifications hiển thị ở góc phải màn hình
- Notifications tự động biến mất sau 5 giây

## Permissions và Groups

### SignalR Groups
- `NewsArticles`: Tất cả users đã login
- `Categories`: Chỉ Admin users
- `SystemAccounts`: Chỉ Admin users
- `Category_{id}`: Users quan tâm đến category cụ thể
- `User_{id}`: User cụ thể để nhận notifications cá nhân
- `Role_{role}`: Users theo role

### Authentication
- Chỉ authenticated users mới có thể kết nối SignalR
- Permissions được kiểm tra trong BaseHub
- Auto-join groups dựa trên user role

## Cấu hình

### Program.cs
```csharp
// Add SignalR
builder.Services.AddSignalR();

// Register SignalR service
builder.Services.AddScoped<ISignalRNotificationService, SignalRNotificationService>();

// Map SignalR Hubs
app.MapHub<NewsArticleHub>("/newsArticleHub");
app.MapHub<CategoryHub>("/categoryHub");
app.MapHub<SystemAccountHub>("/systemAccountHub");
```

### Layout.cshtml
- SignalR JavaScript libraries được load cho authenticated users
- Auto-initialize connections khi page load

## Error Handling
- Tất cả SignalR operations đều có try-catch
- Lỗi SignalR không làm fail các CRUD operations
- Reconnection logic tự động khi mất kết nối

## Performance Considerations
- SignalR connections được quản lý hiệu quả
- Groups được sử dụng để giảm traffic không cần thiết
- Cleanup connections khi user logout hoặc close browser

## Troubleshooting

### Kiểm tra kết nối
1. Mở Developer Tools > Console
2. Kiểm tra log "SignalR connections established successfully"
3. Nếu có lỗi, kiểm tra network tab để xem WebSocket connections

### Test real-time
1. Mở 2 browser tabs với cùng trang
2. Thực hiện CRUD operation ở tab 1
3. Kiểm tra real-time update ở tab 2

### Common Issues
- **Connection failed**: Kiểm tra authentication status
- **No real-time updates**: Kiểm tra JavaScript console errors
- **Permission denied**: Kiểm tra user role và permissions
