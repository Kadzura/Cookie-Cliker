using System.Collections;
using System.Collections.Generic;
// Подключаем необходимые библиотеки
using UnityEngine; // Библиотека Unity для работы со сценой и объектами в ней
using UnityEngine.UI; // Библиотека Unity для работы с интерфейсами пользователя
public class ScoreKeeper : MonoBehaviour // Объявляем класс ScoreKeeper, производный от MonoBehaviour
{
    // Переменные, доступные для настройки в редакторе Unity
    public GameObject SemitransparentPrefab; // Префаб объекта Semitransparent
    public GameObject Granny; // Объект Granny
    public AudioClip ScorePlusSound; // Звук добавления очков
    public AudioClip ScoreMinusSound; // Звук уменьшения очков
    public AudioClip LevelPassedSound; // Звук перехода на новый уровень
    public AudioClip WinningTheGameSound; // Звук победы
    public int ScoreSpeedUpStep = 20; // Шаг увеличения прибавки к очкам
    public int ScoreToWin1 = 100; // счёт до конца уровня 1
    public int ScoreToWin2 = 200; // счёт до конца уровня 2
    public int ScoreToWin3 = 300; // счёт до конца уровня 3 и всей игры
    public float GrannyTime = 5f; // Время, через которое Granny становится невидимой
    public float InvisibleTime = 5f; // Время, через которое Granny снова становится видимой

    // Приватные переменные, которые используются логикой игры
    private AudioSource _audioSource; // ? Фоновая музыка
    private int _level = 1; // Текущий уровень игрока
    private Text _levelText; // Текстовое поле для отображения уровня
    private int _score = 0; // Текущее количество очков игрока
    private int _scoreIncrement = 0; // Прибавка к очкам за клик
    private int _lastScoreStep = 0; // Последнее количество очков, при котором произошло увеличение прибавки
    private int _counter = 0; // Количество кликов
    private Text _scoreText; // Текстовое поле для отображения количества очков
    private Text _scorePerClickText; // Текстовое поле для отображения прибавки к очкам за клик
    private SpriteRenderer _grannySpriteRenderer; // Компонент SpriteRenderer для Granny
    public bool _grannyVisible = false; // Флаг видимости Granny
    private float _timer = 0; // Таймер для отслеживания времени
    private bool _timerEnabled = false; // Флаг, который указывает, включён ли таймер
    private Vector3 _grannyOriginalPosition; // Позиция бабушки

