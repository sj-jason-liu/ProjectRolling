# PROGRESS.md — Project Rolling 進度紀錄

> context 被清掉後從這裡接續工作。每次完成任務後更新對應段落。最後更新：2026-10-31（Engineer）

## 目前階段
早期 prototype（單一場景 `Prototype.unity`）。核心移動、撿放物品、路人生成、車輛往返、倒數計時已有雛形；尚無勝負/計分流程。
本輪：在分支 `upgrade/unity6` 升級到 Unity 6000.3.27f1 並安裝 MCP for Unity。工作已完成但**尚未 commit**，等使用者在 Editor 實測後由 PM commit/merge。

## 團隊角色
| 角色 | 負責 |
|---|---|
| PM（Claude） | 派工、協調、git commit/push/merge |
| Gameplay Designer（Codex） | 遊戲系統企劃 |
| Designer（Impeccable plugin） | 視覺 |
| Engineer（Claude） | 程式實作、技術評估 |

## 已完成
| Commit | 內容 |
|---|---|
| `ecf876b` | Initial commit |
| `3b7c7b7` | 建立專案：基本移動、撿/放物品、倒數計時 |
| `f4a18e9` | 修正 NavMeshAgent 追蹤錯誤 |
| `54d8de9` | Somebody 在玩家靠近時避開（觸發目前已被註解，見未解問題） |
| `ac12191` | 隱形牆、擴大地板 |
| `de9aaa8` | 車輛以 NavMeshAgent 移動 |
| `66c4a2d` | 不用 collider 改用 clamp 限制玩家移動範圍；修正車輛重生 |
| `2abb1ce` | 修正 SpawnManager 生成 Somebody 的問題 |
| `e67004e` | 新增 CLAUDE.md / PROGRESS.md；MCP 與引擎升級評估 |
| （未 commit，`upgrade/unity6`） | 升級到 Unity 6000.3.27f1：batchmode 重新匯入，0 編譯錯誤 / 0 警告。套件自動更新；Unity 自動加入 `com.unity.ai.navigation` 2.0.16 與 `com.unity.multiplayer.center`；手動移除已淘汰的 `com.unity.ide.vscode` |
| （未 commit） | 安裝 MCP for Unity（`com.coplaydev.unity-mcp`，git URL 鎖定 `#v10.3.0`，最低 Unity 2021.3）；用 winget 安裝 uv 0.13.6；新增專案層級 `.mcp.json`（HTTP，`UnityMCP` → `http://localhost:8080/mcp`） |
| （未 commit） | 修 bug：Player 重力（`_gravity * Time.deltaTime` 累加、落地重設為 -1、套用到 `Move`）；SpawnManager 改用 `_startPosition.Length` / `_endPosition.Length` |
| （未 commit） | `.gitignore` 補上 `UserSettings/`、`Recordings/`、`*.slnx`、`.idea/`、`.vscode/`、`.claude/settings.local.json` 等 |
| 驗證 | batchmode 開啟場景：`NavMesh.CalculateTriangulation()` 有 96 個頂點 / 42 個三角形，Car 的 NavMeshAgent 在 NavMesh 上 → 舊 NavMesh 在 Unity 6 可直接使用，**不需要重新烘焙** |

## 決策
- **[已決定 2026-10-31] 遊戲規格以 `GDD_台灣電影拍攝現場.md`（《現場準備！》）為準，但引擎維持 Unity（非 GDD 原寫的 Godot），且做 3D（非 GDD 原寫的 2D）。** 使用者決定。GDD 的多人、觸控、多場景等仍為長期目標；`Docs/Design/GameLoop.md` 需對齊 GDD。
- **[已決定 2026-10-31] 選項 (a)：升級到 Unity 6 LTS（6000.3.27f1）+ CoplayDev/unity-mcp。** 使用者核准。
  - MCP 走 HTTP transport + 專案根目錄 `.mcp.json`：所有 Claude Code 分頁可同時共用同一個 Unity Editor（stdio 模式一次只能一個 client）。
  - 套件鎖定 `v10.3.0` tag，不追 `#main`，避免上游更新突然壞掉；要升級時改 manifest 的 tag。
  - NavMesh 維持舊版烘焙資料；之後若要改場景地形或重新烘焙，改用 AI Navigation 的 NavMeshSurface。
  - 備案：Unity 官方 MCP（`com.unity.ai.assistant`，需 Unity 6）。
- 先前評估（2019.4 時）：主流 MCP 都不支援 2019.4，要用 MCP 就得升級；升級風險低（專案小、沒有第三方套件）。

## 未解問題
1. ~~Player 落地問題~~：已在 `upgrade/unity6` 修正（未 commit，待 Editor 實測）。
2. ~~SpawnManager 寫死範圍~~：已修正（未 commit）。
3. **Timer 結束後沒有行為**：時間到只設 `GameManager.IsTimeGoing = false` 並 log，沒有任何系統讀取它（生成/車輛/玩家都不會停），也沒有結算。
4. **Somebody 躲避未啟用**：`OnTriggerEnter` 被註解掉，`_isSomebodyNoticed` 永遠不會變成 true；`_detectRange` 只用在 Gizmo；`_controller` 欄位沒用到。
5. （次要）Player 旋轉用 `_speed` 當每幀旋轉角度，速度跟旋轉綁在一起，而且受 frame rate 影響。
6. （次要）CarMoves 每幀呼叫 `SetDestination`。
7. （待確認）Player 在 `Update` 直接改 `transform.position` 做範圍 clamp，但 `Physics.autoSyncTransforms` 為 0，CharacterController 可能不會同步到 clamp 後的位置。2019 時就是這樣寫的，請在 Editor 實測邊界行為。
8. （待處理）場景與 prefab 還是 2019 的序列化格式；在 Editor 裡存檔時才會轉成新格式，到時會出現一大批 YAML diff，屬正常現象。
9. （待確認）專案根目錄出現空資料夾 `Docs/Design/`（2026-10-31 12:02 建立，不是 Engineer 建的，可能是 Designer）。

## 下一步
1. **使用者在 Editor 手動操作**：用 Unity 6000.3.27f1 開專案 → Window → MCP for Unity 確認 uv/Python 都是綠燈 → 按 Start Server（建議勾 Auto-Start）→ Claude Code 執行 `/mcp` 確認 `UnityMCP` 已連線。
2. 使用者實測 Play：玩家會貼地、不會浮空，走出平台邊緣會往下掉；路人正常生成；車子正常往返。
3. 實測通過後 PM commit 並 merge `upgrade/unity6` → main（建議另外在 Editor 開場景存一次，讓場景序列化格式的 diff 單獨成一個 commit）。
4. Gameplay Designer 定義 Timer 結束後的遊戲流程（勝負/計分），再由 Engineer 實作問題 3。
5. 確認 Somebody 躲避玩家的設計後重新啟用（問題 4）。
