# ♟️ CHESS EVOLUTION: PROJECT LOG
Dự án biến thể Cờ Vua với cơ chế tích lũy điểm số và hệ thống tiến hóa kỹ năng.

---

# 📜 LORE

### 🛡️ Chapter 1: Painful Pawn
Xuyên suốt lịch sử cờ vua, chỉ có 1 danh phận là tốt thí. Đôi khi được gọi là chiến binh, nhưng bản chất vẫn không khác mấy. Tầng lớp này đại diện cho lính cấp thấp hay thậm chí là nông dân ở những phiên bản đầu tiên.

Ở phiên này, Pawn vẫn là tốt thí. Tuy nhiên họ có 1 ý chí cầu tiến, muốn đổi số phận của mình.

Khi có đủ Chiến khí trên chiến trường, họ sẽ hấp thụ khả năng của một quân cấp cao nếu kết liễu được quân đó.

### ⛪ Chapter 2: Big Bishop
Thủa đầu, Bishop là Voi. Sau đó du nhập qua Châu Âu thành Tu sĩ/Linh mục hay tướng lĩnh.

Ở phiên bản này, Bishop là tiểu tướng lĩnh.

Khi có đủ kinh nghiệm trên chiến trường, họ sẽ trở thành đại tướng lĩnh với khả năng và ngoại hình mới.

### 🐴 Chapter 3: Royal Knight
Như tên gọi, Knight là Kỵ sĩ. Nhờ ngoại hình của mình đôi khi được gọi là Ngựa ở một số phiên bản.

Ở phiên bản này, Knight là Kỵ sĩ.

Khi có đủ kinh nghiệm trên chiến trường, họ sẽ trở thành Kỵ sĩ Hoàng Gia với khả năng và ngoại hình mới.

### 🏰 Chapter 4: Rook
Rook đại diện cho xe ngựa, xe công thành, hoặc pháo đài qua nhiều phiên bản.
Ở phiên bản này, Rook là xe công thành.
Khi có đủ điều kiện, Rook sẽ trở thành xe chiến ý, khích lệ tinh thần cho tất cả đồng mình.

### 👸 Chapter 5: Alpha Queen
Từ sơ khai, vị trí của quân cờ này đại diện cho Đại tướng/Tổng tư lệnh. Sau khi du nhập sang Châu Âu thì trở thành Hoàng Hậu.

Ở phiên bản này, Queen là Nữ Tướng lĩnh, vừa là cánh tay phải vừa là Hoàng Hậu của Vua.

Khi có đủ tiếng tăm trên chiến trường, trở thành Nữ Thần Tướng. Khi này tất cả quân cấp cao đồng minh trở thành tín đồ của Nữ Thần Tướng. Khi Nữ Thần Tướng hy sinh, 1 trong các tín đồ có thể tiếp nhận ý chí của Nữ Thần Tướng để trở thành Tân Nữ Thần Tướng.

Mặt khác, khi đồng đội hy sinh quá nhiều. Nữ Tướng sẽ đau buồn và trở thành Nữ Tà Tướng. Nữ Tà Tướng có khả năng càn quét cực mạnh và miễn tử. Tuy nhiên sau đó sẽ chết ngay lập tức.

### 🤴 Chapter 6: King
Mọi phiên bản đều là người đứng đầu. Chết = thua.

Khi Nữ Tà Tướng ở đội đối phương. Vua sẽ cảm thấy bị đe dọa và sử dụng súng của hắn. Nếu tận dụng tốt có thể khắc chế được Nữ Tà Tướng.

---

## 📊 Phân loại cấp bậc và điểm
* **Tốt(1):** Quân nhẹ
* **Tượng(3), Mã(3), Xe(5):** Quân nặng
* **Hậu(9), Vua(0):** Quân triều đình

### ⚔️ Quy tắc hấp thụ (Absorption Rule)
Khi quân **A** ăn quân **B**, quân **A** sẽ cộng dồn toàn bộ số điểm hiện tại của quân **B** vào điểm của chính nó.
> **Ví dụ:** Xe(5) ăn Mã(3) -> Xe(8).

---

## ⚡ Hệ Thống Tiến Hóa (Evolution Mechanics)

