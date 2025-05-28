using System;
using System.Collections;
using System.Threading.Tasks;
using ui.windows;
using UnityEngine;

public enum WindowStyle
{
    Classic = 0,
    Blue,
    Night,
    Wood
}

public enum TypePanel
{
    Default = 0,
    Bot,
    Top,
    ThemeSettings,
    Settings,
    Additional,
    Stats,
    Finish,
    DefaultDialog,
    DialogWith2ColumnButtons,
    DialogWith2buttonsRow,
    DialogWith3Buttons,
    FinishCollection,
    Classic,
    Share,
    Premium,
    PremiumActivated,
    CollectModels,
    Collection,
    Relax,
    Journey,
}

public enum CardsStyle
{
    European = 0,
    American,
    NoFaces,
}

public enum CardsBackStyle
{
    Blue = 0,
    Classic,
    Night,
    Wood
}

public enum GameBackground
{
    Green = 0,
    Purple,
    Blue,
    Wooden
}

public class ThemeManager : MonoBehaviour
{
    public static event Action<int> InitializationCompleted;

    private static ThemeManager instance;

    public delegate void ThemeEvent();

    public static event ThemeEvent ThemeLoaded;
    public static event ThemeEvent CardsLoaded;
    public static event ThemeEvent BackgroundLoaded;

    public const string CUSTOM_BACK_TEMPL = "customBack{0}.png";

    [HideInInspector] public Sprite[] cardSprites;
    [HideInInspector] public Sprite[] simpleBackSprites;
    [HideInInspector] public Sprite[] customBackSprites;
    [HideInInspector] public Sprite slotSprite;
    [HideInInspector] public Sprite glowSprite;
    [HideInInspector] public Sprite hintSprite;
    [HideInInspector] public Sprite recycleSprite;
    [HideInInspector] public Sprite stopSprite;
    [HideInInspector] public Sprite[] cardSymbols;
    [HideInInspector] public Sprite[] suitSymbols;
    [HideInInspector] public Sprite backgroundSpriteV;
    [HideInInspector] public Texture2D cardBorder;
    [HideInInspector] public Sprite backgroundSpriteH;

    public static Sprite BackSprite
    {
        get
        {
            // take simple back sprite if index in array range
            if (GameSettings.Instance.Data.CardBack < instance.simpleBackSprites.Length)
                return instance.simpleBackSprites[GameSettings.Instance.Data.CardBack];
            else
            {
                // if index is bigger than simple back count try to take custom card back

                //calculate custom back index
                int customIndex = GameSettings.Instance.Data.CardBack - instance.simpleBackSprites.Length;

                //if there is now custom back with given index just apply default settings
                if (instance.customBackSprites[customIndex] == null)
                {
                    GameSettings.Instance.TryToSetCardBack(0, (b) => { });
                    return BackSprite;
                }
                else
                    return instance.customBackSprites[customIndex];
            }
        }
    }

    public static Sprite BGSpriteVert
    {
        get { return instance.backgroundSpriteV; }
    }

    public static Sprite BGSpriteHor
    {
        get { return instance.backgroundSpriteH; }
    }

    public static Sprite SlotSprite
    {
        get { return instance.slotSprite; }
    }

    public static Sprite GlowSprite
    {
        get { return instance.glowSprite; }
    }

    public static Sprite HintSprite
    {
        get { return instance.hintSprite; }
    }

    public static Sprite RecycleSprite
    {
        get { return instance.recycleSprite; }
    }

    public static Sprite StopSprite
    {
        get { return instance.stopSprite; }
    }

    public static Vector2 CardSize
    {
        get { return CardSprite(0).bounds.size; }
    }

    public static Sprite SymbolSprite(int index)
    {
        if (GameSettings.Instance.Data.CardStyle != (int)CardsStyle.NoFaces)
        {
            return index < 4 ? instance.suitSymbols[index] : instance.cardSymbols[index - 4];
        }
        else
            return instance.suitSymbols[index % 4];
    }

    public static Sprite CardSprite(int index)
    {
        return instance.cardSprites[index];
    }

    public static Sprite GetSimpleSprite(int index)
    {
        return instance.simpleBackSprites[index];
    }

    public static Sprite GetCustomSprite(int index)
    {
        return instance.customBackSprites[index];
    }

    public static Texture2D CardBorder
    {
        get { return instance.cardBorder; }
    }

    public static void SetCustomSprite(int index, Sprite sprite)
    {
        if (instance.customBackSprites[index] != null)
            Destroy(instance.customBackSprites[index].texture);

        instance.customBackSprites[index] = sprite;
    }

