﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class piece_util {
    // =========================================================================
    // PIECE — CREATE / SPRITE
    // =========================================================================
	public static void create_piece(int x, int y, int piece_type, data.army_data army, int pawn_dir_x = 0, int pawn_dir_y = 0) {
        int c = army.color;

	    data.chess_piece cp = new data.chess_piece();
        cp.rect                = rect_2d.create(board_util.board_to_world(x), board_util.board_to_world(y), -1f);
        cp.x                   = x;
        cp.y                   = y;
        cp.piece_type          = piece_type;
        cp.player_color        = army.color;
        cp.score               = 1;
        cp.score_to_envo       = 0;
        cp.unitType            = PieceType.Light; // =_= ...
        cp.evolved             = 0;
        cp.evolved_type        = 0;
        cp.selected            = 0;
        cp.hovered             = 0;
        cp.hover_sprite_scale  = 1.2f;
        cp.normal_sprite_scale = 0.8f;
        cp.shield              = 0;
        cp.has_moved = 0;

        cp.pawn_dir_x          = pawn_dir_x;
        cp.pawn_dir_y          = pawn_dir_y;

        switch (piece_type) {
            case 0: // Pawn
                cp.normal_sprite = c == 0 ? data.mem.wp_pawn : c == 1 ? data.mem.bp_pawn : c == 2 ? data.mem.blue_pawn : data.mem.green_pawn;
                cp.evo_sprite0   = c == 0 ? data.mem.wp_e_pawn_knight : c == 1 ? data.mem.bp_e_pawn_knight : c == 2 ? data.mem.e_blue_pawn_knight : data.mem.e_green_pawn_knight;
                cp.evo_sprite1   = c == 0 ? data.mem.wp_e_pawn_bishop : c == 1 ? data.mem.bp_e_pawn_bishop : c == 2 ? data.mem.e_blue_pawn_bishop : data.mem.e_green_pawn_bishop;
                cp.evo_sprite2   = c == 0 ? data.mem.wp_e_pawn_rook   : c == 1 ? data.mem.bp_e_pawn_rook   : c == 2 ? data.mem.e_blue_pawn_rook   : data.mem.e_green_pawn_rook;
                break;

            case 1: // Rook
                cp.normal_sprite = c == 0 ? data.mem.wp_rook : c == 1 ? data.mem.bp_rook : c == 2 ? data.mem.blue_rook : data.mem.green_rook;
                cp.evo_sprite0   = c == 0 ? data.mem.wp_e_rook : c == 1 ? data.mem.bp_e_rook : c == 2 ? data.mem.e_blue_rook : data.mem.e_green_rook;
                cp.evo_sprite1   = null; cp.evo_sprite2 = null; break;

            case 2: // Knight
                cp.normal_sprite = c == 0 ? data.mem.wp_knight : c == 1 ? data.mem.bp_knight : c == 2 ? data.mem.blue_knight : data.mem.green_knight;
                cp.evo_sprite0   = c == 0 ? data.mem.wp_e_knight : c == 1 ? data.mem.bp_e_knight : c == 2 ? data.mem.e_blue_knight : data.mem.e_green_knight;
                cp.evo_sprite1   = null; cp.evo_sprite2 = null; break;

            case 3: // Bishop
                cp.normal_sprite = c == 0 ? data.mem.wp_bishop : c == 1 ? data.mem.bp_bishop : c == 2 ? data.mem.blue_bishop : data.mem.green_bishop;
                cp.evo_sprite0   = c == 0 ? data.mem.wp_e_bishop : c == 1 ? data.mem.bp_e_bishop : c == 2 ? data.mem.e_blue_bishop : data.mem.e_green_bishop;
                cp.evo_sprite1   = null; cp.evo_sprite2 = null; break;

            case 4: // Queen
                cp.normal_sprite = c == 0 ? data.mem.wp_queen : c == 1 ? data.mem.bp_queen : c == 2 ? data.mem.blue_queen : data.mem.green_queen;
                cp.evo_sprite0   = c == 0 ? data.mem.wp_e_queen : c == 1 ? data.mem.bp_e_queen : c == 2 ? data.mem.e_blue_queen : data.mem.e_green_queen;
                cp.evo_sprite1   = null; cp.evo_sprite2 = null; break;

            case 5: // King
                cp.normal_sprite = c == 0 ? data.mem.wp_king : c == 1 ? data.mem.bp_king : c == 2 ? data.mem.blue_king : data.mem.green_king;
                cp.evo_sprite0   = c == 0 ? data.mem.wp_e_king : c == 1 ? data.mem.bp_e_king : c == 2 ? data.mem.e_blue_king : data.mem.e_green_king;
                cp.evo_sprite1   = null; cp.evo_sprite2 = null; break;

            case 6: // Demon QueenFshield
                cp.normal_sprite = c == 0 ? data.mem.wp_e_dqueen : data.mem.bp_e_dqueen;
                cp.evo_sprite0   = cp.normal_sprite; break;

            case 7: // King (Democracy/Evolved)
                cp.normal_sprite = c == 0 ? data.mem.wp_e_king : c == 1 ? data.mem.bp_e_king : c == 2 ? data.mem.e_blue_king : data.mem.e_green_king;
        cp.evo_sprite0   = cp.normal_sprite; break;
            case 99: cp.normal_sprite = data.mem.rock;
                    cp.evo_sprite0   = null;
                    cp.evo_sprite1   = null;
                    cp.evo_sprite2   = null; break;
            default:cp.normal_sprite = null;
                    cp.evo_sprite0   = null;
                    cp.evo_sprite1   = null;
                    cp.evo_sprite2   = null; break;
        }

        piece_util.apply_piece_data(ref cp);

        int idx = army.troop_count;
        army.troop_list[army.troop_count++] = cp;
        board_util.set_cell(x, y, army.color, idx);
    }
    
	public static void apply_piece_data(ref data.chess_piece cp){
    
        if (cp.evolved == 0) {
            cp.rect.set_sprite(cp.normal_sprite);
            switch (cp.piece_type) {
                case 4: cp.score = 9; cp.score_to_envo = 15; cp.unitType = PieceType.Core;   break;
                case 5: cp.score = 0; cp.score_to_envo = 100;  cp.unitType = PieceType.Core;   break;
                case 1: cp.score = 5; cp.score_to_envo = 7; cp.unitType = PieceType.RHeavy; break;
                case 2: cp.score = 3; cp.score_to_envo = 5;  cp.unitType = PieceType.KHeavy; break;
                case 3: cp.score = 3; cp.score_to_envo = 5;  cp.unitType = PieceType.BHeavy; break;
                case 0: cp.score = 1; cp.score_to_envo = 4;  cp.unitType = PieceType.Light;  break;
                case 6: cp.score = 0; cp.score_to_envo = 0;  cp.unitType = PieceType.Core;   break;
            }
        } else {
            Sprite evo = cp.piece_type == 0
                ? (cp.evolved_type == 0 ? cp.evo_sprite0 : cp.evolved_type == 1 ? cp.evo_sprite1 : cp.evo_sprite2)
                : cp.evo_sprite0;
            cp.rect.set_sprite(evo);
            switch (cp.piece_type) {
                case 4: cp.unitType = PieceType.Core;   break;
                case 5: cp.unitType = PieceType.Core;   break;
                case 1: cp.unitType = PieceType.RHeavy; break;
                case 2: cp.unitType = PieceType.KHeavy; break;
                case 3: cp.unitType = PieceType.BHeavy; break;
                case 0: cp.unitType = PieceType.ELight; break;
                case 6: cp.unitType = PieceType.Core; cp.shield = 6;   break; //added shield to dqueen
                case 99: cp.unitType = PieceType.Rock;  break; 
            }
        }
        cp.rect.fit_collider_to_sprite(cp.rect.sprite);
    }


    // =========================================================================
    // MOVE VALIDATION
    // =========================================================================
    public static bool can_move_to(ref data.chess_piece cp, int tx, int ty){
        if (!board_util.on_board(tx, ty)) return false;
        ref data.board_cell cell = ref board_util.Cell(tx, ty);
        if (cell.has_piece == 1) {
            data.army_data targetArmy = data.mem.get_army(cell.piece_color);
            if (targetArmy.troop_list[cell.piece_index].piece_type == 99) {
                return false;
            }
        }
        if (cell.has_piece == 1 && data.mem.get_army(cell.piece_color).troop_list[cell.piece_index].player_color == cp.player_color) 
            return false;

        int  dir      = (cp.player_color == 0) ? 1 : -1;
        bool baseMove = false;


        switch (cp.piece_type) {
            case 0: baseMove = valid_pawn(ref cp, tx, ty, cp.pawn_dir_x, cp.pawn_dir_y);                            break;
            case 1: baseMove = valid_line(ref cp, tx, ty);                                  break;
            case 2: baseMove = valid_knight(ref cp, tx, ty);                                break;
            case 3: baseMove = valid_diag(ref cp, tx, ty);                                  break;
            case 4: baseMove = valid_line(ref cp, tx, ty) || valid_diag(ref cp, tx, ty); break;
            case 5: baseMove = valid_king(ref cp, tx, ty);                                  break;
            case 6: baseMove = valid_line(ref cp, tx, ty) || valid_diag(ref cp, tx, ty) || valid_knight(ref cp, tx, ty); break;
            case 7: 
                baseMove = valid_king(ref cp, tx, ty);
                if (!baseMove && Mathf.Abs(tx - cp.x) <= 2 && Mathf.Abs(ty - cp.y) <= 2) {
                    baseMove = cell.has_piece == 1 && data.mem.get_army(cell.piece_color).troop_list[cell.piece_index].player_color != cp.player_color;
                }
                break;
        }

        if (cp.evolved == 0) return baseMove;

        switch (cp.piece_type) {
            case 0: 
                if (cp.evolved_type == 2) return baseMove || valid_line(ref cp, tx, ty);
                if (cp.evolved_type == 0) return baseMove || valid_knight(ref cp, tx, ty);
                if (cp.evolved_type == 1) return baseMove || valid_diag(ref cp, tx, ty);
                return baseMove;
            case 2: return baseMove || valid_evo_knight(ref cp, tx, ty);
            case 3: return baseMove || valid_king(ref cp, tx, ty);
            case 6: return baseMove || valid_knight(ref cp, tx, ty); 
            default: return baseMove;
        }
    }

    public static bool valid_pawn(ref data.chess_piece cp, int tx, int ty, int dx_dir, int dy_dir) {
        int dx = tx - cp.x;
        int dy = ty - cp.y;

        // 1. Kiểm tra bước đi chéo (Chỉ dành cho việc ăn quân)
        bool isDiagonalStep = (dy_dir != 0) ? (Mathf.Abs(dx) == 1 && dy == dy_dir) 
                                            : (Mathf.Abs(dy) == 1 && dx == dx_dir);

        if (isDiagonalStep) {
            // Kiểm tra ô đích
            ref data.board_cell cell = ref board_util.Cell(tx, ty);
            
            // ĐIỀU KIỆN QUAN TRỌNG: Phải có quân cờ ở ô đích
            if (cell.has_piece == 1) {
                return data.mem.get_army(cell.piece_color).troop_list[cell.piece_index].player_color != cp.player_color;
            }
            
            // Kiểm tra thêm luật En Passant (Bắt chốt qua đường) nếu bạn có dùng
            if (tx == data.mem.en_passant_x && ty == data.mem.en_passant_y) {
                return true;
            }

            return false; // Đi chéo mà không có quân thì không cho đi
        }

        // 2. Logic Đi thẳng 1 ô (Bắt buộc ô đích phải TRỐNG)
        if (dx == dx_dir && dy == dy_dir) {
            return board_util.Cell(tx, ty).has_piece == 0;
        }

        // 3. Logic Đi thẳng 2 ô (Nước đầu tiên - Bắt buộc cả đường đi và ô đích phải TRỐNG)
        if (cp.has_moved == 0 && dx == dx_dir * 2 && dy == dy_dir * 2) {
            bool intermediateEmpty = board_util.Cell(cp.x + dx_dir, cp.y + dy_dir).has_piece == 0;
            bool destinationEmpty  = board_util.Cell(tx, ty).has_piece == 0;
            return intermediateEmpty && destinationEmpty;
        }

        return false;
    }

    public static bool valid_line(ref data.chess_piece cp, int tx, int ty) {
        if (tx != cp.x && ty != cp.y) return false;
        return !is_blocked(ref cp, tx, ty);
    }

    public static bool valid_diag(ref data.chess_piece cp, int tx, int ty) {
        if (Mathf.Abs(tx - cp.x) != Mathf.Abs(ty - cp.y)) return false;
        return !is_blocked(ref cp, tx, ty);
    }

    public static bool is_blocked(ref data.chess_piece cp, int tx, int ty) {
        int sx = System.Math.Sign(tx - cp.x), sy = System.Math.Sign(ty - cp.y);
        int cx = cp.x + sx, cy = cp.y + sy;
        while (cx != tx || cy != ty) {
            if (!board_util.on_board(cx, cy)) return true;
            if (board_util.Cell(cx, cy).has_piece == 1) return true;
            cx += sx; cy += sy;
        }
        return false;
    }

    public static bool valid_knight(ref data.chess_piece cp, int tx, int ty) {
        int dx = Mathf.Abs(tx - cp.x), dy = Mathf.Abs(ty - cp.y);
        return (dx == 1 && dy == 2) || (dx == 2 && dy == 1);
    }

    public static bool valid_evo_knight(ref data.chess_piece cp, int tx, int ty) {
        int dx = Mathf.Abs(tx - cp.x), dy = Mathf.Abs(ty - cp.y);
        return (dx == 2 && dy == 0) || (dx == 0 && dy == 2);
    }

    public static bool valid_king(ref data.chess_piece cp, int tx, int ty) {
        int dx = Mathf.Abs(tx - cp.x), dy = Mathf.Abs(ty - cp.y);
        return dx <= 1 && dy <= 1 && (dx + dy > 0);
    }

    // =========================================================================
    // EVOLUTION
    // =========================================================================

    public static void absorb_point(ref data.chess_piece cp, ref data.chess_piece victim, Vector3 pos) {
        if (cp.evolved == 1 && cp.piece_type != 0) return;
        if (victim.piece_type == 99) return; // Don't absorb points from obstacles
        cp.score += victim.score;
        if (GATrainer.instance == null || !GATrainer.instance.isTraining)
            Debug.Log($"<color=blue>Absorbed {victim.score} points!</color> Total score: {cp.score}");
        if (cp.score < 0) cp.score = 1;
        if (cp.piece_type == 7 && cp.score >= cp.score_to_envo) //king có súng nhận điểm tích đạn
        {
            cp.score = 0; // cap score at evo threshold for king to prevent overleveling
            // thêm đạn cho súng của vua
            return;
        } 
        if (cp.unitType == PieceType.Light) {
            // Debug.Log($"Checking for weapon evolution... In enemy half: {(cp.player_color == 0 ? cp.y >= 3 : cp.y <= 4)}, Ate heavy piece: {victim.unitType == PieceType.KHeavy || victim.unitType == PieceType.BHeavy || victim.unitType == PieceType.RHeavy}");
            bool inEnemyHalf = (cp.player_color == 0) ? cp.y >= 3 : cp.y <= 4; // not half enough but whatever
            bool ateHeavy    = victim.unitType == PieceType.KHeavy || victim.unitType == PieceType.BHeavy || victim.unitType == PieceType.RHeavy;
            if (inEnemyHalf && ateHeavy) { piece_util.evo_with_weapon(ref cp, victim.unitType, pos); return; }
            return;
        }
        if (cp.score >= cp.score_to_envo) piece_util.evo(ref cp, pos);
    }

	public static void evo(ref data.chess_piece cp, Vector3 pos) {

		// ===== FIX: trigger camera =====
		data.mem.evolving_signal = 1;
		data.mem.evolving_pos    = pos;

		if (cp.piece_type == 5) {
			cp.piece_type = 7;
			cp.score = 0;
		}

		cp.evolved = 1;
		piece_util.apply_piece_data(ref cp);

		int myColor = cp.player_color;
		int opponentColor = (myColor == 0) ? 1 : 0;

		CardType[] pool = { CardType.Buff1, CardType.Buff2, CardType.Debuff };
		CardType rand   = pool[Random.Range(0, pool.Length)];

		card_util.add_card(opponentColor, rand);
        switch (cp.piece_type) {
            case 1:sound_util.play_sound(data.mem.rookEvolveSound); break;
            case 2:sound_util.play_sound(data.mem.knightEvolveSound); break;
            case 3:sound_util.play_sound(data.mem.bishopEvolveSound); break;
            case 4:card_util.add_card(myColor, CardType.Item); card_util.add_card(opponentColor, CardType.DemonQueen);sound_util.play_sound(data.mem.GodqueenEvolveSound); break;
        }
		// if (cp.piece_type == 4) {
		// 	card_util.add_card(myColor, CardType.Item);
		// 	card_util.add_card(opponentColor, CardType.DemonQueen);

        //     if (GATrainer.instance == null || !GATrainer.instance.isTraining)
		// 	    Debug.Log($"<color=cyan>Queen {myColor} evolved!</color>");
		// }
        if (GATrainer.instance == null || !GATrainer.instance.isTraining)
		    Debug.Log($"<color=green>{cp.piece_type} EVOLVED</color>");
	}

	public static void evo_with_weapon(ref data.chess_piece cp, PieceType weapon, Vector3 pos) {

		// ===== FIX: trigger camera =====
		data.mem.evolving_signal = 1;
		data.mem.evolving_pos    = pos;

		cp.evolved       = 1;
		cp.score_to_envo = 100;
		cp.unitType      = PieceType.ELight;

		if      (weapon == PieceType.KHeavy) cp.evolved_type = 0;
		else if (weapon == PieceType.BHeavy) cp.evolved_type = 1;
		else if (weapon == PieceType.RHeavy) cp.evolved_type = 2;
        sound_util.play_sound(data.mem.pawnEvolveSound);

		piece_util.apply_piece_data(ref cp);

		int myColor = cp.player_color;
		int opponentColor = (myColor == 0) ? 1 : 0;

		CardType[] pool = { CardType.Buff1, CardType.Buff2, CardType.Debuff };
		CardType rand   = pool[Random.Range(0, pool.Length)];

		card_util.add_card(opponentColor, rand);
	}

    // =========================================================================
    // ATTACK / MOVE
    // =========================================================================

    public static void piece_attack(ref data.chess_piece attacker, int tx, int ty, Vector3 pos, bool is_counter = false) {
        data.mem.turns_without_progress = 0;
        ref data.board_cell cell = ref board_util.Cell(tx, ty);

        if (cell.has_piece == 0) return;

        data.army_data enemy = data.mem.get_army(cell.piece_color);
        ref data.chess_piece target = ref enemy.troop_list[cell.piece_index];
        if(target.piece_type == 99) return; 
        if (GATrainer.instance == null || !GATrainer.instance.isTraining)
            Debug.Log($"Attacking piece at ({tx}, {ty}) - Type: {target.piece_type}, Player: {target.player_color}");
        // --- LOGIC DEMON QUEEN PHẢN ĐÒN ---
        // Chỉ phản đòn nếu mục tiêu là Demon Queen (6) và ĐÂY KHÔNG PHẢI là đòn phản đòn sẵn có
        if (target.piece_type == 6 && attacker.piece_type != 7 && !is_counter) {
            if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                Debug.Log("<color=purple>Demon Queen phản đòn!</color>");
            
            // Demon Queen đánh ngược lại vị trí của kẻ tấn công (attacker.x, attacker.y)
            // Gửi true vào tham số cuối để kết thúc chuỗi phản đòn
            piece_attack(ref target, attacker.x, attacker.y, attacker.rect.obj.transform.position, true); 
            move_piece(ref target, FindPieceIndex(ref target), target.player_color, attacker.x, attacker.y);
            pvp_util.next_player_turn(); 

            move_plate_util.clear_move_plate();
            return;
        }
        if(target.piece_type == 4 && target.evolved == 1) {
            card_util.add_card(target.player_color, CardType.GodQueen);
        }
        if(attacker.piece_type == 7) {
            sound_util.play_sound(data.mem.burstSound   );
            int txx = attacker.x;
            int tyx = attacker.y;
            int color = attacker.player_color;
            data.army_data army = data.mem.get_army(color);

            if (attacker.rect != null) attacker.rect.self_destroy();
            board_util.clear_cell(txx, tyx);

            piece_util.create_piece(txx, tyx, 5, army); 
            ref data.chess_piece newKing = ref piece_util.get_piece_in_board(txx, tyx);
            newKing.evolved = 0; 
            newKing.score_to_envo = 3;
            piece_util.apply_piece_data(ref newKing);
        }
        if(attacker.piece_type == 1 && attacker.evolved == 1) {
            int myColor = attacker.player_color;


            CardType[] randomCards = { CardType.Buff1, CardType.Buff2}; 
            
            int randomIndex = UnityEngine.Random.Range(0, randomCards.Length); 
            CardType selectedPowerUp = randomCards[randomIndex];

            card_util.add_card(myColor, selectedPowerUp);
        }
        piece_util.absorb_point(ref attacker, ref target, pos);
        if (GATrainer.instance == null || !GATrainer.instance.isTraining)
            sound_util.play_sound(data.mem.captureSound);

        // Hủy quân cờ bị ăn
        if (target.rect != null) {
            target.rect.self_destroy();
            target.rect = null;
        }
        board_util.clear_cell(tx, ty);
    }

    public static void move_piece(ref data.chess_piece cp, int idx, int color, int tx, int ty) {
        ref data.board_cell targetCell = ref board_util.Cell(tx, ty);
        if (targetCell.has_piece == 1) {
            var targetPiece = data.mem.get_army(targetCell.piece_color).troop_list[targetCell.piece_index];
            if (targetPiece.piece_type == 99) {
                if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                    Debug.LogWarning("Cannot move: Target cell is an indestructible Rock!");
                return; 
            }
        }
        if(cp.piece_type == 6){
            cp.shield -= 1; 
            if(cp.shield == 0) {
            cp.rect.self_destroy(); 
            board_util.clear_cell(cp.x, cp.y);
            
            return;}
            }
        if (cp.piece_type == 0) {
            data.mem.turns_without_progress = 0;
        }

        board_util.clear_cell(cp.x, cp.y);
        cp.x = tx;
        cp.y = ty;
        cp.rect.move_to_board(tx, ty, -1f);
        board_util.set_cell(tx, ty, color, idx);
        if (GATrainer.instance == null || !GATrainer.instance.isTraining)
            Debug.Log($"Moved piece to ({tx}, {ty})");
    }


    public static void DQueenSkill(ref data.chess_piece dQueen, int targetX, int targetY) {
        // 1. Xóa vị trí cũ của DQueen trên board
        board_util.Cell(dQueen.x, dQueen.y).has_piece = 0;

        // 2. Cập nhật tọa độ mới (vị trí kẻ địch vừa đứng)
        dQueen.x = targetX;
        dQueen.y = targetY;

        // 3. Cập nhật vị trí hiển thị (Sprite)
        dQueen.rect.move_to_board(targetX, targetY, -1f);

        // 4. Ghi đè DQueen vào ô mới trên mảng Board
        board_util.set_cell(targetX, targetY, dQueen.player_color, FindPieceIndex(ref dQueen));
        
        if (GATrainer.instance == null || !GATrainer.instance.isTraining)
            Debug.Log("<color=purple>DQueen phản đòn và chiếm giữ vị trí của kẻ địch!</color>");
    }
	

    public static ref data.chess_piece get_piece_in_board(int x, int y) {
        ref data.board_cell cell = ref board_util.Cell(x, y);
        if (cell.has_piece == 0) return ref data.mem.void_piece;
        return ref data.mem.get_army(cell.piece_color).troop_list[cell.piece_index];
    }



    // Helper để tìm index chính xác của quân cờ trong danh sách quân
    public static int FindPieceIndex(ref data.chess_piece cp) {
        data.army_data army = data.mem.get_army(cp.player_color);
        for (int i = 0; i < army.troop_count; i++) {
            if (System.Object.ReferenceEquals(army.troop_list[i].rect, cp.rect)) return i;
        }
        return -1;
    }
	

    public static void unselect_all_piece(){
        for (int color = 0; color <= 1; color++) {
            data.army_data army = data.mem.get_army(color);
            for (int i = 0; i < army.troop_count; i++) {
                army.troop_list[i].selected = 0;
                army.troop_list[i].hovered  = 0;
            }
        }
    }

    // =========================================================================
    // CHECK KING CHECKED
    // =========================================================================
    public static bool IsSafeMove(int piece_index, int color, int tx, int ty, bool isAttack) {
        ref data.chess_piece attacker = ref data.mem.get_army(color).troop_list[piece_index];
        
        int old_x = attacker.x;
        int old_y = attacker.y;

        data.chess_piece target_backup = default;
        int target_idx = -1;
        int target_color = -1;

        if (isAttack) {
            ref data.board_cell cell = ref board_util.Cell(tx, ty);
            if (cell.has_piece == 1) {
                target_color = cell.piece_color;
                target_idx = cell.piece_index;
                target_backup = data.mem.get_army(target_color).troop_list[target_idx];
                
                data.mem.get_army(target_color).troop_list[target_idx].rect = null; 
            }
        }

        board_util.clear_cell(old_x, old_y);
        attacker.x = tx;
        attacker.y = ty;
        board_util.set_cell(tx, ty, color, piece_index);

        int kx = -1, ky = -1;
        data.army_data army = data.mem.get_army(color);
        for (int i = 0; i < army.troop_count; i++) {
            if ((army.troop_list[i].piece_type == 5 || army.troop_list[i].piece_type == 7) && army.troop_list[i].rect != null) {
                kx = army.troop_list[i].x;
                ky = army.troop_list[i].y;
                break;
            }
        }

        bool isSafe = true;
        if (kx != -1) {
            isSafe = !AI_util.IsSquareAttacked(kx, ky, color);
        }

        board_util.clear_cell(tx, ty);
        attacker.x = old_x;
        attacker.y = old_y;
        board_util.set_cell(old_x, old_y, color, piece_index);

        if (isAttack && target_color != -1) {
            data.mem.get_army(target_color).troop_list[target_idx] = target_backup;
            board_util.set_cell(tx, ty, target_color, target_idx);
        }

        return isSafe;
    }
    public static void create_obstacle(int x, int y) {

        data.chess_piece obstacle = new data.chess_piece();
        
        // 2. Thiết lập hiển thị (dùng rect_2d)
        obstacle.rect = rect_2d.create(board_util.board_to_world(x), board_util.board_to_world(y), -1f);
        obstacle.rect.set_sprite(data.mem.rock);
        obstacle.rect.set_sprite_size(1f, 1f);
        obstacle.rect.fit_collider_to_sprite(obstacle.rect.sprite);

        obstacle.x = x;
        obstacle.y = y;
        obstacle.piece_type = 99; // Loại đặc biệt: Obstacle
        obstacle.player_color = 1; // Phe thứ 3 (Trung lập)
        obstacle.unitType = PieceType.Rock;
        
        // 4. Đưa vào hệ thống board để is_blocked() nhận diện được
        // Lưu ý: Bạn cần một list riêng cho vật cản hoặc đưa vào một army trung lập
        // Cách nhanh nhất là dùng board_util.set_cell với một index đặc biệt
        board_util.set_cell(x, y, 99, 999); // color 2, index 999 đại diện cho vật cản
        if (GATrainer.instance == null || !GATrainer.instance.isTraining)
            Debug.Log($"<color=gray>Đã đặt vật cản tại {x}, {y}</color>");
    }
}