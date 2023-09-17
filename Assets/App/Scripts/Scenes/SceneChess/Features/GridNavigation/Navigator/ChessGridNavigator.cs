using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        private Dictionary<ChessUnitType, List<Vector2Int>> _possibleMoves;
        public ChessGridNavigator() 
        {
            _possibleMoves = new Dictionary<ChessUnitType, List<Vector2Int>>()
            {
                {ChessUnitType.Pon, new List<Vector2Int>() {new Vector2Int(0, 1), new Vector2Int(0, -1)}},
                {ChessUnitType.Knight, new List<Vector2Int>() {new Vector2Int(2, 1), new Vector2Int(2, -1), new Vector2Int(-2, 1), new Vector2Int(-2, -1), new Vector2Int(1, 2), new Vector2Int(-1, 2), new Vector2Int(-1, -2), new Vector2Int(1, -2)}},
                {ChessUnitType.Rook, new List<Vector2Int>() {new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1)}},
                {ChessUnitType.Bishop, new List<Vector2Int>() {new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1)}},
                {ChessUnitType.King, new List<Vector2Int>() {new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1)}}
            };
            _possibleMoves.Add(ChessUnitType.Queen,_possibleMoves[ChessUnitType.King]);
        }
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            var firstNode = new CellNode(null, to,default(Vector2Int),0); 
            if (!IsValid(firstNode.Position, grid)) return null;
            
            var piecePossibleMoves = _possibleMoves[unit];
            var visitedCellsDistances = new Dictionary<Vector2Int,int>(); 
            var searchQueue = new Queue<CellNode>(); 
            var shortestWay = new CellNode(null, default(Vector2Int), default(Vector2Int), int.MaxValue);
            searchQueue.Enqueue(firstNode);
            while (searchQueue.Count != 0)
            {
                var currentNode = searchQueue.Dequeue();
                if (!visitedCellsDistances.TryAdd(currentNode.Position, currentNode.Distance)) 
                {
                    if (visitedCellsDistances[currentNode.Position] < currentNode.Distance) continue;
                    visitedCellsDistances[currentNode.Position] = currentNode.Distance;
                }
                
                foreach (var move in piecePossibleMoves)
                {
                    var nextNodePosition = currentNode.Position + move; 
                    var distance = currentNode.Direction == move || currentNode.Direction == default(Vector2Int) ? currentNode.Distance  : currentNode.Distance + 1; 
                    if (nextNodePosition == from && shortestWay.Distance > distance)
                        shortestWay = new CellNode(currentNode, nextNodePosition, move, distance);
                    if(!IsValid(nextNodePosition,grid)) continue;
                    searchQueue.Enqueue(new CellNode(currentNode,nextNodePosition,move,distance));
                }
            }
            return GetPositions(shortestWay,unit);
        }
        private bool IsValid(Vector2Int position,ChessGrid grid) 
        {
            var xWithinBorders = position.x >= 0 && position.x < grid.Size.x;
            var yWithinBorders = position.y >= 0 && position.y < grid.Size.y;
            return xWithinBorders && yWithinBorders && grid.Get(position) == null;
        }
        private List<Vector2Int> GetPositions(CellNode node,ChessUnitType unit) 
        {
            if (node.Distance == int.MaxValue) return null;
            var pos = new List<Vector2Int>(){};
            while (node.PreviousCellNode != null)
            {
                if(IsNewTurn(node,unit)) pos.Add(node.PreviousCellNode.Position);
                node = node.PreviousCellNode;
            }
            return pos;
        }
        private bool IsNewTurn(CellNode node, ChessUnitType unit) 
        {
            return unit is ChessUnitType.Pon or ChessUnitType.Knight or ChessUnitType.King || node.PreviousCellNode.Direction != node.Direction;
        }
    }
}