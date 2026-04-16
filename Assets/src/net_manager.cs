using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class net_manager : MonoBehaviourPunCallbacks {

    public static net_manager instance;

    // === State ===
    public bool is_online = false;       // đang ở chế độ online
    public int  my_color  = -1;          // 0=white (master), 1=black (joiner)

    // === Room name input (OnGUI) ===
    string room_input = "";
    string status_msg = "";
    bool   show_gui   = false;
    bool   joining    = false;           // true = đang nhập room name để join
    bool   ready      = false;           // true khi đã vào lobby, sẵn sàng tạo/join room

    void Awake() {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }

    // =========================================================================
    // CONNECTION
    // =========================================================================

    public void ConnectToPhoton() {
        status_msg = "Dang ket noi...";
        show_gui   = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        status_msg = "Da ket noi! Chon phong.";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        ready = true;
        status_msg = "San sang! Chon phong.";
    }

    // =========================================================================
    // ROOM OPERATIONS
    // =========================================================================

    public void CreateRoom() {
        if (!ready) { status_msg = "Chua ket noi xong, cho chut..."; return; }
        string code = Random.Range(1000, 9999).ToString();
        room_input = code;
        RoomOptions opt = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(code, opt);
        status_msg = "Tao phong: " + code + " ...";
    }

    public void JoinRoom(string roomName) {
        if (!ready) { status_msg = "Chua ket noi xong, cho chut..."; return; }
        PhotonNetwork.JoinRoom(roomName);
        status_msg = "Dang vao phong " + roomName + " ...";
    }

    public void JoinRandom() {
        if (!ready) { status_msg = "Chua ket noi xong, cho chut..."; return; }
        PhotonNetwork.JoinRandomRoom();
        status_msg = "Tim phong ngau nhien...";
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        // Không tìm thấy phòng → tạo mới
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        status_msg = "Loi tao phong: " + message;
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        status_msg = "Loi vao phong: " + message;
    }

    // =========================================================================
    // ROOM JOINED
    // =========================================================================

    public override void OnJoinedRoom() {
        my_color = PhotonNetwork.IsMasterClient ? 0 : 1;
        room_input = PhotonNetwork.CurrentRoom.Name;
        status_msg = "Phong: " + PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            // Cả 2 đã vào → master bắt đầu game
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC("RPC_StartGame", RpcTarget.All);
        } else {
            // Hiện waiting screen
            Game game = Object.FindObjectOfType<Game>();
            if (game != null) game.ShowWaitingScreen();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            photonView.RPC("RPC_StartGame", RpcTarget.All);
        }
    }

    // =========================================================================
    // DISCONNECT
    // =========================================================================

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if (!data.mem.is_online_game) return;
        data.mem.gameOver = true;
        data.mem.turn_state = 3;
        status_msg = "Doi thu da ngat ket noi!";
        Debug.Log("<color=red>Opponent disconnected!</color>");
    }

    public override void OnDisconnected(DisconnectCause cause) {
        if (data.mem.is_online_game) {
            data.mem.is_online_game = false;
            is_online = false;
            Debug.Log("<color=red>Disconnected: " + cause + "</color>");
        }
        status_msg = "";
        show_gui   = false;
    }

    public void LeaveOnlineGame() {
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
        data.mem.is_online_game = false;
        is_online  = false;
        ready      = false;
        show_gui   = false;
        status_msg = "";
    }

    // =========================================================================
    // RPC — START GAME
    // =========================================================================

    [PunRPC]
    void RPC_StartGame() {
        my_color = PhotonNetwork.IsMasterClient ? 0 : 1;
        data.mem.is_online_game = true;
        is_online = true;
        show_gui  = false;

        // Tìm Game MonoBehaviour để gọi StartGame
        Game game = Object.FindObjectOfType<Game>();
        if (game != null) {
            // Gọi StartOnlineGame thông qua SendMessage vì StartGame là private
            game.SendMessage("StartOnlineGame");
        }
    }

    // =========================================================================
    // RPC — MOVE
    // =========================================================================

    public void SendMove(int piece_index, int targetX, int targetY, bool isAttack) {
        photonView.RPC("RPC_PlayMove", RpcTarget.Others, piece_index, targetX, targetY, isAttack);
    }

    [PunRPC]
    void RPC_PlayMove(int piece_index, int targetX, int targetY, bool isAttack) {
        int color = data.mem.current_player_color;
        ref data.chess_piece attacker = ref data.mem.armies[color].troop_list[piece_index];

        if (!isAttack) {
            sound_util.play_sound(data.mem.moveSound);
            // En passant
            if (attacker.piece_type == 0 && Mathf.Abs(targetY - attacker.y) == 2) {
                data.mem.en_passant_x = attacker.x;
                data.mem.en_passant_y = attacker.y + (targetY - attacker.y) / 2;
            } else {
                data.mem.en_passant_x = -1;
                data.mem.en_passant_y = -1;
            }
            // Castle
            if ((attacker.piece_type == 5 || attacker.piece_type == 7) && Mathf.Abs(targetX - attacker.x) == 2) {
                bool kingside = targetX > attacker.x;
                int  rookX    = kingside ? attacker.x + 3 : attacker.x - 4;
                int  newRookX = kingside ? attacker.x + 1 : attacker.x - 1;
                ref data.board_cell rookCell = ref board_util.Cell(rookX, attacker.y);
                if (rookCell.has_piece == 1) {
                    ref data.chess_piece rook = ref data.mem.get_army(rookCell.piece_color).troop_list[rookCell.piece_index];
                    piece_util.move_piece(ref rook, rookCell.piece_index, rookCell.piece_color, newRookX, attacker.y);
                    rook.has_moved = 1;
                }
            }
            attacker.has_moved = 1;
            piece_util.move_piece(ref attacker, piece_index, color, targetX, targetY);
        } else {
            int ty = board_util.Cell(targetX, targetY).has_piece == 0 ? attacker.y : targetY;
            piece_util.piece_attack(ref attacker, targetX, ty, attacker.rect.obj.transform.position);
            if (attacker.piece_type != 7)
                piece_util.move_piece(ref attacker, piece_index, color, targetX, targetY);
            attacker.has_moved = 1;
            data.mem.en_passant_x = -1;
            data.mem.en_passant_y = -1;
        }

        data.mem.selected_a_piece = 0;
        piece_util.unselect_all_piece();
        pvp_util.next_player_turn();
        move_plate_util.clear_move_plate();
    }

    // =========================================================================
    // RPC — CARD
    // =========================================================================

    public void SendCardAction(int player_color, int cardIndex, int tx, int ty) {
        photonView.RPC("RPC_PlayCard", RpcTarget.Others, player_color, cardIndex, tx, ty);
    }

    [PunRPC]
    void RPC_PlayCard(int player_color, int cardIndex, int tx, int ty) {
        var cardDataList = (player_color == 0) ? data.mem.whiteHand : data.mem.blackHand;
        if (cardIndex < 0 || cardIndex >= cardDataList.Count) return;

        data.Card card = cardDataList[cardIndex];

        // Tạo effectPos từ board coords
        Vector3 effectPos = new Vector3(board_util.board_to_world(tx), board_util.board_to_world(ty), 0);

        if (board_util.on_board(tx, ty)) {
            ref data.board_cell cell = ref board_util.Cell(tx, ty);
            data.chess_piece targetPiece = data.mem.void_piece;
            if (cell.has_piece == 1)
                targetPiece = data.mem.get_army(cell.piece_color).troop_list[cell.piece_index];

            bool success = card_util.apply_card_effect(card, ref targetPiece, effectPos);
            if (success) {
                cardDataList.RemoveAt(cardIndex);
                card_util.refresh_card_visuals(player_color);
                sound_util.play_sound(data.mem.cardPlaySound);
            }
        }
    }

    // =========================================================================
    // RPC — ADD CARD (sync card draws từ evolution)
    // =========================================================================

    public void SendAddCard(int player_color, int cardType) {
        photonView.RPC("RPC_AddCard", RpcTarget.Others, player_color, cardType);
    }

    [PunRPC]
    void RPC_AddCard(int player_color, int cardType) {
        card_util.add_card(player_color, (CardType)cardType, true);
    }

    // =========================================================================
    // OnGUI — Room name input (chỉ hiện khi cần nhập tên phòng)
    // =========================================================================

    void OnGUI() {
        if (!show_gui) return;

        float w = 300, h = 140;
        float x = (Screen.width - w) / 2;
        float y = (Screen.height - h) / 2;

        GUI.Box(new Rect(x, y, w, h), joining ? "Nhap ma phong:" : "Tao phong");

        room_input = GUI.TextField(new Rect(x + 20, y + 30, 260, 30), room_input, 8);

        if (GUI.Button(new Rect(x + 20, y + 70, 120, 30), "OK")) {
            show_gui = false;
            if (joining)
                JoinRoom(room_input);
            else
                PhotonNetwork.CreateRoom(room_input, new RoomOptions { MaxPlayers = 2 });
        }

        if (GUI.Button(new Rect(x + 160, y + 70, 120, 30), "Huy")) {
            show_gui = false;
        }
    }

    // =========================================================================
    // PUBLIC — cho menu gọi
    // =========================================================================

    public void ShowJoinRoomGUI() {
        joining    = true;
        room_input = "";
        show_gui   = true;
    }

    public string GetStatusMsg() => status_msg;
    public string GetRoomCode() => PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : room_input;
}
