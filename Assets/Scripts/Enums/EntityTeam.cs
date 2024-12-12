using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EntityTeam
{
    NoTeam,
    PlayerTeam,
    NeutralTeam,
    EnemyTeam,
}

public static class EntityTeamExtensions
{
    public static EntityTeam OppositeTeam(this EntityTeam entityTeam)
    {
        return entityTeam switch
        {
            EntityTeam.PlayerTeam => EntityTeam.EnemyTeam,
            EntityTeam.EnemyTeam => EntityTeam.PlayerTeam,
            EntityTeam.NeutralTeam => EntityTeam.NeutralTeam,
            _ => EntityTeam.NoTeam
        };
    }

    public static List<EntityClass> GetTeamMates(this EntityTeam entityTeam)
    {
        return entityTeam switch
        {
            EntityTeam.PlayerTeam => CombatManager.Instance.GetPlayers(),
            EntityTeam.EnemyTeam => CombatManager.Instance.GetEnemies(),
            EntityTeam.NeutralTeam => CombatManager.Instance.GetNeutral(),
            _ => new()
        };
    }
}
