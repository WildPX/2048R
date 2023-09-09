using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MusicVisualizer : MonoBehaviour
{
    public Image backgroundImage; // ������ �� ��������� Image ��� ������� ����
    public int tempo;
    public int bars;

    private float colorChangeDuration; // ����������������� ��������� �����
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

        // ������ �������� ��������� ����� ������� ����
        StartColorAnimation();
    }

    private void Update()
    {
        // �������� ������ � ���������, ���������� �� ������� ����������������� ��������
        timer += Time.deltaTime;
        if (timer >= colorChangeDuration)
        {
            // ������ ����� ���� � ������ �������� ��������� ����� ������� ����
            initialColor = targetColor;
            targetColor = GetRandomColor();
            timer = 0f;
            StartColorAnimation();
        }

        // ��������������� ������� ���� ������� ���� ����� ��������� � ������� ������
        Color interpolatedColor = Color.Lerp(initialColor, targetColor, timer / colorChangeDuration);

        // ��������� ����� ���� ���������� Image ������� ����
        backgroundImage.color = interpolatedColor;
    }

    private void StartColorAnimation()
    {
        // ������� ����� �������� ��������� ����� ������� ����
        var colorAnimation = backgroundImage.gameObject.AddComponent<ColorAnimation>();
        colorAnimation.duration = colorChangeDuration;
        colorAnimation.initialColor = initialColor;
        colorAnimation.targetColor = targetColor;
    }

    private Color GetRandomColor()
    {
        // ������������ ��������� ����
        return new Color(Random.value, Random.value, Random.value);
    }
}
