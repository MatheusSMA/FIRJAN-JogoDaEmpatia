# Jogo da Empatia - Sistema de Duas Imagens com Fade Effect

## 📋 Mudanças Implementadas

### 🎮 Novo Comportamento das Rodadas
Cada uma das 3 rodadas agora segue esta sequência com efeitos visuais:

1. **Primeira Imagem com Fade In + Descrição Inicial**
   - A primeira imagem aparece com efeito de fade in suave
   - Mostra o texto: *"Observe a situação apresentada"*
   - Duração: 10 segundos (configurável)

2. **Segunda Imagem ao Lado com Fade In + Pergunta**
   - A segunda imagem aparece **ao lado** da primeira com fade in
   - **Ambas ficam visíveis simultaneamente**
   - Altera o texto para: *"Qual sua opinião sobre isso?"*
   - Exibe as opções de palavras para seleção
   - Mostra o botão "Confirmar"

3. **Confirmação e Feedback**
   - Após confirmação das escolhas
   - Mostra feedback da Aya
   - Avança para próxima rodada ou vai para resultados

## 🖼️ Sistema de Duas Imagens com Fade Effect

### Configuração no Inspector
O `ScreenGame.cs` agora possui **6 campos públicos** para as sprites:

```csharp
[Header("Round Images - 6 Sprites Total")]
public Sprite round1Image1;  // Primeira imagem da Rodada 1
public Sprite round1Image2;  // Segunda imagem da Rodada 1
public Sprite round2Image1;  // Primeira imagem da Rodada 2
public Sprite round2Image2;  // Segunda imagem da Rodada 2
public Sprite round3Image1;  // Primeira imagem da Rodada 3
public Sprite round3Image2;  // Segunda imagem da Rodada 3
```

### UI Components com Fade Effect
- **imageDisplay1**: Primeira `Image` com `CanvasGroup` para fade effect
- **imageDisplay1CanvasGroup**: CanvasGroup da primeira imagem
- **imageDisplay2**: Segunda `Image` com `CanvasGroup` para fade effect  
- **imageDisplay2CanvasGroup**: CanvasGroup da segunda imagem
- **descriptionText**: `TextMeshProUGUI` para as descrições
- **wordsContainer**: Contêiner que fica inicialmente desativado
- **confirmButton**: Botão que aparece apenas na segunda fase
- **fadeDuration**: Duração do efeito de fade (padrão: 1 segundo)

## 🔧 Mudanças Técnicas

### Estrutura `RoundData` Simplificada
```csharp
[System.Serializable]
public class RoundData
{
    public List<WordChoice> words = new List<WordChoice>();
    // Removidos: Sprite image1, Sprite image2
}
```

### Novos Métodos Implementados
- `RoundImageSequence()`: Corrotina que gerencia a sequência de imagens com fade
- `FadeCanvasGroup()`: Corrotina para efeito de fade in/out suave
- `GetImage1ForRound()`: Retorna a primeira imagem da rodada
- `GetImage2ForRound()`: Retorna a segunda imagem da rodada

### Configurações Personalizáveis
```csharp
[Header("Screen Settings")]
[SerializeField] private float imageTransitionDelay = 10f;  // Tempo entre imagens
[SerializeField] private float fadeDuration = 1f;           // Duração do fade effect
[SerializeField] private string initialDescription = "Observe a situação apresentada";
[SerializeField] private string questionDescription = "Qual sua opinião sobre isso?";
```

## 📖 Como Configurar no Unity

1. **Arraste as 6 sprites** nos campos correspondentes no `ScreenGame` component

2. **Configure os dois displays de imagem**:
   - Crie dois GameObjects com componentes `Image` e `CanvasGroup`
   - Posicione-os **lado a lado** na tela
   - Configure `imageDisplay1` + `imageDisplay1CanvasGroup`
   - Configure `imageDisplay2` + `imageDisplay2CanvasGroup`
   - **Importante**: Inicie ambos CanvasGroups com Alpha = 0

3. **Configure as outras referências de UI**:
   - `descriptionText`: GameObject com component `TextMeshProUGUI`
   - `wordsContainer`, `wordButtonPrefab`, `confirmButton`, `feedbackText`

4. **Ajuste timing e efeitos**:
   - `imageTransitionDelay`: Tempo de espera entre imagens (padrão 10s)
   - `fadeDuration`: Velocidade do fade effect (padrão 1s)

## ✅ Funcionalidades Mantidas

- ✅ Sistema de 3 rodadas
- ✅ 24 palavras pré-configuradas (8 por rodada)
- ✅ Cálculo de pontuação empática
- ✅ Feedback da Aya após cada rodada
- ✅ Transição automática para tela de resultados
- ✅ Sistema de pontuação final das habilidades

## 🆕 Novas Funcionalidades

- ✅ **Duas imagens lado a lado**: Primeira aparece, segunda aparece ao lado após 10s
- ✅ **Efeito de fade suave**: CanvasGroup com alpha animado para transições elegantes
- ✅ **Sequência temporal**: Imagem 1 fade in (10s) → Imagem 2 fade in + Opções
- ✅ **Descrições contextuais** que mudam durante a rodada
- ✅ **UI adaptativa** (elementos aparecem/somem conforme necessário)
- ✅ **6 sprites organizadas** por rodada
- ✅ **Configurações flexíveis** no Inspector (timing + efeitos)
- ✅ **Animação interpolada** com Mathf.Lerp para suavidade

---

**Arquivos Modificados:**
- `ScreenGame.cs` - Lógica principal atualizada
- `GameSetupExample.cs` - Documentação atualizada
- `README_Updates.md` - Este arquivo de documentação