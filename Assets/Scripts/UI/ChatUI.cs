using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections.Generic;

public class ChatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chatDisplayText;
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private ScrollRect chatScrollRect;
    
    private List<string> chatMessages = new List<string>();
    private const int MAX_MESSAGES = 100;
    
    private void Start()
    {
        sendButton.onClick.AddListener(SendMessage);
        chatInputField.onSubmit.AddListener(delegate { SendMessage(); });
    }
    
    private void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(chatInputField.text))
            return;
        
        string message = chatInputField.text;
        chatInputField.text = "";
        
        // Отправляем сообщение на сервер
        SendMessageRpc(message);
    }
    
    [Rpc(SendTo.Server)]
    private void SendMessageRpc(string message)
    {
        var playerData = PlayerDataManager.Instance;
        if (playerData == null) return;
        
        string playerName = playerData.GetNickname();
        string fullMessage = $"{playerName}: {message}";
        
        // Отправляем всем клиентам
        ReceiveMessageClientRpc(fullMessage);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void ReceiveMessageClientRpc(string message)
    {
        chatMessages.Add(message);
        
        // Ограничиваем количество сообщений
        if (chatMessages.Count > MAX_MESSAGES)
        {
            chatMessages.RemoveAt(0);
        }
        
        UpdateChatDisplay();
    }
    
    private void UpdateChatDisplay()
    {
        chatDisplayText.text = string.Join("\n", chatMessages);
        
        // Скроллим к концу
        if (chatScrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
