using UnityEngine;

public static class cam_2d {

    public static Camera cam;

    public static float x, y, z;
    public static float size;

    public static float minSize = 0.5f;
    public static float maxSize = 20f;
    public static float zoomSpeed = 5f;

    //==== AUTO INIT ====
    [RuntimeInitializeOnLoadMethod]
    static void init(){
        GameObject o = new GameObject("main_camera");

        cam = o.AddComponent<Camera>();
        cam.orthographic = true;

        o.tag = "MainCamera";

        x = 0f;
        y = 0f;
        z = -10f;
        size = 5f;

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        apply();
    }

    //==== APPLY ====
    public static void apply(){
        if (cam == null) return;

        cam.transform.position = new Vector3(x, y, z);
        cam.orthographicSize = size;
    }

    //==== MOVE ====
    public static void move(float dx, float dy){
        x += dx;
        y += dy;
		apply();
    }

    //==== SET ====
    public static void set_pos(float px, float py){
        x = px;
        y = py;
		apply();
    }

    //==== SCALE ====
	public static void scale(float factor){
		if (factor <= 0f) return;

		size = Mathf.Clamp(size * factor, minSize, maxSize);
		apply();
	}

    //==== ZOOM (USE FOR CONTINUOUS MOUSE SCROLL) ====
    public static void zoom_to_point(float scroll, float mx, float my){
		if (scroll == 0f) return;

		// world point BEFORE zoom
		Vector3 before = cam.ScreenToWorldPoint(Input.mousePosition);

		// apply zoom
		size = Mathf.Clamp(size - scroll * zoomSpeed, minSize, maxSize);
		cam.orthographicSize = size;

		// world point AFTER zoom
		Vector3 after = cam.ScreenToWorldPoint(Input.mousePosition);

		// move camera so point stays under cursor
		x += before.x - after.x;
		y += before.y - after.y;

		apply();
    }
	
	public static void zoom(float scroll){
		if (scroll == 0f) return;

		size = Mathf.Clamp(size - scroll * zoomSpeed, minSize, maxSize);

		apply();
	}
}