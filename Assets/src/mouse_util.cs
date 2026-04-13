using UnityEngine;

public class mouse_util : MonoBehaviour {

    public struct button_signal {
        public int click;
        public int unclick;
        public int hold;
    }

    public static float x, y;
    public static float dx, dy;
    public static float scroll;

    public static button_signal left;
    public static button_signal right;
    public static button_signal middle;

    static Vector3 last;

    [RuntimeInitializeOnLoadMethod]
    static void init(){
        GameObject o = new GameObject("mouse_util");
        o.AddComponent<mouse_util>();
    }

	void Start(){
		Camera c = Camera.main;
		if (c == null) return;

		Vector3 mp = Input.mousePosition;
		mp.z = -c.transform.position.z;

		last = c.ScreenToWorldPoint(mp);
	}

	void Update(){
		Camera c = cam_2d.cam; // use your own camera
		if (c == null) return;

		// --- world position (for clicking / zoom) ---
		Vector3 mp = Input.mousePosition;
		mp.z = -c.transform.position.z;

		Vector3 cur = c.ScreenToWorldPoint(mp);

		x = cur.x;
		y = cur.y;

		// --- FIX: use screen-space delta (no feedback loop) ---
		dx = Input.GetAxis("Mouse X");
		dy = Input.GetAxis("Mouse Y");

		scroll = Input.GetAxis("Mouse ScrollWheel");

		update_button(ref left, 0);
		update_button(ref right, 1);
		update_button(ref middle, 2);

		last = cur; // still useful for other stuff if needed
	}

    void update_button(ref button_signal b, int id){
        b.click   = Input.GetMouseButtonDown(id) ? 1 : 0;
        b.unclick = Input.GetMouseButtonUp(id)   ? 1 : 0;
        b.hold    = Input.GetMouseButton(id)     ? 1 : 0;
    }
}