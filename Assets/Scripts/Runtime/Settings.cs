using System;
using System.IO;
using UnityEngine;

namespace NovaLux
{
    public static class Settings
    {
        static string _filePath;
        static SettingsData _current;
        static bool _dirty;

        // null이면 Application.persistentDataPath. 테스트가 사용자 설정을 건드리지 않게 바꾸는 이음새.
        public static string FilePath
        {
            get => _filePath ?? Path.Combine(Application.persistentDataPath, "settings.json");
            set
            {
                _filePath = value;
                _current = null;
                _dirty = false;
            }
        }

        public static SettingsData Current => _current ??= Load();

        // 바꾸고 바로 저장한다.
        public static void Set(Action<SettingsData> change)
        {
            Edit(change);
            SaveIfDirty();
        }

        // 메모리에서만 바꾼다. 슬라이더를 끌 때처럼 값이 자주 바뀌는 곳에서 쓰고, 끝나면 SaveIfDirty를 부른다.
        public static void Edit(Action<SettingsData> change)
        {
            change(Current);
            Current.Clamp();
            _dirty = true;
        }

        public static void SaveIfDirty()
        {
            if (_dirty) Save(Current);
        }

        public static void Reload()
        {
            _current = Load();
            _dirty = false;
        }

        static SettingsData Load()
        {
            var data = new SettingsData();
            var path = FilePath;
            if (!File.Exists(path)) return data;

            try
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(path), data);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"설정 파일을 읽지 못해 기본값을 쓴다: {e.Message}");
                data = new SettingsData();
                KeepBadCopy(path);
            }

            data.Clamp();
            return data;
        }

        static void KeepBadCopy(string path)
        {
            var bad = path + ".bad";
            if (File.Exists(bad)) File.Delete(bad);
            File.Move(path, bad);
        }

        static void Save(SettingsData data)
        {
            var path = FilePath;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, JsonUtility.ToJson(data));
            if (File.Exists(path)) File.Replace(tmp, path, null);
            else File.Move(tmp, path);
            _dirty = false;
        }
    }
}
