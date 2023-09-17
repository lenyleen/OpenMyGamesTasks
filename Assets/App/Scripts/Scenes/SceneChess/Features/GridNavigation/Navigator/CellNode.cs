using System;
using System.Collections.Generic;
using UnityEngine;
namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
  public class CellNode
  {
    public  Vector2Int Position { get; private set; }
    public  CellNode PreviousCellNode { get; private set; }
    public  Vector2Int Direction { get; private set; }
    public  int Distance { get; set; }
    public CellNode(CellNode previousCellNode, Vector2Int position, Vector2Int direction, int distance)
    {
      Position = position;
      PreviousCellNode = previousCellNode;
      Direction = direction;
      Distance = distance;
    }
  }
}
