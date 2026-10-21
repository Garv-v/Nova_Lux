using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NovaLux
{
    // 9-slice 알약 스프라이트의 모서리 반지름을 크기와 상관없이 일정하게 맞춘다.
    // 9-slice 모서리는 가로·세로를 따로 줄이므로, 스프라이트 높이가 반지름의 두 배에 딱 맞게 배율을 잡는다.
    [RequireComponent(typeof(Image))]
    public class PillCorners : UIBehaviour
    {
        float _radius;

        // 0이면 높이의 절반(완전한 알약). 양수면 그 반지름(작은 변의 절반이 상한).
        public float Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                Apply();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Apply();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            Apply();
        }

        void Apply()
        {
            var image = GetComponent<Image>();
            if (image == null || image.sprite == null) return;
            var rect = ((RectTransform)transform).rect;
            if (rect.height <= 0f || image.pixelsPerUnit <= 0f) return;
            var r = rect.height / 2f;
            if (_radius > 0f) r = Mathf.Min(_radius, r, rect.width / 2f);
            image.pixelsPerUnitMultiplier = image.sprite.rect.height / 2f / (image.pixelsPerUnit * r);
        }
    }
}
