using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Zenject;

public class Sector : MonoBehaviour
{
    [SerializeField] private List<TileSetConfig> _tileSets;
    [SerializeField] private SectorUI _sectorUi;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private LineRenderer _borders;
    //public Tilemap _tilemap { get; private set; }
        
    public bool IsActive;
    public bool IsFirstCellActivated {  get; set; }
    public bool IsExploded { get; set; }
    public bool IsPrizePlaced { get; set; }
    public bool IsCellsInitialized { get; set; }
    public bool IsSectorCompleted {  get; set; }
    public int CurrentBuyoutCost { get; set; }

    public Dictionary<Vector3Int, InfiniteCell> Cells => _cells;
    public bool IsLoaded;
       

    private InfiniteGridManager _infiniteGridManager;
    private Dictionary<Vector3Int, InfiniteCell> _cells = new Dictionary<Vector3Int, InfiniteCell>();
    private TileSetConfig _currentTileSet;
    private int _currentTileSetIndex;
    private int _currentRevealedCells;
    private int _totalCellsCount;


    private ThemeManager _themeManager;
    private PlayerProgress _playerProgress;
    private GameManager _gameManager;
    private SceneLoader _sceneLoader;

    [Inject]
    private void Construct(ThemeManager themeManager, PlayerProgress playerProgress, GameManager gameManager, SceneLoader sceneLoader)
    {
        _themeManager = themeManager;
        _playerProgress = playerProgress;
        _gameManager = gameManager;
        _sceneLoader = sceneLoader;
    }


    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();        

        if (_cells.Count == 0)
        {
            InitializeCells();
            GenerateAward();
        }
        //DrawBorders();

        SectorUIInit();
        CheckExplodedSector();
        SectorCompletionCheck();

