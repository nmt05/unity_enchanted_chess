using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour {

    // =========================================================================
    // GUI LAYOUT CONSTANTS  — adjust freely
    // =========================================================================

    // Main menu
    const float MENU_PVP_X   =  2.5f;  const float MENU_PVE_X   = -2.5f;
    const float MENU_CNT1_X  =  3f;    const float MENU_CNT2_X  =  0f;    const float MENU_CNT3_X = -3f;
    const float MENU_DIFF1_X =  4.5f;  const float MENU_DIFF2_X =  1.5f;
    const float MENU_DIFF3_X = -1.5f;  const float MENU_DIFF4_X = -4.5f;
    const float MENU_ROW_Y   =  -3.0f;    const float MENU_BACK_Y  = 1f;

    // In-game settings button
    const float SETTINGS_BTN_X  = 6.5f; const float SETTINGS_BTN_Y  = 7.0f;
    const float SETTINGS_MENU_X = 6.5f; const float SETTINGS_MENU_Y = 6.0f;
    const float SETTINGS_BACK_X = 6.5f; const float SETTINGS_BACK_Y = 5.0f;
    const float SETTINGS_LOSE_X = 9.5f; const float SETTINGS_LOSE_Y = 7.0f;
    const float CAM_SCALE_2P = 1.7f; const float CAM_SCALE_4P = 3.7f;
    // =========================================================================
    // LIFECYCLE
    // =========================================================================

    void Start()  {
        if(GATrainer.instance != null && GATrainer.instance.isTraining){
            data.mem.ai_difficulty = AIDifficulty.Normal;
            int pCount = GATrainer.instance.playersPerMatch;
            StartGame(pCount, pCount);
        }
        else{
            if (Camera.main != null) {
                Camera.main.clearFlags = CameraClearFlags.SolidColor; 
                Camera.main.backgroundColor = new Color(0.384f, 0.713f, 0.627f);
            }
            ShowMainMenu(); PlayThemeMusic();
        } 
    }

	int test = 0;
    void PlayThemeMusic() {
    if (data.mem.themesound != null && data.mem.audioSource != null) {
        data.mem.audioSource.clip = data.mem.themesound;
        data.mem.audioSource.loop = true; // Bật lặp lại

        data.mem.audioSource.Play();

    }
}
    void Update() {
        HandleMenuInput();
        HandleSettingsButton();
        bool isPvP1v1 = (data.mem.total_players == 2 && data.mem.play_against_AI == 0);
        bool isMyTurnOnline = !data.mem.is_online_game || (net_manager.instance != null && data.mem.current_player_color == net_manager.instance.my_color);
        if (isPvP1v1 && isMyTurnOnline) {
            card_util.handle_card_input(data.mem.current_player_color);
            
            if (Input.GetKeyDown(KeyCode.K)) card_util.add_card(0, CardType.Rock);
            if (Input.GetKeyDown(KeyCode.L)) card_util.add_card(1, CardType.DemonQueen);
        }
        if (data.mem.game_started == 0) return;

        cam_2d.zoom(mouse_util.scroll);
        if (mouse_util.right.hold == 1)
            cam_2d.move(-mouse_util.dx * 0.3f, -mouse_util.dy * 0.3f);

        zoom_on_evolving_piece();

		if(test == 0 && mouse_util.middle.hold == 1){
			test = 1;
			ShowLoseOverlay();
			data.mem.lose_ui_shown = true;
		}

		
        if (data.mem.gameOver) {
		if (!data.mem.lose_ui_shown && data.mem.turn_state >= 2) {
			ShowLoseOverlay();
			data.mem.lose_ui_shown = true;
		}

		return;
        }

        bool isAITurn = false;
        
        if (GATrainer.instance != null && GATrainer.instance.isTraining) {
            isAITurn = true; 
        } 
        else if (data.mem.play_against_AI == 1 && data.mem.current_player_color != 0) {
            isAITurn = true; 
        }

        if (isAITurn) {
            if (!data.mem.isAIThinking) StartCoroutine(AI_util.PlayAITurn());
            ClearEnPassant();
            return;
        }

        if (data.mem.menu_state == data.MenuState.Settings) return;

        HandlePieceInput();
        HandleMovePlateInput();
    }

    // =========================================================================
    // MENU — SHOW
    // =========================================================================

    void MakeMenuBg() {
        data.mem.main_screen_gui = rect_2d.create(0f, 0f);
        data.mem.main_screen_gui.set_sprite(data.mem.menu_bg_sprite);
        data.mem.main_screen_gui.set_sprite_size(20f, 10f);
        data.mem.main_screen_gui.set_collider_size(20f, 10f);
    }

    void ShowMainMenu() {
        gui_util.clear_menu();
        MakeMenuBg();
        data.mem.pvp_button = gui_util.make_button_sprite(MENU_PVP_X, MENU_ROW_Y, data.mem.pvp_btn_sprite,3.0f, 1.5f);
        data.mem.pve_button = gui_util.make_button_sprite(MENU_PVE_X, MENU_ROW_Y, data.mem.pve_btn_sprite,3.0f, 1.5f);
        data.mem.online_pvp_button = gui_util.make_button_wide(0f, MENU_ROW_Y - 1.2f, new Color(0.2f, 0.8f, 0.9f), "Online PvP");
        data.mem.menu_state = data.MenuState.Main;
        }

    void ShowPlayerCountMenu(bool is_pve) {
        gui_util.clear_menu();
        MakeMenuBg();
        if (is_pve) {
            data.mem.btn_count1 = gui_util.make_button_sprite(MENU_CNT1_X, MENU_ROW_Y, data.mem.pve_btn_sprite, 3.0f, 1.5f);
            data.mem.btn_count2 = gui_util.make_button_sprite(MENU_CNT2_X, MENU_ROW_Y, data.mem.pve2_btn_sprite, 3.0f, 1.5f);
            data.mem.btn_count3 = gui_util.make_button_sprite(MENU_CNT3_X, MENU_ROW_Y, data.mem.pve3_btn_sprite, 3.0f, 1.5f);
            data.mem.menu_state = data.MenuState.PickBotCount;
        } else {
            data.mem.btn_count1 = gui_util.make_button_sprite(MENU_CNT1_X, MENU_ROW_Y, data.mem.count1_btn_sprite, 3.0f, 1.5f);
            data.mem.btn_count2 = gui_util.make_button_sprite(MENU_CNT2_X, MENU_ROW_Y, data.mem.count2_btn_sprite, 3.0f, 1.5f);
            data.mem.btn_count3 = gui_util.make_button_sprite(MENU_CNT3_X, MENU_ROW_Y, data.mem.count3_btn_sprite, 3.0f, 1.5f);
            data.mem.menu_state = data.MenuState.PickPlayerCount;
        }
        data.mem.back_button = gui_util.make_button_sprite(0f, MENU_BACK_Y, data.mem.back_btn_sprite, 2.0f, 2.0f);
    }

    void ShowDifficultyMenu() {
        gui_util.clear_menu();
        MakeMenuBg();
        data.mem.btn_diff1   = gui_util.make_button_sprite(MENU_DIFF1_X, MENU_ROW_Y, data.mem.diff1_btn_sprite, 3.0f, 1.5f);
        data.mem.btn_diff2   = gui_util.make_button_sprite(MENU_DIFF2_X, MENU_ROW_Y, data.mem.diff2_btn_sprite, 3.0f, 1.5f);
        data.mem.btn_diff3   = gui_util.make_button_sprite(MENU_DIFF3_X, MENU_ROW_Y, data.mem.diff3_btn_sprite, 3.0f, 1.5f);
        data.mem.btn_diff4   = gui_util.make_button_sprite(MENU_DIFF4_X, MENU_ROW_Y, data.mem.diff4_btn_sprite, 3.0f, 1.5f);
        data.mem.back_button = gui_util.make_button_sprite(0f, MENU_BACK_Y, data.mem.back_btn_sprite, 2.0f, 2.0f);
        data.mem.menu_state  = data.MenuState.PickBotDifficulty;
    }

    // =========================================================================
    // MENU — INPUT
    // =========================================================================

    void HandleMenuInput() {
        bool anyClicked = false;
        switch (data.mem.menu_state) {
            case data.MenuState.Main:
                if (gui_util.clicked(data.mem.pvp_button)) {
                    ShowPlayerCountMenu(false);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.pve_button)) {
                    ShowPlayerCountMenu(true);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.online_pvp_button)) {
                    if (net_manager.instance != null) {
                        net_manager.instance.ConnectToPhoton();
                        ShowOnlineLobby();
                    } else {
                        Debug.LogError("net_manager not found! Add NetworkManager to scene.");
                    }
                    anyClicked = true;
                }
                break;

            case data.MenuState.PickPlayerCount:
                if (gui_util.clicked(data.mem.btn_count1)) {
                    StartGame(2, 0);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.btn_count2)) {
                    StartGame(3, 0);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.btn_count3)) {
                    StartGame(4, 0);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.back_button)) {
                    ShowMainMenu();
                    anyClicked = true;
                }
                break;

            case data.MenuState.PickBotCount:
                if (gui_util.clicked(data.mem.btn_count1)) {
                    PickBotCount(2, 1);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.btn_count2)) {
                    PickBotCount(3, 2);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.btn_count3)) {
                    PickBotCount(4, 3);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.back_button)) {
                    ShowMainMenu();
                    anyClicked = true;
                }
                break;

            case data.MenuState.PickBotDifficulty:
                if (gui_util.clicked(data.mem.btn_diff1)) {
                    StartGame_AI(AIDifficulty.Baby);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.btn_diff2)) {
                    StartGame_AI(AIDifficulty.Easy);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.btn_diff3)) {
                    StartGame_AI(AIDifficulty.Normal);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.btn_diff4)) {
                    StartGame_AI(AIDifficulty.Asean);
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.back_button)) {
                    ShowPlayerCountMenu(true);
                    anyClicked = true;
                }
                break;

            case data.MenuState.Settings:
                if (gui_util.clicked(data.mem.btn_main_menu)) {
                    ReturnToMainMenu();
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.back_button)) {
                    HideSettingsOverlay();
                    anyClicked = true;
                }
                break;

            case data.MenuState.OnlineLobby:
                if (gui_util.clicked(data.mem.btn_create_room)) {
                    net_manager.instance.CreateRoom();
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.btn_join_room)) {
                    net_manager.instance.ShowJoinRoomGUI();
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.btn_random_match)) {
                    net_manager.instance.JoinRandom();
                    anyClicked = true;
                }
                if (gui_util.clicked(data.mem.back_button)) {
                    if (net_manager.instance != null) net_manager.instance.LeaveOnlineGame();
                    ShowMainMenu();
                    anyClicked = true;
                }
                break;

            case data.MenuState.WaitingForOpponent:
                if (gui_util.clicked(data.mem.back_button)) {
                    if (net_manager.instance != null) net_manager.instance.LeaveOnlineGame();
                    ShowMainMenu();
                    anyClicked = true;
                }
                break;
        }
        if (anyClicked) {
        sound_util.play_sound(data.mem.clicksound); // Phát tiếng click
    }
    }

    void PickBotCount(int players, int bots) {
        data.mem.total_players = players;
        data.mem.bot_count     = bots;
        ShowDifficultyMenu();
    }

    void StartGame_AI(AIDifficulty diff) {
        data.mem.ai_difficulty = diff;
        StartGame(data.mem.total_players, data.mem.bot_count);
    }

    // =========================================================================
    // ONLINE LOBBY
    // =========================================================================

    void ShowOnlineLobby() {
        gui_util.clear_menu();
        MakeMenuBg();
        data.mem.btn_create_room = gui_util.make_button_wide(0f, 0.5f,  new Color(0.3f, 0.85f, 0.4f), "Tao Phong");
        data.mem.btn_join_room   = gui_util.make_button_wide(0f, -0.8f, new Color(0.3f, 0.6f, 0.95f), "Vao Phong");
        data.mem.btn_random_match= gui_util.make_button_wide(0f, -2.1f, new Color(0.95f, 0.7f, 0.2f), "Tim Nhanh");
        data.mem.back_button     = gui_util.make_button_wide(0f, -3.4f, new Color(0.8f, 0.3f, 0.3f),  "Quay Lai");
        data.mem.menu_state = data.MenuState.OnlineLobby;
    }

    public void ShowWaitingScreen() {
        gui_util.clear_menu();
        MakeMenuBg();
        string code = net_manager.instance != null ? net_manager.instance.GetRoomCode() : "???";
        data.mem.online_status_label = gui_util.make_button_wide(0f, 0.5f, new Color(0.9f, 0.9f, 0.3f), "Phong: " + code);
        gui_util.make_button_wide(0f, -0.5f, new Color(0.6f, 0.6f, 0.6f), "Cho doi thu...");
        data.mem.back_button = gui_util.make_button_wide(0f, -2.5f, new Color(0.8f, 0.3f, 0.3f), "Huy");
        data.mem.menu_state = data.MenuState.WaitingForOpponent;
    }

    // Called by net_manager RPC when both players are ready
    void StartOnlineGame() {
        data.mem.is_online_game = true;
        StartGame(2, 0);
    }

    // =========================================================================
    // GAME START
    // =========================================================================

    void StartGame(int total_players, int bot_count) {
        gui_util.clear_menu();

        data.mem.total_players        = total_players;
        data.mem.bot_count            = bot_count;
        data.mem.current_player_color = 0;
        data.mem.game_started         = 1;
        data.mem.menu_state           = data.MenuState.None;
        data.mem.play_against_AI      = bot_count > 0 ? 1 : 0;

        data.mem.armies = new data.army_data[total_players];
        for (int i = 0; i < total_players; i++)
            data.mem.armies[i] = new data.army_data(i);

        data.mem.white_army = data.mem.armies[0];
        if (total_players > 1) data.mem.black_army = data.mem.armies[1];

        if (total_players == 2) SetupBoard_2P();
        else                    SetupBoard_Cross(total_players);

        if (GATrainer.instance == null || !GATrainer.instance.isTraining) {
            sound_util.play_sound(data.mem.startSound);
            if(total_players == 2 && bot_count == 0)
            {
            card_util.init_card_table_white();

            card_util.init_card_table_black();
            }

            card_util.refresh_card_visuals(0);
            SpawnSettingsButton();
        } 
        else {//turn off camera in training mode
            GameObject camObj = GameObject.Find("main_camera");
            if (camObj != null) {
                Camera cam = camObj.GetComponent<Camera>();
                if (cam != null) cam.enabled = false;
            }
        }
    }

    // =========================================================================
    // BOARD — SHARED TILE HELPER
    // =========================================================================

    static void PlaceTile(int x, int y, Sprite s) {
        int fy = data.mem.board_h - 1 - y;
        ref data.board_cell c = ref board_util.Cell(x, y);
        c.tile_sprite  = s;
        c.tile         = rect_2d.create(board_util.board_to_world(x), board_util.board_to_world(fy), 0f);
        c.tile.set_sprite(s);
        c.tile.set_sprite_scale(1f, 1f);
        c.tile.col.enabled = false;
    }

    // =========================================================================
    // 2P BOARD
    // =========================================================================

    void SetupBoard_2P() {
        board_util.InitFlat(8, 8);

        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++) {
            bool light = (x + y) % 2 == 0;
            PlaceTile(x, y, light ? data.mem.board_tile0 : data.mem.board_tile1);
        }

        int[] back = { 1, 2, 3, 4, 5, 3, 2, 1 };
        for (int x = 0; x < 8; x++) piece_util.create_piece(x, 0, back[x], data.mem.armies[0], 0, +1);
        for (int x = 0; x < 8; x++) piece_util.create_piece(x, 1, 0,       data.mem.armies[0], 0, +1);
        for (int x = 0; x < 8; x++) piece_util.create_piece(x, 7, back[x], data.mem.armies[1], 0, -1);
        for (int x = 0; x < 8; x++) piece_util.create_piece(x, 6, 0,       data.mem.armies[1], 0, -1);

        cam_2d.scale(CAM_SCALE_2P);
    }

    // =========================================================================
    // CROSS BOARD (3P / 4P)
    // =========================================================================

    const int CROSS_SIZE = 24;
    const int ARM        = 8;

    static bool IsCrossCell(int x, int y) =>
        (x >= ARM && x < ARM * 2) || (y >= ARM && y < ARM * 2);

    void SetupBoard_Cross(int n) {
        board_util.InitFlat(CROSS_SIZE, CROSS_SIZE);

        for (int y = 0; y < CROSS_SIZE; y++)
        for (int x = 0; x < CROSS_SIZE; x++) {
            ref data.board_cell c = ref board_util.Cell(x, y);
            if (!IsCrossCell(x, y)) { c.valid = 0; continue; }

            c.valid = 1;
            bool light = (x + y) % 2 == 0;

            Sprite s0, s1;
            if      (y >= ARM * 2) { s0 = data.mem.board_tile0; s1 = data.mem.board_tile1; }
            else if (y < ARM)      { s0 = data.mem.board_tile2; s1 = data.mem.board_tile3; }
            else if (x < ARM)      { s0 = data.mem.board_tile0; s1 = data.mem.board_tile2; }
            else if (x >= ARM * 2) { s0 = data.mem.board_tile1; s1 = data.mem.board_tile3; }
            else                   { s0 = data.mem.board_tile0; s1 = data.mem.board_tile1; }

            PlaceTile(x, y, light ? s0 : s1);
        }

        PlaceArm_V(data.mem.armies[0],            0,            1, +1);
        PlaceArm_H(data.mem.armies[n==3 ? 1 : 2], 0,            1, +1);
        PlaceArm_H(data.mem.armies[n==3 ? 2 : 3], CROSS_SIZE-1, CROSS_SIZE-2, -1);
        if (n == 4) PlaceArm_V(data.mem.armies[1], CROSS_SIZE-1, CROSS_SIZE-2, -1);

        cam_2d.set_pos(10.5f, 10.5f);
        cam_2d.scale(CAM_SCALE_4P);
    }

    // =========================================================================
    // ARM PLACE HELPERS
    // =========================================================================

    static readonly int[] ARM_BACK = { 1, 2, 3, 5, 4, 3, 2, 1 };

    static void PlaceArm_V(data.army_data army, int back_row, int pawn_row, int dy) {
        for (int i = 0; i < ARM; i++) piece_util.create_piece(ARM+i, back_row, ARM_BACK[i], army, 0, dy);
        for (int i = 0; i < ARM; i++) piece_util.create_piece(ARM+i, pawn_row, 0,           army, 0, dy);
    }

    static void PlaceArm_H(data.army_data army, int back_col, int pawn_col, int dx) {
        for (int i = 0; i < ARM; i++) piece_util.create_piece(back_col, ARM+i, ARM_BACK[i], army, dx, 0);
        for (int i = 0; i < ARM; i++) piece_util.create_piece(pawn_col, ARM+i, 0,           army, dx, 0);
    }

    // =========================================================================
    // PIECE INPUT
    // =========================================================================

    void HandlePieceInput() {
        // Block input khi là lượt của đối thủ online
        if (data.mem.is_online_game && net_manager.instance != null
            && data.mem.current_player_color != net_manager.instance.my_color) return;

        int hovered_i = -1, hovered_color = -1;

        for (int color = 0; color < data.mem.total_players; color++) {
            data.army_data army = data.mem.armies[color];
            for (int i = 0; i < army.troop_count; i++) {
                ref data.chess_piece cp = ref army.troop_list[i];
                if (cp.rect == null) continue;

                bool isCurrent = cp.player_color == data.mem.current_player_color;

                if (cp.rect.mouse_unclick == 1) {
                    cp.rect.mouse_unclick = 0;
                    if (isCurrent) {
                        if (cp.selected == 1) {
                            cp.selected = 0;
                            data.mem.selected_a_piece = 0;
                            move_plate_util.clear_move_plate();
                        } else {
                            piece_util.unselect_all_piece();
                            data.mem.selected_a_piece = 1;
                            cp.selected = 1;
                            move_plate_util.clear_move_plate();
                            move_plate_util.spawn_plate(ref cp, i, color);
                        }
                    }
                }

                if (data.mem.selected_a_piece == 1) { cp.hovered = 0; continue; }
                if (cp.rect.mouse_hover == 0) cp.hovered = 0;
                if (cp.rect.mouse_hover == 1 && cp.selected == 0 && isCurrent && cp.hovered == 0) {
                    hovered_i = i; hovered_color = color;
                }
            }
        }

        if (data.mem.selected_a_piece == 0 && hovered_i >= 0) {
            data.army_data src = data.mem.armies[hovered_color];
            src.troop_list[hovered_i].hovered = 1;
            move_plate_util.clear_move_plate();
            move_plate_util.spawn_plate(ref src.troop_list[hovered_i], hovered_i, hovered_color);
        }

        ScalePieces();
    }

    void ScalePieces() {
        for (int color = 0; color < data.mem.total_players; color++) {
            data.army_data army = data.mem.armies[color];
            for (int i = 0; i < army.troop_count; i++) {
                ref data.chess_piece cp = ref army.troop_list[i];
                if (cp.rect == null) continue;
                float s = (cp.selected == 1 || cp.hovered == 1) ? cp.hover_sprite_scale : cp.normal_sprite_scale;
                cp.rect.set_sprite_scale(s, s);
            }
        }
    }

    // =========================================================================
    // MOVEPLATE INPUT
    // =========================================================================

    void HandleMovePlateInput() {
        // Block input khi là lượt của đối thủ online
        if (data.mem.is_online_game && net_manager.instance != null
            && data.mem.current_player_color != net_manager.instance.my_color) return;

        for (int i = 0; i < data.mem.move_plate_list.Count; i++) {
            data.move_plate mp = data.mem.move_plate_list[i];
            if (mp.rect == null) continue;

            bool  hov        = mp.rect.mouse_hover == 1;
            Color base_color = mp.attack ? Color.red : Color.white;
            mp.rect.set_color(hov ? base_color + new Color(0.4f, 0.4f, 0.4f, 0f) : base_color);
            float sc = hov ? mp.hover_sprite_scale : mp.normal_sprite_scale;
            mp.rect.set_sprite_scale(sc, sc);

            if (mp.rect.mouse_unclick != 1) continue;
            mp.rect.mouse_unclick = 0;

            ref data.chess_piece attacker = ref data.mem.armies[mp.piece_color].troop_list[mp.piece_index];

            if (!mp.attack) {
                if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                    sound_util.play_sound(data.mem.moveSound);
                TrySetEnPassant(ref attacker, mp.mat_y);
                TryCastle(ref attacker, mp.mat_x);
                attacker.has_moved = 1;
                piece_util.move_piece(ref attacker, mp.piece_index, mp.piece_color, mp.mat_x, mp.mat_y);
            } else {
                int ty = board_util.Cell(mp.mat_x, mp.mat_y).has_piece == 0 ? attacker.y : mp.mat_y;
                piece_util.piece_attack(ref attacker, mp.mat_x, ty, mp.rect.obj.transform.position);
                if (attacker.piece_type != 7)
                    piece_util.move_piece(ref attacker, mp.piece_index, mp.piece_color, mp.mat_x, mp.mat_y);
                attacker.has_moved = 1;
                ClearEnPassant();
            }

            // Gửi nước đi qua mạng cho đối thủ
            if (data.mem.is_online_game && net_manager.instance != null)
                net_manager.instance.SendMove(mp.piece_index, mp.mat_x, mp.mat_y, mp.attack);

            data.mem.selected_a_piece = 0;
            piece_util.unselect_all_piece();
            pvp_util.next_player_turn();
            move_plate_util.clear_move_plate();
            return;
        }
    }

    // move helpers
    static void TrySetEnPassant(ref data.chess_piece p, int ty) {
        if (p.piece_type == 0 && Mathf.Abs(ty - p.y) == 2) {
            data.mem.en_passant_x = p.x;
            data.mem.en_passant_y = p.y + (ty - p.y) / 2;
        } else {
            ClearEnPassant();
        }
    }

    static void TryCastle(ref data.chess_piece king, int tx) {
        if ((king.piece_type != 5 && king.piece_type != 7) || Mathf.Abs(tx - king.x) != 2) return;
        bool kingside = tx > king.x;
        int  rookX    = kingside ? king.x + 3 : king.x - 4;
        int  newRookX = kingside ? king.x + 1 : king.x - 1;

        ref data.board_cell rookCell = ref board_util.Cell(rookX, king.y);
        if (rookCell.has_piece != 1) return;

        ref data.chess_piece rook = ref data.mem.get_army(rookCell.piece_color).troop_list[rookCell.piece_index];
        piece_util.move_piece(ref rook, rookCell.piece_index, rookCell.piece_color, newRookX, king.y);
        rook.has_moved = 1;
    }

    static void ClearEnPassant() {
        data.mem.en_passant_x = -1;
        data.mem.en_passant_y = -1;
    }

    // =========================================================================
    // EVO ZOOM
    // =========================================================================

    void zoom_on_evolving_piece() {
        if (data.mem.evolving_signal == 1) {
            data.mem.evolving_signal = 0;
            data.mem.evoStartX       = cam_2d.x;
            data.mem.evoStartY       = cam_2d.y;
            data.mem.evoStartSize    = cam_2d.size;
            data.mem.evoTargetSize   = cam_2d.size * 0.7f;
            data.mem.evoTimer        = 0f;
            data.mem.evoZoom         = 1;
            cam_2d.set_pos(data.mem.evolving_pos.x, data.mem.evolving_pos.y);
        }

        if (data.mem.evoZoom != 1) return;

        data.mem.evoTimer += Time.deltaTime;
        float t = data.mem.evoTimer / 0.5f;

        if (t <= 1f) {
            cam_2d.size = Mathf.Lerp(data.mem.evoStartSize, data.mem.evoTargetSize, t);
        } else if (t <= 2f) {
            float k = t - 1f;
            cam_2d.size = Mathf.Lerp(data.mem.evoTargetSize, data.mem.evoStartSize, k);
            cam_2d.x    = Mathf.Lerp(data.mem.evolving_pos.x, data.mem.evoStartX, k);
            cam_2d.y    = Mathf.Lerp(data.mem.evolving_pos.y, data.mem.evoStartY, k);
        } else {
            cam_2d.size = data.mem.evoStartSize;
            cam_2d.x    = data.mem.evoStartX;
            cam_2d.y    = data.mem.evoStartY;
            data.mem.evoZoom = 0;
        }

        cam_2d.apply();
    }

    // =========================================================================
    // SETTINGS  (in-game gear button + overlay)
    // =========================================================================

	void SpawnSettingsButton() {
		DestroyRef(ref data.mem.settings_button);
		Vector2 off = GetSettingsOffset();
		data.mem.settings_button = gui_util.make_button_sprite(SETTINGS_BTN_X + off.x, SETTINGS_BTN_Y + off.y, data.mem.settings_btn_sprite, 1f, 1f);
		data.mem.menu_rects.Remove(data.mem.settings_button);
	}


	void ShowSettingsOverlay() {
		Vector2 off = GetSettingsOffset();
		data.mem.btn_main_menu = gui_util.make_button_sprite(SETTINGS_MENU_X + off.x, SETTINGS_MENU_Y + off.y, data.mem.back_btn_sprite, 1f, 1f);
		data.mem.back_button = gui_util.make_button_sprite(SETTINGS_BACK_X + off.x, SETTINGS_BACK_Y + off.y, data.mem.menu_btn_sprite, 1f, 1f);
		data.mem.menu_state = data.MenuState.Settings;
	}

    void HideSettingsOverlay() {
        DestroyRef(ref data.mem.btn_main_menu);
        DestroyRef(ref data.mem.back_button);
        data.mem.menu_state = data.MenuState.None;
    }

    void HandleSettingsButton() {
        if (data.mem.settings_button == null || !gui_util.clicked(data.mem.settings_button)) return;
        if (data.mem.menu_state == data.MenuState.Settings) HideSettingsOverlay();
        else                                                ShowSettingsOverlay();
    }

	Vector2 GetSettingsOffset(){
		if (data.mem.total_players == 2) return Vector2.zero; // 2P = centered
		return new Vector2(10.5f + 1f, 10.5f + 2f); // 3P / 4P = board centered at (10.5, 10.5)
	}
	
	void ShowLoseOverlay() {
		Vector2 off = GetSettingsOffset();

		DestroyRef(ref data.mem.lose_button);

		string txt = (data.mem.turn_state == 2) ? "Lose" : "Draw";
		data.mem.lose_button = gui_util.make_button(SETTINGS_LOSE_X + off.x, SETTINGS_LOSE_Y + off.y, Color.red,txt);
		data.mem.btn_main_menu = gui_util.make_button_sprite(SETTINGS_MENU_X + off.x, SETTINGS_MENU_Y + off.y, data.mem.back_btn_sprite, 1f, 1f);
		data.mem.menu_state = data.MenuState.Settings;
	}
    // =========================================================================
    // RETURN TO MAIN MENU
    // =========================================================================

    void ReturnToMainMenu() {
        StopAllCoroutines();

        // Ngắt kết nối Photon nếu đang online
        if (data.mem.is_online_game && net_manager.instance != null)
            net_manager.instance.LeaveOnlineGame();

        // Destroy board
        if (data.mem.board != null) {
            foreach (var cell in data.mem.board)
                if (cell.tile != null) cell.tile.self_destroy();
            data.mem.board = null;
        }

        // Destroy pieces
        if (data.mem.armies != null)
            foreach (var army in data.mem.armies)
                if (army != null)
                    for (int i = 0; i < army.troop_count; i++)
                        if (army.troop_list[i].rect != null)
                            army.troop_list[i].rect.self_destroy();


        DestroyRef(ref data.mem.card_table_obj_w);

        ClearCardHandVisual(data.mem.white_hand_visual);
        ClearCardHandVisual(data.mem.black_hand_visual);
        data.mem.whiteHand.Clear();
        data.mem.blackHand.Clear();
        move_plate_util.clear_move_plate();
        DestroyRef(ref data.mem.settings_button);
        DestroyRef(ref data.mem.card_table_obj_b);
		DestroyRef(ref data.mem.lose_button);
		data.mem.lose_ui_shown = false;
        // Reset state
        data.mem.game_started         = 0;
        data.mem.gameOver             = false;
        data.mem.is_online_game       = false;
        data.mem.current_player_color = 0;
        data.mem.selected_a_piece     = 0;
        data.mem.play_against_AI      = 0;
        data.mem.bot_count            = 0;
        data.mem.isAIThinking         = false;
        data.mem.evoZoom              = 0;
        data.mem.evolving_signal      = 0;
        ClearEnPassant();
        data.mem.whiteHand.Clear();
        data.mem.blackHand.Clear();
        data.mem.white_hand_visual.card_rects.Clear();
        data.mem.black_hand_visual.card_rects.Clear();

        // Reset armies (keep total_players for the inverse scale below, then reset it)
        int last_players = data.mem.total_players;
        data.mem.total_players = 2;
        data.mem.white_army    = new data.army_data(0);
        data.mem.black_army    = new data.army_data(1);
        data.mem.armies        = new data.army_data[] { data.mem.white_army, data.mem.black_army };

        // Undo camera zoom — scale() multiplies current size, so we invert.
        cam_2d.set_pos(0f, 0f);
        cam_2d.scale(last_players == 2 ? 1f / CAM_SCALE_2P : 1f / CAM_SCALE_4P);

        ShowMainMenu();
    }

    // =========================================================================
    // UTILITY
    // =========================================================================

    static void DestroyRef(ref rect_2d r) {
        if (r == null) return;
        r.self_destroy();
        r = null;
    }
        void ClearCardHandVisual(data.CardHand hand) {
        if (hand != null && hand.card_rects != null) {
            for (int i = 0; i < hand.card_rects.Count; i++) {
                if (hand.card_rects[i] != null) {
                    hand.card_rects[i].self_destroy();
                }
            }
            hand.card_rects.Clear(); // Làm trống list sau khi destroy
        }
    }
    
}
