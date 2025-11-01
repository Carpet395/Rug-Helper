using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Rug.Module;

public class BadelineSpriteModule : Sprite
{
    //SpriteBank sprite;
    public const string Idle = "idle";

    public const string Shaking = "shaking";

    public const string FrontEdge = "edge";

    public const string LookUp = "lookUp";

    public const string Walk = "walk";

    public const string RunSlow = "runSlow";

    public const string RunFast = "runFast";

    public const string RunWind = "runWind";

    public const string RunStumble = "runStumble";

    public const string JumpSlow = "jumpSlow";

    public const string FallSlow = "fallSlow";

    public const string Fall = "fall";

    public const string JumpFast = "jumpFast";

    public const string FallFast = "fallFast";

    public const string FallBig = "bigFall";

    public const string LandInPose = "fallPose";

    public const string Tired = "tired";

    public const string TiredStill = "tiredStill";

    public const string WallSlide = "wallslide";

    public const string ClimbUp = "climbUp";

    public const string ClimbDown = "climbDown";

    public const string ClimbLookBackStart = "climbLookBackStart";

    public const string ClimbLookBack = "climbLookBack";

    public const string Dangling = "dangling";

    public const string Duck = "duck";

    public const string Dash = "dash";

    public const string Sleep = "sleep";

    public const string Sleeping = "asleep";

    public const string Flip = "flip";

    public const string Skid = "skid";

    public const string DreamDashIn = "dreamDashIn";

    public const string DreamDashLoop = "dreamDashLoop";

    public const string DreamDashOut = "dreamDashOut";

    public const string SwimIdle = "swimIdle";

    public const string SwimUp = "swimUp";

    public const string SwimDown = "swimDown";

    public const string StartStarFly = "startStarFly";

    public const string StarFly = "starFly";

    public const string StarMorph = "starMorph";

    public const string IdleCarry = "idle_carry";

    public const string RunCarry = "runSlow_carry";

    public const string JumpCarry = "jumpSlow_carry";

    public const string FallCarry = "fallSlow_carry";

    public const string PickUp = "pickup";

    public const string Throw = "throw";

    public const string Launch = "launch";

    public const string TentacleGrab = "tentacle_grab";

    public const string TentacleGrabbed = "tentacle_grabbed";

    public const string TentaclePull = "tentacle_pull";

    public const string TentacleDangling = "tentacle_dangling";

    public const string SitDown = "sitDown";

    public string spriteName;

    public int HairCount = 4;

    public static Dictionary<string, PlayerAnimMetadata> FrameMetadata = new Dictionary<string, PlayerAnimMetadata>(StringComparer.OrdinalIgnoreCase);

    public PlayerSpriteMode Mode
    {
        get;
        set;
    }

    public Vector2 HairOffset
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get
        {
            if (Texture != null && FrameMetadata.TryGetValue(Texture.AtlasPath, out var value))
            {
                return value.HairOffset;
            }

            return Vector2.Zero;
        }
    }

    public float CarryYOffset
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get
        {
            if (Texture != null && FrameMetadata.TryGetValue(Texture.AtlasPath, out var value))
            {
                return (float)value.CarryYOffset * Scale.Y;
            }

            return 0f;
        }
    }

    public int HairFrame
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get
        {
            if (Texture != null && FrameMetadata.TryGetValue(Texture.AtlasPath, out var value))
            {
                return value.Frame;
            }

            return 0;
        }
    }

    public bool HasHair
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get
        {
            if (Texture != null && FrameMetadata.TryGetValue(Texture.AtlasPath, out var value))
            {
                return value.HasHair;
            }

            return false;
        }
    }

    public bool Running
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get
        {
            if (base.LastAnimationID != null)
            {
                if (!(base.LastAnimationID == "flip"))
                {
                    return base.LastAnimationID.StartsWith("run");
                }

                return true;
            }

            return false;
        }
    }

    public bool DreamDashing
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get
        {
            if (base.LastAnimationID != null)
            {
                return base.LastAnimationID.StartsWith("dreamDash");
            }

            return false;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Render()
    {
        Vector2 renderPosition = base.RenderPosition;
        base.RenderPosition = base.RenderPosition.Floor();
        base.Render();
        base.RenderPosition = renderPosition;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CreateFramesMetadata(string sprite)
    {
        foreach (SpriteDataSource source in GFX.SpriteBank.SpriteData[sprite].Sources)
        {
            XmlElement xmlElement = source.XML["Metadata"];
            string text = source.Path;
            if (xmlElement == null)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(source.OverridePath))
            {
                text = source.OverridePath;
            }

            foreach (XmlElement item in xmlElement.GetElementsByTagName("Frames"))
            {
                string text2 = text + item.Attr("path", "");
                string[] array = item.Attr("hair").Split(new char[1] { '|' });
                string[] array2 = item.Attr("carry", "").Split(new char[1] { ',' });
                for (int i = 0; i < Math.Max(array.Length, array2.Length); i++)
                {
                    PlayerAnimMetadata playerAnimMetadata = new PlayerAnimMetadata();
                    string text3 = text2 + ((i < 10) ? "0" : "") + i;
                    if (i == 0 && !GFX.Game.Has(text3))
                    {
                        text3 = text2;
                    }

                    FrameMetadata[text3] = playerAnimMetadata;
                    if (i < array.Length)
                    {
                        if (array[i].Equals("x", StringComparison.OrdinalIgnoreCase) || array[i].Length <= 0)
                        {
                            playerAnimMetadata.HasHair = false;
                        }
                        else
                        {
                            string[] array3 = array[i].Split(new char[1] { ':' });
                            string[] array4 = array3[0].Split(new char[1] { ',' });
                            playerAnimMetadata.HasHair = true;
                            playerAnimMetadata.HairOffset = new Vector2(Convert.ToInt32(array4[0]), Convert.ToInt32(array4[1]));
                            playerAnimMetadata.Frame = ((array3.Length >= 2) ? Convert.ToInt32(array3[1]) : 0);
                        }
                    }

                    if (i < array2.Length && array2[i].Length > 0)
                    {
                        playerAnimMetadata.CarryYOffset = int.Parse(array2[i]);
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ClearFramesMetadata()
    {
        FrameMetadata.Clear();
    }

    public BadelineSpriteModule(string mode, bool use_enum = false)
        : base()
	{
        //Mode = PlayerSpriteMode.Badeline;

        //base.Active = false;
        //base.Visible = false;
        //RemoveSelf();
        //this.Visible = true;
        //this.Active = true;

        if (!use_enum)
		{
			spriteName = mode;
			//GFX.Unload();
			GFX.SpriteBank.CreateOn(this, mode);
		}
		//base.Visible = false;
	}
    /*public override void Update()
    {
        if (this.CurrentAnimationID != null && this.CurrentAnimationID != "")
            base.Play(this.CurrentAnimationID, false, false);
    }*/
}
