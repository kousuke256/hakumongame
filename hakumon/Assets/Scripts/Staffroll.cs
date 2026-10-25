using UnityEngine;

public class Staffroll : MonoBehaviour
{
    public float speed = 2f;

    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }
}
