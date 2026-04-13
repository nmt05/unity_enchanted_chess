using System.Collections.Generic;
using UnityEngine;

public enum CardType { Buff1, Buff2, Debuff, GodQueen, DemonQueen, Event, Item, Rock}
public enum PieceType { Light, ELight, KHeavy, BHeavy, RHeavy, Core, Rock }
public enum AIDifficulty { Baby, Easy, Normal, Asean }

public class data : MonoBehaviour {
    public static data mem;

    // =========================================================================
    // MENU STATE
    // =========================================================================

    public enum MenuState { None, Main, PickPlayerCount, PickBotCount, PickBotDifficulty, Settings }
    public MenuState menu_state = MenuState.None;

    // =========================================================================
    // BOARD CELL
    // =========================================================================

    public struct board_cell {
        public int     valid;
        public int     has_piece;
        public int     piece_color;
        public int     piece_index;
        public rect_2d tile;
        public Sprite  tile_sprite;
    }

    // =========================================================================
    // CHESS PIECE
    // =========================================================================

    public struct chess_piece {
        public rect_2d rect;
        public Sprite  normal_sprite;
        public Sprite  evo_sprite0;
        public Sprite  evo_sprite1;
        public Sprite  evo_sprite2;
        public int     x;
        public int     y;
        public int     player_color;
        public int     score;
        public int     score_to_envo;
        public PieceType unitType;
        public int     piece_type;   // 0=Pawn 1=Rook 2=Knight 3=Bishop 4=Queen 5=King 6=DQueen 7=KingWithGun
        public int     evolved;
        public int     evolved_type;
        public int     selected;
        public int     hovered;
        public float   hover_sprite_scale;
        public float   normal_sprite_scale;
        public int     shield;
        // direction this piece's pawns advance: (0,+1)=up (0,-1)=down (+1,0)=right (-1,0)=left
        public int     pawn_dir_x;
        public int     pawn_dir_y;
        public int has_moved; //0 = not moved. 1 = moved
    }

    // =========================================================================
    // MOVE PLATE
    // =========================================================================

    public struct move_plate {
        public rect_2d rect;
        public bool    attack;
        public int     mat_x;
        public int     mat_y;
        public int     piece_index;
        public int     piece_color;
        public float   hover_sprite_scale;
        public float   normal_sprite_scale;
    }

    // =========================================================================
    // ARMY DATA
    // =========================================================================

    public class army_data {
        public int           color;
        public chess_piece[] troop_list  = new chess_piece[32]; // more room for 4P
        public int           troop_count = 0;
        public army_data(int color) { this.color = color; }
    }

    // =========================================================================
    // CARD / AI  (unchanged)
    // =========================================================================

    public class Card {
        public string   cardName;
        public CardType type;
        public int      value;
        public Sprite   artwork;
        public string   description;
    }

    public List<Card> whiteHand = new List<Card>();
    public List<Card> blackHand = new List<Card>();
    public GameObject cardPrefab;


    public class CardHand {
        public List<rect_2d> card_rects = new List<rect_2d>();
        public float start_x = -3.2f;  // Vị trí bắt đầu hàng thẻ
        public float spacing = 2.13f; // Khoảng cách giữa các thẻ
        public float y_pos = -8f;  // Vị trí Y nằm dưới bàn cờ
    }
    public rect_2d card_table_obj_w;
    public rect_2d card_table_obj_b;
    public CardHand white_hand_visual = new CardHand();
    public CardHand black_hand_visual = new CardHand{y_pos = 8f};

    public struct AIMove {
        public int  piece_index;
        public int  targetX;
        public int  targetY;
        public bool isAttack;
    }

    public class MCTSNode {
        public AIMove         move;
        public MCTSNode       parent;
        public List<MCTSNode> children     = new List<MCTSNode>();
        public float          wins         = 0;
        public int            visits       = 0;
        public int            colorToMove;
        public List<AIMove>   untriedMoves;
    }

    public struct UndoData {
        public int attacker_color;
        public int attacker_idx;
        public int old_x;
        public int old_y;
        public int old_score;
        
        public bool is_attack;
        public int target_color;
        public int target_idx;
        public int target_score;
        public rect_2d target_rect;
        
        public bool is_king_dead;
    }

    // =========================================================================
    // BOARD
    // =========================================================================

    public int          board_w;
    public int          board_h;
    public board_cell[] board;
    public chess_piece  void_piece;

    // N-player armies
    public army_data[]  armies;          // length = total_players
    // legacy shortcuts
    public army_data    white_army;
    public army_data    black_army;

    public army_data get_army(int color)  => armies[color];
    public army_data get_enemy(int color) => armies[(color + 1) % total_players];

    // =========================================================================
    // GUI BUTTONS
    // =========================================================================
    [Header("Menu Sprites")]
    public Sprite camera_bg_sprite;
    public Sprite menu_bg_sprite;
    public Sprite pve2_btn_sprite;
    public Sprite pve3_btn_sprite;
    public Sprite pvp_btn_sprite;
    public Sprite pve_btn_sprite;
    public Sprite back_btn_sprite;
    public Sprite count1_btn_sprite;
    public Sprite count2_btn_sprite;
    public Sprite count3_btn_sprite;
    public Sprite diff1_btn_sprite; 
    public Sprite diff2_btn_sprite;
    public Sprite diff3_btn_sprite;
    public Sprite diff4_btn_sprite;
    public Sprite settings_btn_sprite;
    public Sprite menu_btn_sprite;
    public rect_2d main_screen_gui;