    private void Start() // Метод Start() вызывается при старте игры
    {
        FillComponents(); // Выполняем метод FillComponents()
        InitValues(); // Выполняем метод InitValues()
    }
    private void FillComponents() // Метод для заполнения компонентов (может пригодиться и вне Start())
    {
        // Находим и присваиваем необходимые компоненты и значение свойств
        _levelText = GameObject.Find("LevelText").GetComponent<Text>(); // Находим текстовое поле LevelText и получаем его компонент Text
        _scoreText = GameObject.Find("ScoreText").GetComponent<Text>(); // Находим текстовое поле ScoreText и получаем его компонент Text
        _scorePerClickText = GameObject.Find("ScorePerClickText").GetComponent<Text>(); // Находим текстовое поле ScorePerClickText и получаем его компонент Text
        _grannySpriteRenderer = Granny.GetComponent<SpriteRenderer>(); // Получаем компонент SpriteRenderer из объекта Granny
        _audioSource = gameObject.GetComponent<AudioSource>(); // 
    }
    private void InitValues() // Метод для заполнения начальных значений переменных (может пригодиться и вне Start())
    {
        _score = 0; // Обнуляем значение очков
        _scoreIncrement = 0;  // Обнуляем значение, на которое будут увеличиваться очки
        _lastScoreStep = 0; // Обнуляем счётчик очков, который увеличивает «стоимость» одного клика, когда игрок набирает, например, 20, 40, 60 и более очков
        _counter = 0; // Обнуляем счётчик кликов по печеньке
        _scoreText.text = "Очки:"; // Устанавливаем стартовое значение текстового поля ScoreText
        _scorePerClickText.text = ""; // Очищаем текстовое поле ScorePerClickText
        _grannySpriteRenderer.enabled = false; // Выключаем отображение Granny
        _grannyOriginalPosition = Granny.transform.localPosition; // Записываем позицию бабушки
    }
    private void Update() // Метод Update() вызывается на каждом кадре игры
    {
        TimerTick(); // Выполняем метод TimerTick()
        CheckRightClick(); // Проверка ПКМ
        // Передвижение бабушки
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
                SubScore("Правый клик на уровне 2", 15);
            }
            else if (_level == 3)
            {
                SubScore("Правый клик на уровне 3", 30);
            }
        }
    }
    private void SubScore(string reason, int number)
    {
        Debug.Log(reason + ": -" + number + " очков!");
        _score -= number;
        _scoreText.text = "Очки: " + _score;
        _audioSource.PlayOneShot(ScoreMinusSound);
    }
    private void TimerTick() // Метод для отсчёта времени (может пригодиться и вне Update())
    {
        if (!_timerEnabled) // Если таймер не включен
        {
            return; // Команда return выходит из метода, в котором написана (в данном случае, из TimerTick())
        }
        _timer += Time.deltaTime; // Увеличиваем значение таймера на время, прошедшее с предыдущего кадра
        TrySetActiveGranny(); // Выполняем метод TrySetActiveGranny()
    }
    private void TrySetActiveGranny() // Метод для проверки условий для показа бабушки
    {
        if (_grannyVisible && _timer >= GrannyTime) // Если бабушка видима и таймер достиг заданного времени
        {
            SetActiveGranny(false); // Скрываем бабушку
        }
        else if (!_grannyVisible && _timer >= InvisibleTime) // Если бабушка скрыта и таймер достиг заданного времени
        {
            SetActiveGranny(true); // Показываем бабушку
        }
    }
    private void SetActiveGranny(bool value) // Метод для показа и скрытия бабушки по значению value
    {
        _grannyVisible = value; // Устанавливаем флаг видимости бабушки в заданное значение
        _grannySpriteRenderer.enabled = value; // Включаем или выключаем отображение спрайта бабушки в зависимости от значения флага видимости
        ResetTimer(); // Сбрасываем таймер
    }
    private void ResetTimer() // Метод для установки таймеру значения 0
    {
        _timer = 0; // Сбрасываем таймер
    }
    public void OnMouseDown() // Метод для обработки левого клика по печеньке
    {
        if (_grannyVisible && _level == 1) // Если бабушка видима, то ничего не делаем
        {
            return; // Команда return выходит из метода, в котором написана (в данном случае, из OnMouseDown())
        }
        CheckGrannyClick(); // Штраф за действия
        NewLine(); // Пытаемся добавить новую строку в поле увеличения очков за один клик
        TrySpawnCookie(); // Пытаемся создать новую печеньку
        StartGrannyLogic(); // Пытаемся запустить логику бабушки
        // Уменьшаем размер исходного объекта, на который добавлен скрипт, (печеньки) со 100% до 80%
        transform.localScale *= 0.8f;
        // Командой Invoke() вызываем метод EnlargeCookie(), который увеличит размер печеньки через 0.1 секунду
        Invoke(nameof(EnlargeCookie), 0.1f);
    }
    private void EnlargeCookie()
    {
        // Увеличиваем размер печеньки до исходного значения
        transform.localScale /= 0.8f;
    }
    private void CheckGrannyClick()
    {
        if (_grannyVisible && _level == 2)
        {
            SubScore("Клик при бабушке на уровне 2", 10);
        }
        else if (_grannyVisible && _level == 3)
        {
            SubScore("Клик при бабушке на уровне 3", 20);
        }
    }
    private void NewLine() // Метод для перехода на новую строку в поле увеличения очков за один клик
    {
        if (_counter % 20 == 0 && _counter != 0) // Если счётчик делится на 20 без остатка и не равен 0
        {
            _scorePerClickText.text += "\n"; // Добавляем новую строку в поле увеличения очков за один клик
        }
    }
    private void TrySpawnCookie() // Метод для попытки добавить очки и создать новый префаб печеньки
    {
        if (_score < ScoreToWin3) // Если количество очков меньше количества, необходимого для победы
        {
            IncreaseScoreIncrement(); // Увеличиваем количество очков за один клик
            AddScore(); // Добавляем очки
            SpawnCookie(); // Создаём новый префаб печеньки
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
            _scoreText.text = "Игра закончена, вы победили!\nКоличество очков: " + _score + "\nКоличество кликов: " + _counter;
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
        _levelText.text = "Уровень " + number;
        _level = number;
        _audioSource.PlayOneShot(LevelPassedSound);
        // Увеличиваем размер надписи _levelText со 100% до 150%
        _levelText.transform.localScale *= 1.5f;
        // Командой Invoke() вызываем метод ResetLevelTextSize(), который уменьшит размер надписи через 0.3 секунды
        Invoke(nameof(ResetLevelTextSize), 0.3f);
    }
    private void ResetLevelTextSize()
    {
        // Уменьшаем размер надписи до исходного значения
        _levelText.transform.localScale /= 1.5f;
    }
    private void IncreaseScoreIncrement() // Метод для увеличения количества очков за клик
    {
        if (_score >= _lastScoreStep) // Если игрок достиг нужного количества очков для того, чтобы «стоимость» одного клика была увеличена, или превысил его
        {
            _scoreIncrement++; // Увеличиваем количество очков за один клик
            _lastScoreStep += ScoreSpeedUpStep; // Обновляем счётчик очков, который увеличивает «стоимость» одного клика, когда игрок набирает, например, 20, 40, 60 и более очков
        }
    }
    private void AddScore() // Метод для добавления очков
    {
        _score += _scoreIncrement; // Увеличиваем общее количество очков на количество очков за один клик
        _scoreText.text = "Очки: " + _score;
        if (_grannyVisible) // Обновляем текстовое поле для отображения количества очков
        {
            _scorePerClickText.text = " - " + _scoreIncrement;
        }
        else
        {
            _scorePerClickText.text = " + " + _scoreIncrement; 
        }
        _counter++; // Увеличиваем счётчик кликов
        _audioSource.PlayOneShot(ScorePlusSound);
    }
    private void SpawnCookie() // Метод для создания нового префаба печеньки
    {
        float randomX = Random.Range(-10f, 10f); // Получаем случайное значение X в заданном диапазоне
        float randomY = Random.Range(-1f, 1f); // Получаем случайное значение Y в заданном диапазоне
        Instantiate(SemitransparentPrefab, new Vector3(randomX, randomY, 0), Quaternion.identity); // Создаем новый экземпляр префаба печеньки в случайном месте на экране
    }
    private void StartGrannyLogic() // Метод для появления бабушки в игре
    {
        if (_score >= 20 && !_timerEnabled) // Если количество очков больше или равно 20 и таймер не активирован
        {
            SetActiveGranny(true); // Показываем бабушку
            _timerEnabled = true; // Активируем таймер
        }
    }
}