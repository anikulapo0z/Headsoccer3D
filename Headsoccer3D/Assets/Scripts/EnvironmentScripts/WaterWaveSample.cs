using UnityEngine;

public class WaterWaveSample : MonoBehaviour
{
    public float waveAmplitude = 1.0f;

    public float waveFrequency = 1.0f;

    private float actualWaveFrequency;
    [SerializeField] private float timer;

    [SerializeField] private Material waterMaterial;

    private void Start()
    {
        //timer = 0;
        actualWaveFrequency = waveFrequency;

        waterMaterial.SetFloat("_actualWaveFrequency", actualWaveFrequency);
        waterMaterial.SetFloat("_waveAmplitude", waveAmplitude / 4.79258f);
    }

    private void Update()
    {
        //timer += Time.deltaTime;
        waterMaterial.SetFloat("_Timer", timer);

    }

    //here will be the math for reference later
    //https://graphtoy.com/?f1(x,t)=(t*%F0%9D%9C%8B)&v1=false&f2(x,t)=abs(sin(f1(x,t)))&v2=false&f3(x,t)=fract(f1(x,t)/(3*%F0%9D%9C%8B))&v3=false&f4(x,t)=step(f3(x,t),(1/3))&v4=false&f5(x,t)=f2(x,t)*f4(x,t)&v5=true&f6(x,t)=&v6=false&grid=1&coords=1.208212361384535,0.502985503002449,3.04779094467458**
    public Vector2 getWaveOffsetAndRotation(float _positionZ)
    {
        Vector2 _OffsetRotation = new Vector2(0.0f,0.0f);

        float _in = _positionZ + timer;
        float _theta = _in * Mathf.PI;

        float _period = fract(_theta / (actualWaveFrequency * Mathf.PI));
        int _steppedPeriod = (_period < (1 / actualWaveFrequency)) ? 1 : 0;

        //no need for further calculation if we are returning 0
        if(_steppedPeriod == 0)
            return new Vector2(0,0);

        //now we calc the wave
        float _sineWave = Mathf.Abs(Mathf.Sin(_theta));
        //for the rotation, we know it goes from flat, turn left, to flat, turn right, to flat. 
        //it is tempting to use cos as its the derivative, but if you math from cos 
        //you'll realize that the angle is just the double speed sin without absolute values
        float _sineWaveDoubleSpeed  = Mathf.Sin(_theta * 2);

        
        _OffsetRotation.x = _sineWave * waveAmplitude;
        _OffsetRotation.y = _sineWaveDoubleSpeed;
        return _OffsetRotation;
    }

    private float fract(float _in)
    {
        _in = Mathf.Abs(_in);
        return _in - Mathf.Floor(_in);
    }
}
