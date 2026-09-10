using Unity.Mathematics;
using UnityEngine;

public class EnemySponer : MonoBehaviour
{
    private int i = 0;
    private float countUp = 0f; 
    public GameObject enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        countUp += Time.deltaTime;
        if(countUp >= 1f)
        {
            countUp -= 1f;
            i++;
        }
        if(i == 3)
        {
            i = 0;
            Instantiate(enemy, transform.position, quaternion.identity);
        }
    }
}
