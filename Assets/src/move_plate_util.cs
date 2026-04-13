using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class move_plate_util {

    // =========================================================================
    // MOVEPLATE SPAWN
    // =========================================================================

    public static void spawn_plate(ref data.chess_piece cp, int idx, int color) {
        switch (cp.piece_type) {
            case 0: move_plate_util.spawn_pawn_plate(ref cp, idx, color);           break;
            case 1: move_plate_util.spawn_line_plate(ref cp, idx, color);           break;
            case 2: move_plate_util.spawn_knight_plate(ref cp, idx, color);         break;
            case 3: move_plate_util.spawn_diag_plate(ref cp, idx, color);           break;
            case 4: move_plate_util.spawn_line_plate(ref cp, idx, color);
                    move_plate_util.spawn_diag_plate(ref cp, idx, color);           break;
            case 5: move_plate_util.spawn_king_plate(ref cp, idx, color);           break;
            case 6: move_plate_util.spawn_line_plate(ref cp, idx, color);
                    move_plate_util.spawn_diag_plate(ref cp, idx, color);
                    move_plate_util.spawn_knight_plate(ref cp, idx, color);         break;
            case 7: move_plate_util.spawn_gun_king_plate(ref cp, idx, color);       break;
        }

        if (cp.evolved == 0) return;

        switch (cp.piece_type) {
            case 0:
                if      (cp.evolved_type == 2) move_plate_util.spawn_line_plate(ref cp, idx, color);
                else if (cp.evolved_type == 0) move_plate_util.spawn_knight_plate(ref cp, idx, color);
                else if (cp.evolved_type == 1) move_plate_util.spawn_diag_plate(ref cp, idx, color);
                break;
            case 2: move_plate_util.spawn_evo_knight_plate(ref cp, idx, color); break;
            case 3: move_plate_util.spawn_king_plate(ref cp, idx, color);       break;
            case 5: move_plate_util.spawn_king_plate(ref cp, idx, color);       break;
        }
    }

    public static void spawn_line_plate(ref data.chess_piece cp, int i, int c) {
        move_plate_util.ray_plate(ref cp,i,c,  1, 0, 8,1);
        move_plate_util.ray_plate(ref cp,i,c, -1, 0, 8,1);
        move_plate_util.ray_plate(ref cp,i,c,  0, 1, 8,1);
        move_plate_util.ray_plate(ref cp,i,c,  0,-1, 8,1);
    }

    public static void spawn_diag_plate(ref data.chess_piece cp, int i, int c) {
        move_plate_util.ray_plate(ref cp,i,c,  1, 1, 8,1);
        move_plate_util.ray_plate(ref cp,i,c,  1,-1, 8,1);
        move_plate_util.ray_plate(ref cp,i,c, -1, 1, 8,1);
        move_plate_util.ray_plate(ref cp,i,c, -1,-1, 8,1);
    }

    public static void spawn_king_plate(ref data.chess_piece cp, int i, int c) {
    move_plate_util.ray_plate(ref cp,i,c,  1, 0, 1,1); move_plate_util.ray_plate(ref cp,i,c, -1, 0, 1,1);
    move_plate_util.ray_plate(ref cp,i,c,  0, 1, 1,1); move_plate_util.ray_plate(ref cp,i,c,  0,-1, 1,1);
    move_plate_util.ray_plate(ref cp,i,c,  1, 1, 1,1); move_plate_util.ray_plate(ref cp,i,c,  1,-1, 1,1);
    move_plate_util.ray_plate(ref cp,i,c, -1, 1, 1,1); move_plate_util.ray_plate(ref cp,i,c, -1,-1, 1,1);

    if (cp.has_moved == 0 && !AI_util.IsSquareAttacked(cp.x, cp.y, c)) {
        if (board_util.on_board(cp.x + 3, cp.y)) {
            ref data.board_cell rookCell = ref board_util.Cell(cp.x + 3, cp.y);
            if (rookCell.has_piece == 1) {
                ref data.chess_piece rook = ref data.mem.get_army(rookCell.piece_color).troop_list[rookCell.piece_index];
                if (rook.piece_type == 1 && rook.has_moved == 0) { // Nếu là Xe và chưa đi
                    if (board_util.Cell(cp.x + 1, cp.y).has_piece == 0 && board_util.Cell(cp.x + 2, cp.y).has_piece == 0) {
                        if (!AI_util.IsSquareAttacked(cp.x + 1, cp.y, c) && !AI_util.IsSquareAttacked(cp.x + 2, cp.y, c)) {
                            move_plate_util.spawn_plate(i, c, cp.x + 2, cp.y, false);
                        }
                    }
                }
            }
        }
        if (board_util.on_board(cp.x - 4, cp.y)) {
            ref data.board_cell rookCell = ref board_util.Cell(cp.x - 4, cp.y);
            if (rookCell.has_piece == 1) {
                ref data.chess_piece rook = ref data.mem.get_army(rookCell.piece_color).troop_list[rookCell.piece_index];
                if (rook.piece_type == 1 && rook.has_moved == 0) {
                    if (board_util.Cell(cp.x - 1, cp.y).has_piece == 0 && board_util.Cell(cp.x - 2, cp.y).has_piece == 0 && board_util.Cell(cp.x - 3, cp.y).has_piece == 0) {
                        if (!AI_util.IsSquareAttacked(cp.x - 1, cp.y, c) && !AI_util.IsSquareAttacked(cp.x - 2, cp.y, c)) {
                            move_plate_util.spawn_plate(i, c, cp.x - 2, cp.y, false);
                        }
                    }
                }
            }
        }
    }
}

    public static void spawn_gun_king_plate(ref data.chess_piece cp, int i, int c) {
        for (int xOffset = -2; xOffset <= 2; xOffset++) {
            for (int yOffset = -2; yOffset <= 2; yOffset++) {
                if (xOffset == 0 && yOffset == 0) continue;

                int targetX = cp.x + xOffset;
                int targetY = cp.y + yOffset;

                if (board_util.on_board(targetX, targetY)) {
                    ref data.board_cell cell = ref board_util.Cell(targetX, targetY);
                    if (cell.has_piece == 1) {
                        ref data.chess_piece target = ref piece_util.get_piece_in_board(targetX, targetY);
                        if (target.player_color != cp.player_color)
                            move_plate_util.spawn_plate(i, c, targetX, targetY, true);
                    } else {
                        if (Mathf.Abs(xOffset) <= 1 && Mathf.Abs(yOffset) <= 1)
                            move_plate_util.spawn_plate(i, c, targetX, targetY, false);
                    }
                }
            }
        }
    }

    public static void spawn_knight_plate(ref data.chess_piece cp, int i, int c) {
        move_plate_util.ray_plate(ref cp,i,c,  1, 2, 1,1, skip_obs:true);
        move_plate_util.ray_plate(ref cp,i,c, -1, 2, 1,1, skip_obs:true);
        move_plate_util.ray_plate(ref cp,i,c,  2, 1, 1,1, skip_obs:true);
        move_plate_util.ray_plate(ref cp,i,c,  2,-1, 1,1, skip_obs:true);
        move_plate_util.ray_plate(ref cp,i,c,  1,-2, 1,1, skip_obs:true);
        move_plate_util.ray_plate(ref cp,i,c, -1,-2, 1,1, skip_obs:true);
        move_plate_util.ray_plate(ref cp,i,c, -2, 1, 1,1, skip_obs:true);
        move_plate_util.ray_plate(ref cp,i,c, -2,-1, 1,1, skip_obs:true);
    }

    public static void spawn_evo_knight_plate(ref data.chess_piece cp, int i, int c) {
        move_plate_util.ray_plate(ref cp,i,c,  1, 0, 1,2, skip_obs:true);
        move_plate_util.ray_plate(ref cp,i,c, -1, 0, 1,2, skip_obs:true);
        move_plate_util.ray_plate(ref cp,i,c,  0, 1, 1,2, skip_obs:true);
        move_plate_util.ray_plate(ref cp,i,c,  0,-1, 1,2, skip_obs:true);
    }

    public static void spawn_pawn_plate(ref data.chess_piece cp, int i, int c) {
        int dx = cp.pawn_dir_x;
        int dy = cp.pawn_dir_y;

        if (dx == 0 && dy == 0) {
            dy = (cp.player_color == 0) ? 1 : -1;
        }

        int progress  = cp.x * Mathf.Abs(dx) + cp.y * Mathf.Abs(dy);
        int startLine = (dx + dy > 0) ? 1 : (dx == 0 ? data.mem.board_h : data.mem.board_w) - 2;
        int steps     = (progress == startLine) ? 2 : 1;

        move_plate_util.ray_plate(ref cp,i,c,  dx,  dy, steps, 1, skip_obs:false, capture:false);
        int px = Mathf.Abs(dy); // 1 when moving vertically,   0 when moving horizontally
        int py = Mathf.Abs(dx); // 1 when moving horizontally, 0 when moving vertically
        move_plate_util.ray_plate(ref cp,i,c, dx + px, dy + py, 1, 1, skip_obs:false, capture_only:true);
        move_plate_util.ray_plate(ref cp,i,c, dx - px, dy - py, 1, 1, skip_obs:false, capture_only:true);

        if (data.mem.en_passant_x != -1) {
        if (Mathf.Abs(cp.x - data.mem.en_passant_x) == 1 && (cp.y + dy) == data.mem.en_passant_y) {
            move_plate_util.spawn_plate(i, c, data.mem.en_passant_x, data.mem.en_passant_y, true);
        }
    }
    }

    // =========================================================================
    // CORE RAY
    // =========================================================================

    public static void ray_plate(ref data.chess_piece cp, int idx, int color,
                   int dir_x, int dir_y, int step_count, int step_jump,
                   bool skip_obs = false, bool capture = true, bool capture_only = false) {

        int x = cp.x + dir_x * step_jump;
        int y = cp.y + dir_y * step_jump;

        for (int s = 0; s < step_count; s++) {
            if (!board_util.on_board(x, y)) break;

            ref data.board_cell cell = ref board_util.Cell(x, y);

            if (cell.has_piece == 1) {
                ref data.chess_piece target = ref data.mem.get_army(cell.piece_color).troop_list[cell.piece_index];
                if (!skip_obs) {
                    if (capture && target.player_color != cp.player_color)
                        move_plate_util.spawn_plate(idx, color, x, y, true);
                    break;
                }
                if (target.player_color != cp.player_color)
                    move_plate_util.spawn_plate(idx, color, x, y, true);
            } else {
                if (!capture_only)
                    move_plate_util.spawn_plate(idx, color, x, y, false);
            }

            x += dir_x * step_jump;
            y += dir_y * step_jump;
        }
    }

    public static void spawn_plate(int piece_index, int piece_color, int mx, int my, bool isAttack) {
        if (isAttack) {
            ref data.board_cell cell = ref board_util.Cell(mx, my);
            if (cell.has_piece == 1) {
                ref data.chess_piece target = ref data.mem.get_army(cell.piece_color).troop_list[cell.piece_index];
                if (target.piece_type == 5 || target.piece_type == 7) return; 
            }
        }

        if (!piece_util.IsSafeMove(piece_index, piece_color, mx, my, isAttack)) {
            return;
        }

        Sprite sprite = (isAttack && data.mem.mp_attack != null) ? data.mem.mp_attack : data.mem.mp_normal;

        data.move_plate mp;
        mp.rect                = rect_2d.create(board_util.board_to_world(mx), board_util.board_to_world(my), -2f);
        mp.attack              = isAttack;
        mp.piece_index         = piece_index;
        mp.piece_color         = piece_color;
        mp.mat_x               = mx;
        mp.mat_y               = my;
        mp.hover_sprite_scale  = 1.2f;
        mp.normal_sprite_scale = 0.8f;
        mp.rect.set_sprite(sprite);
        mp.rect.set_color(isAttack ? Color.red : Color.white);
        if (sprite != null) mp.rect.col.size = sprite.bounds.size;

        data.mem.move_plate_list.Add(mp);
    }

    public static void clear_move_plate() {
        foreach (data.move_plate mp in data.mem.move_plate_list)
            if (mp.rect != null) mp.rect.self_destroy();
        data.mem.move_plate_list.Clear();
    }
}