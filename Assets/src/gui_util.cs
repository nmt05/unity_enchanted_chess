using UnityEngine;

// Helpers for building and destroying menu GUI elements.
public static class gui_util {

    const float BTN_W = 2.5f;
    const float BTN_H = 0.8f;

    // Create a labelled button rect and register it for bulk destroy.
    public static rect_2d make_button(float x, float y, Color color, string text = ""){
        rect_2d btn = rect_2d.create(x, y, -1f);
        btn.set_sprite(data.mem.rect_2d_sprite);
        btn.set_sprite_size(BTN_W, BTN_H);
        btn.set_collider_size(BTN_W, BTN_H);
        btn.set_color(color);
        data.mem.menu_rects.Add(btn);

        //text
        if (!string.IsNullOrEmpty(text) && btn.obj != null) {
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(btn.obj.transform);
            textObj.transform.localPosition = new Vector3(0, 0, -0.1f);
            
            TextMesh tm = textObj.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = 0.15f; 
            tm.fontSize = 40;         
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.black;   
        }

        return btn;
    }
    public static rect_2d make_button_sprite(float x, float y, Sprite buttonSprite, float width = BTN_W, float height = BTN_H,string text = "") {
    // Tạo object button tại vị trí x, y
        rect_2d btn = rect_2d.create(x, y, -1f);    
        
        // Gán sprite tùy chỉnh được truyền vào (ví dụ: sprite nút xanh, nút vàng...)
        btn.set_sprite(buttonSprite);
        
        btn.set_sprite_size(width, height);
        btn.set_collider_size(width, height);
        
        // Quan trọng: Set màu trắng để giữ nguyên màu gốc của Sprite
        btn.set_color(Color.white); 
        
        data.mem.menu_rects.Add(btn);

        // Xử lý Text
        if (!string.IsNullOrEmpty(text) && btn.obj != null) {
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(btn.obj.transform);
            
            // Đưa text lên phía trước một chút để không bị đè bởi sprite nút
            textObj.transform.localPosition = new Vector3(0, 0, -0.1f);
            
            TextMesh tm = textObj.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = 0.15f; 
            tm.fontSize = 40;         
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            
            // Bạn có thể đổi màu text thành trắng nếu sprite nút có màu tối
            tm.color = Color.black;   
        }

        return btn;
    }

    // Returns true and consumes the click if the button was unclicked.
    public static bool clicked(rect_2d btn) {
        if (btn == null) return false;
        if (btn.mouse_unclick == 1) {
            btn.mouse_unclick = 0;
            return true;
        }
        return false;
    }

    // Destroy every registered menu rect and the background.
    public static void clear_menu() {
        if (data.mem.main_screen_gui != null) {
            data.mem.main_screen_gui.self_destroy();
            data.mem.main_screen_gui = null;
        }
        foreach (rect_2d r in data.mem.menu_rects) {
            if (r != null) r.self_destroy();
        }
        data.mem.menu_rects.Clear();

        // null out named refs so HandleMenuInput skips dead objects
        data.mem.pvp_button   = null;
        data.mem.pve_button   = null;
        data.mem.btn_count1   = null;
        data.mem.btn_count2   = null;
        data.mem.btn_count3   = null;
        data.mem.back_button  = null;
        data.mem.btn_diff1    = null;
        data.mem.btn_diff2    = null;
        data.mem.btn_diff3    = null;
        data.mem.btn_diff4    = null;
        data.mem.btn_main_menu = null;
        // NOTE: settings_button is NOT cleared here — it persists during gameplay.
    }

}