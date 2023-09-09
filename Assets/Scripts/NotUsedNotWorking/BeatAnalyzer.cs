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

        // Определение битов
        for (int i = 0; i < sampleSize; i++)
        {
            if (spectrumData[i] > beatThreshold && spectrumData[i] > previousSpectrumData[i])
            {
                // Обнаружен бит в позиции i
                beatData[i] = spectrumData[i];
            }
            else
            {
                // Если нет бита, уменьшаем значение с течением времени
                beatData[i] *= 0.95f;
            }
        }

        // Далее вы можете использовать значения beatData для реагирования на биты или ритмические события в вашей игре.
        // Например, вы можете вызывать функции или выполнять действия, когда бит обнаруживается.

        // Сохраняем текущие данные спектра для следующего кадра
        //Array.Copy(spectrumData, previousSpectrumData, sampleSize);
    }
}
