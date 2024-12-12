using System.Collections;
using System.Collections.Generic;
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

}
