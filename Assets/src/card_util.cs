using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public static class card_util {


    // =========================================================================
    // CARD
    // =========================================================================
    
    public static void init_card_table_white() {
    // Đặt bảng ở vị trí trung tâm hàng bài, Z= -4f (nằm dưới lá bài Z=-5f)
    // Tọa độ Y nên khớp với y_pos của CardHand
    float table_y = data.mem.white_hand_visual.y_pos - 1.1f; 
    
    data.mem.card_table_obj_w = rect_2d.create(0f, table_y, -4f);
    data.mem.card_table_obj_w.set_sprite(data.mem.Card_board_bg_sprite);
    
    // Giả sử bảng của bạn chứa 5 cột, 2 hàng thẻ. 
    // Độ rộng khoảng: 5 cột * 1.6f spacing = 8f. 
    float scale = 1.4f;
    data.mem.card_table_obj_w.set_sprite_size(7.12f * scale, 5.4f * scale);
}
    public static void init_card_table_black() {
    float table_y = data.mem.black_hand_visual.y_pos + 1.1f; // Đặt bảng ở vị trí trung tâm hàng bài, Z= -4f (nằm dưới lá bài Z=-5f)
    
    data.mem.card_table_obj_b   = rect_2d.create(0f, table_y, -4f);
    data.mem.card_table_obj_b.set_sprite(data.mem.Card_board_bg_sprite);
    
    // Giả sử bảng của bạn chứa 5 cột, 2 hàng thẻ. 
    // Độ rộng khoảng: 5 cột * 1.6f spacing = 8f. 
    float scale = 1.4f;
    data.mem.card_table_obj_b.set_sprite_size(7.12f * scale, 5.4f * scale);
}

    public static void draw_debug_card() {
        if (data.mem == null) return;

        // Tạo một dữ liệu thẻ bài mới (ví dụ: thẻ Gun)
        data.Card newCard = new data.Card();
        newCard.cardName = "Gun Item";
        newCard.type = CardType.Item;
        newCard.artwork = data.mem.card_gun; // Gán sprite từ data
        newCard.value = 0;

        // Thêm vào danh sách bài trên tay của quân Trắng
        data.mem.whiteHand.Add(newCard);

        // Vẽ lại hiển thị
        refresh_card_visuals(0); 
    }
public static void refresh_card_visuals(int player_color) {
    var visualData = (player_color == 0) ? data.mem.white_hand_visual : data.mem.black_hand_visual;
    var cardDataList = (player_color == 0) ? data.mem.whiteHand : data.mem.blackHand;

    foreach (var r in visualData.card_rects) {
        if (r != null) r.self_destroy();
    }
    visualData.card_rects.Clear();

    float row_spacing = 2.2f; 

    for (int i = 0; i < cardDataList.Count; i++) {
        int column = i % 4; 
        int row    = i / 4; 

        float x = visualData.start_x + (column * visualData.spacing);
        
        // --- LOGIC SPAWN NGƯỢC ---
        float y;
        if (player_color == 0) {
            // Quân Trắng: Hàng mới thấp dần (trừ Y)
            y = visualData.y_pos - (row * row_spacing); 
        } else {
            // Quân Đen: Hàng mới cao dần (cộng Y)
            y = visualData.y_pos + (row * row_spacing); 
        }
        
        rect_2d card_r = rect_2d.create(x, y, -5f);
        card_r.set_sprite(cardDataList[i].artwork);
        card_r.set_sprite_size(1.6f, 1.6f);
        card_r.set_collider_size(1.6f, 1.6f);
        
        visualData.card_rects.Add(card_r);
    }
}
public static void handle_card_input(int player_color) {
    if (data.mem == null) return;

    // Lấy đúng vùng hiển thị và danh sách bài dựa trên phe truyền vào
    var visualData = (player_color == 0) ? data.mem.white_hand_visual : data.mem.black_hand_visual;
    var cardDataList = (player_color == 0) ? data.mem.whiteHand : data.mem.blackHand;

    // Chỉ thực hiện vòng lặp nếu visualData có chứa các rect_2d (thẻ bài)
    if (visualData == null || visualData.card_rects == null) return;

    for (int i = 0; i < visualData.card_rects.Count; i++) {
        rect_2d r = visualData.card_rects[i];
        if (r == null) continue;

        // --- LOGIC KÉO THẺ ---
        if (r.mouse_click == 1) {
            r.move_to(mouse_util.x, mouse_util.y, -8f);
        }

        // --- LOGIC THẢ THẺ ---
        if (r.mouse_unclick == 1) {
            r.mouse_unclick = 0;
            r.mouse_click = 0;

            int tx = Mathf.RoundToInt((r.obj.transform.position.x + 4.48f) / 1.28f);
            int ty = Mathf.RoundToInt((r.obj.transform.position.y + 4.48f) / 1.28f);

            bool success = false;

            if (board_util.on_board(tx, ty)) {
                if (can_active(cardDataList[i], tx, ty)) {
                    ref data.board_cell cell = ref board_util.Cell(tx, ty);
                    
                    data.chess_piece targetPiece = data.mem.void_piece; 
                    if (cell.has_piece == 1) {
                        targetPiece = data.mem.get_army(cell.piece_color).troop_list[cell.piece_index];
                    }

                    success = apply_card_effect(cardDataList[i], ref targetPiece, r.obj.transform.position);

                    if (success) {
                        // Gửi card action qua mạng cho đối thủ
                        if (data.mem.is_online_game && net_manager.instance != null)
                            net_manager.instance.SendCardAction(player_color, i, tx, ty);

                        cardDataList.RemoveAt(i);
                        refresh_card_visuals(player_color);
                        sound_util.play_sound(data.mem.cardPlaySound);
                        // Cập nhật đúng phe
                        return;
                    }
                }
            }

            // Nếu thả trượt, trả bài về vị trí cũ của phe đó
            refresh_card_visuals(player_color);
            return;
        }
    }
}
    public static bool can_active(data.Card card, int tx, int ty) {
        if (!board_util.on_board(tx, ty)) return false;
        
        ref data.board_cell cell = ref board_util.Cell(tx, ty);
        int myColor = data.mem.current_player_color;

        switch (card.type) {
            case CardType.Event:
                // Yêu cầu KHÔNG có lính đứng trên
                return (cell.has_piece == 0);

            case CardType.Buff1:
            case CardType.Buff2:
            case CardType.Item:
                // Yêu cầu CÓ lính và PHẢI là quân mình
                return (cell.has_piece == 1 && cell.piece_color == myColor);

            case CardType.Debuff:
                // Yêu cầu CÓ lính và PHẢI là quân địch
                return (cell.has_piece == 1 && cell.piece_color != myColor);
            case CardType.GodQueen:
                // Yêu cầu CÓ lính và PHẢI là quân mình, KHÔNG áp dụng lên King
                return (cell.has_piece == 1 && cell.piece_color == myColor && data.mem.get_army(cell.piece_color).troop_list[cell.piece_index].piece_type != 5);
            case CardType.DemonQueen:
                // Yêu cầu CÓ lính và PHẢI là quân mình, CHỈ áp dụng lên Queen
                return (cell.has_piece == 1 && cell.piece_color == myColor && data.mem.get_army(cell.piece_color).troop_list[cell.piece_index].piece_type == 4);
            case CardType.Rock:
                if (cell.has_piece == 1) return false;
                if (myColor == 0 && ty <= 3) return true;
                if (myColor == 1 && ty >= 4) return true;
                return false;
            default:
                return false;
        }
    }
public static void add_card(int player_color, CardType type, bool fromNetwork = false) {
    if (data.mem.total_players != 2 || data.mem.bot_count != 0) return;
    sound_util.play_sound(data.mem.cardDrawSound);
    if (data.mem == null) return;

    // Sync card draw qua mạng (chỉ gửi nếu là local action, không phải nhận từ RPC)
    if (!fromNetwork && data.mem.is_online_game && net_manager.instance != null)
        net_manager.instance.SendAddCard(player_color, (int)type);

    // 1. Khởi tạo đối tượng thẻ mới
    data.Card newCard = new data.Card();
    newCard.type = type;
    
    // 2. Tự động gán Sprite và dữ liệu dựa trên Type từ data.mem
    switch (type) {
        case CardType.Item:
            newCard.cardName = "Gun Item";
            newCard.artwork = data.mem.card_gun;
            break;
        case CardType.Buff1:
            newCard.cardName = "Power Up 1";
            newCard.artwork = data.mem.card_plus1;
            newCard.value = 1;
            break;
        case CardType.Buff2:
            newCard.cardName = "Power Up 2";
            newCard.artwork = data.mem.card_plus2;
            newCard.value = 2;
            break;
        case CardType.Debuff:
            newCard.cardName = "Curse";
            newCard.artwork = data.mem.card_thunder;
            newCard.value = 3;
            break;
        case CardType.GodQueen:
            newCard.cardName = "God Queen";
            newCard.artwork = data.mem.card_god;
            break;
        case CardType.DemonQueen:
            newCard.cardName = "Demon Queen";
            newCard.artwork = data.mem.card_demon;
            break;
            case CardType.Event:
                newCard.cardName = "Lightning";
                newCard.artwork = data.mem.card_expandc;
            break;
        case CardType.Rock:
            newCard.cardName = "Rock";  
            newCard.artwork = data.mem.card_rock; // Tạm dùng sprite sự kiện cho thẻ nước
            break;
    }

    List<data.Card> targetHand = (player_color == 0) ? data.mem.whiteHand : data.mem.blackHand;
    
    if (targetHand == null) {
        if (player_color == 0) data.mem.whiteHand = new List<data.Card>();
        else data.mem.blackHand = new List<data.Card>();
        targetHand = (player_color == 0) ? data.mem.whiteHand : data.mem.blackHand;
    }

    targetHand.Add(newCard);
    refresh_card_visuals(player_color);

    string colorText = (player_color == 0) ? "white" : "black";
    if (GATrainer.instance == null || !GATrainer.instance.isTraining)
        Debug.Log($"<color={colorText}>Đã thêm thẻ {newCard.cardName} cho {(player_color == 0 ? "Trắng" : "Đen")}.</color>");
}
	public static bool apply_card_effect(data.Card card, ref data.chess_piece target, Vector3 effectPos) {
        switch (card.type) {
            case CardType.Buff1:
                ref data.chess_piece real2 = ref piece_util.get_piece_in_board(target.x, target.y);
                real2.score += 1;
                if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                    Debug.Log($"{real2.piece_type} được Buff! Score: {real2.score}");
                if (real2.score >= real2.score_to_envo) piece_util.evo(ref real2, effectPos);
                sound_util.play_sound(data.mem.cardBuffSound);
                return true;
            case CardType.Buff2:

                ref data.chess_piece real = ref piece_util.get_piece_in_board(target.x, target.y);
                real.score += 2;
                if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                    Debug.Log($"{real.piece_type} được Buff! Score: {real.score}");
                if (real.score >= real.score_to_envo) piece_util.evo(ref real, effectPos);
                sound_util.play_sound(data.mem.cardBuffSound);
                return true;

            case CardType.Debuff:
                int minhchau = target.piece_type;
                if (minhchau != 6 && minhchau != 7) {
                    
                    int tx = target.x;
                    int ty = target.y;
                    int color = target.player_color;
                    
                    data.army_data army = data.mem.get_army(color);
                    if (target.rect != null) {
                        target.rect.self_destroy();
                    }
                    board_util.clear_cell(tx, ty);

                    piece_util.create_piece(tx, ty, minhchau, army); 

                    ref data.chess_piece downgradedPiece = ref piece_util.get_piece_in_board(tx, ty);
                    
                    downgradedPiece.evolved = 0;
                    downgradedPiece.score_to_envo = 100;
                    downgradedPiece.score = 0;
                    
                    piece_util.apply_piece_data(ref downgradedPiece);
                    sound_util.play_sound(data.mem.cardDebuffSound);

                    return true; // Trả về true để xóa lá bài sau khi dùng
                }
                return false; // Trả về false nếu mục tiêu không hợp lệ (để người chơi không mất bài vô ích)

            case CardType.GodQueen:
                // Logic đặc biệt: Hồi sinh hoặc nâng cấp lên Queen
                if (target.piece_type != 5) { // Không áp dụng lên King
                    int tx = target.x;
                    int ty = target.y;
                    int color = target.player_color;
                    data.army_data army = data.mem.get_army(color);

                    // Xóa quân cũ khỏi màn hình và bộ nhớ
                    if (target.rect != null) target.rect.self_destroy();
                    board_util.clear_cell(tx, ty);
                    // Tạo quân Queen (loại 4) mới tại vị trí đó    
                    piece_util.create_piece(tx, ty, 4, army);   

                    if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                        Debug.Log("<color=yellow>Đã hồi sinh/nâng cấp lên Queen mới!</color>");
                    
                    return true;
                }
                return false;
            case CardType.DemonQueen:
                if (target.piece_type == 4) { 
                    // 1. Lưu lại các thông số cần thiết của quân Hậu cũ
                    int tx = target.x;
                    int ty = target.y;
                    int color = target.player_color;
                    data.army_data army = data.mem.get_army(color);

                    // 2. Xóa quân Hậu cũ khỏi màn hình và bộ nhớ
                    if (target.rect != null) target.rect.self_destroy();
                    board_util.clear_cell(tx, ty);

                    // 3. Tạo quân Demon Queen (loại 6) mới tại vị trí đó
                    // Hàm này sẽ tự động gán đúng Sprite, Score và UnitType cho loại 6
                    piece_util.create_piece(tx, ty, 6, army); 

                    // 4. Lấy thực thể vừa tạo để kích hoạt trạng thái tiến hóa (nhảy Mã + Shield)
                    ref data.chess_piece newDQueen = ref piece_util.get_piece_in_board(tx, ty);
                    newDQueen.evolved = 1; 
                    piece_util.apply_piece_data(ref newDQueen);
                    sound_util.play_sound(data.mem.DemonqueenEvolveSound);

                    if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                        Debug.Log("<color=purple>Đã thay thế Hậu bằng Demon Queen mới!</color>");
                    return true;
                }
                return false;
            case CardType.Item:
                if(target.piece_type == 5)
                {
                    int tx = target.x;
                    int ty = target.y;
                    int color = target.player_color;
                    int opponentColor = (color + 1) % 2;
                    data.army_data army = data.mem.get_army(color);

                    // 2. Xóa quân Hậu cũ khỏi màn hình và bộ nhớ
                    if (target.rect != null) target.rect.self_destroy();
                    board_util.clear_cell(tx, ty);

                    piece_util.create_piece(tx, ty, 7, army); 
                    ref data.chess_piece newKing = ref piece_util.get_piece_in_board(tx, ty);
                    newKing.evolved = 1; 
                    piece_util.apply_piece_data(ref newKing);
                    card_util.add_card(color, CardType.Rock); 
                    card_util.add_card(opponentColor, CardType.Rock); 
                    sound_util.play_sound(data.mem.kingEvolveSound);
                    return true;

                }
                return false;
            case CardType.Event:
                return true;
            case CardType.Rock:
                // Dùng effectPos thay vì mouse position (tương thích online)
                int txyy = Mathf.RoundToInt((effectPos.x + 4.48f) / 1.28f);
                int tyyy = Mathf.RoundToInt((effectPos.y + 4.48f) / 1.28f);

                // 3. Kiểm tra hợp lệ
                if (!board_util.on_board(txyy, tyyy)) return false;

                ref data.board_cell cell = ref board_util.Cell(txyy, tyyy);

                if (cell.has_piece == 0) {
                    // Đảm bảo armies[2] đã tồn tại (Neutral)
                    if (data.mem.armies.Length < 3) {
                        data.army_data[] newArmies = new data.army_data[3];
                        for(int i=0; i<data.mem.armies.Length; i++) newArmies[i] = data.mem.armies[i];
                        newArmies[2] = new data.army_data(2);
                        data.mem.armies = newArmies;
                    }

                    // Tạo đá
                    piece_util.create_piece(txyy, tyyy, 99, data.mem.armies[2]);
                    ref data.chess_piece rock = ref piece_util.get_piece_in_board(txyy, tyyy);
                    rock.rect.set_sprite(data.mem.rock); // Sprite hòn đá
                    rock.rect.set_sprite_size(1.1f, 1.1f);
                    
                    return true;
                }
                return false;

        }
        if (GATrainer.instance == null || !GATrainer.instance.isTraining)
            sound_util.play_sound(data.mem.startSound); 
        return false;
    }

}