using System.Collections;
using System.Collections.Generic;
// ���������� ����������� ����������
using UnityEngine; // ���������� Unity ��� ������ �� ������ � ��������� � ���
using UnityEngine.UI; // ���������� Unity ��� ������ � ������������ ������������
public class ScoreKeeper : MonoBehaviour // ��������� ����� ScoreKeeper, ����������� �� MonoBehaviour
{
    // ����������, ��������� ��� ��������� � ��������� Unity
    public GameObject SemitransparentPrefab; // ������ ������� Semitransparent
    public GameObject Granny; // ������ Granny
    public AudioClip ScorePlusSound; // ���� ���������� �����
    public AudioClip ScoreMinusSound; // ���� ���������� �����
    public AudioClip LevelPassedSound; // ���� �������� �� ����� �������
    public AudioClip WinningTheGameSound; // ���� ������
    public int ScoreSpeedUpStep = 20; // ��� ���������� �������� � �����
    public int ScoreToWin1 = 100; // ���� �� ����� ������ 1
    public int ScoreToWin2 = 200; // ���� �� ����� ������ 2
    public int ScoreToWin3 = 300; // ���� �� ����� ������ 3 � ���� ����
    public float GrannyTime = 5f; // �����, ����� ������� Granny ���������� ���������
    public float InvisibleTime = 5f; // �����, ����� ������� Granny ����� ���������� �������

    // ��������� ����������, ������� ������������ ������� ����
    private AudioSource _audioSource; // ? ������� ������
    private int _level = 1; // ������� ������� ������
    private Text _levelText; // ��������� ���� ��� ����������� ������
    private int _score = 0; // ������� ���������� ����� ������
    private int _scoreIncrement = 0; // �������� � ����� �� ����
    private int _lastScoreStep = 0; // ��������� ���������� �����, ��� ������� ��������� ���������� ��������
    private int _counter = 0; // ���������� ������
    private Text _scoreText; // ��������� ���� ��� ����������� ���������� �����
    private Text _scorePerClickText; // ��������� ���� ��� ����������� �������� � ����� �� ����
    private SpriteRenderer _grannySpriteRenderer; // ��������� SpriteRenderer ��� Granny
    public bool _grannyVisible = false; // ���� ��������� Granny
    private float _timer = 0; // ������ ��� ������������ �������
    private bool _timerEnabled = false; // ����, ������� ���������, ������� �� ������
    private Vector3 _grannyOriginalPosition; // ������� �������

