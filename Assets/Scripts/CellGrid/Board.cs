using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

[RequireComponent(typeof(Tilemap))]
public class Board : MonoBehaviour
{ 
    public Tilemap Tilemap { get; private set; }

    [SerializeField] private List<TileSetConfig> _tileSets;

    private TileSetConfig _currentTileSet;
    private int _currentTileSetIndex;

    private ThemeManager _themeManager;

    [Inject]
    private void Construct(ThemeManager themeManager)
    {
        _themeManager = themeManager;
    }

    private void Awake()
    {
        Tilemap = GetComponent<Tilemap>();        
    }

    private void Start()
    {
        SignalBus.Subscribe<ThemeChangeSignal>(OnThemeChanged);
        TryApplyTheme(_themeManager.CurrentThemeIndex);
    }
    /*public void DrawFreeForm(FreeForm freeGrid, CellGrid grid)
    {
        var width = (int)Mathf.Sqrt(freeGrid.GridSize);

        for (int x = 0; x < freeGrid.GridSize; x++)
        {            
            if (freeGrid.ActiveCells[x])
            {
                Cell cell = grid[x % width, x / width];                   
                Tilemap.SetTile(cell.CellPosition, GetTile(cell));
            }

            *//*else
            {
                Cell cell = grid[x - 1, y-1];
                Tilemap.SetTile(cell.CellPosition, null);
            } *//*
            
        }
    }*/
    public void ClearGrid()
    {
        if (Tilemap != null)
        {
            Tilemap.ClearAllTiles();
        }
    }

    public async void UpdateTile(Vector3Int localPosition, BaseCell cell)
    {
        if (Tilemap == null) return;

        if (cell == null) return;
        Tilemap.SetTile(localPosition, GetTile(cell));
        Tilemap.RefreshTile(localPosition);

        if (cell.IsRevealed && cell.CellState == CellState.Mine && !cell.HasAnimated)
        {
            cell.HasAnimated = true;

            float duration = 0.7f; // Длительность тряски
            float shakeStrength = 0.2f; // Сила тряски
            try
            {
                await DOTween.To(() => 0f, x =>
                {
                    // Генерируем случайные смещения (эмуляция DOShakePosition)
                    float shakeX = Random.Range(-shakeStrength, shakeStrength);
                    float shakeY = Random.Range(-shakeStrength, shakeStrength);
                    Matrix4x4 shakeMatrix = Matrix4x4.TRS(new Vector3(shakeX, shakeY, 0), Quaternion.identity, Vector3.one);
                    Tilemap.SetTransformMatrix(localPosition, shakeMatrix);
                    Tilemap.RefreshTile(localPosition);
                }, 1f, duration).SetEase(Ease.Linear).AsyncWaitForCompletion();
            }
            catch { return; }
            // Анимация увеличения (взрыв)
            //await DOTween.To(() => 1f, x => {
            //    Matrix4x4 explodeMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(x * 3f, x * 3f, 1f));
            //    _tilemap.SetTransformMatrix(localPosition, explodeMatrix);
            //    _tilemap.RefreshTile(localPosition);
            //}, 1f, 0.15f).SetEase(Ease.InOutExpo).AsyncWaitForCompletion();

            // **Ставим статичным тайлом**
            if (Tilemap == null) return;
            TileBase mineTile = GetFinalTile(cell);
            Tilemap.SetTile(localPosition, mineTile);
            Tilemap.SetTransformMatrix(localPosition, Matrix4x4.identity);
            Tilemap.RefreshTile(localPosition);
            //Debug.Log("+1");
            return;
        }

        if (cell.IsRevealed && (cell.CellState == CellState.Empty || cell.CellState == CellState.Number) && !cell.HasAnimated)
        {
            cell.HasAnimated = true;

            TileBase staticTile = GetFinalTile(cell);
            if (Tilemap == null) return;
            Tilemap.SetTile(localPosition, staticTile);

            // Начальная матрица: масштаб 0 (тайл "невидим")
            Matrix4x4 startMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.zero);

            // Устанавливаем начальную матрицу
            if (Tilemap == null) return;
            Tilemap.SetTransformMatrix(localPosition, startMatrix);
            Tilemap.RefreshTile(localPosition);

            float duration = 0.2f; // Длительность анимации (настраивается по желанию)

            // Анимируем масштаб (поскольку DOTween напрямую не интерполирует матрицы,
            // интерполируем скалярное значение от 0 до 1, и каждый раз пересчитываем матрицу)
            float currentScale = 0.3f;
            try
            {
                await DOTween.To(() => currentScale, x =>
                {
                    currentScale = x;
                    Matrix4x4 currentMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(currentScale, currentScale, 1f));
                    Tilemap.SetTransformMatrix(localPosition, currentMatrix);
                    Tilemap.RefreshTile(localPosition);
                }, 1f, duration).AsyncWaitForCompletion();
            }
            catch { return; }


