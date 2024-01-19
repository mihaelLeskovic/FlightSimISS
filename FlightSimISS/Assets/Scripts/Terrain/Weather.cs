using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class Weather : MonoBehaviour
{
    [SerializeField] VolumeProfile volumeProfile;
    [SerializeField] CubemapParameter skyDay;
    [SerializeField] CubemapParameter skyNight;
    [SerializeField] Light lightSource;
    Volume volume;

    bool isNight;

    void Start()
    {
        volume = FindObjectOfType<Volume>();
        volume.profile = volumeProfile;
        volumeProfile.TryGet<HDRISky>(out var sky);
        sky.hdriSky.SetValue(skyDay);

        volumeProfile.TryGet<VolumetricClouds>(out var clouds);
        clouds.active = true;

        volumeProfile.TryGet<Fog>(out var fog);
        fog.active = false;
        fog.meanFreePath.SetValue(new FloatParameter(300f));

        setDay();
    }

    public void ToggleClouds()
    {
        volumeProfile.TryGet<VolumetricClouds>(out var clouds);
        clouds.active = !clouds.active;
    }

    public void ToggleDayTime()
    {
        if (isNight)
        {
            setDay();
        }
        else
        {
            setNight();
        }
        isNight = !isNight;
    }

    void setDay()
    {
        volumeProfile.TryGet<HDRISky>(out var sky);
        sky.hdriSky.SetValue(skyDay);
        sky.exposure.SetValue(new FloatParameter(15f));
        lightSource.intensity = 100000;

    }

    void setNight()
    {
        volumeProfile.TryGet<HDRISky>(out var sky);
        sky.hdriSky.SetValue(skyNight);
        sky.exposure.SetValue(new FloatParameter(3f));
        lightSource.intensity = 0;
    }

    public void ToggleFog()
    {
        volumeProfile.TryGet<Fog>(out var fog);
        fog.active = !fog.active;
    }

    public void ChangeFogDensity(Single fogDensity)
    {
        volumeProfile.TryGet<Fog>(out var fog);
        fog.meanFreePath.SetValue(new FloatParameter(300f - fogDensity));
    }
}
