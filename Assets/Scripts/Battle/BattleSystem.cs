using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance;
    
    [Header("Prefabs")] 
    public BattleZone zone;
    public BattleZoneUI zoneUI;

    [Header("InBattleCards")]
    public List<Card> persons = new();
    public List<Card> enemies = new();

    public readonly float RemoveCardDelay = 0.2f;
    
    public event Action<BattleSystem> DeleteBattle;
    
    public event Action<Vector3, Vector3> SetCanvas;
    public event Action<Card, Card> CreateAttackEffect;


    public bool preemptiveFlag;
    
    
    
    private readonly Random _random = new Random();
    public bool IsCardInBattle(Card card) => persons.Contains(card) || enemies.Contains(card);

    public bool IsEnemyNearby(Vector3 pos, float range)
    {
        return Vector3.Distance(zone.transform.position, pos) <= range;
    }

    private void Awake()
    {
        Instance = this;
        if (Instance != this)
        {
            Destroy(this);
        }
        
        preemptiveFlag = false;// 이거 수정해야함. (적 필드일 때, 나의 필드 일때)
    }
    
    public async UniTask Init(List<Card> oriPerson, List<Card> oriEnemy)
    {
        await UniTask.WaitForFixedUpdate();
        
        persons.AddRange(oriPerson);
        enemies.AddRange(oriEnemy);
        
        var data = zone.ResizeBackground(enemies.Count, persons.Count);
        
        SetCanvas?.Invoke(data.Item1, data.Item2);

        await TryStartBattle();
    }
    
    private async UniTask TryStartBattle()
    {
        await zone.ArrangeCard(persons, enemies);
        
        while (true)
        {
            Debug.Log($"🕛 { (preemptiveFlag ? "적" : "아군")} 턴 시작");

            var battleOver = preemptiveFlag
                    ? await Attack(enemies, persons)
                    : await Attack(persons, enemies);

            if (battleOver)
            {
                Debug.Log(preemptiveFlag ? "👿 적 승리" : "🎉 플레이어 승리");
                break;
            }

            preemptiveFlag = !preemptiveFlag;
        }
        
        await UniTask.Delay(300);
        
        EndBattle(preemptiveFlag);
    }
    
    private async UniTask<bool> Attack(List<Card> attackers, List<Card> targets)
    {
        var attackerIdx = _random.Next(0, attackers.Count);
        var targetIdx = _random.Next(0, targets.Count);

        if (attackerIdx >= attackers.Count || targetIdx >= targets.Count)
        {
            attackerIdx = 0;
            targetIdx = 0;
        }
        
        var attacker  = attackers[attackerIdx];
        var target = targets[targetIdx];
        
        int damage = BattleManager.Instance.CardBattles[attacker].GetDamage();
        
        CreateAttackEffect?.Invoke(attacker, target);
        
        bool isDead = await BattleManager.Instance.CardBattles[target].ReceiveDamage(damage);

        if (isDead)
        {
            Debug.Log($"🟥 {target.name} 파괴됨");
            await HandleRemove(targets, target);
        }
        else
        {
            await UniTask.Delay(200);
        }
        
        await UniTask.Delay(300);
        
        return targets.Count == 0;
    }

    
    private async UniTask HandleRemove(List<Card> group, Card card)
    {
        if (!group.Remove(card)) return;
        if(!BattleManager.Instance.CardBattles.Remove(card)) return;
        
        
        Debug.Log($"{card.cardData.cardType}");
        
        Debug.Log("Destroy");
        
        await UniTask.Delay(500);
        
        var cardPos = card.transform.position;
        
        Destroy(card.gameObject);
        Destroy(card);

        Instantiate(GameTableManager.Instance.smokeEffectPrefab, cardPos, Quaternion.identity);
    }

    private void RestoreCardComponents(List<Card> list, bool isEnemy)
    {
        list.RemoveAll(c => c == null);
        
        for (int i = 0; i < list.Count; i++)
        {
            var card = list[i];
            if(!isEnemy) card.GetComponent<CardDrag>().enabled = true;
            if(card.owningStack == null) continue;
            if (card.owningStack.GetComponent<StackRepulsion>() is { } stackRepulsion)
            {
                stackRepulsion.enabled = true;
            }

            if (isEnemy && i >= 1)
            {
                var randomStack = list[i].owningStack.GetRandomStackFromSameField();
                Debug.Log(randomStack);
                if (randomStack != null)
                {
                    StackManager.Instance.AddCardToStack(card, randomStack);
                }
            }
        }
    } 

    private void EndBattle(bool isEnemy)
    {
        Debug.Log("전투 종료");
        
        var list = isEnemy ? enemies : persons;
        RestoreCardComponents(list, isEnemy);
        
        DeleteBattle?.Invoke(this);
    }
}