    public rect_2d pvp_button;
    public rect_2d pve_button;
    public rect_2d btn_count1;
    public rect_2d btn_count2;
    public rect_2d btn_count3;
    public rect_2d back_button;

    //difficult
    public rect_2d settings_button;   // in-game gear button (always visible)
    public rect_2d btn_main_menu;     // inside settings overlay
	public rect_2d lose_button;


    public rect_2d btn_diff1;
    public rect_2d btn_diff2;
    public rect_2d btn_diff3;
    public rect_2d btn_diff4;

    // flat list for easy bulk-destroy
    public List<rect_2d> menu_rects = new List<rect_2d>();
    [Header("Card Sprites")]
    public Sprite Card_board_bg_sprite;
    public Sprite card_plus1;      
    public Sprite card_plus2;    
    public Sprite card_demon;     
    public Sprite card_expandc;   
    public Sprite card_expandr;    
    public Sprite card_god;       
    public Sprite card_gun;       
    public Sprite card_thunder;    
    public Sprite card_rock;
    public Sprite rock;

    // =========================================================================
    // GAME STATE
    // =========================================================================

    public List<move_plate> move_plate_list      = new List<move_plate>();
    public bool             gameOver             = false;
    public int              current_player_color = 0;
    public int              selected_a_piece     = 0;
    public int              game_started         = 0;
    public int              total_players        = 2;
    public int              bot_count            = 0;
    public int              play_against_AI      = 0;
	public int              evolving_signal      = 0;
	public Vector3          evolving_pos;
	public float evoTimer = 0f;
	public int   evoZoom = 0;

	public float evoStartSize;
	public float evoTargetSize;

	public float evoStartX, evoStartY;
	public int              zoom_cross_board    = 0;
	public int turn_state = 0; //  turn result signal 0 = normal 1 = check 2 = checkmate 3 = stalemate
	public bool lose_ui_shown = false;
    public AIDifficulty ai_difficulty = AIDifficulty.Baby;
    public int current_turn_count = 0;
    // =========================================================================
    // AI
    // =========================================================================


    public int        aiColor     = 1;
    public bool       isAIThinking= false;
    public static AIMove    lastAIMove           = new AIMove { piece_index = -1 };
    public List<BotDNA> pveBrains = new List<BotDNA>();
    public int turns_without_progress = 0;

    // =========================================================================
    // SPRITES
    // =========================================================================

    public Sprite board_tile0, board_tile1, board_tile2, board_tile3;

    public Sprite wp_pawn, wp_rook, wp_knight, wp_bishop, wp_queen, wp_king;
    public Sprite bp_pawn, bp_rook, bp_knight, bp_bishop, bp_queen, bp_king;

    public Sprite wp_e_rook, wp_e_knight, wp_e_bishop, wp_e_queen, wp_e_king, wp_e_dqueen;
    public Sprite bp_e_rook, bp_e_knight, bp_e_bishop, bp_e_queen, bp_e_king, bp_e_dqueen;

    public Sprite wp_e_pawn_knight, wp_e_pawn_bishop, wp_e_pawn_rook;
    public Sprite bp_e_pawn_knight, bp_e_pawn_bishop, bp_e_pawn_rook;

    public Sprite blue_pawn, blue_rook, blue_knight, blue_bishop, blue_queen, blue_king, e_blue_pawn_knight, e_blue_pawn_bishop, e_blue_pawn_rook,e_blue_rook, e_blue_knight, e_blue_bishop, e_blue_queen, e_blue_king;
    public Sprite green_pawn, green_rook, green_knight, green_bishop, green_queen, green_king, e_green_pawn_knight, e_green_pawn_bishop, e_green_pawn_rook,e_green_rook, e_green_knight, e_green_bishop, e_green_queen, e_green_king;
    
    public Sprite mp_normal, mp_attack, rect_2d_sprite;

    // =========================================================================
    // AUDIO
    // =========================================================================

    public AudioSource audioSource;
    public AudioClip   moveSound, captureSound, checkSound, startSound, endSound, timeLess;
    public AudioClip   pawnEvolveSound, knightEvolveSound, GodqueenEvolveSound, kingEvolveSound, DemonqueenEvolveSound,bishopEvolveSound, rookEvolveSound, burstSound;
    public AudioClip   cardDrawSound, cardPlaySound, cardBuffSound, cardDebuffSound;
    public AudioClip   themesound, clicksound, winSound, loseSound;
    public Sprite mutebutn, unmutebtn
    ;

    // =========================================================================
    // CARDS
    // =========================================================================


    // =========================================================================
    // SPECIAL
    // =========================================================================
    public int en_passant_x = -1;
    public int en_passant_y = -1;


    // =========================================================================
    // INIT
    // =========================================================================

    void Awake() {
        mem        = this;
        white_army = new army_data(0);
        black_army = new army_data(1);
        armies     = new army_data[] { white_army, black_army };
    }
}

[System.Serializable]
public class GAPopulationData {
    public int generation;
    public List<BotDNA> dnaList;
}