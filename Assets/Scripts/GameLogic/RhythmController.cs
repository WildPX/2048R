using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RhythmController : MonoBehaviour
{
    // Group of text elements for showing best score for this song
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI bestScoreAccuracyText;
    public TextMeshProUGUI bestScoreStreakText;
    //public TextMeshProUGUI bestScoreRank;
    // Group of text elements for showing current score for this song
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI currentAccuracyText;
    public TextMeshProUGUI currentComboText;
    public TextMeshProUGUI currentBestComboText;
    public TextMeshProUGUI currentQualityScoreText;
    public TextMeshProUGUI qualityText;
    public TextMeshProUGUI breakText;
    // Starting screen
    public CanvasGroup startScreen;
    // Ranking Screen
    public CanvasGroup endingScreen;
    //public TextMeshProUGUI endingScreenRank;
    public TextMeshProUGUI endingScreenScore;
    public TextMeshProUGUI endingScreenAccuracy;
    public TextMeshProUGUI endingScreenStreak;
    public TextMeshProUGUI endingScreenSongName;
    public TextMeshProUGUI endingScreenArtistName;
    public TextMeshProUGUI endingScreenNewRecord;
    public TextMeshProUGUI endingScreenPerfect;
    public TextMeshProUGUI endingScreenPerfectHits;
    public TextMeshProUGUI endingScreenGoodHits;
    public TextMeshProUGUI endingScreenBadHits;
    public TextMeshProUGUI endingScreenMissHits;
    // List of all gradients
    public List<TMP_ColorGradient> endingScreenGradients;
    public List<TMP_ColorGradient> qualityGradients;
    // List of all available ranks
    private static readonly List<string> endingScreenRanks = new List<string> { "S", "A", "B", "C", "D", "F" };

    // Song completion meter
    public RectTransform songCompletionMeter;
    // For something
    public RectTransform mainCanvas;
    private float songCompletionIncrease;
    private Vector2 songCompletionIncreaseVector2 = Vector2.zero;

    // SFX
    public AudioSource sfxClick;
    //public AudioSource sfxClickNote;

    // Pause Screen
    // Button for pausing
    public Button pauseButton;
    public CanvasGroup pauseScreen;
    public CanvasGroup fadeOutBackground;
    //public Image startScreenBlackout;
    //public TextMeshProUGUI startScreenText;
    //private Color startScreenColor = Color.black;
    //private Color startScreenColorEnd = new Color(0f, 0f, 0f, 0f);
    //private Color startScreenColorTextEnd = new Color(1f, 1f, 1f, 0f);

    // Reference to board. Needed for logic elements
    public TileBoard board;
    // Used when beat frame is hit
    public Image boardBorder;
    // Background of this song
    public Image musicBackground;
    // Slider for showing current life of player
    public Slider lifeSlider;
    // Container for data of this song
    public MapDataConfig MapConfig { get; private set; }
    // Time Manager to track time
    public TimeManager TimeManagerGlobal { get; private set; }
    // Pause time
    private float pauseStartTime;
    // Current music clip
    private AudioSource audioSource;
    // Note screen helper
    // Note
    public RectTransform noteHitShow;
    private float fixedLengthTime;
    public GameObject notePrefab;
    private Queue<GameObject> notes = new Queue<GameObject>();
    private Color startNoteColor = new Color(1f, 1f, 1f, 0f);
    private Color endNoteColor = Color.white;
    // Note border. Should be lit when ideal hit is available
    public Image idealNoteBorder;

    /// GAME LOGIC
    // OVERALL
    // Global timer
    //private float globalTime;
    // Current life
    private float life;
    // Decrease life this each time
    private const float lifeDecreaseDuration = 0.1f;
    // Seconds to wait before start of the song
    private const float delayBeforeStart = 3f;
    // Seconds to wait before showing ranking screen
    private const float delayBeforeEnd = 3f;
    // Seconds to wait after the pause
    private const float delayAfterPause = 1f;
    // Combo down multiplier
    private const int comboBreakerMultiplier = 5;
    // Check for starting screen (Get Ready!)
    private bool isStarting;
    // Check if song is over with positive result
    private bool isEnded;
    // Check if there's no more time stamps but the song is still playing
    private bool isSongEnding;
    // Check if there is a pause
    private bool isPaused;
    // workaround
    private bool isUnpausing;
    // workaround
    private bool isEnding;
    // Current beat frame
    private float currentBeat;
    // Current beat frame index
    private int currentBeatIndex;
    // Limited frame rate
    private const int fixedFrameRate = 60;
    // Metrics
    // Current score
    private int currentScore;
    // Current streak
    private int currentCombo;
    // Best combo
    private int bestCombo;
    // Best combo, points
    private int bestComboPoints;
    // Minimum combo for combo bonus to work
    private const int bestComboMinimum = 5;
    // Current accuracy metric
    private float currentAccuracy;
    // Current rank
    //private string currentRank;
    // Music time in seconds
    private int musicTime;

    // Hits 
    // Used for comparing it and inputTime from tileboard. If numbers are different, there was input
    private float currentInputTime;
    // User managed to hit note in the time of (currentBeat + missThreshold)
    private bool changeNote;

    /// Coroutines
    private Coroutine decreaseLife;
    private Coroutine borderOutlineBacklight;
    private Coroutine comboBreakerRoutine;
    //private Queue<Coroutine> noteBacklight = new Queue<Coroutine>();
    //private Queue<Coroutine> noteToEnd = new Queue<Coroutine>();

    ///  GameObjects
    //private List<GameObject> noteObjects = new List<GameObject>();
    GameObject[] noteObjects;

    // Metrics
    private int perfectHits;
    private int goodHits;
    private int badHits;
    private int missHits;
    private const float idealWeight = 1.0f;
    private const float goodWeight = 0.3f;
    private const float badWeight = 0.16f;
    private const float missThreshold = 0.2f;
    private const float missThresholdIdeal = 0.05f;
    private const float missThresholdGood = 0.1f;
    private const float missThresholdBad = 0.15f;
    private int maxCombo;

    /// UI
    // Colors used for board border
    private Color startAlpha = Color.white;
    private Color endAlpha = new Color(1f, 1f, 1f, 0f);

    private void Awake()
    {
        InitializeVariables();
    }

    private void Start()
    {
        // Limit the frame rate
        //Application.targetFrameRate = 60;
        // Load all data from JSON and use it
        LoadMusicData();
        StartCoroutine(StartGame());
    }
    // Function to start a new game. Updates and prepares the board
    public void NewGame()
    {
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.SetInputLock(false);
    }
    // If game is ended. Check = won or lost
    private void GameOver(bool check)
    {
        isEnded = true;
        isEnding = true;
        pauseButton.gameObject.SetActive(false);
        board.SetInputLock(true);

        SetRankingScreenTexts();

        StartCoroutine(MusicFadeOut(check));
        StartCoroutine(RankScreenFadeIn());
        StopCoroutine(borderOutlineBacklight);
        StopCoroutine(decreaseLife);

        if (check)
        {
            SaveBestScores();
        }
    }

    private void Update()
    {
        //Debug.Log(Time.fixedDeltaTime);
        // Complete the completition meter
        if (isEnded && isEnding)
        {
            UpdateSongCompletionMeter();
        }

        // If the game is paused, unpausing or ending => do not use Update()
        if (isStarting || isUnpausing || isEnding)
        {
            return;
        }

        // If time stamps are over but the song is not
        if (isSongEnding)
        {
            TimeManagerGlobal.IncreaseGlobalTime();
            UpdateSongCompletionMeter();
            UpdateLifeMeter();

            if (Mathf.Abs(TimeManagerGlobal.GetGlobalTime() - musicTime) <= delayBeforeEnd)
            {
                isEnded = true;
                isSongEnding = false;
            }
        }
        else
        {
            // If pause is activated
            if (!isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                //Debug.Log("Pause");
                isPaused = true;
                TogglePause(true);
            }
            // Unpause
            else if (isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                //Debug.Log("Unpause");
                isUnpausing = true;
                TogglePause(false);
            }
            // Song is in progress
            if (!isEnded && !isPaused)
            {
                TimeManagerGlobal.IncreaseGlobalTime();
                UpdateSongCompletionMeter();
                UpdateLifeMeter();

                // If player died or there is a 2048 game over
                if (life <= 0 || board.GameOver)
                {
                    board.SetInputLock(true);
                    GameOver(false);
                    return;
                }
                // Process input of a user every frame
                ProcessInput(TimeManagerGlobal.GetGlobalTime());
            }
            // Song has ended
            else if (isEnded)
            {
                GameOver(true);
            }
        }
    }

    // Update song completion meter
    private void UpdateSongCompletionMeter()
    {
        songCompletionIncreaseVector2.x = songCompletionIncrease * Time.deltaTime;
        songCompletionMeter.sizeDelta += songCompletionIncreaseVector2;
    }
    // Update life meter
    private void UpdateLifeMeter()
    {
        lifeSlider.value = life;
    }
    // Initialize all needed variables
    private void InitializeVariables()
    {
        currentScore = 0;
        currentCombo = 0;
        bestCombo = 0;
        bestComboPoints = 0;
        currentAccuracy = 1f;
        TimeManagerGlobal = new TimeManager();
        pauseStartTime = 0f;
        isStarting = true;
        //godMode = false;
        changeNote = false;
        isPaused = false;
        isEnded = false;
        isEnding = false;
        isUnpausing = false;
        currentInputTime = 0f;
        perfectHits = 0;
        goodHits = 0;
        badHits = 0;
        missHits = 0;
        life = 1f;
        boardBorder.color = endAlpha;
        audioSource = GetComponent<AudioSource>();

        board.TMGlobal = TimeManagerGlobal;
    }
    // Coroutine for starting the game
    private IEnumerator StartGame()
    {
        // Load data
        MusicDataLoad();
        MusicLoad();
        LoadBestScores();

        // Soft increase in alpha channel of startScreen
        float elapsedTime = 0f;
        while (elapsedTime < delayBeforeStart)
        {
            float t = elapsedTime / delayBeforeStart;
            startScreen.alpha = Mathf.Lerp(1f, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        startScreen.alpha = 0f;

        // Clear board, create tiles
        NewGame();
        TimeManagerGlobal.InitializeStartTime();
        TimeManagerGlobal.InitializeGlobalTime();
        
        MusicPlay();

        // Start Coroutines
        borderOutlineBacklight = StartCoroutine(SingleBorderOutlineBacklight());
        SpawnNote();

        // Damage to life every 0.1f seconds
        decreaseLife = StartCoroutine(DecreaseLifeCoroutine());

        pauseButton.gameObject.SetActive(true);

        isStarting = false;
        //Debug.Log(TimeManagerGlobal.GetStartTime());
        //Debug.Log(TimeManagerGlobal.GetGlobalTime());
    }
    // Coroutine for music fade out
    private IEnumerator MusicFadeOut(bool win)
    {
        float elapsedTime = 0f;
        float startVolume = audioSource.volume;
        float startPitch = audioSource.pitch;
        while (elapsedTime < delayBeforeEnd)
        {
            float t = elapsedTime / delayBeforeEnd;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            if (!win)
            {
                audioSource.pitch = Mathf.Lerp(startPitch, 0f, t);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        audioSource.pitch = 0f;
    }
    // Load current JSON
    private void LoadMusicData()
    {
        var md = FindObjectOfType<MapData>();

        if (md != null)
        {
            MapConfig = md.DataConfig;
            if (MapConfig != null)
            {
                currentBeatIndex = 0;
                currentBeat = MapConfig.musicData.beatFrames[currentBeatIndex];
                musicTime = MapConfig.musicData.time;
            }
            else
            {
                Debug.LogError("mapData.musicData is null.");
            }
        }
        else
        {
            Debug.LogError("MapData object not found in the scene.");
        }

        //Debug.Log(MapConfig.backgroundImagePath);

        musicBackground.sprite = Resources.Load<Sprite>(MapConfig.backgroundImagePath);
    }
    // Load audio clip
    private void MusicLoad()
    {
        audioSource.clip = Resources.Load<AudioClip>(MapConfig.audioPath);
    }
    // Play audio clip
    private void MusicPlay()
    {
        audioSource.Play();
    }
    // Pause audio clip
    private void MusicPause()
    {
        audioSource.Pause();
    }
    // Load MusicData file where data is located
    private void MusicDataLoad()
    {
        endingScreenSongName.text = MapConfig.name;
        endingScreenArtistName.text = MapConfig.artistName;
        maxCombo = MapConfig.musicData.beatFrames.Count;

        fixedLengthTime = 60f / MapConfig.musicData.tempo;
        //Debug.Log(mainCanvas.sizeDelta.x);
        songCompletionIncrease = mainCanvas.sizeDelta.x / MapConfig.musicData.time;
    }
    // Process user input every frame
    private void ProcessInput(float globalTime)
    {
        // If player managed to do input in needed frame or just pressed button in available direction
        if (TimeManagerGlobal.CompareTimeGE(currentBeat + missThreshold) || currentInputTime < board.InputTime)
        {
            CheckInput(globalTime);
        }
    }
    // Check how good player managed to hit current frame
    private void CheckInput(float globalTime)
    {
        // Get input time from board
        currentInputTime = board.InputTime;

        // Check time difference between the time stamp and input time
        float timeDifference = Mathf.Abs(currentInputTime - currentBeat);
        // String to write state
        string result;

        // Points gotten in the process of the current move
        int points;
        // If combo broke
        int additionalPoints = 0;
        // Multiplier for points
        int multiplier = 1;
        // Decrease or Increase life on that move
        float changeHP = -0.1f;
        // Check if combo broke
        bool comboBreaker = false;
        // Check if best combo changed
        bool bestComboChanged = false;

        // Main Check for quality of the current move
        if (timeDifference >= 0f)
        {
            if (timeDifference <= missThresholdIdeal)
            {
                result = "PERFECT!";
                multiplier = 5;
                changeHP = 0.25f;
                currentCombo++;
                if (currentCombo > bestCombo)
                {
                    bestCombo = currentCombo;
                    bestComboChanged = true;
                }
                perfectHits++;
            }
            else if (timeDifference <= missThresholdGood)
            {
                result = "GOOD!";
                multiplier = 2;
                changeHP = 0.1f;
                currentCombo++;
                if (currentCombo > bestCombo)
                {
                    bestCombo = currentCombo;
                    bestComboChanged = true;
                }
                goodHits++;
            }
            else if (timeDifference <= missThresholdBad)
            {
                result = "BAD!";
                changeHP = -0.05f;
                comboBreaker = true;
                badHits++;
            }
            else
            {
                result = "MISS!";
                multiplier = 0;
                comboBreaker = true;
                missHits++;
            }
        }
        else
        {
            result = "MISS!";
            multiplier = 0;
            comboBreaker = true;
            missHits++;
        }

        // Formula for accuracy
        currentAccuracy = 100 * (idealWeight * perfectHits + goodWeight * goodHits + badWeight * badHits) / (perfectHits + goodHits + badHits + missHits);
        //Debug.Log($"{result} {currentInputTime}, {currentBeat}: {timeDifference}");
        // Decrease or Increase life
        ChangeLife(changeHP);

        // Points to add
        points = multiplier * board.CurrentMovePoints * board.NumberOfMerges;

        if (currentCombo >= bestComboMinimum && (comboBreaker || (bestComboPoints > 0 && currentBeatIndex == (maxCombo - 1))))
        {
            if (!comboBreaker)
            {
                bestComboPoints += board.CurrentMovePoints;
            }
            additionalPoints = bestComboPoints * (currentCombo / comboBreakerMultiplier + 1);
            bestComboPoints = 0;
        }
        else
        {
            // Add to best combo points the number of 'clean' points
            bestComboPoints += board.CurrentMovePoints;
        }

        if (comboBreaker)
        {
            currentCombo = 0;
        }

        //Debug.Log($"{currentBeatIndex}:: P:{points}, AP:{additionalPoints}, BCP:{bestComboPoints}, x{currentCombo}, {result}");

        SetCurrentTexts(points, result, currentCombo, currentAccuracy);

        if (additionalPoints > 0)
        {
            points += additionalPoints;
            SetAdditionalQualityPoints(additionalPoints);
        }

        if (comboBreaker)
        {
            comboBreakerRoutine = StartCoroutine(SetComboBreakerText());
        }

        if (bestComboChanged)
        {
            SetBestComboText(bestCombo);
        }

        IncreaseScore(points);

        // If input got through in a specific threshold (that is more than all of the hits thresholds)
        if (timeDifference <= missThreshold)
        {
            changeNote = true;
        }

        // If there's a need in changing a current time stamp
        if (Mathf.Approximately(globalTime, currentBeat + missThreshold) || globalTime >= currentBeat + missThreshold || changeNote)
        {
            ProcessBeat();
        }
    }
    // Change current time stamp to the next one
    private void ProcessBeat()
    {
        changeNote = false;
        currentBeatIndex++;

        // If there are no more time stamps
        if (currentBeatIndex >= maxCombo)
        {
            if (Mathf.Abs(TimeManagerGlobal.GetGlobalTime() - musicTime) <= delayBeforeEnd)
            {
                isEnded = true;
            }
            // Time stamps are already ended, but the song is still going
            else
            {
                isSongEnding = true;
                board.SetInputLock(true);
                StopCoroutine(decreaseLife);
                decreaseLife = StartCoroutine(DecreaseLifeCoroutine(-0.001f));
            }
        }
        // Change beat
        else
        {
            currentBeat = MapConfig.musicData.beatFrames[currentBeatIndex];
            NextBeatCoroutines();
        }
    }
    // Highlight the board on next time stamp and spawn next note
    private void NextBeatCoroutines()
    {
        borderOutlineBacklight = StartCoroutine(SingleBorderOutlineBacklight());
        SpawnNote();
    }
    // Spawns note with the current time stamp
    private void SpawnNote()
    {
        GameObject note = Instantiate(notePrefab, noteHitShow);
        note.GetComponent<NoteScript>().originalTime = Mathf.Abs(currentBeat - TimeManagerGlobal.GetGlobalTime());
        //note.GetComponent<NoteScript>().originalTime = currentBeat;
        note.GetComponent<NoteScript>().fixedTimeLength = fixedLengthTime;
        //noteObjects.Add(note);
    }
    // Kill all the notes that are outsise of the viewing area
    //private void CheckNotes()
    //{
    //    for (int i = noteObjects.Count - 1; i >= 0; i--)
    //    {
    //        if (noteObjects[i] == null)
    //            noteObjects.RemoveAt(i);
    //    }
    //}
    // Highlight the board on time stamp
    private IEnumerator SingleBorderOutlineBacklight()
    {
        // Alpha == 0
        boardBorder.color = endAlpha;
        idealNoteBorder.color = startNoteColor;

        float timeDifference = Mathf.Abs(currentBeat - TimeManagerGlobal.GetGlobalTime());
        yield return new WaitForSeconds(timeDifference);
        float elapsedTime = 0f;
        // Soft decrease in alpha channel
        while (elapsedTime < missThreshold)
        {
            float t = elapsedTime / missThreshold;
            boardBorder.color = Color.Lerp(startAlpha, endAlpha, t);
            idealNoteBorder.color = Color.Lerp(endNoteColor, startNoteColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        boardBorder.color = endAlpha;
        idealNoteBorder.color = startNoteColor;
    }
    // Decrease life (standart)
    private IEnumerator DecreaseLifeCoroutine()
    {
        while (!isEnded)
        {
            ChangeLife(-0.005f);
            yield return new WaitForSeconds(lifeDecreaseDuration);
        }
    }
    // Decrease life (with the value)
    private IEnumerator DecreaseLifeCoroutine(float value)
    {
        while (!isEnded)
        {
            ChangeLife(value);
            yield return new WaitForSeconds(lifeDecreaseDuration);
        }
    }
    // Increase the number of points
    private void IncreaseScore(int points)
    {
        currentScore += points;
        currentScoreText.text = currentScore.ToString();
        board.SetCurrentPoints(0);
    }
    // Change current life
    private void ChangeLife(float value)
    {
        life += value;
        life = Mathf.Clamp01(life);
    }
    // Save the current scores as the best ones if they are better than the previous
    private void SaveBestScores()
    {
        int bestScore = LoadBestScore();
        //int bestRank = endingScreenRanks.IndexOf(LoadBestScoreRank());

        if (missHits == 0 && goodHits == 0 && badHits == 0)
        {
            endingScreenPerfect.gameObject.SetActive(true);
        }

        if (currentScore >= bestScore)
        {
            PlayerPrefs.SetInt(MapConfig.name + "_BestScore", currentScore);
            PlayerPrefs.SetFloat(MapConfig.name + "_BestScoreAccuracy", currentAccuracy);
            PlayerPrefs.SetInt(MapConfig.name + "_BestScoreStreak", bestCombo);
            endingScreenNewRecord.gameObject.SetActive(true);
        }
    }
    // Functions to Load previous best scores
    private int LoadBestScore()
    {
        return PlayerPrefs.GetInt(MapConfig.name + "_BestScore", 0);
    }

    private float LoadBestScoreAccuracy()
    {
        return PlayerPrefs.GetFloat(MapConfig.name + "_BestScoreAccuracy", 0f);
    }

    private int LoadBestScoreStreak()
    {
        return PlayerPrefs.GetInt(MapConfig.name + "_BestScoreStreak", 0);
    }

    private void LoadBestScores()
    {
        bestScoreText.text = LoadBestScore().ToString();
        bestScoreAccuracyText.text = LoadBestScoreAccuracy().ToString("F2") + "%";
        bestScoreStreakText.text = "X" + LoadBestScoreStreak().ToString();
    }
    // Set current scores with new values
    private void SetCurrentTexts(int qualityPoints, string result, int curStreak, float accuracy)
    {
        SetQualityPoints(qualityPoints);
        SetQualityText(result);
        SetStreakText(curStreak);
        SetAccuracyText(accuracy);
    }
    private IEnumerator SetComboBreakerText()
    {
        float elapsedTime = 0f;
        breakText.alpha = 1f;
        float startAlpha = breakText.alpha;
        while (elapsedTime < fixedLengthTime)
        {
            float t = elapsedTime / fixedLengthTime;
            breakText.alpha = Mathf.Lerp(startAlpha, 0f, t);
            // !!
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        breakText.alpha = 0f;
    }
    // Set quality points with new values
    private void SetQualityPoints(int points)
    {
        currentQualityScoreText.text = points.ToString();
    }
    private void SetAdditionalQualityPoints(int points)
    {
        currentQualityScoreText.text += $" + {points.ToString()}";
    }
    // Paint the quality text
    private void SetQualityText(string result)
    {
        qualityText.text = result;
        if (result == "PERFECT!")
        {
            qualityText.colorGradientPreset = qualityGradients[0];
        }
        else if (result == "GOOD!")
        {
            qualityText.colorGradientPreset = qualityGradients[1];
        }
        else if (result == "BAD!")
        {
            qualityText.colorGradientPreset = qualityGradients[2];
        }
        else
        {
            qualityText.colorGradientPreset = qualityGradients[3];
        }
    }

    private void SetStreakText(int curStreak)
    {
        currentComboText.text = "X" + curStreak.ToString();
    }

    private void SetBestComboText(int curBestStreak)
    {
        currentBestComboText.text = "X" + curBestStreak.ToString();
    }

    private void SetAccuracyText(float accuracy)
    {
        currentAccuracyText.text = accuracy.ToString("F2") + "%";
    }
    // Set rankiing screen texts
    private void SetRankingScreenTexts()
    {
        endingScreenScore.text = currentScore.ToString();
        endingScreenAccuracy.text = currentAccuracy.ToString("F2") + "%";
        endingScreenStreak.text = "X" + bestCombo.ToString();
        endingScreenPerfectHits.text = perfectHits.ToString();
        endingScreenGoodHits.text = goodHits.ToString();
        endingScreenBadHits.text = badHits.ToString();
        endingScreenMissHits.text = missHits.ToString();
    }
    // Coroutine to show the Rank Screen
    private IEnumerator RankScreenFadeIn()
    {
        float elapsedTime = 0f;
        float startAlpha = endingScreen.alpha;
        while (elapsedTime < delayBeforeEnd)
        {
            float t = elapsedTime / delayBeforeEnd;
            endingScreen.alpha = Mathf.Lerp(startAlpha, 1f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        endingScreen.interactable = true;
        endingScreen.alpha = 1;
    }
    // Toggle pause / Stop time
    private void TogglePause(bool check)
    {
        // Set the ending screen unclickable
        endingScreen.blocksRaycasts = !endingScreen.blocksRaycasts;
        if (check)
        {
            pauseStartTime = Time.time;
            pauseButton.gameObject.SetActive(false);
            // Pause the music, stop all the coroutines, lock the board
            MusicPause();
            StopCoroutine(decreaseLife);
            StopCoroutine(borderOutlineBacklight);
            if (comboBreakerRoutine != null)
            {
                StopCoroutine(comboBreakerRoutine);
            }
            // Kill notes that are outside of the viewing board
            //CheckNotes();
            noteObjects = GameObject.FindGameObjectsWithTag("Note");
            foreach (GameObject note in noteObjects)
            {
                note.GetComponent<NoteScript>().isLocked = true;
                note.GetComponent<NoteScript>().isMoving = false;
            }
            board.SetInputLock(true);
            // Set breaker text alpha to zero
            breakText.alpha = 0f;
            // Let the player interact with pause screen
            pauseScreen.alpha = 1f;
            pauseScreen.interactable = true;
            //Debug.Log(pauseStartTime);
        }
        // Untoggle Pause
        else if (!check)
        {
            StartCoroutine(TurnOffPause());
        }
    }
    // Coroutine to unpause
    private IEnumerator TurnOffPause()
    {
        // Soft transition to gameplay
        float elapsedTime = 0f;
        float startAlpha = pauseScreen.alpha;
        while (elapsedTime < delayAfterPause)
        {
            float t = elapsedTime / delayAfterPause;
            pauseScreen.alpha = Mathf.Lerp(startAlpha, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Lock the pause screen
        pauseScreen.alpha = 0f;
        pauseScreen.interactable = false;
        // Make the board interactable, uncheck the bool variables
        board.SetInputLock(false);
        isPaused = false;
        isUnpausing = false;

        MusicPlay();

        // Start all the coroutines
        if (!isSongEnding)
        {
            decreaseLife = StartCoroutine(DecreaseLifeCoroutine());
            borderOutlineBacklight = StartCoroutine(SingleBorderOutlineBacklight());
            foreach (GameObject note in noteObjects)
            {
                note.GetComponent<NoteScript>().isLocked = false;
            }
        }

        TimeManagerGlobal.IncreasePauseTime(Time.time - pauseStartTime);
        pauseStartTime = 0f;
        //Debug.Log(TimeManagerGlobal.GetPauseTime());
        pauseButton.gameObject.SetActive(true);
    }
    // workaround
    public void SetUnpause()
    {
        isUnpausing = true;
        TogglePause(false);
    }

    public void OnPauseButton()
    {
        sfxClick.Play();
        isPaused = true;
        TogglePause(true);
    }

    public void SetIsEnding(bool state)
    {
        isEnding = state;
    }
}

// Time manager to count ingame time on the board, not the actual real time
// Useful so that the game will actually work
public class TimeManager
{
    private float globalTime;
    private float startTime;
    private float pauseTime = 0f;

    public void InitializeStartTime()
    {
        startTime = Time.time;
    }

    public void InitializeGlobalTime()
    {
        globalTime = Time.time - startTime;
    }

    public float GetStartTime()
    {
        return startTime;
    }

    public float GetGlobalTime()
    {
        return globalTime;
    }

    public float GetPauseTime()
    {
        return pauseTime;
    }

    public void SetGlobalTime(float value)
    {
        globalTime = value;
    }

    public void IncreaseGlobalTime()
    {
        globalTime = Time.time - startTime - pauseTime;
    }

    public void IncreasePauseTime(float time)
    {
        pauseTime += time;
    }

    public bool CompareTimeGE(float value)
    {
        if (Mathf.Approximately(globalTime, value) || globalTime >= value)
        {
            return true;
        }

        return false;
    }
}
