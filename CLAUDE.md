# CLAUDE.md

> **開工前先讀 `PROGRESS.md`**：裡面有目前階段、已做決策、未解問題與下一步。

## 專案概要
Project Rolling：手機平台的「電影拍攝助理」模擬遊戲，目前是早期 prototype。
- Unity **2019.4.29f1**（`ProjectSettings/ProjectVersion.txt`）
- Built-in Render Pipeline、舊版 Input Manager（`Input.GetAxisRaw` / `GetKey`）、NavMesh（內建 `UnityEngine.AI`，非 NavMesh Components 套件）、UI 用 `UnityEngine.UI.Text`（已裝 TextMeshPro 2.1.6，但尚未使用）

## 資料夾結構
```
Assets/
  Scenes/Prototype.unity        唯一場景
  Scenes/Prototype/NavMesh.asset 烘焙好的 NavMesh
  Scripts/                      所有遊戲腳本（無 asmdef、無 namespace）
  Prefabs/                      Player, Somebody
  Materials/                    Floor, Item, People, Road
Packages/manifest.json          套件清單
```
`Library/`、`Temp/`、`obj/`、`Logs/` 與 `.csproj/.sln` 為自動產生，不要手改。

## 腳本
| 腳本 | 功能 |
|---|---|
| `Player.cs` | `CharacterController` 移動（WASD/方向鍵），面向移動方向旋轉；Left Shift 速度加倍；位置 clamp 在 x∈[-9,9]、z∈[-9,6]；在 `Item` tag 觸發區內按 E 撿起/放下物品（掛到 `_itemHolder`，切換 Rigidbody kinematic）。 |
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
- 常留中文/英文的步驟式註解描述預定邏輯。

## 開啟與執行
1. Unity Hub 安裝 **2019.4.29f1**（須同版本，避免觸發升級）。
2. 以 Unity Hub 開啟專案根目錄 `C:\Unity Projects\Project Rolling`。
3. 開啟 `Assets/Scenes/Prototype.unity`，按 Play。操作：WASD 移動、Left Shift 加速、E 撿/放物品。
4. 目前無自動化測試（test-framework 已裝但未使用）。

## 已知問題
詳見 `PROGRESS.md` 的「未解問題」。重點：Player 重力未套用、SpawnManager 寫死索引範圍、Timer 結束後無行為、Somebody 躲避玩家未啟用。

## 團隊規則
- PM 負責派工與 git commit/push；其他角色不要自行 commit。
- 每次完成工作後更新 `PROGRESS.md`。
