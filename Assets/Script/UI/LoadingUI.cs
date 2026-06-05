using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingUI : UIBase
{
    //[SerializeField] private GameObject _loadingCanvas; 
    [SerializeField] private Slider _loadingSlider;       
    private readonly float _targetLoadingTime = 1.0f;
    private void OnEnable()
    {
        ChangeScene();
    }
    public void ChangeScene()
    {
        StartCoroutine(FixedLoadingRoutine());
    }

    private IEnumerator FixedLoadingRoutine()
    {
        _loadingSlider.value = 0f;
        gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < _targetLoadingTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / _targetLoadingTime);
            _loadingSlider.value = progress;
            yield return null;
        }
        _loadingSlider.value = 1f;
        UIManager.Inst.CloseLoadingUI();
    }
}
