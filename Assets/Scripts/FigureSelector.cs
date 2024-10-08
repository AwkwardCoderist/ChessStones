using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureSelector : MonoBehaviour
{


    [SerializeField] private List<TMPro.TMP_Dropdown> player1selectors;
    [SerializeField] private List<TMPro.TMP_Dropdown> player2selectors;

    [SerializeField] private AudioClip _dropdownAudio;
    [SerializeField] [Range(0,1)] private float _dropdownAudioVolume;

    public List<FigureRolePack> figuresIds;

    [System.Serializable]
    public struct FigureRolePack
    {
        public string name;
        public List<FigureInteract> figures;
    }

    public List<List<int>> teamsSelectedFigures = new List<List<int>>();

    public List<int> debug_player1selectedIndexes;
    public List<int> debug_player2selectedIndexes;

    private void Start()
    {
        teamsSelectedFigures.Add(new List<int>());
        teamsSelectedFigures.Add(new List<int>());

        FillDropDowns(0, player1selectors, debug_player1selectedIndexes);
        FillDropDowns(1, player2selectors, debug_player1selectedIndexes);

        /*
        for (int i = 0; i < 6; i++)
        {
            int figureRole = i;
            player1selectors[i].onValueChanged.AddListener((x) => UpdateFigure(0, figureRole, x));
            player1selectors[i].options.Clear();

            for (int k = 0; k < figuresIds[i].figures.Count; k++)
            {
                player1selectors[i].options.Add(new TMPro.TMP_Dropdown.OptionData(GetLine(figuresIds[i].figures[k].figureInfo.Name)));
            }

            player1selectors[i].SetValueWithoutNotify(0);

            teamsSelectedFigures[0].Add(0);
            
            if (i < debug_player1selectedIndexes.Count) UpdateFigure(0, i, debug_player1selectedIndexes[i]);
        }

        for (int i = 0; i < 6; i++)
        {
            int figureRole = i;
            player2selectors[i].onValueChanged.AddListener((x) => UpdateFigure(1, figureRole, x));
            player2selectors[i].options.Clear();

            for (int k = 0; k < figuresIds[i].figures.Count; k++)
            {
                player2selectors[i].options.Add(new TMPro.TMP_Dropdown.OptionData(GetLine(figuresIds[i].figures[k].figureInfo.Name)));
            }

            player1selectors[i].SetValueWithoutNotify(0);

            teamsSelectedFigures[1].Add(0);
            
            if(i < debug_player2selectedIndexes.Count) UpdateFigure(1, i, debug_player2selectedIndexes[i]);
        }
        */
        

    }

    private void FillDropDowns(int team, List<TMPro.TMP_Dropdown> dropdowns, List<int> debugSelect)
    {
        for (int i = 0; i < 6; i++)
        {
            int figureRole = i;
            dropdowns[i].onValueChanged.AddListener((x) => UpdateFigure(team, figureRole, x));
            dropdowns[i].options.Clear();

            for (int k = 0; k < figuresIds[i].figures.Count; k++)
            {
                Lean.Localization.LeanLocalizedTMP_Dropdown.Option option = new Lean.Localization.LeanLocalizedTMP_Dropdown.Option();
                option.StringTranslationName = figuresIds[i].figures[k].figureInfo.Name;

                Debug.Log(dropdowns[i].GetComponent<Lean.Localization.LeanLocalizedTMP_Dropdown>());
                Debug.Log(dropdowns[i].GetComponent<Lean.Localization.LeanLocalizedTMP_Dropdown>().Options);

                dropdowns[i].GetComponent<Lean.Localization.LeanLocalizedTMP_Dropdown>().Options.Add(option);
                //dropdowns[i].options.Add(new TMPro.TMP_Dropdown.OptionData(GetLine(figuresIds[i].figures[k].figureInfo.Name)));
                dropdowns[i].onValueChanged.AddListener((x) => SoundsSystem.Instance.Play(_dropdownAudio, _dropdownAudioVolume));
            }

            dropdowns[i].SetValueWithoutNotify(0);

            teamsSelectedFigures[team].Add(0);

            if (i < debugSelect.Count) UpdateFigure(team, i, debugSelect[i]);
        }
    }

    private void UpdateFigure(int playerId, int figureRole, int newFigureId)
    {
        if (figureRole >= 6) return;

        teamsSelectedFigures[playerId][figureRole] = newFigureId;
    }
    private string GetLine(string leanPhrase)
    {
        return Lean.Localization.LeanLocalization.GetTranslationText(leanPhrase);
    }
}
