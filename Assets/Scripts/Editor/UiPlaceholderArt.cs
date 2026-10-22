using System.IO;
using UnityEditor;
using UnityEngine;

namespace NovaLux.EditorTools
{
    // 최종 아트 전의 임시 UI 그림(알약 9-slice, 원 칩, 메뉴 아이콘)을 절차적으로 그려 PNG로 만든다.
    // 외부 소스나 폰트 글리프 없이 이 코드로만 그린 자체 제작물이다. 다시 실행하면 같은 결과로 덮어쓴다.
    public static class UiPlaceholderArt
    {
        const string Dir = "Assets/Art/UI/Placeholder";
        const string ThemePath = "Assets/Resources/UiTheme.asset";

        [MenuItem("Nova Lux/Generate Placeholder UI Art")]
        public static void Generate()
        {
            Directory.CreateDirectory(Dir);
            var pill = Save("pill", 384, 192, Pill, new Vector4(96f, 95f, 96f, 95f));
            var circle = Save("circle", 192, 192, Circle, Vector4.zero);
            var theme = AssetDatabase.LoadAssetAtPath<UiTheme>(ThemePath);
            theme.pillSprite = pill;
            theme.circleSprite = circle;
            theme.iconNewGame = Save("icon_newgame", 128, 128, Play, Vector4.zero);
            theme.iconContinue = Save("icon_continue", 128, 128, Bookmark, Vector4.zero);
            theme.iconGallery = Save("icon_gallery", 128, 128, Gallery, Vector4.zero);
            theme.iconSettings = Save("icon_settings", 128, 128, Gear, Vector4.zero);
            theme.iconQuit = Save("icon_quit", 128, 128, Power, Vector4.zero);
            theme.iconBgm = Save("icon_bgm", 128, 128, Note, Vector4.zero);
            theme.iconSfx = Save("icon_sfx", 128, 128, Speaker, Vector4.zero);
            theme.iconVoice = Save("icon_voice", 128, 128, Microphone, Vector4.zero);
            theme.iconTextSpeed = Save("icon_textspeed", 128, 128, FastForward, Vector4.zero);
            theme.iconAuto = Save("icon_auto", 128, 128, Clock, Vector4.zero);
            theme.iconLocked = Save("icon_locked", 128, 128, Lock, Vector4.zero);
            EditorUtility.SetDirty(theme);
            AssetDatabase.SaveAssets();
        }

        // 도형 함수: 픽셀 좌표(왼쪽 아래 원점, 위가 +y)를 받아 부호 거리(안쪽이 음수, 픽셀 단위)를 돌려준다.
        delegate float Shape(Vector2 p);

        static Sprite Save(string name, int width, int height, Shape shape, Vector4 border)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var pixels = new Color32[width * height];
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                // 투명 픽셀도 RGB는 흰색이라 가장자리 보간에 검은 테가 끼지 않는다.
                var coverage = Mathf.Clamp01(0.5f - shape(new Vector2(x + 0.5f, y + 0.5f)));
                pixels[y * width + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(coverage * 255f));
            }
            texture.SetPixels32(pixels);
            var path = $"{Dir}/{name}.png";
            File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100f;
            importer.spriteBorder = border;
            importer.alphaIsTransparency = true;
            // 화면에서는 원본보다 3~10배 작게 그려진다. 밉맵 없이 줄이면 가장자리 알파가 0/1로 끊겨 계단이 진다.
            importer.mipmapEnabled = true;
            importer.filterMode = FilterMode.Trilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        // --- 면 ---

        static float Pill(Vector2 p) => Capsule(p, new Vector2(96f, 96f), new Vector2(288f, 96f), 96f);

        static float Circle(Vector2 p) => Vector2.Distance(p, new Vector2(96f, 96f)) - 96f;

        // --- 아이콘 (128x128) ---

        static float Play(Vector2 p) =>
            Polygon(p, new[] { new Vector2(46f, 36f), new Vector2(46f, 92f), new Vector2(92f, 64f) }) - 9f;

        static float Bookmark(Vector2 p) =>
            Polygon(p, new[]
            {
                new Vector2(40f, 108f), new Vector2(88f, 108f), new Vector2(88f, 26f), new Vector2(64f, 48f),
                new Vector2(40f, 26f),
            }) - 5f;

        static float Gallery(Vector2 p)
        {
            var c = new Vector2(64f, 64f);
            var frame = Mathf.Max(Box(p - c, new Vector2(46f, 38f), 10f), -Box(p - c, new Vector2(36f, 28f), 4f));
            var mountain = Polygon(p, new[] { new Vector2(40f, 42f), new Vector2(60f, 70f), new Vector2(80f, 42f) }) - 4f;
            var sun = Vector2.Distance(p, new Vector2(84f, 78f)) - 8f;
            return Mathf.Min(frame, Mathf.Min(mountain, sun));
        }

        static float Lock(Vector2 p)
        {
            var body = Box(p - new Vector2(64f, 44f), new Vector2(38f, 28f), 8f);
            // 몸통 위의 U자 고리: 위쪽 반원 호와 양쪽 다리.
            var q = p - new Vector2(64f, 76f);
            var arc = Mathf.Max(Mathf.Abs(q.magnitude - 22f) - 6f, -q.y);
            var legs = Mathf.Min(Capsule(p, new Vector2(42f, 76f), new Vector2(42f, 68f), 6f),
                Capsule(p, new Vector2(86f, 76f), new Vector2(86f, 68f), 6f));
            return Mathf.Min(body, Mathf.Min(arc, legs));
        }

