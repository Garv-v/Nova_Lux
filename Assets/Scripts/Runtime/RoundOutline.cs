using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NovaLux
{
    // 알약·원 스프라이트용 둥근 테두리. 기본 Outline은 대각선 네 방향 복제라 테두리가 대각선에서 √2배 두꺼워져 원이 각져 보인다.
    // 대신 메시 바깥쪽 가장자리 정점만 effectDistance.x만큼 밖으로 밀어 한 장 뒤에 깐다.
    // 9-slice 모서리 칸이 반지름 r에서 r+w로 같은 비율로 커지므로 테두리 두께가 어디서나 w다.
    public class RoundOutline : Outline
    {
        static readonly List<UIVertex> Verts = new List<UIVertex>();

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive()) return;
            Verts.Clear();
            vh.GetUIVertexStream(Verts);
            var count = Verts.Count;
            if (count == 0) return;

            var min = Verts[0].position;
            var max = min;
            for (var i = 1; i < count; i++)
            {
                min = Vector3.Min(min, Verts[i].position);
                max = Vector3.Max(max, Verts[i].position);
            }

            var w = Mathf.Abs(effectDistance.x);
            for (var i = 0; i < count; i++)
            {
                var v = Verts[i];
                var p = v.position;
                if (Mathf.Approximately(p.x, min.x)) p.x -= w;
                else if (Mathf.Approximately(p.x, max.x)) p.x += w;
                if (Mathf.Approximately(p.y, min.y)) p.y -= w;
                else if (Mathf.Approximately(p.y, max.y)) p.y += w;
                v.position = p;
                Color32 c = effectColor;
                if (useGraphicAlpha) c.a = (byte)(c.a * v.color.a / 255);
                v.color = c;
                Verts.Add(v);
            }

            // 테두리를 앞쪽에 두어 원래 면보다 먼저(뒤에) 그린다.
            Verts.AddRange(Verts.GetRange(0, count));
            Verts.RemoveRange(0, count);
            vh.Clear();
            vh.AddUIVertexTriangleStream(Verts);
        }
    }
}
