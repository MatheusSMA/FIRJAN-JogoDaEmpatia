# Sistema NFC - Jogo da Empatia

Este documento descreve a implementação completa do sistema NFC para o Jogo da Empatia.

## Arquivos Criados

### 1. Scripts Principais
- `Assets/1. Project/Scripts/NFC/NFCGameService.cs` - Serviço principal do sistema NFC
- `Assets/1. Project/Scripts/NFC/EmpathyGameModel.cs` - Modelo de dados específico do jogo
- `Assets/1. Project/Scripts/NFC/NFCSetupHelper.cs` - Helper para configuração automática

### 2. Configuração
- `Assets/StreamingAssets/serverconfig.json` - Configuração do servidor

### 3. Integração
- Modificado `ScreenResult.cs` para integrar com o sistema NFC

## Configuração da Cena

### Pré-requisitos
1. Adicione os prefabs da pasta `Assets/4. NFC Firjan/`:
   - `[Adicionar na Cena] NFC.prefab`
   - `[Adicionar na Cena] Server.prefab`

2. Certifique-se de que as DLLs estão configuradas:
   - `Lando.dll`
   - `Newtonsoft.Json.dll`

### Setup Automático
1. Crie um GameObject vazio na cena
2. Adicione o componente `NFCSetupHelper`
3. Clique em "Setup NFC System" no Inspector (ou deixe `autoSetupOnStart` ativo)

### Setup Manual
1. Crie um GameObject com o componente `NFCGameService`
2. O serviço se inicializará automaticamente e encontrará os outros componentes

## Funcionamento

### Fluxo Principal
1. **Jogo termina** → `ScreenResult.ShowFinalResults()` é chamado
2. **Sistema calcula habilidades** → Empatia, Escuta Ativa, Autoconsciência
3. **Chama NFCGameService** → `SubmitGameResult(empathy, activeListening, selfAwareness)`
4. **Se cartão NFC está presente** → Envia dados imediatamente
5. **Se não há cartão** → Armazena dados como pendentes
6. **Cartão é aproximado** → Sistema detecta e envia dados automaticamente

### Dados Enviados
```json
{
  "nfcId": "CARD123",
  "gameId": 5,
  "skill1": 8,  // Empatia (0-9)
  "skill2": 6,  // Escuta Ativa (0-7) 
  "skill3": 4   // Autoconsciência (0-5)
}
```

### API Endpoint
- **Método**: PUT
- **URL**: `http://[IP]:[PORT]/users/[NFC_ID]`
- **Headers**: 
  - `Content-Type: application/json`
  - `User-Agent: insomnia/11.6.0`

## Configuração do Servidor

Edite `Assets/StreamingAssets/serverconfig.json`:

```json
{
    "serverIP": "127.0.0.1",
    "serverPort": "3000"
}
```

## Pontuação das Habilidades

Baseado na pontuação final do jogo (0-12):

### Pontuação Alta (9-12 pontos)
- Empatia: 9
- Escuta Ativa: 7
- Autoconsciência: 5

### Pontuação Média (6-8 pontos)
- Empatia: 8
- Escuta Ativa: 6
- Autoconsciência: 4

### Pontuação Baixa (≤5 pontos)
- Empatia: 7
- Escuta Ativa: 5
- Autoconsciência: 3

## Debug e Testes

### Métodos de Context Menu
- **NFCGameService**: 
  - `Test Submit Game Result`
  - `Clear Pending Data`
  - `Debug Service State`

- **NFCSetupHelper**:
  - `Setup NFC System`
  - `Test NFC System`
  - `Clear NFC Pending Data`
  - `Debug NFC System`

- **ScreenResult**:
  - `Test NFC Submit`

### Logs Importantes
Todos os logs aparecem com prefixo `[NFC]`:

- `[NFC] Cartão lido e conectado: {id} | Leitor: {reader}`
- `[NFC] Dados enviados com sucesso para o cartão {id}`
- `[NFC] Dados armazenados para envio automático`
- `[NFC] JSON: {jsonData}` (antes do envio)

## Características Especiais

### Sistema de Dados Pendentes
- Jogo pode terminar sem cartão NFC presente
- Dados ficam armazenados na memória
- Quando cartão é aproximado, dados são enviados automaticamente
- Sistema robusto contra falhas de rede

### Preservação do NFC ID
- Último ID lido é mantido mesmo após remoção do cartão
- Permite envio mesmo com aproximação rápida do cartão

### Singleton Pattern
- `NFCGameService.Instance` está sempre disponível
- Serviço persiste entre cenas
- Inicialização automática se não existir

## Troubleshooting

### Problemas Comuns
1. **"NFCReceiver não encontrado"** → Adicione o prefab NFC na cena
2. **"ServerComunication não encontrado"** → Adicione o prefab Server na cena
3. **"Erro ao enviar dados"** → Verifique IP/porta do servidor
4. **"Arquivo de configuração não encontrado"** → Crie `serverconfig.json` em StreamingAssets

### Verificação do Sistema
Use o `NFCSetupHelper` para diagnóstico completo do sistema.