        static float Gear(Vector2 p)
        {
            var q = p - new Vector2(64f, 64f);
            var d = q.magnitude - 34f;
            for (var i = 0; i < 4; i++)
            {
                var a = i * Mathf.PI / 4f;
                var rotated = new Vector2(q.x * Mathf.Cos(a) + q.y * Mathf.Sin(a), -q.x * Mathf.Sin(a) + q.y * Mathf.Cos(a));
                d = Mathf.Min(d, Box(rotated, new Vector2(9f, 45f), 3f));
            }
            return Mathf.Max(d, -(q.magnitude - 16f));
        }

        static float Power(Vector2 p)
        {
            var q = p - new Vector2(64f, 64f);
            var ring = Mathf.Abs(q.magnitude - 34f) - 5f;
            // 위쪽 35도 쐐기를 잘라 고리를 튼다. 쐐기 안은 음수, 아래쪽 절반은 항상 양수다.
            var half = 35f * Mathf.Deg2Rad;
            var wedge = Mathf.Abs(q.x) * Mathf.Cos(half) - q.y * Mathf.Sin(half);
            ring = Mathf.Max(ring, -wedge);
            var bar = Capsule(p, new Vector2(64f, 62f), new Vector2(64f, 100f), 5f);
            return Mathf.Min(ring, bar);
        }

        static float Note(Vector2 p)
        {
            var head = Vector2.Distance(p, new Vector2(48f, 36f)) - 15f;
            var stem = Capsule(p, new Vector2(60f, 40f), new Vector2(60f, 100f), 5f);
            var flag = Capsule(p, new Vector2(60f, 100f), new Vector2(92f, 78f), 5f);
            return Mathf.Min(head, Mathf.Min(stem, flag));
        }

        static float Speaker(Vector2 p)
        {
            var body = Polygon(p, new[]
            {
                new Vector2(22f, 52f), new Vector2(40f, 52f), new Vector2(64f, 28f), new Vector2(64f, 100f),
                new Vector2(40f, 76f), new Vector2(22f, 76f),
            }) - 3f;
            var q = p - new Vector2(64f, 64f);
            var angle = Mathf.Abs(Mathf.Atan2(q.y, q.x));
            var d = body;
            foreach (var r in new[] { 24f, 40f })
            {
                // 오른쪽 ±40도 호만 남긴다.
                var wave = Mathf.Max(Mathf.Abs(q.magnitude - r) - 4f, (angle - 40f * Mathf.Deg2Rad) * r);
                d = Mathf.Min(d, wave);
            }
            return d;
        }

        static float Microphone(Vector2 p)
        {
            var capsule = Capsule(p, new Vector2(64f, 70f), new Vector2(64f, 88f), 17f);
            var q = p - new Vector2(64f, 70f);
            // 아래쪽 반원 호(U자)
            var arc = Mathf.Max(Mathf.Abs(q.magnitude - 30f) - 5f, q.y);
            var stand = Capsule(p, new Vector2(64f, 40f), new Vector2(64f, 24f), 5f);
            var foot = Capsule(p, new Vector2(46f, 22f), new Vector2(82f, 22f), 5f);
            return Mathf.Min(Mathf.Min(capsule, arc), Mathf.Min(stand, foot));
        }

        static float FastForward(Vector2 p)
        {
            var a = Polygon(p, new[] { new Vector2(24f, 34f), new Vector2(24f, 94f), new Vector2(62f, 64f) }) - 6f;
            var b = Polygon(p, new[] { new Vector2(66f, 34f), new Vector2(66f, 94f), new Vector2(104f, 64f) }) - 6f;
            return Mathf.Min(a, b);
        }

        static float Clock(Vector2 p)
        {
            var q = p - new Vector2(64f, 64f);
            var ring = Mathf.Abs(q.magnitude - 38f) - 6f;
            var hour = Capsule(p, new Vector2(64f, 64f), new Vector2(64f, 90f), 4.5f);
            var minute = Capsule(p, new Vector2(64f, 64f), new Vector2(82f, 54f), 4.5f);
            return Mathf.Min(ring, Mathf.Min(hour, minute));
        }

        // --- 도형 부품 ---

        static float Capsule(Vector2 p, Vector2 a, Vector2 b, float radius)
        {
            var ab = b - a;
            var t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / ab.sqrMagnitude);
            return Vector2.Distance(p, a + ab * t) - radius;
        }

        static float Box(Vector2 p, Vector2 half, float radius)
        {
            var d = new Vector2(Mathf.Abs(p.x), Mathf.Abs(p.y)) - (half - Vector2.one * radius);
            return Vector2.Max(d, Vector2.zero).magnitude + Mathf.Min(Mathf.Max(d.x, d.y), 0f) - radius;
        }

        // 단순 다각형의 부호 거리: 변까지의 최소 거리, 안쪽이면 음수(짝홀 규칙).
        static float Polygon(Vector2 p, Vector2[] v)
        {
            var min = float.MaxValue;
            var inside = false;
            for (int i = 0, j = v.Length - 1; i < v.Length; j = i++)
            {
                var e = v[j] - v[i];
                var w = p - v[i];
                var b = w - e * Mathf.Clamp01(Vector2.Dot(w, e) / e.sqrMagnitude);
                min = Mathf.Min(min, b.magnitude);
                if ((v[i].y > p.y) != (v[j].y > p.y) && p.x < (v[j].x - v[i].x) * (p.y - v[i].y) / (v[j].y - v[i].y) + v[i].x)
                    inside = !inside;
            }
            return inside ? -min : min;
        }
    }
}
