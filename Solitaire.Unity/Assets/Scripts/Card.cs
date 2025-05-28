using DG.Tweening;
using System;
using System.Collections;
using System.ComponentModel;
using Scripts.Library.Extenstions;
using ui;
using ui.windows;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class that represents card event arguments.
/// </summary>
public class CardEventArgs
{
    public Card Card { get; private set; }

    public CardEventArgs(Card cardRef)
    {
        Card = cardRef;
    }
}

public enum Suit
{
    [Description("♦")]
    Diamonds,
    [Description("♥")]
    Hearts,
    [Description("♣")]
    Clubs,
    [Description("♠")]
    Spades,
}

public enum CardColor
{
    Red,
    Black
}

public enum ValueName
{
    [Description("2")]
    Two = 0,
    [Description("3")]
    Three,
    [Description("4")]
    Four,
    [Description("5")]
    Five,
    [Description("6")]
    Six,
    [Description("7")]
    Seven,
    [Description("8")]
    Eight,
    [Description("9")]
    Nine,
    [Description("10")]
    Ten,
    [Description("J")]
    Jack,
    [Description("Q")]
    Queen,
    [Description("K")]
    King,
    [Description("A")]
    Ace
}

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // inspector fields, dont access them directly or from outside, please
    public SpriteRenderer sr;

    public SpriteRenderer glow;
    // todo вырезал
    // public tk2dSlicedSprite shadow;

    public delegate void CardPressed(CardEventArgs a);

    public event CardPressed Pressed;
    public event CardPressed Released;

    public bool IsUp { get; private set; }

    public int Index { get; set; }

    private Sprite face;
    private Sprite jacket;
    private Sprite glowSprite;
    private Sprite hintGlow;
    private Tweener t_select;
    private Tweener t_move;
    private Tweener t_glow;
    private Tweener t_hint;
    private Tweener t_rotate;
    private Vector3 originalPos;
    private Vector2 originalShadowSize = Vector2.zero;
    private Vector2 shadowBorder;
    private BoxCollider2D _collider;
    private bool isShaking;
    private int layerCount = 3;
    private bool prepared = false;
    private Coroutine shake;

    /// <summary>
    /// Gets the name of the given card.
    /// </summary>
    /// <returns>The card name.</returns>
    /// <param name="c">Card object.</param>
    public static string GetCardName(Card c)
    {
        if (c == null)
            return string.Empty;

        return $"{c.ValueString()}{c.Suit.SuitString()}";
    }

    /// <summary>
    /// Determines if given cards are of opposite colors (one red, one black)
    /// </summary>
    /// <returns><c>true</c>, if cards of opposite colors, <c>false</c> otherwise.</returns>
    /// <param name="c1">First card .</param>
    /// <param name="c2">Second card.</param>
    public static bool CardsOfOppositeColor(Card c1, Card c2)
    {
        if (c1 == null || c2 == null)
            return false;

        return c1.CardColor != c2.CardColor;
    }

    /// <summary>
    /// Iis this card in any animation
    /// </summary>
    /// <value><c>true</c> if in animation; otherwise, <c>false</c>.</value>
    public bool InAnimation
    {
        get
        {
            return (t_glow != null && t_glow.IsPlaying()) ||
                   (t_select != null && t_select.IsPlaying()) ||
                   (t_move != null && t_move.IsPlaying()) ||
                   (t_rotate != null && t_rotate.IsPlaying()) ||
                   isShaking;
        }
    }

    /// <summary>
    /// Gets or sets the shadow position.
    /// </summary>
    /// <value>The shadow position.</value>
    public Vector3 ShadowPosition
    {
        get;
        set;
        // todo вырезал
        /*get {
            return shadow.transform.localPosition;
        }
        set {
            shadow.transform.localPosition = value;
        }*/
    }

    /// <summary>
    /// Gets the size of card.
    /// </summary>
    /// <value>The size.</value>
    public Vector2 Size
    {
        get { return sr.sprite.bounds.size; }
    }

    /// <summary>
    /// How much layers card object occupies
    /// </summary>
    /// <value>The layer count.</value>
    public int LayerCount
    {
        get { return layerCount; }
    }

    /// <summary>
    /// Gets and sets collider height
    /// </summary>
    /// <value>The collider height.</value>
    public float ColliderH
    {
        get { return Collider.size.y; }
        set
        {
            Collider.size = new Vector2(_collider.size.x, value);
            //place collider at the top of the card
            Collider.offset = new Vector2(0f, sr.sprite.bounds.size.y / 2f - value / 2f);
        }
    }

    /// <summary>
    /// Gets the reference to collider component of this card.
    /// </summary>
    /// <value>The collider component.</value>
    public BoxCollider2D Collider
    {
        get
        {
            if (_collider == null)
            {
                _collider = GetComponent<BoxCollider2D>();
            }

            return _collider;
        }
    }

    /// <summary>
    /// Gets the shadow border size vector.
    /// </summary>
    /// <value>The shadow border size.</value>
    public Vector2 ShadowBorder
    {
        get { return shadowBorder; }
    }

    /// <summary>
    /// Gets or sets the size scale of the shadow.
    /// </summary>
    /// <value>The size of the shadow.</value>
    public Vector2 ShadowSize
    {
        get;
        set;
        // todo вырезал
        /*get {
            return new Vector2 (shadow.dimensions.x / originalShadowSize.x,
                shadow.dimensions.y / originalShadowSize.y);
        }

        set {
            shadow.dimensions = Vector2.Scale (originalShadowSize, value);
        }*/
    }

    /// <summary>
    /// Gets or sets if glow renderer enabled
    /// </summary>
    /// <value><c>true</c> if glow renderer enabled; otherwise, <c>false</c>.</value>
    public bool GlowRendererEnabled
    {
        set { glow.enabled = value; }
        get { return glow.enabled; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether sprite renderer is enabled.
    /// </summary>
    /// <value><c>true</c> if sprite enabled; otherwise, <c>false</c>.</value>
    public bool SpriteRendererEnabled
    {
        set { sr.enabled = value; }
        get { return sr.enabled; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the shadow renderer enabled.
    /// </summary>
    /// <value><c>true</c> if shadow renderer enabled; otherwise, <c>false</c>.</value>
    public bool ShadowRendererEnabled
    {
        get;
        set;
        // todo вырезал
        /*set {
            shadow.GetComponent<MeshRenderer> ().enabled = value;
        }
        get {
            return shadow.GetComponent<MeshRenderer> ().enabled;
        }*/
    }

    /// <summary>
    /// Gets or sets a value indicating whether this card is active (interactable).
    /// </summary>
    /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
    public bool Active
    {
        get { return GetComponent<BoxCollider2D>().enabled; }
        set { GetComponent<BoxCollider2D>().enabled = value; }
    }

    /// <summary>
    /// Gets or sets the layer of card object.
    /// </summary>
    /// <value>The layer index.</value>
    public int Layer
    {
        get { return gameObject.layer; }
        set
        {
            gameObject.layer = value;
            sr.gameObject.layer = value;
            glow.gameObject.layer = value;
            // todo вырезал
            // shadow.gameObject.layer = value;
        }
    }

    /// <summary>
    /// Gets or sets the sorting layer of this card object.
    /// </summary>
    /// <value>The sorting layer name.</value>
    public string SortingLayer
    {
        get => sr.sortingLayerName;
        set => sr.sortingLayerName = value;
        /*get {
            return sr.sortingLayerName;
        }
        set {
            glow.sortingLayerName = shadow.GetComponent<MeshRenderer> ().sortingLayerName = sr.sortingLayerName = value;
        }*/
    }

    /// <summary>
    /// Gets the color of the card (red or black).
    /// </summary>
    /// <value>The color of the card.</value>
    public CardColor CardColor
    {
        get { return (Suit == Suit.Clubs || Suit == Suit.Spades) ? CardColor.Black : CardColor.Red; }
    }

    /// <summary>
    /// Gets the suit of this card.
    /// </summary>
    /// <value>The suit.</value>
    public Suit Suit => (Suit)(Index / 13);

    /// <summary>
    /// Gets the value of this card.
    /// </summary>
    /// <value>The value.</value>
    public int Value
    {
        get { return Index - ((Index) / 13) * 13; }
    }

    /// <summary>
    /// Sets the hint glow sprite.
    /// </summary>
    /// <param name="spr">Sprite.</param>
    public void SetHintSprite(Sprite spr)
    {
        hintGlow = spr;
    }

    /// <summary>
    /// Sets the glow sprite which is used to highlight card when it is pressed.
    /// </summary>
    /// <param name="spr">Sprite.</param>
    public void SetGlowSprite(Sprite spr)
    {
        glowSprite = glow.sprite = spr;
    }

    /// <summary>
    /// Moves the shadow object with animation.
    /// </summary>
    /// <returns>The animation tweener object.</returns>
    /// <param name="pos">New position.</param>
    // todo вырезал
    /*public Tweener MoveShadowWithAnimation (Vector3 pos)
    {
        return shadow.transform.DOLocalMove (pos, CardAnimationSettings.SelectDuration);
    }*/

    /// <summary>
    /// Stops the hint animation.
    /// </summary>
    public void StopHintAnimation()
    {
        if (t_hint != null)
            t_hint.Complete();
    }

    /// <summary>
    /// Starts the hint animation.
    /// </summary>
    public void PlayHintAnimation()
    {
        //when hint is playing glow must be stopped
        if (t_glow != null)
        {
            t_glow.Rewind();
        }

        bool glowEnabled = glow.enabled;

        //use glow sprite renderer to show hint sprite
        glow.sprite = hintGlow;
        glow.enabled = true;

        if (t_hint == null)
        {
            t_hint = CreateHintAnimation(glow);

            //after animation is over return glow sprite renderer to its original state
            t_hint.OnComplete(() =>
            {
                glow.sprite = glowSprite;
                glow.enabled = glowEnabled;
            });

            t_hint.Play();
        }
        else
        {
            t_hint.Restart();
        }
    }

    /// <summary>
    /// Creates the hint animation.
    /// </summary>
    /// <returns>The hint animation tweener object.</returns>
    /// <param name="sr">Sprite renderer to animate.</param>
    public static Tweener CreateHintAnimation(SpriteRenderer sr)
    {
        //animate alpha value to simulate smooth blinking
        Tweener res = DOTween.ToAlpha(() => sr.color, (x) => sr.color = x, 1f, CardAnimationSettings.GlowDuration)
            .SetLoops(4, LoopType.Yoyo);
        res.SetAutoKill(false);
        return res;
    }

    /// <summary>
    /// Sets the card style sprite to this object.
    /// </summary>
    /// <param name="spr">Card style sprite.</param>
    public void SetCardStyle(Sprite spr)
    {
        //override sprite reference
        face = spr;

        //if card is face-up apply new style to it 
        if (IsUp)
        {
            sr.sprite = face;
        }

        //if card components isnt prepared yes - prepare them
        if (!prepared)
        {
            //fit collider to card size
            GetComponent<BoxCollider2D>().size = new Vector2(face.bounds.size.x, face.bounds.size.y);
            //remember original shadow size
            // todo вырезал
            //originalShadowSize = shadow.dimensions;

            //fit shadow to card size
            // todo вырезал
            /*MeshRenderer shadowRenderer = shadow.GetComponent<MeshRenderer> ();
            shadowBorder = new Vector2 ((shadowRenderer.bounds.size.x - face.bounds.size.x) / 2f,
                (shadowRenderer.bounds.size.y - face.bounds.size.y) / 2f);	*/

            prepared = true;
        }
    }

    /// <summary>
    /// Sets the card back sprite.
    /// </summary>
    /// <param name="spr">New sprite.</param>
    public void SetCardBack(Sprite spr)
    {
        //override back sprite reference
        jacket = spr;

        //if card is face down apply new sprite immediately
        if (!IsUp)
        {
            sr.sprite = jacket;
        }
    }

    /// <summary>
    /// Sets card side (face up or face down) and changes its order if specified.
    /// </summary>
    /// <param name="back">If set to <c>true</c> card will be face down, otherwise face up.</param>
    /// <param name="withAnimation">If set to <c>true</c> simulate rotation with animation.</param>
    /// <param name="delay">Animation delay.</param>
    /// <param name="newOrder">New card order.</param>
    public void SetSide(bool back, bool withAnimation = false, float delay = 0f, int newOrder = -1)
    {
        //dont use animation if its disabled in settings
        if (!GameSettings.Instance.Data.BgAnimEnabled || !gameObject.activeSelf)
            withAnimation = false;

        Action setAction = () =>
        {
            //use face or back sprite according to card new state
            sr.sprite = back ? jacket : face;

            //if new card sorting order specified - apply it 
            if (newOrder != -1)
            {
                SetSortingOrder(newOrder);
            }
        };

        //if card already in given state just invoke action
        if (!back == IsUp)
        {
            setAction();
            return;
        }

        //apply new value
        IsUp = Active = !back;

        if (!withAnimation)
            setAction();
        else
        {
            //clear animation object from previous calls
            if (t_rotate != null)
            {
                t_rotate.Rewind();
                t_rotate.Kill();
            }

            // simulate rotation of card using X scale
            t_rotate = gameObject.transform.DOScaleX(0f, CardAnimationSettings.CardRotateDuration)
                .SetLoops(2, LoopType.Yoyo);
            t_rotate.SetAutoKill(false);
            t_rotate.SetDelay(delay);
            t_rotate.Play();

            //in the middle of animation  (when sprite has zero X scale) we actually change sprite
            StartCoroutine(CoroutineExtension.ExecuteAfterTime(delay + CardAnimationSettings.CardRotateDuration,
                () => { setAction(); }));
        }
    }

    /// <summary>
    /// Stops all animation of this card.
    /// </summary>
    public void StopAllAnimation()
    {
        if (t_move != null)
        {
            t_move.Complete();
            t_move.Kill();
        }

        StopShake();

        if (t_select != null)
        {
            t_select.Rewind();
        }

        if (t_glow != null)
        {
            t_glow.Rewind();
            glow.enabled = false;
        }

        if (t_hint != null)
        {
            t_hint.Rewind();
        }
    }

    /// <summary>
    /// Sets the sorting order for this card sprite renderers.
    /// </summary>
    /// <param name="value">Value.</param>
    public void SetSortingOrder(int value)
    {
        sr.sortingOrder = value;
        // todo вырезал
        //shadow.SortingOrder = (sr.sortingOrder = value) - 2;
        glow.sortingOrder = value - 1;
    }

    /// <summary>
    /// Determines whether given rect is rect overlaps card rect.
    /// </summary>
    /// <returns><c>true</c> if given rect is rect overlaps card rect; otherwise, <c>false</c>.</returns>
    /// <param name="rect">Rect to check.</param>
    public bool IsRectOverlapsCard(Rect rect)
    {
        Rect r = new Rect(transform.position - sr.sprite.bounds.size / 2f, sr.sprite.bounds.size);
        bool res = rect.Overlaps(r);

        return res;
    }

    /// <summary>
    /// Moves card object instantly.
    /// </summary>
    /// <param name="pos">Position.</param>
    public void MoveInstantly(Vector3 pos)
    {
        SortingLayer = UILayout.SORTING_DEFAULT;
        gameObject.transform.position = pos;
    }

    /// <summary>
    /// Moves card with animation.
    /// </summary>
    /// <param name="pos">New position of card.</param>
    /// <param name="duration">Animation duration.</param>
    /// <param name="delay">Animation delay.</param>
    /// <param name="callback">Finish callback.</param>
    /// <param name="onTopLayer">If set to <c>true</c> card will be on top sorting layer during animation.</param>
    public void MoveWithAnimation(Vector3 pos, float duration = 0.1f, float delay = 0f, Action callback = null,
        bool onTopLayer = false)
    {
        // clear prev animation
        if (t_move != null && t_move.IsPlaying())
        {
            t_move.Pause();
            t_move.Kill();
        }

        //if card must be on top sorting layer, otherwise leave it in its own
        if (onTopLayer)
            SortingLayer = UILayout.SORTING_TOP;

        t_move = gameObject.transform.DOMove(pos, duration);
        t_move.SetAutoKill(true);
        t_move.SetDelay(delay);

        t_move.OnComplete(() =>
        {
            //after animation return original sorting layer and invoke callback
            SortingLayer = UILayout.SORTING_DEFAULT;
            if (callback != null)
                callback();
        });

        t_move.Play();
    }

    //int lastOrder = -1000;
    /// <summary>
    /// Plays the select animation
    /// </summary>
    public void PlaySelectAnimation()
    {
        //lastOrder = sr.sortingOrder;
        //SetSortingOrder(1000);

        float duration = CardAnimationSettings.SelectDuration;

        //if card is shaking - stop it
        StopShake();

        // if hint is showing on this card stop this animation too
        if (t_hint != null)
        {
            t_hint.Complete();
        }

        //enable glow ernderer
        glow.enabled = true;

        if (t_glow == null)
        {
            //smoothly show select glow by increasing its alpha
            t_glow = DOTween.ToAlpha(() => glow.color, (x) => glow.color = x, 1f, duration);
            t_glow.SetAutoKill(false);
        }
        else
        {
            //clear callback, which possibly was added in ReleaseAnimation method
            t_glow.OnStepComplete(null);
            t_glow.Restart();
        }

        if (t_select == null)
        {
            //slightly increase card scale to highlight it	
            t_select = gameObject.gameObject.transform.DOScale(CardAnimationSettings.SelectedScaleRate, duration);
            t_select.SetAutoKill(false);
            t_select.Play();
        }
        else
        {
            //clear callback, which possibly was added in ReleaseAnimation method	
            t_select.OnStepComplete(null);
            t_select.Restart();
        }
    }

    /// <summary>
    /// Plays animation on card release. It reversed copy of PlaySelectAnimation method
    /// </summary>
    /// <param name="endAction">End action.</param>
    public void PlayReleaseAnimation(Action endAction = null)
    {
        //this callback action will be invoked after card scale will return to normal
        if (endAction != null)
        {
            t_select.OnStepComplete(() =>
            {
                //if (lastOrder != -1000 && lastOrder < sr.sortingOrder) SetSortingOrder(lastOrder);
                endAction();
            });
        }

        //play scale animation backwards (return scale to normal)
        t_select.PlayBackwards();

        t_glow.OnStepComplete(() =>
        {
            //if (lastOrder != -1000 && lastOrder < sr.sortingOrder) SetSortingOrder(lastOrder);
            //disable glow renderer after animation complete
            glow.enabled = false;
        });

        //hide glow sprite smoothly by playing its animation backwards
        t_glow.PlayBackwards();
    }

    /// <summary>
    /// Plaies the shake animation.
    /// </summary>
    public void PlayShakeAnimation()
    {
        //if card already is shaking we must stop it first
        StopShake();

        //start shacking routine
        shake = StartCoroutine(ShakeRoutine());
    }

    /// <summary>
    /// Stops the shake animation.
    /// </summary>
    private void StopShake()
    {
        //stop coroutine
        if (shake != null)
            StopCoroutine(shake);

        //return card to original position
        if (isShaking)
            gameObject.transform.position = originalPos;

        isShaking = false;
    }

    /// <summary>
    /// Shakes the game object.
    /// </summary>
    private IEnumerator ShakeRoutine()
    {
        isShaking = true;

        float time = 0f;
        float animationLength = 1f;

        //calculate movement speed
        float speed = animationLength / CardAnimationSettings.FailShakeDuration;

        //remember original card pos to return it after animation
        originalPos = gameObject.transform.position;

        while (time < animationLength)
        {
            //calculate current card position acording to animation elapsed time
            gameObject.transform.position = originalPos +
                                            new Vector3(
                                                CardAnimationSettings.ShakeStrength *
                                                CardAnimationSettings.ShakeCurve.Evaluate(time), 0f, 0f);
            //modify elapsed time
            time += Time.deltaTime * speed;
            yield return new WaitForFixedUpdate();
        }

        isShaking = false;
    }

    #region IPointerDownHandler implementation

    public void OnPointerDown(PointerEventData eventData)
    {
        if (InAnimation)
            return;

        if (Pressed != null)
            Pressed.Invoke(new CardEventArgs(this));
    }

    #endregion

    #region IPointerUpHandler implementation

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Released != null)
            Released.Invoke(new CardEventArgs(this));
    }

    #endregion
}