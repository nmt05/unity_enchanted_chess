using UnityEngine;

public static class pvp_util {

    // =========================================================
    // CHECK ANY VALID MOVE
    // =========================================================
    public static bool CheckForAnyValidMove(int color) {
        var army = data.mem.get_army(color);

        for (int i = 0; i < army.troop_count; i++) {
            ref var cp = ref army.troop_list[i];
            if (cp.rect == null) continue;

            for (int x = 0; x < data.mem.board_w; x++) {
            for (int y = 0; y < data.mem.board_h; y++) {

                if (!piece_util.can_move_to(ref cp, x, y)) continue;

                bool atk = board_util.Cell(x, y).has_piece == 1;
                if (piece_util.IsSafeMove(i, color, x, y, atk))
                    return true;
            }}
        }
        return false;
    }

    // =========================================================
    // CHECK KING STATE
    // =========================================================
    static bool IsKingChecked(int color) {
        var army = data.mem.get_army(color);

        for (int i = 0; i < army.troop_count; i++) {
            ref var cp = ref army.troop_list[i];

            if (cp.piece_type == 5 && cp.rect != null) {
                return AI_util.IsSquareAttacked(cp.x, cp.y, color);
            }
        }
        return false;
    }

    // =========================================================
    // FIND NEXT ALIVE PLAYER
    // =========================================================
    static int GetNextPlayer(int current) {
        int n = data.mem.total_players;
        int next = (current + 1) % n;

        for (int i = 0; i < n; i++) {
            if (data.mem.armies[next].troop_count > 0)
                return next;

            next = (next + 1) % n;
        }
        return next;
    }

    // =========================================================
    // NEXT TURN
    // =========================================================

    public static void next_player_turn() {
        int next = GetNextPlayer(data.mem.current_player_color);
        data.mem.current_player_color = next;
        data.mem.current_turn_count++;
        
        // --- THÊM LOGIC ĐẾM SỐ NƯỚC KHÔNG TIẾN TRIỂN ---
        data.mem.turns_without_progress++; 
        
        bool checkedKing = IsKingChecked(next);
        bool hasMove     = CheckForAnyValidMove(next);
        data.mem.turn_state = 0;

        bool forceDraw = false;
        
        int noProgressLimit = data.mem.total_players * 35; 
        
        // 🚨 THÊM GIỚI HẠN SINH TỬ: Dù có tiến triển hay không, tới Turn này là CHÉM!
        int absoluteLimit = data.mem.total_players * 120; 

        // Ép hòa nếu vượt quá giới hạn rác HOẶC quá giới hạn sinh tử
        if (GATrainer.instance != null && GATrainer.instance.isTraining) {
            if (data.mem.turns_without_progress >= noProgressLimit || data.mem.current_turn_count >= absoluteLimit) {
                forceDraw = true;
                hasMove = false;
            }
        }

        if (!hasMove) {
            data.mem.gameOver = true;
            
            if (forceDraw) {
                data.mem.turn_state = 3; // Hòa
                if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                    Debug.Log("<color=yellow>HÒA DO LUẬT 50 NƯỚC KHÔNG TIẾN TRIỂN!</color>");
            } else {
                data.mem.turn_state = checkedKing ? 2 : 3;
                if (data.mem.endSound) 
                    if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                        sound_util.play_sound(data.mem.endSound);
                
                if (checkedKing) 
                    if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                        Debug.Log($"<color=red>CHECKMATE! Player {next} lost.</color>");
                else 
                    if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                        Debug.Log($"<color=yellow>STALEMATE! Player {next} has no moves.</color>");
            }

            // BÁO CÁO CHI TIẾT CHO GA TRAINER (DẠNG MẢNG)
            if (GATrainer.instance != null && GATrainer.instance.isTraining) {
                bool isDraw = !checkedKing || forceDraw;
                
                // Thu thập điểm của tất cả các Player
                int pCount = GATrainer.instance.playersPerMatch;
                float[] scores = new float[pCount];
                for (int i = 0; i < pCount; i++) {
                    scores[i] = CalculateMaterial(i);
                }

                int matchNum = GATrainer.instance.currentMatchIndex + 1;
                int totalMatches = GATrainer.instance.populationSize / pCount;
                Debug.Log($"<color=orange>=> Xong trận {matchNum}/{totalMatches} (Tổng: {data.mem.current_turn_count} Turns)</color>");

                GATrainer.instance.ReportMatchResult(next, isDraw, data.mem.current_turn_count, scores);
            }
            return;
        }

        if (checkedKing) {
            data.mem.turn_state = 1;
            if (data.mem.checkSound) 
                if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                    sound_util.play_sound(data.mem.checkSound);
            if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                Debug.Log($"<color=red>CHECK! Player {next} king is threatened.</color>");
        }
    }

    static float CalculateMaterial(int color) {
        float score = 0;
        var army = data.mem.get_army(color);
        for (int i = 0; i < army.troop_count; i++) {
            ref var cp = ref army.troop_list[i];
            if (cp.rect != null) { // Quân cờ còn sống
                score += AI_util.GetPieceValue(ref cp);
            }
        }
        return score;
    }
}