using Cinemachine;
using System.Collections;
using Systems.Persistence;
using UnityEngine;

public class ScreenShakeHandler : MonoBehaviour
{
    public static bool IsScreenShakeEnabled
    {
        get => SaveLoadSystem.Instance.GetUserPreferences().screenShakePreference.IsScreenShakeEnabled;
        set => SaveLoadSystem.Instance.GetUserPreferences().screenShakePreference.IsScreenShakeEnabled = value;
    }

    private CinemachineVirtualCamera dynamicCamera;

    public CinemachineVirtualCamera DynamicCamera
    {
        get { return dynamicCamera; }
        set { dynamicCamera = value; }
    }
    public void AttackCameraEffect(float percentageMax)
    {
        if (!IsScreenShakeEnabled) return;
        if (percentageMax < 0.25f)
        {
            StartCoroutine(ShakeCamera(percentageMax));
            StartCoroutine(ZoomEffect(-percentageMax));
        }
        else if (percentageMax < 0.75f)
        {
            StartCoroutine(ShakeCamera(percentageMax));
            StartCoroutine(ZoomEffect(-percentageMax));
        }
        else
        {
            StartCoroutine(ShakeCamera(percentageMax));
            StartCoroutine(ZoomEffect(-percentageMax));
            StartCoroutine(TiltEffect(percentageMax));
        }
    }

    //(@param percentageMax) is a float [0, 1]
    private IEnumerator ShakeCamera(float percentageMax)
    {
        float time = Mathf.Min(0.3f, percentageMax);
        float shakeAmplitude = 1f + percentageMax / 2f;
        float shakeFrequency = 1f + percentageMax * 3f / 4f;
        CinemachineBasicMultiChannelPerlin virtualCameraNoise = DynamicCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        virtualCameraNoise.m_AmplitudeGain = shakeAmplitude;
        virtualCameraNoise.m_FrequencyGain = shakeFrequency;
        yield return new WaitForSeconds(time + percentageMax / 20f);
        virtualCameraNoise.m_AmplitudeGain = 0f;
        virtualCameraNoise.m_FrequencyGain = 0f;
    }

    //(@param percentageMax) is a float [0, 1]
    private IEnumerator ZoomEffect(float percentageMax)
    {
        float startFOV = DynamicCamera.m_Lens.OrthographicSize;
        float endFOV = startFOV - percentageMax;
        yield return StartCoroutine(ZoomCamera(startFOV, endFOV));
        yield return new WaitForSeconds(0.4f);
        yield return StartCoroutine(ZoomCamera(endFOV, startFOV));
    }

    private IEnumerator ZoomCamera(float startFOV, float endFOV)
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            DynamicCamera.m_Lens.OrthographicSize = Mathf.Lerp(startFOV, endFOV, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        DynamicCamera.m_Lens.OrthographicSize = endFOV;
    }

    private IEnumerator TiltEffect(float tilt)
    {
        yield break;
    }
}

[System.Serializable]
public class ScreenShakePreference : ISaveable
{
    public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [field: SerializeField] public bool IsScreenShakeEnabled { get; set; } = true;
    
}