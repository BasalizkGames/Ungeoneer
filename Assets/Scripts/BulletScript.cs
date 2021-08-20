using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    Vector3 targetDestination;
    public Camera Cam;
    float step;
    Vector3 direction;
    
    public float speed = 2.5f;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        step = speed * Time.fixedDeltaTime;
        Cam = Camera.main;
        targetDestination = Cam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        targetDestination.z = -1.0f;
        direction = (targetDestination - transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (WorldScript.Instance.gameRunning)
        {
            transform.position += direction * speed * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.ToString() == "Enemy")
        {
            //Do damage to enemy
            other.GetComponent<EnemyScript>().health -= damage;
            Destroy(this.gameObject);
        }
        else if (other.tag.ToString() == "Wall" || other.tag.ToString() == "Door")
        {
            Destroy(this.gameObject);
        }
    }

}
