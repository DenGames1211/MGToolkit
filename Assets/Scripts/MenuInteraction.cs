using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MicroGestureToggleNavigator : MonoBehaviour
{
    [Header("Meta XR")]
    public OVRHand hand;

    [Header("Targets")]
    public bool autoFindToggles = true;
    public bool autoRefresh = true;
    [Tooltip("Seconds between automatic rescans.")]
    public float refreshInterval = 0.5f;

    [Header("Highlight")]
    public Color highlightColor = Color.yellow;
    public bool useOutline = false;
    public float gestureCooldown = 0.35f;

    private List<Toggle> _toggles = new();
    private int _currentIndex = -1;
    private float _lastGestureTime;

    private readonly Dictionary<Graphic, Color> _origColors = new();
    private readonly Dictionary<Toggle, Outline> _outlines = new();
    private HashSet<int> _toggleIdCache = new();

    Coroutine _refreshRoutine;

    [Header("ScrollRect")]
    public ScrollRect activeScrollRect;          // assign or auto-find
    [Range(0.01f, 0.5f)]
    public float scrollStep = 0.12f;             // how much to scroll per swipe
    public bool autoFindScrollRect = true;

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        Canvas.willRenderCanvases += OnCanvasWillRenderCanvases;

        // Ensure something is highlighted when this object gets enabled again
        EnsureFirstHighlighted();
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
        Canvas.willRenderCanvases -= OnCanvasWillRenderCanvases;
        if (_refreshRoutine != null) StopCoroutine(_refreshRoutine);
    }

   void Start()
    {
        RefreshToggles(true);
        if (autoRefresh)
            _refreshRoutine = StartCoroutine(AutoRefresh());

        if (autoFindScrollRect) FindActiveScrollRect();

        EnsureFirstHighlighted();
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        RefreshToggles(false);
        if (autoFindScrollRect) FindActiveScrollRect();
        EnsureFirstHighlighted();
    }

    void OnCanvasWillRenderCanvases()
    {
        if (autoRefresh && autoFindToggles && DetectToggleSetChanged())
        {
            RefreshToggles(false);
            EnsureFirstHighlighted();
        }

        if (autoFindScrollRect) FindActiveScrollRect();
    }

    void Update()
    {
        if (hand == null) return;

        var gesture = hand.GetMicrogestureType();
        if (gesture == OVRHand.MicrogestureType.NoGesture) return;
        if (Time.time - _lastGestureTime < gestureCooldown) return;
        _lastGestureTime = Time.time;

        switch (gesture)
        {
            case OVRHand.MicrogestureType.SwipeRight:
                Next();
                break;
            case OVRHand.MicrogestureType.SwipeForward:
                TryScroll(true);
                break;
            case OVRHand.MicrogestureType.SwipeLeft:
                Previous();
                break;
            case OVRHand.MicrogestureType.SwipeBackward:
                TryScroll(false);
                break;
            case OVRHand.MicrogestureType.ThumbTap:
                ToggleCurrent();
                break;
        }
    }

    // ------------- Scrolling -------------
    bool TryScroll(bool forward)
    {
        if (activeScrollRect == null || !activeScrollRect.enabled || !activeScrollRect.gameObject.activeInHierarchy)
            return false;

        // Vertical first (adjust sign so "forward" feels like moving content up)
        if (activeScrollRect.vertical)
        {
            // Unity: 1 = top, 0 = bottom. Swiping "forward" often means down the list visually.
            float delta = forward ? -scrollStep : scrollStep;
            var v = Mathf.Clamp01(activeScrollRect.verticalNormalizedPosition + delta);
            activeScrollRect.verticalNormalizedPosition = v;
            activeScrollRect.velocity = Vector2.zero; // stop inertia fighting you
            return true;
        }

        // Horizontal fallback
        if (activeScrollRect.horizontal)
        {
            float delta = forward ? scrollStep : -scrollStep;
            var h = Mathf.Clamp01(activeScrollRect.horizontalNormalizedPosition + delta);
            activeScrollRect.horizontalNormalizedPosition = h;
            activeScrollRect.velocity = Vector2.zero;
            return true;
        }

        return false;
    }

    void FindActiveScrollRect()
    {
        // Simple heuristic: first enabled ScrollRect under the active canvas
        Canvas activeCanvas = Canvas.GetDefaultCanvasMaterial() != null
            ? FindObjectsByType<Canvas>(FindObjectsSortMode.None).FirstOrDefault(c => c.isActiveAndEnabled)
            : null;

        if (activeCanvas == null) return;

        activeScrollRect = activeCanvas.GetComponentsInChildren<ScrollRect>(true)
            .FirstOrDefault(sr => sr.enabled && sr.gameObject.activeInHierarchy);
    }

    // -------- Navigation --------
    void Next()
    {
        if (_toggles.Count == 0) return;
        FocusIndex((_currentIndex + 1) % _toggles.Count);
    }

    void Previous()
    {
        if (_toggles.Count == 0) return;
        FocusIndex((_currentIndex - 1 + _toggles.Count) % _toggles.Count);
    }

    void ToggleCurrent()
    {
        if (_currentIndex < 0 || _currentIndex >= _toggles.Count) return;
        var t = _toggles[_currentIndex];
        t.isOn = !t.isOn;
        // t.onValueChanged.Invoke(t.isOn); // if you need manual invoke
    }

    // -------- Highlight --------
    void FocusIndex(int index)
    {
        if (index == _currentIndex) return;

        if (_currentIndex >= 0 && _currentIndex < _toggles.Count)
            SetHighlight(_toggles[_currentIndex], false);

        _currentIndex = Mathf.Clamp(index, 0, _toggles.Count - 1);

        SetHighlight(_toggles[_currentIndex], true);
    }

    void SetHighlight(Toggle t, bool on)
    {
        if (t == null) return;

        var g = t.targetGraphic;

        if (!useOutline)
        {
            if (g != null)
            {
                if (on)
                {
                    if (!_origColors.ContainsKey(g))
                        _origColors[g] = g.color;
                    g.color = highlightColor;
                }
                else
                {
                    if (_origColors.TryGetValue(g, out var orig))
                        g.color = orig;
                }
            }
        }
        else
        {
            if (on)
            {
                if (!_outlines.TryGetValue(t, out var outline))
                {
                    outline = t.gameObject.AddComponent<Outline>();
                    _outlines[t] = outline;
                }
                outline.enabled = true;
                outline.effectDistance = new Vector2(4, 4);
            }
            else
            {
                if (_outlines.TryGetValue(t, out var outline))
                    outline.enabled = false;
            }
        }
    }

    // -------- Auto Refresh --------
    IEnumerator AutoRefresh()
    {
        var wait = new WaitForSeconds(refreshInterval);
        while (true)
        {
            yield return wait;
            if (autoFindToggles && DetectToggleSetChanged())
                RefreshToggles(false);
        }
    }

    bool DetectToggleSetChanged()
    {
        var current = GetAllInteractableToggles();
        if (current.Count != _toggleIdCache.Count) return true;

        foreach (var t in current)
            if (!_toggleIdCache.Contains(t.GetInstanceID()))
                return true;

        return false;
    }

    void RefreshToggles(bool firstTime)
    {
        if (!autoFindToggles)
            return;

        var newList = GetAllInteractableToggles();

        // Preserve focus if possible
        Toggle focused = null;
        if (!firstTime && _currentIndex >= 0 && _currentIndex < _toggles.Count)
            focused = _toggles[_currentIndex];

        // Clear old highlight
        if (_currentIndex >= 0 && _currentIndex < _toggles.Count)
            SetHighlight(_toggles[_currentIndex], false);

        _toggles = newList;

        _toggleIdCache.Clear();
        foreach (var t in _toggles)
            _toggleIdCache.Add(t.GetInstanceID());

        if (_toggles.Count == 0)
        {
            _currentIndex = -1;
            return;
        }

        // Restore focus or fall back to first
        int newIndex = focused != null ? _toggles.IndexOf(focused) : 0;
        if (newIndex < 0) newIndex = 0;

        FocusIndex(newIndex);

        // Guarantee first highlighted if something went wrong
        EnsureFirstHighlighted();
    }

    List<Toggle> GetAllInteractableToggles()
    {
        // Choose a root to respect UI order. If you don’t set one, use ALL scene roots.
        // You can expose a Transform searchRoot if you want to limit the scope.
        var toggles = new List<Toggle>();

        // Depth-first traversal keeps the same order you see in the Hierarchy
        foreach (var go in gameObject.scene.GetRootGameObjects())
            DFS(go.transform, toggles);

        return toggles;
    }

    void DFS(Transform t, List<Toggle> list)
    {
        // Add toggle on this node first (pre-order)
        var toggle = t.GetComponent<Toggle>();
        if (toggle && toggle.interactable && toggle.gameObject.activeInHierarchy)
            list.Add(toggle);

        // Then visit children in sibling order
        for (int i = 0; i < t.childCount; i++)
            DFS(t.GetChild(i), list);
    }

    // -------- Helper --------
    void EnsureFirstHighlighted()
    {
        if (_toggles.Count == 0) return;

        if (_currentIndex < 0 || _currentIndex >= _toggles.Count)
        {
            FocusIndex(0);
            return;
        }

        // If somehow not highlighted, re-apply
        SetHighlight(_toggles[_currentIndex], true);
    }
}