using UnityEngine;
using System.Collections.Generic;


public class FreedomMapPedestrians : MonoBehaviour
{
    public Transform startZone, endZone;
    public float walkSpeed = 0.2f;
    public float runSpeed = 0.5f;

    private float speed = 0f;
    List<Material> pedestrianMats;
    Transform[] pedestrians;
    
    void Start()
    {
        pedestrianMats = new List<Material>();
        pedestrians = new Transform[transform.childCount];

        Material _temp;

        //set their random color
        for (int i = 0; i < transform.childCount; i++)
        {
            pedestrians[i] = transform.GetChild(i);
            _temp = pedestrians[i].GetComponent<Renderer>().material;
            pedestrianMats.Add(_temp);
            _temp.SetFloat("_ID_from_Script", i);
        }

        SetAnimation(2);

    }

    //you are welcome
    private void FixedUpdate()
    {
        for (int i = 0; i < pedestrians.Length; i++)
        {
            pedestrians[i].transform.position += pedestrians[i].transform.forward * Time.deltaTime * speed;

            if (pedestrians[i].transform.localPosition.z < -13.5f)
                pedestrians[i].transform.localPosition = new Vector3(pedestrians[i].transform.localPosition.x,
                                                                        pedestrians[i].transform.localPosition.y,
                                                                        65);

            if (pedestrians[i].transform.localPosition.z > 65.5f)
                pedestrians[i].transform.localPosition = new Vector3(pedestrians[i].transform.localPosition.x,
                                                                        pedestrians[i].transform.localPosition.y,
                                                                        -13);
        }
    }

    /// <summary>
    /// 0 = run (bell broken)
    /// 1 = panic (bell just broke)
    /// 2 = walk (normal)
    /// </summary>
    /// <param name="_index"></param>
    public void SetAnimation(int _index)
    {
        _index = Mathf.Clamp(_index, 0, 2);

        for (int i = 0; i < pedestrianMats.Count; i++)
        {
            pedestrianMats[i].SetFloat("_Animation_Index", _index + 0.5f);
            pedestrianMats[i].SetFloat("_speed", (1 -_index) * (1 - _index < 0 ? 80 : 180));
        }

        switch (_index)
        {
            case 0:
                speed = runSpeed;
                break;
            case 1:
                speed = 0;
                break;
            case 2:
                speed = walkSpeed;
                break; 
            default:
                speed = 0;
                break;
        }
    }
}
