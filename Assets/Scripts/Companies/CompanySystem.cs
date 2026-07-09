using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

public class CompanySystem : NetworkBehaviour
{
    private static CompanySystem instance;
    
    // Словарь всех компаний на сервере
    private Dictionary<string, CompanyData> companies = new Dictionary<string, CompanyData>();
    
    // Словарь игроков в компаниях
    private Dictionary<ulong, string> playerCompanies = new Dictionary<ulong, string>();
    
    public static CompanySystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CompanySystem>();
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Создает новую компанию (Server-только)
    /// </summary>
    [Rpc(SendTo.Server)]
    public void CreateCompanyRpc(string companyName, ulong creatorId)
    {
        if (!IsServer) return;
        
        // Проверка баланса игрока
        var player = FindObjectOfType<PlayerDataManager>();
        if (player == null || player.GetBalance() < GameConstants.COMPANY_CREATION_COST)
        {
            Debug.LogWarning("Insufficient funds to create company");
            return;
        }
        
        // Создание компании
        string companyId = System.Guid.NewGuid().ToString();
        var newCompany = new CompanyData
        {
            CompanyId = companyId,
            CompanyName = companyName,
            CreatorClientId = creatorId,
            CreationTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            TornadosCaught = 0,
            CompanyBalance = 0f
        };
        
        // Добавление создателя в качестве главы
        newCompany.Members.Add(new CompanyMember
        {
            ClientId = creatorId,
            Nickname = "Unknown",
            Rank = PlayerRank.Chief,
            JoinTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            TornadosCaught = 0
        });
        
        companies[companyId] = newCompany;
        playerCompanies[creatorId] = companyId;
        
        // Списание денег
        player.TrySpendBalance(GameConstants.COMPANY_CREATION_COST);
        
        // Уведомление клиентов
        NotifyCompanyCreatedClientRpc(companyId, companyName);
        
        Debug.Log($"Company '{companyName}' created with ID: {companyId}");
    }
    
    /// <summary>
    /// Присоединяет игрока к компании
    /// </summary>
    [Rpc(SendTo.Server)]
    public void JoinCompanyRpc(ulong playerId, string companyId)
    {
        if (!IsServer) return;
        
        if (!companies.ContainsKey(companyId))
        {
            Debug.LogWarning($"Company {companyId} not found");
            return;
        }
        
        var company = companies[companyId];
        
        // Проверка лимита
        if (company.Members.Count >= GameConstants.MAX_COMPANY_MEMBERS)
        {
            Debug.LogWarning("Company is full");
            return;
        }
        
        // Проверка, не состоит ли уже в компании
        if (playerCompanies.ContainsKey(playerId))
        {
            Debug.LogWarning("Player already in a company");
            return;
        }
        
        // Добавление в компанию
        company.Members.Add(new CompanyMember
        {
            ClientId = playerId,
            Nickname = "Unknown",
            Rank = PlayerRank.Interceptor,
            JoinTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            TornadosCaught = 0
        });
        
        playerCompanies[playerId] = companyId;
        
        // Уведомление
        NotifyPlayerJoinedClientRpc(playerId, companyId);
        
        Debug.Log($"Player {playerId} joined company {companyId}");
    }
    
    /// <summary>
    /// Повышает ранг игрока в компании
    /// </summary>
    [Rpc(SendTo.Server)]
    public void PromotePlayerRpc(ulong issuerId, ulong targetPlayerId, string companyId)
    {
        if (!IsServer) return;
        
        if (!companies.ContainsKey(companyId)) return;
        
        var company = companies[companyId];
        var issuer = company.Members.FirstOrDefault(m => m.ClientId == issuerId);
        var target = company.Members.FirstOrDefault(m => m.ClientId == targetPlayerId);
        
        // Проверка прав
        if (issuer?.Rank < PlayerRank.Deputy) return;
        
        if (target != null && target.Rank < PlayerRank.Chief)
        {
            target.Rank++;
            NotifyPlayerPromotedClientRpc(targetPlayerId, companyId, (int)target.Rank);
        }
    }
    
    /// <summary>
    /// Добавляет награду в компанию за перехват торнадо
    /// </summary>
    [Rpc(SendTo.Server)]
    public void AddCompanyRewardRpc(string companyId, float amount)
    {
        if (!IsServer) return;
        
        if (companies.ContainsKey(companyId))
        {
            companies[companyId].CompanyBalance += amount;
            companies[companyId].TornadosCaught++;
        }
    }
    
    /// <summary>
    /// Получает информацию о компании
    /// </summary>
    public CompanyData GetCompanyData(string companyId)
    {
        if (companies.TryGetValue(companyId, out var company))
        {
            return company;
        }
        return null;
    }
    
    /// <summary>
    /// Получает ID компании игрока
    /// </summary>
    public string GetPlayerCompanyId(ulong playerId)
    {
        if (playerCompanies.TryGetValue(playerId, out var companyId))
        {
            return companyId;
        }
        return "";
    }
    
    /// <summary>
    /// Получает всех игроков в компании
    /// </summary>
    public List<CompanyMember> GetCompanyMembers(string companyId)
    {
        if (companies.TryGetValue(companyId, out var company))
        {
            return new List<CompanyMember>(company.Members);
        }
        return new List<CompanyMember>();
    }
    
    // ========== CLIENT RPC NOTIFICATIONS ==========
    
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyCompanyCreatedClientRpc(string companyId, string companyName)
    {
        Debug.Log($"[CLIENT] Company created: {companyName} ({companyId})");
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyPlayerJoinedClientRpc(ulong playerId, string companyId)
    {
        Debug.Log($"[CLIENT] Player {playerId} joined company {companyId}");
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyPlayerPromotedClientRpc(ulong playerId, string companyId, int newRank)
    {
        Debug.Log($"[CLIENT] Player {playerId} promoted to rank {newRank} in company {companyId}");
    }
}
