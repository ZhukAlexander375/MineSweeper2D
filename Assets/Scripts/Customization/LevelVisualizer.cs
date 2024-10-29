using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LevelVisualizer : MonoBehaviour
{
    [SerializeField] private LevelConfig _levelConfig; // ������ �� LevelConfig ��� ������
    [SerializeField] private Color activeCellColor = Color.green; // ���� ��� �������� ������
    [SerializeField] private Vector2 cellSize = new Vector2(1, 1); // ������ ������ ��� ������������

    /*private void OnDrawGizmos()
    {
        if (_levelConfig == null || _levelConfig.Shape == null)
            return;

        Gizmos.color = activeCellColor;

        // ���������� �������� ������ �� LevelShape
        foreach (var cellPosition in _levelConfig.Shape.ActiveCells)
        {
            Vector3 position = new Vector3(cellPosition.x * cellSize.x, cellPosition.y * cellSize.y, 0);
            Gizmos.DrawCube(transform.position + position, new Vector3(cellSize.x, cellSize.y, 0.1f));
        }
    }*/
}
