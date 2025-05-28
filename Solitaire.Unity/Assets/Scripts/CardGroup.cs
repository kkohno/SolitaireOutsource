using DG.Tweening;
using Scripts.Library.Extenstions;
using System;
using System.Collections.Generic;
using ui;
using ui.windows;
using UnityEngine;

public class CardGroup
{
    /// <summary>
    /// The root card of the group. Root is the card that user holds.
    /// </summary>
    /// <value>The root card.</value>
    public Card Root { get; private set; }

    /// <summary>
    /// Gets or sets the slot this group is belong to.
    /// </summary>
    /// <value>The slot.</value>
    public Slot Slot { get; set; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="CardGroup"/> is active.
    /// </summary>
    /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
    public bool Active { get; private set; }

    /// <summary>
    /// Gets or sets the shadow Y offset in units.
    /// </summary>
    /// <value>The shadow Y offset.</value>
    public float ShadowOffset { get; set; }

    /// <summary>
    /// Gets the element list of this group.
    /// </summary>
    /// <value>The elements.</value>
    public List<Card> Elements { get; private set; }

    /// <summary>
    /// Position offset of each card in group relative to root card position
    /// </summary>
    private List<Vector3> dragOffsets;

    private Tweener t_shadow;
    private float shadowY;

    public CardGroup(Card[] c) : this(c[0])
    {
        for (int i = 1; i < c.Length; i++) {
            AddElement(c[i]);
        }
    }

    public CardGroup(Card c)
    {
        ShadowOffset = 0.1f;
        Active = true;
        Root = c;
        Elements = new List<Card>();
        dragOffsets = new List<Vector3>();
    }

    /// <summary>
    /// Counts how many cards in group are face-down or face-up
    /// </summary>
    /// <returns>The count.</returns>
    /// <param name="isUp">If set to <c>true</c> counts face-up cards, otherwise face-down cards.</param>
    private int FaceCount(bool isUp)
    {
        int res = 0;

        if (Root.IsUp == isUp)
            res++;

        foreach (var e in Elements) {
            if (e.IsUp == isUp)
                res++;
        }

        return res;
    }

    /// <summary>
    /// Gets the face down cards count.
    /// </summary>
    /// <value>The face down count.</value>
    public int FaceDownCount
    {
        get
        {
            return FaceCount(false);
        }
    }

    /// <summary>
    /// Gets the face up cards count.
    /// </summary>
    /// <value>The face up count.</value>
    public int FaceUpCount
    {
        get
        {
            return FaceCount(true);
        }
    }

    /// <summary>
    /// Gets the size of this card group.
    /// </summary>
    /// <value>The size.</value>
    public int Size
    {
        get
        {
            return Elements.Count + 1;
        }
    }

    /// <summary>
    /// Gets the Y length (height) of this group in units.
    /// </summary>
    /// <value>The Y length (height).</value>
    public float Length
    {
        get
        {
            return dragOffsets == null ? 0f : Mathf.Abs(dragOffsets[dragOffsets.Count - 1].y);
        }
    }

    /// <summary>
    /// Streches height of the shadow of root element to fit given offset.
    /// </summary>
    /// <param name="offset">Offset.</param>
    /// <param name="root">Root card reference.</param>
    public static void StrechShadowY(float offset, Card root)
    {

        float sizeY = root.Size.y + root.ShadowBorder.y * 2f;
        float scaleY = (offset + sizeY) / sizeY;

        //correcting Y pos of shadow with pivot at center according new scale
        float shadowPosY = (scaleY - 1) * (root.Size.y + root.ShadowBorder.y * 2f) / 2;

        //stretch root shadow for all group
        root.ShadowSize = new Vector3(1f, scaleY, 1f);

        //place shadow with center pivot to fit group 
        root.ShadowPosition = new Vector3(0f, -shadowPosY, 0f);
    }
    /// <summary>
    /// Adds the element to card group.
    /// </summary>
    /// <param name="c">Card.</param>
    public void AddElement(Card c)
    {
        var top = Elements.Count == 0 ? null : Elements[Elements.Count - 1];
        if (top != null && !top.IsUp) top = null;
        if (c.IsUp) SolitaireRoolsExtension.CheckStack(top, c);

        Elements.Add(c);
        dragOffsets.Add(new Vector3(0f, -ThemeManager.CardSize.y / UILayout.FACE_OFFSET_Y * Elements.Count, 0f));
    }

    /// <summary>
    /// Gets or sets the sorting layer of this group.
    /// </summary>
    /// <value>The sorting layer name.</value>
    public string Layer
    {
        get { return Root.SortingLayer; }
        set
        {

            Root.SortingLayer = value;

            for (int i = 0; i < Elements.Count; i++) {
                Elements[i].SortingLayer = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the position of this card group (its root).
    /// </summary>
    /// <value>The position of card group.</value>
    public Vector3 Position
    {
        set
        {
            Root.transform.position = value;

            for (int i = 0; i < Elements.Count; i++) {
                Elements[i].transform.position = value + dragOffsets[i];
            }
        }
        get
        {
            return Root.transform.position;
        }
    }

    /// <summary>
    /// Plays the select animation of every card of the group
    /// </summary>
    public void PlaySelectAnimation()
    {
        if (!GameSettings.Instance.Data.BgAnimEnabled)
            return;

        int count;

        if ((count = dragOffsets.Count) > 0) {
            //!!! during selection scaling our offsets will not change, so we must correct value with scale delta
            float lastOffset = Mathf.Abs(dragOffsets[count - 1].y) / CardAnimationSettings.SelectedScaleRate;
            //calculate size of shadow to fit all group
            StrechShadowY(lastOffset, Root);
            shadowY = -Root.ShadowPosition.y;
        }

        //animate shadow of group appearing
        // todo вырезал
        //t_shadow = Root.MoveShadowWithAnimation (new Vector3 (ShadowOffset, -shadowY - ShadowOffset, 0f));						
        //t_shadow.SetAutoKill (true);
        //t_shadow.Play ();


        //run selection animation for all elements and root
        Root.PlaySelectAnimation();

        foreach (var e in Elements) {
            //as we use one shadow (of the root card) we hide all the element individual shadows
            e.ShadowRendererEnabled = false;
            e.Active = false;
            e.PlaySelectAnimation();
        }
    }

    /// <summary>
    /// Plays the release animation.
    /// </summary>
    /// <param name="endAction">End action callback.</param>
    public void PlayReleaseAnimation(Action endAction = null)
    {
        Active = false;

        if (!GameSettings.Instance.Data.BgAnimEnabled)
            return;

        //to prevent this group from updating from controller during release animation we mark it as innactive
        if (t_shadow != null && t_shadow.IsPlaying()) {
            t_shadow.Rewind();
            t_shadow.Kill();
        }

        Root.PlayReleaseAnimation(endAction);
        //return regular individual shadow size
        Root.ShadowSize = Vector3.one;
        //correcting shadow position as its size changed
        Root.ShadowPosition = new Vector3(ShadowOffset, -ShadowOffset, 0f);

        //animate shadow disappearance
        // todo вырезал
        //t_shadow = Root.MoveShadowWithAnimation (Vector3.zero);
        //t_shadow.SetAutoKill (true);
        //t_shadow.Play ();

        //and animate each element 
        Tweener t_elemShadow;
        foreach (var e in Elements) {
            //place individual shadow to proper place
            e.ShadowPosition = new Vector3(ShadowOffset, -ShadowOffset, 0f);
            // todo вырезал
            //t_elemShadow = e.MoveShadowWithAnimation (Vector3.zero);
            //t_elemShadow.SetAutoKill (true);
            //t_elemShadow.Play ();
            e.PlayReleaseAnimation();
            e.ShadowRendererEnabled = true;
            e.Active = true;
        }
    }

    /// <summary>
    /// Stops all animation of card group.
    /// </summary>
    public void StopAllAnimation()
    {
        Root.StopAllAnimation();

        foreach (var e in Elements) {
            e.StopAllAnimation();
        }
    }

    /// <summary>
    /// Plays the shake animation of every card in group.
    /// </summary>
    public void PlayShakeAnimation()
    {
        Slot.RestoreGroupPos(this, Position, false);

        Root.PlayShakeAnimation();

        for (int i = 0; i < Elements.Count; i++) {
            Elements[i].PlayShakeAnimation();
        }
    }

    /// <summary>
    /// Cancel card move with shake animation
    /// </summary>
    /// <returns><c>true</c> if this instance cancel move with shake; otherwise, <c>false</c>.</returns>
    public void CancelMoveWithShake(Vector3 originalCardsPos)
    {
        //otherwise return cards to original pos and layer and play shake animation
        Position = originalCardsPos;
        Layer = UILayout.SORTING_DEFAULT;

        SoundManager.Instance.PlaySound(7, 0.4f);
        PlayShakeAnimation();
    }
}

