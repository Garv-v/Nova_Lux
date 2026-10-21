using TMPro;
using UnityEngine;

namespace NovaLux
{
    // 화면 사이에 공유되는 UI 겉모습 값. 아트 스타일이 정해지면 값만 바꾼다.
    // 기본은 Resources/UiTheme 에셋이고, 없으면 코드가 만든 기본값을 쓴다. 테스트는 Current에 다른 인스턴스를 끼운다.
    public class UiTheme : ScriptableObject
    {
        public Color buttonColor = new Color(0.25f, 0.25f, 0.3f);
        public Color accentColor = new Color(0.45f, 0.6f, 0.9f);
        public Color textColor = Color.white;
        public Color handleColor = Color.white;
        public Color panelColor = new Color(0.05f, 0.05f, 0.08f, 1f);
        public int fontSize = 40;
        public int smallFontSize = 36;

        // 언어별 서체. 언어 선택(#4)이 들어오기 전까지는 한국어를 쓴다. null이면 TMP 기본 서체.
        public TMP_FontAsset fontKo;
        public TMP_FontAsset fontJa;
        public TMP_FontAsset Font => fontKo;

        // 초기화면 (1920x1080 기준, 화면 중앙 세로 배치)
        public Vector2 titleButtonSize = new Vector2(480f, 90f);
        public float titleButtonGap = 24f;

        // 환경설정: 라벨은 왼쪽, 조작부는 오른쪽 열
        public float rowGap = 95f;
        public float firstRowY = 330f;
        public float labelX = -300f;
        public float controlX = 250f;
        public Vector2 labelSize = new Vector2(420f, 60f);
        public Vector2 sliderSize = new Vector2(500f, 40f);
        public Vector2 modeButtonSize = new Vector2(500f, 60f);
        public Vector2 resButtonSize = new Vector2(115f, 60f);
        public float resFirstX = 57.5f;
        public float resStep = 125f;
        public Vector2 closeButtonSize = new Vector2(300f, 80f);
        public float closeOffsetY = -20f;

        // 슬라이더 내부
        public float sliderFillInset = 12f;
        public float sliderHandleInset = 15f;
        public float sliderHandleWidth = 30f;

        static UiTheme _current;

        // null을 넣으면 기본값으로 돌아간다.
        public static UiTheme Current
        {
            get
            {
                if (_current == null) _current = Resources.Load<UiTheme>("UiTheme") ?? CreateInstance<UiTheme>();
                return _current;
            }
            set => _current = value;
        }
    }
}
