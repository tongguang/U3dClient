using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace U3dClient
{
    public class RadarChart : Graphic
    {
        [SerializeField]
        private List<float> m_LinePercents = new List<float>() { 1, 1, 1 };
        [SerializeField]
        private float m_OffsetAngle = 0f;
        [SerializeField]
        private float m_LineSize = 1f;
        [SerializeField]
        private Color m_FillColor = Color.cyan;
        [SerializeField]
        private Color m_LineColor = Color.yellow;

        private List<Vector3> m_LinePoints = new List<Vector3>();

        public List<float> LinePercents
        {
            set { m_LinePercents = value; SetVerticesDirty(); }
            get { return m_LinePercents; }
        }

        public float OffsetAngle
        {
            set { m_OffsetAngle = value; SetVerticesDirty(); }
            get { return m_OffsetAngle; }
        }

        public float LineSize
        {
            set { m_LineSize = value; SetVerticesDirty(); }
            get { return m_LineSize; }
        }

        public Color FillColor
        {
            set { FillColor = value; SetVerticesDirty(); }
            get { return FillColor; }
        }

        public Color LineColor
        {
            set { m_LineColor = value; SetVerticesDirty(); }
            get { return m_LineColor; }
        }


        [Button("Test1")]
        void Test1()
        {
            SetLinePercent(new List<float>() { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f });
        }

        [Button("Test2")]
        void Test2()
        {
            ChangeLinePercent(5, 0.5f);
        }

        public void SetLinePercent(List<float> linePercents)
        {
            m_LinePercents.Clear();
            foreach (var linePercent in linePercents)
            {
                m_LinePercents.Add(linePercent);
            }
            SetVerticesDirty();
        }

        public void ChangeLinePercent(int index, float linePercent)
        {
            if (index < 0 || index >= m_LinePercents.Count)
            {
                return;
            }

            m_LinePercents[index] = linePercent;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Rect pixelAdjustedRect = this.GetPixelAdjustedRect();
            //        Vector4 vector4 = new Vector4(pixelAdjustedRect.x, pixelAdjustedRect.y, pixelAdjustedRect.x + pixelAdjustedRect.width, pixelAdjustedRect.y + pixelAdjustedRect.height);
            vh.Clear();
            var center = new Vector2(pixelAdjustedRect.x + pixelAdjustedRect.width / 2, pixelAdjustedRect.y + pixelAdjustedRect.height / 2);
            vh.AddVert(new Vector3(center.x, center.y), m_FillColor, Vector2.zero);
            var pointNum = m_LinePercents.Count;
            var dirAngle = (360.0f / pointNum);
            for (int i = 0; i < pointNum; i++)
            {
                var angle = i * dirAngle + m_OffsetAngle;
                var radian = Mathf.Deg2Rad * angle;
                var lineLen = (pixelAdjustedRect.width / 2) * m_LinePercents[i];
                vh.AddVert(new Vector3(center.x + lineLen * Mathf.Cos(radian), lineLen * Mathf.Sin(radian)), m_FillColor, Vector2.zero);
            }

            for (int i = 2; i <= pointNum; i++)
            {
                vh.AddTriangle(0, i - 1, i);
            }
            vh.AddTriangle(0, pointNum, 1);

            m_LinePoints.Clear();
            for (int i = 0; i < pointNum; i++)
            {
                var angle = i * dirAngle + m_OffsetAngle;
                var radian = Mathf.Deg2Rad * angle;
                var lineLen = (pixelAdjustedRect.width / 2);
                m_LinePoints.Add(new Vector3(center.x + lineLen * Mathf.Cos(radian), lineLen * Mathf.Sin(radian)));
            }

            for (int i = 1; i < m_LinePoints.Count; i++)
            {
                DrawLine(vh, m_LinePoints[i - 1], m_LinePoints[i], m_LineSize, m_LineColor);
            }
            DrawLine(vh, m_LinePoints[m_LinePoints.Count - 1], m_LinePoints[0], m_LineSize, m_LineColor);
        }

        private void DrawLine(VertexHelper vh, Vector3 p1, Vector3 p2, float size, Color color)
        {
            Vector3 v = Vector3.Cross(p2 - p1, Vector3.forward).normalized * size;

            UIVertex[] vertex = new UIVertex[4];
            vertex[0].position = p1 + v;
            vertex[1].position = p2 + v;
            vertex[2].position = p2 - v;
            vertex[3].position = p1 - v;
            for (int j = 0; j < 4; j++)
            {
                vertex[j].color = color;
                vertex[j].uv0 = Vector2.zero;
            }
            vh.AddUIVertexQuad(vertex);
        }
    }
}

