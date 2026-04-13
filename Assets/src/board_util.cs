using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class board_util {

    public static bool on_board(int x, int y) {
        if (x < 0 || y < 0 || x >= data.mem.board_w || y >= data.mem.board_h) return false;
        return board_util.Cell(x, y).valid == 1;
    }

    public static ref data.board_cell Cell(int x, int y) {
        return ref data.mem.board[x + y * data.mem.board_w];
    }

    public static void set_cell(int x, int y, int color, int idx) {
        ref data.board_cell cell = ref board_util.Cell(x, y);
        cell.has_piece   = 1;
        cell.piece_color = color;
        cell.piece_index = idx;
    }

    public static void clear_cell(int x, int y) {
		board_util.Cell(x, y).has_piece = 0;
    }

    public static float board_to_world(int v) => v * 1.28f - 4.48f;







    // Allocate a clean flat board of w×h cells, all valid and empty.
    public static void InitFlat(int w, int h) {
        data.mem.board_w = w;
        data.mem.board_h = h;
        data.mem.board   = new data.board_cell[w * h];
 
        for (int i = 0; i < w * h; i++) {
            data.mem.board[i].valid     = 1;
            data.mem.board[i].has_piece = 0;
        }
    }
 

 
    // Board world-space position.
    // Centered: tile 0 maps to -(board_w/2 - 0.5) * cell_size, etc.
    //public static float board_to_world(int v) {
    //    return v * 1.28f - (data.mem.board_w * 1.28f * 0.5f) + 0.64f;
    //}
 
    // World-space board coordinate (inverse of above).
    public static int world_to_board(float w) {
        return Mathf.RoundToInt((w - 0.64f + data.mem.board_w * 1.28f * 0.5f) / 1.28f);
    }
 
    // True if (x,y) is inside the board bounds AND the cell is valid.
    public static bool IsPlayable(int x, int y) {
        if (x < 0 || y < 0 || x >= data.mem.board_w || y >= data.mem.board_h) return false;
        return data.mem.board[x + y * data.mem.board_w].valid == 1;
    }
}