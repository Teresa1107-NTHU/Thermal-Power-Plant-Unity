using System.Collections;
using UnityEngine;

public class MoveAlongPoints : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;
    public float startDelay = 0f;
    public bool loop = true;
    public float minScale = 0.04f;
    public float maxScale = 0.07f;
    public float flickerSpeed = 20f;

    private int currentIndex = 0;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);

        transform.position = points[0].position;
        currentIndex = 1;
    }

    void Update()
    {
        if (points.Length < 2) return;

        Transform target = points[currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            currentIndex++;

            if (currentIndex >= points.Length)
            {
                transform.position = points[0].position;
                currentIndex = 1;
            }
        }
        float s = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * flickerSpeed) + 1f) * 0.5f);
        transform.localScale = new Vector3(s, s, s);
    }
}