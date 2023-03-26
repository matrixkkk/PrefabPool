using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SampleScene : MonoBehaviour
{
    public AssetReference bulletAsset;
    public Text noticeText;

    private PrefabPool prefabPool = new PrefabPool();
    private List<Bullet> activeBullet = new List<Bullet>();

    // Start is called before the first frame update
    void Start()
    {
        prefabPool.Root = transform;
        noticeText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            OnClickBulletButton();
        }
    }

    public void OnClickBulletButton()
    {
        prefabPool.Pop<Bullet>(bulletAsset.AssetGUID, 
            (bullet)=>
            {
                var camera = Camera.main;
                bullet.Fire(camera.transform.position, camera.transform.forward);   
                bullet.OnEnd = this.OnEndBullet; 
                activeBullet.Add(bullet);
                SetNotiveText();
            });
    } 

    private void SetNotiveText()
    {
        noticeText.text = "Pool : " + prefabPool.GetCount(bulletAsset.AssetGUID) +
            " Active : " + activeBullet.Count; 
    }   


    private void OnEndBullet(Bullet bullet)
    {
        activeBullet.Remove(bullet);
        prefabPool.Push(bulletAsset.AssetGUID, bullet.gameObject);
        SetNotiveText();
    }
}
