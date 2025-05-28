using System;
using System.Collections.Generic;
using System.Linq;
using ui.windows;
using UnityEngine;

/// <summary>
/// Stack of cards
/// </summary>
public class KlondikeCardStack : Slot
{
    private float maxLen;
    private float faceOffset;
    private float backOffset;
    private float maxOffset;

    public KlondikeCardStack(int index, GameObject o) : base(index, o)
    {
        baseOrder = 100;
        slotPosition = new Vector3(index * ThemeManager.CardSize.x, 0f, 0f);
        slotObject.SetActive(false);
    }

    /// <summary>
    /// Gets or sets the maximum stack lenght this stack can occupy in units
    /// </summary>
    /// <value>The length.</value>
    public float MaxLen
    {
        get { return maxLen; }
        set
        {

            float prev = maxLen;
            maxLen = value;

            if (maxLen != prev) {
                CalculateOffset();
                UpdatePosition();
            }
        }
    }

    /// <summary>
    /// Gets or sets offset between face-down cards in units
    /// </summary>
    /// <value>The back offset.</value>
    public float BackOffset
    {
        get { return backOffset; }
        set
        {
            float prev = backOffset;
            backOffset = value;

            if (backOffset != prev) {
                UpdatePosition();
            }
        }
    }

    /// <summary>
    /// Gets or sets maximmum offset between face-up cards (used if stack shorter than MaxLen) in units
    /// </summary>
    /// <value>The max offset.</value>
    public float MaxOffset
    {
        get { return maxOffset; }
        set
        {
            float prev = maxOffset;
            maxOffset = value;

            if (maxOffset != prev && prev == faceOffset) {
                faceOffset = maxOffset;
                UpdatePosition();
            }
        }
    }

    /// <summary>
    /// Updates all cards position of this stack
    /// </summary>
    private void UpdatePosition()
    {
        for (int i = 0; i < Size; i++) {
            Cards[i].MoveInstantly(CardPosition(i));
        }
    }

    /// <summary>
    /// Gets the count of face-up card
    /// </summary>
    /// <value>The face up count.</value>
    private int FaceUpCount
    {
        get { return Cards.Count((c => c.IsUp)); }
    }

    /// <summary>
    /// Gets the first face up card.
    /// </summary>
    /// <value>The first face up card.</value>
    public Card FirstFaceUp
    {
        get
        {

            for (int i = 0; i < Size; i++)
                if (Cards[i].IsUp) {
                    return Cards[i];
                }

            return null;
        }
    }

    /// <summary>
    /// Updates shadow size and position to fit cards of given count
    /// </summary>
    /// <param name="count">Card count.</param>
    public void UpdateShadow(int count = 0)
    {
        UpdateShadow(CardOffset(count));
    }

    /// <summary>
    /// Updates root shadow size and position to fit given length in units
    /// </summary>
    /// <param name="length">Length in units.</param>
    public void UpdateShadow(float length)
    {
        int size = Size;

        if (size - FaceUpCount < 1) {
            return;
        }

        Card root = Cards[0];

        if (size > 1) {
            //strech root card shadow to given offset (length)
            CardGroup.StrechShadowY(length, root);
        }
        else {
            root.ShadowSize = Vector3.one;
            root.ShadowPosition = Vector3.zero;
        }

        root.ShadowRendererEnabled = true;
    }

    /// <summary>
    /// Calculates offset of card with given index.
    /// </summary>
    /// <returns>The offset.</returns>
    /// <param name="index">Card index.</param>
    public float CardOffset(int index)
    {
        float res = 0f;

        if (index >= Size || index <= 0)
            return res;

        for (int i = 0; i < index; i++) {
            res += Cards[i].IsUp ? faceOffset : backOffset;
        }
        return res;
    }

    /// <summary>
    /// Calculates face offset between cards to fit given group in MaxLen but not bigger than specified MaxOffset.
    /// </summary>
    /// <param name="groupToAdd">Group to add.</param>
    private void CalculateOffset(CardGroup groupToAdd = null)
    {
        //if there is no enough cards to calculate offset simply use maxOffset
        if ((Size == 0 || FaceUpCount < 2) && groupToAdd == null) {
            faceOffset = maxOffset;
            return;
        }

        float newOffset = MaxOffset;

        int faceDownCount = Size - FaceUpCount;
        int groupFaceUps = (groupToAdd == null ? 0 : groupToAdd.FaceUpCount);
        int groupFaceDowns = (groupToAdd == null ? 0 : groupToAdd.FaceDownCount);

        //calculate offset to fit stack in MaxLen
        newOffset = (MaxLen - (faceDownCount + groupFaceDowns) * BackOffset) / (FaceUpCount + groupFaceUps - 1);

        //dont allow offsets bigger that maxOffset
        faceOffset = Mathf.Min(maxOffset, newOffset);
    }

    #region Slot

