using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using _4._NFC_Firjan.Scripts.Server;
using _4._NFC_Firjan.Scripts.NFC;

/// <summary>
/// Serviço principal do sistema NFC para o Jogo da Empatia.
/// Implementa singleton pattern e integração automática com NFCReceiver e ServerComunication.
/// Gerencia dados pendentes e envio automático quando cartão NFC é aproximado.
/// </summary>
public class NFCGameService : MonoBehaviour
{
    #region Singleton

    private static NFCGameService _instance;
    public static NFCGameService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<NFCGameService>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("NFCGameService");
                    _instance = go.AddComponent<NFCGameService>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    #endregion

    #region Fields

    [Header("NFC Configuration")]
    [SerializeField] private bool enableDebugLogs = true;

    [Header("Server Configuration")]
    [SerializeField] private string serverIP = "127.0.0.1";
    [SerializeField] private string serverPort = "3000";

    private NFCReceiver nfcReceiver;
    private ServerComunication serverCommunication;
    private string lastReadNfcId;
    private EmpathyGameModel pendingGameData;
    private bool isInitialized = false;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeNFCService();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadServerConfiguration();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Inicializa o serviço NFC e configura as integrações.
    /// </summary>
    private void InitializeNFCService()
    {
        LogNFC("Inicializando serviço NFC do Jogo da Empatia...");

        // Busca componentes automaticamente
        nfcReceiver = FindFirstObjectByType<NFCReceiver>();
        serverCommunication = FindFirstObjectByType<ServerComunication>();

        if (nfcReceiver == null)
        {
            LogNFC("ERRO: NFCReceiver não encontrado! Certifique-se de que o prefab NFC está na cena.");
            return;
        }

        if (serverCommunication == null)
        {
            LogNFC("ERRO: ServerComunication não encontrado! Certifique-se de que o prefab Server está na cena.");
            return;
        }

        // Configura eventos do NFC
        SetupNFCEvents();

        isInitialized = true;
        LogNFC("Serviço NFC inicializado com sucesso!");
    }

    /// <summary>
    /// Configura os eventos do sistema NFC.
    /// </summary>
    private void SetupNFCEvents()
    {
        if (nfcReceiver != null)
        {
            // Remove listeners existentes para evitar duplicatas
            nfcReceiver.OnNFCConnected.RemoveListener(OnNFCCardRead);
            nfcReceiver.OnNFCDisconnected.RemoveListener(OnNFCCardRemoved);

            // Adiciona novos listeners
            nfcReceiver.OnNFCConnected.AddListener(OnNFCCardRead);
            nfcReceiver.OnNFCDisconnected.AddListener(OnNFCCardRemoved);

            LogNFC("Eventos NFC configurados.");
        }
    }

    /// <summary>
    /// Carrega configuração do servidor do arquivo serverconfig.json.
    /// </summary>
    private void LoadServerConfiguration()
    {
        string configPath = System.IO.Path.Combine(Application.streamingAssetsPath, "serverconfig.json");

        if (System.IO.File.Exists(configPath))
        {
            try
            {
                string configJson = System.IO.File.ReadAllText(configPath);
                ServerConfig config = JsonUtility.FromJson<ServerConfig>(configJson);

                if (config != null && !string.IsNullOrEmpty(config.serverIP))
                {
                    serverIP = config.serverIP;
                    serverPort = config.serverPort;
                    LogNFC($"Configuração carregada: {serverIP}:{serverPort}");
                }
            }
            catch (System.Exception ex)
            {
                LogNFC($"Erro ao carregar configuração: {ex.Message}");
            }
        }
        else
        {
            LogNFC($"Arquivo de configuração não encontrado: {configPath}");
        }
    }

    #endregion

    #region NFC Events

