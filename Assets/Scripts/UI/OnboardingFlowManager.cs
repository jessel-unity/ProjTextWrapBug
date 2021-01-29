using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OnboardingFlowManager : MonoBehaviour
{
    public bool showOnboarding = true;
    public List<VisualTreeAsset> pageAssets;

    private ScrollView scrollview;
    private Button exit;
    private Button skip;
    private Button back;
    private List<VisualElement> pages;
    private VisualElement page;
    private List<VisualElement> pageIndicators;
    private int pageIndex = -1;

    // Todo: Don't show onboarding unless it has not been completed yet.
    void OnEnable()
    {
        // Get reference to nav controls
        var root = GetComponent<UIDocument>().rootVisualElement;
        scrollview = root.Q<ScrollView>();
        exit = root.Q<Button>("exit");
        skip = root.Q<Button>("nav-skip");
        back = root.Q<Button>("nav-back");

        // Initialize page indicators
        var pageIndicatorsContainer = root.Q<VisualElement>("nav-page-indicators");
        pageIndicators = new List<VisualElement>();
        pageIndicatorsContainer.hierarchy.Clear();
        for (int i = 0; i < pageAssets.Count; i++)
        {
            var indicator = new VisualElement();
            indicator.AddToClassList("page-indicator");
            pageIndicators.Add(indicator);
            pageIndicatorsContainer.Add(indicator);
        }

        // Setup button callbacks
        exit.RegisterCallback<ClickEvent>(HandleExit);
        skip.RegisterCallback<ClickEvent>(HandleNextPage);
        back.RegisterCallback<ClickEvent>(HandlePreviousPage);

        // Show first page
        pages = new List<VisualElement>();
        for (int i = 0; i < pageAssets.Count; i++)
        {
            pages.Add(pageAssets[i].CloneTree());
            pages[i].AddToClassList("container");
            scrollview.Insert(i + 1, pages[i]);
            pages[i].style.display = DisplayStyle.None;
        }
        //scrollview.contentContainer.style.flexGrow = 1;
        NextPage();
    }


    private void OnDisable()
    {
        // Destroy pageElements?

        // Destroy pageIndicators
        pageIndicators.Clear();

        // Remove button callbacks
        exit.UnregisterCallback<ClickEvent>(HandleExit);
        skip.UnregisterCallback<ClickEvent>(HandleNextPage);
        back.UnregisterCallback<ClickEvent>(HandlePreviousPage);
    }

    private void UpdatePageIndicators()
    {
        for (int i = 0; i < pageIndicators.Count; i++)
        {
            if (i == pageIndex)
            {
                pageIndicators[i].AddToClassList("page-indicator--active");
                continue;
            }

            pageIndicators[i].RemoveFromClassList("page-indicator--active");
        }
    }

    private void NextPage()
    {
        if (++pageIndex >= pageAssets.Count)
        {
            HideOnboarding();
            return;
        }
        page?.RemoveFromHierarchy();
        page = pageAssets[pageIndex].CloneTree();
        page.AddToClassList("container");
        scrollview.contentContainer.style.flexGrow = 0;
        scrollview.Insert(1, page);
        scrollview.scrollOffset = new Vector2(0f, 0f);
        scrollview.contentContainer.style.flexGrow = 1;
        scrollview.showHorizontal = false;
        UpdatePageIndicators();
    }

    private void PreviousPage()
    {
        if (pageIndex <= 0)
        {
            return;
        }
        page.RemoveFromHierarchy();
        pageIndex--;
        page = pageAssets[pageIndex].CloneTree();
        page.AddToClassList("container");
        scrollview.contentContainer.style.flexGrow = 0;
        scrollview.Insert(1, page);
        scrollview.scrollOffset = new Vector2(0f, 0f);
        scrollview.contentContainer.style.flexGrow = 1;
        scrollview.showHorizontal = false;
        UpdatePageIndicators();
    }

    private void HideOnboarding()
    {
        gameObject.SetActive(false);
    }

    private void HandleExit(ClickEvent evt)
    {
        HideOnboarding();
    }

    private void HandleNextPage(ClickEvent evt)
    {
        NextPage();
    }

    private void HandlePreviousPage(ClickEvent evt)
    {
        PreviousPage();
    }
}
