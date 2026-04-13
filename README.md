# Project: Ecommerce Demo System

Dự án xây dựng hệ thống Backend cho ứng dụng thương mại điện tử, tập trung vào luồng nghiệp vụ từ xác thực, quản lý sản phẩm đến thanh toán và khuyến mãi.

---

## Yêu cầu kỹ thuật chung
- **Tài liệu API:** Cài đặt **Swagger/OpenAPI** để xem danh sách và test API.
- **Bảo mật:** Sử dụng **JWT** và mã hóa mật khẩu **BCrypt**.
- **Dữ liệu:** Hỗ trợ phân trang (Pagination), Transaction cho thanh toán và Soft Delete cho sản phẩm.

---

## Danh sách các Task chi tiết

### Task 1: Authentication & Authorization (Xác thực & Phân quyền)
- **Task 1.1: Register API**
  - **Endpoint:** `POST /api/v1/auth/register`
  - **Yêu cầu:** Nhận thông tin user, kiểm tra email trùng lặp, mã hóa mật khẩu bằng BCrypt trước khi lưu vào DB.
- **Task 1.2: Login API**
  - **Endpoint:** `POST /api/v1/auth/login`
  - **Yêu cầu:** Xác thực thông tin, tạo và trả về JWT Token (chứa `userId`, `username`, `roles`).
- **Task 1.3: Security Middleware/Filter**
  - **Yêu cầu:** Cấu hình phân quyền (Role-based). 
  - *Ví dụ:* Admin mới được thêm sản phẩm, User mới được đặt hàng.

### Task 2: Product Catalog (Quản lý sản phẩm)
- **Task 2.1: List Products API**
  - **Endpoint:** `GET /api/v1/products`
  - **Yêu cầu:** Trả về danh sách sản phẩm có **Pagination** (`page`, `size`) và **Filtering** (theo danh mục, khoảng giá).
- **Task 2.2: Product Details API**
  - **Endpoint:** `GET /api/v1/products/{id}`
  - **Yêu cầu:** Trả về thông tin chi tiết của một sản phẩm.
- **Task 2.3: Admin Product Management (CRUD)**
  - **Endpoints:** `POST`, `PUT`, `DELETE` cho `/api/v1/products`.
  - **Yêu cầu:** Chỉ Admin mới có quyền gọi. Thực hiện **Soft Delete** (đánh dấu xóa, không xóa hẳn khỏi DB).

### Task 3: Shopping Cart (Giỏ hàng)
- **Task 3.1: Add to Cart API**
  - **Endpoint:** `POST /api/v1/cart/add`
  - **Yêu cầu:** Kiểm tra sản phẩm có tồn tại và còn đủ hàng trong kho (Inventory) không trước khi thêm vào giỏ.
- **Task 3.2: View & Update Cart**
  - **Endpoints:** `GET /api/v1/cart`, `PUT /api/v1/cart/items/{id}`.
  - **Yêu cầu:** Lấy `userId` từ Token để truy xuất đúng giỏ hàng của chính người dùng đó.

### Task 4: Order & Checkout (Đặt hàng & Thanh toán)
- **Task 4.1: Checkout API**
  - **Endpoint:** `POST /api/v1/orders/checkout`
  - **Logic yêu cầu (Phải nằm trong 1 Transaction):**
    1. Kiểm tra tồn kho lần cuối cho tất cả item trong giỏ.
    2. Trừ số lượng tồn kho (Sử dụng Locking để tránh race condition).
    3. Tạo bản ghi `Order` và các `OrderItem`.
    4. Xóa giỏ hàng sau khi đặt hàng thành công.
- **Task 4.2: Order History API**
  - **Endpoint:** `GET /api/v1/orders`
  - **Yêu cầu:** Trả về danh sách đơn hàng của User đang đăng nhập.

### Task 5: Promotion & Loyalty (Khuyến mãi & Khách hàng thân thiết)
- **Task 5.1: Coupon Management (Admin)**
  - **Endpoint:** `POST /api/v1/admin/coupons`
  - **Yêu cầu:** Tạo mã giảm giá với các thuộc tính: `code` (duy nhất), `discountType` (PERCENTAGE hoặc FIXED_AMOUNT), `value`, `minOrderValue`, `startDate`, `endDate`, và `usageLimit`.
- **Task 5.2: Apply Coupon Logic**
  - **Endpoint:** `POST /api/v1/cart/apply-coupon`
  - **Yêu cầu:** Kiểm tra tính hợp lệ (hạn dùng, lượt dùng, giá trị đơn tối thiểu). Tính toán số tiền được giảm và lưu vào Session/Database.
- **Task 5.3: Ranking & Loyalty Points**
  - **Tích điểm:** Sau khi đơn hàng chuyển sang trạng thái `COMPLETED`, cộng điểm cho User (VD: 100,000 VNĐ = 1 điểm).
  - **Ranking:** Tự động cập nhật hạng thành viên (Silver, Gold, Diamond) dựa trên tổng điểm tích lũy.
  - **Ưu đãi hạng:** Tự động giảm thêm X% cho mỗi đơn hàng tùy theo Rank của User.

### Task 6: Product Reviews & Rating (Đánh giá sản phẩm)
- **Task 6.1: Post Review API**
  - **Endpoint:** `POST /api/v1/reviews`
  - **Yêu cầu:** Chỉ cho phép User đã mua và nhận hàng thành công (`Order Status = COMPLETED`) mới được đánh giá sản phẩm đó.
  - **Dữ liệu:** `productId`, `rating` (1-5 sao), `comment`.
- **Task 6.2: Review Summary & Display**
  - **Endpoint:** `GET /api/v1/products/{id}/reviews`
  - **Yêu cầu:** Trả về danh sách đánh giá có phân trang và tính toán điểm trung bình (**Average Rating**) của sản phẩm.
