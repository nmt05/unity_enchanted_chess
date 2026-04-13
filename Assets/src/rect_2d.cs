using UnityEngine;

public class rect_2d : MonoBehaviour {

    //==== CORE ====
    public GameObject obj;
    public GameObject visual;
    public SpriteRenderer sr;
    public BoxCollider2D col;
    public Sprite sprite;

    //==== INPUT STATE ====
    public int mouse_click = 0;
    public int mouse_unclick = 0;
    public int mouse_hover = 0;

    //==== SIZE RECORD (ABSOLUTE WORLD UNITS) ====
    public float sprite_x = 1f;
    public float sprite_y = 1f;
    public float hitbox_x = 1f;
    public float hitbox_y = 1f;

    //==== DEPTH (Z POSITION) ====
    public float depth = 0f;

    //==== CREATE ====
    public static rect_2d create(float x, float y, float z = 0f){
        GameObject o = new GameObject("rect_2d");
        o.transform.position = new Vector3(x, y, z);

        rect_2d r = o.AddComponent<rect_2d>();
        r.init();
        r.depth = z; // record initial depth
        return r;
    }

    void init() {
        obj = gameObject;

        col = obj.AddComponent<BoxCollider2D>();

        visual = new GameObject("visual");
        visual.transform.SetParent(obj.transform);
        visual.transform.localPosition = Vector3.zero;

        sr = visual.AddComponent<SpriteRenderer>();
    }

    //==== INPUT ====
    void OnMouseEnter() { mouse_hover = 1; }
    void OnMouseExit()  { mouse_hover = 0; }
    void OnMouseDown()  { mouse_click = 1; }
    void OnMouseUp()    { mouse_unclick = 1; }

    //==== LIFECYCLE ====
    public void self_destroy(){ Destroy(obj); }

    //==== VISUAL ====
    public void set_sprite(Sprite s){
        sprite = s;
        sr.sprite = s;

        if (s != null){
            Vector2 size = s.bounds.size;
            sprite_x = size.x;
            sprite_y = size.y;
        }
    }

    public void set_color(Color c){
        sr.color = c;
    }

    public void set_sprite_scale(float x_scale, float y_scale){
        visual.transform.localScale = new Vector3(x_scale, y_scale, 1f);

        if (sprite != null){
            Vector2 baseSize = sprite.bounds.size;
            sprite_x = baseSize.x * x_scale;
            sprite_y = baseSize.y * y_scale;
        }
    }

    public void set_sprite_size(float w, float h){
        if (sprite == null) return;

        Vector2 size = sprite.bounds.size;
        if (size.x == 0 || size.y == 0) return;

        float sx = w / size.x;
        float sy = h / size.y;

        visual.transform.localScale = new Vector3(sx, sy, 1f);

        sprite_x = w;
        sprite_y = h;
    }

    public void reset_visual_scale(){
        visual.transform.localScale = Vector3.one;

        if (sprite != null){
            Vector2 size = sprite.bounds.size;
            sprite_x = size.x;
            sprite_y = size.y;
        }
    }

    //==== COLLIDER ====
    public void set_collider_size(float x, float y){
        col.size = new Vector2(x, y);
        hitbox_x = x;
        hitbox_y = y;
    }

    public void fit_collider_to_sprite(Sprite s){
        if (s != null){
            Vector2 size = s.bounds.size;
            col.size = size;
            hitbox_x = size.x;
            hitbox_y = size.y;
        }
    }

    public void toggle_hitbox(int signal){
        col.enabled = (signal == 1);
    }

    //==== TRANSFORM ====
    public void move_to(float x, float y, float z = 0f){
        obj.transform.position = new Vector3(x, y, z);
        depth = z; // always record
    }

    public void move_to_board(int bx, int by, float z = 0f){
        move_to(board_to_world(bx), board_to_world(by), z);
    }

    public void set_depth(float z){
        Vector3 p = obj.transform.position;
        obj.transform.position = new Vector3(p.x, p.y, z);
        depth = z;
    }

    public void bring_to_front(float z = -1f){
        set_depth(z);
    }

    public void send_to_back(float z = 1f){
        set_depth(z);
    }

    public float board_to_world(int v){
        return v * 1.28f - 4.48f;
    }
}