            // По окончании анимации устанавливаем финальный статичный тайл и сбрасываем матрицу в Identity
            //TileBase staticTile = GetFinalTile(cell);
            //_tilemap.SetTile(localPosition, staticTile);
            if (Tilemap == null) return;
            Tilemap.SetTransformMatrix(localPosition, Matrix4x4.identity);
            Tilemap.RefreshTile(localPosition);            
        }
        else
        {
            // Для остальных случаев просто обновляем тайл
            if (Tilemap == null) return;
            Tilemap.SetTile(localPosition, GetTile(cell));
            Tilemap.RefreshTile(localPosition);
        }
    }

    public void Draw(CellGrid grid)
    {
        if (_themeManager != null)
        {
            _currentTileSetIndex = _themeManager.CurrentThemeIndex;
        }        

        ClearGrid();

        if (grid == null)
        {
            Debug.LogWarning("Grid is null. Cannot draw.");
            return;
        }

        int width = grid.Width;
        int height = grid.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                BaseCell cell = grid[x, y];
                Tilemap.SetTile(cell.GlobalCellPosition, GetTile(cell));
            }
        }
    }

    private Tile GetTile(BaseCell cell)
    {
        if (cell.IsRevealed)
        {
            return GetFinalTile(cell);
        }
        else if (cell.IsFlagged)
        {
            return _tileSets[_currentTileSetIndex].TileFlag;
        }
        else
        {
            return _tileSets[_currentTileSetIndex].TileActive;
        }
    }

    private Tile GetFinalTile(BaseCell cell)
    {
        switch (cell.CellState)
        {
            case CellState.Empty: return _tileSets[_currentTileSetIndex].TileEmpty;
            case CellState.Mine: return cell.IsExploded ? _tileSets[_currentTileSetIndex].TileExploded : _tileSets[_currentTileSetIndex].TileMine;
            case CellState.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile(BaseCell cell)
    {
        switch (cell.CellNumber)
        {
            case 1: return _tileSets[_currentTileSetIndex].TileNum1;
            case 2: return _tileSets[_currentTileSetIndex].TileNum2;
            case 3: return _tileSets[_currentTileSetIndex].TileNum3;
            case 4: return _tileSets[_currentTileSetIndex].TileNum4;
            case 5: return _tileSets[_currentTileSetIndex].TileNum5;
            case 6: return _tileSets[_currentTileSetIndex].TileNum6;
            case 7: return _tileSets[_currentTileSetIndex].TileNum7;
            case 8: return _tileSets[_currentTileSetIndex].TileNum8;
            default: return null;
        }
    }

    public void TryApplyTheme(int themeIndex)
    {
        if (themeIndex < 0 || themeIndex >= _tileSets.Count)
        {
            Debug.LogWarning($"Недопустимый индекс темы: {themeIndex}");
            return;
        }
        
        _currentTileSet = _tileSets[themeIndex];
        _currentTileSetIndex = themeIndex;       
                
        RedrawGrid();
    }

    private void RedrawGrid()
    {
        if (Tilemap == null) return;
        {
            Tilemap.RefreshAllTiles();
        }
    }


    private void OnThemeChanged(ThemeChangeSignal signal)
    {
        TryApplyTheme(signal.ThemeIndex);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<ThemeChangeSignal>(OnThemeChanged);
    }
}
