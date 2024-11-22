using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
public class SquareBoard : Board
{
    [Header("Grid Size")]
    [SerializeField] Vector2Int m_gridSize;
    public Vector2Int gridSize => m_gridSize;

    [Header("Grid Element Layout")]
    [SerializeField] Vector2 gridElementSize;
    [SerializeField] Vector2 gridSpacing;

    [Header("Tiles")]
    [SerializeField] SquareTile[] normalTiles;
    [SerializeField] SquareTile[] specialTiles;

    [Header("Tile Generation")]
    [SerializeField] float specialChance = 5.0f;

    [Header("Debug")]
    [SerializeField] string FSMPath = "";

    [HideInInspector] public SquareTile[,] tiles;

    TopLayer topLayer;

    private void OnValidate()
    {
        rectTransform.sizeDelta = new Vector2(gridElementSize.x * gridSize.x + gridSpacing.x * (gridSize.x + 1), gridElementSize.y * gridSize.y + gridSpacing.y * (gridSize.y + 1));
    }
    public Vector2 GetPos(int x, int y)
    {
        float posX;
        if(gridSize.x % 2 == 0)
        {
            posX = (x - gridSize.x / 2.0f + 0.5f) * gridSpacing.x + (x - gridSize.x / 2.0f + 0.5f) * gridElementSize.x;
        }
        else
        {
            posX = (x - gridSize.x / 2.0f + 0.5f) * gridElementSize.x + (x - gridSize.x / 2.0f + 0.5f) * gridSpacing.x;
        }
        float posY;
        if (gridSize.y % 2 == 0)
        {
            posY = (y - gridSize.y / 2.0f + 0.5f) * gridSpacing.y + (y - gridSize.y / 2.0f + 0.5f) * gridElementSize.y;
        }
        else
        {
            posY = (y - gridSize.y / 2.0f + 0.5f) * gridElementSize.y + (y - gridSize.y / 2.0f + 0.5f) * gridSpacing.y;
        }
        return transform.TransformPoint(new Vector2(posX, -posY));
    }
    public Vector2 GetPos(Vector2Int pos) => GetPos(pos.x, pos.y);
    public SquareTile CreateRandomTile()
    {
        if(UnityEngine.Random.Range(0, 100.0f) <= specialChance)
        {
            return CreateRandomSpecialTile();
        }
        else
        {
            SquareTile tmp = normalTiles[UnityEngine.Random.Range(0, normalTiles.Length)].Instantiate(this);
            tmp.rectTransform.sizeDelta = gridElementSize;
            tmp.transform.localScale = new Vector3(1, 1, 1);
            return tmp;
        }
    }
    public SquareTile CreateRandomSpecialTile()
    {
        SquareTile tmp = specialTiles[UnityEngine.Random.Range(0, specialTiles.Length)].Instantiate(this);
        tmp.rectTransform.sizeDelta = gridElementSize;
        tmp.transform.localScale = new Vector3(1, 1, 1);
        return tmp;
    }
    public void GenerateGrid()
    {
        tiles = new SquareTile[gridSize.x, gridSize.y];
        for(int i = 0; i < gridSize.x; i++)
        {
            for(int k = 0; k < gridSize.y; k++)
            {
                tiles[i, k] = CreateRandomTile();
                tiles[i, k].gridPos = new Vector2Int(i, k);
            }
        }
    }
    private void Awake()
    {
        GenerateGrid();
        topLayer = new TopLayer(this, new SquareBoardFSMVals());
#if UNITY_EDITOR
        topLayer.onFSMChange += () => FSMPath = topLayer.GetFSMPath();
#endif
        topLayer.OnStateEnter();
    }
    private void Start()
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int k = 0; k < gridSize.y; k++)
            {
                tiles[i, k].transform.position = GetPos(i, k);
            }
        }
    }
    private void Update()
    {
        topLayer.OnStateUpdate();
    }
    public bool OutOfBound(Vector2Int pos)
    {
        return pos.x >= gridSize.x || pos.x < 0 || pos.y >= gridSize.y || pos.y < 0;
    }
    List<SquareTile> popping = new();
    public void Pop(SquareTile tile)
    {
        if (tile == null || popping.Contains(tile)) return;
        popping.Add(tile);
        tile.Pop((SquareTile tmp) => popping.Remove(tmp));
        combo++;
        onTilePop?.Invoke();
    }
    public void PopAt(Vector2Int pos)
    {
        if (OutOfBound(pos)) return;
        Pop(tiles[pos.x, pos.y]);
    }
    public List<SquareTile> ScanPopTargets()
    {
        List<SquareTile> results = new();
        for(int i = 0; i < gridSize.y; i++)
        {
            int last = 0;
            int k;
            for(k = 1; k < gridSize.x; k++)
            {
                if (tiles[last, i].type != tiles[k, i].type)
                {
                    if(k - last >= requiredLineLength)
                    {
                        for (int l = last; l < k; l++) if (!results.Contains(tiles[l, i])) results.Add(tiles[l, i]);
                    }
                    last = k;
                }
            }
            if (k - last >= requiredLineLength)
            {
                for (int l = last; l < k; l++) if (!results.Contains(tiles[l, i])) results.Add(tiles[l, i]);
            }
        }
        for (int i = 0; i < gridSize.x; i++)
        {
            int last = 0;
            int k;
            for (k = 1; k < gridSize.y; k++)
            {
                if (tiles[i, last].type != tiles[i, k].type)
                {
                    if (k - last >= requiredLineLength)
                    {
                        for (int l = last; l < k; l++) if (!results.Contains(tiles[i, l])) results.Add(tiles[i, l]);
                    }
                    last = k;
                }
            }
            if (k - last >= requiredLineLength)
            {
                for (int l = last; l < k; l++) if (!results.Contains(tiles[i, l])) results.Add(tiles[i, l]);
            }
        }
        return results;
    }
    class SquareBoardFSMVals : FSMVals
    {
        public Vector2 grabTouchBeganPos;
        public SquareTile grabbingTile = null;
        public Vector2Int grabTileMoveDir;

        public List<SquareTile> popTargets;
    }
    class TopLayer : TopLayer<SquareBoard, SquareBoardFSMVals>
    {
        public TopLayer(SquareBoard origin, SquareBoardFSMVals values) : base(origin, values)
        {
            defaultState = new PlayerInteract(origin, this);
            AddState("PlayerInteract", defaultState);
            AddState("Popping", new Popping(origin, this));
            AddState("Moving", new Moving(origin, this));
        }
        class PlayerInteract : Layer<SquareBoard, SquareBoardFSMVals>
        {
            public PlayerInteract(SquareBoard origin, Layer<SquareBoard, SquareBoardFSMVals> parent) : base(origin, parent)
            {
                defaultState = new Waiting(origin, this);
                AddState("Waiting", defaultState);
                AddState("Grabbing", new Grabbing(origin, this));
                AddState("MoveAndCheck", new MoveAndCheck(origin, this));
            }
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                origin.combo = 0;
            }
            class Waiting : State<SquareBoard, SquareBoardFSMVals>
            {
                public Waiting(SquareBoard origin, Layer<SquareBoard, SquareBoardFSMVals> parent) : base(origin, parent)
                {

                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if (InputManager.IsTouchDown() && UIScanner.FindWithTag(InputManager.GetTouchPosition(), "Tile", out GameObject tmp))
                    {
                        if (tmp.transform.parent != origin.transform) return;
                        values.grabTouchBeganPos = InputManager.GetTouchPosition();
                        values.grabbingTile = tmp.GetComponent<SquareTile>();
                        parentLayer.ChangeState("Grabbing");
                    }
                }
            }
            class Grabbing : State<SquareBoard, SquareBoardFSMVals>
            {
                public Grabbing(SquareBoard origin, Layer<SquareBoard, SquareBoardFSMVals> parent) : base(origin, parent)
                {

                }
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    if (values.grabbingTile == null) parentLayer.ChangeState("Waiting");
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if (InputManager.IsTouchOver())
                    {
                        parentLayer.ChangeState("Waiting");
                        return;
                    }
                    else
                    {
                        Vector2 moved = InputManager.GetTouchPosition() - values.grabTouchBeganPos;
                        if(moved.magnitude > 5.0f)
                        {
                            Vector2Int dir;
                            if (Mathf.Abs(moved.x) > Mathf.Abs(moved.y))
                            {
                                if (moved.x > 0) dir = new Vector2Int(1, 0);
                                else dir = new Vector2Int(-1, 0);
                            }
                            else
                            {
                                if (moved.y > 0) dir = new Vector2Int(0, -1);
                                else dir = new Vector2Int(0, 1);
                            }
                            values.grabTileMoveDir = dir;
                            parentLayer.ChangeState("MoveAndCheck");
                            return;
                        }
                    }
                }
            }
            class MoveAndCheck : State<SquareBoard, SquareBoardFSMVals>
            {
                public MoveAndCheck(SquareBoard origin, Layer<SquareBoard, SquareBoardFSMVals> parent) : base(origin, parent)
                {

                }
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    if (values.grabbingTile == null || origin.OutOfBound(values.grabbingTile.gridPos + values.grabTileMoveDir)) parentLayer.ChangeState("Waiting");
                    else origin.StartCoroutine(Move());
                }
                IEnumerator Move()
                {
                    Vector2Int originPos = values.grabbingTile.gridPos;
                    Vector2Int targetPos = originPos + values.grabTileMoveDir;
                    SquareTile originTile = values.grabbingTile;
                    SquareTile movingTile = origin.tiles[targetPos.x, targetPos.y];
                    bool originMoving = true, movingMoving = true;
                    originTile.MoveTo(origin.GetPos(movingTile.gridPos), (SquareTile tmp) => originMoving = false);
                    movingTile.MoveTo(origin.GetPos(originTile.gridPos), (SquareTile tmp) => movingMoving = false);
                    while (originMoving || movingMoving) yield return null;
                    origin.tiles[targetPos.x, targetPos.y] = originTile;
                    origin.tiles[originPos.x, originPos.y] = movingTile;
                    originTile.gridPos = targetPos;
                    movingTile.gridPos = originPos;
                    List<SquareTile> targets = origin.ScanPopTargets();
                    if(targets.Count == 0)
                    {
                        yield return new WaitForSeconds(0.5f);
                        originMoving = true; movingMoving = true;
                        originTile.MoveTo(origin.GetPos(originPos), (SquareTile tmp) => originMoving = false);
                        movingTile.MoveTo(origin.GetPos(targetPos), (SquareTile tmp) => movingMoving = false);
                        while (originMoving || movingMoving) yield return null;
                        origin.tiles[targetPos.x, targetPos.y] = movingTile;
                        origin.tiles[originPos.x, originPos.y] = originTile;
                        originTile.gridPos = originPos;
                        movingTile.gridPos = targetPos;
                        parentLayer.ChangeState("Waiting");
                        yield break;
                    }
                    else
                    {
                        values.popTargets = targets;
                        parentLayer.parentLayer.ChangeState("Popping");
                        yield break;
                    }
                }
            }
        }
        class Popping : State<SquareBoard, SquareBoardFSMVals>
        {
            public Popping(SquareBoard origin, Layer<SquareBoard, SquareBoardFSMVals> parent) : base(origin, parent)
            {

            }
            List<SquareTile> popping = new();
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                foreach(var i in values.popTargets)
                {
                    origin.Pop(i);
                }
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if (origin.popping.Count == 0)
                {
                    parentLayer.ChangeState("Moving");
                    return;
                }
            }
        }
        class Moving : State<SquareBoard, SquareBoardFSMVals>
        {
            public Moving(SquareBoard origin, Layer<SquareBoard, SquareBoardFSMVals> parent) : base(origin, parent)
            {

            }
            List<SquareTile> moving = new();
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                for(int i = 0; i < origin.gridSize.x; i++)
                {
                    int added = 0;
                    for(int k = origin.gridSize.y - 1; k >= 0; k--)
                    {
                        if (origin.tiles[i, k] == null)
                        {
                            bool found = false;
                            for(int l = k-1; l >= 0; l--)
                            {
                                if (origin.tiles[i, l] != null)
                                {
                                    origin.tiles[i, k] = origin.tiles[i, l];
                                    origin.tiles[i, l] = null;

                                    origin.tiles[i, k].gridPos = new Vector2Int(i, k);
                                    Move(origin.tiles[i, k], new Vector2Int(i, k));
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                SquareTile tmp = origin.CreateRandomTile();
                                tmp.transform.position = origin.GetPos(i, -(++added));
                                origin.tiles[i, k] = tmp;

                                origin.tiles[i, k].gridPos = new Vector2Int(i, k);
                                Move(origin.tiles[i, k], new Vector2Int(i, k));
                            }
                        }
                    }
                }
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if(moving.Count == 0)
                {
                    List<SquareTile> targets = origin.ScanPopTargets();
                    if(targets.Count == 0)
                    {
                        parentLayer.ChangeState("PlayerInteract");
                        return;
                    }
                    else
                    {
                        values.popTargets = targets;
                        parentLayer.ChangeState("Popping");
                        return;
                    }
                }
            }
            void Move(SquareTile tile, Vector2Int pos)
            {
                moving.Add(tile);
                tile.MoveTo(origin.GetPos(pos), (SquareTile tmp) => moving.Remove(tile));
            }
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(SquareBoard))]
public class SquareBoard_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Scan"))
        {
            Debug.Log((target as SquareBoard).ScanPopTargets().Count);
        }
    }
}
#endif