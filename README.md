# Frontier Conquest 3D

Unityで開発している3Dストラテジーボードゲームです。

六角形グリッドをグラフ構造として設計し、オブジェクト指向設計と状態遷移(State Machine)を用いてゲームルールを実装しています。

カタンをベースに、「領地占領」「戦闘」「ミニゲーム」など独自要素を追加した作品を開発中です。

ゲーム開発を通して、

- オブジェクト指向設計
- グラフ構造
- 状態管理(State Machine)
- 3Dゲーム制作

を学習・実践することを目的としています。

---

## Current Features

### Gameplay

- 初期配置フェーズ
- ターン制進行
- サイコロによる資源配布
- 開拓地建設
- 街道建設
- 都市へのアップグレード

### Rule System

- 建設可能判定
- 道路接続判定
- 隣接建設禁止
- 初期配置ルール

### System

- GameStateによる状態管理
- PlayerManagerによるターン管理
- ResourceManagerによる資源配布
- グラフ構造による盤面管理

### Assets

- Blender製オリジナルモデル

### Architecture

- Object-Oriented Design
- Graph Structure
- State Machine
- Responsibility Separation

---

## Design

盤面をグラフ構造として設計しました。

VertexとEdgeが互いに接続情報を保持することで、

- 建設判定
- 隣接判定
- 道路接続判定
- 資源配布

などをシンプルに実装しています。

また、
建設ルールはVertex(建物)・Edge(街道)自身が保持し、
HexTileは地形、PlayerManagerはターン、GameManagerは状態遷移のみを担当する責務分離を意識しています。

```text
GameManager
      │
      ▼
PlayerManager

      │

HexBoard
 ├── HexTile
 │
 ├── Vertex
 │     └ Building
 │          ├ Settlement
 │          └ City
 │
 └── Edge
       └ Road
```

### HexTile

六角形の地形タイルです。

保持情報

- resourceType
- numberToken
- adjacentVertices

---

### Vertex

タイル同士の交点です。

担当機能

- 開拓地建設
- 都市建設
- 隣接判定
- 建設可能判定

---

### Edge

交点同士を結ぶ辺です。

担当機能

- 街道建設
- 接続判定
- 道路ネットワーク管理

---

この構造によって、

- 建設可能判定
- 隣接判定
- 道路接続判定
- 資源配布判定

をシンプルに実装できるようにしています。

---

## State Management

ゲーム進行は状態遷移によって管理しています。

```text
InitialSettlement
        ↓
InitialRoad
        ↓
RollDice
        ↓
PlayerAction
        ↓
EndTurn
        ↓
RollDice
```

各状態ごとに操作可能なオブジェクトを制御しています。

---

## Technologies

Engine

- Unity 6

Language

- C#

Architecture

- Object-Oriented Design
- Graph Structure
- State Machine

Tools

- Blender
- Visual Studio Code

Version Control

- Git
- GitHub

---

## What I Learned

この制作を通して、

ゲーム制作では「動けば良い」のではなく、

- クラスの責務分離
- 保守性
- 拡張性

を意識した設計の重要性を学びました。

特に、

ゲームルールをManagerへ集中させるのではなく、

盤面オブジェクト自身(Vertex・Edge)にルールを持たせる設計を採用しました。

これにより、

- 処理の見通し
- 拡張性
- 責務分離

を意識した実装になっています。

### プログラミング

- オブジェクト指向設計
- 状態遷移管理
- グラフ構造
- プレイヤー管理
- ゲームルール実装

### Unity

- Prefab管理
- Layer制御
- Raycast
- Script設計

### 3D制作

- Blenderモデリング
- FBXエクスポート
- Unityとの連携

### 開発運用

- Git
- GitHub
- バージョン管理

---

## Future Plans Roadmap

### Phase 1

- 都市
- 勝利点
- 資源交換

### Phase 2

- 発展カード
- 港システム
- 最大街道

### Phase 3

- AIプレイヤー
- オリジナル領地占領システム
- ミニゲーム戦闘

### Phase 4

- Save / Load
- AI
- Networking

---

## Author

Individual Project

### Roles

- Planning
- Programming
- 3D Modeling
- Game Design

Developed by Ugallun
