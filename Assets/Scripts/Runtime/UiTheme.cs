using TMPro;
using UnityEngine;

namespace NovaLux
{
    // 화면 사이에 공유되는 UI 겉모습 값. 아트 스타일이 정해지면 값만 바꾼다.
    // 기본은 Resources/UiTheme 에셋이고, 없으면 코드가 만든 기본값을 쓴다. 테스트는 Current에 다른 인스턴스를 끼운다.
    public class UiTheme : ScriptableObject
    {
        public Color buttonColor = new Color32(0xF0, 0xF7, 0xFF, 255);
        public Color accentColor = new Color32(0x3C, 0x8A, 0xE0, 255);
        public Color textColor = new Color32(0x26, 0x43, 0x5F, 255);
        public Color handleColor = new Color32(0x26, 0x43, 0x5F, 255);
        public Color panelColor = new Color32(0xD1, 0xE3, 0xF5, 255);

        // 알약 버튼 상태 색. 선택·호버·포커스는 accentColor로 채우고 selectedTextColor 글자를 쓴다.
        public Color selectedTextColor = Color.white;
        public Color pressedColor = new Color32(0x2C, 0x74, 0xC9, 255);
        public Color disabledColor = new Color32(0x94, 0xAE, 0xC6, 255);
        public Color chipColor = new Color32(0xCF, 0xE5, 0xFB, 255);
        public Color iconColor = new Color32(0x28, 0x78, 0xCC, 255);
        public Color selectedChipColor = Color.white;
        public Color disabledChipColor = new Color32(0xE1, 0xEE, 0xFA, 255);

        // 9-slice 알약 면, 아이콘 칩(원), 초기화면 메뉴 아이콘. null이면 단색 사각형이 된다.
        public Sprite pillSprite;
        public Sprite circleSprite;
        public Sprite iconNewGame;
        public Sprite iconContinue;
        public Sprite iconGallery;
        public Sprite iconSettings;
        public Sprite iconQuit;

        public int fontSize = 40;
        public int smallFontSize = 36;

        // 언어별 서체. 언어 선택(#4)이 들어오기 전까지는 한국어를 쓴다. null이면 TMP 기본 서체.
        public TMP_FontAsset fontKo;
        public TMP_FontAsset fontJa;
        public TMP_FontAsset Font => fontKo;

        // 초기화면 (1920x1080 기준, 왼쪽 정렬 세로 배치)
        public Vector2 titleButtonSize = new Vector2(480f, 90f);
        public float titleButtonGap = 24f;
        public float titleMenuX = -560f;
        public float titleChipSize = 64f;
        public float titleChipInset = 13f;
        public float titleIconSize = 36f;
        public float titleLabelGap = 20f;

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
