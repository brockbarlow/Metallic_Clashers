﻿using System.Collections.Generic;

using Combat;
using Combat.Board;

using UnityEngine;

public class Enemy
{
    public Attribute health = new Attribute();
    public Attribute attack = new Attribute();
    public Attribute defense = new Attribute();

    public float attackSpeed;
    public int movesUntilAttack;
    public GemType damageType;

    public List<GemType> resistances;
    public List<GemType> weaknesses;

    private int movesCounter = 0;
    private float attackCountdown;

    public Enemy()
    {
        CombatManager.self.onCombatBegin.AddListener(OnCombatBegin);
        CombatManager.self.onPlayerTurn.AddListener(OnPlayerTurn);
        CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
        CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
    }

    public Enemy(float pHealth, float pAttack, float pDefense, float pattackSpeed, int pmovesUntilAttack)
    {
        CombatManager.self.onCombatBegin.AddListener(OnCombatBegin);
        CombatManager.self.onPlayerTurn.AddListener(OnPlayerTurn);
        CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
        CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);

        health.value = pHealth;
        health.coefficient = 1;

        attack.value = pAttack;
        attack.coefficient = 1;

        defense.value = pDefense;
        defense.coefficient = 1;

        attackSpeed = pattackSpeed;
        attackCountdown = attackSpeed;

        movesUntilAttack = pmovesUntilAttack;

    }

    private void OnCombatBegin()
    {
    }

    private void OnCombatUpdate()
    {
        attackCountdown -= Time.deltaTime;

        if (attackCountdown > 0) return;
        Attack();
        attackCountdown = attackSpeed;
    }

    private void OnCombatEnd()
    {
        //TODO: Define what the enemy should do here. Possibly Nothing.
    }

    private void OnPlayerTurn()
    {
        movesCounter++;
        if (movesCounter != 0 && movesCounter % movesUntilAttack == 0)
        {
            Attack();
        }
    }

    private void Attack()
    {
        GameManager.self.playerData.TakeDamage(attack.totalValue, damageType);
    }

    public void TakeDamage(float damage, GemType gemType)
    {
        var percentage = damage / defense.totalValue;
        var finalDamage = damage * Mathf.Clamp(percentage, 0f, 1f);

        if (resistances.Contains(gemType))
        {
            finalDamage *= .75f;
        }

        else if (weaknesses.Contains(gemType))
        {
            finalDamage *= 1.25f;
        }

        health.modifier -= finalDamage;
    }
}
