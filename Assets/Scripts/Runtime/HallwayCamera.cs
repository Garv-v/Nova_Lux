using UnityEngine;

namespace NovaLux
{
    // 금태양을 따라가되 복도 경계 밖이 보이지 않게 x를 가둔다.
    // 화면 반폭은 카메라 aspect 대신 16:9 상수로 계산한다. 레터박스가 항상 16:9로 맞추고, 갱신 순서에 흔들리지 않게 하려는 것이다.
    [RequireComponent(typeof(Camera))]
    public class HallwayCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Hallway hallway;

        public static float ClampX(float targetX, float xMin, float xMax, float halfView)
        {
            if (xMax - xMin <= 2f * halfView) return (xMin + xMax) / 2f;
            return Mathf.Clamp(targetX, xMin + halfView, xMax - halfView);
        }

        void LateUpdate()
        {
            var halfView = GetComponent<Camera>().orthographicSize * Letterbox.Aspect;
            var p = transform.position;
            p.x = ClampX(target.position.x, hallway.XMin, hallway.XMax, halfView);
            transform.position = p;
        }
    }
}
