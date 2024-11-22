using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class HexBoard : Board
{
    public bool canInteract = false;

    [Header("Grid Size")]
    [SerializeField] Vector2Int gridSize;

    [Header("Grid Element Layout")]
    [SerializeField] float hexSize = 100;
    [SerializeField] float spacing = 10;

    [Header("Tiles")]
    [SerializeField] HexTile[] normalTiles;
    [SerializeField] HexTile[] specialTiles;

    [Header("Tile Generation")]
    [SerializeField] float specialChance = 5.0f;

    [Header("Hint")]
    [SerializeField] float hintStartTime = 10.0f;

    [Header("Debug")]
    [SerializeField] string FSMPath = "";

    public HexTile[,] tiles;

    TopLayer topLayer;
    private void OnValidate()
    {
        //rectTransform.sizeDelta = new Vector2((hexSize * 2 + spacing) * Mathf.Cos(30 * Mathf.Deg2Rad) * gridSize.x + 60, (hexSize + spacing * 0.5f) * gridSize.y + 100);
    }
    public Vector2 GetPos(int x, int y)
    {
        float distX = (hexSize * 2 + spacing) * Mathf.Cos(30 * Mathf.Deg2Rad);
        float posX;
        if (gridSize.x % 2 == 0)
        {
            posX = (x - gridSize.x / 2 + 0.5f) * distX;
        }
        else
        {
            posX = (x - gridSize.x / 2) * distX;
        }
        float distY = (hexSize + spacing * 0.5f);
        float posY;
        if (gridSize.y % 2 == 0)
        {
            posY = (y - gridSize.y / 2 + 0.5f) * -distY;
        }
        else
        {
            posY = (y - gridSize.y / 2) * -distY;
        }
        return transform.TransformPoint(new Vector2(posX, posY));
    }
    public Vector2 GetPos(Vector2Int pos) => GetPos(pos.x, pos.y);
    public bool OutOfBound(int x, int y)
    {
        if (x < 0 || x >= gridSize.x || y < 0 || y >= gridSize.y) return true;
        else return false;
    }
    public bool OutOfBound(Vector2Int pos) => OutOfBound(pos.x, pos.y);
    int specialQueue = 0;
    public HexTile CreateRandomTile()
    {
        if(specialQueue > 0)
        {
            specialQueue--;
            return CreateRandomSpecialTile();
        }
        else if (UnityEngine.Random.Range(0, 100.0f) <= specialChance)
        {
            return CreateRandomSpecialTile();
        }
        else
        {
            HexTile tmp = normalTiles[UnityEngine.Random.Range(0, normalTiles.Length)].Instantiate(this);
            tmp.transform.localScale = new Vector3(1, 1, 1);
            return tmp;
        }
    }
    public HexTile CreateRandomSpecialTile()
    {
        HexTile tmp = specialTiles[UnityEngine.Random.Range(0, specialTiles.Length)].Instantiate(this);
        tmp.transform.localScale = new Vector3(1, 1, 1);
        return tmp;
    }
    int[] gridEnds;
    void GenerateGrid()
    {
        tiles = new HexTile[gridSize.x, gridSize.y];
        gridEnds = new int[gridSize.x];
        for (int i = 0; i < gridSize.x; i++)
        {
            int k;
            for (k = i % 2; k < gridSize.y; k += 2)
            {
                tiles[i, k] = CreateRandomTile();
                tiles[i, k].gridPos = new Vector2Int(i, k);
                tiles[i, k].transform.position = GetPos(i, k);
            }
            gridEnds[i] = k - 2;
        }
    }
    void ResetGrid()
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            int k;
            for (k = i % 2; k < gridSize.y; k += 2)
            {
                tiles[i, k].Release();
                tiles[i, k] = CreateRandomTile();
                tiles[i, k].gridPos = new Vector2Int(i, k);
                tiles[i, k].transform.position = GetPos(i, k);
            }
            gridEnds[i] = k - 2;
        }
    }
    private void Awake()
    {
        GenerateGrid();
        topLayer = new TopLayer(this, new HexBoardFSMVals());
#if UNITY_EDITOR
        topLayer.onFSMChange += () => FSMPath = topLayer.GetFSMPath();
#endif
        topLayer.OnStateEnter();
    }
    private void Update()
    {
        topLayer.OnStateUpdate();
    }
    List<HexTile> popping = new();
    public void Pop(HexTile tile)
    {
        if (tile == null || popping.Contains(tile)) return;
        popping.Add(tile);
        tile.Pop((HexTile tmp) => popping.Remove(tmp));
        combo++;
        onTilePop?.Invoke();
    }
    public void PopAt(Vector2Int pos)
    {
        if (OutOfBound(pos)) return;
        Pop(tiles[pos.x, pos.y]);
    }
    public class HexTileLine
    {
        public List<HexTile> tiles = new();
        public Vector2Int direction;
        public TileType lineType;
    }
    public List<HexTileLine> ScanPopTargets()
    {
        List<HexTileLine> results = new();
        for(int i = 0; i < gridSize.x; i++)
        {
            ScanDownLeft(new Vector2Int(i, i % 2), results);
            ScanDown(new Vector2Int(i, i % 2), results);
            ScanDownRight(new Vector2Int(i, i % 2), results);
        }
        for(int i = 0; i < gridSize.y; i += 2)
        {
            ScanDownRight(new Vector2Int(0, i), results);
        }
        for(int i = ((gridSize.x - 1) % 2 == 0 ? 0 : 1); i < gridSize.y; i += 2)
        {
            ScanDownLeft(new Vector2Int(gridSize.x - 1, i), results);
        }
        return results;
    }
    void ScanDownLeft(Vector2Int startPos, List<HexTileLine> results, int requiredLength = requiredLineLength)
    {
        int last = 0;
        int k;
        for (k = 1; !OutOfBound(startPos.x - k, startPos.y + k); k++)
        {
            if (tiles[startPos.x - last, startPos.y + last].type != tiles[startPos.x - k, startPos.y + k].type)
            {
                if (k - last >= requiredLength)
                {
                    HexTileLine line = new();
                    line.direction = new Vector2Int(-1, 1);
                    for (int l = last; l < k; l++) line.tiles.Add(tiles[startPos.x - l, startPos.y + l]);
                    line.lineType = line.tiles[0].type;
                    results.Add(line);
                }
                last = k;
            }
        }
        if (k - last >= requiredLength)
        {
            HexTileLine line = new();
            line.direction = new Vector2Int(-1, 1);
            for (int l = last; l < k; l++) line.tiles.Add(tiles[startPos.x - l, startPos.y + l]);
            line.lineType = line.tiles[0].type;
            results.Add(line);
        }
    }
    void ScanDown(Vector2Int startPos, List<HexTileLine> results, int requiredLength = requiredLineLength)
    {
        int last = 0;
        int k;
        for(k = 1; !OutOfBound(startPos.x, startPos.y + k * 2); k++)
        {
            if (tiles[startPos.x, startPos.y + last * 2].type != tiles[startPos.x, startPos.y + k * 2].type)
            {
                if (k - last >= requiredLength)
                {
                    HexTileLine line = new();
                    line.direction = new Vector2Int(0, 2);
                    for (int l = last; l < k; l++) line.tiles.Add(tiles[startPos.x, startPos.y + l * 2]);
                    line.lineType = line.tiles[0].type;
                    results.Add(line);
                }
                last = k;
            }
        }
        if (k - last >= requiredLength)
        {
            HexTileLine line = new();
            line.direction = new Vector2Int(0, 2);
            for (int l = last; l < k; l++) line.tiles.Add(tiles[startPos.x, startPos.y + l * 2]);
            line.lineType = line.tiles[0].type;
            results.Add(line);
        }
    }
    void ScanDownRight(Vector2Int startPos, List<HexTileLine> results, int requiredLength = requiredLineLength)
    {
        int last = 0;
        int k;
        for(k = 1; !OutOfBound(startPos.x + k, startPos.y + k); k++)
        {
            if (tiles[startPos.x + last, startPos.y + last].type != tiles[startPos.x + k, startPos.y + k].type)
            {
                if (k - last >= requiredLength)
                {
                    HexTileLine line = new();
                    line.direction = new Vector2Int(1, 1);
                    for (int l = last; l < k; l++) line.tiles.Add(tiles[startPos.x + l, startPos.y + l]);
                    line.lineType = line.tiles[0].type;
                    results.Add(line);
                }
                last = k;
            }
        }
        if (k - last >= requiredLength)
        {
            HexTileLine line = new();
            line.direction = new Vector2Int(1, 1);
            for (int l = last; l < k; l++) line.tiles.Add(tiles[startPos.x + l, startPos.y + l]);
            line.lineType = line.tiles[0].type;
            results.Add(line);
        }
    }
    readonly int[] dirX = new int[] { 1, 0, -1, -1, 0, 1 };
    readonly int[] dirY = new int[] { -1, -2, -1, 1, 2, 1 };
    public bool ScanPossibilities(out (HexTile, HexTile) swap)
    {
        List<HexTileLine> results = new();
        for (int i = 0; i < gridSize.x; i++)
        {
            ScanDownLeft(new Vector2Int(i, i % 2), results, requiredLineLength - 1);
            ScanDown(new Vector2Int(i, i % 2), results, requiredLineLength - 1);
            ScanDownRight(new Vector2Int(i, i % 2), results, requiredLineLength - 1);
        }
        for (int i = 0; i < gridSize.y; i += 2)
        {
            ScanDownRight(new Vector2Int(0, i), results, requiredLineLength - 1);
        }
        for (int i = ((gridSize.x - 1) % 2 == 0 ? 0 : 1); i < gridSize.y; i += 2)
        {
            ScanDownLeft(new Vector2Int(gridSize.x - 1, i), results, requiredLineLength - 1);
        }
        for(int i = 0; i < results.Count; i++)
        {
            Vector2Int pos = results[i].tiles[0].gridPos - results[i].direction;
            if (!OutOfBound(pos))
            {
                for (int k = 0; k < 6; k++)
                {
                    Vector2Int tmp = pos + new Vector2Int(dirX[k], dirY[k]);
                    if (tmp == results[i].tiles[0].gridPos || OutOfBound(tmp)) continue;
                    if (tiles[tmp.x, tmp.y].type == results[i].lineType)
                    {
                        Debug.Log(pos);
                        Debug.Log(tmp);
                        swap = (tiles[pos.x, pos.y], tiles[tmp.x, tmp.y]);
                        return true;
                    }
                }
            }
            pos = results[i].tiles[results[i].tiles.Count - 1].gridPos + results[i].direction;
            if (!OutOfBound(pos))
            {
                for (int k = 0; k < 6; k++)
                {
                    Vector2Int tmp = pos + new Vector2Int(dirX[k], dirY[k]);
                    if (tmp == results[i].tiles[results[i].tiles.Count - 1].gridPos || OutOfBound(tmp)) continue;
                    if (tiles[tmp.x, tmp.y].type == results[i].lineType)
                    {
                        Debug.Log(pos);
                        Debug.Log(tmp);
                        swap = (tiles[pos.x, pos.y], tiles[tmp.x, tmp.y]);
                        return true;
                    }
                }
            }
        }
        swap = (null, null);
        return false;
    }
    class HexBoardFSMVals : FSMVals
    {
        public Vector2 grabTouchBeganPos;
        public HexTile grabbingTile = null;
        public Vector2Int grabTileMoveDir;
        public (HexTile, HexTile) possibleSwap;

        public List<HexTileLine> popTargets;
    }
    class TopLayer : TopLayer<HexBoard, HexBoardFSMVals>
    {
        public TopLayer(HexBoard origin, HexBoardFSMVals values) : base(origin, values)
        {
            defaultState = new PlayerInteract(origin, this);
            AddState("PlayerInteract", defaultState);
            AddState("Popping", new Popping(origin, this));
            AddState("Moving", new Moving(origin, this));
        }
        class PlayerInteract : Layer<HexBoard, HexBoardFSMVals>
        {
            public PlayerInteract(HexBoard origin, Layer<HexBoard, HexBoardFSMVals> parent) : base(origin, parent)
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
                while (!origin.ScanPossibilities(out values.possibleSwap)) origin.ResetGrid();
            }
            class Waiting : State<HexBoard, HexBoardFSMVals>
            {
                public Waiting(HexBoard origin, Layer<HexBoard, HexBoardFSMVals> parent) : base(origin, parent)
                {

                }
                float counter = 0.0f;
                bool hinting = false;
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    counter = 0.0f;
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if (!origin.canInteract) return;
                    if (InputManager.IsTouchDown() && UIScanner.FindWithTag(InputManager.GetTouchPosition(), "Tile", out GameObject tmp))
                    {
                        if (tmp.transform.parent != origin.transform) return;
                        values.grabTouchBeganPos = InputManager.GetTouchPosition();
                        values.grabbingTile = tmp.GetComponent<HexTile>();
                        parentLayer.ChangeState("Grabbing");
                    }
                    else if(!hinting)
                    {
                        counter += Time.deltaTime;
                        if(counter >= origin.hintStartTime)
                        {
                            values.possibleSwap.Item1.Hint();
                            values.possibleSwap.Item2.Hint();
                            hinting = true;
                            Debug.Log("Hint");
                        }
                    }
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    if (hinting)
                    {
                        values.possibleSwap.Item1.EndHint();
                        values.possibleSwap.Item2.EndHint();
                        hinting = false;
                    }
                }
            }
            class Grabbing : State<HexBoard, HexBoardFSMVals>
            {
                public Grabbing(HexBoard origin, Layer<HexBoard, HexBoardFSMVals> parent) : base(origin, parent)
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
                        if (moved.magnitude > 10.0f)
                        {
                            float angle = Mathf.Atan2(moved.y, moved.x) * Mathf.Rad2Deg;
                            if (angle < 0.0f) angle += 360.0f;
                            Vector2Int dir;
                            if (angle > 0.0f && angle <= 60.0f) dir = new Vector2Int(1, -1);
                            else if (angle > 60.0f && angle <= 120.0f) dir = new Vector2Int(0, -2);
                            else if (angle > 120.0f && angle <= 180.0f) dir = new Vector2Int(-1, -1);
                            else if (angle > 180.0f && angle <= 240.0f) dir = new Vector2Int(-1, 1);
                            else if (angle > 240.0f && angle <= 300.0f) dir = new Vector2Int(0, 2);
                            else dir = new Vector2Int(1, 1); 
                            values.grabTileMoveDir = dir;
                            parentLayer.ChangeState("MoveAndCheck");
                            return;
                        }
                    }
                }
            }
            class MoveAndCheck : State<HexBoard, HexBoardFSMVals>
            {
                public MoveAndCheck(HexBoard origin, Layer<HexBoard, HexBoardFSMVals> parent) : base(origin, parent)
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
                    HexTile originTile = values.grabbingTile;
                    HexTile movingTile = origin.tiles[targetPos.x, targetPos.y];
                    bool originMoving = true, movingMoving = true;
                    originTile.MoveTo(origin.GetPos(movingTile.gridPos), (HexTile tmp) => originMoving = false);
                    movingTile.MoveTo(origin.GetPos(originTile.gridPos), (HexTile tmp) => movingMoving = false);
                    while (originMoving || movingMoving) yield return null;
                    origin.tiles[targetPos.x, targetPos.y] = originTile;
                    origin.tiles[originPos.x, originPos.y] = movingTile;
                    originTile.gridPos = targetPos;
                    movingTile.gridPos = originPos;
                    List<HexTileLine> targets = origin.ScanPopTargets();
                    if (targets.Count == 0)
                    {
                        yield return new WaitForSeconds(0.5f);
                        originMoving = true; movingMoving = true;
                        originTile.MoveTo(origin.GetPos(originPos), (HexTile tmp) => originMoving = false);
                        movingTile.MoveTo(origin.GetPos(targetPos), (HexTile tmp) => movingMoving = false);
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
        class Popping : State<HexBoard, HexBoardFSMVals>
        {
            public Popping(HexBoard origin, Layer<HexBoard, HexBoardFSMVals> parent) : base(origin, parent)
            {

            }
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                foreach (var i in values.popTargets)
                {
                    if (i.tiles.Count >= specialSpawnLineLength) origin.specialQueue++;
                    foreach (var k in i.tiles) origin.Pop(k);
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
        class Moving : State<HexBoard, HexBoardFSMVals>
        {
            public Moving(HexBoard origin, Layer<HexBoard, HexBoardFSMVals> parent) : base(origin, parent)
            {

            }
            List<HexTile> moving = new();
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                for (int i = 0; i < origin.gridSize.x; i++)
                {
                    int added = 0;
                    for (int k = origin.gridEnds[i]; k >= 0; k -= 2)
                    {
                        if (origin.tiles[i, k] == null)
                        {
                            bool found = false;
                            for(int l = k - 2; l >= 0; l -= 2)
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
                                origin.tiles[i, k] = origin.CreateRandomTile();
                                origin.tiles[i, k].transform.position = origin.GetPos(i, i % 2 -(++added * 2));

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
                if (moving.Count == 0)
                {
                    List<HexTileLine> targets = origin.ScanPopTargets();
                    if (targets.Count == 0)
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
            void Move(HexTile tile, Vector2Int pos)
            {
                moving.Add(tile);
                tile.MoveTo(origin.GetPos(pos), (HexTile tmp) => moving.Remove(tile));
            }
        }
    }
}
/*[System.Serializable]
public struct HexTileRelation
{
    public HexTile upRight, up, upLeft, downRight, down, downLeft;
}
#if UNITY_EDITOR
[CustomEditor(typeof(HexTile))]
public class HexTile_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
    }
}
[CustomPropertyDrawer(typeof(HexTileRelation))]
public class HexTileRelationDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position.x + 30, position.y + 30, 100, 18), property.FindPropertyRelative("upLeft"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(position.x + 100, position.y + 10, 100, 18), property.FindPropertyRelative("up"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(position.x + 170, position.y + 30, 100, 18), property.FindPropertyRelative("upRight"), GUIContent.none);

        EditorGUI.PropertyField(new Rect(position.x + 30, position.y + 60, 100, 18), property.FindPropertyRelative("downLeft"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(position.x + 100, position.y + 80, 100, 18), property.FindPropertyRelative("down"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(position.x + 170, position.y + 60, 100, 18), property.FindPropertyRelative("downRight"), GUIContent.none);

        EditorGUI.LabelField(new Rect(position.x + 125, position.y + 30, 50, 50), new GUIContent() { image = Resources.Load<Texture>("Editor/Hexagon") });
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 100;
    }
}
#endif
*/