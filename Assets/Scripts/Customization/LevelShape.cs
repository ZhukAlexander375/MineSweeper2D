using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelShape", menuName = "Minesweeper/Level Shape")]
public class LevelShape : ScriptableObject
{
    public Vector2Int[] ActiveCells;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // ������������ ������ � ���������
        foreach (var position in ActiveCells)
        {
            Gizmos.DrawCube(new Vector3(position.x, position.y, 0), Vector3.one); // ��� ������ ������ ������������
        }
    }
}
