using System;
using UnityEngine;

namespace NovaLux
{
    public static class AppQuit
    {
        // 테스트가 종료 호출을 가로채는 자리. null이면 실제로 끈다.
        public static Action Handler;

        public static void Quit()
        {
            if (Handler != null)
            {
                Handler();
                return;
            }
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
