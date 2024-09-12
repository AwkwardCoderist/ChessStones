using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ModelViewer_Model
{
    public GameObject Model;
    public string Name;
    public string BattleDescription;
    public string Description;
}

public class ModelsViewer : MonoBehaviour
{
    [SerializeField] private FigureSelector _selector;
    [SerializeField] private Transform _buttonContainer;
    [SerializeField] private Transform _modelContainer;
    [SerializeField] private TMPro.TMP_Text _nameText;
    [SerializeField] private TMPro.TMP_Text _shortDescText;
    [SerializeField] private TMPro.TMP_Text _descText;
    [SerializeField] private Button _buttonPrefab;
    [SerializeField] private SingleUnityLayer _layerIndex;

    private Button _createdButton;
    private ModelViewer_Model _createdModel;
    private int _selectedIndex = -1;

    private List<ModelViewer_Model> _modelList = new List<ModelViewer_Model>();

    private void Start()
    {        
        foreach (FigureSelector.FigureRolePack pack in _selector.figuresIds)
        {
            foreach (FigureInteract figure in pack.figures)
            {
                _createdButton = Instantiate(_buttonPrefab, _buttonContainer);

                //_createdButton.GetComponentInChildren<TMPro.TMP_Text>().text = GetLine(figure.figureInfo.Name);
                _createdButton.GetComponentInChildren<Lean.Localization.LeanLocalizedTextMeshProUGUI>().TranslationName = figure.figureInfo.Name;
                int buttonIndex = _createdButton.transform.GetSiblingIndex();
                _createdButton.onClick.AddListener(() => OnSelectFigure(buttonIndex));

                _createdModel = new ModelViewer_Model();
                _createdModel.Name = figure.figureInfo.Name;
                _createdModel.BattleDescription = figure.figureInfo.BattleDescription;
                _createdModel.Description = figure.figureInfo.DetailDescription;
                _createdModel.Model = Instantiate(figure.Visual, _modelContainer);
                _createdModel.Model.SetActive(false);

                _createdModel.Model.gameObject.layer = _layerIndex.LayerIndex;

                foreach (Transform trans in _createdModel.Model.GetComponentsInChildren<Transform>())
                {
                    trans.gameObject.layer = _layerIndex.LayerIndex;
                }

                _createdModel.Model.transform.localPosition = Vector3.zero;

                _modelList.Add(_createdModel );
            }
        }

    }

    private void OnSelectFigure(int index)
    {
        if (_selectedIndex != -1) _modelList[_selectedIndex].Model.SetActive(false);
        
        _modelList[index].Model.SetActive(true);
        _nameText.text = GetLine(_modelList[index].Name);
        _shortDescText.text = GetLine(_modelList[index].BattleDescription);
        _descText.text = GetLine(_modelList[index].Description);

        _selectedIndex = index;
    }


    private string GetLine(string leanPhrase)
    {
        return Lean.Localization.LeanLocalization.GetTranslationText(leanPhrase);
    }

    private void OnEnable()
    {
        if(_selectedIndex != -1) OnSelectFigure(_selectedIndex);
    }
}
