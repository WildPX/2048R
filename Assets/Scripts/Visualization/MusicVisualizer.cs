using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MusicVisualizer : MonoBehaviour
{
    public Image backgroundImage; // Ссылка на компонент Image для заднего фона
    public int tempo;
    public int bars;

    private float colorChangeDuration; // Продолжительность изменения цвета
    private AudioSource audioSource;
    private Color initialColor;
    private Color targetColor;
    private float timer;

    private void Start()
    {
        colorChangeDuration = 60f / tempo * bars;
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        initialColor = backgroundImage.color;
        targetColor = GetRandomColor();
        timer = 0f;

        // Начать анимацию изменения цвета заднего фона
        StartColorAnimation();
    }

    private void Update()
    {
        // Обновить таймер и проверить, достигнута ли целевая продолжительность анимации
        timer += Time.deltaTime;
        if (timer >= colorChangeDuration)
        {
            // Задать новый цвет и начать анимацию изменения цвета заднего фона
            initialColor = targetColor;
            targetColor = GetRandomColor();
            timer = 0f;
            StartColorAnimation();
        }

        // Интерполировать текущий цвет заднего фона между начальным и целевым цветом
        Color interpolatedColor = Color.Lerp(initialColor, targetColor, timer / colorChangeDuration);

        // Присвоить новый цвет компоненту Image заднего фона
        backgroundImage.color = interpolatedColor;
    }

    private void StartColorAnimation()
    {
        // Создать новую анимацию изменения цвета заднего фона
        var colorAnimation = backgroundImage.gameObject.AddComponent<ColorAnimation>();
        colorAnimation.duration = colorChangeDuration;
        colorAnimation.initialColor = initialColor;
        colorAnimation.targetColor = targetColor;
    }

    private Color GetRandomColor()
    {
        // Генерировать случайный цвет
        return new Color(Random.value, Random.value, Random.value);
    }
}
