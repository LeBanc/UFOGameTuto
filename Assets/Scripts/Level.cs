using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public int width;
    public int height;

    public List<Position> blocksPositions;
    public List<Position> pickUpPositions;
    public Position[] ufoPositions = new Position[4];

    private void Awake()
    {
        Position pos = new Position();
        pos.xPos = 0;
        pos.yPos = 0;
        ufoPositions[0] = pos;
        ufoPositions[1] = pos;
        ufoPositions[2] = pos;
        ufoPositions[3] = pos;
    }

    public void DefineWidth(int w)
    {
        width = w;

        if (blocksPositions.Count > 0)
        {

            for (int i = 0; i < blocksPositions.Count; i++)
            {
                if (blocksPositions[i].xPos > width || blocksPositions[i].xPos < -width)
                {
                    RemoveBlock(blocksPositions[i]);
                    i--;
                }
            }
        }
        if (pickUpPositions.Count > 0)
        {
            for (int i = 0; i < pickUpPositions.Count; i++)
            {
                if (pickUpPositions[i].xPos > width || pickUpPositions[i].xPos < -width)
                {
                    RemovePickup(pickUpPositions[i]);
                    i--;
                }
            }
        }
        for (int i = 0; i < ufoPositions.Length; i++)
        {
            if (ufoPositions[i].xPos > width || ufoPositions[i].xPos < -width) ufoPositions[i] = new Position();
        }
    }

    public void DefineHeigth(int h)
    {
        height = h;

        if (blocksPositions.Count > 0)
        {
            for (int i = 0; i < blocksPositions.Count; i++)
            {
                if (blocksPositions[i].yPos > height || blocksPositions[i].yPos < -height)
                {
                    RemoveBlock(blocksPositions[i]);
                    i--;
                }
            }
        }
        if (pickUpPositions.Count > 0)
        {
            for (int i = 0; i < pickUpPositions.Count; i++)
            {
                if (pickUpPositions[i].yPos > height || pickUpPositions[i].yPos < -height)
                {
                    RemovePickup(pickUpPositions[i]);
                    i--;
                }
            }
        }
        for (int i = 0; i < ufoPositions.Length; i++)
        {
            if (ufoPositions[i].yPos > height || ufoPositions[i].yPos < -height) ufoPositions[i] = new Position();
        }
    }

    public void AddBlock(Position pos)
    {
        blocksPositions.Add(pos);
    }

    public void AddPickup(Position pos)
    {
        pickUpPositions.Add(pos);
    }

    public void SetUFOPosition(int i,Position pos)
    {
        if(i<4 && i>=0) ufoPositions[i] = pos;
    }

    public void RemoveBlock(Position pos)
    {
        // List is object-oriented so it has to find the exact object to remove it and not just an object with the same values
        Position positionToRemove = null;
        foreach (Position p in blocksPositions)
        {
            if (p.xPos == pos.xPos)
            {
                if (p.yPos == pos.yPos)
                {
                    positionToRemove = p;
                    break;
                }
            }
        }
        // if found, remove it
        if (positionToRemove != null)
        {
            blocksPositions.Remove(positionToRemove);
        }
    }

    public void RemovePickup(Position pos)
    {
        // List is object-oriented so it has to find the exact object to remove it and not just an object with the same values
        Position positionToRemove = null;
        foreach (Position p in pickUpPositions)
        {
            if (p.xPos == pos.xPos)
            {
                if(p.yPos == pos.yPos)
                {
                    positionToRemove = p;
                    break;
                }
            }
        }
        // if found, remove it
        if (positionToRemove != null)
        {
            pickUpPositions.Remove(positionToRemove);
        }
    }
}