    //calculate average texture color
    public static Color TextureAvgColor(Texture2D tex)
    {
        Color[] pixels = tex.GetPixels();
        Color res = Color.white;

        //take only one pixel out of N to make it faster
        int step = 10;

        for (int i = 0; i < pixels.Length; i += step)
        {
            if (i == 0)
            {
                res = pixels[i];
            }
            else
            {
                res = Color.Lerp(res, pixels[i], 0.5f);
            }
        }

        return res;
    }

    public ThemeManager() : base()
    {
        instance = this;

        customBackSprites = new Sprite[4];
    }

    public static async Task Init()
    {
        instance.LoadCards(GameSettings.Instance.Data.CardStyle);
        instance.LoadTheme("default");
        instance.LoadBackground(GameSettings.Instance.Data.Background);
        await instance.LoadSimpleCardBacks();
        instance.LoadCustomCardBacks();
        InitializationCompleted?.Invoke(GameSettings.Instance.Data.Background);
    }

    void Start()
    {
        GameSettings.Instance.BackgroundSelected += OnBackgroundSelected;
        GameSettings.Instance.CardStyleSelected += OnCardStyleChanged;
    }

    private void OnCardStyleChanged()
    {
        LoadCards(GameSettings.Instance.Data.CardStyle);
    }

    private void OnBackgroundSelected()
    {
        LoadBackground(GameSettings.Instance.Data.Background);
    }

    private async Task LoadSimpleCardBacks()
    {
        string[] backStyles = System.Enum.GetNames(typeof(CardsBackStyle));
        simpleBackSprites = new Sprite[backStyles.Length];

        //load card sprites for each style
        for (int i = 0; i < backStyles.Length; i++)
        {
            simpleBackSprites[i] = Resources.Load<Sprite>("back/" + backStyles[i]);
        }
    }

    private void LoadCustomCardBacks()
    {
        // try to load 4 custom card textures
        for (int i = 0; i < 4; i++)
        {
            //form sprite file name
            string fileName =
                System.IO.Path.Combine(Application.persistentDataPath, string.Format(CUSTOM_BACK_TEMPL, i));

            //if no such file do nothing
            if (!System.IO.File.Exists(fileName))
                return;

            //read sprite bytes
            byte[] bytes = System.IO.File.ReadAllBytes(fileName);

            //if no bytes were read do nothing
            if (bytes == null || bytes.Length < 1)
                return;

            //load bytes in texture
            Texture2D tex = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            tex.LoadImage(bytes);

            //create sprite from texture
            customBackSprites[i] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2f, 180, 0,
                SpriteMeshType.FullRect);
        }
    }

    private void LoadBackground(int bgIndex)
    {
        Sprite prev1 = backgroundSpriteV;
        Sprite prev2 = backgroundSpriteH;

        backgroundSpriteV = Resources.Load<Sprite>("background/bg" + bgIndex + "Vert");
        backgroundSpriteH = Resources.Load<Sprite>("background/bg" + bgIndex + "Hor");

        //notify listeners that new sprites are loaded
        if (BackgroundLoaded != null)
            BackgroundLoaded();

        //unload previous images only after we load and apply new ones
        Resources.UnloadAsset(prev1);
        Resources.UnloadAsset(prev2);
    }

    private void LoadCards(int styleIndex)
    {
        //store references to old sprites
        Sprite[] prevSprites = cardSprites;

        //load cards according to given style index
        cardSprites = Resources.LoadAll<Sprite>("cards/" + ((CardsStyle)styleIndex).ToString());


        //load card symbols of given style
        if (GameSettings.Instance.Data.CardStyle != (int)CardsStyle.NoFaces)
            cardSymbols =
                Resources.LoadAll<Sprite>("symbols/" + ((CardsStyle)GameSettings.Instance.Data.CardStyle).ToString());

        //notify listeners that new sprites are loaded
        if (CardsLoaded != null)
            CardsLoaded();

        //unload prev sprites from memory
        foreach (var s in prevSprites)
            Resources.UnloadAsset(s);
    }

    private void LoadTheme(string themeName)
    {
        slotSprite = Resources.Load<Sprite>(themeName + "/slot");
        glowSprite = Resources.Load<Sprite>(themeName + "/glow");
        hintSprite = Resources.Load<Sprite>(themeName + "/glowTip");
        recycleSprite = Resources.Load<Sprite>(themeName + "/recycle");
        stopSprite = Resources.Load<Sprite>(themeName + "/stop");
        cardBorder = Resources.Load<Texture2D>(themeName + "/cardBorder");
        suitSymbols = Resources.LoadAll<Sprite>("symbols/suits");
    }
}