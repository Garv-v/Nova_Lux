using TMPro;
using UnityEngine;

namespace NovaLux
{
    // TMP 라벨에 붙어 언어가 바뀌면 문자열과 서체를 다시 채운다.
    // 키가 없으면 문자열은 그대로 두고 서체만 따른다(해상도 이름처럼 언어와 무관한 글자).
    [RequireComponent(typeof(TMP_Text))]
    public class LocText : MonoBehaviour
    {
        TMP_Text _text;
        string _key;

        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                Refresh();
            }
        }

        void Awake() => _text = GetComponent<TMP_Text>();

        void OnEnable()
        {
            Loc.Changed += Refresh;
            Refresh();
        }

        void OnDisable() => Loc.Changed -= Refresh;

        void Refresh()
        {
            if (_text == null) _text = GetComponent<TMP_Text>();
            if (_key != null) _text.text = Loc.Get(_key);
            var font = UiTheme.Current.Font;
            if (font != null) _text.font = font;
        }
    }
}
