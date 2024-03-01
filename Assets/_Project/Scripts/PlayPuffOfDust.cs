using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayPuffOfDust : MonoBehaviour
{
    private bool _destory;
    
    void Awake()
    {
        _destory = false; 
        this.GetComponent<VisualEffect>().Play();
        StartCoroutine(DestroyOverTime());
    }

    private IEnumerator DestroyOverTime()
    {
        yield return new WaitForSeconds(2f);
        _destory = true;
    }

    private void LateUpdate()
    {
        if (_destory) Destroy(this.gameObject);
    }
}