    /// <summary>
    /// Restores group position, if it was moved, but not deleted from slot.
    /// </summary>
    /// <param name="c">Card group.</param>
    /// <param name="pos">Position.</param>
    /// <param name="withAnimation">If set to <c>true</c> with animation.</param>
    public override void RestoreGroupPos(CardGroup c, Vector3 pos, bool withAnimation)
    {
        //dont use animation if its disabled
        if (!GameSettings.Instance.Data.BgAnimEnabled)
            withAnimation = false;

        float duration = CardAnimationSettings.CardMoveDuration;
        float delay = CardAnimationSettings.CardGroupDelay;

        //move root card to position
        if (withAnimation)
            c.Root.MoveWithAnimation(CardPosition(Cards.IndexOf(c.Root)), duration);
        else {
            c.Root.MoveInstantly(CardPosition(Cards.IndexOf(c.Root)));
        }

        //move all elements to given position
        for (int i = 0; i < c.Elements.Count; i++) {
            if (withAnimation)
                c.Elements[i].MoveWithAnimation(CardPosition(Cards.IndexOf(c.Elements[i])), duration, delay * (i + 1));
            else
                c.Elements[i].MoveInstantly(CardPosition(Cards.IndexOf(c.Elements[i])));
        }
    }

    /// <summary>
    /// Gets card position of given index.
    /// </summary>
    /// <returns>The position of card.</returns>
    /// <param name="index">Card index.</param>
    private Vector3 CardPosition(int index)
    {
        return new Vector3(slotPosition.x, slotPosition.y - CardOffset(index), 0f);
    }

    /// <summary>
    /// Stops the hint animation.
    /// </summary>
    public override void StopHintAnimation()
    {
        foreach (var c in Cards) {
            c.StopHintAnimation();
        }
        base.StopHintAnimation();
    }

    /// <summary>
    /// Plays the hint animation.
    /// </summary>
    /// <param name="c">Card object.</param>
    public override void PlayHintAnimation(CardGroup c)
    {
        if (c == null) {

            if (Head != null)
                //play hint animation on head card only
                Head.PlayHintAnimation();
            else
                //if head is null (this stack is empty) just highlight stack itself using base class method
                base.PlayHintAnimation(c);

        }
        else if (c != null) {

            //play hint animation on whole group
            if (c.Size > 1) {
                foreach (var e in c.Elements) {
                    e.PlayHintAnimation();
                }
            }

            c.Root.PlayHintAnimation();
        }
    }

    /// <summary>
    /// Gets or sets the position of this slot.
    /// </summary>
    /// <value>The position.</value>
    public override Vector3 Position
    {
        set
        {
            base.Position = value;
            UpdatePosition();
        }
    }

    /// <summary>
    /// Clear this stack from cards.
    /// </summary>
    public override void Clear()
    {
        base.Clear();
    }

    /// <summary>
    /// Gets the name of this stack.
    /// </summary>
    /// <value>The name.</value>
    public override string Name
    {
        get
        {
            return "Stack_" + index;
        }
    }

    /// <summary>
    /// Removes the card from stack.
    /// </summary>
    /// <param name="c">Card object.</param>
    public override void RemoveCard(Card c)
    {
        base.RemoveCard(c);

        if (Head != null && !Head.IsUp)
            UpdateShadow(CardOffset(Size - 2));

        OpenCard(Head);
    }

    /// <summary>
    /// Opens the card. Set ig face-up.
    /// </summary>
    /// <param name="c">Card object.</param>
    private void OpenCard(Card c)
    {
        if (c == null)
            return;

        if (!c.IsUp) {
            c.SetSide(false, true);
            c.SetSortingOrder(CardOrder(c));
            c.ShadowRendererEnabled = true;
            SoundManager.Instance.PlaySound(6, 0.5f);
        }

        c.ColliderH = ThemeManager.CardSize.y;
    }

    /// <summary>
    /// Removes the card group from this stack.
    /// </summary>
    /// <param name="cg">Card group</param>
    public override void RemoveGroup(CardGroup cg)
    {
        base.RemoveGroup(cg);

        if (CardOffset(Size - 1) <= maxLen) {
            CalculateOffset();
            UpdatePosition();
        }
    }

    /// <summary>
    /// Determines if given card can be placed on another.
    /// </summary>
    /// <returns><c>true</c>, if card can be placed, <c>false</c> otherwise.</returns>
    /// <param name="from">Card to place.</param>
    /// <param name="to">Destination card.</param>
    public static bool CheckCard(Card from, Card to)
    {
        if (from == null || to == null)
            return false;
        //check if:
        //cards are of opposite color
        //new card value is less than destination card value by one point
        //or its Ace placed on Two
        return (Card.CardsOfOppositeColor(from, to) && ((to.Value - from.Value == 1 && to.Value != 12) || (to.Value == 0 && from.Value == 12)));
    }

