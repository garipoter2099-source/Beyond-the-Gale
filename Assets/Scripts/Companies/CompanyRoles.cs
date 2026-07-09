using UnityEngine;
using System.Collections.Generic;

public enum PlayerRankType
{
    Interceptor = 0,  // Новичок
    Senior = 1,        // Опытный
    Deputy = 2,         // Заместитель
    Chief = 3           // Главный
}

public static class CompanyRoles
{
    private static Dictionary<PlayerRankType, string> rankNames = new()
    {
        { PlayerRankType.Interceptor, "Interceptor" },
        { PlayerRankType.Senior, "Senior" },
        { PlayerRankType.Deputy, "Deputy" },
        { PlayerRankType.Chief, "Chief" }
    };
    
    private static Dictionary<PlayerRankType, string> rankDescriptions = new()
    {
        { PlayerRankType.Interceptor, "Basic tornado interceptor" },
        { PlayerRankType.Senior, "Experienced interceptor" },
        { PlayerRankType.Deputy, "Deputy - Can promote members" },
        { PlayerRankType.Chief, "Company Chief - Full control" }
    };
    
    /// <summary>
    /// Получает имя ранга
    /// </summary>
    public static string GetRankName(PlayerRankType rank)
    {
        return rankNames.TryGetValue(rank, out var name) ? name : "Unknown";
    }
    
    /// <summary>
    /// Получает описание ранга
    /// </summary>
    public static string GetRankDescription(PlayerRankType rank)
    {
        return rankDescriptions.TryGetValue(rank, out var desc) ? desc : "Unknown";
    }
    
    /// <summary>
    /// Проверяет, может ли ранг повышать других игроков
    /// </summary>
    public static bool CanPromote(PlayerRankType rank)
    {
        return rank >= PlayerRankType.Deputy;
    }
    
    /// <summary>
    /// Проверяет, может ли ранг демотировать других игроков
    /// </summary>
    public static bool CanDemote(PlayerRankType rank)
    {
        return rank >= PlayerRankType.Deputy;
    }
    
    /// <summary>
    /// Проверяет, может ли ранг приглашать игроков
    /// </summary>
    public static bool CanInvite(PlayerRankType rank)
    {
        return rank >= PlayerRankType.Senior;
    }
    
    /// <summary>
    /// Проверяет, может ли ранг выгонять игроков
    /// </summary>
    public static bool CanKick(PlayerRankType rank)
    {
        return rank >= PlayerRankType.Deputy;
    }
}
