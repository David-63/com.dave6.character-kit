using UnityEngine;

namespace Dave6.CharacterKit
{
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] RectTransform crosshairRect;   // 본인
        [SerializeField] Canvas canvas;                 // 부모 캔버스
        

        public void LateUpdateCrosshair(Vector3 worldPoint)
        {
            if (crosshairRect == null) return;

            // world 좌표를 screen 좌표로 변환
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPoint);

            if (screenPos.z < 0)
            {
                crosshairRect.gameObject.SetActive(false);
                return;
            }
            crosshairRect.gameObject.SetActive(true);

            // screen 좌표를 Canvas의 로컬 좌표로 변환
            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle
            (canvas.transform as RectTransform, screenPos, null, out localPos))
            {
                crosshairRect.anchoredPosition = localPos;
            }

        }
    }
}
