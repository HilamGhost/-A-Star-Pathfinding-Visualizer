using System.Collections.Generic;
using UnityEngine;

namespace Hilam
{
    public static class PathFinder
    {
        public static VirtualCell[] FindPath(VirtualCell[,] grid, Vector2Int startPosition, Vector2Int endPosition)
        {
            var searchedCells = new List<Vector2Int>();
            var cellsToSearch = new List<Vector2Int> { startPosition };
            var finalPath = new List<VirtualCell>();
            int gridSize = grid.GetLength(0);

            VirtualCell startCell = grid[startPosition.x, startPosition.y];
            startCell.GCost = 0;
            startCell.HCost = GetDistance(startPosition, endPosition);
            startCell.FCost = startCell.GCost + startCell.HCost;
            startCell.Parent = null;

            while (cellsToSearch.Count > 0)
            {
                Vector2Int currentPos = cellsToSearch[0];
                VirtualCell currentCell = grid[currentPos.x, currentPos.y];

                foreach (var pos in cellsToSearch)
                {
                    var c = grid[pos.x, pos.y];
                    if (c.FCost < currentCell.FCost || (c.FCost == currentCell.FCost && c.HCost < currentCell.HCost))
                    {
                        currentPos = pos;
                        currentCell = grid[currentPos.x, currentPos.y];
                    }
                }

                cellsToSearch.Remove(currentPos);
                searchedCells.Add(currentPos);

                if (currentPos == endPosition)
                {
                    VirtualCell traceCell = grid[endPosition.x, endPosition.y];
                    while (traceCell != null && traceCell.GridPosition != startPosition)
                    {
                        finalPath.Add(traceCell);
                        traceCell = traceCell.Parent;
                    }
                    finalPath.Reverse();
                    return finalPath.ToArray();
                }

                List<Vector2Int> neighbors = GetNeighbors(currentPos, gridSize);

                foreach (Vector2Int neighborPos in neighbors)
                {
                    if (searchedCells.Contains(neighborPos)) continue;

                    VirtualCell neighborCell = grid[neighborPos.x, neighborPos.y];
                    if (neighborCell == null || !neighborCell.IsWalkable) continue;

                    int tentativeGCost = currentCell.GCost + GetDistance(currentPos, neighborPos);

                    if (!cellsToSearch.Contains(neighborPos) || tentativeGCost < neighborCell.GCost)
                    {
                        neighborCell.GCost = tentativeGCost;
                        neighborCell.HCost = GetDistance(neighborPos, endPosition);
                        neighborCell.FCost = neighborCell.GCost + neighborCell.HCost;
                        neighborCell.Parent = currentCell;

                        if (!cellsToSearch.Contains(neighborPos))
                        {
                            cellsToSearch.Add(neighborPos);
                        }
                    }
                }
            }

            return finalPath.ToArray();
        }

        private static List<Vector2Int> GetNeighbors(Vector2Int position, int gridSize)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int checkX = position.x + x;
                    int checkY = position.y + y;

                    if (checkX >= 0 && checkX < gridSize && checkY >= 0 && checkY < gridSize)
                    {
                        neighbors.Add(new Vector2Int(checkX, checkY));
                    }
                }
            }
            return neighbors;
        }

        private static int GetDistance(Vector2Int positionOne, Vector2Int positionTwo)
        {
            Vector2Int dist = new Vector2Int(Mathf.Abs(positionOne.x - positionTwo.x), Mathf.Abs(positionOne.y - positionTwo.y));
            int lowest = Mathf.Min(dist.x, dist.y);
            int highest = Mathf.Max(dist.x, dist.y);
            
            int horizontalMovesRequired = highest - lowest;
            
            return (lowest * 14) + (horizontalMovesRequired * 10);
        }
    }
}