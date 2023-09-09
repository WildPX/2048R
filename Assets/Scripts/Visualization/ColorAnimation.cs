using UnityEngine;
using UnityEngine.UI;

public class ColorAnimation : MonoBehaviour
{
    public float duration = 1f; // Продолжительность анимации в секундах
    public Color initialColor; // Начальный цвет
    public Color targetColor; // Целевой цвет

    private float timer;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        // Увеличить таймер
        timer += Time.deltaTime;

        // Интерполировать текущий цвет между начальным и целевым цветом
        Color interpolatedColor = Color.Lerp(initialColor, targetColor, timer / duration);

        // Присвоить новый цвет компоненту Image
        image.color = interpolatedColor;

        // Проверить, достигнута ли целевая продолжительность анимации
        if (timer >= duration)
        {
            // Удалить компонент ColorAnimation после завершения анимации
            Destroy(this);
        }
    }
}
