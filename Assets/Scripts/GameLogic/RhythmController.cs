using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RhythmController : MonoBehaviour
{
    #region UI_ELEMENTS
    // Group of text elements for showing best score for this song
    [SerializeField] private TextMeshProUGUI _bestScoreText;
    [SerializeField] private TextMeshProUGUI _bestScoreAccuracyText;
    [SerializeField] private TextMeshProUGUI _bestScoreStreakText;
    //public TextMeshProUGUI bestScoreRank;
    // Group of text elements for showing current score for this song
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private TextMeshProUGUI _currentAccuracyText;
    [SerializeField] private TextMeshProUGUI _currentComboText;
    [SerializeField] private TextMeshProUGUI _currentBestComboText;
    [SerializeField] private TextMeshProUGUI _currentQualityScoreText;
    [SerializeField] private TextMeshProUGUI _qualityText;
    [SerializeField] private TextMeshProUGUI _breakText;
    // Starting screen
    [SerializeField] private CanvasGroup _startScreen;
    // Ranking Screen
    [SerializeField] private CanvasGroup _endingScreen;
    //public TextMeshProUGUI endingScreenRank;
    [SerializeField] private TextMeshProUGUI _endingScreenScore;
    [SerializeField] private TextMeshProUGUI _endingScreenAccuracy;
    [SerializeField] private TextMeshProUGUI _endingScreenStreak;
    [SerializeField] private TextMeshProUGUI _endingScreenSongName;
    [SerializeField] private TextMeshProUGUI _endingScreenArtistName;
    [SerializeField] private TextMeshProUGUI _endingScreenNewRecord;
    [SerializeField] private TextMeshProUGUI _endingScreenPerfect;
    [SerializeField] private TextMeshProUGUI _endingScreen_perfectHits;
    [SerializeField] private TextMeshProUGUI _endingScreen_goodHits;
    [SerializeField] private TextMeshProUGUI _endingScreen_badHits;
    [SerializeField] private TextMeshProUGUI _endingScreen_missHits;
    // List of all gradients
    [SerializeField] private List<TMP_ColorGradient> _endingScreenGradients;
    [SerializeField] private List<TMP_ColorGradient> _qualityGradients;
    // List of all available ranks
    //private static readonly List<string> endingScreenRanks = new List<string> { "S", "A", "B", "C", "D", "F" };

    // Song completion meter
    [SerializeField] private RectTransform _songCompletionMeter;
    // For something
    [SerializeField] private RectTransform _mainCanvas;
    private float _songCompletionIncrease;
    private Vector2 _songCompletionIncreaseVector2 = Vector2.zero;

    // SFX
    [SerializeField] private AudioSource _sfxClick;
    //public AudioSource sfxClickNote;

    // Pause Screen
    // Button for pausing
    // TODO CHANGE
    public Button _pauseButton;
    public CanvasGroup _pauseScreen;
    public CanvasGroup _fadeOutBackground;
    //public Image startScreenBlackout;
    //public TextMeshProUGUI startScreenText;
    //private Color startScreenColor = Color.black;
    //private Color startScreenColorEnd = new Color(0f, 0f, 0f, 0f);
    //private Color startScreenColorTextEnd = new Color(1f, 1f, 1f, 0f);


    // Reference to board. Needed for logic elements
    [SerializeField] private TileBoard _board;
    // Used when beat frame is hit
    [SerializeField] private Image _boardBorder;
    // Background of this song
    [SerializeField] private Image _musicBackground;
    // Slider for showing current life of player
    [SerializeField] private Slider _lifeSlider;
    // Container for data of this song
    private MapDataConfig _mapConfig;
    // Time Manager to track time
    public TimeManager TimeManagerGlobal { get; private set; }
    // Pause time
    private float _pauseStartTime;
    // Current music clip
    private AudioSource _audioSource;
    // Note screen helper
    // Note
    [SerializeField] private RectTransform _noteHitShow;
    private float _fixedLengthTime;
    [SerializeField] private GameObject _notePrefab;
    //private Queue<GameObject> notes = new Queue<GameObject>();
    private Color _startNoteColor = new Color(1f, 1f, 1f, 0f);
    private Color _endNoteColor = Color.white;
    // Note border. Should be lit when ideal hit is available
    [SerializeField] private Image _idealNoteBorder;

    // UI
    // Colors used for board border
    private Color _startAlpha = Color.white;
    private Color _endAlpha = new Color(1f, 1f, 1f, 0f);
    #endregion

    #region GAME_LOGIC
    // Decrease life this much time
    private const float LIFE_DECREASE_DURATION = 0.1f;
    // Seconds to wait before start of the song
    private const float DELAY_BEFORE_START = 3f;
    // Seconds to wait before showing ranking screen
    private const float DELAY_BEFORE_END = 3f;
    // Seconds to wait after the pause
    private const float DELAY_AFTER_PAUSE = 1f;
    // Combo down multiplier
    private const int COMBOBREAKER_MULTIPLIER = 5;
    // Current life
    private float _life;
    // Check for starting screen (Get Ready!)
    private bool _isStarting;
    // Check if song is over with positive result
    private bool _isEnded;
    // Check if there's no more time stamps but the song is still playing
    private bool _isSongEnding;
    // Check if there is a pause
    private bool _isPaused;
    // workaround
    private bool _isUnpausing;
    // workaround
    private bool _isEnding;
    // Current beat frame
    private float _currentBeat;
    // Current beat frame index
    private int _currentBeatIndex;

    // Beat processing
    // Current score
    private int _currentScore;
    // Current streak
    private int _currentCombo;
    // Best combo
    private int _bestCombo;
    // Best combo, points
    private int _bestComboPoints;
    // Minimum combo for combo bonus to work
    private const int BEST_COMBO_MINIMUM = 5;
    // Current accuracy metric
    private float _currentAccuracy;
    // Music time in seconds
    private int _musicTime;
    // Used for comparing it and inputTime from tileboard. If numbers are different, there was input
    private float _currentInputTime;
    // User managed to hit note in the time of (currentBeat + missThreshold)
    private bool _changeNote;
    // Current rank
    //private string currentRank;
    #endregion

    #region HITS_SYSTEM
    // List of all game objects 'notes'
    private GameObject[] _noteObjects;

    // Hit system
    private const float IDEAL_WEIGHT = 1.0f;
    private const float GOOD_WEIGHT = 0.3f;
    private const float BAD_WEIGHT = 0.16f;
    private const float MISS_THRESHOLD = 0.2f;
    private const float MISS_THRESHOLD_IDEAL = 0.05f;
    private const float MISS_THRESHOLD_GOOD = 0.1f;
    private const float MISS_THRESHOLD_BAD = 0.15f;
    private int _perfectHits;
    private int _goodHits;
    private int _badHits;
    private int _missHits;
    private int _maxCombo;
    #endregion

    #region COROUTINES
    private Coroutine _decreaseLife;
    private Coroutine _borderOutlineBacklight;
    private Coroutine _comboBreakerRoutine;
    #endregion

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
    /// <summary>
    /// Prepares the board for a new session.
    /// </summary>
    public void NewGame()
    {
        _board.ClearBoard();
        _board.CreateTile();
        _board.CreateTile();
        _board.SetInputLock(false);
    }
    /// <summary>
    /// Ends the game. Stops all coroutines, saves the best scores, if necessary.
    /// </summary>
    /// <param name="check">If true, player won. If false, player lost.</param>
    private void GameOver(bool check)
    {
        _isEnded = true;
        _isEnding = true;
        _pauseButton.gameObject.SetActive(false);
        _board.SetInputLock(true);

        SetRankingScreenTexts();

        StartCoroutine(MusicFadeOut(check));
        StartCoroutine(RankScreenFadeIn());
        StopCoroutine(_borderOutlineBacklight);
        StopCoroutine(_decreaseLife);

        if (check)
        {
            SaveBestScores();
        }
    }

    private void Update()
    {
        //Debug.Log(Time.fixedDeltaTime);
        // Complete the completition meter
        if (_isEnded && _isEnding)
        {
            UpdateSongCompletionMeter();
        }

        // If the game is paused, unpausing or ending => do not use Update()
        if (_isStarting || _isUnpausing || _isEnding)
        {
            return;
        }

        // If time stamps are over but the song is not
        if (_isSongEnding)
        {
            TimeManagerGlobal.IncreaseGlobalTime();
            UpdateSongCompletionMeter();
            UpdateLifeMeter();

            if (Mathf.Abs(TimeManagerGlobal.GetGlobalTime() - _musicTime) <= DELAY_BEFORE_END)
            {
                _isEnded = true;
                _isSongEnding = false;
            }
        }
        else
        {
            // If pause is activated
            if (!_isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                //Debug.Log("Pause");
                _isPaused = true;
                TogglePause(true);
            }
            // Unpause
            else if (_isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                //Debug.Log("Unpause");
                _isUnpausing = true;
                TogglePause(false);
            }
            // Song is in progress
            if (!_isEnded && !_isPaused)
            {
                TimeManagerGlobal.IncreaseGlobalTime();
                UpdateSongCompletionMeter();
                UpdateLifeMeter();

                // If player died or there is a 2048 game over
                if (_life <= 0 || _board.GameOver)
                {
                    _board.SetInputLock(true);
                    GameOver(false);
                    return;
                }
                // Process input of a user every frame
                ProcessInput(TimeManagerGlobal.GetGlobalTime());
            }
            // Song has ended
            else if (_isEnded)
            {
                GameOver(true);
            }
        }
    }

    /// <summary>
    /// Increases the song completion meter, that is in the background of the game.
    /// Uses Time.deltaTime to transition further.
    /// </summary>
    private void UpdateSongCompletionMeter()
    {
        _songCompletionIncreaseVector2.x = _songCompletionIncrease * Time.deltaTime;
        _songCompletionMeter.sizeDelta += _songCompletionIncreaseVector2;
    }
    /// <summary>
    /// Set new value in the _lifeSlider.
    /// </summary>
    private void UpdateLifeMeter()
    {
        _lifeSlider.value = _life;
    }
    /// <summary>
    /// Initializes all the variables with default values. 
    /// </summary>
    private void InitializeVariables()
    {
        _currentScore = 0;
        _currentCombo = 0;
        _bestCombo = 0;
        _bestComboPoints = 0;
        _currentAccuracy = 1f;
        TimeManagerGlobal = new TimeManager();
        _pauseStartTime = 0f;
        _isStarting = true;
        //godMode = false;
        _changeNote = false;
        _isPaused = false;
        _isEnded = false;
        _isEnding = false;
        _isUnpausing = false;
        _currentInputTime = 0f;
        _perfectHits = 0;
        _goodHits = 0;
        _badHits = 0;
        _missHits = 0;
        _life = 1f;
        _boardBorder.color = _endAlpha;
        _audioSource = GetComponent<AudioSource>();

        _board.SetTimeManager(TimeManagerGlobal);
    }
    /// <summary>
    /// Before the player can play, we need to set multiple values. Start coroutines, sets the needed text in game objects.
    /// </summary>
    private IEnumerator StartGame()
    {
        // Load data
        MusicDataLoad();
        MusicLoad();
        SetBestScoresTexts();

        // Soft increase in alpha channel of startScreen
        float elapsedTime = 0f;
        while (elapsedTime < DELAY_BEFORE_START)
        {
            float t = elapsedTime / DELAY_BEFORE_START;
            _startScreen.alpha = Mathf.Lerp(1f, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _startScreen.alpha = 0f;

        // Clear board, create tiles
        NewGame();
        TimeManagerGlobal.InitializeStartTime();
        TimeManagerGlobal.InitializeGlobalTime();
        
        MusicPlay();

        // Start Coroutines
        _borderOutlineBacklight = StartCoroutine(SingleBorderOutlineBacklight());
        SpawnNote();

        // Damage to life every 0.1f seconds
        _decreaseLife = StartCoroutine(DecreaseLifeCoroutine());

        _pauseButton.gameObject.SetActive(true);

        _isStarting = false;
        //Debug.Log(TimeManagerGlobal.GetStartTime());
        //Debug.Log(TimeManagerGlobal.GetGlobalTime());
    }
    /// <summary>
    /// Before the end, music fades out.
    /// </summary>
    /// <param name="win">If player lost, pitch changes to zero.</param>
    private IEnumerator MusicFadeOut(bool win)
    {
        float elapsedTime = 0f;
        float startVolume = _audioSource.volume;
        float startPitch = _audioSource.pitch;
        while (elapsedTime < DELAY_BEFORE_END)
        {
            float t = elapsedTime / DELAY_BEFORE_END;
            _audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            if (!win)
            {
                _audioSource.pitch = Mathf.Lerp(startPitch, 0f, t);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _audioSource.pitch = 0f;
    }
    /// <summary>
    /// Load the needed data from JSON.
    /// </summary>
    private void LoadMusicData()
    {
        var md = FindObjectOfType<MapData>();

        if (md != null)
        {
            _mapConfig = md.DataConfig;
            if (_mapConfig != null)
            {
                _currentBeatIndex = 0;
                _currentBeat = _mapConfig.musicData.beatFrames[_currentBeatIndex];
                _musicTime = _mapConfig.musicData.time;
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

        _musicBackground.sprite = Resources.Load<Sprite>(_mapConfig.backgroundImagePath);
    }
    /// <summary>
    /// Load JSON with parameters for this map.
    /// </summary>
    private void MusicLoad()
    {
        _audioSource.clip = Resources.Load<AudioClip>(_mapConfig.audioPath);
    }
    /// <summary>
    /// Play music clip.
    /// </summary>
    private void MusicPlay()
    {
        _audioSource.Play();
    }
    /// <summary>
    /// Pause music clip.
    /// </summary>
    private void MusicPause()
    {
        _audioSource.Pause();
    }
    /// <summary>
    /// Load data from JSON.
    /// </summary>
    private void MusicDataLoad()
    {
        _endingScreenSongName.text = _mapConfig.name;
        _endingScreenArtistName.text = _mapConfig.artistName;
        _maxCombo = _mapConfig.musicData.beatFrames.Count;

        _fixedLengthTime = 60f / _mapConfig.musicData.tempo;
        //Debug.Log(mainCanvas.sizeDelta.x);
        _songCompletionIncrease = _mainCanvas.sizeDelta.x / _mapConfig.musicData.time;
    }
    /// <summary>
    /// Processes input. If time period for this beat ended or player pressed the button, check input.
    /// </summary>
    private void ProcessInput(float globalTime)
    {
        if (TimeManagerGlobal.CompareTimeGE(_currentBeat + MISS_THRESHOLD) || _currentInputTime < _board.InputTime)
        {
            CheckInput(globalTime);
        }
    }
    /// <summary>
    /// Checks how good player hit this note. 
    /// Calculates the scores by formula, increases or breaks the combo, 
    /// changes current beat, if necessary.
    /// </summary>
    private void CheckInput(float globalTime)
    {
        // Get input time from board
        _currentInputTime = _board.InputTime;

        // Check time difference between the time stamp and input time
        float timeDifference = Mathf.Abs(_currentInputTime - _currentBeat);
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
            if (timeDifference <= MISS_THRESHOLD_IDEAL)
            {
                result = "PERFECT!";
                multiplier = 5;
                changeHP = 0.25f;
                _currentCombo++;
                if (_currentCombo > _bestCombo)
                {
                    _bestCombo = _currentCombo;
                    bestComboChanged = true;
                }
                _perfectHits++;
            }
            else if (timeDifference <= MISS_THRESHOLD_GOOD)
            {
                result = "GOOD!";
                multiplier = 2;
                changeHP = 0.1f;
                _currentCombo++;
                if (_currentCombo > _bestCombo)
                {
                    _bestCombo = _currentCombo;
                    bestComboChanged = true;
                }
                _goodHits++;
            }
            else if (timeDifference <= MISS_THRESHOLD_BAD)
            {
                result = "BAD!";
                changeHP = -0.05f;
                comboBreaker = true;
                _badHits++;
            }
            else
            {
                result = "MISS!";
                multiplier = 0;
                comboBreaker = true;
                _missHits++;
            }
        }
        else
        {
            result = "MISS!";
            multiplier = 0;
            comboBreaker = true;
            _missHits++;
        }

        // Formula for accuracy
        _currentAccuracy = 100 * (IDEAL_WEIGHT * _perfectHits + GOOD_WEIGHT * _goodHits + BAD_WEIGHT * _badHits) / (_perfectHits + _goodHits + _badHits + _missHits);
        //Debug.Log($"{result} {currentInputTime}, {currentBeat}: {timeDifference}");
        // Decrease or Increase life
        ChangeLife(changeHP);

        // Points to add
        points = multiplier * _board.CurrentMovePoints * _board.NumberOfMerges;

        if (_currentCombo >= BEST_COMBO_MINIMUM && (comboBreaker || (_bestComboPoints > 0 && _currentBeatIndex == (_maxCombo - 1))))
        {
            if (!comboBreaker)
            {
                _bestComboPoints += _board.CurrentMovePoints;
            }
            additionalPoints = _bestComboPoints * (_currentCombo / COMBOBREAKER_MULTIPLIER + 1);
            _bestComboPoints = 0;
        }
        else
        {
            // Add to best combo points the number of 'clean' points
            _bestComboPoints += _board.CurrentMovePoints;
        }

        if (comboBreaker)
        {
            _currentCombo = 0;
        }

        //Debug.Log($"{currentBeatIndex}:: P:{points}, AP:{additionalPoints}, BCP:{bestComboPoints}, x{currentCombo}, {result}");

        SetCurrentTexts(points, result, _currentCombo, _currentAccuracy);

        if (additionalPoints > 0)
        {
            points += additionalPoints;
            SetAdditionalQualityPoints(additionalPoints);
        }

        if (comboBreaker)
        {
            _comboBreakerRoutine = StartCoroutine(SetComboBreakerText());
        }

        if (bestComboChanged)
        {
            SetBestComboText(_bestCombo);
        }

        IncreaseScore(points);

        // If input got through in a specific threshold (that is more than all of the hits thresholds)
        if (timeDifference <= MISS_THRESHOLD)
        {
            _changeNote = true;
        }

        // If there's a need in changing a current time stamp
        if (Mathf.Approximately(globalTime, _currentBeat + MISS_THRESHOLD) || globalTime >= _currentBeat + MISS_THRESHOLD || _changeNote)
        {
            ProcessBeat();
        }
    }
    /// <summary>
    /// Changes current beat, if necessary.
    /// </summary>
    private void ProcessBeat()
    {
        _changeNote = false;
        _currentBeatIndex++;

        // If there are no more time stamps
        if (_currentBeatIndex >= _maxCombo)
        {
            if (Mathf.Abs(TimeManagerGlobal.GetGlobalTime() - _musicTime) <= DELAY_BEFORE_END)
            {
                _isEnded = true;
            }
            // Time stamps are already ended, but the song is still going
            else
            {
                _isSongEnding = true;
                _board.SetInputLock(true);
                StopCoroutine(_decreaseLife);
                _decreaseLife = StartCoroutine(DecreaseLifeCoroutine(-0.001f));
            }
        }
        // Change beat
        else
        {
            _currentBeat = _mapConfig.musicData.beatFrames[_currentBeatIndex];
            NextBeatCoroutines();
        }
    }
    /// <summary>
    /// Starts coroutines for the next beat
    /// </summary>
    private void NextBeatCoroutines()
    {
        _borderOutlineBacklight = StartCoroutine(SingleBorderOutlineBacklight());
        SpawnNote();
    }
    /// <summary>
    /// Spawns note from prefab with current time stamp
    /// </summary>
    private void SpawnNote()
    {
        GameObject note = Instantiate(_notePrefab, _noteHitShow);
        note.GetComponent<NoteScript>().SetOriginalTime(Mathf.Abs(_currentBeat - TimeManagerGlobal.GetGlobalTime()));
        //note.GetComponent<NoteScript>().originalTime = currentBeat;
        note.GetComponent<NoteScript>().SetFixedTimeLength(_fixedLengthTime);
        //noteObjects.Add(note);
    }
    /// <summary>
    /// Highlights the board on time stamp.
    /// </summary>
    private IEnumerator SingleBorderOutlineBacklight()
    {
        // Alpha == 0
        _boardBorder.color = _endAlpha;
        _idealNoteBorder.color = _startNoteColor;

        float timeDifference = Mathf.Abs(_currentBeat - TimeManagerGlobal.GetGlobalTime());
        yield return new WaitForSeconds(timeDifference);
        float elapsedTime = 0f;
        // Soft decrease in alpha channel
        while (elapsedTime < MISS_THRESHOLD)
        {
            float t = elapsedTime / MISS_THRESHOLD;
            _boardBorder.color = Color.Lerp(_startAlpha, _endAlpha, t);
            _idealNoteBorder.color = Color.Lerp(_endNoteColor, _startNoteColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _boardBorder.color = _endAlpha;
        _idealNoteBorder.color = _startNoteColor;
    }
    /// <summary>
    /// Decreases life by 0.005f (default value).
    /// </summary>
    private IEnumerator DecreaseLifeCoroutine()
    {
        while (!_isEnded)
        {
            ChangeLife(-0.005f);
            yield return new WaitForSeconds(LIFE_DECREASE_DURATION);
        }
    }
    /// <summary>
    /// Decreases life by value.
    /// </summary>
    /// <param name="value">Value on which life decreases.</param>
    /// <returns></returns>
    private IEnumerator DecreaseLifeCoroutine(float value)
    {
        while (!_isEnded)
        {
            ChangeLife(value);
            yield return new WaitForSeconds(LIFE_DECREASE_DURATION);
        }
    }
    /// <summary>
    /// Increases current score.
    /// </summary>
    /// <param name="points">Adds this amount of points to the current score.</param>
    private void IncreaseScore(int points)
    {
        _currentScore += points;
        _currentScoreText.text = _currentScore.ToString();
        _board.SetCurrentPoints(0);
    }
    /// <summary>
    /// Change life by value.
    /// </summary>
    /// <param name="value">Adds this value to the life.</param>
    private void ChangeLife(float value)
    {
        _life += value;
        _life = Mathf.Clamp01(_life);
    }
    /// <summary>
    /// Save current scores as the best, if they are better.
    /// </summary>
    private void SaveBestScores()
    {
        int bestScore = LoadBestScore();
        //int bestRank = endingScreenRanks.IndexOf(LoadBestScoreRank());

        if (_missHits == 0 && _goodHits == 0 && _badHits == 0)
        {
            _endingScreenPerfect.gameObject.SetActive(true);
        }

        if (_currentScore >= bestScore)
        {
            PlayerPrefs.SetInt(_mapConfig.name + "_BestScore", _currentScore);
            PlayerPrefs.SetFloat(_mapConfig.name + "_BestScoreAccuracy", _currentAccuracy);
            PlayerPrefs.SetInt(_mapConfig.name + "_BestScoreStreak", _bestCombo);
            _endingScreenNewRecord.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Load best score from PlayerPrefs.
    /// </summary>
    /// <returns>Best score from PlayerPrefs.</returns>
    private int LoadBestScore()
    {
        return PlayerPrefs.GetInt(_mapConfig.name + "_BestScore", 0);
    }
    /// <summary>
    /// Load best accuracy from PlayerPrefs.
    /// </summary>
    /// <returns>Best accuracy from PlayerPrefs.</returns>
    private float LoadBestScoreAccuracy()
    {
        return PlayerPrefs.GetFloat(_mapConfig.name + "_BestScoreAccuracy", 0f);
    }
    /// <summary>
    /// Load best combo from PlayerPrefs.
    /// </summary>
    /// <returns>Best combo from PlayerPrefs</returns>
    private int LoadBestScoreStreak()
    {
        return PlayerPrefs.GetInt(_mapConfig.name + "_BestScoreStreak", 0);
    }
    /// <summary>
    /// Set texts of best scores.
    /// </summary>
    private void SetBestScoresTexts()
    {
        _bestScoreText.text = LoadBestScore().ToString();
        _bestScoreAccuracyText.text = LoadBestScoreAccuracy().ToString("F2") + "%";
        _bestScoreStreakText.text = "X" + LoadBestScoreStreak().ToString();
    }
    /// <summary>
    /// Set current scores.
    /// </summary>
    /// <param name="qualityPoints">Points player got by hitting the note.</param>
    /// <param name="result">Results of current hit of the note.</param>
    /// <param name="curStreak">Current combo.</param>
    /// <param name="accuracy">Current accuracy.</param>
    private void SetCurrentTexts(int qualityPoints, string result, int curStreak, float accuracy)
    {
        SetQualityPoints(qualityPoints);
        SetQualityText(result);
        SetComboText(curStreak);
        SetAccuracyText(accuracy);
    }
    /// <summary>
    /// Flashes the combo breaker text.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetComboBreakerText()
    {
        float elapsedTime = 0f;
        _breakText.alpha = 1f;
        float startAlpha = _breakText.alpha;
        while (elapsedTime < _fixedLengthTime)
        {
            float t = elapsedTime / _fixedLengthTime;
            _breakText.alpha = Mathf.Lerp(startAlpha, 0f, t);
            // !!
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _breakText.alpha = 0f;
    }
    /// <summary>
    /// Set quality points.
    /// </summary>
    /// <param name="points">Quality points to set.</param>
    private void SetQualityPoints(int points)
    {
        _currentQualityScoreText.text = points.ToString();
    }
    /// <summary>
    /// Set additional quality points.
    /// </summary>
    /// <param name="points">Additional quality points to set.</param>
    private void SetAdditionalQualityPoints(int points)
    {
        _currentQualityScoreText.text += $" + {points.ToString()}";
    }
    /// <summary>
    /// Set gradients for quality text.
    /// </summary>
    /// <param name="result">Result of hitting current beat.</param>
    private void SetQualityText(string result)
    {
        _qualityText.text = result;
        if (result == "PERFECT!")
        {
            _qualityText.colorGradientPreset = _qualityGradients[0];
        }
        else if (result == "GOOD!")
        {
            _qualityText.colorGradientPreset = _qualityGradients[1];
        }
        else if (result == "BAD!")
        {
            _qualityText.colorGradientPreset = _qualityGradients[2];
        }
        else
        {
            _qualityText.colorGradientPreset = _qualityGradients[3];
        }
    }
    /// <summary>
    /// Set current combo text.
    /// </summary>
    /// <param name="curStreak">Current combo.</param>
    private void SetComboText(int curCombo)
    {
        _currentComboText.text = "X" + curCombo.ToString();
    }
    /// <summary>
    /// Set current best combo text.
    /// </summary>
    /// <param name="curBestStreak">Current best combo.</param>
    private void SetBestComboText(int curBestStreak)
    {
        _currentBestComboText.text = "X" + curBestStreak.ToString();
    }
    /// <summary>
    /// Set current accuracy text.
    /// </summary>
    /// <param name="accuracy">Current accuracy.</param>
    private void SetAccuracyText(float accuracy)
    {
        _currentAccuracyText.text = accuracy.ToString("F2") + "%";
    }
    /// <summary>
    /// Set ranking screen texts. 
    /// </summary>
    private void SetRankingScreenTexts()
    {
        _endingScreenScore.text = _currentScore.ToString();
        _endingScreenAccuracy.text = _currentAccuracy.ToString("F2") + "%";
        _endingScreenStreak.text = "X" + _bestCombo.ToString();
        _endingScreen_perfectHits.text = _perfectHits.ToString();
        _endingScreen_goodHits.text = _goodHits.ToString();
        _endingScreen_badHits.text = _badHits.ToString();
        _endingScreen_missHits.text = _missHits.ToString();
    }
    /// <summary>
    /// Fades in rank screen after there are no more beats or player lost.
    /// </summary>
    private IEnumerator RankScreenFadeIn()
    {
        float elapsedTime = 0f;
        float startAlpha = _endingScreen.alpha;
        while (elapsedTime < DELAY_BEFORE_END)
        {
            float t = elapsedTime / DELAY_BEFORE_END;
            _endingScreen.alpha = Mathf.Lerp(startAlpha, 1f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _endingScreen.interactable = true;
        _endingScreen.alpha = 1;
    }
    /// <summary>
    /// Toggle pause. Stop coroutines, stop notes from moving, pause music.
    /// </summary>
    /// <param name="check">Pause = true, unpause = false.</param>
    private void TogglePause(bool check)
    {
        // Set the ending screen unclickable
        _endingScreen.blocksRaycasts = !_endingScreen.blocksRaycasts;
        if (check)
        {
            _pauseStartTime = Time.time;
            _pauseButton.gameObject.SetActive(false);
            // Pause the music, stop all the coroutines, lock the board
            MusicPause();
            StopCoroutine(_decreaseLife);
            StopCoroutine(_borderOutlineBacklight);
            if (_comboBreakerRoutine != null)
            {
                StopCoroutine(_comboBreakerRoutine);
            }
            // Kill notes that are outside of the viewing board
            //CheckNotes();
            _noteObjects = GameObject.FindGameObjectsWithTag("Note");
            foreach (GameObject note in _noteObjects)
            {
                note.GetComponent<NoteScript>().SetLocked(true);
                note.GetComponent<NoteScript>().SetMoving(false);
            }
            _board.SetInputLock(true);
            // Set breaker text alpha to zero
            _breakText.alpha = 0f;
            // Let the player interact with pause screen
            _pauseScreen.alpha = 1f;
            _pauseScreen.interactable = true;
            //Debug.Log(pauseStartTime);
        }
        // Untoggle Pause
        else if (!check)
        {
            StartCoroutine(TurnOffPause());
        }
    }
    /// <summary>
    /// Untoggle pause. Return player in the game, plays music from the pause and start coroutines.
    /// </summary>
    private IEnumerator TurnOffPause()
    {
        // Soft transition to gameplay
        float elapsedTime = 0f;
        float startAlpha = _pauseScreen.alpha;
        while (elapsedTime < DELAY_AFTER_PAUSE)
        {
            float t = elapsedTime / DELAY_AFTER_PAUSE;
            _pauseScreen.alpha = Mathf.Lerp(startAlpha, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Lock the pause screen
        _pauseScreen.alpha = 0f;
        _pauseScreen.interactable = false;
        // Make the board interactable, uncheck the bool variables
        _board.SetInputLock(false);
        _isPaused = false;
        _isUnpausing = false;

        MusicPlay();

        // Start all the coroutines
        if (!_isSongEnding)
        {
            _decreaseLife = StartCoroutine(DecreaseLifeCoroutine());
            _borderOutlineBacklight = StartCoroutine(SingleBorderOutlineBacklight());
            foreach (GameObject note in _noteObjects)
            {
                note.GetComponent<NoteScript>().SetLocked(false);
            }
        }

        TimeManagerGlobal.IncreasePauseTime(Time.time - _pauseStartTime);
        _pauseStartTime = 0f;
        //Debug.Log(TimeManagerGlobal.GetPauseTime());
        _pauseButton.gameObject.SetActive(true);
    }
    /// <summary>
    /// Unpausing state.
    /// </summary>
    public void SetUnpause()
    {
        _isUnpausing = true;
        TogglePause(false);
    }
    /// <summary>
    /// On pause button.
    /// </summary>
    public void OnPauseButton()
    {
        _sfxClick.Play();
        _isPaused = true;
        TogglePause(true);
    }
    /// <summary>
    /// Set IsEnding state.
    /// </summary>
    /// <param name="state">State to set.</param>
    public void SetIsEnding(bool state)
    {
        _isEnding = state;
    }
}

/// <summary>
/// Time manager to count ingame time on the board, not the actual real time.
/// Useful so that the game will actually work.
/// </summary>
public class TimeManager
{
    private float globalTime;
    private float startTime;
    private float pauseTime = 0f;

    /// <summary>
    /// Sets start time as current Time.time.
    /// </summary>
    public void InitializeStartTime()
    {
        startTime = Time.time;
    }
    /// <summary>
    /// Sets global time as the difference between the current Time.time and startTime
    /// </summary>
    public void InitializeGlobalTime()
    {
        globalTime = Time.time - startTime;
    }
    /// <summary>
    /// Return start time.
    /// </summary>
    public float GetStartTime()
    {
        return startTime;
    }
    /// <summary>
    /// Returns global time.
    /// </summary>
    public float GetGlobalTime()
    {
        return globalTime;
    }
    /// <summary>
    /// Returns pause time.
    /// </summary>
    public float GetPauseTime()
    {
        return pauseTime;
    }
    /// <summary>
    /// Sets new value to global time.
    /// </summary>
    /// <param name="value">Value to set.</param>
    public void SetGlobalTime(float value)
    {
        globalTime = value;
    }
    /// <summary>
    /// Increases global time. 
    /// It is the difference between current Time.time and startTime with pauseTime
    /// </summary>
    public void IncreaseGlobalTime()
    {
        globalTime = Time.time - startTime - pauseTime;
    }
    /// <summary>
    /// Increases pause time by amount.
    /// </summary>
    /// <param name="time">Value to increase pause time by.</param>
    public void IncreasePauseTime(float time)
    {
        pauseTime += time;
    }
    /// <summary>
    /// Compare global time with the value. 
    /// Returns true, if global time >= value. Otherwise returns false.
    /// </summary>
    /// <param name="value">Value to compare time to.</param>
    public bool CompareTimeGE(float value)
    {
        if (Mathf.Approximately(globalTime, value) || globalTime >= value)
        {
            return true;
        }

        return false;
    }
}
