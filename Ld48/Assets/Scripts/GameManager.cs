﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using yaSingleton;
using Random = UnityEngine.Random;


[CreateAssetMenu(fileName = "GameManager", menuName = "Singletons/GameManager")]
public class GameManager : Singleton<GameManager> {
	public bool IsDebugMode {
		get => isDebugMode;
		set {
			if (isDebugMode != value) {
				isDebugMode = value;
				OnDebugModeChange?.Invoke(isDebugMode);
			}
		}
	}
	public Action<bool> OnDebugModeChange;
#if UNITY_EDITOR
	bool isDebugMode = true;
#else
	bool isDebugMode = false;
#endif

	public int HelpLevelMode {
		get => helpLevelMode;
		set {
			if (helpLevelMode != value) {
				helpLevelMode = value;
				OnHelpModeChange?.Invoke(helpLevelMode);
			}
		}
	}
	public Action<int> OnHelpModeChange;
	int helpLevelMode = 0;

	public Cell SelectedCell => hightlightBlockUnderMouse.lastHightlightedCell;

	[ReadOnly] public MouseTooltip tooltip;

	[NonSerialized] public Player player;
	[NonSerialized] public HightlightBlockUnderMouse hightlightBlockUnderMouse;
	[NonSerialized] public Grid grid;
	[NonSerialized] public GameObject draggedParent;

	[Header("Help"), Space]
	[Tooltip("Inclusive minimum value")] public int minHelpLevel = 0;
	[Tooltip("Inclusive maximum value")] public int maxHelpLevel = 2;
	[Tooltip("")] public int startHelpLevel = 2;

	[Header("Cells"), Space]
	public ForegroundGoData[] foregroundCells;
	public BackgroundGoData[] backgroundCells;
	public OreGoData[] oreCells;

	[Header("Item on ground"), Space]
	public ItemOnGround[] itemsOnGroundPrefabs;

	[Header("Crafts"), Space]
	public CraftSO[] crafts;

	protected override void Initialize() {
		base.Initialize();

		helpLevelMode = startHelpLevel;

		StartCoroutine(DelayedSetup());

		IEnumerator DelayedSetup() {
			yield return null;
			yield return null;
		}
	}

	protected override void Deinitialize() {
		base.Deinitialize();
	}

	public Cell GetCellAtMousePosWithInteractClamp() {
		return GetCellAtPosWithInteractClamp(TemplateGameManager.Instance.Camera.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue()).SetZ(0.0f));
	}

	public Cell GetCellAtPosWithInteractClamp(Vector3 pos) {
		Vector3 diff = Vector3.ClampMagnitude(pos - player.mover.transform.position, player.maxInteractDistance);
		return GetCellAtPos(player.mover.transform.position + diff);
	}

	public Cell GetCellAtPos(Vector3 pos) {
		return grid.GetCellWorldPos(pos);
	}

	public Cell GetCellAtMousePosWithInteractClamp(out Vector2 actualPos) {
		actualPos = TemplateGameManager.Instance.Camera.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue()).SetZ(0.0f);
		return GetCellAtPosWithInteractClamp(actualPos, out actualPos);
	}

	public Cell GetCellAtPosWithInteractClamp(Vector3 pos, out Vector2 actualPos) {
		Vector3 diff = Vector3.ClampMagnitude(pos - player.mover.transform.position, player.maxInteractDistance);
		actualPos = player.mover.transform.position + diff;
		return GetCellAtPos(actualPos);
	}

	public GameObject GetCellForeground(Cell.CellContentForegroud type) {
		foreach (var data in foregroundCells) {
			if (data.type == type)
				return data.prefab;
		}

		Debug.LogError($"Can't find Cell Foreground - {type}");
		return foregroundCells[0].prefab;
	}

	public GameObject GetCellBackground(Cell.CellContentBackground type) {
		foreach (var data in backgroundCells) {
			if (data.type == type)
				return data.prefab;
		}

		Debug.LogError($"Can't find Cell Background - {type}");
		return backgroundCells[0].prefab;
	}

	public GameObject GetCellOre(Cell.CellContentOre type) {
		foreach (var data in oreCells) {
			if (data.type == type)
				return data.prefab;
		}

		Debug.LogError($"Can't find Cell Ore - {type}");
		return oreCells[0].prefab;
	}

	public GameObject GetItemOnGround(ItemSO.ItemType type) {
		foreach (var data in itemsOnGroundPrefabs) {
			if (data.item.itemSO.type == type)
				return data.gameObject;
		}

		Debug.LogError($"Can't find item on ground - {type}");
		return itemsOnGroundPrefabs[0].gameObject;
	}

	public PlacebleBlock GetItemPlacable(ItemSO.ItemType type) {
		foreach (var data in itemsOnGroundPrefabs) {
			if (data.item.itemSO.type == type)
				return data.placebleBlock;
		}

		Debug.LogError($"Can't find placeble item on ground - {type}");
		return itemsOnGroundPrefabs[0].placebleBlock;
	}

	[Serializable]
	public struct ForegroundGoData {
		public Cell.CellContentForegroud type;
		public GameObject prefab;
	}

	[Serializable]
	public struct BackgroundGoData {
		public Cell.CellContentBackground type;
		public GameObject prefab;
	}

	[Serializable]
	public struct OreGoData {
		public Cell.CellContentOre type;
		public GameObject prefab;
	}
}
