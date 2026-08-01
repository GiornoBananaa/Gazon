using System.Collections.Generic;
using DG.Tweening;
using Game.Runtime.MusicInstrumentSystem;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Runtime.MusicInstrumentFeature.Animations
{
    public class HandsInstrumentAnimation : MonoBehaviour
    {
        private class HandNode
        {
            public int Start;
            public int End;
            public float MaxFingerLength;
            public float MinFingerLength;
            public Vector3 AveragePosition;
            public readonly List<int> Keys = new List<int>();
        }
        
        [SerializeField] private HandRig _leftPrefab;
        [SerializeField] private HandRig _rightPrefab;
        [SerializeField] private int _fingerCount = 5;
        [SerializeField] private float _maxDistanceOnOneHand = 0.4f;
        [SerializeField] private float _showAnimationSpeed = 0.8f;
        [SerializeField] private PianoKeysAnimation _instrumentKeysAnimation;
        
        private readonly List<int> _pressedKeys = new();
        private readonly Dictionary<HandRig, Tween> _tweens = new();
        private List<HandRig> _hands = new();
        private List<HandRig> _freeHands = new();
        private ObjectPool<HandRig> _leftHandsPool;
        private ObjectPool<HandRig> _rightHandsPool;
        private IEnumerable<IInstrumentKeyPresser> _instrumentKeyPressers;
        private NotesPlayer _notesPlayer;
        private bool _updateHands;
        private int _nodeStartRig;
        private int _createdRightHands;
        private int _createdLeftHands;

        [Inject]
        public void Construct(IEnumerable<IInstrumentKeyPresser> instrumentKeyPressers, NotesPlayer notesPlayer)
        {
            UnsubscribeEvents();
            _instrumentKeyPressers = instrumentKeyPressers;
            _notesPlayer = notesPlayer;
        }
        
        private void Awake()
        {
            _rightHandsPool = new(CreateRightHand);
            _leftHandsPool = new(CreateLeftHand);
        }

        private void Update()
        {
            if(_updateHands)
            {
                _updateHands = false;
                UpdateHands();
            }
        }

        public void Enable()
        {
            SubscribeEvents();
        }
        
        public void Disable()
        {
            UnsubscribeEvents();
            HideAll();
            foreach (var hand in _freeHands)
            {
                if(hand.IsRight)
                    _rightHandsPool.Release(hand);
                else
                    _leftHandsPool.Release(hand);
            }
            _freeHands.Clear();
        }
        
        private void OnPressedKeyNote(int keyIndex, int noteIndex)
        {
            OnPressedKeyRhythm(noteIndex);
        }
        
        private void OnReleasedKeyNote(int keyIndex, int noteIndex)
        {
            OnReleasedKeyRhythm(noteIndex);
        }
        
        private void OnPressedKeyRhythm(int note)
        {
            bool inserted = false;
            for (int i = 0; i < _pressedKeys.Count; i++)
            {
                if(_pressedKeys[i] < note) continue;
                _pressedKeys.Insert(i, note);
                inserted = true;
                break;
            }
            if(!inserted)
                _pressedKeys.Add(note);
            
            _updateHands = true;
        }

        private void OnReleasedKeyRhythm(int note)
        {
            _pressedKeys.Remove(note);
            _updateHands = true;
        }

        private void UpdateHands()
        {
            List<HandNode> handNodes = new();
            List<HandRig> hands = new();
            int index = 0;
            int lastNotFullHand = 0;

            while (index < _pressedKeys.Count)
            {
                var handNode = new HandNode();
                handNodes.Add(handNode);
                
                for (int i = index; i < _pressedKeys.Count; i++)
                {
                    Vector3 pressPosition = _instrumentKeysAnimation.GetPressPosition(_pressedKeys[i]);
                    if (handNode.Keys.Count >= _fingerCount 
                        || (handNode.Keys.Count > 0 
                            && Vector3.Distance(pressPosition, _instrumentKeysAnimation.GetPressPosition(handNode.Keys[0])) > _maxDistanceOnOneHand)) break;
                    handNode.Keys.Add(_pressedKeys[i]);
                    index++;
                }
                if(handNode.Keys.Count < _fingerCount)
                    lastNotFullHand = index-1;
                handNode.Start = handNode.Keys[0];
                handNode.End = handNode.Keys[^1];
                handNode.AveragePosition = (_instrumentKeysAnimation.GetPressPosition(handNode.Start) + _instrumentKeysAnimation.GetPressPosition(handNode.End)) / 2f;
            }

            lastNotFullHand = Mathf.Clamp(lastNotFullHand, 0, handNodes.Count - 1);
            
            for (int i = lastNotFullHand; i > 0; i--)
            {
                if(handNodes[i].Keys.Count >= _fingerCount) continue;
                Vector3 middle = (_instrumentKeysAnimation.GetPressPosition(handNodes[i-1].Start) + _instrumentKeysAnimation.GetPressPosition(handNodes[i].End)) / 2f;
                bool modified = false;
                for (int j = handNodes[i-1].Keys.Count - 1; j > 0 ; j--)
                {
                    if(handNodes[i].Keys.Count >= _fingerCount
                       || _instrumentKeysAnimation.GetPressPosition(handNodes[i-1].Keys[j]).x < middle.x
                       || Vector3.Distance(_instrumentKeysAnimation.GetPressPosition(handNodes[i].Keys[^1]), _instrumentKeysAnimation.GetPressPosition(handNodes[i-1].Keys[j])) > _maxDistanceOnOneHand) break;
                    
                    handNodes[i].Keys.Add(handNodes[i-1].Keys[j]);
                    handNodes[i-1].Keys.RemoveAt(j);
                    modified = true;
                }
                
                if (modified)
                {
                    handNodes[i].Start = handNodes[i].Keys[0];
                    handNodes[i].End = handNodes[i].Keys[^1];
                    handNodes[i].AveragePosition = (_instrumentKeysAnimation.GetPressPosition(handNodes[i].Start) + _instrumentKeysAnimation.GetPressPosition(handNodes[i].End)) / 2f;
                    
                    handNodes[i-1].Start = handNodes[i-1].Keys[0];
                    handNodes[i-1].End = handNodes[i-1].Keys[^1];
                    handNodes[i-1].AveragePosition = (_instrumentKeysAnimation.GetPressPosition(handNodes[i-1].Start) + _instrumentKeysAnimation.GetPressPosition(handNodes[i-1].End)) / 2f;
                }
            }
            
            int startNodeIndex = 0;
            int startHandIndex = 0;
            
            if(_hands.Count != 0 && handNodes.Count != 0)
            {
                if (_hands.Count < handNodes.Count)
                {
                    if (_hands.Count == 1)
                        startNodeIndex = _hands[0].IsRight ? handNodes.Count - 1 : handNodes.Count - 2;
                    else
                        startNodeIndex = handNodes.Count - _hands.Count;
                }
                else if (_hands.Count > handNodes.Count)
                {
                    if (handNodes.Count == 1)
                        startHandIndex = handNodes[0].AveragePosition.x > transform.position.x ? _hands.Count - 1 : _hands.Count - 2;
                    else
                    {
                        startHandIndex = Mathf.Abs(handNodes.Count - _hands.Count);
                        /*
                        for (int i = 0; i < Mathf.Abs(handNodes.Count - _hands.Count); i++)
                        {


                            if (handNodes[0].AveragePosition.x - _hands[startHandIndex].transform.position.x >
                                handNodes[^1].AveragePosition.x - _hands[startHandIndex + handNodes.Count].transform.position.x)
                                startHandIndex++;
                        }
                        */
                    }
                }
            }
            
            if (_hands.Count < handNodes.Count)
            {
                if (_hands.Count == 0)
                {
                    for (int i = 0; i < handNodes.Count; i++)
                    {
                        bool right = ((handNodes.Count - 1) - i) % 2 == 0;
                        if (handNodes.Count == 1)
                            right = handNodes[i].AveragePosition.x > transform.position.x;
                        hands.Add(GetNewHand(right, handNodes[i]));
                    }
                }
                else
                {
                    for (int i = 0; i < startNodeIndex; i++)
                    {
                        bool right = _hands[0].IsRight == (startNodeIndex % 2 == 0) == (i % 2 == 0);
                        hands.Add(GetNewHand(right, handNodes[i]));
                    }
                    for (int i = 0; i < _hands.Count; i++)
                    {
                        hands.Add(_hands[i]);
                    }
                    for (int i = startNodeIndex + _hands.Count; i < handNodes.Count; i++)
                    {
                        bool right = _hands[0].IsRight == (startNodeIndex % 2 == 0) == (i % 2 == 0);
                        hands.Add(GetNewHand(right, handNodes[i]));
                    }
                }
            }
            else if (_hands.Count > handNodes.Count)
            {
                for (int i = startHandIndex; i < startHandIndex + handNodes.Count; i++)
                {
                    var hand = _hands[i];
                    hands.Add(hand);
                }
            }
            else if (_hands.Count == handNodes.Count)
            {
                for (int i = 0; i < _hands.Count; i++)
                {
                    hands.Add(_hands[i]);
                }
            }
            for (int i = 0; i < handNodes.Count; i++)
            {
                float minLength = float.MaxValue;
                float maxLength = 0;
                for (int j = 0; j < _fingerCount; j++)
                {
                    float length = hands[i].GetMaxLength(j);
                    if(length < minLength)
                        minLength = length;
                    if(length > maxLength)
                        maxLength = length;
                }
                handNodes[i].MaxFingerLength = maxLength;
                handNodes[i].MinFingerLength = minLength;
            }
            
            for (int handIndex = 0; handIndex < handNodes.Count; handIndex++)
            {
                var handNode = handNodes[handIndex];
                var handRig = hands[handIndex];
                
                bool rightHand = handRig.IsRight;
                int finger = rightHand ? 0 : _fingerCount - 1;
                
                if (handNode.Keys.Count == 1)
                {
                    handRig.Release(finger);
                    if(rightHand)
                        finger++;
                    else
                        finger--;
                }
                int lastFinger = finger;
                bool first = true;
                Vector3 lastFingerPosition = Vector3.zero;
                for (int i = rightHand ? 0 : handNode.Keys.Count - 1; rightHand ? i < handNode.Keys.Count : i >= 0;)
                {
                    if(finger < 0 || finger >= _fingerCount)
                    {
                        finger = rightHand ? _fingerCount - 1 : 0;
                        break;
                    }
                    Vector3 pressPosition = _instrumentKeysAnimation.GetPressPosition(handNode.Keys[i]);
                    float distanceToLast = Vector3.Distance(lastFingerPosition, pressPosition);
                    while (!first && finger > 0 && finger < _fingerCount-1 && distanceToLast > handRig.GetMaxDistance(lastFinger, finger))
                    {
                        handRig.Release(finger);
                        if (rightHand) finger++;
                        else finger--;
                    }
                    if(finger < 0 || finger >= _fingerCount) 
                    { 
                        finger = rightHand ? _fingerCount - 1 : 0;
                        break;
                    }
                    pressPosition += transform.forward.normalized * ((handNode.MaxFingerLength - handNode.MinFingerLength) * Mathf.InverseLerp(handNode.MinFingerLength, handNode.MaxFingerLength, handRig.GetMaxLength(finger)));
                    
                    handRig.SetPosition(finger, pressPosition);
                    
                    lastFingerPosition = pressPosition;
                    lastFinger = finger;
                    first = false;
                    if (rightHand)
                    {
                        finger++;
                        i++;
                    }
                    else
                    {
                        finger--;
                        i--;
                    }
                }
                for (int i = finger; i >= 0 && i < _fingerCount;)
                {
                    handRig.Release(i);
                    if (rightHand) i++;
                    else i--;
                }
            }
            
            if(hands.Count < _hands.Count)
            {
                for (int i = 0; i < _hands.Count; i++)
                {
                    if (i >= startHandIndex && i < startHandIndex + hands.Count)
                    {
                        continue;
                    }
                    
                    _hands[i].ReleaseAll();
                    if(handNodes.Count < 2 && i >= _hands.Count - 2)
                    {
                        _freeHands.Add(_hands[i]);
                    }
                    else
                    {
                        HideHand(_hands[i]);
                        if (_hands[i].IsRight)
                            _rightHandsPool.Release(_hands[i]);
                        else
                            _leftHandsPool.Release(_hands[i]);
                    }
                }
            }

            if (_freeHands.Count > 0 && handNodes.Count > 0)
            {
                for (int i = 0; i < _freeHands.Count; i++)
                {
                    var hand = _freeHands[i];
                    float offset = hand.transform.position.x - handNodes[0].AveragePosition.x;
                    if ((hand.IsRight && offset < 0.12f) || (!hand.IsRight && offset > -0.12f))
                    {
                        hand.SetRelaxedPosition(
                            new Vector3(handNodes[0].AveragePosition.x + (hand.IsRight ? 0.12f : -0.12f),
                                hand.transform.position.y, Mathf.Min(hand.transform.position.z, handNodes[0].AveragePosition.z - 0.3f)));
                    }
                }
            }
            _hands = hands;
        }

        private HandRig GetNewHand(bool right, HandNode node)
        {
            for (int i = 0; i < _freeHands.Count; i++)
            {
                var freeHand = _freeHands[i];
                if(freeHand.IsRight != right) continue;
                
                _freeHands.RemoveAt(i);
                return freeHand;
            }
            var hand = right ? _rightHandsPool.Get() : _leftHandsPool.Get();
            ShowHand(hand, node);
            return hand;
        }
        
        private HandRig CreateLeftHand()
        {
            HandRig handRig = Instantiate(_leftPrefab, transform);
            handRig.gameObject.name = "LeftHand " + _createdLeftHands;
            _createdLeftHands++;
            return handRig;
        }
        
        private HandRig CreateRightHand()
        {
            HandRig handRig = Instantiate(_rightPrefab, transform);
            handRig.gameObject.name = "RightHand " + _createdRightHands;
            _createdRightHands++;
            return handRig;
        }
        
        private void ShowHand(HandRig handRig, HandNode handNode)
        {
            if(_tweens.TryGetValue(handRig, out Tween tween))
                tween?.Kill();
            handRig.KillTween();
            handRig.transform.position = handNode.AveragePosition + new Vector3(0, 0.05f, -0.3f);
            handRig.gameObject.SetActive(true);
            handRig.SetTransparency(1, 0.5f);
        }
        
        private void HideHand(HandRig handRig)
        {
            if(_tweens.TryGetValue(handRig, out Tween tween))
                tween?.Kill();
            handRig.KillTween();
            Vector3 offset = new Vector3(0, 0, -0.3f);
            float duration = offset.magnitude / _showAnimationSpeed;
            _tweens[handRig] = handRig.transform.DOMove(handRig.transform.position + offset, duration)
                .OnComplete(()=>handRig.gameObject.SetActive(false));
            handRig.SetTransparency(0, duration);
        }

        private void HideAll()
        {
            foreach (var hand in _freeHands)
            {
                HideHand(hand);
            }
            foreach (var hand in _hands)
            {
                HideHand(hand);
            }
        }
        
        private void UnsubscribeEvents()
        {
            if(_instrumentKeyPressers != null)
            {
                foreach (var keyPresser in _instrumentKeyPressers)
                {
                    keyPresser.OnPressedKeyNoteIndexes -= OnPressedKeyNote;
                    keyPresser.OnReleasedKeyNoteIndexes -= OnReleasedKeyNote;
                }
            }
            if (_notesPlayer != null)
            {
                _notesPlayer.OnNoteStart -= OnPressedKeyRhythm;
                _notesPlayer.OnNoteEnd -= OnReleasedKeyRhythm;
            }
        }
        
        private void SubscribeEvents()
        {
            if(_notesPlayer != null)
            {
                _notesPlayer.OnNoteStart += OnPressedKeyRhythm;
                _notesPlayer.OnNoteEnd += OnReleasedKeyRhythm;
                _notesPlayer.OnCompleted += HideAll;
            }
            if(_instrumentKeyPressers != null)
            {
                foreach (var keyPresser in _instrumentKeyPressers)
                {
                    keyPresser.OnPressedKeyNoteIndexes += OnPressedKeyNote;
                    keyPresser.OnReleasedKeyNoteIndexes += OnReleasedKeyNote;
                }
            }
        }
    }
}