        SignalBus.Subscribe<OnCellActiveSignal>(SectorActivate);
        SignalBus.Subscribe<ThemeChangeSignal>(OnThemeChanged);
        //SignalBus.Subscribe<OnVisibleMinesSignal>(ShowMines);
        TryApplyTheme(_themeManager.CurrentThemeIndex);
    }

    private void SectorUIInit()
    {
        _sectorUi.SectorInit(this, _themeManager, _playerProgress, _gameManager, _sceneLoader);
    }

    private void DrawBorders()      //FOR CHANGE COLOR MB
    {        
        if (_borders == null)
        {
            Debug.LogWarning("LineRenderer не найден в префабе сектора.");
            return;
        }

        _borders.positionCount = 5;
        _borders.SetPositions(new Vector3[]
        {
            new Vector3(0, 0, 0),               // Нижний левый угол
            new Vector3(9, 0, 0),                // Нижний правый угол
            new Vector3(9, 9, 0),                  // Верхний правый угол
            new Vector3(0, 9, 0),                // Верхний левый угол
            new Vector3(0, 0, 0)               // Замыкаем линию
        });
    }

    private void CheckExplodedSector()
    {
        if (IsExploded)
        {
            CloseSector();
        }
    }

    private void InitializeCells()
    {
        //_totalCellsCount = _infiniteGridManager.SectorSize * _infiniteGridManager.SectorSize;
        //Debug.Log(_totalCellsCount);

        Vector3Int boundsMin = _tilemap.cellBounds.min;
        Vector3Int boundsMax = _tilemap.cellBounds.max;

        for (int x = boundsMin.x; x < boundsMax.x; x++)
        {
            for (int y = boundsMin.y; y < boundsMax.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                TileBase tile = _tilemap.GetTile(position);

                if (tile != null)
                {
                    InfiniteCell cell = new InfiniteCell();
                    _cells[new Vector3Int(x, y, 0)] = cell;                    
                    cell.GlobalCellPosition = position + new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
                    
                    cell.SetOwnerSector(this);
                    cell.SetGridManager(_infiniteGridManager);
                }
            }
        }        
        //LogAllCellKeys();
    }

    public void HandleCellClick(int cellX, int cellY)
    {

        cellX %= 9;
        cellY %= 9;

        if (cellX >= 0 && cellX < 9 && cellY >= 0 && cellY < 9) ///9 - hard
        {
            if (IsExploded || IsSectorCompleted)
            {
                //Debug.Log("This sector is exploded and cannot be interacted with.");
                return;
            }

            //var clickedCell = _cells[new Vector3Int(cellX, cellY, 0)];

            Vector3Int cellKey = new Vector3Int(cellX, cellY, 0);
                        
            if (!_cells.TryGetValue(cellKey, out var clickedCell))
            {
                Debug.LogWarning($"Attempting to click on a non-existent cell {cellKey} in sector. Total cells in the sector: {_cells.Count}");
                return;
            }

            if (!_infiniteGridManager.IsFirstClick)
            {
                _infiniteGridManager.GenerateFirstSectors(this, clickedCell);
                _infiniteGridManager.Reveal(this, clickedCell);
                clickedCell.IsActive = true; // pri samom pervom click 
            }

            else if (clickedCell.IsActive)
            {
                _infiniteGridManager.Reveal(this, clickedCell);
            }

            else if (!clickedCell.IsActive)
            {
                SignalBus.Fire(new WrongClickSignal());
            }
            
            //_gridManager.Reveal(this, clickedCell);
        }
    }

    public void HandleCellFlag(int cellX, int cellY)
    {

        cellX %= 9;
        cellY %= 9;

        if (cellX >= 0 && cellX < 9 && cellY >= 0 && cellY < 9) ///8 - hard
        {
            if (IsExploded || IsSectorCompleted)
            {                
                return;
            }

            var clickedCell = _cells[new Vector3Int(cellX, cellY, 0)];

            if (!_infiniteGridManager.IsFirstClick)
            {
                return;
            }

            else if (clickedCell.IsActive)
            {
                _infiniteGridManager.Flag(this, clickedCell);
            }
        }
    }

    public void HandleChord(int cellX, int cellY)
    {
        cellX %= 9;
        cellY %= 9;
        
        if (cellX >= 0 && cellX < 9 && cellY >= 0 && cellY < 9) // 9 - размер сектора
        {
            if (IsExploded)
            {
                return;
            }

            var clickedCell = _cells[new Vector3Int(cellX, cellY, 0)];

            if (!_infiniteGridManager.IsFirstClick)
            {
                return;
            }

            if (clickedCell.IsRevealed && clickedCell.CellState == CellState.Number)
            {
                int adjacentFlags = _infiniteGridManager.CountAdjacentFlags(clickedCell);
                if (adjacentFlags >= clickedCell.CellNumber)
                {                    
                    _infiniteGridManager.Unchord(this, clickedCell);
                }
            }
        }
    }

    public void GenerateAward()
    {
        if (_infiniteGridManager.IsFirstClick && !IsPrizePlaced)
        {
            Vector3Int randomPosition = new Vector3Int(
                Random.Range(0, _infiniteGridManager.SectorSize),
                Random.Range(0, _infiniteGridManager.SectorSize),
                0
                );
            InfiniteCell cell = GetCell(randomPosition);

            cell.IsAward = true;
            IsPrizePlaced = true;

            UpdateTile(cell.GlobalCellPosition, cell);
            //DrawSector();
        }
    }

    /*public void DrawSector()
    {
        if (_tilemap == null)
        {
            Debug.LogError("Tilemap не установлен для сектора!");
            return;
        }

        int width = _infiniteGridManager.SectorSize;
        int height = _infiniteGridManager.SectorSize;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                InfiniteCell cell = _cells[new Vector3Int(x, y, 0)];
                _tilemap.SetTile(new Vector3Int(x, y, 0), GetTile(cell));
            }
        }

        _tilemap.RefreshAllTiles();
    }*/

    public async void UpdateTile(Vector3Int globalPosition, InfiniteCell cell)
    {
        if (_tilemap == null) return;
        Vector3Int localPosition = GetLocalPosition(globalPosition);

        _tilemap.SetTile(localPosition, GetTile(cell));
        _tilemap.RefreshTile(localPosition);

        if (cell.IsRevealed && cell.CellState == CellState.Mine && !cell.HasAnimated)
        {
            cell.HasAnimated = true;

            float duration = 0.7f; // Длительность тряски
            float shakeStrength = 0.2f; // Сила тряски

            await DOTween.To(() => 0f, x =>
            {
                // Генерируем случайные смещения (эмуляция DOShakePosition)
                float shakeX = Random.Range(-shakeStrength, shakeStrength);
                float shakeY = Random.Range(-shakeStrength, shakeStrength);
                Matrix4x4 shakeMatrix = Matrix4x4.TRS(new Vector3(shakeX, shakeY, 0), Quaternion.identity, Vector3.one);
                _tilemap.SetTransformMatrix(localPosition, shakeMatrix);
                _tilemap.RefreshTile(localPosition);
            }, 1f, duration).SetEase(Ease.Linear).AsyncWaitForCompletion();

            // Анимация увеличения (взрыв)
            //await DOTween.To(() => 1f, x => {
            //    Matrix4x4 explodeMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(x * 3f, x * 3f, 1f));
            //    _tilemap.SetTransformMatrix(localPosition, explodeMatrix);
            //    _tilemap.RefreshTile(localPosition);
            //}, 1f, 0.15f).SetEase(Ease.InOutExpo).AsyncWaitForCompletion();

            // **Ставим статичным тайлом**
            TileBase mineTile = GetFinalTile(cell);
            _tilemap.SetTile(localPosition, mineTile);
            _tilemap.SetTransformMatrix(localPosition, Matrix4x4.identity);
            _tilemap.RefreshTile(localPosition);
            //Debug.Log("+1");
            return;
        }

        if (cell.IsRevealed && (cell.CellState == CellState.Empty || cell.CellState == CellState.Number) && !cell.HasAnimated)
        {
            cell.HasAnimated = true;

            TileBase staticTile = GetFinalTile(cell);
            _tilemap.SetTile(localPosition, staticTile);

            // Начальная матрица: масштаб 0 (тайл "невидим")
            Matrix4x4 startMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.zero);            

            // Устанавливаем начальную матрицу
            _tilemap.SetTransformMatrix(localPosition, startMatrix);
            _tilemap.RefreshTile(localPosition);

            float duration = 0.2f; // Длительность анимации (настраивается по желанию)

            // Анимируем масштаб (поскольку DOTween напрямую не интерполирует матрицы,
            // интерполируем скалярное значение от 0 до 1, и каждый раз пересчитываем матрицу)
            float currentScale = 0.3f;

            await DOTween.To(() => currentScale, x => {
                currentScale = x;
                Matrix4x4 currentMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(currentScale, currentScale, 1f));
                _tilemap.SetTransformMatrix(localPosition, currentMatrix);
                _tilemap.RefreshTile(localPosition);
            }, 1f, duration).AsyncWaitForCompletion();

            // По окончании анимации устанавливаем финальный статичный тайл и сбрасываем матрицу в Identity
            //TileBase staticTile = GetFinalTile(cell);
            //_tilemap.SetTile(localPosition, staticTile);

            _tilemap.SetTransformMatrix(localPosition, Matrix4x4.identity);
            _tilemap.RefreshTile(localPosition);
            //Debug.Log("+1");
        }
        else
        {
            // Для остальных случаев просто обновляем тайл
            _tilemap.SetTile(localPosition, GetTile(cell));
            _tilemap.RefreshTile(localPosition);
        }
    }

    public void GenerateNumbersInSector()
    {
        _infiniteGridManager.GenerateNumbers(this);
    }

    private Tile GetTile(InfiniteCell cell)
    { 
        if (cell.IsRevealed)
        {
            //SectorCompletionCheck();
            return GetFinalTile(cell);            
        }

        else if (cell.IsFlagged)
        {
            return _tileSets[_currentTileSetIndex].TileFlag;
        }

        else if (cell.IsActive && cell.IsAward)
        {
            return _tileSets[_currentTileSetIndex].TileActivePrize;
        }

        else if (cell.IsActive && !cell.IsRevealed)
        {
            return _tileSets[_currentTileSetIndex].TileActive;            ///////////////
        }        

        else if (!cell.IsActive && cell.IsAward)
        {
            return _tileSets[_currentTileSetIndex].TileInactivePrize;
        }        

        else
        {
            return _tileSets[_currentTileSetIndex].TileInactive;              /////////////////////
        }
    }

    public void SectorCompletionCheck()
    {
        if (IsExploded) return;

        if (IsSectorCompleted)
        {
            OnSectorCompleted();
            return;
        }

        bool isSectorCompleted = true;
        
        foreach (var cell in Cells.Values)
        {
            if (!cell.IsRevealed && cell.CellState != CellState.Mine)
            {
                isSectorCompleted = false;
                break;
            }

            if (cell.CellState == CellState.Mine && !cell.IsRevealed && !cell.IsFlagged && !cell.IsExploded)
            {
                isSectorCompleted = false;
                break;
            }

            if (cell.CellState == CellState.Mine && cell.IsExploded)
            {
                continue;
            }

            if (cell.IsFlagged && cell.CellState != CellState.Mine)
            {
                isSectorCompleted = false;
                break;
            }
        }
        
        if (isSectorCompleted)
        {
            IsSectorCompleted = true;
            OnSectorCompleted();
        }
    }

    private void OnSectorCompleted()
    {
        _sectorUi.gameObject.SetActive(true);
        _sectorUi.CompletedSector();
        //Debug.Log($"Сектор завершён: {name}");        
    }

    private Tile GetFinalTile(InfiniteCell cell)
    {
        switch (cell.CellState)
        {
            case CellState.Empty:                
                return _tileSets[_currentTileSetIndex].TileEmpty;

            case CellState.Mine: 
                return cell.IsExploded ? _tileSets[_currentTileSetIndex].TileExploded : _tileSets[_currentTileSetIndex].TileMine;

            case CellState.Number:               
                return GetNumberTile(cell);

            default: return null;
        }
    }

    private Vector3Int GetLocalPosition(Vector3Int globalPosition)
    {
        Vector2Int sectorPosition = new Vector2Int(
            Mathf.FloorToInt(transform.position.x / _infiniteGridManager.SectorSize),
            Mathf.FloorToInt(transform.position.y / _infiniteGridManager.SectorSize)
        );

        Vector3Int sectorOrigin = new Vector3Int(
            sectorPosition.x * _infiniteGridManager.SectorSize,
            sectorPosition.y * _infiniteGridManager.SectorSize,
            0
        );

        return globalPosition - sectorOrigin;
    }    

    private Tile GetNumberTile(InfiniteCell cell)
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


    public InfiniteCell GetCell(Vector3Int coordinates)
    {
        if (_cells.ContainsKey(coordinates))
        {
            return _cells[coordinates];
        }
        return null;
    }

    public void SetManager(InfiniteGridManager gridManager)
    {
        _infiniteGridManager = gridManager;
    }    

    public void SectorActivate(OnCellActiveSignal signal)
    {
        if (signal.Cell.Sector == this)
        {
            var cell = signal.Cell;
            
            if (!IsFirstCellActivated)
            {                
                _infiniteGridManager.GenerateSectors(this, cell);
                IsFirstCellActivated = true;
                IsActive = true;                
            }

            if (IsFirstCellActivated)
            {
                return;
            }
        }
    }


    public void NotifyThemeChange()
    {
        SignalBus.Fire(new ThemeChangeSignal(_themeManager.CurrentTheme, _themeManager.CurrentThemeIndex));
    }

    public void TryApplyTheme(int themeIndex)
    {
        if (themeIndex < 0 || themeIndex >= _tileSets.Count)
        {
            return;
        }

        _currentTileSet = _tileSets[themeIndex];
        _currentTileSetIndex = themeIndex;
        if (_tilemap != null)
        {
            RedrawSector();
        }
    }

    public void RedrawSector()
    {
        if (_tilemap == null) return;
               
        foreach (var cellPosition in _cells.Keys)
        {
            var cell = _cells[cellPosition];
            _tilemap.SetTile(cellPosition, GetTile(cell));
        }

        _tilemap.RefreshAllTiles();
    }

    public void ExplodeSector(InfiniteCell cell)
    {
        cell.IsExploded = true;
        cell.IsRevealed = true;
        CloseSector();
    }

    public void SetBuyoutCost(SectorBuyoutCostConfig sectorBuyoutCostConfig, int explodedMines)
    {
        int index = Mathf.Clamp(explodedMines - 1, 0, sectorBuyoutCostConfig.SectorBuyoutCost.Length - 1);
        CurrentBuyoutCost = sectorBuyoutCostConfig.SectorBuyoutCost[index];

        //CurrentBuyoutCost = sectorBuyoutCostConfig.SectorBuyoutCost[explodedMines - 1];
        //Debug.Log(CurrentBuyoutCost);
    }

    public async void CloseSector()
    {
        IsExploded = true;

        await UniTask.Delay(1000);

        _sectorUi.gameObject.SetActive(true);
        _sectorUi.HideLostSector();
    }

    public void OpenSector(int priseCount)
    {
        if (_playerProgress.CheckAwardCount(priseCount))
        {
            IsExploded = false;
            SignalBus.Fire(new OnGameRewardSignal(0, -priseCount));
            _sectorUi.gameObject.SetActive(false);

            SectorCompletionCheck();
        }
    }

    private void OnThemeChanged(ThemeChangeSignal signal)
    {
        TryApplyTheme(signal.ThemeIndex);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnCellActiveSignal>(SectorActivate);
        SignalBus.Unsubscribe<ThemeChangeSignal>(OnThemeChanged);
        //SignalBus.Unsubscribe<OnVisibleMinesSignal>(ShowMines);
    }


    /// <summary>
    /// DELETE!!!!!!!!!!!!!!!!!!!!!11 It's for tests
    /// </summary>

    //private bool _minesVisible = false;

    /*public void ShowMines(OnVisibleMinesSignal signal)
    {
        _minesVisible = signal.IsVisible;
        DrawMinesOfSector(_minesVisible);
    }*/

    /*private void DrawMinesOfSector(bool visible)
    {
        if (_tilemap == null)
        {
            Debug.LogError("Tilemap не установлен для сектора!");
            return;
        }

        foreach (var cellEntry in _cells)
        {
            InfiniteCell cell = cellEntry.Value;
            Vector3Int pos = cellEntry.Key;

            if (cell.CellState == CellState.Mine)
            {
                _tilemap.SetTile(pos, visible ? _tileSets[_currentTileSetIndex].TileMineVisible : GetTile(cell));
            }
        }

        _tilemap.RefreshAllTiles();
    }    */


    [ContextMenu("Test")]
    public void TestSector()
    {        
        StartCoroutine(OpenCellsGradually());
    }

    private IEnumerator OpenCellsGradually()
    {        
        foreach (var pos in _cells.Keys)
        {            
            Vector3Int tilePosition = new Vector3Int(pos.x, pos.y, 0);
            _tilemap.SetTile(tilePosition, _tileSets[_currentTileSetIndex].TileFlag);
            yield return new WaitForSeconds(0.5f);
        }        
    }

    public SectorData SaveSectorData()
    {
        var sectorData = new SectorData
        {
            SectorPosition = new Vector2Int(
                            Mathf.FloorToInt(transform.position.x / _infiniteGridManager.SectorSize),
                            Mathf.FloorToInt(transform.position.y / _infiniteGridManager.SectorSize)), //save with sector position -2 -1 i t.d.( / 8) OK!!!!
            IsActive = IsActive,
            IsFirstCellActivated = IsFirstCellActivated,
            IsExploded = IsExploded,
            IsPrizePlaced = IsPrizePlaced,
            IsLOADED = IsLoaded,            ///DELETE    
            IsCellsInitialized = IsCellsInitialized,
            IsSectorCompleted = IsSectorCompleted,
            CurrentBuyoutCost = CurrentBuyoutCost,            
        };

        foreach (var cellPair in _cells)
        {
            var cell = cellPair.Value;
            var cellData = new CellData
            {
                GlobalCellPosition = cell.GlobalCellPosition,
                CellState = cell.CellState,
                IsActive = cell.IsActive,
                IsAward = cell.IsAward,
                IsRevealed = cell.IsRevealed,
                IsFlagged = cell.IsFlagged,
                IsExploded = cell.IsExploded,
                Chorded = cell.Chorded,
                CellNumber = cell.CellNumber,
                HasAnimated = cell.HasAnimated,
            };

            //Debug.Log($"Cell position {cell.GlobalCellPosition}");
            sectorData.Cells.Add(cellData);
        }

        return sectorData;
    }


    public void InitializeCellsFromData(List<CellData> cells, InfiniteGridManager infiniteGridManager)
    {
        foreach (var cellData in cells)
        {
            Vector3Int globalPosition = cellData.GlobalCellPosition;

            Vector2Int sectorPosition = new Vector2Int(Mathf.FloorToInt(transform.position.x / _infiniteGridManager.SectorSize),
                                                       Mathf.FloorToInt(transform.position.y / _infiniteGridManager.SectorSize));
            
            Vector3Int sectorOrigin = new Vector3Int(sectorPosition.x * _infiniteGridManager.SectorSize,
                                                      sectorPosition.y * _infiniteGridManager.SectorSize, 0);
            
            Vector3Int localPosition = globalPosition - sectorOrigin;

            TileBase tile = _tilemap.GetTile(localPosition);

            if (tile != null)
            {
                if (!_cells.ContainsKey(localPosition))
                {
                    InfiniteCell cell = new InfiniteCell();
                    _cells[localPosition] = cell;                    

                    cell.SetOwnerSector(this);
                    cell.SetGridManager(infiniteGridManager);

                    cell.GlobalCellPosition = cellData.GlobalCellPosition;
                    cell.CellState = cellData.CellState;
                    cell.IsRevealed = cellData.IsRevealed;
                    cell.IsFlagged = cellData.IsFlagged;                    
                    cell.IsExploded = cellData.IsExploded;
                    cell.Chorded = cellData.Chorded;
                    cell.IsActive = cellData.IsActive;
                    cell.IsAward = cellData.IsAward;
                    cell.CellNumber = cellData.CellNumber;
                    cell.HasAnimated = cellData.HasAnimated;

                    UpdateTile(cell.GlobalCellPosition, cell);
                    //Debug.Log($"Cell position {cell.GlobalCellPosition} CellState {cell.CellState} cell.IsRevealed {cell.IsRevealed}, IsFlagged {cell.IsFlagged}, IsAward {cell.IsAward}");
                }
                else
                {
                    Debug.Log($"Cell at position {localPosition} not found in sector {name}");
                }
            }
            else
            {
                Debug.Log($"No tile found at position {localPosition} in sector {name}");
            }
        }        
        //LogAllCellKeys();
        //DrawSector();
    }

    public void LogAllCellKeys()
    {
        if (_cells == null)
        {
            Debug.LogError("Словарь _cells не инициализирован!");
            return;
        }

        foreach (var key in _cells.Keys)
        {
            Debug.Log($"Ключ: {key}");
        }
    }
}

