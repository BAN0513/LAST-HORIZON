using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour
{
    [SerializeField] private Transform parentObj;
    private TitleUIManager t;

    public enum SlideerType
    {
        SE,
        BGM,
        Light
    }

    Dictionary<SlideerType, Action> sliderDict = new Dictionary<SlideerType, Action>();

    private void Start()
    {
        t = TitleUIManager.Instance;

        sliderDict[SlideerType.SE]    = () => t.SystemChage(t.sliderSE, t.textSE, t.SEChange);
        sliderDict[SlideerType.BGM]   = () => t.SystemChage(t.sliderBGM, t.textBGM, t.BGMChange);
        sliderDict[SlideerType.Light] = () => t.SystemChage(t.sliderLight, t.textLight, t.LightChange);

        foreach (var type in parentObj.GetComponentsInChildren<SliderTypeSet>())
        {
            var id = type.type;

            type.GetComponent<Slider>().onValueChanged.AddListener((float v) => SliderChange(id));
        }
    }

    public void SliderChange(SlideerType type)
    {
        if (sliderDict.TryGetValue(type, out Action value))
        {
            value.Invoke();
        }
    }
}
