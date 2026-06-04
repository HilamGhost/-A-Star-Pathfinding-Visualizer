using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Hilam
{
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")] 
        [SerializeField] private int _gridSize = 10;
        [SerializeField] private float _cellSize = 1f;
        [SerializeField,Range(0,1f)] private float _obstacleOpacity;
            
        [Header("Cell Settings")] 
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private GridChanger _startGridChanger;
        [SerializeField] private GridChanger _goalGridChanger;
        
        private Cell[,] m_grid;
        private VirtualCell[,] m_virtualGrid;
        private Camera m_gameCamera;
        
        private Vector2Int m_startCellPosition;
        private Vector2Int m_goalCellPosition;
        private bool m_isInitialized;

        #region Properties

        public Action<CellType, Cell> CellChanged { get; private set; }

        #endregion

        #region Unity Events

        private void Awake()
        {
            m_gameCamera = Camera.main;
            GenerateGrid();
        }

        private void OnEnable()
        {
            CellChanged += OnCellChanged;
        }

        private void OnDisable()
        {
            CellChanged -= OnCellChanged;
        }

        #endregion
        
        #region Grid Generation

        private void GenerateGrid()
        {
            m_grid = new Cell[_gridSize, _gridSize];
            m_virtualGrid = new VirtualCell[_gridSize, _gridSize];
            
    
            for (int x = 0; x < _gridSize; x++)
            {
                for (int y = 0; y < _gridSize; y++)
                {
                    Vector2 worldPosition = new Vector2(x * _cellSize, y * _cellSize);
            
                    GameObject spawnedCell = Instantiate(_cellPrefab, worldPosition, Quaternion.identity, transform);
            
                    Cell cellComponent = spawnedCell.GetComponent<Cell>();
            
                    bool isObstacle = Random.value < _obstacleOpacity;
            
                    cellComponent.Initialize(new Vector2Int(x, y), isObstacle ? CellType.Obstacle : CellType.Normal, _cellSize, this);
                    m_virtualGrid[x,y] = new VirtualCell(new  Vector2Int(x, y), !isObstacle);
            
                    m_grid[x, y] = cellComponent;
                }
            }
    
            CenterGridCamera();
            PlaceEntitiesRandomly();
        }
        
        private void PlaceEntitiesRandomly()
        {
            List<Vector2Int> walkablePositions = new List<Vector2Int>();
            
            for (int x = 0; x < _gridSize; x++)
            {
                for (int y = 0; y < _gridSize; y++)
                {
                    if (m_virtualGrid[x, y] != null && m_virtualGrid[x, y].IsWalkable)
                    {
                        walkablePositions.Add(new Vector2Int(x, y));
                    }
                }
            }

            if (walkablePositions.Count >= 2)
            {
                int startIndex = Random.Range(0, walkablePositions.Count);
                Vector2Int startPos = walkablePositions[startIndex];
                walkablePositions.RemoveAt(startIndex);

                int goalIndex = Random.Range(0, walkablePositions.Count);
                Vector2Int goalPos = walkablePositions[goalIndex];

                if (_startGridChanger != null)
                {
                    _startGridChanger.transform.position = new Vector3(startPos.x * _cellSize, startPos.y * _cellSize, -1f);
                    m_startCellPosition = startPos;
                }

                if (_goalGridChanger != null)
                {
                    _goalGridChanger.transform.position = new Vector3(goalPos.x * _cellSize, goalPos.y * _cellSize, -1f);
                    m_goalCellPosition = goalPos;
                }
                
                CreatePath();
                m_isInitialized = true;
            }
        }
        
        private void CenterGridCamera()
        {
            if (m_gameCamera != null)
            {
                float centerOffset = (_gridSize * _cellSize) / 2f - (_cellSize / 2f);
                m_gameCamera.transform.position = new Vector3(centerOffset, centerOffset, -10f);
                m_gameCamera.orthographicSize = (_gridSize * _cellSize) / 1.5f;
            }
        }

        #endregion

        #region Path Finding

        private void OnCellChanged(CellType cellType, Cell cell)
        {
            if (cellType is CellType.Start)
            {
                m_startCellPosition = cell.GridPosition;
                if(m_isInitialized) CreatePath();
                return;
            }

            if (cellType is CellType.Goal)
            {
                m_goalCellPosition = cell.GridPosition;
                if(m_isInitialized)  CreatePath();
                return;
            }
        }
        private void CreatePath()
        {
            var path = PathFinder.FindPath(m_virtualGrid, m_startCellPosition, m_goalCellPosition);
            foreach (var cell in m_grid)
            {
                cell.ChangeCellColor(false);
            }
            
            foreach (var pathCell in path)
            {
                var cell = m_grid[pathCell.GridPosition.x, pathCell.GridPosition.y];
                cell.ChangeCellColor(true);
            }
        }

        #endregion
        
        #region Public API

        public bool IsValidGridPosition(Vector2Int gridPos)
        {
            return gridPos.x >= 0 && gridPos.x < _gridSize && gridPos.y >= 0 && gridPos.y < _gridSize;
        }

        public Cell GetCell(Vector2Int gridPos)
        {
            if (IsValidGridPosition(gridPos) && m_virtualGrid[gridPos.x, gridPos.y]?.IsWalkable != null && m_virtualGrid[gridPos.x, gridPos.y].IsWalkable)
            {
                return m_grid[gridPos.x, gridPos.y];
            }
            return null;
        }

        public float GetCellSize()
        {
            return _cellSize;
        }
        
        public Vector2Int GetGridPositionFromWorld(Vector2 worldPosition)
        {
            int x = Mathf.RoundToInt(worldPosition.x / _cellSize);
            int y = Mathf.RoundToInt(worldPosition.y / _cellSize);
            return new Vector2Int(x, y);
        }
        
        public Cell GetClosestWalkableCell(Vector2 worldPosition)
        {
            Vector2Int gridPos = GetGridPositionFromWorld(worldPosition);
            
            gridPos.x = Mathf.Clamp(gridPos.x, 0, _gridSize - 1);
            gridPos.y = Mathf.Clamp(gridPos.y, 0, _gridSize - 1);

            if (m_virtualGrid[gridPos.x, gridPos.y] != null && m_virtualGrid[gridPos.x, gridPos.y].IsWalkable)
            {
                return m_grid[gridPos.x, gridPos.y];
            }

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            
            queue.Enqueue(gridPos);
            visited.Add(gridPos);

            Vector2Int[] directions = {
                Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,
                new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 1)
            };

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                if (m_virtualGrid[current.x, current.y] != null && m_virtualGrid[current.x, current.y].IsWalkable)
                {
                    return m_grid[current.x, current.y];
                }

                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighbor = current + dir;

                    if (IsValidGridPosition(neighbor) && !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return null;
        }
        
        #endregion
    }
}