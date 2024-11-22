using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RainbowImage : MonoBehaviour
{
    Image origin;

    const float speed = 2.0f;
    const float saturation = 0.5f;
    const float value = 1.0f;
    private void Awake()
    {
        origin = GetComponent<Image>();
    }
    float counter = 0.0f;
    private void Update()
    {
        counter += Time.deltaTime * speed;
        if (counter > 1.0f) counter -= 1.0f;
        origin.color = Color.HSVToRGB(counter, saturation, value);
    }
}
