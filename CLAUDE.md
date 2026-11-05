# CLAUDE.md

> **開工前先讀 `PROGRESS.md`**：裡面有目前階段、已做決策、未解問題與下一步。

## 專案概要
Project Rolling：手機平台的「電影拍攝助理」模擬遊戲，目前是早期 prototype。
- Unity **6000.3.27f1**（Unity 6 LTS；2026-10-31 從 2019.4.29f1 升級；見 `ProjectSettings/ProjectVersion.txt`）
- 開發環境：**macOS（Apple Silicon）**，專案路徑 `/Users/mouse76229/Dev Projects/ProjectRolling`（2026-11 從 Windows 移過來）
- Built-in Render Pipeline、舊版 Input Manager（`Input.GetAxisRaw` / `GetKey`）
- NavMesh：runtime 用 `UnityEngine.AI.NavMeshAgent`；場景仍使用舊版烘焙的 `NavMesh.asset`（Unity 6 可正常載入）。已裝 `com.unity.ai.navigation` 2.0.16，日後重新烘焙請用 NavMeshSurface。
- UI 用 `UnityEngine.UI.Text`（Unity 6 中標為 Legacy，仍可用；TMP 已併入 ugui 2.0）
- MCP：`com.coplaydev.unity-mcp`（MCP for Unity，鎖定 `#v10.3.0`），見下方「Unity MCP」

## 資料夾結構
```
Assets/
  Scenes/Prototype.unity        唯一場景
  Scenes/Prototype/NavMesh.asset 烘焙好的 NavMesh
  Scripts/                      所有遊戲腳本（無 asmdef、無 namespace）
  Prefabs/                      Player, Somebody
  Materials/                    Floor, Item, People, Road
Packages/manifest.json          套件清單
.mcp.json                       Claude Code 專案層級 MCP 設定（UnityMCP）
```
`Library/`、`Temp/`、`obj/`、`Logs/`、`UserSettings/` 與 `.csproj/.sln/.slnx` 都是自動產生的，不要手改。

## 腳本
| 腳本 | 功能 |
|---|---|
| `Player.cs` | `CharacterController` 移動（WASD/方向鍵），面向移動方向旋轉；Left Shift 速度加倍；重力（`_gravity`，落地時 `_yVelocity` 重設）；位置 clamp 在 x∈[-9,9]、z∈[-9,6]；在 `Item` tag 觸發區內按 E 撿起/放下物品（掛到 `_itemHolder`，切換 Rigidbody kinematic）。 |
| `Somebody.cs` | 路人 NPC，`NavMeshAgent` 走向 SpawnManager 指定的目標點，抵達後 3 秒銷毀。「發現玩家就回原點」的邏輯已寫但觸發被註解掉。 |
| `SpawnManager.cs` | Coroutine 每 2–7 秒從 `_startPosition[]` 隨機生成 `Somebody`，並指派 `_endPosition[]` 中的隨機目標。 |
| `CarMoves.cs` | 車子用 `NavMeshAgent` 從 site A 開到 site B，抵達後等 5–8 秒瞬移回 A 重來。 |
| `Timer.cs` | 倒數計時（預設 10 秒），以 `mm:ss` 顯示於 `Text`；結束時設 `GameManager.IsTimeGoing = false`。 |
| `GameManager.cs` | Singleton（`GameManager.Instance`，於 `Awake` 指派），目前只有 `IsTimeGoing` 屬性。 |

## 程式慣例（照現有程式碼）
- Private 欄位用 `_camelCase`；需要在 Inspector 調整的用 `[SerializeField] private`，不用 public 欄位。
- 公開狀態用 auto-property（`public bool IsTimeGoing { get; set; }`）。
- `Start()` 中 `GetComponent` 後做 null 檢查並 `Debug.LogError("X is NULL!")`。
- Singleton 模式：`private static _instance` + `Instance` getter（null 時 LogError）。
- 時間行為用 Coroutine + `WaitForSeconds`；冷卻用 `Time.time > _canX`。
- 用 tag 字串判斷碰撞（`"Item"`, `"Player"`）。
- 常留步驟式註解描述預定邏輯。

## 開啟與執行
1. Unity Hub 安裝 **6000.3.27f1**。必須用這個版本開，用其他版本開會再次觸發升級。
2. 以 Unity Hub 開啟專案根目錄 `/Users/mouse76229/Dev Projects/ProjectRolling`。
3. 開啟 `Assets/Scenes/Prototype.unity`，按 Play。操作：WASD 移動、Left Shift 加速、E 撿/放物品。
4. 目前沒有自動化測試（test-framework 已裝但未使用）。
5. Headless 編譯檢查（Editor 必須先關閉，同一專案不能同時開兩個 Unity）：
   ```
   "/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "/Users/mouse76229/Dev Projects/ProjectRolling" -logFile <log>
   ```
   之後在 log 中搜尋 `error CS`；看到 `Exiting batchmode successfully` 表示成功。

## Unity MCP（MCP for Unity）
- 專案根目錄的 `.mcp.json` 用 HTTP 連到 `http://localhost:8080/mcp`（server 名稱 `UnityMCP`），本專案所有 Claude Code 分頁共用。HTTP 模式支援多個 client 同時連線。
- 前置需求：`uv`/`uvx`（裝在 `~/.local/bin`）、Python 3.10+（Homebrew `/opt/homebrew/bin/python3`）。
- Editor 執行時的 log：`~/Library/Logs/Unity/Editor.log`（MCP 狀態訊息以 `MCP-FOR-UNITY` 開頭）。
- 注意：本機 Docker 在 `100.64.13.57:8080` 監聽（非 127.0.0.1）。若 Start Server 失敗並回報 port 被占用，先查 `lsof -nP -iTCP:8080 -sTCP:LISTEN`。
- 使用前要先用 Unity Editor 開著此專案，並在 **Window → MCP for Unity** 按 **Start Server**（或在 Advanced 勾選 *Auto-Start Server on Editor Load*）。狀態顯示 `Connected` 後，在 Claude Code 用 `/mcp` 確認 `UnityMCP` 已連線。
- **不需要**在該視窗對 Claude Code 按 Configure。它會另外用 `claude mcp add --scope local` 寫一份設定，本專案已經用 `.mcp.json` 了。
- Editor 開著時不能跑上面的 batchmode 檢查；Editor 開著時改用 MCP 讀 Console。

## 已知問題
詳見 `PROGRESS.md` 的「未解問題」。重點：Timer 結束後無行為、Somebody 躲避玩家未啟用。

## 團隊規則
- PM 負責派工與 git commit/push；其他角色不要自行 commit。
- 每次完成工作後更新 `PROGRESS.md`。
