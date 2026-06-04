using UnityEngine;

namespace Hilam
{
    public class Cell : MonoBehaviour
    {
        [Header("Cell Infos")] 
        [SerializeField] private Vector2Int _gridPosition;
        [SerializeField] private CellType _cellType = CellType.Normal;

        [Header("Cell Properties")]
        [SerializeField] private Color _cellColor = Color.white;
        [SerializeField] private Color _cellObstacleColor = Color.black;
        [SerializeField] private Color _cellPathColor = Color.darkGreen;
        
        public Vector2Int GridPosition => _gridPosition;
        private GridManager m_gridManager;
        private SpriteRenderer m_spriteRenderer;
        
        public void Initialize(Vector2Int gridPosition, CellType cellType, float cellScale, GridManager gridManager)
        {
            _gridPosition = gridPosition;
            _cellType = cellType;
            
            m_gridManager = gridManager;
            m_spriteRenderer = GetComponent<SpriteRenderer>();

            m_spriteRenderer.color = GetDefaultColor();
            transform.localScale = Vector3.one * cellScale;
            
            transform.name = $"Cell {_gridPosition.x}_{_gridPosition.y}";
        }
        
        private Color GetDefaultColor() => _cellType is CellType.Obstacle? _cellObstacleColor: _cellColor;
        
        public void ChangeCellColor(bool isPath) => m_spriteRenderer.color = isPath ? _cellPathColor : GetDefaultColor();
    }

    public class VirtualCell
    {
        public Vector2Int GridPosition {get; private set;}
        public bool IsWalkable {get; private set;}
        
        public int GCost {get; set;} 
        public int HCost {get; set;} 
        public int FCost {get; set;}
        public VirtualCell Parent { get; set; }

        public VirtualCell(Vector2Int gridPosition, bool isWalkable)
        {
            GridPosition = gridPosition;
            IsWalkable = isWalkable;
        }
    }
}