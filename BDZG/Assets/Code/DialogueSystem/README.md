# 对话系统使用说明

本文件夹包含一套基于 **ScriptableObject（`DialogueNode`）** 的轻量对话：场景中由 **`DialogueManager`** 驱动流程，**`DialogueUI`** 负责显示与按钮，**`DialogueLauncher`** 作为对外入口（按钮或脚本调用）。

---

## 1. 场景里需要有什么

| 组件 | 作用 |
|------|------|
| **DialogueManager** | 单例，维护当前节点、发事件、处理「继续」与选项跳转。场景中应只有 **一个**。 |
| **DialogueUI** | 订阅 Manager 事件，显示姓名、正文、立绘、继续按钮与选项按钮；对话开始显示根物体，结束隐藏。 |
| **DialogueLauncher**（可选） | 封装 `Begin` / `Stop`、开始与结束时的 UnityEvent / C# 事件。 |

**启动顺序**：`DialogueUI` 在 `Start` 里会访问 `DialogueManager.Instance`，请保证 **Manager 的 `Awake` 先于 UI 的 `Start` 执行**（通常把 Manager 放在层级里更靠上，或挂在同一物体上且脚本顺序 Manager 在 UI 之前）。

---

## 2. 对话数据：`DialogueNode` 资源

在 Project 窗口：**Create → Dialogue → DialogueNode**，得到 `.asset` 文件。

每个节点可配置：

- **Speaker Name**：显示在姓名栏。
- **Character Id**：立绘查表用；**留空**时 UI 会用 `speakerName` 作为查表键。
- **Dialogue Text**：当前句正文。
- **Options**：选项列表（见下）。
- **Next Node**：**无选项**时，玩家点「继续」后进入的下一节点；最后一节可留空表示对话结束。

### 2.1 线性对话（只有继续、没有分支）

- `options` 留空或数量为 0。
- 用 **Next Node** 串成链：A → B → C；最后一句的 **Next Node** 为空则结束。

### 2.2 分支对话（有选项）

- 在 **Options** 里添加多条，每条填写：
  - **Option Text**：选项文案（会写入按钮上的 **TextMeshProUGUI**）。
  - **Target Node**：选中后跳转到的节点。
  - **Option Button Prefab**（可选）：该选项专用按钮预制体；**留空**则用 `DialogueUI` 上的默认选项按钮预制体。
- 若某选项的 **Target Node** 为空，选中后会 **直接结束整段对话**。

---

## 3. `DialogueUI` Inspector 绑定要点

- **Speaker Name Text / Dialog Text**：`TextMeshProUGUI`。
- **Portrait Image / Portrait Database**：可选；数据库为 **Create → Dialogue → Character Portrait Database**，其中每条 **Character Id** 需与节点上的 `characterId`（或回退用的 `speakerName`）一致（不区分大小写）。
- **Options Parent**：选项按钮实例化的父级 `Transform`。
- **Option Button Prefab**：默认选项按钮预制体，须满足：
  - 子层级中有 **`TextMeshProUGUI`**（用于显示 `optionText`）；
  - 有 **`Button`**（可挂在根或子物体上）。
- **Continue Button**：无选项时显示的继续按钮，点击会调用 `DialogueManager.Advance()`。

**选项不显示文字**：常见原因是预制体里没有激活的 **TMP** 组件、用了旧版 **UI Text**、或 `DialogueOption.optionText` 在资源里为空。详见控制台 Warning/Error。

---

## 4. `DialogueLauncher`：如何开始 / 结束对话

### 4.1 Inspector

- **Default Start Node**：无参 **Begin()** 时使用的起点 `DialogueNode`。
- **On Dialogue Began / On Dialogue Ended**：UnityEvent，可绑音效、任务标记等。

### 4.2 代码

```csharp
// 使用 Inspector 里配置的默认起点
launcher.Begin();

// 指定起点（例如随机多套固定稿中的一套）
launcher.Begin(someDialogueNodeAsset);

// 打断当前对话（若由本 Launcher 开启，仍会收到结束回调）
launcher.Stop();

// 运行时改默认起点再无参开始
launcher.DefaultStartNode = anotherNode;
launcher.Begin();
```

**注意**：若已有对话在进行、或上一轮 Launcher 会话尚未结束，再次 `Begin` 会被忽略并打 Warning。

---

## 5. 不经过 Launcher、直接开对话

若场景里已有 `DialogueManager`：

```csharp
DialogueManager.Instance.StartDialogue(startNode);
```

结束可调用 `DialogueManager.Instance.StopDialogue()`。自行订阅 `OnDialogueEnded` 等事件时，注意在对象销毁时取消订阅，避免泄漏。

---

## 6. 多套固定内容「随机一种」的做法（概念）

每种内容一套 **独立的 `DialogueNode` 根资源**（各自链或分支写死在 asset 里）。在调用 `Begin` 前由你的逻辑 **随机选一个根节点**（或按任务条件选），再 `Begin(选中的根)` 或赋值 `DefaultStartNode` 后 `Begin()`。无需改 Manager/UI 核心逻辑，可加一个很薄的「随机路由器」脚本专门负责选节点。

---

## 7. 预制体参考

- **OptionButton.prefab**：默认选项按钮示例，含 TMP 子物体与 Button，可在 `DialogueUI` 的 **Option Button Prefab** 中引用。

---

## 8. 文件一览

| 文件 | 说明 |
|------|------|
| `DialogueNode.cs` | 节点与选项数据结构 + CreateAssetMenu |
| `DialogueManager.cs` | 流程与事件 |
| `DialogueUI.cs` | UI 与选项实例化 |
| `DialogueLauncher.cs` | 对外入口、`Begin` / `Stop`、事件 |
| `CharacterPortraitDatabase.cs` | 角色 ID → 立绘 |
| `OptionButton.prefab` | 默认选项按钮预制体 |

示例场景若工程中有 `Assets/Scenes/Dialogue.unity`，可对照其中的 `DialogueManager`、`DialogueUI`、`Launcher` 布置方式。

---

## 9. 常见问题：Launch / 继续 / 选项点不了

1. **DialogueUI 根物体 RectTransform 的 Scale 为 (0,0,0)**  
   其下所有子物体（含 Launcher、选项、继续）的 UI 交互会异常。请将 **Scale 改为 (1,1,1)**。

2. **`DialogueUI` 的 Option Button Prefab 为 Missing 或 GUID 错误**  
   应拖入 `Assets/Code/DialogueSystem/OptionButton.prefab`，否则有选项时无法正确生成按钮。

3. **场景缺少 `EventSystem`**，或 **Canvas 未启用 `Graphic Raycaster`**（本场景的 DialogueUI Canvas 已带 Raycaster，勿删）。

4. **全屏 Image 打开 Raycast Target** 且盖在上层，会挡住点击；可调层级或关闭该 Image 的 Raycast Target。
