using UnityEngine;

public class WaterWaveSample : MonoBehaviour
{
    public float waveAmplitude = 1.0f; 
    public float waveFrequency = 1.0f; 

    public Vector2 getWaveOffsetAndRotation(Vector3 _position)
    {
        Vector2 _OffsetRotation = new Vector2(0.0f,0.0f);

        float _theta = (_position.z + 100 + Time.timeSinceLevelLoad) * waveFrequency;

        _OffsetRotation.x = (Mathf.Sin(_theta) + 1.0f /2.0f) * waveAmplitude;

        //for the rotation, we know it goes from flat, turn left, to flat, turn right, to flat. 
        //so when we enter the wave, the inital degree of change is increasing, hits 0 at crest, and then change is negative, and 0 at valley and repeat
        //and that change of sine wave, i.e. its derivative, is cosine wave at the same angle at 0 phase difference
        float _rateOfChange = Mathf.Cos(_theta);
        _OffsetRotation.y = 1.0f - _rateOfChange;

        _OffsetRotation.y = (Mathf.Sin(_theta * 2) + 1.0f / 2.0f) * waveAmplitude;
        return _OffsetRotation;
    }
}