Mỗi quân cờ sẽ mở khóa khả năng mới khi đạt đủ điều kiện nhất định:

### 1. ♟️ Tốt (Pawn)
* **Điều kiện:** Ăn được 1 **Quân nặng** với điều kiện vị trí đứng của Tốt ở nửa sân bên kia khi ăn.
* **Khả năng:** Sở hữu khả năng di chuyển của chính nó và quân nặng mà nó vừa ăn.
* **Lưu ý:** Vẫn được tính là quân nhẹ. Nếu ăn thêm quân nặng khác sau khi đã tiến hóa, Tốt sẽ không được cộng thêm khả năng di chuyển mới.

### 2. ⛪ Tượng (Bishop)
* **Điều kiện:** Tích lũy được **5 điểm** trở lên. (Tức cần thêm 2 điểm)
* **Khả năng:** Có thể di chuyển sang các ô **khác màu** (chỉ áp dụng cho các ô nằm cạnh nó).

### 3. 🐴 Mã (Knight)
* **Điều kiện:** Tích lũy được **5 điểm** trở lên. (Tức cần thêm 2 điểm)
* **Khả năng [Euclid Jump]:** Có thể di chuyển tới bất kỳ ô nào cách nó 2 đơn vị (tính theo phần nguyên của khoảng cách Euclid). Tạo thành một vùng di chuyển hình tròn bán kính 2 quanh vị trí hiện tại.

### 4. 🏰 Xe (Rook)
* **Điều kiện:** Tích lũy được **6 điểm** trở lên. (Tức cần thêm 1 điểm)
* **Khả năng:** Nhả ra 1 thẻ Buff mỗi khi kết liễu kẻ địch.

### 5. 👸 Hậu (Queen)
Hậu chỉ được nhận 1 loại khả năng trong suốt ván đấu.
* **Khả năng [Thần Tướng]:** Sau khi tích đủ 15 điểm, có thể chọn **hiến tế** 1 tín đồ đang có trên bàn cờ để hồi sinh tại vị trí của quân đó (Mất 1 lượt để thực hiện). Sau khi hồi sinh, điểm số bị reset về 9.
* **Khả năng [Tà Tướng]:** Sau khi đối phương có Nữ Thần Tướng. Có thể chọn **cuồng sát** thành Nữ Tà Tướng, có thêm khả năng di chuyển của mã, điểm reset về 0, miễn trừ và nhận điểm, miễn tử trong 4 hiệp(nếu bị tấn công, sẽ phản công). Hết 4 hiệp lập tức hy sinh. Trong thời gian miễn tử, bị bắn đạn bởi Dân Chủ sẽ chết lập tức.

> *Tà Tướng sinh ra để bên yếu thế dành lại lợi thế.*

### 6. 🤴 Vua (King)
* **Điều kiện:** Hậu đối thủ tiến hóa thành Tà Tướng == Hậu mình biến thành Nữ Thần Tướng
* **Khả năng [Dân chủ]:** Vua nhận được một khẩu súng có 1 viên đạn, có thể kết liễu 1 đối tượng trong tầm. Trong trường hợp bắn trúng Nữ Tà Tướng, lập tức giết ả. Mỗi 5 điểm mà Vua nhận được sẽ hồi 1 viên đạn cho súng.

---

## 🧧 Hệ Thống Thẻ Bài 

### 🟢 Buff
Gồm 2 thẻ +1 và +2. Cộng điểm quân đồng minh chỉ định. Lá này sẽ xuất hiện khi bị thất thế hoặc xe tiến hóa đồng minh ăn được quân địch.

### 🔴 Debuff
Gồm 1 thẻ là sét. Giáng cấp 1 quân không phải Dân Chủ và Nữ Thần/Tà Tướng.

### 👼 GodQueen
Hồi sinh Nữ Thần Tướng chỉ định vào tín đồ của nàng.

### 👿 DemonQueen
Biến hóa Nữ Tướng thành Nữ Tà Tướng.

### 🗺️ Event
Gồm 2 thẻ mở rộng map, 1 hàng hoặc 1 cột.

### 🔫 Gun
Trao súng cho Vua.
