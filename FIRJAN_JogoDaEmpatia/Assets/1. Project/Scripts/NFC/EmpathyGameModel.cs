using System;

/// <summary>
/// Modelo de dados específico para o Jogo da Empatia.
/// Contém as 3 habilidades principais: Empatia, Escuta Ativa e Autoconsciência.
/// </summary>
[Serializable]
public class EmpathyGameModel
{
    public string nfcId;
    public int gameId = 5; // ID fixo do Jogo da Empatia
    public int skill1; // Empatia (máximo 9)
    public int skill2; // Escuta Ativa (máximo 7) 
    public int skill3; // Autoconsciência (máximo 5)

    public EmpathyGameModel(string nfcId, int empathy, int activeListening, int selfAwareness)
    {
        this.nfcId = nfcId;
        this.skill1 = empathy;
        this.skill2 = activeListening;
        this.skill3 = selfAwareness;
    }

    /// <summary>
    /// Converte o modelo para JSON para envio à API.
    /// </summary>
    public string ToJson()
    {
        return UnityEngine.JsonUtility.ToJson(this);
    }

    public override string ToString()
    {
        return $"EmpathyGame - NFC:{nfcId}, Empatia:{skill1}, EscutaAtiva:{skill2}, Autoconsciência:{skill3}";
    }
}