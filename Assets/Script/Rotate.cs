using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("±ÛÂà³t«×")]
    public Vector3 speed = new Vector3(0f, 200f, 0f);

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime);
    }
}