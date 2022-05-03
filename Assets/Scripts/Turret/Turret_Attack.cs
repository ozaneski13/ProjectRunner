using UnityEngine;

[RequireComponent(typeof(Turret))]
public class Turret_Attack : Turret_Control
{
    [Header("Fire Bullet")]
    [SerializeField] private GameObject _bulletPrefab = null;

    private AttackerTurret _attackerTurret = null;

    private float _fireRate = 0f;

    private void Start()
    {
        _attackerTurret = _turret as AttackerTurret;

        _fireRate = _attackerTurret.FireRate;
        _fireCountdown = 1f / _fireRate;
    }

    private void Update()
    {
        if (_target == null)
        {
            TurnToStartingRotation();

            return;
        }

        RotateToEnemy();
        
        if (_fireCountdown <= 0f)
        {
            FireBullet();
            
            _fireCountdown = 1f / _fireRate;
        }
        
        _fireCountdown -= Time.deltaTime;
    }

    private void FireBullet()
    {
        int cannonCount = _attackerTurret.CannonCount;

        for (int i = 0; i < cannonCount; i++)
        {
            GameObject bullet = Instantiate(_bulletPrefab, _firePoints[i].position, _firePoints[cannonCount - 1].rotation, AmmunitionHolder.Instance.transform);
            bullet.GetComponent<Ammunition>().SetTarget(_target);
        }
    }
}