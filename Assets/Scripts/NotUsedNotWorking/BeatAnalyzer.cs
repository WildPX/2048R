using UnityEngine;

public class BeatAnalyzer : MonoBehaviour
{
    public AudioSource audioSource;
    public int sampleSize = 1024;
    public FFTWindow windowType = FFTWindow.BlackmanHarris;
    public float beatThreshold = 0.7f;

    private float[] spectrumData;
    private float[] previousSpectrumData;
    private float[] beatData;

    private void Start()
    {
        spectrumData = new float[sampleSize];
        previousSpectrumData = new float[sampleSize];
        beatData = new float[sampleSize];
    }

    private void Update()
    {
        audioSource.GetSpectrumData(spectrumData, 0, windowType);

        // ����������� �����
        for (int i = 0; i < sampleSize; i++)
        {
            if (spectrumData[i] > beatThreshold && spectrumData[i] > previousSpectrumData[i])
            {
                // ��������� ��� � ������� i
                beatData[i] = spectrumData[i];
            }
            else
            {
                // ���� ��� ����, ��������� �������� � �������� �������
                beatData[i] *= 0.95f;
            }
        }

        // ����� �� ������ ������������ �������� beatData ��� ������������ �� ���� ��� ����������� ������� � ����� ����.
        // ��������, �� ������ �������� ������� ��� ��������� ��������, ����� ��� ��������������.

        // ��������� ������� ������ ������� ��� ���������� �����
        //Array.Copy(spectrumData, previousSpectrumData, sampleSize);
    }
}