    /// <summary>
    /// Callback chamado quando um cartão NFC é lido na tela final.
    /// </summary>
    private void OnNFCCardRead(string nfcId, string readerName)
    {
        LogNFC($"Cartão lido na tela final: {nfcId} | Leitor: {readerName}");

        lastReadNfcId = nfcId;

        // Se há dados do jogo armazenados, envia imediatamente
        if (pendingGameData != null)
        {
            LogNFC("Dados do jogo encontrados. Enviando POST imediatamente...");
            pendingGameData.nfcId = nfcId;
            SendGameDataToServer(pendingGameData);
            pendingGameData = null; // Limpa após envio
        }
        else
        {
            LogNFC("Cartão lido mas não há dados do jogo para enviar.");
        }
    }    /// <summary>
         /// Callback chamado quando um cartão NFC é removido.
         /// </summary>
    private void OnNFCCardRemoved()
    {
        LogNFC($"Cartão removido");
        // Mantém o lastReadNfcId mesmo após remoção conforme especificado
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Método público para submeter resultado do jogo.
    /// Armazena os dados e aguarda o cartão NFC ser lido na tela final.
    /// </summary>
    /// <param name="empathy">Pontuação de Empatia (0-9)</param>
    /// <param name="activeListening">Pontuação de Escuta Ativa (0-7)</param>
    /// <param name="selfAwareness">Pontuação de Autoconsciência (0-5)</param>
    public void SubmitGameResult(int empathy, int activeListening, int selfAwareness)
    {
        if (!isInitialized)
        {
            LogNFC("ERRO: Serviço NFC não inicializado!");
            return;
        }

        LogNFC($"SubmitGameResult chamado - Empatia:{empathy}, EscutaAtiva:{activeListening}, Autoconsciência:{selfAwareness}");

        // Armazena os dados do jogo para envio quando cartão for lido
        pendingGameData = new EmpathyGameModel("", empathy, activeListening, selfAwareness);
        LogNFC("Dados do jogo armazenados. Aguardando cartão NFC ser aproximado na tela final.");
    }

    /// <summary>
    /// Obtém o último NFC ID lido (método auxiliar para debug).
    /// </summary>
    public string GetLastNfcId()
    {
        return lastReadNfcId;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Envia dados do jogo para o servidor.
    /// </summary>
    private void SendGameDataToServer(EmpathyGameModel gameData)
    {
        if (gameData == null || string.IsNullOrEmpty(gameData.nfcId))
        {
            LogNFC("ERRO: Dados do jogo inválidos para envio.");
            return;
        }

        StartCoroutine(SendPostRequest(gameData));
    }

    /// <summary>
    /// Corrotina para enviar requisição POST para a API.
    /// </summary>
    private IEnumerator SendPostRequest(EmpathyGameModel gameData)
    {
        string url = $"http://{serverIP}:{serverPort}/users/{gameData.nfcId}";
        string jsonData = gameData.ToJson();

        LogNFC($"Enviando dados para: {url}");
        LogNFC($"JSON: {jsonData}");

        using (UnityWebRequest request = UnityWebRequest.Post(url, jsonData, "application/json"))
        {
            // Headers conforme especificado
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("User-Agent", "insomnia/11.6.0");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                LogNFC($"Dados enviados com sucesso para o cartão {gameData.nfcId}");
                LogNFC($"Resposta do servidor: {request.downloadHandler.text}");
            }
            else
            {
                LogNFC($"Erro ao enviar dados: {request.error}");
                LogNFC($"Código de resposta: {request.responseCode}");
                LogNFC("ERRO: Falha ao enviar dados do jogo. Verifique a conexão com o servidor.");
            }
        }
    }

    /// <summary>
    /// Método auxiliar para logs com prefixo [NFC].
    /// </summary>
    private void LogNFC(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[NFC] {message}");
        }
    }

    #endregion

    #region Debug Methods

    [ContextMenu("Test Submit Game Result")]
    public void TestSubmitGameResult()
    {
        SubmitGameResult(8, 6, 4);
    }

    [ContextMenu("Clear NFC Data")]
    public void ClearNfcData()
    {
        lastReadNfcId = null;
        pendingGameData = null;
        LogNFC("Dados NFC limpos.");
    }

    [ContextMenu("Debug Service State")]
    public void DebugServiceState()
    {
        LogNFC("=== ESTADO DO SERVIÇO NFC ===");
        LogNFC($"Inicializado: {isInitialized}");
        LogNFC($"Último NFC ID: {lastReadNfcId ?? "Nenhum"}");
        LogNFC($"Dados do jogo pendentes: {(pendingGameData != null ? pendingGameData.ToString() : "Nenhum")}");
        LogNFC($"Servidor: {serverIP}:{serverPort}");
        LogNFC($"NFCReceiver: {(nfcReceiver != null ? "Conectado" : "Não encontrado")}");
        LogNFC($"ServerComunication: {(serverCommunication != null ? "Conectado" : "Não encontrado")}");
    }

    #endregion
}

/// <summary>
/// Estrutura para carregar configuração do servidor.
/// </summary>
[System.Serializable]
public class ServerConfig
{
    public string serverIP = "127.0.0.1";
    public string serverPort = "3000";
}