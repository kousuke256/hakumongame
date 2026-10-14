using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    public bool OnOff;
    public Player PlayerScript;
    private float oldGravityDirectionY = -1f;
    public GameObject laser;
    public float laserLength = 5f;
    

    void Start()
    {


        // 初期状態を設定する
        if (OnOff)
        {
            laser.SetActive(true);

        }
        else
        {
            laser.SetActive(false);

        }
    }


    void Update()
    {
        Vector3 scale = laser.transform.localScale;
        scale.y = laserLength;
        laser.transform.localScale = scale;
        if (oldGravityDirectionY == PlayerScript.gravityDirection.y)
            return;

        oldGravityDirectionY = PlayerScript.gravityDirection.y;

        if (oldGravityDirectionY == -1)
        {
            // 下向きの重力に切り替えたら
            if (OnOff)
            {
                laser.SetActive(true);

            }
            else
            {
                laser.SetActive(false);

            }
        }
        else
        {
            // 上向きの重力に切り替えたら
            if (OnOff)
            {
                laser.SetActive(false);

            }
            else
            {
                laser.SetActive(true);

            }
        }
    }
}
