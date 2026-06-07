using System.Collections;
using UnityEngine;

public class MonsterBoss : Monster
{
    private Coroutine _patternLoopCoroutine;
    private bool _isBossAlive = false;

    // 패턴 3에서 사용할 실시간 회전 각도 (Update 대신 패턴 함수 내부에서 소화)
    private float _machineGunAngle = 0f;

    private void Start()
    {
        //  테스트용: 보스가 생성되거나 배틀이 시작될 때 구동
        StartPatternLoop();
    }

    private void OnDisable()
    {
        StopPatternLoop();
    }

    public void StartPatternLoop()
    {
        _isBossAlive = true;
        if (_patternLoopCoroutine != null) StopCoroutine(_patternLoopCoroutine);

        //  메인 루프 코루틴 가동!
        _patternLoopCoroutine = StartCoroutine(BossPatternLoopCo());
    }

    public void StopPatternLoop()
    {
        _isBossAlive = false;
        if (_patternLoopCoroutine != null)
        {
            StopCoroutine(_patternLoopCoroutine);
            _patternLoopCoroutine = null;
        }
    }

    /// <summary>
    ///  [총지휘관] 5초마다 주사위를 굴려 랜덤 패턴을 무한 실행하는 코루틴
    /// </summary>
    private IEnumerator BossPatternLoopCo()
    {
        // 배틀 시작 후 보스가 완전히 자리 잡을 수 있도록 첫 1초는 대기해주는 센스!
        yield return new WaitForSeconds(1.0f);

        while (_isBossAlive)
        {
            // 0, 1, 2 중 하나의 숫자를 랜덤으로 뽑습니다.
            int randomPattern = Random.Range(0, 4);

            switch (randomPattern)
            {
                case 0:
                    StartCoroutine(PlayCirclePatternCo());
                    break;
                case 1:
                    StartCoroutine(PlayFanPatternCo());
                    break;
                case 2:
                    //  머신건은 '지속 시간'이 필요한 연출이므로, 얘만 별도 코루틴으로 실행합니다.
                    yield return StartCoroutine(PlayMachineGunPatternCo(3.0f));
                    break;
                case 3:
                    StartCoroutine(PlayMonsterWavePatternCo(5.0f));
                    break;
            }

            //  패턴을 한 번 시원하게 쏟아부은 후, 정확히 5초 동안 쉬고 다음 주사위를 굴립니다.
            yield return new WaitForSeconds(5.0f);
        }
    }
    private IEnumerator PlayMonsterWavePatternCo(float duration) // duration = 5.0f
    {
       
        StageManager.Inst.ResumeSpawners();
        yield return new WaitForSeconds(duration);
        StageManager.Inst.StopSpawners();
    }

    // ================= [ 가벼운 반복문 패턴 함수 3종 세트 ] =================

    /// <summary>
    ///  패턴 1: 보스 중심으로 사방(16방향)으로 퍼지는 단발성 방사형 탄막
    /// </summary>
    private IEnumerator PlayCirclePatternCo()
    {
        Debug.Log(" [보스 패턴] 방사형 서클탄 5연사 발사!");
        int bulletCount = 16;
        float angleStep = 360f / bulletCount;

        for (int j = 0; j < 3; j++)
        {
            //  [연출 꿀팁] 매 웨이브마다 살짝씩 각도를 틀어주면(예: j * 5도)
            // 총알들이 소용돌이치며 엇갈려 나가기 때문에 회피 액션감이 2배가 됩니다!
            float startAngle = j * 5f;

            for (int i = 0; i < bulletCount; i++)
            {
                float currentAngle = startAngle + (i * angleStep);
                float rad = currentAngle * Mathf.Deg2Rad;
                Vector2 fireDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                StageManager.Inst.StartFireBulletForBoss(transform.position, fireDirection);
            }

            //  한 파동(16발)을 쏘고 나서 0.3초씩 쉬어줍니다.
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    ///  패턴 2: 플레이어를 향해 5방향 부채꼴로 흩뿌리는 조준형 확산 탄막
    /// </summary>
    private IEnumerator PlayFanPatternCo()
    {
        if (Player.Inst == null) yield break;
        Debug.Log(" [보스 패턴] 조준 확산탄 5연사 발사!");

        int bulletCount = 5;
        float fanAngle = 45f;

        // 5번 연속 발사 루프
        for (int j = 0; j < 10; j++)
        {
            //  [꿀팁] 매 발사 때마다 플레이어의 실시간 위치를 새로 갱신하면 
            // 플레이어가 도망쳐도 끝까지 쫓아가며 조준 사격을 합니다!
            Vector2 dirToPlayer = (Player.Inst.transform.position - transform.position).normalized;
            float baseAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

            float startAngle = baseAngle - (fanAngle / 2f);
            float angleStep = fanAngle / (bulletCount - 1);

            // 5방향 확산탄 뿜기
            for (int i = 0; i < bulletCount; i++)
            {
                float currentAngle = startAngle + (i * angleStep);
                float rad = currentAngle * Mathf.Deg2Rad;
                Vector2 fireDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                StageManager.Inst.StartFireBulletForBoss(transform.position, fireDirection);
            }

            //  한 번 뿜고 나서 0.2초씩 쉬어줍니다. (두두두두둥 소리 나게!)
            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    ///  패턴 3: 지정된 시간(duration) 동안 0.05초 간격으로 회전하며 총알을 난사하는 개틀링 코루틴
    /// </summary>
    private IEnumerator PlayMachineGunPatternCo(float duration)
    {
        Debug.Log($" [보스 패턴] 개틀링 회전 난사 시작! ({duration}초간 유지)");
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            _machineGunAngle += 20f; // 매 발사마다 20도씩 회전
            if (_machineGunAngle >= 360f) _machineGunAngle -= 360f;

            for (int i = 0; i < 2; i++)
            {
                float finalAngle = _machineGunAngle + (i * 180f);
                float rad = finalAngle * Mathf.Deg2Rad;
                Vector2 fireDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                StageManager.Inst.StartFireBulletForBoss(transform.position, fireDirection);
            }

            // 0.05초 쉬고 다음 줄기 발사 (매우 가볍게 시간만 쪼갭니다)
            yield return new WaitForSeconds(0.05f);
            elapsedTime += 0.05f;
        }
        Debug.Log(" 개틀링 난사 종료.");
    }

}
