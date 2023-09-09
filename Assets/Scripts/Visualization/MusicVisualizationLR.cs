using UnityEngine;

public class MusicVisualizerLR : MonoBehaviour
{
    public int numberOfBars = 64;
    public float barHeightMultiplier = 10f;
    public float barWidth = 1f;
    public float spacing = 0.1f;
    public Transform leftBarsParent;
    public Transform rightBarsParent;
    public AudioSource audioSource { get; private set; }

    private float[] spectrumData;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spectrumData = new float[numberOfBars];
    }

    private void Update()
    {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);

        for (int i = 0; i < numberOfBars; i++)
        {
            Vector3 leftBarScale = leftBarsParent.GetChild(i).localScale;
            leftBarScale.y = spectrumData[i] * barHeightMultiplier;
            leftBarsParent.GetChild(i).localScale = leftBarScale;

            Vector3 rightBarScale = rightBarsParent.GetChild(i).localScale;
            rightBarScale.y = spectrumData[i] * barHeightMultiplier;
            rightBarsParent.GetChild(i).localScale = rightBarScale;

            Vector3 leftBarPosition = leftBarsParent.GetChild(i).localPosition;
            leftBarPosition.y = leftBarScale.y / 2;
            leftBarsParent.GetChild(i).localPosition = leftBarPosition;

            Vector3 rightBarPosition = rightBarsParent.GetChild(i).localPosition;
            rightBarPosition.y = rightBarScale.y / 2;
            rightBarsParent.GetChild(i).localPosition = rightBarPosition;

            Vector3 barPositionOffset = new Vector3((barWidth + spacing) * i, 0, 0);
            leftBarsParent.GetChild(i).localPosition = barPositionOffset;
            rightBarsParent.GetChild(i).localPosition = -barPositionOffset;
        }
    }
}
