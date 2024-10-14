using Cinemachine;
using System.Collections;
using UnityEngine;

public class ScreenShakeHandler : MonoBehaviour
{
    private CinemachineVirtualCamera dynamicCamera;
    public bool shakeEnabled = true;
    public CinemachineVirtualCamera DynamicCamera
    {
        get { return dynamicCamera; }
        set { dynamicCamera = value; }
    }
    public void AttackCameraEffect(float percentageMax)
    {
        if (!shakeEnabled) return;
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

    private IEnumerator ShakeCamera(float percentageMax)
    {
        float time = Mathf.Min(0.3f, percentageMax);
        float shakeAmplitude = 1f + percentageMax / 2f;
        float shakeFrequency = 1f + percentageMax;
        Debug.Log("My shaking intensity is" + shakeFrequency);
        Debug.Log("My shaking amplitude is" + shakeAmplitude);
        CinemachineBasicMultiChannelPerlin virtualCameraNoise = DynamicCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        virtualCameraNoise.m_AmplitudeGain = shakeAmplitude;
        virtualCameraNoise.m_FrequencyGain = shakeFrequency;
        yield return new WaitForSeconds(time + percentageMax / 10f);
        virtualCameraNoise.m_AmplitudeGain = 0f;
        virtualCameraNoise.m_FrequencyGain = 0f;
    }

    private IEnumerator ZoomEffect(float zoomIn)
    {
        Debug.Log("My zoom in intensity is" + zoomIn);
        float startFOV = DynamicCamera.m_Lens.OrthographicSize;
        float endFOV = Mathf.Max(2.80f, startFOV - zoomIn);
        Debug.Log("My startFOV in intensity is" + startFOV);
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
