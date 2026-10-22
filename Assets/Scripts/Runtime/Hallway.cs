using UnityEngine;

namespace NovaLux
{
    // 복도 배경의 가로 범위가 걷기와 카메라 경계의 출처다.
    [RequireComponent(typeof(SpriteRenderer))]
    public class Hallway : MonoBehaviour
    {
        SpriteRenderer Background => GetComponent<SpriteRenderer>();

        public float XMin => Background.bounds.min.x;
        public float XMax => Background.bounds.max.x;
    }
}
