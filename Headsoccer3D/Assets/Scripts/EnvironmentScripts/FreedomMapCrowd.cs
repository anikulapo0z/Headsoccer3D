using UnityEngine;
using UnityEngine.Splines;

public class FreedomMapCrowd : MonoBehaviour
{
    public float runSpeed = 0.5f;
    public Transform bell;

    Material[] crowdMats;
    Transform[] crowdPivot;
    Transform[] crowdActualObj;
    

    void Start()
    {
        crowdMats = new Material[transform.childCount];
        crowdPivot = new Transform[transform.childCount];
        crowdActualObj = new Transform[transform.childCount];

        Material _temp;

        //set their random color
        for (int i = 0; i < transform.childCount; i++)
        {
            crowdPivot[i] = transform.GetChild(i);
            crowdActualObj[i] = crowdPivot[i].GetChild(0); //the actual is the child of the pivot
            _temp = crowdActualObj[i].GetComponent<Renderer>().material;
            crowdMats[i] = _temp;
            _temp.SetFloat("_ID_from_Script", i);
        }

        SetAnimation(2);

    }


    /// <summary>
    /// 0 = panic run (bell broken)
    /// 1 = shock (bell just broke)
    /// 2 = idle 1 (normal) 
    /// 3 = idle 2
    /// </summary>
    /// <param name="_index"></param>
    public void SetAnimation(int _index)
    {
        _index = Mathf.Clamp(_index, 0, 2);

        //if index is 2, random bertween 2 and 3
        _index = (_index == 2 ? (Random.value < 0.5f ? 2 : 3) : _index);

        for (int i = 0; i < crowdMats.Length; i++)
        {
            //set mate data
            crowdMats[i].SetFloat("_Animation_Index", _index + 0.5f);
            crowdMats[i].SetFloat("_speed", _index == 0 ? 481 : 50); //fast speed if panic, other wise slow

            //if in shock, face the bell
            if (_index == 1)
                crowdActualObj[i].LookAt(bell, Vector3.up);
            else
                crowdActualObj[i].localEulerAngles = Vector3.zero;

            //and set the speed along the spline // some random to add variatoin
            crowdPivot[i].GetComponent<SplineAnimate>().MaxSpeed = _index == 0 ? Random.Range(0.98f, 1.02f) * runSpeed : 0.001f; 
        }

    }
}
