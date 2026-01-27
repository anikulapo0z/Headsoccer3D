using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum HorizontalSpace
{
    Defensive = 0,
    Midfield = 1,
    Attacking = 2
};

[System.Serializable]
public enum VerticalSpace
{
    Left = 0,
    Central = 1,
    Right = 2
};

public class Raumdeuter : MonoBehaviour
{
    public Transform topLeftPoint;
    public Transform bottomLeftPoint;
    public Transform topRightPoint;
    public Transform bottomRightPoint;
    
    private bool[,] spaces;
    /* SPACES EXPLAINED
     * 
     * Direction of Attack -------------------> 
     * left Defence       Left Mid         Left Attk
     * center Defence     center Mid       center Attk
     * right Defence      right Mid        right Attk
     * 
     * TRUE: FREE UNOCCUPIED, FALSE: OCCCUPIED
     */

    public Transform[] charactersToLookFor;

    //size of field
    private float sizeX, sizeZ;
    void Start()
    {
        spaces = new bool[,]{{ false, false, false}, 
                             { false, false, false }, 
                             { false, false, false } };
        /* SPACES EXPLAINED
         * 
         * left Defence       Left Mid         Left Attk
         * center Defence     center Mid       center Attk
         * right Defence      right Mid        right Attk
         * 
         * TRUE: FREE UNOCCUPIED, FALSE: OCCCUPIED
         */

        sizeX = (topRightPoint.position.x - topLeftPoint.position.x);
        sizeZ = (topLeftPoint.position.z - bottomLeftPoint.position.z);
        StartCoroutine(updateSpacesEveryFewFrames());
    }

    private void OnDrawGizmos()
    {
        Vector3 _zoneSize = new Vector3(sizeX / 3, 0.493f, sizeZ / 3);

        for (int _row = 0; _row < 3; _row++)
        {
            for (int _col = 0; _col < 3; _col++)
            {
                bool _isFree = spaces[_row, _col];
                Gizmos.color = _isFree ? Color.red : Color.green;

                Vector3 _cellCenter = new Vector3(topLeftPoint.position.x + (sizeX * (1 + (_row * 2)) / 6), 
                                                    0.493f,
                                                    topLeftPoint.position.z - (sizeZ * (1 + (_col*2)) / 6));

                Gizmos.DrawWireCube(_cellCenter, _zoneSize * 0.948f);
            }
        }
    }

    IEnumerator updateSpacesEveryFewFrames()
    {
        do
        {
            //Debug.Log("Updating Zones");

            spaces = new bool[,]{{ false, false, false},
                             { false, false, false },
                             { false, false, false } };

            for (int i = 0; i < charactersToLookFor.Length; i++)
            {
                float _xPos = charactersToLookFor[i].position.x;
                float _zPos = charactersToLookFor[i].position.z;

                //if within Bounds
                if (_xPos < topRightPoint.position.x && _xPos > topLeftPoint.position.x
                    &&
                    _zPos > bottomLeftPoint.position.z && _zPos < topLeftPoint.position.z)
                {
                    Vector2 _onGrid = convertToSpaceGrid(_xPos, _zPos);
                    spaces[Mathf.Clamp((int)_onGrid.x, 0, 2), Mathf.Clamp((int)_onGrid.y, 0, 2)] = true;
                }
            }

            yield return new WaitForSeconds(0.39f);

        } while (true);
        
    }

    public bool isGridSPaceOccupied(int _x, int _z)
    {
        return spaces[_x,_z];
    }

    public Vector2 convertToSpaceGrid(float _xPos, float _zPos)
    {
        //floor(dist(topLeftPoint.position.x, _xPos)/(lengthOfX/3))
        int _xIndex = Mathf.FloorToInt(Mathf.Abs(_xPos - topLeftPoint.position.x) / (sizeX / 3));
        int _zIndex = Mathf.FloorToInt(Mathf.Abs(topLeftPoint.position.z - _zPos) / (sizeZ / 3));

        return new Vector2(_xIndex, _zIndex);
    }

    public Vector3 getPointOnFreeSpace(Transform _newMovement)
    {
        int _freeSpaceIndex = GetRandomClosestFreeSpace(_newMovement);

        float _xIndex = Mathf.Floor(_freeSpaceIndex/10);
        float _zIndex = _freeSpaceIndex - _xIndex;

        float _randX = ((Random.Range(_xIndex, _xIndex + 1))/3) * sizeX;
        float _randZ = ((Random.Range(_zIndex, _zIndex + 1))/3) * sizeX;

        return new Vector3(_randX, _newMovement.position.y, _randZ);
    }

    private int GetRandomClosestFreeSpace(Transform _point)
    {
        Vector2 _onGrid = convertToSpaceGrid(_point.position.x, _point.position.z);
        int _x = (int)_onGrid.x;
        int _z = (int)_onGrid.y;
        List<int> _possibleSpaces = new List<int>();

        for (int i = -1; i <= 1; i++)
        {
            //left, center and right of the current grid pos
            _x = Mathf.Clamp((int)_onGrid.x - i, 0, 2);
            for (int j = -1; j <= 1; j++)
            {
                //up, center, amd bottom of the current grid space
                _z = Mathf.Clamp((int)_onGrid.x - i, 0, 2);
                //only for the spaces not itself is in
                if (!(i == 0  && j == 0))
                {
                    //if that is a free space
                    if (spaces[i,j] == false)
                    {
                        _possibleSpaces.Add((i * 10) + j);
                    }
                }
            }
        }

        if(_possibleSpaces.Count == 0)
        {
            //no free space, stay put
            return (_x * 10) + _z;
        }
        else
        {
            int _rand = Random.Range(0, _possibleSpaces.Count);
            return _possibleSpaces[_rand];
        }

    }
}
