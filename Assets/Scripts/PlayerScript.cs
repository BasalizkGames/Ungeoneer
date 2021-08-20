using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 movementVec;

    

    public GameObject Bullet;
    public Camera Cam;

    public float speed = 15.0f;
    public int shotDamage = 5;
    public int health = 15;

    bool invincible = false;

    // Start is called before the first frame update
    void Start()
    {
        Cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movementVec.x = Input.GetAxisRaw("Horizontal");
        movementVec.y = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 mousePos = Cam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

            GameObject bullet = Instantiate(Bullet, new Vector3(transform.position.x, transform.position.y, -1.0f), Quaternion.identity);
            bullet.GetComponent<BulletScript>().damage = shotDamage;
        }
    }

    private void FixedUpdate()
    {
        rb.transform.position = (rb.position + movementVec * speed * Time.fixedDeltaTime);
        if(health <= 0)
        {
            WorldScript.Instance.showGameOver();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody.tag.ToString() == "Door" && WorldScript.Instance.Enemies.Count <= 0)
        {
            float tempSpeed = speed;
            speed = 0.0f;
            var side = NormalToSide(collision.contacts[0].normal);
            switch (side)
            {
                case Side.Left:
                    WorldScript.Instance.roomTransition("right");
                    rb.transform.position = new Vector3(transform.position.x + 5, transform.position.y, -1.0f);
                    break;
                case Side.Right:
                    WorldScript.Instance.roomTransition("left");
                    rb.transform.position = new Vector3(transform.position.x - 5, transform.position.y, -1.0f);
                    break;
                case Side.Top:
                    WorldScript.Instance.roomTransition("down");
                    rb.transform.position = new Vector3(transform.position.x, transform.position.y - 5, -1.0f);
                    break;
                case Side.Bottom:
                    WorldScript.Instance.roomTransition("up");
                    rb.transform.position = new Vector3(transform.position.x, transform.position.y + 5, -1.0f);
                    break;
            }
        }

        if (collision.rigidbody.tag.ToString() == "Chest")
        {
            collision.rigidbody.tag = "OpenChest";
            collision.gameObject.GetComponent<SpriteRenderer>().sprite = WorldScript.Instance.ChestOpen[0];
            collision.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = WorldScript.Instance.ChestLoot[Random.Range(0,WorldScript.Instance.ChestLoot.Count)];
            switch (Random.Range(0, 3))
            {
                case(0):
                    health += 5;
                    break;
                case (1):
                    speed += 2.5f;
                    break;
                case (2):
                    shotDamage += 2;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.rigidbody.tag.ToString() == "Enemy" && invincible == false)
        {
            Debug.Log("enemy hit");
            invincible = true;
            health -= collision.gameObject.GetComponent<EnemyScript>().damage;
            //visual/audio feedback
            StartCoroutine(invincibility());
        }
    }

    IEnumerator invincibility()
    {
        yield return new WaitForSeconds(2);
        invincible = false;
        Debug.Log("you can now take damage");

    }

    enum Side
    {
        Top, Right, Bottom, Left
    }
    // ...
    static Side NormalToSide(Vector2 normal)
    {

        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
        {
            return normal.x > 0 ? Side.Right : Side.Left;
        }
        return normal.y > 0 ? Side.Top : Side.Bottom;
    }
}


