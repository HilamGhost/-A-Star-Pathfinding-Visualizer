using UnityEngine;

namespace Hilam
{
    public class GridChanger : MonoBehaviour
    {
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private CellType _cellType;

        private Camera m_mainCamera;
        private Cell m_currentOccupiedCell;

        private void Start()
        {
            m_mainCamera = Camera.main;
            
            Invoke(nameof(SnapToClosestCell), 0.1f);
        }

        private void OnMouseDrag()
        {
            Vector3 mousePos = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = -1f;
            transform.position = mousePos;
        }

        private void OnMouseUp()
        {
            SnapToClosestCell();
        }

        private void SnapToClosestCell()
        {
            Cell closestCell = _gridManager.GetClosestWalkableCell(transform.position);

            if (closestCell != null)
            {
                if (m_currentOccupiedCell != null && m_currentOccupiedCell != closestCell)
                {
                    m_currentOccupiedCell.Initialize(m_currentOccupiedCell.GridPosition, CellType.Normal, _gridManager.GetCellSize(), _gridManager);
                }

                transform.position = new Vector3(closestCell.transform.position.x, closestCell.transform.position.y, -1f);
                m_currentOccupiedCell = closestCell;

                m_currentOccupiedCell.Initialize(m_currentOccupiedCell.GridPosition, _cellType, _gridManager.GetCellSize(), _gridManager);
                _gridManager.CellChanged.Invoke(_cellType, closestCell);
            }
        }
    }
}
