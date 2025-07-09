using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "LightningStrikeGimmickSO", menuName = "StageGimmick/LightningStrike")]
public class LightningStrikeGimmickSO : StageGimmickSO
{
    public GameObject lightningStrikePrefab;
    public GameObject warningPrefab;
    public Vector3 spawnOffset = new Vector3(0, 5, 0);
    public float duration = 10f;
    public float interval = 5f;
    public int lightningPerWave = 3;
    public float spawnRadius = 5f;
    public float warningDelay = 1f;

    public override GameObject Execute(Vector3 origin)
    {
        if (lightningStrikePrefab == null)
        {
            Debug.LogWarning("[낙뢰기믹] 프리팹이 비어 있음");
            return null;
        }

        LightningRunner runner = new GameObject("LightningRunner").AddComponent<LightningRunner>();
        runner.StartCoroutine(SpawnLightningRoutine(runner));
        return runner.gameObject;
    }

    private IEnumerator SpawnLightningRoutine(MonoBehaviour runner)
    {
        float elapsed = 0f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("[낙뢰기믹] Player 태그 오브젝트를 찾을 수 없음");
            yield break;
        }

        while (elapsed < duration)
        {
            Vector3 playerPos = player.transform.position;

            for (int i = 0; i < lightningPerWave; i++)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius)
                );
                Vector3 spawnPos = playerPos + randomOffset + spawnOffset;

                // 예고 마커 생성
                if (warningPrefab != null)
                {
                    GameObject warning = GameObject.Instantiate(warningPrefab, spawnPos - spawnOffset, Quaternion.identity);
                    GameObject.Destroy(warning, warningDelay + 0.5f);
                }

                // 일정 시간 뒤 낙뢰 생성
                runner.StartCoroutine(DelayedStrike(spawnPos));
            }

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        GameObject.Destroy(runner.gameObject);
    }

    private IEnumerator DelayedStrike(Vector3 pos)
    {
        yield return new WaitForSeconds(warningDelay);

        GameObject lightning = GameObject.Instantiate(lightningStrikePrefab, pos, Quaternion.identity);
        GameObject.Destroy(lightning, 2f);
    }
}
