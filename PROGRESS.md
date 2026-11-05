# PROGRESS.md — Project Rolling 進度紀錄

> context 被清掉後從這裡接續工作。每次完成任務後更新對應段落。最後更新：2026-11-05（PM）

## 目前階段
早期 prototype（單一場景 `Prototype.unity`）。核心移動、撿放物品、路人生成、車輛往返、倒數計時已有雛形；尚無勝負/計分流程。
本輪（2026-10-31 結束）：分支 `upgrade/unity6` 已完成 Unity 6000.3.27f1 升級 + MCP for Unity + 兩個 bug 修正，並加入使用者 GDD v0.2 與對齊 GDD 的第一切片設計 `Docs/Design/GameLoop.md`。全部已 commit 並 push 到 `origin/upgrade/unity6`（尚未 merge 回 main）。

### ⏸ 下次開工時 PM 必須先向使用者確認的待決事項
**A. Unity 6 升級的 Editor 實測（使用者手動）**：用 6000.3.27f1 開專案 → Window → MCP for Unity 確認 uv/Python 綠燈 → Start Server（建議勾 Auto-Start；不要按 Claude Code 的 Configure）→ `/mcp` 確認 UnityMCP 連線 → Play 測：玩家貼地/邊緣會掉落、路人生成、車子往返、移動邊界是否正確。通過後 PM 才 merge `upgrade/unity6` → main（Editor 存檔產生的場景 YAML diff 另開一個 commit）。

**B. 第一切片設計 `Docs/Design/GameLoop.md` 的 6 個待確認問題**（確認後才派 Engineer 實作）：
1. 時間：準備 45 秒／每 Take 有效拍攝 20 秒／全局 120 秒？準備到點自動 Action，或清場完可提早開拍？
2. 重拍：NG 至少整理 5 秒、只受全局時間限制（不設次數上限）？玩家本人入鏡算 NG 嗎？
3. 原型輸入：先用鍵盤（WASD、E 撿放、F 請離開／按住舉牌），觸控留後續？
4. 首片內容量：三類任務＋三種 NPC＋汽機車＋兩種交管全放？3 件雜物／6 位 NPC／4 次車流是否合適，或先縮減？
5. 設備停拍：停拍 2 秒、保留 Take 進度、期間入鏡照常 NG？
6. 評星：任務／時間／失誤權重 50／30／20、2 星 60 分、3 星 85 分？時間耗盡未完成鏡頭給 1 星或 0 星？

**C. ~~轉移到 Mac~~：已完成（2026-11-05 確認）。** 開發改在 macOS：專案在 `/Users/mouse76229/Dev Projects/ProjectRolling`，Unity 6000.3.27f1 已裝在 `/Applications/Unity/Hub/Editor/`，Editor 已能開啟專案（0 編譯錯誤）。`uv`/`uvx` 在 `~/.local/bin`；git 認證改用 `osxkeychain`，`gh` 已安裝；herdr 分工已在 workspace `w5` 重建（PM `w5:p1`、Gameplay Designer `w5:p2` codex、Designer `w5:p3`、Engineer `w5:p4`；IT 不建）。CLAUDE.md 的 Windows 路徑已改成 Mac 路徑。
   - MCP 已連線（2026-11-05）：Server 在 `127.0.0.1:8181`（8080 被 Docker 占用），Console 0 錯誤。

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
| `3a703b1` | 升級到 Unity 6000.3.27f1：batchmode 重新匯入，0 編譯錯誤 / 0 警告。套件自動更新；Unity 自動加入 `com.unity.ai.navigation` 2.0.16 與 `com.unity.multiplayer.center`；手動移除已淘汰的 `com.unity.ide.vscode` |
| `3a703b1` | 安裝 MCP for Unity（`com.coplaydev.unity-mcp`，git URL 鎖定 `#v10.3.0`，最低 Unity 2021.3）；用 winget 安裝 uv 0.13.6；新增專案層級 `.mcp.json`（HTTP，`UnityMCP` → `http://localhost:8080/mcp`） |
| `3a703b1` | 修 bug：Player 重力（`_gravity * Time.deltaTime` 累加、落地重設為 -1、套用到 `Move`）；SpawnManager 改用 `_startPosition.Length` / `_endPosition.Length` |
| `3a703b1` | `.gitignore` 補上 `UserSettings/`、`Recordings/`、`*.slnx`、`.idea/`、`.vscode/`、`.claude/settings.local.json` 等 |
| `607f749` | Gameplay Designer 第一版遊戲循環草案（已被 5369c45 取代） |
| `5369c45` | 加入使用者 GDD（`GDD_台灣電影拍攝現場.md`）v0.2：引擎改 Unity 6、美術改 3D；`Docs/Design/GameLoop.md` 改寫為忠於 GDD 的單人第一切片（假設 A1–A21 待確認） |
| `f203987` `c949601` | Mac 環境文件更新；MCP 設定改由插件管理（`.mcp.json` 清空、`~/.claude.json` local scope → `127.0.0.1:8181`） |
| `8112037` | 改用新 Input System（`com.unity.inputsystem` 1.20.1，Active Input Handling = 新版 only）：新增 `Assets/Input/PlayerControls.inputactions`（Move/Sprint/Interact，鍵盤＋搖桿，Touch scheme 預留空殼）、`Player.cs` 改讀 action、EventSystem 換 `InputSystemUIInputModule`。使用者 Play 實測移動/加速/撿放正常。場景與 prefab 首次以 Unity 6 存檔，diff 含大量序列化重寫 |
| 驗證 | batchmode 開啟場景：`NavMesh.CalculateTriangulation()` 有 96 個頂點 / 42 個三角形，Car 的 NavMeshAgent 在 NavMesh 上 → 舊 NavMesh 在 Unity 6 可直接使用，**不需要重新烘焙** |

