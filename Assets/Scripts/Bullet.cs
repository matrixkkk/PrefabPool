using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 1f;

    public delegate void OnEndCallback(Bullet bullet);
    public OnEndCallback OnEnd { get; set;}

    private bool isFire = false;
     
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(isFire)
            transform.position += transform.forward * 20f * Time.deltaTime;
    }

    public void Fire(Vector3 pos, Vector3 dir)
    {
        transform.position = pos;
        transform.forward = dir;
        gameObject.SetActive(true);
        isFire = true;
        StartCoroutine(DelayEnd());
    }

    IEnumerator DelayEnd()
    {
        yield return new WaitForSeconds(lifeTime);
        OnEnd?.Invoke(this);
        isFire = false;
    }
}
