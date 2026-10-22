using TMPro;
using UnityEngine;

namespace NovaLux
{
    // 복도의 문 하나. 금태양의 x가 문 폭 안이면 앞에 선 것이다. 입장 표시는 문 위에 뜬다.
    // 문 오브젝트는 Hallway 밖(루트)에 둔다. Hallway의 scale이 글자를 찌그러뜨린다.
    public class Door : MonoBehaviour
    {
        [SerializeField] string id;
        [SerializeField] float halfWidth = 1f;

        TextMeshPro _prompt;

        public string Id => id;
        public float X => transform.position.x;

        public bool Contains(float x) => Mathf.Abs(x - X) <= halfWidth;

        void Awake()
        {
            var sprite = GetComponent<SpriteRenderer>();
            var top = sprite != null ? sprite.bounds.max.y : transform.position.y;

            // 문의 scale을 물려받지 않도록 자식으로 두지 않는다.
            var go = new GameObject("Prompt_" + id, typeof(TextMeshPro), typeof(LocText));
            go.transform.position = new Vector3(X, top + 0.8f, 0f);
            _prompt = go.GetComponent<TextMeshPro>();
            _prompt.fontSize = 5f;
            _prompt.alignment = TextAlignmentOptions.Center;
            _prompt.color = Color.white;
            _prompt.sortingOrder = 10;
            go.GetComponent<LocText>().Key = "hallway.enter";
            go.SetActive(false);
        }

        void OnDestroy()
        {
            if (_prompt != null) Destroy(_prompt.gameObject);
        }

        public void SetPromptVisible(bool visible)
        {
            if (_prompt.gameObject.activeSelf != visible) _prompt.gameObject.SetActive(visible);
        }
    }
}
