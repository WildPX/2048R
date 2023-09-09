using UnityEngine;
using UnityEngine.UI;

public class ColorAnimation : MonoBehaviour
{
    public float duration = 1f; // ����������������� �������� � ��������
    public Color initialColor; // ��������� ����
    public Color targetColor; // ������� ����

    private float timer;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        // ��������� ������
        timer += Time.deltaTime;

        // ��������������� ������� ���� ����� ��������� � ������� ������
        Color interpolatedColor = Color.Lerp(initialColor, targetColor, timer / duration);

        // ��������� ����� ���� ���������� Image
        image.color = interpolatedColor;

        // ���������, ���������� �� ������� ����������������� ��������
        if (timer >= duration)
        {
            // ������� ��������� ColorAnimation ����� ���������� ��������
            Destroy(this);
        }
    }
}
