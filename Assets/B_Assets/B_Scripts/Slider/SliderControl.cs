using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour
{
    [SerializeField] private Transform parentObj;

    public enum SlideerType
    {
        SE,
        BGM,
        Light
    }

    Dictionary<SlideerType, Action> sliderDict = new Dictionary<SlideerType, Action>();

    private void Start()
    {
        sliderDict[SlideerType.SE] = () => TitleUIManager.Instance.SEChange();
        sliderDict[SlideerType.BGM] = () => TitleUIManager.Instance.BGMChange();
        sliderDict[SlideerType.Light] = () => TitleUIManager.Instance.LightChange();

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
