using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Point")]
    [SerializeField] private Transform _spawnPoint = null;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _countdownText = null;

    [Header("Wave Options")]
    [SerializeField] private int _maxWaveNumber = 100;

    [SerializeField] private float _timeBetweenWaves = 5f;
    [SerializeField] private float _timeBetweenEachSpawn = 1f;
    [SerializeField] private float _lateStartTimer = 0f;

    [SerializeField] private bool _startsLate = false;

    [Header("Route")]
    [SerializeField] private int _defaultWay = 0;

    private LevelSettingsSO _levelSettingsSO = null;
    private LevelSettings _levelSettings = null;

    private List<GameObject> _enemyTypesList = new List<GameObject>();
    private List<GameObject> _enemyBossTypesList = new List<GameObject>();

    private Transform _enemyParent = null;

    private List<GameObject[]> _waves = null;

    private float _countdown = 5f;

    private int _waveNumber = 0;

    private bool _currentWaveSpawned = true;
    private bool _isInvoked = false;

    public Action AllWavesSpawned;

    private void Start()
    {
        _waves = new List<GameObject[]>();

        _enemyParent = EnemyHolder.Instance.transform;

        _levelSettingsSO = LevelSettingsHolder.Instance.LevelSettingsSO;
        _levelSettings = _levelSettingsSO.GetLevelSettingsByLevelID(SceneManager.GetActiveScene().buildIndex);

        _maxWaveNumber = _levelSettings.WaveCount;
        _enemyTypesList = _levelSettings.Enemies;
        _enemyBossTypesList = _levelSettings.Bosses;
    }

    private void Update()
    {
        if (_countdown <= 0f && _waveNumber < _maxWaveNumber && _currentWaveSpawned)
            StartCoroutine(SpawnWave());

        else if (_waveNumber >= _maxWaveNumber)
        {
            _countdown = 00.0f;

            if (_countdownText != null)
                _countdownText.text = string.Format("{0:00.0}", _countdown);

            StopAllCoroutines();

            if (GetWaveStatus() == _maxWaveNumber && !_isInvoked)
            {
                _isInvoked = true;
                AllWavesSpawned?.Invoke();
            }

            return;
        }

        if (_countdown > 0f)
            _countdown -= Time.deltaTime;

        _countdown = Mathf.Clamp(_countdown, 00.0f, Mathf.Infinity);

        if (_countdownText != null)
            _countdownText.text = string.Format("{0:00.0}", _countdown);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnWave()
    {
        _currentWaveSpawned = false;

        if (_startsLate)
            yield return new WaitForSeconds(_lateStartTimer);

        GameObject[] enemies = new GameObject[_waveNumber + 1];

        for (int i = 0; i < _waveNumber + 1; i++)
        {
            enemies[i] = SpawnEnemy();
            
            yield return new WaitForSeconds(_timeBetweenEachSpawn);
        }

        if (_enemyBossTypesList.Count != 0)
            StartCoroutine(CheckBossWave());

        _waves.Add(enemies);

        _waveNumber++;

        _currentWaveSpawned = true;

        if (_waveNumber == _maxWaveNumber)
            _countdown = 00.0f;
        else
            _countdown = _timeBetweenWaves;

        yield return new WaitForSeconds(_timeBetweenWaves);
    }

    private IEnumerator CheckBossWave()
    {
        int bossCount = 0;

        if (_waveNumber == _maxWaveNumber)
            bossCount = 16;
        else if (_waveNumber == _maxWaveNumber / 2)
            bossCount = 8;
        if (_waveNumber % 10 == 0 && _waveNumber != 0)
            bossCount = 4;

        for (int i = 0; i < bossCount; i++)
        {
            if (i < bossCount / (float)2)
                Instantiate(_enemyBossTypesList[0], _enemyParent);
            else
                Instantiate(_enemyBossTypesList[1], _enemyParent);

            yield return new WaitForSeconds(_timeBetweenEachSpawn * 2);
        }
    }

    private GameObject SpawnEnemy()
    {
        int enemyTypeIndex = UnityEngine.Random.Range(0, _enemyTypesList.Count);

        GameObject enemy = Instantiate(_enemyTypesList[enemyTypeIndex], _enemyParent);
        enemy.transform.position = _spawnPoint.position;
        enemy.GetComponent<Enemy_Movement>().SetDefaultWay(_defaultWay);

        return enemy;
    }

    public int GetWaveStatus()
    {
        int finishedWaves = 0;

        foreach(GameObject[] enemies in _waves)
        {
            int enemyCounter = 0;

            foreach (GameObject enemy in enemies)
            {
                if (enemy == null)
                    enemyCounter++;
            }

            if (enemyCounter == enemies.Length)
            {
                finishedWaves++;

                int finishedWaveCount = PlayerPrefs.GetInt("finishedWaves", 0);
                finishedWaveCount++;
                PlayerPrefs.SetInt("finishedWaves", finishedWaveCount);
            }
        }

        return finishedWaves;
    }
}