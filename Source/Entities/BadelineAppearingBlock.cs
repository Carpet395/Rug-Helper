using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Celeste.Mod.Entities;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using static Celeste.TrackSpinner;

namespace Celeste.Mod.Rug.Entities;

//[Tracked(false)]
[CustomEntity("Rug/BadAppearBlock")]
public class AppearingBlock : Solid
{

    // -- idk how tf does it work but i know it does so yeppe!! -- //

    // tile sprites
    public TileGrid sprite;
    //public TileGrid highlight;

    // nodex
    public Vector2[] nodes;

    // sine wave
    //public SineWave sine;

    //public float freq;
    //public float peak;

    public bool Appear = false;

    // tile
    public char tileType;
    //public char HightileType;

    // flag logic
    public string Flag;
    public bool OnFlag;

    public bool onNode;
    public int count;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public AppearingBlock(Vector2[] nodes, float width, float height, char tileType, string flag, bool state, bool onNode = false, int count = 0)
        : base(nodes[0], width, height, safe: false)
    {
        this.onNode = onNode;
        this.count = count;
        //BossNodeIndex = bossNodeIndex;
        //this.freq = freq;
        //peak = 1f;
        Flag = flag;
        OnFlag = state;
        this.nodes = nodes;
        //int newSeed = Calc.Random.Next();
        //Calc.PushRandom(newSeed);
        this.tileType = tileType;
        //Calc.PopRandom();
        //Calc.PushRandom(newSeed);
        //highlight = GFX.FGAutotiler.GenerateBox(highlightTileType, (int)(base.Width / 8f), (int)base.Height / 8).TileGrid;
        //highlight.Alpha = 0f;
        //Add(highlight);
        //Calc.PopRandom();
        Add(new LightOcclude());
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        //level.SolidTiles.Add(sprite);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public AppearingBlock(EntityData data, Vector2 offset)
    : this(data.NodesWithPosition(offset), data.Width, data.Height, data.Char("tiletype"), data.String("flag"), data.Bool("state"), data.Bool("onNode", false), data.Int("count", 0))
    {
    }

    public bool GetNodeMatch(int toMatch)
    {
        foreach (var i in Engine.Scene.Entities.FindAll<FinalBoss>())
        {
            var field = typeof(FinalBoss).GetField("nodeIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            int index = (int)field.GetValue(i);
            if (index == toMatch) return true;
        }
        return false;
    }

    public bool fade = false;
    public float TargetAlpha = 0;

    public override void Update()
    {
        bool hm = GetNodeMatch(count);
        if (sprite == null && ((BlockedCheck<FinalBoss>(true) && !onNode) || (onNode && hm)))
            Add(new Coroutine(waitForClear(onNode && hm)));
        if (sprite != null)
        {

            fade = sprite.Alpha != TargetAlpha;
            //Logger.Log(LogLevel.Info, "neiw", fade.ToString());
            if (fade)
            {

                //sprite.Alpha = a;
                //while (current != to)
                //{
                //CassetteBlock
                sprite.Alpha = Calc.Approach(sprite.Alpha, TargetAlpha, Engine.DeltaTime);
                StartShaking(Engine.DeltaTime);
                if (sprite.Alpha == TargetAlpha)
                {
                    fade = false;
                    sprite.Position = Vector2.Zero;
                }
                //sprite.Alpha = current;
                //yield return null;
                //}
                //sprite.Alpha = to;
            }
            else
            {
                sprite.Position = Vector2.Zero;
            }
        }
        base.Update();
    }

    public override void Render()
    {
        if (sprite != null && !fade) sprite.Position = Vector2.Zero;
        base.Render();
    }

    public void RebuildOverlay()
    {
        if (!Collidable || sprite == null)
            return;

        Level level = Scene as Level;
        Rectangle tileBounds = level.Session.MapData.TileBounds;
        VirtualMap<char> solidsData = level.SolidsData;
        VirtualMap<char> map = new VirtualMap<char>(solidsData.Columns, solidsData.Rows, '0');

        for (int tx = 0; tx < solidsData.Columns; tx++)
            for (int ty = 0; ty < solidsData.Rows; ty++)
                map[tx, ty] = solidsData[tx, ty];

        foreach (AppearingBlock block in Scene.Entities.FindAll<AppearingBlock>())
        {
            if (block.tileType == tileType && block.Collidable)
            {
                int bx = (int)block.X / 8 - tileBounds.Left;
                int by = (int)block.Y / 8 - tileBounds.Top;
                int bw = (int)block.Width / 8;
                int bh = (int)block.Height / 8;
                for (int xx = 0; xx < bw; xx++)
                    for (int yy = 0; yy < bh; yy++)
                        map[bx + xx, by + yy] = tileType;
            }
        }

        int x = (int)X / 8 - tileBounds.Left;
        int y = (int)Y / 8 - tileBounds.Top;

        // Regenerate autotiler overlay
        TileGrid newTile = GFX.FGAutotiler.GenerateOverlay(tileType, x, y, (int)(Width / 8), (int)(Height / 8), map).TileGrid;
        sprite.Tiles = newTile.Tiles;
    }

    public IEnumerator waitForClear(bool shakeExtra)
    {
        int tilesX = (int)(Width / 8);
        int tilesY = (int)(Height / 8);

        Level level = Scene as Level;
        Rectangle tileBounds = level.Session.MapData.TileBounds;
        VirtualMap<char> solidsData = level.SolidsData;
        VirtualMap<char> map = new VirtualMap<char>(solidsData.Columns, solidsData.Rows, '0');
        for (int tx = 0; tx < solidsData.Columns; tx++)
            for (int ty = 0; ty < solidsData.Rows; ty++)
                map[tx, ty] = solidsData[tx, ty];

        // we are now generating the sprite for the first time
        
        // this isnt needed 
        /*foreach (AppearingBlock block in Scene.Entities.FindAll<AppearingBlock>())
        {
            if (block.tileType == tileType)
            {
                if (block.Appear)
                {
                    int bx = (int)block.X / 8 - tileBounds.Left;
                    int by = (int)block.Y / 8 - tileBounds.Top;
                    int bw = (int)block.Width / 8;
                    int bh = (int)block.Height / 8;
                    for (int xx = 0; xx < bw; xx++)
                        for (int yy = 0; yy < bh; yy++)
                            map[bx + xx, by + yy] = tileType;
                }
            }
        }*/
        int x = (int)X / 8 - tileBounds.Left;
        int y = (int)Y / 8 - tileBounds.Top;
        sprite = GFX.FGAutotiler.GenerateOverlay(tileType, x, y, tilesX, tilesY, map).TileGrid;
        sprite.Position = Vector2.Zero;
        sprite.Alpha = 0;
        Add(sprite);
        Add(new TileInterceptor(sprite, highPriority: true));
        sprite.Visible = true;
        sprite.Active = true;
        TargetAlpha = 0.25f;
        if (count == 0) sprite.Alpha = 0.25f;
        //fade = true;
        //Add(new Coroutine(ChangeAlpha(0f, 0.25f, 0.25f)));
        //sprite.Color = new Color(1, 1, 1, 0.25f);
        // now it should all connect
        yield return null;
        RebuildOverlay();
        while (BlockedCheck(true)) //|| BlockedCheck<FinalBoss>(true))
        {
            StartShaking(0.05f);
            yield return 0.05f;
        }
        //SceneAs<Level>().Shake(0.25f);
        //StartShaking(shakeExtra? 1f : 0.5f);
        if (Flag != "")
        {
            SceneAs<Level>().Session.SetFlag(Flag, OnFlag);
        }
        //sprite.Color = new Color(1, 1, 1, 1f);
        //Add(new Coroutine(ChangeAlpha(0.25f, 1, 0.75f)));
        TargetAlpha = 1f;
        if (count == 0) sprite.Alpha = 1f;
        //fade = true;
        Collidable = true;
        foreach (AppearingBlock other in Scene.Entities.FindAll<AppearingBlock>())
        {
            if (other == this || other.sprite == null)
                continue;

            // this can be used to make the tiles node sensitive
            //bool connect =
            //(!onNode && !other.onNode) ||                   // both are global
            //(!onNode && other.onNode) ||                    // this is global
            //(onNode && !other.onNode) ||                    // other is global
            //(onNode && other.onNode && other.count == count); // both onNode and same count
            
            bool connect = true;

            // this calls all the tiles that are neighbours (should at least)
            if (connect &&
                other.Collidable &&
                other.CollideRect(new Rectangle(
                    (int)X - 8, (int)Y - 8,
                    (int)Width + 16, (int)Height + 16)))
            {
                other.RebuildOverlay();
            }
        }
        // to make sure it all connects
        RebuildOverlay();
        if (count == 0) sprite.Alpha = 1f;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void OnShake(Vector2 amount)
    {
        base.OnShake(amount);
        if (fade) sprite.Position = amount;
    }

    public bool BlockedCheck(bool doShit = false)
    {
        TheoCrystal theoCrystal = CollideFirst<TheoCrystal>();
        if (doShit ? theoCrystal != null && TryActorWiggleUp(theoCrystal) : theoCrystal != null)
        {
            return true;
        }

        Player player = CollideFirst<Player>();
        if (doShit ? player != null && TryActorWiggleUp(player) : player != null)
        {
            return true;
        }

        return false;
    }

    public bool BlockedCheck<T>(bool doShit = false) where T : Entity
    {
        bool cl = Collidable;
        List<T> disabled = new List<T>();
        foreach (var i in Engine.Scene.Entities.FindAll<T>())
        {
            if (i.Collidable == false) disabled.Add(i);
        }
        foreach (var item in disabled)
        {
            item.Collidable = true;
        }
        Collidable = true;
        T Tcollider = CollideFirst<T>();
        if (Tcollider != null)
        {
            foreach (var item in disabled)
            {
                item.Collidable = false;
            }
            Collidable = cl;
            return true;
        }
        foreach (var item in disabled)
        {
            item.Collidable = false;
        }
        Collidable = cl;
        return false;
    }

    public bool TryActorWiggleUp(Entity actor)
    {
        bool collidable = Collidable;
        Collidable = true;
        for (int i = 1; i <= 4; i++)
        {
            if (!actor.CollideCheck<Solid>(actor.Position - Vector2.UnitY * i))
            {
                actor.Position -= Vector2.UnitY * i;
                Collidable = collidable;
                return true;
            }
        }

        Collidable = collidable;
        return false;
    }

    public override void Awake(Scene scene)
    {
        //sine = new SineWave(freq, 1);
        Collidable = false;
        if (sprite != null)
        {
            sprite.Visible = false;
            sprite.Active = false;
        }
        base.Awake(scene);
    }
}