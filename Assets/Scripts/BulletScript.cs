using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer % 60 > 1f)//delete self after 1 second
        { Destroy(gameObject); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag.Contains("bullet") == true)
        {
            Destroy(gameObject);
        }
    }
}