    private void Start() // ����� Start() ���������� ��� ������ ����
    {
        FillComponents(); // ��������� ����� FillComponents()
        InitValues(); // ��������� ����� InitValues()
    }
    private void FillComponents() // ����� ��� ���������� ����������� (����� ����������� � ��� Start())
    {
        // ������� � ����������� ����������� ���������� � �������� �������
        _levelText = GameObject.Find("LevelText").GetComponent<Text>(); // ������� ��������� ���� LevelText � �������� ��� ��������� Text
        _scoreText = GameObject.Find("ScoreText").GetComponent<Text>(); // ������� ��������� ���� ScoreText � �������� ��� ��������� Text
        _scorePerClickText = GameObject.Find("ScorePerClickText").GetComponent<Text>(); // ������� ��������� ���� ScorePerClickText � �������� ��� ��������� Text
        _grannySpriteRenderer = Granny.GetComponent<SpriteRenderer>(); // �������� ��������� SpriteRenderer �� ������� Granny
        _audioSource = gameObject.GetComponent<AudioSource>(); // 
    }
    private void InitValues() // ����� ��� ���������� ��������� �������� ���������� (����� ����������� � ��� Start())
    {
        _score = 0; // �������� �������� �����
        _scoreIncrement = 0;  // �������� ��������, �� ������� ����� ������������� ����
        _lastScoreStep = 0; // �������� ������� �����, ������� ����������� ����������� ������ �����, ����� ����� ��������, ��������, 20, 40, 60 � ����� �����
        _counter = 0; // �������� ������� ������ �� ��������
        _scoreText.text = "����:"; // ������������� ��������� �������� ���������� ���� ScoreText
        _scorePerClickText.text = ""; // ������� ��������� ���� ScorePerClickText
        _grannySpriteRenderer.enabled = false; // ��������� ����������� Granny
        _grannyOriginalPosition = Granny.transform.localPosition; // ���������� ������� �������
    }
    private void Update() // ����� Update() ���������� �� ������ ����� ����
    {
        TimerTick(); // ��������� ����� TimerTick()
        CheckRightClick(); // �������� ���
        // ������������ �������
        if (_grannyVisible)
        {
            float xPosition = _grannyOriginalPosition.x + Mathf.Sin(Time.time) * 0.5f;
            Granny.transform.localPosition = new Vector3(xPosition, Granny.transform.localPosition.y, Granny.transform.localPosition.z);
        }
    }
    private void CheckRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (_level == 2)
            {
                SubScore("������ ���� �� ������ 2", 15);
            }
            else if (_level == 3)
            {
                SubScore("������ ���� �� ������ 3", 30);
            }
        }
    }
    private void SubScore(string reason, int number)
    {
        Debug.Log(reason + ": -" + number + " �����!");
        _score -= number;
        _scoreText.text = "����: " + _score;
        _audioSource.PlayOneShot(ScoreMinusSound);
    }
    private void TimerTick() // ����� ��� ������� ������� (����� ����������� � ��� Update())
    {
        if (!_timerEnabled) // ���� ������ �� �������
        {
            return; // ������� return ������� �� ������, � ������� �������� (� ������ ������, �� TimerTick())
        }
        _timer += Time.deltaTime; // ����������� �������� ������� �� �����, ��������� � ����������� �����
        TrySetActiveGranny(); // ��������� ����� TrySetActiveGranny()
    }
    private void TrySetActiveGranny() // ����� ��� �������� ������� ��� ������ �������
    {
        if (_grannyVisible && _timer >= GrannyTime) // ���� ������� ������ � ������ ������ ��������� �������
        {
            SetActiveGranny(false); // �������� �������
        }
        else if (!_grannyVisible && _timer >= InvisibleTime) // ���� ������� ������ � ������ ������ ��������� �������
        {
            SetActiveGranny(true); // ���������� �������
        }
    }
    private void SetActiveGranny(bool value) // ����� ��� ������ � ������� ������� �� �������� value
    {
        _grannyVisible = value; // ������������� ���� ��������� ������� � �������� ��������
        _grannySpriteRenderer.enabled = value; // �������� ��� ��������� ����������� ������� ������� � ����������� �� �������� ����� ���������
        ResetTimer(); // ���������� ������
    }
    private void ResetTimer() // ����� ��� ��������� ������� �������� 0
    {
        _timer = 0; // ���������� ������
    }
    public void OnMouseDown() // ����� ��� ��������� ������ ����� �� ��������
    {
        if (_grannyVisible && _level == 1) // ���� ������� ������, �� ������ �� ������
        {
            return; // ������� return ������� �� ������, � ������� �������� (� ������ ������, �� OnMouseDown())
        }
        CheckGrannyClick(); // ����� �� ��������
        NewLine(); // �������� �������� ����� ������ � ���� ���������� ����� �� ���� ����
        TrySpawnCookie(); // �������� ������� ����� ��������
        StartGrannyLogic(); // �������� ��������� ������ �������
        // ��������� ������ ��������� �������, �� ������� �������� ������, (��������) �� 100% �� 80%
        transform.localScale *= 0.8f;
        // �������� Invoke() �������� ����� EnlargeCookie(), ������� �������� ������ �������� ����� 0.1 �������
        Invoke(nameof(EnlargeCookie), 0.1f);
    }
    private void EnlargeCookie()
    {
        // ����������� ������ �������� �� ��������� ��������
        transform.localScale /= 0.8f;
    }
    private void CheckGrannyClick()
    {
        if (_grannyVisible && _level == 2)
        {
            SubScore("���� ��� ������� �� ������ 2", 10);
        }
        else if (_grannyVisible && _level == 3)
        {
            SubScore("���� ��� ������� �� ������ 3", 20);
        }
    }
    private void NewLine() // ����� ��� �������� �� ����� ������ � ���� ���������� ����� �� ���� ����
    {
        if (_counter % 20 == 0 && _counter != 0) // ���� ������� ������� �� 20 ��� ������� � �� ����� 0
        {
            _scorePerClickText.text += "\n"; // ��������� ����� ������ � ���� ���������� ����� �� ���� ����
        }
    }
    private void TrySpawnCookie() // ����� ��� ������� �������� ���� � ������� ����� ������ ��������
    {
        if (_score < ScoreToWin3) // ���� ���������� ����� ������ ����������, ������������ ��� ������
        {
            IncreaseScoreIncrement(); // ����������� ���������� ����� �� ���� ����
            AddScore(); // ��������� ����
            SpawnCookie(); // ������ ����� ������ ��������
        }
        CheckLevel();
    }
    private void CheckLevel()
    {
        if (_score >= ScoreToWin1 && _score < ScoreToWin2)
        {
            ChangeLevel(2);
        }
        else if (_score >= ScoreToWin2 && _score < ScoreToWin3)
        {
            ChangeLevel(3);
        }
        else if (_score >= ScoreToWin3)
        {
            _scoreText.text = "���� ���������, �� ��������!\n���������� �����: " + _score + "\n���������� ������: " + _counter;
            _audioSource.PlayOneShot(WinningTheGameSound);
            Application.Quit();
        }
    }
    private void ChangeLevel(int number)
    {
        if (number == _level)
        {
            return;
        }
        _levelText.text = "������� " + number;
        _level = number;
        _audioSource.PlayOneShot(LevelPassedSound);
        // ����������� ������ ������� _levelText �� 100% �� 150%
        _levelText.transform.localScale *= 1.5f;
        // �������� Invoke() �������� ����� ResetLevelTextSize(), ������� �������� ������ ������� ����� 0.3 �������
        Invoke(nameof(ResetLevelTextSize), 0.3f);
    }
    private void ResetLevelTextSize()
    {
        // ��������� ������ ������� �� ��������� ��������
        _levelText.transform.localScale /= 1.5f;
    }
    private void IncreaseScoreIncrement() // ����� ��� ���������� ���������� ����� �� ����
    {
        if (_score >= _lastScoreStep) // ���� ����� ������ ������� ���������� ����� ��� ����, ����� ����������� ������ ����� ���� ���������, ��� �������� ���
        {
            _scoreIncrement++; // ����������� ���������� ����� �� ���� ����
            _lastScoreStep += ScoreSpeedUpStep; // ��������� ������� �����, ������� ����������� ����������� ������ �����, ����� ����� ��������, ��������, 20, 40, 60 � ����� �����
        }
    }
    private void AddScore() // ����� ��� ���������� �����
    {
        _score += _scoreIncrement; // ����������� ����� ���������� ����� �� ���������� ����� �� ���� ����
        _scoreText.text = "����: " + _score;
        if (_grannyVisible) // ��������� ��������� ���� ��� ����������� ���������� �����
        {
            _scorePerClickText.text = " - " + _scoreIncrement;
        }
        else
        {
            _scorePerClickText.text = " + " + _scoreIncrement; 
        }
        _counter++; // ����������� ������� ������
        _audioSource.PlayOneShot(ScorePlusSound);
    }
    private void SpawnCookie() // ����� ��� �������� ������ ������� ��������
    {
        float randomX = Random.Range(-10f, 10f); // �������� ��������� �������� X � �������� ���������
        float randomY = Random.Range(-1f, 1f); // �������� ��������� �������� Y � �������� ���������
        Instantiate(SemitransparentPrefab, new Vector3(randomX, randomY, 0), Quaternion.identity); // ������� ����� ��������� ������� �������� � ��������� ����� �� ������
    }
    private void StartGrannyLogic() // ����� ��� ��������� ������� � ����
    {
        if (_score >= 20 && !_timerEnabled) // ���� ���������� ����� ������ ��� ����� 20 � ������ �� �����������
        {
            SetActiveGranny(true); // ���������� �������
            _timerEnabled = true; // ���������� ������
        }
    }
}