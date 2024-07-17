using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonHandler : MonoBehaviour
{
    [Header("Creator")]
    public GameHandler Game;
    public GameObject EnviromentName;
    public GameObject MiniatureEnviroment;
    public int enviromentStatus = 0;
    public GameObject General;
    public GameObject DietSelector;
    public GameObject SizeSelector;
    public GameObject SizeVisualizer;
    public GameObject ReproSelector;
    public GameObject ReproVisualizer;
    public GameObject ColorR;
    public GameObject ColorG;
    public GameObject ColorB;
    public GameObject ColorVisualizer;
    public GameObject NumIndividuals;
    public GameObject ResumePanel;
    public GameObject resumePrefab;
    public Sprite PlainsSprite;
    public Sprite DesertSprite;
    public Sprite JungleSprite;
    public GameObject[] SpecieModels;
    public GameObject SpecieModel;
    public int speciesStatus = 0;
    [Header("InGame")]
    public GameObject LowBar;
    public GameObject LateralBar;
    public bool show = false;
    public bool showLateral = false;
    bool movingPanel = false;
    bool lateralOpened = false;
    public GameObject addPlants;
    public GameObject addWater;
    public int activeEffect = 0;
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void NextEnviroment() 
    {
        switch (enviromentStatus) 
        {
            case 0:
                enviromentStatus = 1;
                EnviromentName.GetComponent<TextMeshProUGUI>().text = "Desert";
                MiniatureEnviroment.GetComponent<Image>().sprite = DesertSprite;
                break;
            case 1:
                enviromentStatus = 2;
                EnviromentName.GetComponent<TextMeshProUGUI>().text = "Jungle";
                MiniatureEnviroment.GetComponent<Image>().sprite = JungleSprite;
                break;
            case 2:
                enviromentStatus = 0;
                EnviromentName.GetComponent<TextMeshProUGUI>().text = "Plains";
                MiniatureEnviroment.GetComponent<Image>().sprite = PlainsSprite;
                break;
        }
    }
    public void PreviousEnviroment()
    {
        switch (enviromentStatus)
        {
            case 0:
                enviromentStatus = 2;
                EnviromentName.GetComponent<TextMeshProUGUI>().text = "Jungle";
                MiniatureEnviroment.GetComponent<Image>().sprite = JungleSprite;
                break;
            case 1:
                enviromentStatus = 0;
                EnviromentName.GetComponent<TextMeshProUGUI>().text = "Plains";
                MiniatureEnviroment.GetComponent<Image>().sprite = PlainsSprite;
                break;
            case 2:
                enviromentStatus = 1;
                EnviromentName.GetComponent<TextMeshProUGUI>().text = "Desert";
                MiniatureEnviroment.GetComponent<Image>().sprite = DesertSprite;
                break;
        }
    }
    public void GoToSpeciesCreator() 
    {
        Game.Enviroment = EnviromentName.GetComponent<TextMeshProUGUI>().text;
        UpdateSizeVisualizer();
        UpdateColorVisualizer();
        StartCoroutine(MoveLeft(1f));
    }
    public void NextSpecie() 
    {
        if (speciesStatus == (SpecieModels.Length - 1))
        {
            SpecieModel.GetComponent<MeshRenderer>().sharedMaterials = SpecieModels[0].GetComponent<MeshRenderer>().sharedMaterials;
            SpecieModel.GetComponent<MeshRenderer>().materials = SpecieModels[0].GetComponent<MeshRenderer>().sharedMaterials;
            SpecieModel.GetComponent<MeshFilter>().mesh = SpecieModels[0].GetComponent<MeshFilter>().sharedMesh;
            speciesStatus = 0;
        }
        else
        {
            speciesStatus++;
            SpecieModel.GetComponent<MeshRenderer>().sharedMaterials = SpecieModels[speciesStatus].GetComponent<MeshRenderer>().sharedMaterials;
            SpecieModel.GetComponent<MeshRenderer>().materials = SpecieModels[speciesStatus].GetComponent<MeshRenderer>().sharedMaterials;
            SpecieModel.GetComponent<MeshFilter>().mesh = SpecieModels[speciesStatus].GetComponent<MeshFilter>().sharedMesh;
        }
        UpdateColorVisualizer();
    }
    public void PreviousSpecie()
    {
        if (speciesStatus == 0)
        {
            SpecieModel.GetComponent<MeshRenderer>().sharedMaterials = SpecieModels[SpecieModels.Length-1].GetComponent<MeshRenderer>().sharedMaterials;
            SpecieModel.GetComponent<MeshRenderer>().materials = SpecieModels[SpecieModels.Length - 1].GetComponent<MeshRenderer>().sharedMaterials;
            SpecieModel.GetComponent<MeshFilter>().mesh = SpecieModels[SpecieModels.Length - 1].GetComponent<MeshFilter>().sharedMesh;
            speciesStatus = SpecieModels.Length - 1;
        }
        else
        {
            speciesStatus--;
            SpecieModel.GetComponent<MeshRenderer>().sharedMaterials = SpecieModels[speciesStatus].GetComponent<MeshRenderer>().sharedMaterials;
            SpecieModel.GetComponent<MeshRenderer>().materials = SpecieModels[speciesStatus].GetComponent<MeshRenderer>().sharedMaterials;
            SpecieModel.GetComponent<MeshFilter>().mesh = SpecieModels[speciesStatus].GetComponent<MeshFilter>().sharedMesh;
        }
        UpdateColorVisualizer();
    }
    public void UpdateSizeVisualizer() 
    {
        SizeVisualizer.GetComponent<TextMeshProUGUI>().text = SizeSelector.GetComponent<Slider>().value.ToString("F2");
    }
    public void UpdateReproVisualizer()
    {
        ReproVisualizer.GetComponent<TextMeshProUGUI>().text = ReproSelector.GetComponent<Slider>().value.ToString();
    }
    public void UpdateColorVisualizer() 
    {
        ColorVisualizer.GetComponent<Image>().color = new Color(ColorR.GetComponent<Slider>().value, ColorG.GetComponent<Slider>().value, ColorB.GetComponent<Slider>().value);
        SpecieModel.GetComponent<MeshRenderer>().materials[0].color = ColorVisualizer.GetComponent<Image>().color;
    }
    public void AddSpecie() 
    {
        Game.ApplyDNA(DietSelector.GetComponent<TMP_Dropdown>().value, SizeSelector.GetComponent<Slider>().value, (int)ReproSelector.GetComponent<Slider>().value, ColorVisualizer.GetComponent<Image>().color, SpecieModels[speciesStatus], int.Parse(NumIndividuals.GetComponent<TMP_InputField>().text));
        GameObject newSpecie = Instantiate(resumePrefab, ResumePanel.transform);
        newSpecie.GetComponent<RectTransform>().anchoredPosition = new Vector2(newSpecie.GetComponent<RectTransform>().anchoredPosition.x, newSpecie.GetComponent<RectTransform>().anchoredPosition.y - 125 * (Game.SpeciesDNA.Count - 1));
        newSpecie.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = DietSelector.GetComponent<TMP_Dropdown>().options[DietSelector.GetComponent<TMP_Dropdown>().value].text + "\n" + "Size: " + SizeSelector.GetComponent<Slider>().value.ToString("F2") + "\n" + "Rep Compatibility: " + ReproSelector.GetComponent<Slider>().value.ToString()+"\n"+"Individuals: "+ NumIndividuals.GetComponent<TMP_InputField>().text;
    }
    public void GoToMap() 
    {
        SceneManager.LoadScene(enviromentStatus+2);
    }
    void UpdateDataPanel(Individuo selected)
    {
        switch (selected.adn.diet) 
        {
            case 0:
                LateralBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Diet: Herbivore";
                break;
            case 1:
                LateralBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Diet: Carnivore";
                break;
        }
        LateralBar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Reproduction Compatibility: " + selected.adn.reproductionCompatibility.ToString();
    }
    public void SelectEffect(int effect) 
    {
        if (effect != activeEffect)
        {
            activeEffect = effect;
        }
        else
        {
            activeEffect = 0;
        }
        switch (effect)
        {
            case 0:
                addPlants.GetComponent<Image>().color = Color.white;
                addWater.GetComponent<Image>().color = Color.white;
                break;
            case 1:
                addPlants.GetComponent<Image>().color = Color.green;
                addWater.GetComponent<Image>().color = Color.white;
                break;
            case 2:
                addPlants.GetComponent<Image>().color = Color.white;
                addWater.GetComponent<Image>().color = Color.green;
                break;
        }
    }
    public void ChangeShowLateralBar(bool touchedCriter, Individuo selected)
    {
        if (touchedCriter)
        {
            UpdateDataPanel(selected);
            if (!lateralOpened) {
                if (!movingPanel)
                {
                    lateralOpened = true;
                    movingPanel = true;
                    StartCoroutine(LerpLateralBar(1f));
                    showLateral = !showLateral;
                }
            }
        }
        else 
        {
            if (lateralOpened) 
            {
                if (!movingPanel)
                {
                    lateralOpened = false;
                    movingPanel = true;
                    StartCoroutine(LerpLateralBar(1f));
                    showLateral = !showLateral;
                }
            }
        }
    }
    public void ChangeShowLowBar() 
    {
        if (!movingPanel)
        {
            movingPanel = true;
            StartCoroutine(LerpLowBar(1f));
            show = !show;
        }
    }
    IEnumerator MoveLeft(float duration)
    {
        float time = 0;
        Vector2 startPosition = General.GetComponent<RectTransform>().anchoredPosition;
        Vector2 targetPosition = new Vector2(-1900f, startPosition.y);

        while (time < duration)
        {
            General.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        General.GetComponent<RectTransform>().anchoredPosition = targetPosition;
    }
    IEnumerator LerpLowBar(float duration)
    {
        float time = 0;
        Vector2 startPosition = LowBar.GetComponent<RectTransform>().anchoredPosition;
        Vector2 targetPosition = new Vector2(startPosition.x, -startPosition.y);

        while (time < duration)
        {
            LowBar.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        LowBar.GetComponent<RectTransform>().anchoredPosition = targetPosition;
        movingPanel = false;
    }
    IEnumerator LerpLateralBar(float duration)
    {
        float time = 0;
        Vector2 startPosition = LateralBar.GetComponent<RectTransform>().anchoredPosition;
        Vector2 targetPosition = new Vector2(-startPosition.x, startPosition.y);

        while (time < duration)
        {
            LateralBar.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        LateralBar.GetComponent<RectTransform>().anchoredPosition = targetPosition;
        movingPanel = false;
    }
}
