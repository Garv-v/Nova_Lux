using UnityEngine;

namespace NovaLux
{
    // 복도 배경의 가로 범위가 걷기와 카메라 경계의 출처다.
    [RequireComponent(typeof(SpriteRenderer))]
    public class Hallway : MonoBehaviour
    {
        SpriteRenderer _background;

        void Awake() => _background = GetComponent<SpriteRenderer>();

        public float XMin => _background.bounds.min.x;
        public float XMax => _background.bounds.max.x;
    }
}
