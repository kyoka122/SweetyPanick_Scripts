using UnityEngine;
using Utility;

public class PopTextGeneratorView:MonoBehaviour,IPoolObjectGenerator<PopText>
{
    [SerializeField] private PopText popText;
    
    public PopText Generate()
    {
        return Instantiate(popText);
    }
}