## 決策
- **[已決定 2026-10-31] 遊戲規格以 `GDD_台灣電影拍攝現場.md`（《現場準備！》）為準，但引擎維持 Unity（非 GDD 原寫的 Godot），且做 3D（非 GDD 原寫的 2D）。** 使用者決定。GDD 的多人、觸控、多場景等仍為長期目標；`Docs/Design/GameLoop.md` 需對齊 GDD。
- **[已決定 2026-10-31] 選項 (a)：升級到 Unity 6 LTS（6000.3.27f1）+ CoplayDev/unity-mcp。** 使用者核准。
  - MCP 走 HTTP transport：所有 Claude Code 分頁可同時共用同一個 Unity Editor（stdio 模式一次只能一個 client）。（原本用專案根目錄 `.mcp.json`，已被下方 2026-11-05 的決定取代。）
  - 套件鎖定 `v10.3.0` tag，不追 `#main`，避免上游更新突然壞掉；要升級時改 manifest 的 tag。
  - NavMesh 維持舊版烘焙資料；之後若要改場景地形或重新烘焙，改用 AI Navigation 的 NavMeshSurface。
  - 備案：Unity 官方 MCP（`com.unity.ai.assistant`，需 Unity 6）。
- **[已決定 2026-11-05] MCP 設定交給 MCP for Unity 插件管理。** 使用者決定。插件 Start Server 時自動寫入 `~/.claude.json` 的 local scope（目前 `127.0.0.1:8181`），並清空 `.mcp.json`；`.mcp.json` 以清空狀態 commit，不再手動維護。取代先前「HTTP + 專案根目錄 `.mcp.json`」的做法。
- 先前評估（2019.4 時）：主流 MCP 都不支援 2019.4，要用 MCP 就得升級；升級風險低（專案小、沒有第三方套件）。

## 未解問題
1. ~~Player 落地問題~~：已修正（`3a703b1`，待 Editor 實測）。
2. ~~SpawnManager 寫死範圍~~：已修正（`3a703b1`）。
3. **Timer 結束後沒有行為**（設計已在 GameLoop.md 定義，待使用者確認後實作）：時間到只設 `GameManager.IsTimeGoing = false` 並 log，沒有任何系統讀取它（生成/車輛/玩家都不會停），也沒有結算。
4. **Somebody 躲避未啟用**：`OnTriggerEnter` 被註解掉，`_isSomebodyNoticed` 永遠不會變成 true；`_detectRange` 只用在 Gizmo；`_controller` 欄位沒用到。
5. （次要）Player 旋轉用 `_speed` 當每幀旋轉角度，速度跟旋轉綁在一起，而且受 frame rate 影響。
6. （次要）CarMoves 每幀呼叫 `SetDestination`。
7. **移動邊界過小**（2026-11-05 使用者實測）：玩家貼地、邊緣不會掉，但活動範圍感覺比地板小、像有隱形牆。來源是 `Player.cs` 寫死的 clamp x∈[-9,9]、z∈[-9,6]（`ac12191` 擴大地板後沒跟著調整）。另外 `Physics.autoSyncTransforms` 為 0，直接改 `transform.position` 可能不同步到 CharacterController。待決定新範圍（或改用碰撞邊界）。
8. （待處理）場景與 prefab 還是 2019 的序列化格式；在 Editor 裡存檔時才會轉成新格式，到時會出現一大批 YAML diff，屬正常現象。
9. ~~Docs/Design/ 空資料夾~~：是 Gameplay Designer 建的，已放入 GameLoop.md。
10. **路人與車子只有單向移動**（2026-11-05 使用者實測回報）：符合目前程式設計（Somebody 走到終點 3 秒後銷毀；Car 開到 B 後瞬移回 A），待使用者確認期望行為。
11. （次要，Engineer 2026-11-05 發現）Sprint 用「按下 ×2、放開 ÷2」：放開那幀沒偵測到（例如切換視窗）會卡在 2 倍，再按變 4 倍；Player 同時掛 CharacterController 與 kinematic Rigidbody。
12. （次要）Console 有 2 則 `Can't Generate Mesh, No Font Asset has been assigned.`（TextMeshPro 元件缺字型），來源待查。

## 下一步
1. 下次開工：PM 先向使用者確認「目前階段」中的待決事項 A（Editor 實測）與 B（6 個設計問題）。
2. A 通過 → PM merge `upgrade/unity6` → main。
3. B 確認 → Gameplay Designer 依回覆定稿 GameLoop.md → Engineer 依定稿實作第一切片（GameManager 狀態機、計時、任務帳本、交管、NPC 疏散、雜物、設備停拍、結算 UI）。
4. Designer（Impeccable 已安裝，首次派工前需 `/reload-plugins`）可依 GDD 美術方向（3D 低多邊形、chibi、台灣街景色調、固定俯視）提出視覺方向。
