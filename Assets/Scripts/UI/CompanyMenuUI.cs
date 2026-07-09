using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections.Generic;

public class CompanyMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI companyNameText;
    [SerializeField] private TextMeshProUGUI memberCountText;
    [SerializeField] private TextMeshProUGUI tornadosCaughtText;
    
    [SerializeField] private Transform memberListContainer;
    [SerializeField] private GameObject memberItemPrefab;
    
    [SerializeField] private Button createCompanyButton;
    [SerializeField] private Button joinCompanyButton;
    [SerializeField] private Button backButton;
    
    [SerializeField] private Canvas createCompanyCanvas;
    [SerializeField] private TMP_InputField companyNameInput;
    [SerializeField] private Button confirmCreateButton;
    
    [SerializeField] private Canvas joinCompanyCanvas;
    [SerializeField] private TMP_InputField searchCompanyInput;
    [SerializeField] private Button searchButton;
    
    private CompanySystem companySystem;
    private PlayerDataManager playerData;
    private string currentCompanyId = "";
    
    private void Start()
    {
        companySystem = CompanySystem.Instance;
        playerData = PlayerDataManager.Instance;
        
        createCompanyButton.onClick.AddListener(() => createCompanyCanvas.enabled = true);
        joinCompanyButton.onClick.AddListener(() => joinCompanyCanvas.enabled = true);
        backButton.onClick.AddListener(() => gameObject.SetActive(false));
        confirmCreateButton.onClick.AddListener(CreateCompany);
        searchButton.onClick.AddListener(SearchCompany);
        
        RefreshCompanyInfo();
    }
    
    private void CreateCompany()
    {
        if (string.IsNullOrWhiteSpace(companyNameInput.text))
        {
            Debug.LogWarning("Company name is empty");
            return;
        }
        
        companySystem.CreateCompanyRpc(companyNameInput.text, playerData.GetPlayerId());
        companyNameInput.text = "";
        createCompanyCanvas.enabled = false;
    }
    
    private void SearchCompany()
    {
        // Здесь можно добавить поиск компании по названию
        Debug.Log("Searching for company: " + searchCompanyInput.text);
    }
    
    private void RefreshCompanyInfo()
    {
        currentCompanyId = playerData.GetCompanyId();
        
        if (string.IsNullOrEmpty(currentCompanyId))
        {
            companyNameText.text = "No Company";
            memberCountText.text = "0/50";
            tornadosCaughtText.text = "0";
            return;
        }
        
        var companyData = companySystem.GetCompanyData(currentCompanyId);
        if (companyData == null) return;
        
        companyNameText.text = companyData.CompanyName;
        memberCountText.text = $"{companyData.Members.Count}/{GameConstants.MAX_COMPANY_MEMBERS}";
        tornadosCaughtText.text = companyData.TornadosCaught.ToString();
        
        // Обновляем список участников
        RefreshMemberList(companyData.Members);
    }
    
    private void RefreshMemberList(List<CompanyMember> members)
    {
        // Очищаем старый список
        foreach (Transform child in memberListContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Добавляем новых членов
        foreach (var member in members)
        {
            GameObject memberItem = Instantiate(memberItemPrefab, memberListContainer);
            var memberText = memberItem.GetComponent<TextMeshProUGUI>();
            if (memberText != null)
            {
                memberText.text = $"{member.Nickname} ({CompanyRoles.GetRankName((PlayerRankType)member.Rank)})";
            }
        }
    }
}
