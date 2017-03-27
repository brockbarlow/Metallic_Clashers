﻿using UnityEngine;
using UnityEngine.UI;

public class BarLines : Graphic
{
    [Space, SerializeField]
    private Image m_ParentImage;
    [SerializeField]
    private float m_LineWidth = 1f;

    protected override void Start()
    {
        base.Start();

        if (!Application.isPlaying)
            return;

        GameManager.self.playerData.health.onTotalValueChanged.AddListener(OnTotalValueChanged);
        GameManager.self.playerData.defense.onTotalValueChanged.AddListener(OnTotalValueChanged);
    }

    private void OnTotalValueChanged()
    {
        SetAllDirty();
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (!Application.isPlaying || m_ParentImage == null)
            return;

        var vertex = UIVertex.simpleVert;
        vertex.color = color;

        var startingPoint = rectTransform.anchorMin;
        startingPoint -= rectTransform.pivot;
        startingPoint =
            new Vector2(
                rectTransform.rect.width * startingPoint.x,
                rectTransform.rect.height * startingPoint.y);

        startingPoint -= new Vector2(m_LineWidth / 2f, 0f);

        var parentWidth = m_ParentImage.rectTransform.rect.width;
        if (GameManager.self.playerData.health.totalValue + GameManager.self.playerData.defense.totalValue
            > GameManager.self.playerData.health.value)
            parentWidth = m_ParentImage.rectTransform.rect.width * m_ParentImage.fillAmount;

        var spacing = 10f / GameManager.self.playerData.health.value * parentWidth;

        for (var i = 0; i < GameManager.self.playerData.health.totalValue / 10; ++i)
        {
            var currentPosition = startingPoint + i * new Vector2(spacing, 0f);

            vertex.position = currentPosition;
            vh.AddVert(vertex);

            vertex.position = currentPosition + Vector2.up * rectTransform.rect.height;
            vh.AddVert(vertex);

            vertex.position = currentPosition + Vector2.right * m_LineWidth;
            vh.AddVert(vertex);

            vertex.position =
                currentPosition + Vector2.right * m_LineWidth + Vector2.up * rectTransform.rect.height;
            vh.AddVert(vertex);

            var vertexOffset = i * 4;
            vh.AddTriangle(0 + vertexOffset, 1 + vertexOffset, 2 + vertexOffset);
            vh.AddTriangle(1 + vertexOffset, 3 + vertexOffset, 2 + vertexOffset);
        }
    }
}
