using System.Collections;
using UnityEngine;

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
            Debug.Log("Updating Zones");

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
                    //floor(dist(topLeftPoint.position.x, _xPos)/(lengthOfX/3))
                    int _xIndex = Mathf.FloorToInt(Mathf.Abs(_xPos - topLeftPoint.position.x) / (sizeX / 3));
                    int _zIndex = Mathf.FloorToInt(Mathf.Abs(topLeftPoint.position.z - _zPos) / (sizeZ / 3));
                    //Debug.Log("Occipied index: " + _xIndex + "," + _zIndex);
                    spaces[Mathf.Clamp(_xIndex, 0, 2), Mathf.Clamp(_zIndex, 0, 2)] = true;
                }
            }

            yield return new WaitForSeconds(0.39f);

        } while (true);
        
    }

    public Transform getPointOnFreeSpace(Transform _newMovement, Bounds _bounds)
    {

        return null;
    }
}
