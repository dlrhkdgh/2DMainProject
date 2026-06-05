using System.Collections;
using UnityEngine;

public class BombEffect : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(BombEffectCo());
    }
    private IEnumerator BombEffectCo()
    {
        yield return new WaitForSeconds(1f);
       gameObject.SetActive(false);
    }
}
