using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Cấu hình hiển thị")]
    public Sprite hoverSprite;      // Kéo ảnh bạn muốn hiện vào đây
    public Image followImage;       // Kéo cái Image UI (vật thể bay theo chuột) vào đây

    private bool isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GATrainer.instance == null || !GATrainer.instance.isTraining)
            Debug.Log("IN: " + gameObject.name);
        
        if (hoverSprite != null && followImage != null)
        {
            isHovering = true;
            followImage.sprite = hoverSprite;
            followImage.gameObject.SetActive(true);
            
            // Đưa lên lớp trên cùng để không bị che
            followImage.transform.SetAsLastSibling();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GATrainer.instance == null || !GATrainer.instance.isTraining)
            Debug.Log("OUT: " + gameObject.name);
        
        isHovering = false;
        if (followImage != null)
        {
            followImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Nếu đang hover, cập nhật vị trí ảnh liên tục theo tọa độ chuột
        if (isHovering && followImage != null)
        {
            // Input.mousePosition là tọa độ màn hình, khớp với UI Overlay
            // Cộng thêm offset (vd: 50, -50) để ảnh không đè lên chính con trỏ chuột
            followImage.transform.position = Input.mousePosition + new Vector3(50, -50, 0);
        }
    }
}