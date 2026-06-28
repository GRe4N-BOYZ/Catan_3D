# Frontier Conquest 3D

Unityで開発中の3D戦略ボードゲームです。

プレイヤーは道路や開拓地を建設し、資源を獲得しながら領土を拡張していきます。

六角形タイルを利用した資源管理ゲームをベースに、領地占領や対戦要素を組み合わせた独自のゲーム体験を目指しています。

---

## Overview

本作品は、

- Unityによるゲーム開発
- オブジェクト指向設計
- グラフ構造を用いた盤面管理
- Blenderによる3Dモデリング

の学習を目的として制作しています。

単なるボードゲームの再現ではなく、プレイヤー同士の競争や領地争奪要素を加えたオリジナル作品として開発しています。

---

## Current Features

### 実装済み

- 六角形タイル(HexTile)による盤面構築
- プレイヤー管理システム
- 開拓地(Settlement)建設
- 街道(Road)建設
- 初期配置フェーズ
- 建設可能判定
- 隣接建設制限
- 道路接続判定
- プレイヤーカラー対応
- GameStateによる状態管理
- Blender製オリジナル3Dモデル

### 開発中

- サイコロシステム
- 資源獲得システム
- 都市(City)
- 勝利点システム
- 領地占領システム
- ミニゲームシステム
- UI実装
- サウンド実装

---

## Architecture

ゲーム盤面はグラフ構造として管理しています。

```text
HexTile
  ├─ Vertex
  │    └─ Settlement
  │
  └─ Edge
       └─ Road
```

### HexTile

六角形の地形タイルです。

保持情報

- ResourceType
- DiceNumber
- AdjacentVertices

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
NormalTurn
```

各状態ごとに操作可能なオブジェクトを制御しています。

---

## Technologies

### Engine

- Unity

### Programming

- C#

### Modeling

- Blender

### Version Control

- Git
- GitHub

---

## What I Learned

この制作を通して以下の技術を学習しました。

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

## Future Plans

今後は以下の機能を追加予定です。

- 資源システム完成
- 都市システム
- 勝利点システム
- 領地占領システム
- ミニゲームによる戦闘
- AIプレイヤー
- オンライン対戦対応検討

---

## Author

Individual Project

### Roles

- Planning
- Programming
- 3D Modeling
- Game Design

Developed by Ugallun
