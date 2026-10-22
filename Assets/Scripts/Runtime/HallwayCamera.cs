using UnityEngine;

namespace NovaLux
{
    // 금태양을 따라가되 복도 경계 밖이 보이지 않게 x를 가둔다.
    // 화면 반폭은 카메라 aspect 대신 16:9 상수로 계산한다. 레터박스 갱신 순서와 무관하게 하려는 것이다.
    [RequireComponent(typeof(Camera))]
    public class HallwayCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Hallway hallway;

        Camera _camera;

        void Awake() => _camera = GetComponent<Camera>();

        public static float ClampX(float targetX, float xMin, float xMax, float halfView)
        {
            if (xMax - xMin <= 2f * halfView) return (xMin + xMax) / 2f;
            return Mathf.Clamp(targetX, xMin + halfView, xMax - halfView);
        }

        void LateUpdate()
        {
            var halfView = _camera.orthographicSize * Letterbox.Aspect;
            var p = transform.position;
            p.x = ClampX(target.position.x, hallway.XMin, hallway.XMax, halfView);
            transform.position = p;
        }
    }
}