    /// <summary>
    /// Checks if card can be placed to this stack head.
    /// </summary>
    /// <returns><c>true</c>, if card can be placed, <c>false</c> otherwise.</returns>
    /// <param name="c">Card.</param>
    public override bool CanPlaceCard(Card c)
    {
        if (c == null)
            return false;

        return (Size == 0 && c.Value == 11) || CheckCard(c, Head);
    }

    /// <summary>
    /// Checks if card group can be placed to this stack.
    /// </summary>
    /// <returns><c>true</c>, if card group can be placed, <c>false</c> otherwise.</returns>
    /// <param name="c">Card.</param>
    public override bool CanPlaceGroup(CardGroup c)
    {
        return CanPlaceCard(c.Root);
    }

    /// <summary>
    /// Places (adds) the group to this stack.
    /// </summary>
    /// <param name="cg">Card group.</param>
    /// <param name="withAnimation">If set to <c>true</c> use animation.</param>
    public override void PlaceGroup(CardGroup cg, bool withAnimation = false)
    {
        FitSize(cg);

        AddCard(cg.Root, withAnimation);

        float delay = CardAnimationSettings.CardGroupDelay;

        if (cg.Size > 1) {
            foreach (var e in cg.Elements) {
                AddCard(e, withAnimation, delay);
                delay += CardAnimationSettings.CardGroupDelay;
            }
        }
    }

    /// <summary>
    /// Fits specified cardGroup into stack not to exceed maximum available screen length.
    /// </summary>
    /// <param name="cg">Card group.</param>
    private void FitSize(CardGroup cg)
    {
        float groupMaxSize = (cg.Size - 1) * faceOffset + ThemeManager.CardSize.y / 2f;

        //if we reached end of the screen
        if (CardOffset(Size - 1) + groupMaxSize > maxLen) {
            CalculateOffset(cg);
            UpdatePosition();
        }
    }

    /// <summary>
    /// Returns sorting order of given card.
    /// </summary>
    /// <returns>The sorting order.</returns>
    /// <param name="c">Card.</param>
    private int CardOrder(Card c)
    {
        int pos = Cards.IndexOf(c);
        int faceUpCount = FaceUpCount;

        return c.IsUp ? baseOrder + (pos - (Size - faceUpCount)) * c.LayerCount : pos * c.LayerCount;
    }

    /// <summary>
    /// Update cards sorting order depending on their index
    /// </summary>
    public override void UpdateOrder()
    {
        for (int i = 0; i < Size; i++) {
            Cards[i].SetSortingOrder(CardOrder(Cards[i]));
        }
        UpdateShadow(Size - FaceUpCount - 1);
    }

    /// <summary>
    /// Adds the card to stack.
    /// </summary>
    /// <param name="c">Card.</param>
    /// <param name="withAnimation">If set to <c>true</c> use animation.</param>
    /// <param name="delay">Animation delay.</param>
    public override void AddCard(Card c, bool withAnimation = false, float delay = 0f)
    {
        //remeber previous stack head
        Card prev = Head;

        Cards.Add(c);

        //activity of previous head is dependent of its side
        //if its became face-down it must be innactive
        if (Size > 1)
            prev.Active = prev.IsUp;

        //make sure rotation is zero
        c.transform.rotation = Quaternion.identity;

        int count = Cards.Count;

        c.SetSortingOrder(CardOrder(c));
        c.ColliderH = ThemeManager.CardSize.y;
        c.gameObject.SetActive(true);

        float cardOffset = Head.IsUp ? faceOffset : backOffset;

        if (count > 1) {
            //modify previous stack head collider height	
            prev.ColliderH = cardOffset;
        }

        //calculate last card offset realtive to root card
        float offset = CardOffset(count - 1);

        //place new card on its new position according to offset
        Vector3 newPos = new Vector3(slotPosition.x, slotPosition.y - offset, c.transform.position.z);

        //this actions will be performed after animation (if its enabled)
        Action cardArrived = () => {

            //update shadow to fit all cards in stack
            if (!c.IsUp)
                UpdateShadow(offset);

            //if new card is not alone in stack - disable its shadow
            //we use only shadow of the root to reduce draw calls
            c.ShadowRendererEnabled = c.IsUp || count == 1;
        };

        //move card to its new position with animation or not
        if (!withAnimation) {
            c.MoveInstantly(newPos);
            cardArrived();
        }
        else
            c.MoveWithAnimation(newPos, CardAnimationSettings.CardMoveDuration, delay, cardArrived, true);

    }

    /// <summary>
    /// Gets the group starting from given card.
    /// </summary>
    /// <returns>The card group.</returns>
    /// <param name="c">Card.</param>
    public override CardGroup GetGroup(Card c)
    {
        CardGroup res = base.GetGroup(c);

        int ind = Cards.IndexOf(c);

        for (int i = ind + 1; i < Size; i++) {
            res.AddElement(Cards[i]);
        }
        return res;
    }

    #endregion
}


